
namespace ResistorHelper
{
  /// <summary>
  /// Struktur einer Widerstands-Kombination, welche seriell oder angeordnet wurde
  /// </summary>
  public sealed class ResistorCombined : Resistor
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
    /// <param name="serial">gibt an, ob die Widerstände seriell angeordnet wurden (false = parallel)</param>
    /// <param name="resistor1">erster Widerstand</param>
    /// <param name="resistor2">zweiter Widerstand</param>
    public ResistorCombined(bool serial, Resistor resistor1, Resistor resistor2)
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

    /// <summary>
    /// gibt den Wert als kürzere Zeichenfolge zurück (8 Zeichen Länge)
    /// </summary>
    /// <returns>lesbare Zeichenfolge</returns>
    public override string ToStringSimple()
    {
      return "[" + (serial ? resistor1.ToStringSimple() + " + " + resistor2.ToStringSimple() : resistor1.ToStringSimple() + " || " + resistor2.ToStringSimple()) + "]";
    }
  }
}
