using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Properties;
using RtMidi;

sealed class MidiOutTest : MonoBehaviour
{
    #region Logging

    Queue<string> _logLines = new Queue<string>();

    [CreateProperty]
    public string InfoText => string.Join("\n", _logLines);

    void AddLog(string line)
    {
        _logLines.Enqueue(line);
        while (_logLines.Count > 100) _logLines.Dequeue();
    }

    #endregion

    #region MIDI port open/close

    MidiOut _probe;
    List<(MidiOut dev, string name)> _ports = new List<(MidiOut, string)>();

    void OpenAvailablePorts()
    {
        for (var i = 0; i < _probe.PortCount; i++)
        {
            var name = _probe.GetPortName(i);
            if (!Util.IsValidPort(name))
            {
                _ports.Add((null, name));
                continue;
            }

            var dev = MidiOut.Create();
            dev.ErrorReceived = (t, msg) => OnError(name, t, msg);
            dev.OpenPort(i);

            _ports.Add((dev, name));
            AddLog($"Port opened: {name}");
        }
    }

    void CloseAllPorts()
    {
        foreach (var p in _ports) p.dev?.Dispose();
        _ports.Clear();
    }

    void OnError(string name, ErrorType type, string message)
      => AddLog($"[{name}] Error {type}: {message}");

    #endregion

    #region MIDI sequence sender

    async Awaitable SendMidiSequenceAsync()
    {
        const float interval = 0.1f;
        await Awaitable.WaitForSecondsAsync(interval);

        foreach (var port in _ports)
            SendAllOff(port.dev, 0);

        for (var i = 0; true; i++)
        {
            var note = 40 + (i % 30);

            AddLog("Note-On " + note);
            foreach (var port in _ports)
                SendNoteOn(port.dev, 0, note, 100);

            await Awaitable.WaitForSecondsAsync(interval);

            AddLog("Note-Off " + note);
            foreach (var port in _ports)
                SendNoteOff(port.dev, 0, note);

            await Awaitable.WaitForSecondsAsync(interval);
        }
    }

    unsafe void SendNoteOn(MidiOut dev, int channel, int note, int velocity)
    {
        if (dev == null) return;
        var msg = stackalloc byte[3]
          { (byte)(0x90 + channel), (byte)note, (byte)velocity };
        dev.SendMessage(new ReadOnlySpan<byte>(msg, 3));
    }

    unsafe void SendNoteOff(MidiOut dev, int channel, int note)
    {
        if (dev == null) return;
        var msg = stackalloc byte[3]
          { (byte)(0x80 + channel), (byte)note, 64 };
        dev.SendMessage(new ReadOnlySpan<byte>(msg, 3));
    }

    unsafe void SendAllOff(MidiOut dev, int channel)
    {
        if (dev == null) return;
        var msg = stackalloc byte[3]
          { (byte)(0xb0 + channel), 120, 0 };
        dev.SendMessage(new ReadOnlySpan<byte>(msg, 3));
    }

    #endregion

    #region MonoBehaviour implementation

    async Awaitable Start()
    {
        _probe = MidiOut.Create();
        await SendMidiSequenceAsync();
    }

    void Update()
    {
        if (_ports.Count != _probe.PortCount)
        {
            CloseAllPorts();
            OpenAvailablePorts();
        }
    }

    void OnDestroy()
    {
        CloseAllPorts();
        _probe?.Dispose();
    }

    #endregion
}
