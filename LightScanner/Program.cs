using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightScanner
{
  static class Program
  {
    const string TestPhotos = @"..\..\..\testphotos\";

    /// <summary>
    /// Testmethode zum auslesen von Meta-Informationen (ISO-Rate und Belichtungszeit)
    /// </summary>
    static void TestMetaInfos()
    {
      foreach (var file in new DirectoryInfo(TestPhotos).GetFiles("*.jpg"))
      {
        Console.Write("{0} ({1:N1} kByte) ", file.Name, file.Length / 1024.0);
        var pic = Image.FromFile(file.FullName);

        int iso = pic.GetIsoRating();
        var exp = pic.GetExposureTime();
        double expTime = exp.Item1 / (double)exp.Item2;
        double lightMulti = iso * expTime;


        Console.WriteLine(" - mul: {0:N2}", lightMulti);
      }
    }

    static Tuple<int, int, int> BestLight(SimpleBitmap pic, int area)
    {
      int bestX = 0;
      int bestY = 0;
      int bestV = 0;
      for (int y = 0; y < pic.height - area; y++)
      {
        for (int x = 0; x < pic.width - area; x++)
        {
          int v = 0;
          for (int cy = 0; cy < area; cy++)
          {
            for (int cx = 0; cx < area; cx++)
            {
              int pix = pic.pixel[x + cx + (y + cy) * pic.width];
              v += pix & 0xff;
              v += (pix >> 8) & 0xff;
              v += (pix >> 16) & 0xff;
            }
          }
          if (v > bestV)
          {
            bestX = x;
            bestY = y;
            bestV = v;
          }
        }
      }
      return new Tuple<int, int, int>(bestX + area / 2, bestY + area / 2, bestV);
    }

    /// <summary>
    /// Testmethode zum auslesen der Helligkeit in Fotos
    /// </summary>
    static void TestBright()
    {
      foreach (var file in new DirectoryInfo(TestPhotos + "\\Blue").GetFiles("*.JPG"))
      {
        Console.Write("{0} ({1:N1} kByte) ", file.Name, file.Length / 1024.0);
        var pic = Image.FromFile(file.FullName);

        var simple = new SimpleBitmap(pic);

        var light = BestLight(simple, 256);

        Console.WriteLine("{0} x {1} - val: {2:N0}", light.Item1, light.Item2, light.Item3);
      }
    }

    static void Main()
    {
      // TestMetaInfos();

      TestBright();
    }
  }
}
