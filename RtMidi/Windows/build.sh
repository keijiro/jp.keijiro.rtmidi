#!/bin/sh

FLAGS="-O2 -Wall -shared -I../Source -D RTMIDI_EXPORT -D __WINDOWS_MM__"

x86_64-w64-mingw32-g++ $FLAGS -c ../Source/RtMidi.cpp
x86_64-w64-mingw32-g++ $FLAGS -c ../Source/rtmidi_c.cpp
x86_64-w64-mingw32-g++ $FLAGS RtMidi.o rtmidi_c.o -Wl,-Bstatic -lwinmm -static-libgcc -o RtMidi.dll
x86_64-w64-mingw32-strip RtMidi.dll
