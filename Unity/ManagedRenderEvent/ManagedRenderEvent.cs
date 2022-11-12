using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

public delegate void RenderPluginDelegate(int eventId, IntPtr data);

public static class ManagedRenderEvent
{
    private static HashSet<RenderPluginDelegate> delegateHolder = new HashSet<RenderPluginDelegate>();
    private const string _pluginName = "ManagedRenderEvent";
    
#if UNITY_EDITOR
    private const int MaxCallsPerFrame = 8192;
    private static IntPtr renderCallbackPtr;
    
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeCallbackData{
        internal IntPtr runtimeMethodPointer;
        internal int eventId;
        internal int __unused;
        internal IntPtr addData;
    };

    private static NativeCallbackData[] callbackDatas = new NativeCallbackData[MaxCallsPerFrame];
    private static long _currentData = 0;
    private static ulong gcHandle;
    private static unsafe NativeCallbackData* basePtr;
    private static bool _initialized = false;
    
    private static unsafe NativeCallbackData* GetData(in NativeCallbackData data)
    {
        var prev = _currentData;
        _currentData = (_currentData + 1) % callbackDatas.Length;
        
        var ptr = basePtr + prev;
        *ptr = data;
        return ptr;
    }
    
    [DllImport("__Internal")]
    public static extern unsafe void* mono_thread_attach(void *domain);

    [DllImport("__Internal")]
    public static extern unsafe void* mono_get_root_domain();
    private unsafe delegate void* MonoAttachThreadDelegate(void*domain);
    
    [DllImport(_pluginName)]
    private static extern unsafe void ManagedRenderEvent_SetMonoData(MonoAttachThreadDelegate thread_attach, void* domain);
    
    [DllImport(_pluginName)]
    private static extern IntPtr ManagedRenderEvent_GetCallback();

    private static RenderPluginDelegate renderCallbackDelegate;
    
    static unsafe ManagedRenderEvent()
    {
        basePtr = (NativeCallbackData*)UnsafeUtility.PinGCArrayAndGetDataAddress(callbackDatas, out gcHandle);
        _currentData = 0;
        Initialize();
    }
    
    public static unsafe void Initialize()
    {
        if (_initialized)
            return;

        _initialized = true;
        renderCallbackPtr = ManagedRenderEvent_GetCallback();
        renderCallbackDelegate = Marshal.GetDelegateForFunctionPointer<RenderPluginDelegate>(renderCallbackPtr);
        ManagedRenderEvent_SetMonoData(mono_thread_attach, mono_get_root_domain());
    }
#else
    public static unsafe void Initialize()
    {
        
    }
#endif
    
#if UNITY_EDITOR
    public static unsafe void IssuePluginEventAndData(this CommandBuffer cmdBuffer, RenderPluginDelegate @delegate, int eventId, IntPtr data)
    {
        Initialize();
        
        if (!@delegate.Method.IsStatic)
            delegateHolder.Add(@delegate);

        var d = new NativeCallbackData
        {
            eventId = eventId,
            runtimeMethodPointer = Marshal.GetFunctionPointerForDelegate(@delegate),
            addData = data
        };
        
        cmdBuffer.IssuePluginEventAndData(renderCallbackPtr, 0, new IntPtr(GetData(in d)));
    }
    
    public static unsafe void JustCall(RenderPluginDelegate @delegate, int eventId, IntPtr data)
    {
        var d = new NativeCallbackData
        {
            eventId = eventId,
            runtimeMethodPointer = Marshal.GetFunctionPointerForDelegate(@delegate),
            addData = data
        };

        var dataPtr = (IntPtr)GetData(in d);
        renderCallbackDelegate(0, dataPtr);
    }
#else
    public static unsafe void IssuePluginEventAndData(this CommandBuffer cmdBuffer, RenderPluginDelegate @delegate, int eventId, IntPtr data)
    {
        if (!@delegate.Method.IsStatic)
            delegateHolder.Add(@delegate);

        var funcPtr = Marshal.GetFunctionPointerForDelegate(@delegate);
        cmdBuffer.IssuePluginEventAndData(funcPtr, eventId, data);
    }
    
    public static unsafe void JustCall(RenderPluginDelegate @delegate, int eventId, IntPtr data)
    {
        @delegate(eventId, data);
    }
#endif
    
    #region No Args
    [DllImport(_pluginName)]
    public static extern unsafe void ManagedRenderEvent_Call_v(void *funcPtr);

    [DllImport(_pluginName)]
    public static extern unsafe void *ManagedRenderEvent_Call_p(void *funcPtr);

    [DllImport(_pluginName)]
    public static extern unsafe int ManagedRenderEvent_Call_i(void *funcPtr);

    [DllImport(_pluginName)]
    public static extern unsafe long ManagedRenderEvent_Call_l(void *funcPtr);

    [DllImport(_pluginName)]
    public static extern unsafe float ManagedRenderEvent_Call_f(void *funcPtr);

    [DllImport(_pluginName)]
    public static extern unsafe double ManagedRenderEvent_Call_d(void *funcPtr);
    #endregion
    
    #region 1 arg
    [DllImport(_pluginName)]
    public static extern unsafe void ManagedRenderEvent_Call_vp(void *funcPtr, void *a);

    [DllImport(_pluginName)]
    public static extern unsafe void *ManagedRenderEvent_Call_pp(void *funcPtr, void *a);

    [DllImport(_pluginName)]
    public static extern unsafe int ManagedRenderEvent_Call_ip(void *funcPtr, void *a);

    [DllImport(_pluginName)]
    public static extern unsafe long ManagedRenderEvent_Call_lp(void *funcPtr, void *a);

    [DllImport(_pluginName)]
    public static extern unsafe float ManagedRenderEvent_Call_fp(void *funcPtr, void *a);

    [DllImport(_pluginName)]
    public static extern unsafe double ManagedRenderEvent_Call_dp(void *funcPtr, void *a);
    #endregion
}