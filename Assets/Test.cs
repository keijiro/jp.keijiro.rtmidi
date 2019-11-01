using UnityEngine;
using System.Collections.Generic;

sealed class Test : MonoBehaviour
{
    MidiProbe _probe;
    List<MidiInputPort> _ports = new List<MidiInputPort>();

    void Start()
    {
        _probe = new MidiProbe();

        for (var i = 0; i < _probe.PortCount; i++)
        {
            Debug.Log(_probe.GetPortName(i));

            _ports.Add(
                new MidiInputPort(i)
                {
                    OnNoteOn = (byte channel, byte note, byte velocity) =>
                        Debug.Log(string.Format("On ({0}) {1} {2}", channel, note, velocity)),

                    OnNoteOff = (byte channel, byte note) =>
                        Debug.Log(string.Format("Off ({0}) {1}", channel, note)),

                    OnControlChange = (byte channel, byte number, byte value) =>
                        Debug.Log(string.Format("CC ({0}) {1} {2}", channel, number, value))
                }
            );
        }
    }

    void Update()
    {
        foreach (var p in _ports) p.ProcessMessages();
    }

    void OnDestroy()
    {
        _probe?.Dispose();
        foreach (var p in _ports) p.Dispose();
    }
}
