namespace arduino_audio
{
  /// <summary>
  /// Typ des Tones
  /// </summary>
  public enum ToneType
  {
    /// <summary>
    /// einfacher Sinus-Ton
    /// </summary>
    Sine = 0,
    /// <summary>
    /// einfacher Dreieck-Ton
    /// </summary>
    Triangle,
    /// <summary>
    /// einfacher Sägezahn-Ton
    /// </summary>
    Saw,

    /// <summary>
    /// einfacher Rechteck-Ton
    /// </summary>
    Square,
    /// <summary>
    /// zweifach überlagerter Rechteck-Ton mit Verstimmung
    /// </summary>
    Square2Tune,
    /// <summary>
    /// zweifach überlagerter Rechteck-Ton mit doppelter Höhe
    /// </summary>
    Square2Double,
    /// <summary>
    /// zweifach überlagerter Rechteck-Ton mit doppelter Höhe und Verstimmung
    /// </summary>
    Square2DoubleTune,
    /// <summary>
    /// dreifach überlagerter Rechteck-Ton mit doppelter Höhe und Verstimmung
    /// </summary>
    Square3DoubleTune,
  }
}
