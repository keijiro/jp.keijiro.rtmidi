namespace RtMidi {

// Plugin configuration
static class Config
{
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_WEBGL)
    public const string DllName = "__Internal";
#else
    public const string DllName = "RtMidi";
#endif
}

} // namespace RtMidi
