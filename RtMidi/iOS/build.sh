#!/bin/sh

SYSROOT=`xcrun --sdk iphoneos --show-sdk-path`
FLAGS="-O2 -Wall --sysroot $SYSROOT -isysroot $SYSROOT -fembed-bitcode"
FLAGS+=" -I../Source -D RTMIDI_EXPORT -D __MACOSX_CORE__"

gcc $FLAGS -c ../Source/RtMidi.cpp
gcc $FLAGS -c ../Source/rtmidi_c.cpp

ar -crv libRtMidi.a RtMidi.o rtmidi_c.o
