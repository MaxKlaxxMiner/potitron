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

        //Console.WriteLine(" - mul: {0:N2}", lightMulti);
      }
    }

    static void Main()
    {
      // TestMetaInfos();

      TestBright();
    }
  }
}
