using UnityEngine;
using UnityEngine.UIElements;

public sealed class Monitor : MonoBehaviour
{
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        root.Q("system-text").dataSource = FindFirstObjectByType<MidiSystemTest>();
        root.Q("input-text").dataSource = FindFirstObjectByType<MidiInTest>();
        root.Q("output-text").dataSource = FindFirstObjectByType<MidiOutTest>();
    }
}
