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
    const double TestTon = 220.0;

    FilterTiefpass filterL;
    FilterTiefpass filterR = new FilterTiefpass(10000.0 / SampleRate);

    const int RollBufferSize = 65536;
    static short[] rollBufferL = new short[RollBufferSize];
    static short[] rollBufferR = new short[RollBufferSize];
    static int rollBufferPos;

    static double testTonOfs;

    Random rnd = new Random();

    int sig = 2;

    byte NextL()
    {
      testTonOfs = (testTonOfs + TestTon / SampleRate) % 1;

      switch (sig)
      {
        case 0: return (byte)((testTonOfs < 0.5 ? 20 : -20) + 128);
        case 1: return (byte)((testTonOfs - 0.5) * 40 + 128);
        case 2: return (byte)(Math.Sin(testTonOfs * Math.PI * 2) * 20 + 128);
      }
      return 128;
    }

    byte NextR()
    {
      return (byte)rnd.Next(128 - 20, 128 + 20);
    }

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
      filterL.faktor = vScrollBar1.Value / (double)SampleRate;
    }

    Bitmap oszBitmap;
    bool draw;
    int zoom = 4;

    void timer1_Tick(object sender, EventArgs e)
    {
      if (draw) return;
      int width = pictureBox1.Width;
      int height = pictureBox1.Height;
      if (width < 32 || height < 32) return;
      draw = true;

      var readBuffer = rollBufferL;

      var buffer = new short[width];
      int pos = (rollBufferPos - buffer.Length + RollBufferSize) % RollBufferSize;
      int trigger = pos;
      for (int i = 0; i < RollBufferSize; i++)
      {
        int pre = (trigger - 1 + RollBufferSize) % RollBufferSize;
        if (readBuffer[pre] >= 0) break;
        trigger = pre;
      }
      for (int i = 0; i < RollBufferSize; i++)
      {
        int pre = (trigger - 1 + RollBufferSize) % RollBufferSize;
        if (readBuffer[pre] < 0) break;
        trigger = pre;
      }
      pos = trigger;
      for (int i = 0; i < buffer.Length; i++) buffer[i] = readBuffer[(pos + i) % RollBufferSize];

      if (oszBitmap == null || oszBitmap.Width != width || oszBitmap.Height != height)
      {
        oszBitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);
        pictureBox1.Image = oszBitmap;
      }

      var g = Graphics.FromImage(oszBitmap);
      g.Clear(Color.Black);
      var pen = new Pen(Color.FromArgb(0x0080ff - 16777216));
      int mul = height * zoom;
      int ofs = (height * zoom - height) / 2;
      for (int x = 1; x < width; x++)
      {
        int y1 = (32767 - buffer[x - 1]) * mul / 65536 - ofs;
        int y2 = (32767 - buffer[x]) * mul / 65536 - ofs;
        if (y1 < 0) y1 = 0;
        if (y2 < 0) y2 = 0;
        if (y1 >= height) y1 = height - 1;
        if (y2 >= height) y2 = height - 1;
        g.DrawLine(pen, x - 1, y1, x, y2);
      }

      pictureBox1.Refresh();
      draw = false;
    }

    void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
    {
      if (e.Delta < 0) zoom = Math.Max(1, zoom / 2);
      if (e.Delta > 0) zoom = Math.Min(100, zoom * 2);
    }

    void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
    {
      filterL.faktor = e.NewValue / (double)SampleRate;
      if (e.NewValue > 7000) filterL.faktor = 1;
    }

    void pictureBox1_MouseDown(object sender, MouseEventArgs e)
    {
      sig++;
      if (sig == 3) sig = 0;
    }
  }
}
