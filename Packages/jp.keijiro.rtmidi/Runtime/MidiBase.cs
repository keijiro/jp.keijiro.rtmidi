using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace RtMidi {

// Base class for MIDI port handles
public abstract class MidiBase : SafeHandleZeroOrMinusOneIsInvalid
{
    #region SafeHandle implementation

    protected MidiBase(IntPtr ptr) : base(true)
      => SetHandle(ptr);

    protected override bool ReleaseHandle()
    {
        OnReleaseHandle();
        FreeDeviceHandle(handle);
        return true;
    }

    #endregion

    #region Virtual abstract methods

    protected abstract void OnReleaseHandle();
    protected abstract Api GetCurrentApiCore();
    protected abstract void FreeDeviceHandle(IntPtr ptr);

    #endregion

    #region Common public properties and methods

    public bool IsOk => WrapperStruct.IsOk(handle);
    public string Error => WrapperStruct.GetMessage(handle);
    public Api CurrentApi => GetCurrentApiCore();
    public int PortCount => (int)_GetPortCount(this);

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

    #endregion

    #region Shared P/Invoke interface

    [DllImport(Config.DllName, EntryPoint = "rtmidi_open_port")]
    static extern void _OpenPort(MidiBase device, uint portNumber, string portName);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_open_virtual_port")]
    static extern void _OpenVirtualPort(MidiBase device, string portName);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_close_port")]
    static extern void _ClosePort(MidiBase device);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_get_port_count")]
    static extern uint _GetPortCount(MidiBase device);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_get_port_name")]
    static extern int _GetPortName(MidiBase device, uint portNumber, IntPtr bufOut, ref int bufLen);

    #endregion
}

} // namespace RtMidi
