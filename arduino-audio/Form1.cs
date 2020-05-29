#region # using *.*
// ReSharper disable RedundantUsingDirective
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SlimDX.Multimedia;
using XA = SlimDX.XAudio2;
// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable MemberCanBePrivate.Global
#endregion

namespace arduino_audio
{
  public unsafe partial class Form1 : Form
  {
    const int Samples = 16;
    const int SampleRate = 48000;

    Random rnd = new Random();

    byte NextL()
    {
      return (byte)rnd.Next(128 - 20, 128 + 20);
    }

    byte NextR()
    {
      return (byte)rnd.Next(128 - 20, 128 + 20);
    }

    public struct FilterTiefpass
    {
      /// <summary>
      /// merkt sich den zu benutzenden Faktor zwischen 0 und 1 (niedriger = stärkerer Filter)
      /// </summary>
      public double faktor;
      /// <summary>
      /// letzter berechneter Wert
      /// </summary>
      public double wert;

      /// <summary>
      /// Konstruktor
      /// </summary>
      /// <param name="faktor">pro Sample zwischen 0 und 1 (niedriger = stärkerer Filter)</param>
      /// <param name="wert">optionaler Startwert</param>
      public FilterTiefpass(double faktor, double wert = 0.0)
      {
        this.faktor = faktor;
        this.wert = wert;
      }

      /// <summary>
      /// berechnet einen neuen Wert durch den Filter und gibt den neuen Wert zurück
      /// </summary>
      /// <param name="wert">eingehender Wert</param>
      /// <returns>ausgehender Wert</returns>
      public double Next(double wert)
      {
        double dif = wert - this.wert;
        this.wert += dif * faktor;
        return this.wert;
      }
    }

    FilterTiefpass filterL = new FilterTiefpass(20000.0 / SampleRate);
    FilterTiefpass filterR = new FilterTiefpass(20000.0 / SampleRate);

    const int RollBufferSize = 65536;
    static short[] rollBufferL = new short[RollBufferSize];
    static short[] rollBufferR = new short[RollBufferSize];
    static int rollBufferPos;

    void ReadWave(byte[] buffer)
    {
      fixed (byte* bufferP = buffer)
      {
        var samples = (short*)bufferP;
        for (int i = 0; i < Samples * 2; i += 2)
        {
          int l = (NextL() - 128) * 255;
          int r = (NextR() - 128) * 255;

          l = (short)filterL.Next(l);
          r = (short)filterR.Next(r);

          //todo: filter l
          //todo: mix r und l

          rollBufferL[rollBufferPos] = samples[i + 0] = (short)l;
          rollBufferR[rollBufferPos] = samples[i + 1] = (short)r;
          rollBufferPos++;
          if (rollBufferPos == RollBufferSize) rollBufferPos = 0;
        }
      }
    }

    public Form1()
    {
      InitializeComponent();
      var mainThread = Thread.CurrentThread;
      var audioThread = new Thread(() =>
      {
        var xaDev = new XA.XAudio2();
        xaDev.StartEngine();

        var wf = new WaveFormat
        {
          FormatTag = WaveFormatTag.Pcm,
          Channels = 2,
          BitsPerSample = 16,
          BlockAlignment = 4,
          SamplesPerSecond = SampleRate,
          AverageBytesPerSecond = SampleRate * 4
        };

        // ReSharper disable once ObjectCreationAsStatement
        new XA.MasteringVoice(xaDev, 2, 44100, 0);
        var xaSv = new XA.SourceVoice(xaDev, wf, XA.VoiceFlags.None);
        var xaBuf = new XA.AudioBuffer();

        var data = new byte[Samples * 4];
        ReadWave(data);
        xaBuf.AudioData = new MemoryStream(data, false);
        xaBuf.AudioBytes = data.Length;
        xaSv.SubmitSourceBuffer(xaBuf);

        xaSv.BufferStart += delegate
        {
          if (!mainThread.IsAlive) return;
          ReadWave(data);
          xaBuf.AudioData.Position = 0;
          xaSv.SubmitSourceBuffer(xaBuf);
        };

        xaSv.Start();
        while (mainThread.IsAlive)
        {
          Thread.Sleep(1);
        }
        xaSv.Stop();
      });
      audioThread.Start();
    }

    void Form1_Load(object sender, EventArgs e)
    {

    }

    Bitmap oszBitmap = null;
    bool draw = false;

    void timer1_Tick(object sender, EventArgs e)
    {
      if (draw) return;
      int width = pictureBox1.Width;
      int height = pictureBox1.Height;
      if (width < 32 || height < 32) return;
      draw = true;


      var buffer = new short[width];
      int pos = (rollBufferPos - buffer.Length + RollBufferSize) % RollBufferSize;
      for (int i = 0; i < buffer.Length; i++) buffer[i] = rollBufferL[(pos + i) % RollBufferSize];

      if (oszBitmap == null || oszBitmap.Width != width || oszBitmap.Height != height)
      {
        oszBitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);
        pictureBox1.Image = oszBitmap;
      }

      var g = Graphics.FromImage(oszBitmap);
      g.Clear(Color.Black);
      var pen = new Pen(Color.FromArgb(0x0080ff - 16777216));
      for (int x = 1; x < width; x++)
      {
        g.DrawLine(pen, x - 1, (buffer[x - 1] + 32768) * height / 65536, x, (buffer[x] + 32768) * height / 65536);
      }

      pictureBox1.Refresh();
      draw = false;
    }
  }
}
