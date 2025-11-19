using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace RtMidi {

// Base class for MIDI port handles
public abstract class MidiBase : SafeHandleZeroOrMinusOneIsInvalid,
                                 ErrorCallbackBridge.IListener
{
    #region SafeHandle implementation

    protected MidiBase(IntPtr ptr) : base(true)
      => SetHandle(ptr);

    protected override bool ReleaseHandle()
    {
        ReleaseErrorCallback();
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

    public delegate void ErrorReceivedHandler(ErrorType type, string message);

    public ErrorReceivedHandler ErrorReceived
      { get => _onError.handler; set => SetErrorCallback(value); }

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

    #region Error callback handling

    (ErrorReceivedHandler handler, ErrorCallbackBridge bridge) _onError;

    void ErrorCallbackBridge.IListener.OnError(ErrorType type, string message)
      => _onError.handler?.Invoke(type, message);

    void SetErrorCallback(ErrorReceivedHandler handler)
    {
        ReleaseErrorCallback();

        if (handler != null)
        {
            _onError.handler = handler;
            _onError.bridge = new ErrorCallbackBridge(this);
            _SetErrorCallback(this, _onError.bridge.Callback, _onError.bridge.UserData);
        }
    }

    void ReleaseErrorCallback()
    {
        if (_onError.bridge == null) return;
        _CancelErrorCallback(handle);
        _onError.bridge.Dispose();
        _onError = (null, null);
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

    [DllImport(Config.DllName, EntryPoint = "rtmidi_set_error_callback")]
    static extern void _SetErrorCallback(MidiBase device, ErrorCallback callback, IntPtr userData);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_cancel_error_callback")]
    static extern void _CancelErrorCallback(IntPtr device);

    #endregion
}

} // namespace RtMidi
