using System;
using System.Runtime.InteropServices;
using Debug = UnityEngine.Debug;

namespace RtMidi {

//
// This file provides a bridge mechanism to relay unmanaged RtMidi callback
//
// * Rationale
//
// We can't directly bind closed-instance delegates in RtMidi wrappers to
// unmanaged RtMidi callbacks because IL2CPP only supports static delegates. To
// work around this limitation, we implement a bridge callback mechanism that
// relays invocations from unmanaged code to an interface method.
//
// * Call flow
//
// Unmanaged RtMidi
//   └─→ Bridge.Callback (static delegate)
//         └─→ IListener.OnMessage (interface method)
//               └─→ Wrapper.Handler (open delegate)
//
// * Dependency graph
//
//     ┌────────────────────────┐
//     ↓                        ♢
// Wrapper ♦︎-> Bridge ♦︎-> GCHandle<IListener>
//     ♦︎          ↑             ↑
//     └──→ Unmanaged RtMidi ┄┄┄┘
//

//
// Midi callback (on message received)
//

// Delegate type for unmanaged RtMidi callback
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
delegate void MidiCallback
  (double timeStamp, IntPtr message, nuint messageSize, IntPtr userData);

// Bridge class
sealed class MessageCallbackBridge : IDisposable
{
    #region Constructor and IDisposable

    public MessageCallbackBridge(IListener listener)
      => _listener = GCHandle.Alloc(listener);

    public void Dispose()
    {
        if (_listener.IsAllocated) _listener.Free();
    }

    #endregion

    #region Public properties for callback registration

    public MidiCallback Callback => BridgeCallback;
    public IntPtr UserData => GCHandle.ToIntPtr(_listener);

    #endregion

    #region Interface definition and reference

    public interface IListener
    {
        public void OnMessage(double time, ReadOnlySpan<byte> data);
    }

    GCHandle _listener;

    #endregion

    #region Bridge function (called from unmanaged RtMidi)

    [AOT.MonoPInvokeCallback(typeof(MidiCallback))]
    public unsafe static void BridgeCallback
      (double time, IntPtr ptr, nuint size, IntPtr user)
    {
        var self = (IListener)GCHandle.FromIntPtr(user).Target;
        var span = new ReadOnlySpan<byte>(ptr.ToPointer(), (int)size);
        try
        {
            self.OnMessage(time, span);
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception in MIDI callback: {e}");
        }
    }

    #endregion
}

} // namespace RtMidi
