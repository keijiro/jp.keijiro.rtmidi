using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System;

namespace RtMidi {

// MIDI API specifier arguments
public enum Api
{
    Unspecified,
    MacOsXCore,
    LinuxAlsa,
    UnixJack,
    WindowsMM,
    RtMidiDummy,
    WebMidiApi,
    WindowsUwp,
    Android
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
    ThreadError
}

// RtMidiWrapper struct for pointer dereference
[StructLayout(LayoutKind.Sequential)]
public struct WrapperStruct
{
    public IntPtr ptr;
    public IntPtr data;
    [MarshalAs(UnmanagedType.U1)]
    public bool ok;
    public IntPtr msg;

    public unsafe static bool IsOk(IntPtr ptr)
      => ((WrapperStruct*)ptr)->ok;

    public unsafe static string GetMessage(IntPtr ptr)
      => Marshal.PtrToStringAnsi(((WrapperStruct*)ptr)->msg);
}

static class Config
{
    public const string DllName = "RtMidi";
}

public static class MidiSystem
{
    public static string GetVersion()
      => Marshal.PtrToStringAnsi(_GetVersion());

    [DllImport(Config.DllName, EntryPoint = "rtmidi_get_version")]
    static extern IntPtr _GetVersion();

    [DllImport(Config.DllName, EntryPoint = "rtmidi_get_compiled_api")]
    public static extern int GetCompiledApi([Out] Api[] apis, uint apis_size);

    public static string ApiName(Api api)
      => Marshal.PtrToStringAnsi(_ApiName(api));

    [DllImport(Config.DllName, EntryPoint = "rtmidi_api_name")]
    static extern IntPtr _ApiName(Api api);

    public static string ApiDisplayName(Api api)
      => Marshal.PtrToStringAnsi(_ApiDisplayName(api));

    [DllImport(Config.DllName, EntryPoint = "rtmidi_api_display_name")]
    static extern IntPtr _ApiDisplayName(Api api);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_compiled_api_by_name")]
    public static extern Api CompiledApiByName(string name);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_error")]
    public static extern void Error(ErrorType type, string errorString);
}

public unsafe class MidiIn : SafeHandleZeroOrMinusOneIsInvalid
{
    #region SafeHandle implementation

    public MidiIn() : base(true)
      => handle = _CreateDefault();

    public MidiIn(Api api, string clientName, int queueSizeLimit) : base(true)
      => handle = _Create(api, clientName, (uint)queueSizeLimit);

    protected override bool ReleaseHandle()
    {
        _Free(handle);
        return true;
    }

    #endregion

    #region Public properties

    public bool IsOk => WrapperStruct.IsOk(handle);
    public string Error => WrapperStruct.GetMessage(handle);
    public Api CurrentApi => _GetCurrentApi(this);
    public int PortCount => (int)_GetPortCount(this);

    #endregion

    #region Public methods

    public void OpenPort(int portNumber, string portName)
      => _OpenPort(this, (uint)portNumber, portName);

    public void OpenVirtualPort(string portName)
      => _OpenVirtualPort(this, portName);

    public void ClosePort()
      => _ClosePort(this);

    public unsafe string GetPortName(int portNumber)
    {
        var buflen = 256;
        _GetPortName(this, (uint)portNumber, IntPtr.Zero, ref buflen);
        buflen = System.Math.Clamp(buflen, 1, 256);
        var buf = stackalloc byte[buflen];
        _GetPortName(this, (uint)portNumber, (IntPtr)buf, ref buflen);
        return Marshal.PtrToStringAnsi((IntPtr)buf);
    }

    public void SetIgnoreTypes(bool sysex, bool time, bool sense)
      => _IgnoreTypes(this, sysex, time, sense);

    public ReadOnlySpan<byte> GetMessage(Span<byte> buffer, out double time)
    {
        var size = (nuint)buffer.Length;
        fixed (byte* ptr = buffer)
            time = _GetMessage(this, (IntPtr)ptr, ref size);
        return buffer.Slice(0, (int)size);
    }

    #endregion

    #region P/Invoke interface

    [DllImport(Config.DllName, EntryPoint = "rtmidi_in_create_default")]
    static extern IntPtr _CreateDefault();

    [DllImport(Config.DllName, EntryPoint = "rtmidi_in_create")]
    static extern IntPtr _Create(Api api, string clientName, uint queueSizeLimit);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_in_free")]
    static extern void _Free(IntPtr device);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_in_get_current_api")]
    static extern Api _GetCurrentApi(MidiIn device);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_in_ignore_types")]
    static extern void _IgnoreTypes
      (MidiIn device,
       [MarshalAs(UnmanagedType.U1)] bool midiSysex,
       [MarshalAs(UnmanagedType.U1)] bool midiTime,
       [MarshalAs(UnmanagedType.U1)] bool midiSense);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_in_get_message")]
    static extern double _GetMessage(MidiIn device, IntPtr message, ref nuint size);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_open_port")]
    static extern void _OpenPort(MidiIn device, uint portNumber, string portName);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_open_virtual_port")]
    static extern void _OpenVirtualPort(MidiIn device, string portName);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_close_port")]
    static extern void _ClosePort(MidiIn device);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_get_port_count")]
    static extern uint _GetPortCount(MidiIn device);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_get_port_name")]
    static extern int _GetPortName(MidiIn device, uint portNumber, IntPtr bufOut, ref int bufLen);

    #endregion
}

public unsafe class MidiOut : SafeHandleZeroOrMinusOneIsInvalid
{
    #region SafeHandle implementation

    public MidiOut() : base(true)
      => handle = _CreateDefault();

    public MidiOut(Api api, string clientName) : base(true)
      => handle = _Create(api, clientName);

    protected override bool ReleaseHandle()
    {
        _Free(handle);
        return true;
    }

    #endregion

    #region Public properties

    public bool IsOk => WrapperStruct.IsOk(handle);
    public string Error => WrapperStruct.GetMessage(handle);
    public Api CurrentApi => _GetCurrentApi(this);
    public int PortCount => (int)_GetPortCount(this);

    #endregion

    #region Public methods

    public void OpenPort(int portNumber, string portName)
      => _OpenPort(this, (uint)portNumber, portName);

    public void OpenVirtualPort(string portName)
      => _OpenVirtualPort(this, portName);

    public void ClosePort()
      => _ClosePort(this);

    public unsafe string GetPortName(int portNumber)
    {
        var buflen = 256;
        _GetPortName(this, (uint)portNumber, IntPtr.Zero, ref buflen);
        buflen = System.Math.Clamp(buflen, 1, 256);
        var buf = stackalloc byte[buflen];
        _GetPortName(this, (uint)portNumber, (IntPtr)buf, ref buflen);
        return Marshal.PtrToStringAnsi((IntPtr)buf);
    }

    public int SendMessage(ReadOnlySpan<byte> message)
    {
        fixed (byte* ptr = message)
            return _SendMessage(this, (IntPtr)ptr, message.Length);
    }

    #endregion

    #region P/Invoke interface

    [DllImport(Config.DllName, EntryPoint = "rtmidi_out_create_default")]
    static extern IntPtr _CreateDefault();

    [DllImport(Config.DllName, EntryPoint = "rtmidi_out_create")]
    static extern IntPtr _Create(Api api, string clientName);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_out_free")]
    static extern void _Free(IntPtr device);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_out_get_current_api")]
    static extern Api _GetCurrentApi(MidiOut device);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_out_send_message")]
    static extern int _SendMessage(MidiOut device, IntPtr message, int length);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_open_port")]
    static extern void _OpenPort(MidiOut device, uint portNumber, string portName);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_open_virtual_port")]
    static extern void _OpenVirtualPort(MidiOut device, string portName);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_close_port")]
    static extern void _ClosePort(MidiOut device);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_get_port_count")]
    static extern uint _GetPortCount(MidiOut device);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_get_port_name")]
    static extern int _GetPortName(MidiOut device, uint portNumber, IntPtr bufOut, ref int bufLen);

    #endregion
}

} // namespace RtMidi
