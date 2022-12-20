#pragma once

namespace PluginEvent {
	enum PluginEventId {
		None = 0,
		MultiDrawNvApi = 1,
		BindBuffers = 2,
		BindAndMultiDrawNvApi = 3,
		DrawIndexed = 4,
		ManagedCallback = 9999999
	};
}

typedef struct {
	void* functionOrRuntimeMethodPointer;
	void* objectPointer;

	int32_t eventId;
	int32_t __unused;
	int64_t addData;
}NativeCallbackData;