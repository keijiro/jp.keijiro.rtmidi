namespace RtMidi {

// Plugin configuration
static class Config
{
#if UNITY_EDITOR || !UNITY_IOS
    public const string DllName = "RtMidi";
#else
    public const string DllName = "__Internal";
#endif
}

} // namespace RtMidi
