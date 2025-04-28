using System.Linq;
using UnityEngine;
using RtMidi;

sealed class MidiSystemTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log($"RtMidi version: {MidiSystem.GetVersion()}");

        var apis = MidiSystem.GetCompiledApi().ToArray().
          Select(api => MidiSystem.ApiDisplayName(api));
        Debug.Log("Supported API: " + string.Join(",", apis));
    }
}
