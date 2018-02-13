
namespace ResistorHelper
{
  /// <summary>
  /// Struktur eines Widerstandes
  /// </summary>
  public class Resistor : Consts
  {
    /// <summary>
    /// merkt sich den real gemessenen Widerstandswert in milli-Ohm
    /// </summary>
    public long valueMilliOhm;

    /// <summary>
    /// merkt sich den Widerstandswert in milli-Ohm, welcher laut Beschriftung zutreffen sollte (z.B. nach der zugeordneter E24-Reihe)
    /// </summary>
    public long targetMilliOhm;

    /// <summary>
    /// selbstgewählte Bezeichnung einer Gruppe/Klasse (z.B. "2K4 (1)" für einen 10er Streifen von 2,4 kOhm Widerständen)
    /// </summary>
    public string identClass;

    /// <summary>
    /// selbstgewählte Nummer/Bezeichnung innerhalb einer Klasse (z.B. "3" für 3. Widerstand eines 10er Streifens)
    /// </summary>
    public string identNumber;

    /// <summary>
    /// gibt den Wert als lesbare Zeichenfolge zurück
    /// </summary>
    /// <returns>lesbare Zeichenfolge</returns>
    public override string ToString()
    {
      return TxtValue(TackLifeOhm, valueMilliOhm) + "Ω";
    }
  }
}
