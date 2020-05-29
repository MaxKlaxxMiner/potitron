// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
namespace arduino_audio
{
  /// <summary>
  /// einfacher Tiefpass-Filter
  /// </summary>
  public struct FilterTiefpass
  {
    /// <summary>
    /// merkt sich den zu benutzenden Faktor zwischen 0 und 1 (niedriger = stärkerer Filter)
    /// </summary>
    public double faktor;
    /// <summary>
    /// letzter berechneter Wert
    /// </summary>
    public double wert;

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="faktor">pro Sample zwischen 0 und 1 (niedriger = stärkerer Filter)</param>
    /// <param name="wert">optionaler Startwert</param>
    public FilterTiefpass(double faktor, double wert = 0.0)
    {
      this.faktor = faktor;
      this.wert = wert;
    }

    /// <summary>
    /// berechnet einen neuen Wert durch den Filter und gibt den neuen Wert zurück
    /// </summary>
    /// <param name="wert">eingehender Wert</param>
    /// <returns>ausgehender Wert</returns>
    public double Next(double wert)
    {
      double dif = wert - this.wert;
      this.wert += dif * faktor;
      return this.wert;
    }
  }
}
