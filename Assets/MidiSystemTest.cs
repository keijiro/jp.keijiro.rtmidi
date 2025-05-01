using System.Linq;
using UnityEngine;
using Unity.Properties;
using RtMidi;

public sealed class MidiSystemTest : MonoBehaviour
{
    [CreateProperty]
    public string InfoText { get; private set; }

    void Start()
    {
        var ver = MidiSystem.GetVersion();
        var apis = MidiSystem.GetCompiledApi().ToArray();

        InfoText =$"RtMidi version: {ver}\nSupported API: " +
          string.Join(",", apis.Select(api => MidiSystem.ApiDisplayName(api)));
    }
}
