// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable UnusedMember.Global
using System;

namespace arduino_audio
{
  /// <summary>
  /// einfacher Tiefpass-Filter dritter Ordnung
  /// </summary>
  public struct FilterTiefpassO3
  {
    /// <summary>
    /// merkt sich den zu benutzenden Faktor zwischen 0 und 1 (niedriger = stärkerer Filter)
    /// </summary>
    public double faktor;
    /// <summary>
    /// erster berechneter Wert
    /// </summary>
    public double wertA;
    /// <summary>
    /// zweiter berechneter Wert
    /// </summary>
    public double wertB;
    /// <summary>
    /// dritter berechneter Wert
    /// </summary>
    public double wertC;

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="faktor">pro Sample zwischen 0 und 1 (niedriger = stärkerer Filter)</param>
    /// <param name="wert">optionaler Startwert</param>
    public FilterTiefpassO3(double faktor, double wert = 0.0)
    {
      this.faktor = Math.Min(1.0, Math.Max(0.00001, faktor));
      wertA = wert;
      wertB = wert;
      wertC = wert;
    }

    /// <summary>
    /// berechnet einen neuen Wert durch den Filter und gibt den neuen Wert zurück
    /// </summary>
    /// <param name="wert">eingehender Wert</param>
    /// <returns>ausgehender Wert</returns>
    public double Next(double wert)
    {
      double dif = wert - wertA;
      wertA += dif * faktor;

      dif = wertA - wertB;
      wertB += dif * faktor;

      dif = wertB - wertC;
      wertC += dif * faktor;

      return wertC;
    }
  }
}
