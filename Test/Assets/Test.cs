using UnityEngine;
using System.Runtime.InteropServices;
using System;

unsafe sealed class Test : MonoBehaviour
{
    RtMidi.Wrapper* _device = null;

    void Start()
    {
        var apis = new RtMidi.Api[10];
        var res = RtMidi.rtmidi_get_compiled_api(apis, (uint)apis.Length);
        for (var i = 0; i < res; i++)
            Debug.Log(Marshal.PtrToStringAnsi(RtMidi.rtmidi_api_display_name(apis[i])));

        _device = RtMidi.rtmidi_in_create_default();
        if (_device == null || !_device->ok)
        {
            Debug.Log("Failed to create a default device.");
            return;
        }

        var portCount = RtMidi.rtmidi_get_port_count(_device);
        Debug.Log("number of ports " + portCount);

        for (var i = 0; i < portCount; i++)
            Debug.Log(Marshal.PtrToStringAnsi(RtMidi.rtmidi_get_port_name(_device, (uint)i)));

        if (portCount != 0) RtMidi.rtmidi_open_port(_device, 0, "RtMidi");
    }

    void Update()
    {
        if (_device == null || !_device->ok) return;

        byte* msg = stackalloc byte [32];

        while (true)
        {
            ulong size = 32;
            var stamp = RtMidi.rtmidi_in_get_message(_device, msg, ref size);
            if (stamp < 0 || size == 0) break;
            Debug.Log(msg[0]);
        }
    }

    void OnDestroy()
    {
        if (_device != null && _device->ok)
        {
            RtMidi.rtmidi_in_free(_device);
            _device = null;
        }
    }
}
