﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace ManagedRender
{
    public delegate void RenderPluginDelegate(int eventId, IntPtr data);
    public delegate void RenderPluginDelegateWithoutData(int eventId);

    public static class ManagedRenderEvent
    {
        private static HashSet<RenderPluginDelegate> delegateHolder = new HashSet<RenderPluginDelegate>();
        private static HashSet<CustomTextureUpdateV2> delegateHolder2 = new HashSet<CustomTextureUpdateV2>();
        private static HashSet<RenderPluginDelegateWithoutData> delegateHolder3 = new HashSet<RenderPluginDelegateWithoutData>();
        
        private const string _pluginName = "ManagedRenderEvent";
            
    #if UNITY_EDITOR
        private static volatile bool _initialized = false;
        private static volatile bool _fullyInitialized = false;
        public static bool IsInitialized => _initialized;
        public static bool IsFullyInitialized => _initialized && _fullyInitialized;
        
        [DllImport("__Internal")]
        public static extern IntPtr mono_thread_attach(IntPtr domain);

        [DllImport("__Internal")]
        public static extern IntPtr mono_get_root_domain();
        private delegate IntPtr MonoAttachThreadDelegate(IntPtr domain);

        private static Action completeInitCallback;
        
        [DllImport(_pluginName)]
        private static extern void ManagedRenderEvent_SetMonoData(MonoAttachThreadDelegate thread_attach, IntPtr domain, IntPtr onSetupCallback);
        
        [DllImport(_pluginName)]
        private static extern IntPtr ManagedRenderEvent_GetAttachCallback();

        [DllImport(_pluginName)]
        private static extern IntPtr ManagedRenderEvent_GetAttachForTextureUpdate();
        
        private static CommandBuffer bindEvents;
        static ManagedRenderEvent()
        {
            Initialize();
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            if (_initialized)
                return;

            _fullyInitialized = false;
            completeInitCallback = new Action(() =>
            {
                Debug.Log($"ManagedRenderEvent Initialized");
                _fullyInitialized = true;
            });
            
            ManagedRenderEvent_SetMonoData(mono_thread_attach, mono_get_root_domain(), 
                Marshal.GetFunctionPointerForDelegate(completeInitCallback));
            
            //Auto-bind
            bindEvents = new CommandBuffer();
            bindEvents.IssuePluginEvent(ManagedRenderEvent_GetAttachCallback(), 0);
            bindEvents.IssuePluginCustomTextureUpdateV2(ManagedRenderEvent_GetAttachForTextureUpdate(), Texture2D.whiteTexture, 0);
            bindEvents.IssuePluginCustomBlit(ManagedRenderEvent_GetAttachForTextureUpdate(), 0, 
                BuiltinRenderTextureType.CurrentActive,
                BuiltinRenderTextureType.CurrentActive,
                0, 0
                );
            
            bindEvents.IssuePluginEvent(ManagedRenderEvent_GetAttachCallback(), 1);
            Graphics.ExecuteCommandBuffer(bindEvents);
            _initialized = true;
        }
    #else
        public static bool IsInitialized => true;
        public static bool IsFullyInitialized => true;

        public static void Initialize()
        {
            
        }
    #endif

        public static void AllowSharpCallbacks(this CommandBuffer cmdBuffer)
        {
            #if UNITY_EDITOR
            cmdBuffer.IssuePluginEvent(ManagedRenderEvent_GetAttachCallback(), 0);
            #endif
        }
        
    //#if UNITY_EDITOR
        public static void IssuePluginEvent(RenderPluginDelegateWithoutData @delegate, int eventId)
        {
            if (!@delegate.Method.IsStatic)
                delegateHolder3.Add(@delegate);

            var funcPtr = Marshal.GetFunctionPointerForDelegate(@delegate);
            GL.IssuePluginEvent(funcPtr, eventId);
        }

        public static void IssuePluginEventAndData(this CommandBuffer cmdBuffer, RenderPluginDelegate @delegate, int eventId, IntPtr data)
        {
            if (!@delegate.Method.IsStatic)
                delegateHolder.Add(@delegate);

            var funcPtr = Marshal.GetFunctionPointerForDelegate(@delegate);
            cmdBuffer.IssuePluginEventAndData(funcPtr, eventId, data);
        }
        
        public static void IssuePluginCustomTextureUpdateV2(this CommandBuffer cmdBuffer, CustomTextureUpdateV2 @delegate, Texture texture, uint userData)
        {
            if (!@delegate.Method.IsStatic)
                delegateHolder2.Add(@delegate);

            var funcPtr = Marshal.GetFunctionPointerForDelegate(@delegate);
            cmdBuffer.IssuePluginCustomTextureUpdateV2(funcPtr, texture, userData);
        }
    //#endif
    }
}