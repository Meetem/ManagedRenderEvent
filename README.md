# Native Code plugin for safe calling managed method by IssuePluginEventAndData within unity editor

Errm, ok, couldn't I just:
`CommandBuffer.IssuePluginEventAndData(Marshal.GetFunctionPointerForDelegate, evtId, data)`?
Well, yes, you can. 

## What's the point then?

After you call managed method by pointer within separate thread, Unity Editor will stuck at assembly reload phase. This plugin allow you to call managed method from graphics thread and don't be punished for that.

### How?

The trick is to wrap by-pointer managed function call into native call and `mono_thread_attach(rootDomain)` before any managed call

Then you can just use extension method for CommandBuffer like usual. Bonus point: you can pass delegate to it.

* Don't forget to add `[MonoPInvokeCallback(typeof(RenderPluginDelegate))]` for your callback

```csharp
    [MonoPInvokeCallback(typeof(RenderPluginDelegate))]
    private static unsafe void MakeGraphicsCallback(int eventId, IntPtr data)
    {
        // do your precious stuff here
    }

    ...
    // Call method from command buffer (gfx thread)
    cmdBuffer.IssuePluginEventAndData(MakeGraphicsCallback, 123, new IntPtr(11111));
```

### But, wait...
- Q: Couldn't you just call `mono_thread_attach(rootDomain)` once from gfx thread and don't wrap every call?
- A: Yes, you can< and it's implemented as default behaviour. Make sure you don't run C# in graphics thread before ManagedRenderEvent.IsInitialized (or call Initialize() by yourself).

