using UnityEngine;
using System.Runtime.InteropServices;
using System;

static class RtMidi
{
    public enum Api {
        Unspecified, MacOsXCore, LinuxAlsa, UnixJack, WindowsMM, RtMidiDummy
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Callback(double timeStamp, IntPtr message, uint messageSize, IntPtr userData);

    [DllImport("RtMidi.dll")]
    public static extern int rtmidi_get_compiled_api(Api[] apis, uint apis_size);

    [DllImport("RtMidi.dll")]
    public static extern IntPtr rtmidi_api_name(Api api);

    [DllImport("RtMidi.dll")]
    public static extern IntPtr rtmidi_api_display_name(Api api);

    [DllImport("RtMidi.dll")]
    public static extern Api rtmidi_compiled_api_by_name(string name);

    [DllImport("RtMidi.dll")]
    public static extern IntPtr rtmidi_in_create_default();

    [DllImport("RtMidi.dll")]
    public static extern IntPtr rtmidi_in_create(Api api, string clientName, uint queueSizeLimit);

    [DllImport("RtMidi.dll")]
    public static extern void rtmidi_in_free(IntPtr device);

    [DllImport("RtMidi.dll")]
    public static extern void rtmidi_in_set_callback(IntPtr device, Callback callback, IntPtr userData);

    [DllImport("RtMidi.dll")]
    public static extern void rtmidi_close_port(IntPtr device);

    [DllImport("RtMidi.dll")]
    public static extern uint rtmidi_get_port_count(IntPtr device);

    [DllImport("RtMidi.dll")]
    public static extern IntPtr rtmidi_get_port_name(IntPtr device, uint portNumber);

    [DllImport("RtMidi.dll")]
    public static extern void rtmidi_open_port(IntPtr device, uint portNumber, string portName);
}

sealed class Test : MonoBehaviour
{
    static void Callback(double timeStamp, IntPtr message, uint messageSize, IntPtr userData)
    {
        Debug.Log(timeStamp);
    }

    IntPtr _device;

    void Start()
    {
        var apis = new RtMidi.Api[10];
        var res = RtMidi.rtmidi_get_compiled_api(apis, (uint)apis.Length);
        for (var i = 0; i < res; i++)
            Debug.Log(Marshal.PtrToStringAnsi(RtMidi.rtmidi_api_display_name(apis[i])));

        _device = RtMidi.rtmidi_in_create_default();

        var portCount = RtMidi.rtmidi_get_port_count(_device);
        for (var i = 0; i < portCount; i++)
            Debug.Log(Marshal.PtrToStringAnsi(RtMidi.rtmidi_get_port_name(_device, (uint)i)));

        RtMidi.rtmidi_in_set_callback(_device, Callback, IntPtr.Zero);

        if (portCount != 0) RtMidi.rtmidi_open_port(_device, 0, "RtMidi");
    }

    void OnDestroy()
    {
        RtMidi.rtmidi_close_port(_device);
        RtMidi.rtmidi_in_free(_device);
    }
}
