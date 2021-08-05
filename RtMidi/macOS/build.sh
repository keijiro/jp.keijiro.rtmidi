#!/bin/sh

ARGS="-O2 -Wall -shared -rdynamic -fPIC"
ARGS+=" -I../Source -D RTMIDI_EXPORT -D __MACOSX_CORE__"
ARGS+=" ../Source/RtMidi.cpp ../Source/rtmidi_c.cpp"
ARGS+=" -framework CoreAudio -framework CoreFoundation -framework CoreMidi"
ARGS+=" -lstdc++"

set -x

gcc -target x86_64-apple-macos10.12 $ARGS -o x86_64.so
gcc -target  arm64-apple-macos10.12 $ARGS -o arm64.so

lipo -create -output RtMidi.bundle x86_64.so arm64.so
