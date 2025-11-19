#!/bin/sh
set -exuo pipefail

RPATH_FLAGS="-install_name @rpath/RtMidi.bundle"
VER_FLAGS="-current_version 1.0.0 -compatibility_version 1.0.0"

make ARCH=arm64  SO_ARGS="$RPATH_FLAGS $VER_FLAGS" -f Makefile.macos
make ARCH=x86_64 SO_ARGS="$RPATH_FLAGS $VER_FLAGS" -f Makefile.macos

lipo -create -output RtMidi.bundle \
  build-macOS-arm64/libRtMidi.so \
  build-macOS-x86_64/libRtMidi.so

cp RtMidi.bundle ../Packages/jp.keijiro.rtmidi/Runtime/Plugins/macOS/
