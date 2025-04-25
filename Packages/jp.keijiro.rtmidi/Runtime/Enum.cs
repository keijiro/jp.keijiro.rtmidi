namespace RtMidi {

// MIDI API specifiers
public enum Api
{
    Unspecified,
    MacOsXCore,
    LinuxAlsa,
    UnixJack,
    WindowsMM,
    RtMidiDummy,
    WebMidiApi,
    WindowsUwp,
    Android
}

// Defined error types
public enum ErrorType
{
    Warning,
    DebugWarning,
    Unspecified,
    NoDevicesFound,
    InvalidDevice,
    MemoryError,
    InvalidParameter,
    InvalidUse,
    DriverError,
    SystemError,
    ThreadError
}

} // namespace RtMidi
