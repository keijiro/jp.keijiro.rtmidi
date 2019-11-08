using UnityEngine;
using System.Collections.Generic;
using RtMidi.LowLevel;

sealed class MidiInTest : MonoBehaviour
{
    #region Private members

    MidiProbe _probe;
    List<MidiInPort> _ports = new List<MidiInPort>();

    // Does the port seem real or not?
    // This is mainly used on Linux (ALSA) to filter automatically generated
    // virtual ports.
    bool IsRealPort(string name)
    {
        return !name.Contains("Through") && !name.Contains("RtMidi");
    }

    // Scan and open all the available output ports.
    void ScanPorts()
    {
        for (var i = 0; i < _probe.PortCount; i++)
        {
            var name = _probe.GetPortName(i);
            Debug.Log("MIDI-in port found: " + name);

            _ports.Add(IsRealPort(name) ? new MidiInPort(i)
                {
                    OnNoteOn = (byte channel, byte note, byte velocity) =>
                        Debug.Log(string.Format("{0} [{1}] On {2} ({3})", name, channel, note, velocity)),

                    OnNoteOff = (byte channel, byte note) =>
                        Debug.Log(string.Format("{0} [{1}] Off {2}", name, channel, note)),

                    OnControlChange = (byte channel, byte number, byte value) =>
                        Debug.Log(string.Format("{0} [{1}] CC {2} ({3})", name, channel, number, value))
                } : null
            );
        }
    }

    // Close and release all the opened ports.
    void DisposePorts()
    {
        foreach (var p in _ports) p?.Dispose();
        _ports.Clear();
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        _probe = new MidiProbe(MidiProbe.Mode.In);
    }

    void Update()
    {
        // Rescan when the number of ports changed.
        if (_ports.Count != _probe.PortCount)
        {
            DisposePorts();
            ScanPorts();
        }

        // Process queued messages in the opened ports.
        foreach (var p in _ports) p?.ProcessMessages();
    }

    void OnDestroy()
    {
        _probe?.Dispose();
        DisposePorts();
    }

    #endregion
}
