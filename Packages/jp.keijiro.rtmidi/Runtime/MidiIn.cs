using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace RtMidi {

// MIDI-in device handler
public class MidiIn : SafeHandleZeroOrMinusOneIsInvalid
{
    #region SafeHandle implementation

    public MidiIn() : base(ownsHandle: true)
      => handle = _CreateDefault();

    public MidiIn(Api api = Api.Unspecified,
                  string clientName = "RtMidi Input Client",
                  int queueSizeLimit = 100)
      : base(ownsHandle: true)
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

    public delegate void MessageReceivedHandler
      (double time, ReadOnlySpan<byte> data);

    public MessageReceivedHandler MessageReceived
      { get => _messageReceived; set => SetBridgeCallback(value); }

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

    public void IgnoreTypes(bool sysex = true, bool time = true, bool sense = true)
      => _IgnoreTypes(this, sysex, time, sense);

    public unsafe ReadOnlySpan<byte> GetMessage(Span<byte> buffer, out double time)
    {
        var size = (nuint)buffer.Length;
        fixed (byte* ptr = buffer)
            time = _GetMessage(this, (IntPtr)ptr, ref size);
        return buffer.Slice(0, (int)size);
    }

    #endregion

    #region Message callback handling

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate void MidiCallback
      (double timeStamp, IntPtr message, nuint messageSize, IntPtr userData);

    MessageReceivedHandler _messageReceived;
    MidiCallback _bridgeCallback;

    unsafe void BridgeCallback(double time, IntPtr ptr, nuint size, IntPtr _)
    {
        try
        {
            var span = new ReadOnlySpan<byte>(ptr.ToPointer(), (int)size);
            if (_messageReceived != null) _messageReceived(time, span);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"Exception in MIDI callback: {e}");
        }
    }

    void SetBridgeCallback(MessageReceivedHandler handler)
    {
        _messageReceived = handler;
        if (handler != null)
        {
            if (_bridgeCallback == null)
            {
                _bridgeCallback = BridgeCallback;
                _SetCallback(this, _bridgeCallback, IntPtr.Zero);
            }
        }
        else
        {
            if (_bridgeCallback != null)
            {
                _bridgeCallback = null;
                _CancelCallback(this);
            }
        }
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

    [DllImport(Config.DllName, EntryPoint = "rtmidi_in_set_callback")]
    static extern void _SetCallback(MidiIn device, MidiCallback callback, IntPtr userData);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_in_cancel_callback")]
    static extern void _CancelCallback(MidiIn device);

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

} // namespace RtMidi
