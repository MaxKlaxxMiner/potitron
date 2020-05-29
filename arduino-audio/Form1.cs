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

    #region # --- form handling ---
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

    void Form1_Resize(object sender, EventArgs e)
    {
      timer1_Tick(null, null);
    }

    HashSet<Keys> keys = new HashSet<Keys>();

    void Form1_KeyDown(object sender, KeyEventArgs e)
    {
      keys.Add(e.KeyCode);
    }

    void Form1_KeyUp(object sender, KeyEventArgs e)
    {
      keys.Remove(e.KeyCode);
    }

    static readonly Dictionary<Keys, int> KeyMapping = new Dictionary<Keys, int>
    {
      { (Keys)20, -14 + 12 }
      //// --- unten ---

      //new Tuple<int, int, char>(0, -14 + 12, '&'),
      //new Tuple<int, int, char>(20, -14 + 12, 'A'),
      //new Tuple<int, int, char>(226, -13 + 12, '<'),
      //new Tuple<int, int, char>(89, -12 + 12, 'y'),
      //new Tuple<int, int, char>(83, -11 + 12, 's'),
      //new Tuple<int, int, char>(88, -10 + 12, 'x'),
      //new Tuple<int, int, char>(68, -9 + 12, 'd'),
      //new Tuple<int, int, char>(67, -8 + 12, 'c'),
      //new Tuple<int, int, char>(86, -7 + 12, 'v'),
      //new Tuple<int, int, char>(71, -6 + 12, 'g'),
      //new Tuple<int, int, char>(66, -5 + 12, 'b'),
      //new Tuple<int, int, char>(72, -4 + 12, 'h'),
      //new Tuple<int, int, char>(78, -3 + 12, 'n'),
      //new Tuple<int, int, char>(74, -2 + 12, 'j'),
      //new Tuple<int, int, char>(77, -1 + 12, 'm'),

      //new Tuple<int, int, char>(188, 0 + 12, ','),
      //new Tuple<int, int, char>(76, 1 + 12, 'l'),
      //new Tuple<int, int, char>(190, 2 + 12, '.'),
      //new Tuple<int, int, char>(192, 3 + 12, 'ö'),
      //new Tuple<int, int, char>(189, 4 + 12, '-'),
      //new Tuple<int, int, char>(16, 5 + 12, 'S'),
      //new Tuple<int, int, char>(191, 6 + 12, '#'),

      //// --- oben ---

      //new Tuple<int, int, char>(220, -2 + 12, '^'),
      //new Tuple<int, int, char>(9, -1 + 12, 'T'),

      //new Tuple<int, int, char>(81, 0 + 12, 'q'),
      //new Tuple<int, int, char>(50, 1 + 12, '2'),
      //new Tuple<int, int, char>(87, 2 + 12, 'w'),
      //new Tuple<int, int, char>(51, 3 + 12, '3'),
      //new Tuple<int, int, char>(69, 4 + 12, 'e'),
      //new Tuple<int, int, char>(82, 5 + 12, 'r'),
      //new Tuple<int, int, char>(53, 6 + 12, '5'),
      //new Tuple<int, int, char>(84, 7 + 12, 't'),
      //new Tuple<int, int, char>(54, 8 + 12, '6'),
      //new Tuple<int, int, char>(90, 9 + 12, 'z'),
      //new Tuple<int, int, char>(55, 10 + 12, '7'),
      //new Tuple<int, int, char>(85, 11 + 12, 'u'),
      //new Tuple<int, int, char>(73, 12 + 12, 'i'),
      //new Tuple<int, int, char>(57, 13 + 12, '9'),
      //new Tuple<int, int, char>(79, 14 + 12, 'o'),
      //new Tuple<int, int, char>(48, 15 + 12, '0'),
      //new Tuple<int, int, char>(80, 16 + 12, 'p'),
      //new Tuple<int, int, char>(186, 17 + 12, 'ü'),
      //new Tuple<int, int, char>(221, 18 + 12, '´'),
      //new Tuple<int, int, char>(187, 19 + 12, '+'),
      //new Tuple<int, int, char>(8, 20 + 12, 'B'),
      //new Tuple<int, int, char>(13, 21 + 12, 'E'),
      //new Tuple<int, int, char>(45, 22 + 12, 'I'),
      //new Tuple<int, int, char>(46, 23 + 12, 'N'),
      //new Tuple<int, int, char>(35, 24 + 12, 'D')
    };

    #endregion

  }
}
