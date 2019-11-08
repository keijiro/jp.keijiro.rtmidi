using Marshal = System.Runtime.InteropServices.Marshal;
using RtMidiDll = RtMidi.Unmanaged;

namespace RtMidi.LowLevel
{
    unsafe public sealed class MidiProbe : System.IDisposable
    {
        public enum Mode { In, Out }

        RtMidiDll.Wrapper* _rtmidi;
        Mode _mode;

        public MidiProbe(Mode mode)
        {
            if (mode == Mode.In)
                _rtmidi = RtMidiDll.InCreateDefault();
            else // mode == Mode.Out
                _rtmidi = RtMidiDll.OutCreateDefault();

            _mode = mode;

            if (_rtmidi == null || !_rtmidi->ok)
                throw new System.InvalidOperationException("Failed to create a MIDI client.");
        }

        ~MidiProbe()
        {
            if (_rtmidi == null || !_rtmidi->ok) return;

            if (_mode == Mode.In)
                RtMidiDll.InFree(_rtmidi);
            else // _mode == Mode.Out
                RtMidiDll.OutFree(_rtmidi);
        }

        public void Dispose()
        {
            if (_rtmidi == null || !_rtmidi->ok) return;

            if (_mode == Mode.In)
                RtMidiDll.InFree(_rtmidi);
            else // _mode == Mode.Out
                RtMidiDll.OutFree(_rtmidi);

            _rtmidi = null;

            System.GC.SuppressFinalize(this);
        }

        public int PortCount {
            get {
                if (_rtmidi == null || !_rtmidi->ok) return 0;
                return (int)RtMidiDll.GetPortCount(_rtmidi);
            }
        }

        public string GetPortName(int port)
        {
            if (_rtmidi == null || !_rtmidi->ok) return null;
            return Marshal.PtrToStringAnsi(RtMidiDll.GetPortName(_rtmidi, (uint)port));
        }
    }
}
