using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace RtMidi {

// MIDI-in port handle
public class MidiIn : MidiBase
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
        if (_bridgeCallback != null)
        {
            _InCancelCallback(handle);
            _bridgeCallback = null;
        }
        if (_self.IsAllocated) _self.Free();
    }

    protected override Api GetCurrentApiCore()
      => _InGetCurrentApi(this);

    protected override void FreeDeviceHandle(IntPtr ptr)
      => _InFree(ptr);

    #endregion

    #region Public properties

    public delegate void MessageReceivedHandler
      (double time, ReadOnlySpan<byte> data);

    public MessageReceivedHandler MessageReceived
      { get => _messageReceived; set => SetBridgeCallback(value); }

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

    // IL2CPP only supports delegates to static methods. To work around this,
    // we define a static "bridge" callback that relays calls from the unmanaged
    // RtMidi library to a user-provided delegate.

    // Delegate type for RtMidi callback function pointer
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate void MidiCallback
      (double timeStamp, IntPtr message, nuint messageSize, IntPtr userData);

    // Managed callback delegate
    MessageReceivedHandler _messageReceived;

    // Bridge callback delegate
    MidiCallback _bridgeCallback;

    // GCHandle used to pass a "self" reference to unmanaged code
    GCHandle _self;

    // Bridge callback implementation
    [AOT.MonoPInvokeCallback(typeof(MidiCallback))]
    unsafe static void BridgeCallback(double time, IntPtr ptr, nuint size, IntPtr user)
    {
        var span = new ReadOnlySpan<byte>(ptr.ToPointer(), (int)size);
        var self = (MidiIn)GCHandle.FromIntPtr(user).Target;
        try
        {
            self._messageReceived(time, span);
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception in MIDI callback: {e}");
        }
    }

    // Bridge callback setter/resetter
    void SetBridgeCallback(MessageReceivedHandler handler)
    {
        if (handler != null)
        {
            _messageReceived = handler;
            if (_bridgeCallback == null)
            {
                _bridgeCallback = BridgeCallback;
                if (!_self.IsAllocated) _self = GCHandle.Alloc(this);
                _InSetCallback(this, _bridgeCallback, GCHandle.ToIntPtr(_self));
            }
        }
        else
        {
            if (_bridgeCallback != null)
            {
                _bridgeCallback = null;
                _InCancelCallback(this);
                if (_self.IsAllocated) _self.Free();
            }
            _messageReceived = null;
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
