using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightScanner
{
  public static class ImageExtensions
  {
    public static int GetIsoRating(this Image image)
    {
      var p = image.GetPropertyItem(0x8827);

      return BitConverter.ToInt16(p.Value, 0);
    }

    public static Tuple<int, int> GetExposureTime(this Image image)
    {
      var p = image.GetPropertyItem(0x829a);

      return new Tuple<int, int>(BitConverter.ToInt32(p.Value, 0), BitConverter.ToInt32(p.Value, 4));
    }
  }
}
