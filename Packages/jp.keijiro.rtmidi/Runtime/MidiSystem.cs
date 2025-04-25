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

    [DllImport(Config.DllName, EntryPoint = "rtmidi_compiled_api_by_name")]
    public static extern Api CompiledApiByName(string name);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_get_compiled_api")]
    public static extern int GetCompiledApi([Out] Api[] apis, uint apis_size);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_error")]
    public static extern void Error(ErrorType type, string errorString);

    #endregion

    #region Unmanaged implementation

    [DllImport(Config.DllName, EntryPoint = "rtmidi_get_version")]
    static extern IntPtr _GetVersion();

    [DllImport(Config.DllName, EntryPoint = "rtmidi_api_name")]
    static extern IntPtr _ApiName(Api api);

    [DllImport(Config.DllName, EntryPoint = "rtmidi_api_display_name")]
    static extern IntPtr _ApiDisplayName(Api api);

    #endregion
}

} // namespace RtMidi
