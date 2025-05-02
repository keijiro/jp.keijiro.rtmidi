#
# File listings
#

PRODUCT = RtMidi

SRCS = RtMidi.cpp \
       rtmidi_c.cpp

OBJ_DIR = build-$(PLATFORM)-$(ARCH)

#
# Intermediate/output files
#

OBJS = $(addprefix $(OBJ_DIR)/, $(notdir $(SRCS:.cpp=.o)))

ifeq ($(TARGET_TYPE), dll)
  TARGET = $(OBJ_DIR)/$(PRODUCT).$(TARGET_TYPE)
else
  TARGET = $(OBJ_DIR)/lib$(PRODUCT).$(TARGET_TYPE)
endif

#
# Compiler/linker options
#

CCFLAGS += -O2 -Wall -D RTMIDI_EXPORT -D $(MIDI_API)

ifeq ($(findstring clang,$(CC)), clang)
  CCFLAGS += -Wextra
endif

#
# Toolchain
#

ifndef AR
  AR = ar
endif

ifndef CC
  CC = clang
endif

ifndef STRIP
  STRIP = strip
endif

#
# Building rules
#

all: $(TARGET)

clean:
	rm -f $(TARGET) $(OBJS)

copy: $(TARGET)
	cp $(TARGET) ../Packages/jp.keijiro.rtmidi/Runtime/Plugins/$(PLATFORM)

$(OBJ_DIR)/$(PRODUCT).dll: $(OBJS)
	$(CC) $(LDFLAGS) -o $@ $^ $(LIBS)
	$(STRIP) $@

$(OBJ_DIR)/lib$(PRODUCT).dylib: $(OBJS)
	$(CC) $(LDFLAGS) -o $@ $^ $(LIBS)

$(OBJ_DIR)/lib$(PRODUCT).so: $(OBJS)
	$(CC) $(LDFLAGS) -o $@ $^ $(LIBS)
	$(STRIP) $@

$(OBJ_DIR)/lib$(PRODUCT).a: $(OBJS)
	$(AR) -crv $@ $^

$(OBJ_DIR)/%.o: %.cpp | $(OBJ_DIR)
	$(CC) $(CCFLAGS) -c -o $@ $<

$(OBJ_DIR):
	mkdir -p $(OBJ_DIR)
