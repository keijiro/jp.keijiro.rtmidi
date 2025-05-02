#!/bin/sh
CC="x86_64-w64-mingw32-g++"
STRIP="x86_64-w64-mingw32-strip"
CCFLAGS="-O2 -Wall -I../Source -D RTMIDI_EXPORT -D __WINDOWS_MM__"
LDFLAGS="-shared"
LIBS="-Wl,-Bstatic -lwinmm -static-libgcc"
$CC $CCFLAGS $LDFLAGS ../Source/RtMidi.cpp ../Source/rtmidi_c.cpp $LIBS -o RtMidi.dll
$STRIP RtMidi.dll
