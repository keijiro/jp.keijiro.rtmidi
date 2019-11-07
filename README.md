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

How To Install
--------------

This package uses the [scoped registry] feature to import dependent packages.
Please add the following sections to the package manifest file
(`Packages/manifest.json`).

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
"jp.keijiro.rtmidi": "1.0.1"
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
    "jp.keijiro.rtmidi": "1.0.1",
    ...
```

[scoped registry]: https://docs.unity3d.com/Manual/upm-scoped.html
