using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Properties;
using RtMidi;

sealed class MidiOutTest : MonoBehaviour
{
    #region UI interface

    [CreateProperty]
    public string InfoText => string.Join("\n", _infoLines);

    Queue<string> _infoLines = new Queue<string>();

    void AddLog(string line)
    {
        _infoLines.Enqueue(line);
        while (_infoLines.Count > 100) _infoLines.Dequeue();
    }

    #endregion

    #region Private members

    MidiOut _probe;
    List<(MidiOut dev, string name)> _ports = new List<(MidiOut, string)>();

    void ScanPorts()
    {
        for (var i = 0; i < _probe.PortCount; i++)
        {
            var (dev, name) = (MidiOut.Create(), _probe.GetPortName(i));
            dev.OpenPort(i);
            _ports.Add((dev, name));
            AddLog($"MIDI-out port opened: {name}");
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
        _probe = MidiOut.Create();

        await Awaitable.WaitForSecondsAsync(0.1f);

        foreach (var port in _ports) if (port.dev != null) SendAllOff(port.dev, 0);

        for (var i = 0; true; i++)
        {
            var note = 40 + (i % 30);

            AddLog("Note On " + note);
            foreach (var port in _ports) if (port.dev != null) SendNoteOn(port.dev, 0, note, 100);

            await Awaitable.WaitForSecondsAsync(0.1f);

            AddLog("Note Off " + note);
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
