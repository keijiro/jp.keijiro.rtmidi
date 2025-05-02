#!/bin/sh
SYSROOT=`xcrun --sdk iphoneos --show-sdk-path`
XFLAGS="-target arm64-apple-ios -isysroot $SYSROOT -fembed-bitcode"
CCFLAGS="-O2 -Wall -Wextra -I../Source -D RTMIDI_EXPORT -D __MACOSX_CORE__"
clang $XFLAGS $CCFLAGS -c ../Source/RtMidi.cpp ../Source/rtmidi_c.cpp
ar -crv libRtMidi.a RtMidi.o rtmidi_c.o
