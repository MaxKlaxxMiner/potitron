using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace LightScanner
{
  /// <summary>
  /// einfache Struktur eines Bildes
  /// </summary>
  public class SimpleBitmap
  {
    /// <summary>
    /// gespeicherte Pixel
    /// </summary>
    public readonly int[] pixel;

    /// <summary>
    /// Breite des Bildes in Pixeln
    /// </summary>
    public readonly int width;

    /// <summary>
    /// Höhe des Bildes in Pixeln
    /// </summary>
    public readonly int height;

    /// <summary>
    /// Konstruktor zum erstellen eines neuen Bildes
    /// </summary>
    /// <param name="width">Breite des Bildes in Pixeln</param>
    /// <param name="height">Höhe des Bildes in Pixeln</param>
    public SimpleBitmap(int width, int height)
    {
      this.width = width;
      this.height = height;
      pixel = new int[width * height];
    }

    /// <summary>
    /// Konstruktor zum erstellen eines Bildes anhand eines vorhandenen Bitmap/Image
    /// </summary>
    /// <param name="image">Bild, welches ausgelesen werden soll</param>
    /// <param name="offsetX">optionale X-Startposition in Pixeln (default: 0)</param>
    /// <param name="offsetY">optionale Y-Startposition in Pixeln (default: 0)</param>
    /// <param name="maxWidth">maximale Breite des Bildes in Pixeln (default: 65535)</param>
    /// <param name="maxHeight">maximale Höhe des Bildes in Pixeln</param>
    public SimpleBitmap(Image image, int offsetX = 0, int offsetY = 0, int maxWidth = 65535, int maxHeight = 65535)
    {
      if (image == null) throw new ArgumentNullException("image");
      if (offsetX < 0 || offsetX >= image.Width) throw new ArgumentOutOfRangeException("offsetX");
      if (offsetY < 0 || offsetY >= image.Height) throw new ArgumentOutOfRangeException("offsetY");
      if (maxWidth < 1) throw new ArgumentOutOfRangeException("maxWidth");
      if (maxHeight < 1) throw new ArgumentOutOfRangeException("maxHeight");

      var bitmap = image as Bitmap ?? new Bitmap(image);;

      width = Math.Min(image.Width - offsetX, maxWidth);
      height = Math.Min(image.Height - offsetY, maxHeight);
      pixel = new int[width * height];

      var bData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
      if (bData.Stride == width * sizeof(int)) // Zeilenbreite stimmt genau überein? -> komplettes Bild direkt kopieren
      {
        Marshal.Copy(new IntPtr(bData.Scan0.ToInt64() + offsetY * sizeof(int)), pixel, 0, pixel.Length);
      }
      else // Zeilenbreite stimmt nicht exakt überein -> Zeile für Zeile kopieren
      {
        for (int y = 0; y < height; y++)
        {
          Marshal.Copy(new IntPtr(bData.Scan0.ToInt64() + (offsetY + y) * bData.Stride + offsetX * sizeof(int)), pixel, y * width, width);
        }
      }

      bitmap.UnlockBits(bData);
    }

    /// <summary>
    /// erstellt ein neues Bild anhand eines vorhandenen Bitmap/Image
    /// </summary>
    /// <param name="image">Bild, welches ausgelesen werden soll</param>
    /// <returns>fertig ausgelesenen Bild</returns>
    public static SimpleBitmap FromImage(Image image)
    {
      return new SimpleBitmap(image);
    }
  }
}
