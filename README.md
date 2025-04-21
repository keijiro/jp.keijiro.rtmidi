# RtMidi for Unity

**RtMidi for Unity** is a wrapper plugin of [RtMidi] that enables Unity to
access MIDI devices from C# scripts.

[RtMidi]: https://github.com/thestk/rtmidi

Note that this plugin only provides a thin wrapper around the original C API,
which requires the use of unsafe and unmanaged code. It's not recommended to
use this plugin directoly in application code. Instead, consider using a
high-level library like [Minis].

[Minis]: https://github.com/keijiro/Minis

## System Requirements

- Unity 2022.3 LTS or later

Currently, RtMidi for Unity supports only desktop platforms (Windows, macOS,
and Linux).

## Installation

You can install the RtMidi for Unity package (`jp.keijiro.rtmidi`) via the
"Keijiro" scoped registry using the Unity Package Manager. To add the registry
to your project, follow [these instructions].

[these instructions]:
  https://gist.github.com/keijiro/f8c7e8ff29bfe63d86b888901b82644c

## Samples

This repository includes a sample implementation of a low-level wrapper class
for sending and receiving MIDI messages via the plugin.Please refer to the
script files in the `Assets` directory for more details.
