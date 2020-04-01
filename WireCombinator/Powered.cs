
namespace WireCombinator
{
  /// <summary>
  /// Wert, welcher ein Pin annehmen kann
  /// </summary>
  public enum Powered
  {
    /// <summary>
    /// Strom auf High (z.B. 3 Volt)
    /// </summary>
    High,
    /// <summary>
    /// Strom auf Low (0 Volt)
    /// </summary>
    Low,
    /// <summary>
    /// Strom auf hohen Widerstand bzw. getrennt (nur wenn UseCharlieplexing = true)
    /// </summary>
    Off
  }
}
