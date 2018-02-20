
using System.Linq;

namespace ResistorHelper
{
  /// <summary>
  /// Struktur einer Widerstands-Kombination, welche seriell oder angeordnet wurde
  /// </summary>
  public sealed class ResistorCombined : Resistor
  {
    /// <summary>
    /// merkt sich die kombinierten Widerständ
    /// </summary>
    readonly Resistor[] resistors;
    /// <summary>
    /// merkt sich, ob die Widerstände seriell angeordnet wurden (false = parallel)
    /// </summary>
    readonly bool serial;

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="serial">gibt an, ob die Widerstände seriell angeordnet wurden (false = parallel)</param>
    /// <param name="resistors">Widerständ, welche kombiniert wurden</param>
    public ResistorCombined(bool serial, params Resistor[] resistors)
      : base(serial ? resistors.Sum(r => r.valueMilliOhm) : (long)(1.0 / (resistors.Sum(r => 1.0 / r.valueMilliOhm))))
    {
      this.resistors = resistors;
      this.serial = serial;
    }

    /// <summary>
    /// gibt den Inhalt als lesbare Zeichenkette aus
    /// </summary>
    /// <returns>lesbare Zeichenkette</returns>
    public override string ToString()
    {
      return "[" + (serial ? string.Join(" + ", resistors.Select(r => r.ToString())) : string.Join(" || ", resistors.Select(r => r.ToString()))) + "]";
    }

    /// <summary>
    /// gibt den Wert als kürzere Zeichenfolge zurück (8 Zeichen Länge)
    /// </summary>
    /// <returns>lesbare Zeichenfolge</returns>
    public override string ToStringSimple()
    {
      return "[" + (serial ? string.Join(" + ", resistors.Select(r => r.ToStringSimple())) : string.Join(" || ", resistors.Select(r => r.ToStringSimple()))) + "]";
    }
  }
}
