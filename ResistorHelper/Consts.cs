// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.Linq;

namespace ResistorHelper
{
  /// <summary>
  /// Klasse mit verschiedenen Konstanten
  /// </summary>
  public class Consts
  {
    /// <summary>
    /// generiert eine Ex-Reihe der Widerstände
    /// </summary>
    /// <param name="count">Anzahl der zu generierenden Werte</param>
    /// <param name="digits">Anzahl der Dezimalstellen (default: 2 oder 3)</param>
    /// <returns>fertiges Array der entsprechenden Werte</returns>
    static short[] GenExValues(int count, int digits)
    {
      var results = new short[count];
      double val = 1.0;
      double mul = Math.Pow(10.0, 1.0 / count);
      double mx = digits == 2 ? 0.0 : 0.02784;
      double mr = digits == 2 ? 0.459 : 0.4947;
      for (int i = 0; i < count; i++)
      {
        results[i] = (short)(val * Math.Pow(10.0, digits - 1) + (mx / count * i) + mr);
        val *= mul;
      }

      // hack/fixes for E3 to E24
      if (digits == 2 && count <= 24) for (int i = 10; i <= 16; i++) if (i * count % 24 == 0) results[i * count / 24]++;

      return results;
    }

    /// <summary>
    /// Standard-Werte für Widerstände nach ISO-Norm
    /// </summary>
    public static readonly KeyValuePair<string, short[]>[] EValues =
    {
      new KeyValuePair<string, short[]>("E3 (obsolete)", GenExValues(3, 2)),
      new KeyValuePair<string, short[]>("E6 (20% tolerance)", GenExValues(6, 2)),
      new KeyValuePair<string, short[]>("E12 (10% tolerance)", GenExValues(12, 2)),
      new KeyValuePair<string, short[]>("E24 = (5% tolerance, default)", GenExValues(24, 2)),
      new KeyValuePair<string, short[]>("E48 = (2% tolerance)", GenExValues(48, 3)),
      new KeyValuePair<string, short[]>("E96 = (1% tolerance)", GenExValues(96, 3)),
      new KeyValuePair<string, short[]>("E192 = (< 0.5% tolerance)", GenExValues(192, 3))
    };

    /// <summary>
    /// Vorsätze für Maßeinheiten - Symbole
    /// </summary>
    static readonly string[] PrefixSymbol = { "a", "f", "p", "n", "µ", "m", "", "k", "M", "G", "T", "P", "E" };

    /// <summary>
    /// Vorsätze für Maßeinheiten - Namen
    /// </summary>
    static readonly string[] PrefixName = { "atto", "femto", "pico", "nano", "micro", "milli", "", "kilo", "mega", "giga", "tera", "peta", "exa" };

    /// <summary>
    /// Tacklife Standard-Stufen
    /// </summary>
    public static readonly MultimeterValue[] TackLifeOhm =
    {
      new MultimeterValue(6, 600000, 3, 3, 1), // 600 Ohm
      new MultimeterValue(7, 6000000, 6, 1, 3), // 6 kOhm
      new MultimeterValue(7, 60000000, 6, 2, 2), // 60 kOhm
      new MultimeterValue(7, 600000000, 6, 3, 1), // 600 kOhm
      new MultimeterValue(8, 6000000000, 9, 1, 3), // 6 MOhm
      new MultimeterValue(8, 60000000000, 9, 2, 2) // 60 MOhm
    };

    /// <summary>
    /// wandelt einen Wert in eine lesbare Zeichenfolge um
    /// </summary>
    /// <param name="multimeterValues">Werte/Stufen eines Multimeters, welche für die Darstellung benutzt werden sollen</param>
    /// <param name="value">Wert, welcher dargestellt werden soll</param>
    /// <param name="skipZeros">optional: entfernt führende Nullen (default: true)</param>
    /// <param name="prefixFull">optional: gibt für die 1000er-Einheiten die vollständige Beschriftung zurück (z.B. "kilo", "mega")</param>
    /// <returns>fertig lesbare Zeichenfolge</returns>
    public static string TxtValue(MultimeterValue[] multimeterValues, long value, bool skipZeros = true, bool prefixFull = false)
    {
      bool neg = value < 0;
      if (neg) value = -value;

      var m = multimeterValues.FirstOrDefault(x => x.valueMax >= value);
      if (m.valueMax == 0) return "inf "; // Wert, kann im Multimeter nicht dargestellt werden (zu hoch)

      if (m.digitOffset - m.digitsLast > 0)
      {
        long d = 10;
        long r = 5;
        for (int i = 1; i < m.digitOffset - m.digitsLast; i++)
        {
          d *= 10;
          r *= 10;
        }
        value = (value + r) / d;
      }

      string valStr = "";

      long divValue = 1;
      for (int i = 0; i < m.digitsLast; i++) divValue *= 10;

      valStr = (value / divValue).ToString("D" + (skipZeros ? 1 : m.digitsFirst)) + (m.digitsLast > 0 ? "." + (value % divValue).ToString("D" + m.digitsLast) : "");

      return (neg ? "-" : "") + valStr + " " + (prefixFull ? PrefixName[m.prefixIndex] + " " : PrefixSymbol[m.prefixIndex]);
    }
  }
}
