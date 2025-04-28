using System;
using System.Runtime.InteropServices;

namespace RtMidi {

// Global MIDI functions
public static class MidiSystem
{
    #region Public static methods

    public static string GetVersion()
      => Marshal.PtrToStringAnsi(_GetVersion());

    public static string ApiName(Api api)
      => Marshal.PtrToStringAnsi(_ApiName(api));

    public static string ApiDisplayName(Api api)
      => Marshal.PtrToStringAnsi(_ApiDisplayName(api));

    public static Api CompiledApiByName(string name)
      => CompiledApiByName(name);

    public unsafe static ReadOnlySpan<Api> GetCompiledApi()
    {
        var len = _GetCompiledApi(IntPtr.Zero, 0);
        var buf = new Api[len];
        fixed (Api* p = buf) _GetCompiledApi((IntPtr)p, (uint)len);
        return buf;
    }

    public static void Error(ErrorType type, string errorString)
      => Error(type, errorString);

    #endregion

    #region P/Invoke interface

    [DllImport(Config.DllName, EntryPoint = "rtmidi_get_version")]
    static extern IntPtr _GetVersion();

    [DllImport(Config.DllName, EntryPoint = "rtmidi_api_name")]
    static extern IntPtr _ApiName(Api api);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_api_display_name")]
    static extern IntPtr _ApiDisplayName(Api api);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_compiled_api_by_name")]
    public static extern Api _CompiledApiByName(string name);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_get_compiled_api")]
    public static extern int _GetCompiledApi(IntPtr apis, uint apis_size);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_error")]
    public static extern void _Error(ErrorType type, string errorString);

    #endregion
}

} // namespace RtMidi
