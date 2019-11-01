using System.Runtime.InteropServices;
using System;

//
// CAUTION: We assume sizeof(size_t) == sizeof(ulong).
//
// NOTE: The RtMidi callback API is omitted because it doesn't work well under
// the unmanaged-to-managed invocation (it'll be broken when the callback is
// called from an unattached MIDI driver thread).
//

unsafe static class RtMidi
{
    // C wrapper class
    [StructLayout(LayoutKind.Sequential)]
    public struct Wrapper
    {
        public IntPtr ptr;
        public IntPtr data;
        [MarshalAs(UnmanagedType.U1)]
        public bool ok;
        public IntPtr msg;
    }

    // MIDI API specifier arguments
    public enum Api
    {
        Unspecified,
        MacOsXCore,
        LinuxAlsa,
        UnixJack,
        WindowsMM,
        RtMidiDummy
    }

    // Defined error types
    public enum ErrorType
    {
        Warning,
        DebugWarning,
        Unspecified,
        NoDevicesFound,
        InvalidDevice,
        MemoryError,
        InvalidParameter,
        InvalidUse,
        DriverError,
        SystemError,
        Thread_error
    }

    // RtMidi API

    [DllImport("RtMidi.dll")] public static extern
    int rtmidi_get_compiled_api(Api [] apis, uint apis_size);

    [DllImport("RtMidi.dll")] public static extern
    IntPtr rtmidi_api_name(Api api);

    [DllImport("RtMidi.dll")] public static extern
    IntPtr rtmidi_api_display_name(Api api);

    [DllImport("RtMidi.dll")] public static extern
    Api rtmidi_compiled_api_by_name(string name); 

    [DllImport("RtMidi.dll")] public static extern
    void rtmidi_open_port(Wrapper* device, uint portNumber, string portName);

    [DllImport("RtMidi.dll")] public static extern
    void rtmidi_open_virtual_port(Wrapper* device, string portName);

    [DllImport("RtMidi.dll")] public static extern
    void rtmidi_close_port(Wrapper* device);

    [DllImport("RtMidi.dll")] public static extern
    uint rtmidi_get_port_count(Wrapper* device);

    [DllImport("RtMidi.dll")] public static extern
    IntPtr rtmidi_get_port_name(Wrapper* device, uint portNumber);

    // RtMidiIn API

    [DllImport("RtMidi.dll")] public static extern
    Wrapper* rtmidi_in_create_default();

    [DllImport("RtMidi.dll")] public static extern
    Wrapper* rtmidi_in_create(Api api, string clientName, uint queueSizeLimit);

    [DllImport("RtMidi.dll")] public static extern
    void rtmidi_in_free(Wrapper* device);

    [DllImport("RtMidi.dll")] public static extern
    Api rtmidi_in_get_current_api(Wrapper* device);

    [DllImport("RtMidi.dll")] public static extern
    void rtmidi_in_ignore_types(
        Wrapper* device,
        [MarshalAs(UnmanagedType.U1)] bool midiSysex,
        [MarshalAs(UnmanagedType.U1)] bool midiTime,
        [MarshalAs(UnmanagedType.U1)] bool midiSense
    );

    [DllImport("RtMidi.dll")] public static extern
    double rtmidi_in_get_message(Wrapper* device, byte* message, ref ulong size);

    // RtMidiOut API

    [DllImport("RtMidi.dll")] public static extern
    Wrapper* rtmidi_out_create_default();

    [DllImport("RtMidi.dll")] public static extern
    Wrapper* rtmidi_out_create(Api api, string clientName);

    [DllImport("RtMidi.dll")] public static extern
    void rtmidi_out_free(Wrapper* device);

    [DllImport("RtMidi.dll")] public static extern
    Api rtmidi_out_get_current_api(Wrapper* device);

    [DllImport("RtMidi.dll")] public static extern
    int rtmidi_out_send_message(Wrapper* device, byte* message, int length);
}
