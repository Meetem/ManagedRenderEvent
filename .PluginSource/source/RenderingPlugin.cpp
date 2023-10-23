// Example low level rendering Unity plugin
#include "Unity/IUnityInterface.h"
#include "PlatformBase.h"
#include <assert.h>
#include <math.h>
#include <vector>

#ifndef __forceinline
#define __forceinline
#endif

// --------------------------------------------------------------------------
// UnitySetInterfaces
typedef void* MonoObjectPtr;
typedef void* RuntimeMethodInfoPtr;

//typedef void (UNITY_INTERFACE_API *PluginCallback)(int32_t eventId, void *data);
typedef void (UNITY_INTERFACE_API* PluginCallbackWithoutData)(int32_t eventId);
typedef void (UNITY_INTERFACE_API* PluginCallbackUpdate)(int32_t eventId, void *data);
typedef void (UNITY_INTERFACE_API* PluginCallback)(int32_t eventId, void* data);
typedef void (UNITY_INTERFACE_API* InitializedCallback)();
typedef void (UNITY_INTERFACE_API* PluginCallbackNative)(int eventId, void *data);

typedef void* (UNITY_INTERFACE_API *mono_runtime_invoke_ptr)(RuntimeMethodInfoPtr method, MonoObjectPtr classInstance, void** args, MonoObjectPtr* exception);
typedef void* (UNITY_INTERFACE_API* mono_thread_attach_ptr)(void *domain);
typedef void (UNITY_INTERFACE_API* mono_thread_detach_ptr)(void *domain);

static mono_thread_attach_ptr thread_attach_ptr = nullptr;
static InitializedCallback initialized_callback = nullptr;
static void* domain = nullptr;

extern "C" {
	// If exported by a plugin, this function will be called when the plugin is loaded.
	void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginLoad(IUnityInterfaces* unityInterfaces) {

	}

	// If exported by a plugin, this function will be called when the plugin is about to be unloaded.
	void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginUnload() {

	}
}

__forceinline static void AttachCurrentThread() {
	if (thread_attach_ptr == nullptr || domain == nullptr)
		return;

	thread_attach_ptr(domain);
}

extern "C" UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API ManagedRenderEvent_SetMonoData(mono_thread_attach_ptr p2, void *dom, InitializedCallback initCallback) {
	thread_attach_ptr = p2;
	domain = dom;
	initialized_callback = initCallback;
}

typedef void* intptr;

static void UNITY_INTERFACE_API ManagedRenderEvent_Attach(int32_t eventId) {
	AttachCurrentThread();

	if(eventId == 1){
		if(initialized_callback != nullptr){
			initialized_callback();
		}
	}
}

static void UNITY_INTERFACE_API ManagedRenderEvent_AttachTextureUpdate(int32_t eventId, void *data) {
	AttachCurrentThread();
}

extern "C" UNITY_INTERFACE_EXPORT PluginCallbackWithoutData UNITY_INTERFACE_API ManagedRenderEvent_GetAttachCallback() {
	return ManagedRenderEvent_Attach;
}

extern "C" UNITY_INTERFACE_EXPORT PluginCallbackUpdate UNITY_INTERFACE_API ManagedRenderEvent_GetAttachForTextureUpdate() {
	return ManagedRenderEvent_AttachTextureUpdate;
}

#if 0
extern "C" UNITY_INTERFACE_EXPORT PluginCallback UNITY_INTERFACE_API ManagedRenderEvent_GetCallback() {
	return ManagedRenderEvent_MakeCall;
}

#define MAKE_EXPORT_CALL_0(sig, ret) extern "C" UNITY_INTERFACE_EXPORT ret UNITY_INTERFACE_API ManagedRenderEvent_Call_##sig(void* funcPtr) {\
	if (funcPtr == nullptr) return (ret)0;\
	typedef ret (UNITY_INTERFACE_API *CallbackFunc)();\
	AttachCurrentThread();\
	return ((CallbackFunc)funcPtr)();\
}

#define MAKE_EXPORT_CALL_1(sig, ret, t1) extern "C" UNITY_INTERFACE_EXPORT ret UNITY_INTERFACE_API ManagedRenderEvent_Call_##sig(void* funcPtr, t1 a) {\
	if (funcPtr == nullptr) return (ret)0;\
	typedef ret (UNITY_INTERFACE_API* CallbackFunc)(t1);\
	AttachCurrentThread();\
	return ((CallbackFunc)funcPtr)(a);\
}

#define MAKE_EXPORT_CALL_2(sig, ret, t1, t2) extern "C" UNITY_INTERFACE_EXPORT ret UNITY_INTERFACE_API ManagedRenderEvent_Call_##sig(void* funcPtr, t1 a, t2 b) {\
	if (funcPtr == nullptr) return (ret)0;\
	typedef ret (UNITY_INTERFACE_API* CallbackFunc)(t1, t2);\
	AttachCurrentThread();\
	return ((CallbackFunc)funcPtr)(a, b);\
}

#define MAKE_EXPORT_CALL_3(sig, ret, t1, t2, t3) extern "C" UNITY_INTERFACE_EXPORT ret UNITY_INTERFACE_API ManagedRenderEvent_Call_##sig(void* funcPtr, t1 a, t2 b, t3 c) {\
	if (funcPtr == nullptr) return (ret)0;\
	typedef ret (UNITY_INTERFACE_API* CallbackFunc)(t1, t2, t3);\
	AttachCurrentThread();\
	return ((CallbackFunc)funcPtr)(a, b, c);\
}

#define MAKE_EXPORT_CALL_4(sig, ret, t1, t2, t3, t4) extern "C" UNITY_INTERFACE_EXPORT ret UNITY_INTERFACE_API ManagedRenderEvent_Call_##sig(void* funcPtr, t1 a, t2 b, t3 c, t4 d) {\
	if (funcPtr == nullptr) return (ret)0;\
	typedef ret (UNITY_INTERFACE_API* CallbackFunc)(t1, t2, t3, t4);\
	AttachCurrentThread();\
	return ((CallbackFunc)funcPtr)(a, b, c, d);\
}

extern "C" UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API ManagedRenderEvent_Call_v(void* funcPtr) {
	if (funcPtr == nullptr) return;
	typedef void(UNITY_INTERFACE_API* CallbackFunc)();
	AttachCurrentThread();
	return ((CallbackFunc)funcPtr)();
}

extern "C" UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API ManagedRenderEvent_Call_vp(void* funcPtr, void *a) {
	if (funcPtr == nullptr) return;
	typedef void(UNITY_INTERFACE_API* CallbackFunc)(void*);
	AttachCurrentThread();
	return ((CallbackFunc)funcPtr)(a);
}

//MAKE_EXPORT_CALL_0(v, void)
MAKE_EXPORT_CALL_0(p, intptr)
MAKE_EXPORT_CALL_0(i, int32_t)
MAKE_EXPORT_CALL_0(l, int64_t)
MAKE_EXPORT_CALL_0(f, float)
MAKE_EXPORT_CALL_0(d, double)

MAKE_EXPORT_CALL_1(pp, intptr, intptr)
MAKE_EXPORT_CALL_1(ip, int32_t, intptr)
MAKE_EXPORT_CALL_1(lp, int64_t, intptr)
MAKE_EXPORT_CALL_1(fp, float, intptr)
MAKE_EXPORT_CALL_1(dp, double, intptr)

static void UNITY_INTERFACE_API ManagedRenderEvent_MakeCall(int32_t eventId, void *data) {
	if (data == nullptr)
		return;
	
	auto cbData = (NativeCallbackData*)data;
	if (cbData->callbackPtr == nullptr)
		return;

	AttachCurrentThread();
	cbData->callbackPtr(cbData->eventId, cbData->addData);
}
#endif
