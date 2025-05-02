#!/bin/sh
ANDROID_NDK_PATH="/Applications/Unity/Hub/Editor/6000.0.47f1/PlaybackEngines/AndroidPlayer/NDK/"
BIN_PATH=$ANDROID_NDK_PATH/toolchains/llvm/prebuilt/darwin-x86_64/bin
CC=$BIN_PATH/aarch64-linux-android30-clang++
STRIP=$BIN_PATH/llvm-strip
CCFLAGS="-O2 -Wall -fPIC -I.. -D RTMIDI_EXPORT -D __AMIDI__"
LDFLAGS="-shared -rdynamic -fPIC -Wl,-z,max-page-size=16384"
LIBS="-static-libstdc++ -lamidi"
$CC $CCFLAGS $LDFLAGS ../Source/RtMidi.cpp ../Source/rtmidi_c.cpp $LIBS -o libRtMidi.so
$STRIP libRtMidi.so
