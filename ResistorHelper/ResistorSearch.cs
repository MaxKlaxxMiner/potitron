using System;
using System.Collections.Generic;

namespace ResistorHelper
{
  /// <summary>
  /// Struktur eines Widerstandes
  /// </summary>
  public partial class Resistor
  {
    /// <summary>
    /// merkt sich den Such-Cache
    /// </summary>
    static KeyValuePair<long, Resistor>[] doubleCache = null;

    /// <summary>
    /// leert den Such-Cache
    /// </summary>
    public static void ResetSearchCache()
    {
      doubleCache = null;
    }

    /// <summary>
    /// ermittelt alle einzel und doppel Kombinationen aller Widerstände und gibt diese Sortiert zurück
    /// </summary>
    /// <param name="allResistors">Liste mit allen Widerständen</param>
    /// <returns>fertig sortierte Liste mit entsprechenden Widerständen</returns>
    static KeyValuePair<long, Resistor>[] GetAllDoubles(Resistor[] allResistors)
    {
      if (doubleCache != null) return doubleCache;

      var result = new KeyValuePair<long, Resistor>[allResistors.Length * allResistors.Length];
      int p = 0;
      foreach (var r in allResistors) result[p++] = new KeyValuePair<long, Resistor>(r.valueMilliOhm, r);

      // --- serielle und parallele Kombinationen sammeln ---
      for (int y = 0; y < allResistors.Length - 1; y++)
      {
        for (int x = y + 1; x < allResistors.Length; x++)
        {
          var rs = new ResistorCombined(true, allResistors[x], allResistors[y]);
          var rp = new ResistorCombined(false, allResistors[x], allResistors[y]);
          result[p++] = new KeyValuePair<long, Resistor>(rs.valueMilliOhm, rs);
          result[p++] = new KeyValuePair<long, Resistor>(rp.valueMilliOhm, rp);
        }
      }

      Array.Sort(result, (x, y) => x.Key.CompareTo(y.Key));

      return doubleCache = result;
    }

    /// <summary>
    /// binäre Suche nach einem bestimmten Wert und gibt die ungefähre Position im Array zurück
    /// </summary>
    /// <param name="sortedResistors">Array mit Widerständen, welches durchsucht werden soll</param>
    /// <param name="searchValue">Wert, welcher gesucht werden soll</param>
    /// <returns>passende Position im Array</returns>
    static int BinarySearch(KeyValuePair<long, Resistor>[] sortedResistors, long searchValue)
    {
      int startPos = 0;
      int endPos = sortedResistors.Length;
      if (endPos == 0) return 0;
      do
      {
        var middlePos = (startPos + endPos) >> 1;
        if (sortedResistors[middlePos].Key > searchValue) endPos = middlePos; else startPos = middlePos;
      } while (endPos - startPos > 1);

      return startPos;
    }

    /// <summary>
    /// Suchmethode, um passende Widerstände bzw. deren Kombinationen zu finden
    /// </summary>
    /// <param name="allResistors">Alle Widerstände, welche zur Verfügung stehen</param>
    /// <param name="searchValue">gesuchter Widerstandswert</param>
    /// <param name="maxResistors">optional: gibt die maximale Anzahl der kombinierbaren Widerstände an, welche verwendet werden dürfen (default: 2)</param>
    /// <param name="maxError">optional: maximale Abweichung des Ergebnisses (default: 1.10 = 10 % Abweichung)</param>
    /// <returns>Enumerable der gefundenen Ergebnisse</returns>
    public static IEnumerable<ResistorResult> Search(Resistor[] allResistors, Resistor searchValue, int maxResistors = 2, double maxError = 1.10)
    {
      if (allResistors.Length < 2) yield break;

      long search = searchValue.valueMilliOhm;
      long max = (long)(search * maxError) - search;

      foreach (var r in allResistors)
      {
        long err = Math.Abs(r.valueMilliOhm - search);
        if (err <= max) yield return new ResistorResult(r, err);
      }

      var fullList = GetAllDoubles(allResistors);

      if (maxResistors >= 2) // 2-teilige Suche
      {
        int p = BinarySearch(fullList, search - max);

        while (p < fullList.Length && fullList[p].Key - search < max)
        {
          yield return new ResistorResult(fullList[p].Value, Math.Abs(search - fullList[p].Key));
          p++;
        }
      }

      if (maxResistors >= 3)
      {
        // --- 3er Kombination - seriell ---
        for (int z = 0; z < allResistors.Length - 2; z++)
        {
          for (int y = z + 1; y < allResistors.Length - 1; y++)
          {
            for (int x = y + 1; x < allResistors.Length; x++)
            {
              long err = Math.Abs(allResistors[x].valueMilliOhm + allResistors[y].valueMilliOhm + allResistors[z].valueMilliOhm - search);
              if (err <= max) yield return new ResistorResult(new ResistorCombined(true, allResistors[x], allResistors[y], allResistors[z]), err);
            }
          }
        }

        // --- 3er Kombination - parallel ---
        for (int z = 0; z < allResistors.Length - 2; z++)
        {
          for (int y = z + 1; y < allResistors.Length - 1; y++)
          {
            for (int x = y + 1; x < allResistors.Length; x++)
            {
              var c = new ResistorCombined(false, allResistors[x], allResistors[y], allResistors[z]);
              long err = Math.Abs(c.valueMilliOhm - search);
              if (err <= max) yield return new ResistorResult(c, err);
            }
          }
        }
      }
    }
  }
}
