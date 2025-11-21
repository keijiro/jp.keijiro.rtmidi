static class Util
{
    public static bool IsValidPort(string portName)
      => !portName.StartsWith("RtMidi") &&
         !portName.StartsWith("Midi Through");
}
