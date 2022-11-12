// Example low level rendering Unity plugin
#include "Unity/IUnityInterface.h"
#include "PlatformBase.h"
#include <assert.h>
#include <math.h>
#include <vector>

// --------------------------------------------------------------------------
// UnitySetInterfaces
typedef void* MonoObjectPtr;
typedef void* RuntimeMethodInfoPtr;

//typedef void (UNITY_INTERFACE_API *PluginCallback)(int32_t eventId, void *data);
typedef void (UNITY_INTERFACE_API* PluginCallback)(int32_t eventId, void* data);
typedef void (UNITY_INTERFACE_API* PluginCallbackNative)(int eventId, void *data);

typedef void* (UNITY_INTERFACE_API *mono_runtime_invoke_ptr)(RuntimeMethodInfoPtr method, MonoObjectPtr classInstance, void** args, MonoObjectPtr* exception);
typedef void* (UNITY_INTERFACE_API* mono_thread_attach_ptr)(void *domain);
typedef void (UNITY_INTERFACE_API* mono_thread_detach_ptr)(void *domain);

static mono_thread_attach_ptr thread_attach_ptr = nullptr;
static void* domain;

extern "C" {
	// If exported by a plugin, this function will be called when the plugin is loaded.
	void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginLoad(IUnityInterfaces* unityInterfaces) {

	}
	// If exported by a plugin, this function will be called when the plugin is about to be unloaded.
	void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginUnload() {

	}
}

typedef struct {
	PluginCallbackNative callbackPtr;
	int32_t eventId;
	int32_t __unused;
	void* addData;
}NativeCallbackData;

extern "C" UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API ManagedRenderEvent_SetMonoData(mono_thread_attach_ptr p2, void *dom) {
	thread_attach_ptr = p2;
	domain = dom;
}

#define MAKE_EXPORT_CALL_0(sig, ret) extern "C" UNITY_INTERFACE_EXPORT ret UNITY_INTERFACE_API ManagedRenderEvent_Call_##sig(void* funcPtr) {\
	if (funcPtr == nullptr) return (ret)0;\
	typedef ret (UNITY_INTERFACE_API *CallbackFunc)();\
	return ((CallbackFunc)funcPtr)();\
}

#define MAKE_EXPORT_CALL_1(sig, ret, t1) extern "C" UNITY_INTERFACE_EXPORT ret UNITY_INTERFACE_API ManagedRenderEvent_Call_##sig(void* funcPtr, t1 a) {\
	if (funcPtr == nullptr) return (ret)0;\
	typedef ret (UNITY_INTERFACE_API* CallbackFunc)(t1);\
	return ((CallbackFunc)funcPtr)(a);\
}

#define MAKE_EXPORT_CALL_2(sig, ret, t1, t2) extern "C" UNITY_INTERFACE_EXPORT ret UNITY_INTERFACE_API ManagedRenderEvent_Call_##sig(void* funcPtr, t1 a, t2 b) {\
	if (funcPtr == nullptr) return (ret)0;\
	typedef ret (UNITY_INTERFACE_API* CallbackFunc)(t1, t2);\
	return ((CallbackFunc)funcPtr)(a, b);\
}

#define MAKE_EXPORT_CALL_3(sig, ret, t1, t2, t3) extern "C" UNITY_INTERFACE_EXPORT ret UNITY_INTERFACE_API ManagedRenderEvent_Call_##sig(void* funcPtr, t1 a, t2 b, t3 c) {\
	if (funcPtr == nullptr) return (ret)0;\
	typedef ret (UNITY_INTERFACE_API* CallbackFunc)(t1, t2, t3);\
	return ((CallbackFunc)funcPtr)(a, b, c);\
}

#define MAKE_EXPORT_CALL_4(sig, ret, t1, t2, t3, t4) extern "C" UNITY_INTERFACE_EXPORT ret UNITY_INTERFACE_API ManagedRenderEvent_Call_##sig(void* funcPtr, t1 a, t2 b, t3 c, t4 d) {\
	if (funcPtr == nullptr) return (ret)0;\
	typedef ret (UNITY_INTERFACE_API* CallbackFunc)(t1, t2, t3, t4);\
	return ((CallbackFunc)funcPtr)(a, b, c, d);\
}

typedef void* intptr;

extern "C" UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API ManagedRenderEvent_Call_v(void* funcPtr) {
	if (funcPtr == nullptr) return;
	typedef void(UNITY_INTERFACE_API* CallbackFunc)();
	return ((CallbackFunc)funcPtr)();
}

extern "C" UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API ManagedRenderEvent_Call_vp(void* funcPtr, void *a) {
	if (funcPtr == nullptr) return;
	typedef void(UNITY_INTERFACE_API* CallbackFunc)(void*);
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
	if (thread_attach_ptr == nullptr)
		return;

	if (data == nullptr)
		return;
	
	auto cbData = (NativeCallbackData*)data;
	if (cbData->callbackPtr == nullptr)
		return;

	auto thread = thread_attach_ptr(domain);
	cbData->callbackPtr(cbData->eventId, cbData->addData);
}

extern "C" UNITY_INTERFACE_EXPORT PluginCallback UNITY_INTERFACE_API ManagedRenderEvent_GetCallback() {
	return ManagedRenderEvent_MakeCall;
}

