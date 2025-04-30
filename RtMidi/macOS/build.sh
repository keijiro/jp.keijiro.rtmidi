#!/bin/sh
CCFLAGS="-O2 -Wall -Wextra -I../Source -D RTMIDI_EXPORT -D __MACOSX_CORE__"
LDFLAGS="-dynamiclib -lstdc++ -framework CoreAudio -framework CoreFoundation -framework CoreMidi"
clang $CCFLAGS ../Source/RtMidi.cpp ../Source/rtmidi_c.cpp $LDFLAGS -o RtMidi.dylib
