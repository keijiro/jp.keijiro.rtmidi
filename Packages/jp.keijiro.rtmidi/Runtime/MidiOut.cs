using System;
using System.Runtime.InteropServices;

namespace RtMidi {

// MIDI-out port handle
public class MidiOut : MidiBase
{
    #region Factory methods

    public static MidiOut Create()
      => new MidiOut(_OutCreateDefault());

    public static MidiOut Create(Api api = Api.Unspecified,
                                 string clientName = "RtMidi Output Client")
      => new MidiOut(_OutCreate(api, clientName));

    #endregion

    #region MidiBase specialization

    MidiOut(IntPtr ptr) : base(ptr) {}

    protected override void OnReleaseHandle() {}

    protected override Api GetCurrentApiCore()
      => _OutGetCurrentApi(this);

    protected override void FreeDeviceHandle(IntPtr ptr)
      => _OutFree(ptr);

    #endregion

    #region Public methods

    public unsafe int SendMessage(ReadOnlySpan<byte> message)
    {
        fixed (byte* ptr = message)
            return _OutSendMessage(this, (IntPtr)ptr, message.Length);
    }

    #endregion

    #region P/Invoke interface

    [DllImport(Config.DllName, EntryPoint = "rtmidi_out_create_default")]
    static extern IntPtr _OutCreateDefault();

    [DllImport(Config.DllName, EntryPoint = "rtmidi_out_create")]
    static extern IntPtr _OutCreate(Api api, string clientName);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_out_free")]
    static extern void _OutFree(IntPtr device);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_out_send_message")]
    static extern int _OutSendMessage(MidiOut device, IntPtr message, int length);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_out_get_current_api")]
    static extern Api _OutGetCurrentApi(MidiOut device);

    #endregion
}

} // namespace RtMidi
