# About this project

See README.md for basic information about this project.

# Project structure

This directory is a Unity project used to test the plugin.

`/Plugin` contains the native part of the plugin, which is a customized version
of the RtMidi library.

`/rtmidi-upstream` contains the original (upstream) source code of the RtMidi
library.

`/Packages/jp.keijiro.rtmidi` contains the Unity package, which includes the
compiled binary files of the native plugin and the C# part of the plugin.
