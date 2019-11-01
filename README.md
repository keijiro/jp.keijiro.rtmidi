RtMidi for Unity
================

This is a wrapper library of [RtMidi] for Unity that allows Unity program to
access MIDI devices within C# scripts.

[RtMidi]: https://github.com/thestk/rtmidi

Please note that this library only provides very thin wrapper of the original
C language implementation. There are lots of unsafe and inconvenient elements
in the library. In other words, this is a kind of a raw device driver provided
for plugin developers -- It's not recommended to use in application code. 

System Requirements
-------------------

- Unity 2019.1 or later
- Windows, macOS or Linux
- Only supports 64-bit architecture
