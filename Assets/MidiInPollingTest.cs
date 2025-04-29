using UnityEngine;
using System.Collections.Generic;
using RtMidi;
using System;

sealed class MidiInPollingTest : MonoBehaviour
{
    #region Private members

    MidiIn _probe;
    List<(MidiIn dev, string name)> _ports = new List<(MidiIn, string)>();

    void ScanPorts()
    {
        for (var i = 0; i < _probe.PortCount; i++)
        {
            var (dev, name) = (MidiIn.Create(), _probe.GetPortName(i));
            dev.OpenPort(i);
            _ports.Add((dev, name));
            Debug.Log($"MIDI-in port opened: {name}");
        }
    }

    void DisposePorts()
    {
        foreach (var p in _ports) p.dev?.Dispose();
        _ports.Clear();
    }

    void UpdatePort(MidiIn dev, string name)
    {
        if (dev == null) return;
        unsafe
        {
            var buffer = (Span<byte>)(stackalloc byte[32]);
            double time;
            while (true)
            {
                var read = dev.GetMessage(buffer, out time);
                if (read.Length == 0) return;
                ProcessMessages(read, name);
            }
        }
    }

    void ProcessMessages(ReadOnlySpan<byte> msg, string name)
    {
        var status = (byte)(msg[0] >> 4);
        var channel = (byte)(msg[0] & 0xf);
        var (d1, d2) = (msg[1], msg[2]);

        var text = status switch
        {
            0x8 => $"Note Off {d1}",
            0x9 => msg[2] > 0 ? $"Note On {d1} ({d2})" : $"Note Off {d1}",
            0xb => $"CC {d1} ({d2})",
            _ => null
        };

        if (text != null) Debug.Log($"{name} [{channel}] {text}");
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
      => _probe = MidiIn.Create();

    void Update()
    {
        if (_ports.Count != _probe.PortCount)
        {
            DisposePorts();
            ScanPorts();
        }

        foreach (var p in _ports) UpdatePort(p.dev, p.name);
    }

    void OnDestroy()
    {
        _probe?.Dispose();
        DisposePorts();
    }

    #endregion
}
