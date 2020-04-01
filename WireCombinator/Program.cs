using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireCombinator
{
  class LinePin
  {
    /// <summary>
    /// gibt die Nummer zum Controller-Pin an
    /// </summary>
    public int controllerPin;

    /// <summary>
    /// gibt Zeilenummer auf dem Breadboard an
    /// </summary>
    public int lineNumber;

    /// <summary>
    /// merkt sich die LED, welche verbunden wurde (oder null, wenn keine LED gesteckt wurde)
    /// </summary>
    public Led led;
  }

  public abstract class Led
  {
  }

  static class Program
  {
    /// <summary>
    /// Anzahl der µ-Controller Pins, welche insgesamt benutzt werden können (mindestens: 2 bei normalen LEDs und 4 bei RGB LEDs)
    /// </summary>
    const int UsePins = 2;

    /// <summary>
    /// gibt an, ob RGB-Leds (mit 4 Anschlüssen) benutzt werden können
    /// </summary>
    const bool UseRgbLeds = false;

    /// <summary>
    /// gibt an, ob auch volles Charlieplexing benutzt werden kann, also das Pins quasi abgeschaltet werden können
    /// </summary>
    const bool UseCharlieplexing = false;

    /// <summary>
    /// gibt die maximale Anzahl der steckbaren Kabeln pro Pin an (wichtig für Charlieplexing etc.)
    /// Default: 4 (normales Breadboard)
    /// </summary>
    const int MaxCablePerPin = 1;

    /// <summary>
    /// gibt die max. Anzahl der LEDs in übereinander einer Spalte an
    /// Default: 4 (normales Breadboard)
    /// </summary>
    const int MaxLedsPerLine = 1;

    static void Main(string[] args)
    {

    }
  }
}
