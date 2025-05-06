This directory contains a modified version of RtMidi along with
platform-specific Makefiles for building the plugin binary file.

## How to build

### Windows

Requires WSL2 and `mingw-w64` to build the DLL file.

```
$ sudo apt install mingw-w64
$ make -f Makefile.windows copy
```

### macOS

```
$ make -f Makefile.macos copy
```

### iOS

```
$ make -f Makefile.ios copy
```

### Linux

Requires `clang` and `libasound2-dev` to build the shared object file.

```
$ sudo apt install clang libasound2-dev
$ make -f Makefile.linux copy
```

### Android

Requires the Android NDK to build the shared object file. You can use the NDK
included with the Android module in the Unity Editor installation.

```
$ export ANDROID_NDK_PATH=/path/to/ndk/dir
$ make -f Makefile.android copy
```

### Web

Requires the Emscripten toolchain to build the library file. You can use the
Emscripten toolchain with the Web module in the Unity Editor installation.

```
$ export EM_DIR=/path/to/emscripten
$ make -f Makefile.web copy
```
