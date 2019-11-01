//
// RtMidi interop class
//
// This class is a thin interface for the unmanaged RtMidi code that only
// provides an unsafe way to use the library. It's not recommended to directly
// use this class in an application code.
//

using System.Runtime.InteropServices;
using System;

//
// IMPORTANT: We assume sizeof(size_t) is equal to sizeof(ulong). #FIXME
//

//
// NOTE: The RtMidi callback API is intentionally omitted because it doesn't
// work well under the unmanaged-to-managed invocation. It'll be broken when
// the callback is called from an unattached MIDI driver thread.
//

namespace RtMidi
{
    unsafe public static class Unmanaged
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

        [DllImport("RtMidi.dll", EntryPoint="rtmidi_get_compiled_api")]
        public static extern
        int GetCompiledApi(Api [] apis, uint apis_size);

        [DllImport("RtMidi.dll", EntryPoint="rtmidi_api_name")]
        public static extern
        IntPtr ApiName(Api api);

        [DllImport("RtMidi.dll", EntryPoint="rtmidi_api_display_name")]
        public static extern
        IntPtr ApiDisplayName(Api api);

        [DllImport("RtMidi.dll", EntryPoint="rtmidi_compiled_api_by_name")]
        public static extern
        Api CompiledApiByName(string name); 

        [DllImport("RtMidi.dll", EntryPoint="rtmidi_open_port")]
        public static extern
        void OpenPort(Wrapper* device, uint portNumber, string portName);

        [DllImport("RtMidi.dll", EntryPoint="rtmidi_open_virtual_port")]
        public static extern
        void OpenVirtualPort(Wrapper* device, string portName);

        [DllImport("RtMidi.dll", EntryPoint="rtmidi_close_port")]
        public static extern
        void ClosePort(Wrapper* device);

        [DllImport("RtMidi.dll", EntryPoint="rtmidi_get_port_count")]
        public static extern
        uint GetPortCount(Wrapper* device);

        [DllImport("RtMidi.dll", EntryPoint="rtmidi_get_port_name")]
        public static extern
        IntPtr GetPortName(Wrapper* device, uint portNumber);

        // RtMidiIn API

        [DllImport("RtMidi.dll", EntryPoint="rtmidi_in_create_default")]
        public static extern
        Wrapper* InCreateDefault();

        [DllImport("RtMidi.dll", EntryPoint="rtmidi_in_create")]
        public static extern
        Wrapper* InCreate(Api api, string clientName, uint queueSizeLimit);

        [DllImport("RtMidi.dll", EntryPoint="rtmidi_in_free")]
        public static extern
        void InFree(Wrapper* device);

        [DllImport("RtMidi.dll", EntryPoint="rtmidi_in_get_current_api")]
        public static extern
        Api InGetCurrentApi(Wrapper* device);

        [DllImport("RtMidi.dll", EntryPoint="rtmidi_in_ignore_types")]
        public static extern
        void InIgnoreTypes(
            Wrapper* device,
            [MarshalAs(UnmanagedType.U1)] bool midiSysex,
            [MarshalAs(UnmanagedType.U1)] bool midiTime,
            [MarshalAs(UnmanagedType.U1)] bool midiSense
        );

        [DllImport("RtMidi.dll", EntryPoint="rtmidi_in_get_message")]
        public static extern
        double InGetMessage(Wrapper* device, byte* message, ref ulong size);

        // RtMidiOut API

        [DllImport("RtMidi.dll", EntryPoint="rtmidi_out_create_default")]
        public static extern
        Wrapper* OutCreateDefault();

        [DllImport("RtMidi.dll", EntryPoint="rtmidi_out_create")]
        public static extern
        Wrapper* OutCreate(Api api, string clientName);

        [DllImport("RtMidi.dll", EntryPoint="rtmidi_out_free")]
        public static extern
        void OutFree(Wrapper* device);

        [DllImport("RtMidi.dll", EntryPoint="rtmidi_out_get_current_api")]
        public static extern
        Api OutGetCurrentApi(Wrapper* device);

        [DllImport("RtMidi.dll", EntryPoint="rtmidi_out_send_message")]
        public static extern
        int OutSendMessage(Wrapper* device, byte* message, int length);
    }
}
