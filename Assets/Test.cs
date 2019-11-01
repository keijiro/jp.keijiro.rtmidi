using UnityEngine;
using Marshal = System.Runtime.InteropServices.Marshal;
using RtMidiDll = RtMidi.Unmanaged;

unsafe sealed class Test : MonoBehaviour
{
    RtMidiDll.Wrapper* _device = null;

    void Start()
    {
        // Show available APIs.
        var apis = new RtMidiDll.Api[10];
        var apiCount = RtMidiDll.GetCompiledApi(apis, (uint)apis.Length);
        for (var i = 0; i < apiCount; i++)
            Debug.Log(Marshal.PtrToStringAnsi(RtMidiDll.ApiDisplayName(apis[i])));

        // Try creating a default MIDI-in device.
        _device = RtMidiDll.InCreateDefault();

        if (_device == null || !_device->ok)
        {
            Debug.Log("Failed to create a default device.");
            return;
        }

        // Show available ports.
        var portCount = RtMidiDll.GetPortCount(_device);
        for (var i = 0; i < portCount; i++)
            Debug.Log(Marshal.PtrToStringAnsi(RtMidiDll.GetPortName(_device, (uint)i)));

        // On Linux the first port must be a MIDI-thru port, so it's better to
        // choose the last port for testing use.
        if (portCount > 0) RtMidiDll.OpenPort(_device, portCount - 1, "RtMidi");
    }

    void Update()
    {
        if (_device == null || !_device->ok) return;

        // Output queued messages.
        byte* msg = stackalloc byte [32];

        while (true)
        {
            ulong size = 32;
            var stamp = RtMidiDll.InGetMessage(_device, msg, ref size);
            if (stamp < 0 || size == 0) break;

            Debug.Log(string.Format("{0:X} {1} {2}", msg[0], msg[1], msg[2]));
        }
    }

    void OnDestroy()
    {
        if (_device != null && _device->ok)
        {
            // Release the device.
            RtMidiDll.InFree(_device);
            _device = null;
        }
    }
}
