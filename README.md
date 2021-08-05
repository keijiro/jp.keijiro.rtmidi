RtMidi for Unity
================

This is a wrapper library of [RtMidi] for Unity that allows Unity programs to
access MIDI devices within C# scripts.

[RtMidi]: https://github.com/thestk/rtmidi

Note that this library only provides a thin wrapper of the original C language
implementation. There are lots of unsafe and inconvenient elements in the
library. In other words, this is a kind of a raw device driver provided for
plugin developers -- It's not recommended to use directly in application code.

System Requirements
-------------------

- Unity 2019.1 or later
- Windows, macOS or Linux
- Only supports 64-bit architecture

How To Install
--------------

This package uses the [scoped registry] feature to resolve package
dependencies. Please add the following sections to the manifest file
(Packages/manifest.json).

[scoped registry]: https://docs.unity3d.com/Manual/upm-scoped.html

To the `scopedRegistries` section:

```
{
  "name": "Keijiro",
  "url": "https://registry.npmjs.com",
  "scopes": [ "jp.keijiro" ]
}
```

To the `dependencies` section:

```
"jp.keijiro.rtmidi": "1.0.4"
```

After changes, the manifest file should look like below:

```
{
  "scopedRegistries": [
    {
      "name": "Keijiro",
      "url": "https://registry.npmjs.com",
      "scopes": [ "jp.keijiro" ]
    }
  ],
  "dependencies": {
    "jp.keijiro.rtmidi": "1.0.4",
    ...
```

Examples
--------

There is an example implementation of low-level wrapper class for using the
plugin to input/output MIDI messages. Please see the script files under the
Assets directory in this repository for details.

Frequently Asked Questions
--------------------------

#### Nice! So is it possible to implement a music sequencer with Unity?

Unfortunately, it's quite hard -- I'd say it's almost impossible.

The script behaviors in Unity are hard-locked to screen refreshing. Due to this
limitation, you can't achieve enough temporal resolution that is required to
keep a correct tempo.

If the display is refreshing at 60 Hz, and the tempo of the song is 120 BPM, it
could work correctly. But how about 130 BPM? What if the display is refreshing
at 50 Hz?

You may have to create a separate thread and use a media-timer API or something
like that. However, this plugin is only tested in a non-threaded fashion. You
may have to test it on supported platforms too.

There might be more things to be considered. Does it worth investing time? Or
isn't it better to create a new plugin from scratch? There is no clear yes/no
answer to it. I just think it's quite hard.
