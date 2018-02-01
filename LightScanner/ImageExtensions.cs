using System;
using System.Drawing;

namespace LightScanner
{
  /// <summary>
  /// Erweiterungsmethoden für Bilder
  /// </summary>
  public static class ImageExtensions
  {
    /// <summary>
    /// gibt die ISO-Empfindlichkeit zurück, mit welcher das Foto ursprünglich aufgenommen wurde (z.B. 400)
    /// </summary>
    /// <param name="image">Bild, welches abgefragt werden soll</param>
    /// <returns>entsprechende ISO-Rate</returns>
    public static int GetIsoRating(this Image image)
    {
      var p = image.GetPropertyItem(0x8827);

      return BitConverter.ToInt16(p.Value, 0);
    }

    /// <summary>
    /// gibt die Belichtungszeit zurück, mit welcher das Foto ursprünglich aufgenommen wurde (z.B. 1 / 250 = 4 ms)
    /// </summary>
    /// <param name="image">Bild, welches abgefragt werden soll</param>
    /// <returns>entsprechende Belichtungszeit</returns>
    public static Tuple<int, int> GetExposureTime(this Image image)
    {
      var p = image.GetPropertyItem(0x829a);

      return new Tuple<int, int>(BitConverter.ToInt32(p.Value, 0), BitConverter.ToInt32(p.Value, 4));
    }

    /// <summary>
    /// erstellt ein neues Simplebitmap anhand eines vorhandenen Bildes
    /// </summary>
    /// <param name="image">Bild, welches ausgelesen werden soll</param>
    /// <returns>fertig erstelltes SimpleBitmap</returns>
    public static SimpleBitmap ToSimpleBitmap(this Image image)
    {
      return SimpleBitmap.FromImage(image);
    }
  }
}
