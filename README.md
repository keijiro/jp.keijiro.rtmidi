# RtMidi for Unity

![gif](https://github.com/user-attachments/assets/d1387fa6-a3a2-416e-99c7-e9db17df6923)

**RtMidi for Unity** is a wrapper plugin of [RtMidi] that enables Unity to
access MIDI devices from C# scripts.

[RtMidi]: https://github.com/thestk/rtmidi

Note that this plugin only provides low-level MIDI I/O functionality. For
application-level use, it's recommended to use a higher-level library like
[Minis], a MIDI extension for the Input System.

[Minis]: https://github.com/keijiro/Minis

## System Requirements

- Unity 2022.3 LTS or later

Currently, RtMidi for Unity supports the following platform and architecture
combinations:

- Windows: x86_64
- macOS: arm64 (Apple Silicon)
- iOS: arm64
- Linux: x86_64
- Android: arm64

## Installation

You can install the RtMidi for Unity package (`jp.keijiro.rtmidi`) via the
"Keijiro" scoped registry using the Unity Package Manager. To add the registry
to your project, follow [these instructions].

[these instructions]:
  https://gist.github.com/keijiro/f8c7e8ff29bfe63d86b888901b82644c
