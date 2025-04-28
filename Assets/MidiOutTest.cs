using UnityEngine;
using System.Collections.Generic;
using RtMidi;
using System;

sealed class MidiOutTest : MonoBehaviour
{
    #region Private members

    MidiOut _probe;
    List<(MidiOut dev, string name)> _ports = new List<(MidiOut, string)>();

    // Port name check
    bool IsRealPort(string name)
      => !name.Contains("Through") && !name.Contains("RtMidi");

    void ScanPorts()
    {
        for (var i = 0; i < _probe.PortCount; i++)
        {
            var name = _probe.GetPortName(i);
            Debug.Log("MIDI-out port found: " + name);

            var dev = new MidiOut();
            dev.OpenPort(i);
            _ports.Add((dev, name));
        }
    }

    void DisposePorts()
    {
        foreach (var p in _ports) p.dev?.Dispose();
        _ports.Clear();
    }

    unsafe void SendNoteOn(MidiOut dev, int channel, int note, int velocity)
      => dev.SendMessage((ReadOnlySpan<byte>)
           stackalloc byte[] {(byte)(0x90 + channel), (byte)note, (byte)velocity});

    unsafe void SendNoteOff(MidiOut dev, int channel, int note)
      => dev.SendMessage((ReadOnlySpan<byte>)
           stackalloc byte[] {(byte)(0x80 + channel), (byte)note, (byte)64});

    unsafe void SendAllOff(MidiOut dev, int channel)
      => dev.SendMessage((ReadOnlySpan<byte>)
           stackalloc byte[] {(byte)(0xb0 + channel), 120, 0});

    #endregion

    #region MonoBehaviour implementation

    async Awaitable Start()
    {
        _probe = new MidiOut();

        await Awaitable.WaitForSecondsAsync(0.1f);

        foreach (var port in _ports) if (port.dev != null) SendAllOff(port.dev, 0);

        for (var i = 0; true; i++)
        {
            var note = 40 + (i % 30);

            Debug.Log("MIDI Out: Note On " + note);
            foreach (var port in _ports) if (port.dev != null) SendNoteOn(port.dev, 0, note, 100);

            await Awaitable.WaitForSecondsAsync(0.1f);

            Debug.Log("MIDI Out: Note Off " + note);
            foreach (var port in _ports) if (port.dev != null) SendNoteOff(port.dev, 0, note);

            await Awaitable.WaitForSecondsAsync(0.1f);
        }
    }

    void Update()
    {
        if (_ports.Count != _probe.PortCount)
        {
            DisposePorts();
            ScanPorts();
        }
    }

    void OnDestroy()
    {
        _probe?.Dispose();
        DisposePorts();
    }

    #endregion
}
