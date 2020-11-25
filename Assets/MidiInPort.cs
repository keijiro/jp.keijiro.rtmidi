using RtMidiDll = RtMidi.Unmanaged;

namespace RtMidi.LowLevel
{
    unsafe public sealed class MidiInPort : System.IDisposable
    {
        RtMidiDll.Wrapper* _rtmidi;

        public System.Action<byte, byte, byte> OnNoteOn;
        public System.Action<byte, byte> OnNoteOff;
        public System.Action<byte, byte, byte> OnControlChange;

        public MidiInPort(int portNumber)
        {
            _rtmidi = RtMidiDll.InCreateDefault();

            if (_rtmidi != null && _rtmidi->ok)
                RtMidiDll.OpenPort(_rtmidi, (uint)portNumber, "RtMidi In");

            if (_rtmidi == null || !_rtmidi->ok)
                throw new System.InvalidOperationException("Failed to set up a MIDI input port.");
        }

        ~MidiInPort()
        {
            if (_rtmidi == null || !_rtmidi->ok) return;

            RtMidiDll.InFree(_rtmidi);
        }

        public void Dispose()
        {
            if (_rtmidi == null || !_rtmidi->ok) return;

            RtMidiDll.InFree(_rtmidi);
            _rtmidi = null;

            System.GC.SuppressFinalize(this);
        }

        public void ProcessMessages()
        {
            if (_rtmidi == null || !_rtmidi->ok) return;

            byte* msg = stackalloc byte [32];

            while (true)
            {
                ulong size = 32;
                var stamp = RtMidiDll.InGetMessage(_rtmidi, msg, ref size);
                if (stamp < 0 || size == 0) break;

                var status = (byte)(msg[0] >> 4);
                var channel = (byte)(msg[0] & 0xf);

                if (status == 9)
                {
                    if (msg[2] > 0)
                        OnNoteOn?.Invoke(channel, msg[1], msg[2]);
                    else
                        OnNoteOff?.Invoke(channel, msg[1]);
                }
                else if (status == 8)
                {
                    OnNoteOff?.Invoke(channel, msg[1]);
                }
                else if (status == 0xb)
                {
                    OnControlChange?.Invoke(channel, msg[1], msg[2]);
                }
            }
        }
    }
}
