SRCDIR = ../../source
SRCS = $(SRCDIR)/RenderingPlugin.cpp \
OBJS = ${SRCS:.cpp=.o}
UNITY_DEFINES = -DUNITY_LINUX=1
CXXFLAGS = $(UNITY_DEFINES) -O2 -fPIC
LDFLAGS = -shared -rdynamic
LIBS = 
PLUGIN_SHARED = libManagedRenderEvent.so
CXX ?= g++

.cpp.o:
	$(CXX) $(CXXFLAGS) -c -o $@ $<

all: shared

clean:
	rm -f $(OBJS) $(PLUGIN_SHARED)

shared: $(OBJS)
	$(CXX) $(LDFLAGS) -o $(PLUGIN_SHARED) $(OBJS) $(LIBS)
