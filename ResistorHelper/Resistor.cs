using System;
using System.Collections.Generic;
using System.Globalization;

namespace ResistorHelper
{
  /// <summary>
  /// Struktur eines Widerstandes
  /// </summary>
  public partial class Resistor : Consts
  {
    /// <summary>
    /// merkt sich den real gemessenen Widerstandswert in milli-Ohm
    /// </summary>
    internal readonly long valueMilliOhm;

    /// <summary>
    /// merkt sich den Widerstandswert in milli-Ohm, welcher laut Beschriftung zutreffen sollte (z.B. nach der zugeordneter E24-Reihe)
    /// </summary>
    readonly long targetMilliOhm;

    /// <summary>
    /// selbstgewählte Bezeichnung einer Gruppe/Klasse und Nummer des Widerstands
    /// </summary>
    internal string ident;

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="valueMilliOhm">echter Widerstandswert in milli-Ohm</param>
    /// <param name="targetMilliOhm">optional: Widerstands, welcher laut Beschriftung zutreffen sollte</param>
    /// <param name="ident">optional: selbstgewählte Bezeichnung einer Gruppe/Klasse und Nummer des Widerstands</param>
    public Resistor(long valueMilliOhm, long targetMilliOhm = 0, string ident = null)
    {
      this.valueMilliOhm = valueMilliOhm;
      if (targetMilliOhm == 0) targetMilliOhm = SearchNearestEValue(valueMilliOhm, EValues[3]);
      this.targetMilliOhm = targetMilliOhm;
      this.ident = ident;
    }

    /// <summary>
    /// gibt den Wert als lesbare Zeichenfolge zurück
    /// </summary>
    /// <returns>lesbare Zeichenfolge</returns>
    public override string ToString()
    {
      return TxtValue(TackLifeOhm, targetMilliOhm).PadLeft(7) + "Ω (" + TxtValue(TackLifeOhm, valueMilliOhm).PadLeft(7) + "Ω)";
    }

    /// <summary>
    /// gibt den Wert als kürzere Zeichenfolge zurück (8 Zeichen Länge)
    /// </summary>
    /// <returns>lesbare Zeichenfolge</returns>
    public virtual string ToStringSimple()
    {
      return TxtValue(TackLifeOhm, valueMilliOhm).PadLeft(7) + "Ω";
    }

    /// <summary>
    /// sucht nach dem passensten Wert innerhalb einer E-Reihe
    /// </summary>
    /// <param name="val">Wert, welcher gesucht werden soll</param>
    /// <param name="eValues">E-Reihe, welche verwendet werden soll</param>
    /// <returns>fertiges fixes Ergebnis</returns>
    static long SearchNearestEValue(long val, KeyValuePair<string, short[]> eValues)
    {
      long nearestValue = 0;
      double nearestDif = 1000000.0;
      long m = 1;
      for (int i = 0; i < 16; i++)
      {
        foreach (var eV in eValues.Value)
        {
          long v = m * eV;
          double dif = val < v ? v / (double)val : val / (double)v;
          if (dif < nearestDif)
          {
            nearestDif = dif;
            nearestValue = v;
          }
        }
        m *= 10;
      }

      return nearestValue;
    }

    /// <summary>
    /// macht eine Zeichenfolge TSV-kompatibel
    /// </summary>
    /// <param name="value">Wert, welcher umgewandelt werden soll</param>
    /// <returns>fertig umgewandelter Wert</returns>
    static string EncodeTsvValue(string value)
    {
      return (value ?? "").Replace("\\", "\\\\").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t");
    }

    /// <summary>
    /// wnadelt die Maskierungen eines TSV-Wertes wieder zurück
    /// </summary>
    /// <param name="value">Wert, welcher umgewandelt werden soll</param>
    /// <returns>fertig umgewandelter Wert</returns>
    static string DecodeTsvValue(string value)
    {
      return value.Replace("\\t", "\t").Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\\\", "\\");
    }

    /// <summary>
    /// gibt den Inhalt als TSV-Zeile zurück
    /// </summary>
    /// <returns>TSV-Inhalt</returns>
    public string ToTsv()
    {
      return valueMilliOhm + "\t" + targetMilliOhm + "\t" + EncodeTsvValue(ident);
    }

    /// <summary>
    /// wandelt eine TSV-Zeile in ein Datensatz um
    /// </summary>
    /// <param name="line">TSV-Zeile, welche umgewandelt werden soll</param>
    /// <returns>fertiger Datensatz</returns>
    public static Resistor FromTsv(string line)
    {
      var sp = line.Split('\t');

      return new Resistor(long.Parse(sp[0]), long.Parse(sp[1]), DecodeTsvValue(sp[2]));
    }

    /// <summary>
    /// liest eine Zeichenkette ein und gibt den entsprechenden Widerstandswert zurück (oder null, wenn der Wert nicht gelesen werden kann)
    /// </summary>
    /// <param name="val">Wert, welcher eingelesen werden soll</param>
    /// <returns>eingelesener Wert oder null, wenn der wert nicht lesbar war</returns>
    public static Resistor Parse(string val)
    {
      try
      {
        string number = "";
        string prefix = "";

        int p = 0;
        val = val.Trim();
        for (; p < val.Length; p++)
        {
          if (!char.IsDigit(val[p]) && val[p] != '.' && val[p] != ',') break;
          number += val[p].ToString();
        }
        long result = (long)(double.Parse(number.Replace(',', '.'), CultureInfo.InvariantCulture) * 1000.0 + 0.5);

        prefix = val.Substring(p, val.Length - p).ToLower().Replace("ohm", "").Replace("Ω", "").Trim();

        switch (prefix)
        {
          case "": break;
          case "k": result *= 1000; break;
          case "m": result *= 1000000; break;
          default: throw new Exception("unknown prefix \"" + prefix + "\"");
        }

        return new Resistor(result);
      }
      catch
      {
        return null;
      }
    }
  }
}
