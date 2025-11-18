using System;
using System.Runtime.InteropServices;

namespace RtMidi {

// MIDI-in port handle
public class MidiIn : MidiBase, MessageCallbackBridge.IListener
{
    #region Factory methods

    public static MidiIn Create()
      => new MidiIn(_InCreateDefault());

    public static MidiIn Create(Api api = Api.Unspecified,
                                string clientName = "RtMidi Input Client",
                                int queueSizeLimit = 100)
      => new MidiIn(_InCreate(api, clientName, (uint)queueSizeLimit));

    #endregion

    #region MidiBase specialization

    MidiIn(IntPtr ptr) : base(ptr) {}

    protected override void OnReleaseHandle()
    {
        if (_onMessage.bridge != null)
        {
            _InCancelCallback(handle);
            _onMessage.bridge.Dispose();
            _onMessage = (null, null);
        }
    }

    protected override Api GetCurrentApiCore()
      => _InGetCurrentApi(this);

    protected override void FreeDeviceHandle(IntPtr ptr)
      => _InFree(ptr);

    #endregion

    #region Public properties

    public delegate void MessageReceivedHandler(double timeStamp, ReadOnlySpan<byte> data);

    public MessageReceivedHandler MessageReceived
      { get => _onMessage.handler; set => SetMessageCallback(value); }

    #endregion

    #region Public methods

    public void IgnoreTypes(bool sysex = true, bool time = true, bool sense = true)
      => _InIgnoreTypes(this, sysex, time, sense);

    public unsafe ReadOnlySpan<byte> GetMessage(Span<byte> buffer, out double time)
    {
        var size = (nuint)buffer.Length;
        fixed (byte* ptr = buffer)
            time = _InGetMessage(this, (IntPtr)ptr, ref size);
        return buffer.Slice(0, (int)size);
    }

    #endregion

    #region Message callback handling

    (MessageReceivedHandler handler, MessageCallbackBridge bridge) _onMessage;

    void MessageCallbackBridge.IListener.OnMessage(double timeStamp, ReadOnlySpan<byte> data)
      => _onMessage.handler?.Invoke(timeStamp, data);

    void SetMessageCallback(MessageReceivedHandler handler)
    {
        if (_onMessage.bridge != null)
        {
            _InCancelCallback(this);
            _onMessage.bridge.Dispose();
            _onMessage = (null, null);
        }

        if (handler != null)
        {
            _onMessage.handler = handler;
            _onMessage.bridge = new MessageCallbackBridge(this);
            _InSetCallback(this, _onMessage.bridge.Callback, _onMessage.bridge.UserData);
        }
    }

    #endregion

    #region P/Invoke interface

    [DllImport(Config.DllName, EntryPoint = "rtmidi_in_create_default")]
    static extern IntPtr _InCreateDefault();

    [DllImport(Config.DllName, EntryPoint = "rtmidi_in_create")]
    static extern IntPtr _InCreate(Api api, string clientName, uint queueSizeLimit);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_in_free")]
    static extern void _InFree(IntPtr device);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_in_set_callback")]
    static extern void _InSetCallback(MidiIn device, MidiCallback callback, IntPtr userData);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_in_cancel_callback")]
    static extern void _InCancelCallback(MidiIn device);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_in_cancel_callback")]
    static extern void _InCancelCallback(IntPtr device);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_in_ignore_types")]
    static extern void _InIgnoreTypes
      (MidiIn device,
       [MarshalAs(UnmanagedType.U1)] bool midiSysex,
       [MarshalAs(UnmanagedType.U1)] bool midiTime,
       [MarshalAs(UnmanagedType.U1)] bool midiSense);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_in_get_current_api")]
    static extern Api _InGetCurrentApi(MidiIn device);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_in_get_message")]
    static extern double _InGetMessage(MidiIn device, IntPtr message, ref nuint size);

    #endregion
}

} // namespace RtMidi
