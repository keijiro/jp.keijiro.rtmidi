using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace RtMidi {

// MIDI-out device handler
public class MidiOut : SafeHandleZeroOrMinusOneIsInvalid
{
    #region Factory methods

    public static MidiOut Create()
      => new MidiOut(_CreateDefault());

    public static MidiOut Create(Api api = Api.Unspecified,
                                 string clientName = "RtMidi Output Client")
      => new MidiOut(_Create(api, clientName));

    #endregion

    #region SafeHandle implementation

    MidiOut(IntPtr ptr) : base(ownsHandle: true)
      => SetHandle(ptr);

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

    public void OpenPort(int portNumber = 0, string portName = "RtMidi")
      => _OpenPort(this, (uint)portNumber, portName);

    public void OpenVirtualPort(string portName = "RtMidi")
      => _OpenVirtualPort(this, portName);

    public void ClosePort()
      => _ClosePort(this);

    public unsafe string GetPortName(int portNumber = 0)
    {
        var buflen = 0;
        _GetPortName(this, (uint)portNumber, IntPtr.Zero, ref buflen);
        buflen = System.Math.Clamp(buflen, 1, 256);
        var buf = stackalloc byte[buflen];
        _GetPortName(this, (uint)portNumber, (IntPtr)buf, ref buflen);
        return Marshal.PtrToStringAnsi((IntPtr)buf);
    }

    public unsafe int SendMessage(ReadOnlySpan<byte> message)
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
