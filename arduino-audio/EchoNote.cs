// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnassignedField.Global

namespace arduino_audio
{
  /// <summary>
  /// merkt sich eine Echo-Note
  /// </summary>
  public struct EchoNote
  {
    /// <summary>
    /// die gespielte Note (0-127 = Note an, 128-255 = Note aus)
    /// </summary>
    public byte note;
    /// <summary>
    /// zugehörige Lautstärke der Note
    /// </summary>
    public byte volume;
    /// <summary>
    /// Ton-Art
    /// </summary>
    public WaveType waveType;
    /// <summary>
    /// Startzeitpunkt
    /// </summary>
    public uint startMicros;
    /// <summary>
    /// gibt den Inhalt als lesbare Zeichenkette zurück
    /// </summary>
    /// <returns>lesbare Zeichenkette</returns>
    public override string ToString()
    {
      return (new { note = note & 0x7f, noteOn = note < 128, volume, waveType, startMicros }).ToString();
    }
  }
}
