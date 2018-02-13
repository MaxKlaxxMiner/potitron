
namespace ResistorHelper
{
  /// <summary>
  /// Struktur eines darstellbaren Bereiches des Multimeters
  /// </summary>
  public struct MultimeterValue
  {
    /// <summary>
    /// zeigt auf den Prefix (z.B. 7 = kilo, 8 = mega usw.)
    /// </summary>
    public readonly int prefixIndex;
    /// <summary>
    /// maximaler Gesamtwert, welcher dargestellt werden kann (z.B. 6000000)
    /// </summary>
    public readonly long valueMax;
    /// <summary>
    /// Anzahl der zu verschiebenen Dezimalstellen
    /// </summary>
    public readonly long digitOffset;
    /// <summary>
    /// Anzahl der Dezimalstellen vor dem Komma
    /// </summary>
    public readonly int digitsFirst;
    /// <summary>
    /// Anzahl der Dezimalstellen nach dem Komma
    /// </summary>
    public readonly int digitsLast;

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="prefixIndex">zeigt auf den Prefix (z.B. 7 = kilo, 8 = mega usw.)</param>
    /// <param name="valueMax">maximaler Gesamtwert, welcher dargestellt werden kann (z.B. 6000000)</param>
    /// <param name="digitOffset">Anzahl der zu verschiebenen Dezimalstellen</param>
    /// <param name="digitsFirst">Anzahl der Dezimalstellen vor dem Komma</param>
    /// <param name="digitsLast">Anzahl der Dezimalstellen nach dem Komma</param>
    public MultimeterValue(int prefixIndex, long valueMax, long digitOffset, int digitsFirst, int digitsLast)
    {
      this.prefixIndex = prefixIndex;
      this.valueMax = valueMax;
      this.digitOffset = digitOffset;
      this.digitsFirst = digitsFirst;
      this.digitsLast = digitsLast;
    }
  }
}
