using UnityEngine;
using System.Collections.Generic;

sealed class MidiOutTest : MonoBehaviour
{
    MidiProbe _probe;
    List<MidiOutPort> _ports = new List<MidiOutPort>();

    void ScanPorts()
    {
        for (var i = 0; i < _probe.PortCount; i++)
        {
            var name = _probe.GetPortName(i);
            Debug.Log("MIDI-out port found: " + name);
            _ports.Add(new MidiOutPort(i));
        }
    }

    void DisposePorts()
    {
        foreach (var p in _ports) p.Dispose();
        _ports.Clear();
    }

    System.Collections.IEnumerator Start()
    {
        _probe = new MidiProbe(MidiProbe.Mode.Out);

        ScanPorts();

        yield return new WaitForSeconds(0.1f);

        Debug.Log("MIDI All Sound Off");
        foreach (var port in _ports) port.SendAllOff(0);

        for (var i = 0; true; i++)
        {
            var note = 40 + (i % 30);

            Debug.Log("MIDI Out: Note On " + note);
            foreach (var port in _ports) port.SendNoteOn(0, note, 100);

            yield return new WaitForSeconds(0.1f);

            Debug.Log("MIDI Out: Note Off " + note);
            foreach (var port in _ports) port.SendNoteOff(0, note);

            yield return new WaitForSeconds(0.1f);
        }
    }

    void Update()
    {
        if (_ports.Count != _probe.PortCount)
        {
            // Rescan
            DisposePorts();
            ScanPorts();
        }
    }

    void OnDestroy()
    {
        _probe?.Dispose();
        DisposePorts();
    }
}

