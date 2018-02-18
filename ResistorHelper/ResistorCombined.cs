
namespace ResistorHelper
{
  /// <summary>
  /// Struktur einer Widerstands-Kombination, welche seriell oder angeordnet wurde
  /// </summary>
  public class ResistorCombined : Resistor
  {
    /// <summary>
    /// merkt sich den ersten Widerstand
    /// </summary>
    readonly Resistor resistor1;
    /// <summary>
    /// merkt sich den zweiten Widerstand
    /// </summary>
    readonly Resistor resistor2;
    /// <summary>
    /// merkt sich, ob die Widerstände seriell angeordnet wurden (false = parallel)
    /// </summary>
    readonly bool serial;

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="resistor1">erster Widerstand</param>
    /// <param name="resistor2">zweiter Widerstand</param>
    /// <param name="serial">gibt an, ob die Widerstände seriell angeordnet wurden (false = parallel)</param>
    public ResistorCombined(Resistor resistor1, Resistor resistor2, bool serial)
      : base(serial ? resistor1.valueMilliOhm + resistor2.valueMilliOhm : (long)(1.0 / (1.0 / resistor1.valueMilliOhm + 1.0 / resistor2.valueMilliOhm)))
    {
      this.resistor1 = resistor1;
      this.resistor2 = resistor2;
      this.serial = serial;
    }

    /// <summary>
    /// gibt den Inhalt als lesbare Zeichenkette aus
    /// </summary>
    /// <returns>lesbare Zeichenkette</returns>
    public override string ToString()
    {
      return "[" + (serial ? resistor1 + " + " + resistor2 : resistor1 + " || " + resistor2) + "]";
    }
  }
}
