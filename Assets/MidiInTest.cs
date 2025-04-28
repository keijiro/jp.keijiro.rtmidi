using UnityEngine;
using System.Collections.Generic;
using RtMidi;
using System;

sealed class MidiInTest : MonoBehaviour
{
    #region Private members

    MidiIn _probe;
    List<(MidiIn dev, string name)> _ports = new List<(MidiIn, string)>();

    // Port name check
    bool IsRealPort(string name)
      => !name.Contains("Through") && !name.Contains("RtMidi");

    void ScanPorts()
    {
        for (var i = 0; i < _probe.PortCount; i++)
        {
            var name = _probe.GetPortName(i);
            Debug.Log("MIDI-in port found: " + name);

            var dev = new MidiIn();
            dev.OpenPort(i);
            _ports.Add((dev, name));
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

        if (status == 9)
        {
            if (msg[2] > 0)
                Debug.Log(string.Format("{0} [{1}] On {2} ({3})", name, channel, msg[1], msg[2]));
            else
                Debug.Log(string.Format("{0} [{1}] Off {2}", name, channel, msg[1]));
        }
        else if (status == 8)
        {
            Debug.Log(string.Format("{0} [{1}] Off {2}", name, channel, msg[1]));
        }
        else if (status == 0xb)
        {
            Debug.Log(string.Format("{0} [{1}] CC {2} ({3})", name, channel, msg[1], msg[2]));
        }
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
      => _probe = new MidiIn();

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
