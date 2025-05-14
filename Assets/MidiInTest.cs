using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using Unity.Properties;
using RtMidi;

sealed class MidiInTest : MonoBehaviour
{
    #region Exposed attributes

    [SerializeField] bool _useCallback = false;

    #endregion

    #region Logging

    ConcurrentQueue<string> _logLines = new ConcurrentQueue<string>();

    [CreateProperty]
    public string InfoText => string.Join("\n", _logLines);

    void AddLog(string line)
    {
        _logLines.Enqueue(line);
        string temp;
        while (_logLines.Count > 100) _logLines.TryDequeue(out temp);
    }

    #endregion

    #region MIDI port open/close

    MidiIn _probe;
    List<(MidiIn dev, string name)> _ports = new List<(MidiIn, string)>();

    void OpenAvailablePorts()
    {
        for (var i = 0; i < _probe.PortCount; i++)
        {
            var (dev, name) = (MidiIn.Create(), _probe.GetPortName(i));
            if (_useCallback)
                dev.MessageReceived = (t, msg) => OnMessageReceived(msg, name);
            dev.OpenPort(i);
            _ports.Add((dev, name));
            AddLog($"Port opened: {name}");
        }
    }

    void CloseAllPorts()
    {
        foreach (var p in _ports) p.dev.Dispose();
        _ports.Clear();
    }

    #endregion

    #region MIDI message callback

    void OnMessageReceived(ReadOnlySpan<byte> msg, string name)
    {
        var status = (byte)(msg[0] >> 4);
        var channel = (byte)(msg[0] & 0xf);
        var d1 = msg.Length > 1 ? msg[1] : 0;
        var d2 = msg.Length > 2 ? msg[2] : 0;

        var text = status switch
        {
            0x8 => $"Note-Off {d1}",
            0x9 => msg[2] > 0 ? $"Note-On {d1} ({d2})" : $"Note-Off {d1}",
            0xa => $"Aftertouch {d1} ({d2})",
            0xb => $"CC {d1} ({d2})",
            0xe => $"Pitch-Bend {(d2 * 128 + d1) / 16383.0f}",
            _ => null
        };

        if (text != null) AddLog($"{name} ch:{channel} {text}");
    }

    #endregion

    #region MIDI message polling

    unsafe void PollPort(MidiIn dev, string name)
    {
        var buffer = (Span<byte>)(stackalloc byte[32]);
        double time;
        while (true)
        {
            var read = dev.GetMessage(buffer, out time);
            if (read.Length == 0) return;
            OnMessageReceived(read, name);
        }
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
      => _probe = MidiIn.Create();

    void Update()
    {
        if (_ports.Count != _probe.PortCount)
        {
            CloseAllPorts();
            OpenAvailablePorts();
        }

        if (!_useCallback)
            foreach (var p in _ports) PollPort(p.dev, p.name);
    }

    void OnDestroy()
    {
        CloseAllPorts();
        _probe?.Dispose();
    }

    #endregion
}
