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
    //double tonOfsR = Math.Pow(2.0, 7.0 / 12);
    double tonOfsR = 1.0;

    MidiInput midi;

    FilterTiefpassO3 filterL;
    FilterTiefpass filterR;

    const int RollBufferSize = 65536;
    static short[] rollBufferL = new short[RollBufferSize];
    static short[] rollBufferR = new short[RollBufferSize];
    static int rollBufferPos;

    double testTonOfsL;
    double testTonOfsR;

    Random rnd = new Random();

    int sig = 4;

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
      if (midi != null)
      {
        var midiValue = midi.ReadValue();
        if (midiValue.Valid)
        {
          //Text = midiValue.ToString();
          if ((midiValue.control & 0xf0) == 0x90)
          {
            testTon = Math.Pow(2, 1.0 / 12.0 * (midiValue.note - 21)) * 13.75;
          }
          if ((midiValue.control & 0xf0) == 0x80)
          {
            var t = Math.Pow(2, 1.0 / 12.0 * (midiValue.note - 21)) * 13.75;
            if (testTon == t) testTon = 0;
          }
        }
      }

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
        new XA.MasteringVoice(xaDev, 2, SampleRate, 0);
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

        midi = new MidiInput();
        xaSv.Start();
        while (mainThread.IsAlive)
        {
          Thread.Sleep(1);
        }
        xaSv.Stop();
        xaSv.Dispose();
        xaDev.Dispose();
        midi.Dispose();
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

    void Form1_KeyDown(object sender, KeyEventArgs e)
    {
      byte note;
      if (KeyMapping.TryGetValue(e.KeyCode, out note))
      {
        midi.SimulateValue(new MidiValue(true, note));
      }
      if (e.KeyCode == Keys.Escape) Close();
    }

    void Form1_KeyUp(object sender, KeyEventArgs e)
    {
      byte note;
      if (KeyMapping.TryGetValue(e.KeyCode, out note))
      {
        midi.SimulateValue(new MidiValue(false, note));
      }
    }

    static readonly Dictionary<Keys, byte> KeyMapping = new Dictionary<Keys, byte>
    {
      // --- unten ---

      { 0, 57 },         // A3 (capslock - german)
      { Keys.A, 58 },    // A#3
      { (Keys)226, 59 }, // B3 (less than - german)
      { Keys.Y, 60 },    // C4
      { Keys.S, 61 },    // C#4
      { Keys.X, 62 },    // D4
      { Keys.D, 63 },    // D#4
      { Keys.C, 64 },    // E4
      { Keys.V, 65 },    // F4
      { Keys.G, 66 },    // F#4
      { Keys.B, 67 },    // G4
      { Keys.H, 68 },    // G#4
      { Keys.N, 69 },    // A4
      { Keys.J, 70 },    // A#4
      { Keys.M, 71 },    // B4

      { (Keys)188, 72 }, // C5 (comma - german)
      { Keys.L, 73 },    // C#5
      { (Keys)190, 74 }, // D5 (period - german)
      { (Keys)192, 75 }, // D#5 (ö - german)
      { (Keys)189, 76 }, // E5 (hyphen - german)
      { (Keys)16, 77 },  // F5 (shift)
      { (Keys)191, 78 }, // F#5 (# - german)

      // --- oben ---

      { (Keys)220, 70 }, // A#4  (tilde - german)
      { Keys.Tab, 71 },  // B4
      { Keys.Q, 72 },    // C5
      { Keys.D2, 73 },   // C#5
      { Keys.W, 74 },    // D5
      { Keys.D3, 75 },   // D#5
      { Keys.E, 76 },    // E5
      { Keys.R, 77 },    // F5
      { Keys.D5, 78 },   // F#5
      { Keys.T, 79 },    // G5
      { Keys.D6, 80 },   // G#5
      { Keys.Z, 81 },    // A5
      { Keys.D7, 82 },   // A#5
      { Keys.U, 83 },    // B5
      { Keys.I, 84 },    // C6
      { Keys.D9, 85 },   // C#6
      { Keys.O, 86 },    // D6
      { Keys.D0, 87 },   // D#6
      { Keys.P, 88 },    // E6
      { (Keys)186, 89 }, // F6  (Ü - german)
      { (Keys)221, 90 }, // F#6 (` - german)
      { (Keys)187, 91 }, // G6  (+ - german)
      { (Keys)8, 92 },   // G#6 (backspace)
      { (Keys)13, 93 },  // A6  (return)
      { (Keys)45, 94 },  // A#6 (insert)
      { (Keys)46, 95 },  // B6  (del)
      { (Keys)35, 96 },  // C7  (end)
      { (Keys)33, 97 },  // C#7 (pageUp)
      { (Keys)34, 98 },  // D7  (pageDown)
    };

    #endregion
  }
}
