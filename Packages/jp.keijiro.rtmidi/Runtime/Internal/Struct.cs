using System;
using System.Runtime.InteropServices;

namespace RtMidi {

// RtMidiWrapper struct
[StructLayout(LayoutKind.Sequential)]
struct WrapperStruct
{
    public IntPtr ptr;
    public IntPtr data;
    [MarshalAs(UnmanagedType.U1)]
    public bool ok;
    public IntPtr msg;

    public unsafe static bool IsOk(IntPtr ptr)
      => ((WrapperStruct*)ptr)->ok;

    public unsafe static string GetMessage(IntPtr ptr)
      => Marshal.PtrToStringAnsi(((WrapperStruct*)ptr)->msg);
}

} // namespace RtMidi
