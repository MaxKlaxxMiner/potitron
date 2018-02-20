
using System.Globalization;

namespace ResistorHelper
{
  /// <summary>
  /// Struktur eines Ergebnisses einer Widerstands-Suche
  /// </summary>
  public sealed class ResistorResult : Consts
  {
    /// <summary>
    /// merkt sich den entsprechenden Widerstand bzw. eine Kombination
    /// </summary>
    readonly Resistor resultResistor;

    /// <summary>
    /// merkt sich den gesamt berechneten Widerstandswert in milli-Ohm
    /// </summary>
    readonly long resultMilliOhm;

    /// <summary>
    /// merkt sich den Fehler-Abstand in milli-Ohm
    /// </summary>
    public readonly long errorMilliOhm;

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="resultResistor">Widerstand oder eine Kombination, welcher als Ergebnis gefunden wurde</param>
    /// <param name="errorMilliOhm">Fehler-Abstand in milli-Ohm</param>
    public ResistorResult(Resistor resultResistor, long errorMilliOhm)
    {
      this.resultResistor = resultResistor;
      resultMilliOhm = resultResistor.valueMilliOhm;
      this.errorMilliOhm = errorMilliOhm;
    }

    /// <summary>
    /// gibt den Wert als lesbare Zeichenfolge zurück
    /// </summary>
    /// <returns>lesbare Zeichenfolge</returns>
    public override string ToString()
    {
      return TxtValue(TackLifeOhm, resultMilliOhm).PadLeft(7) + "Ω " +
             "(err:" + (100.0 / resultMilliOhm * errorMilliOhm).ToString("0.000", CultureInfo.InvariantCulture).PadLeft(7) + " %)" +
             " = " + resultResistor.ToStringSimple();
    }
  }
}
