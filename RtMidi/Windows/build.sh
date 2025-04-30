#!/bin/sh
CC="x86_64-w64-mingw32-g++"
STRIP="x86_64-w64-mingw32-strip"
CCFLAGS="-O2 -Wall -I../Source -D RTMIDI_EXPORT -D __WINDOWS_MM__"
LDFLAGS="-shared"
LIBS=" -Wl,-Bstatic -lwinmm -static-libgcc"
$CC $CCFLAGS -c ../Source/RtMidi.cpp
$CC $CCFLAGS -c ../Source/rtmidi_c.cpp
$CC $LDFLAGS RtMidi.o rtmidi_c.o $LIBS -o RtMidi.dll
$STRIP RtMidi.dll
