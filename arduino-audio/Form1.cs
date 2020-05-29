#region # using *.*
// ReSharper disable RedundantUsingDirective
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
  public sealed unsafe partial class Form1 : Form
  {
    const int Samples = 16;
    const int SampleRate = 48000;
    double testTon = 110.0 + Math.Pow(2.0, 3.0 / 12.0);
    double tonOfsR = Math.Pow(2.0, 7.0 / 12);
    //double tonOfsR = 1.0;

    FilterTiefpassO3 filterL;
    FilterTiefpass filterR;

    const int RollBufferSize = 65536;
    static short[] rollBufferL = new short[RollBufferSize];
    static short[] rollBufferR = new short[RollBufferSize];
    static int rollBufferPos;

    double testTonOfsL;
    double testTonOfsR;

    Random rnd = new Random();

    int sig = 1;

    byte NextL()
    {
      testTonOfsL = (testTonOfsL + testTon / SampleRate) % 1;

      switch (sig)
      {
        case 1: return (byte)(Math.Sin(testTonOfsL * Math.PI * 2) * 20 + 128);
        case 2: return (byte)((testTonOfsL < 0.5 ? testTonOfsL - 0.25 : 0.75 - testTonOfsL) * 80 + 128);
        case 3: return (byte)((testTonOfsL - 0.5) * 40 + 128);
        case 4: return (byte)((testTonOfsL < 0.5 ? 20 : -20) + 128);
        default: return 128;
      }
    }

    byte NextR()
    {
      testTonOfsR = (testTonOfsR + testTon * tonOfsR / SampleRate) % 1;

      switch (sig)
      {
        case 1: return (byte)(Math.Sin(testTonOfsR * Math.PI * 2) * 20 + rnd.Next(-3, 3) + 128);
        case 2: return (byte)((testTonOfsR < 0.5 ? testTonOfsR - 0.25 : 0.75 - testTonOfsR) * 80 + rnd.Next(-3, 3) + 128);
        case 3: return (byte)((testTonOfsR - 0.5) * 40 + rnd.Next(-3, 3) + 128);
        case 4: return (byte)((testTonOfsR < 0.5 ? 20 : -20) + rnd.Next(-3, 3) + 128);
        default: return (byte)(128 + rnd.Next(-3, 3));
      }
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
        xaSv.Dispose();
        xaDev.Dispose();
      });
      audioThread.Start();
    }

    void Form1_Load(object sender, EventArgs e)
    {
      filterL.faktor = vScrollBar1.Value / (double)SampleRate;
      filterR.faktor = vScrollBar1.Value / (double)SampleRate;
    }

    Bitmap oszBitmap;
    bool draw;
    int zoom = 512;

    static void DrawBuffer(Graphics g, Color color, short[] buffer, int height, int zoom, int ofsY = 0)
    {
      var pen = new Pen(color);
      int mul = height * zoom / 256;
      int ofs = (height * zoom / 256 - height) / 2 + ofsY;
      for (int x = 1; x < buffer.Length; x++)
      {
        int y1 = (32767 - buffer[x - 1]) * mul / 65536 - ofs;
        int y2 = (32767 - buffer[x]) * mul / 65536 - ofs;
        if (y1 < 0) y1 = 0;
        if (y2 < 0) y2 = 0;
        if (y1 >= height) y1 = height - 1;
        if (y2 >= height) y2 = height - 1;
        g.DrawLine(pen, x - 1, y1, x, y2);
      }
    }

    static int GetRollTrigger(short[] rollBuffer, int startPos)
    {
      int trigger = startPos % RollBufferSize;

      for (int i = 0; i < RollBufferSize; i++)
      {
        int pre = (trigger - 1 + RollBufferSize) % RollBufferSize;
        if (rollBuffer[pre] >= 0) break;
        trigger = pre;
      }
      for (int i = 0; i < RollBufferSize; i++)
      {
        int pre = (trigger - 1 + RollBufferSize) % RollBufferSize;
        if (rollBuffer[pre] < 0) break;
        trigger = pre;
      }

      return trigger;
    }

    void timer1_Tick(object sender, EventArgs e)
    {
      if (draw) return;
      int width = pictureBox1.Width;
      int height = pictureBox1.Height;
      if (width < 32 || height < 32) return;
      draw = true;
      if (oszBitmap == null || oszBitmap.Width != width || oszBitmap.Height != height)
      {
        oszBitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);
        pictureBox1.Image = oszBitmap;
      }

      int triggerL = GetRollTrigger(rollBufferL, rollBufferPos - width + RollBufferSize);
      int triggerR = GetRollTrigger(rollBufferR, rollBufferPos - width + RollBufferSize);

      var buffer = new short[width];

      var g = Graphics.FromImage(oszBitmap);
      g.Clear(Color.Black);

      for (int i = 0; i < buffer.Length; i++) buffer[i] = rollBufferR[(triggerR + i) % RollBufferSize];
      DrawBuffer(g, Color.FromArgb(0xff8000 - 16777216), buffer, height, zoom, -height / 4);

      for (int i = 0; i < buffer.Length; i++) buffer[i] = rollBufferL[(triggerL + i) % RollBufferSize];
      DrawBuffer(g, Color.FromArgb(0x0080ff - 16777216), buffer, height, zoom, height / 4);

      pictureBox1.Refresh();
      draw = false;
    }

    bool mouseR;

    void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
    {
      if (mouseR)
      {
        if (e.Delta < 0) zoom = Math.Max(1, zoom / 2);
        if (e.Delta > 0) zoom = Math.Min(8192, zoom * 2);
      }
      else
      {
        if (e.Delta < 0) testTon /= Math.Pow(2.0, 1.0 / 12.0);
        if (e.Delta > 0) testTon *= Math.Pow(2.0, 1.0 / 12.0);
      }
    }

    void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
    {
      filterL.faktor = e.NewValue / (double)SampleRate;
      filterR.faktor = e.NewValue / (double)SampleRate;
      if (e.NewValue > 17000) filterL.faktor = filterR.faktor = 1;
    }

    void pictureBox1_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button.HasFlag(MouseButtons.Right)) mouseR = true;
      if (e.Button.HasFlag(MouseButtons.Left))
      {
        sig++;
        if (sig == 5) sig = 1;
      }
    }

    void pictureBox1_MouseUp(object sender, MouseEventArgs e)
    {
      if (e.Button.HasFlag(MouseButtons.Right)) mouseR = false;
    }

    private void Form1_Resize(object sender, EventArgs e)
    {
      timer1_Tick(null, null);
    }
  }
}
