using UnityEngine;
using System.Collections.Generic;

sealed class Test : MonoBehaviour
{
    MidiProbe _probe;
    List<MidiInputPort> _ports = new List<MidiInputPort>();

    void ScanPorts()
    {
        for (var i = 0; i < _probe.PortCount; i++)
        {
            var name = _probe.GetPortName(i);

            _ports.Add(
                new MidiInputPort(i)
                {
                    OnNoteOn = (byte channel, byte note, byte velocity) =>
                        Debug.Log(string.Format("{0} [{1}] On {2} ({3})", name, channel, note, velocity)),

                    OnNoteOff = (byte channel, byte note) =>
                        Debug.Log(string.Format("{0} [{1}] Off {2}", name, channel, note)),

                    OnControlChange = (byte channel, byte number, byte value) =>
                        Debug.Log(string.Format("{0} [{1}] CC {2} ({3})", name, channel, number, value))
                }
            );
        }
    }

    void DisposePorts()
    {
        foreach (var p in _ports) p.Dispose();
        _ports.Clear();
    }

    void Start()
    {
        _probe = new MidiProbe();
        ScanPorts();
    }

    void Update()
    {
        if (_ports.Count != _probe.PortCount)
        {
            // Rescan
            DisposePorts();
            ScanPorts();
        }

        foreach (var p in _ports) p.ProcessMessages();
    }

    void OnDestroy()
    {
        _probe?.Dispose();
        DisposePorts();
    }
}
