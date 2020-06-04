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

    MidiInput midi = new MidiInput();

    FilterTiefpassO3 filterL;
    FilterTiefpass filterR;

    const int RollBufferSize = 65536;
    static short[] rollBufferL = new short[RollBufferSize];
    static short[] rollBufferR = new short[RollBufferSize];
    static int rollBufferPos;

    Random rnd = new Random();

    /// <summary>
    /// aktueller Abspielzeit
    /// </summary>
    double micros;
    /// <summary>
    /// Echo-Zeit in Microsekunden
    /// </summary>
    const uint EchoMicros = 170000; // 170 ms
    /// <summary>
    /// aktuell abgespielte Primär-Töne
    /// </summary>
    Dictionary<byte, Tone> tones = new Dictionary<byte, Tone>();
    /// <summary>
    /// tatsächlich aktuell abgespielte Echo-Töne
    /// </summary>
    Dictionary<ushort, Tone> echoTones = new Dictionary<ushort, Tone>();
    /// <summary>
    /// Echo-Noten in der Reihenfolge, wie sie noch ausgelöst werden
    /// </summary>
    Queue<EchoNote> echoNotes = new Queue<EchoNote>();

    void MidiUpdate()
    {
      var midiValue = midi.ReadValue();
      if (midiValue.Valid)
      {
        if ((midiValue.control & 0xf0) == 0x90)
        {
          if (!tones.ContainsKey(midiValue.note))
          {
            tones.Add(midiValue.note, new Tone((uint)micros, midiValue.note, (WaveType)sig));
            echoNotes.Enqueue(new EchoNote { note = midiValue.note, waveType = (WaveType)sig, volume = 10, startMicros = (uint)micros + EchoMicros });
          }
        }
        if ((midiValue.control & 0xf0) == 0x80)
        {
          if (tones.ContainsKey(midiValue.note))
          {
            tones.Remove(midiValue.note);
            echoNotes.Enqueue(new EchoNote { note = (byte)(midiValue.note | 0x80), waveType = (WaveType)sig, volume = 10, startMicros = (uint)micros + EchoMicros });
          }
        }
      }

      // --- Echos verarbeiten ---
      for (; ; )
      {
        if (echoNotes.Count == 0) break; // keine Echos mehr vorhanden
        if (echoNotes.Peek().startMicros > (uint)micros) break; // Echo wird noch nicht ausgelöst

        var echoNote = echoNotes.Dequeue();
        ushort key = (ushort)(echoNote.volume << 8 | echoNote.note & 0x7f);
        if (echoNote.note < 128) // --- start Note ---
        {
          echoTones.Add(key, new Tone(echoNote.startMicros, echoNote.note, echoNote.waveType, echoNote.volume));
        }
        else // --- end Note ---
        {
          echoTones.Remove(key);
        }

        // --- weitere Wiederholungen des Echos hinzufügen ---
        if (echoNote.volume > 1)
        {
          echoNotes.Enqueue(new EchoNote { note = echoNote.note, waveType = echoNote.waveType, volume = (byte)(echoNote.volume / 2), startMicros = echoNote.startMicros + EchoMicros });
        }
      }
    }

    int sig = 7;

    byte NextL()
    {
      micros += 1000000.0 / SampleRate;
      uint mc = ((uint)micros) & 0xfffffffc;

      int v = 128;
      foreach (var tone in tones.Values)
      {
        v += tone.Calc(mc);
      }
      foreach (var tone in echoTones.Values)
      {
        v += tone.Calc(mc);
      }

      if (v < 0) v = 0;
      if (v > 255) v = 255;
      return (byte)v;
    }

    byte NextR()
    {
      uint mc = ((uint)micros) & 0xfffffffc;

      int v = rnd.Next(-3, 3) + 128;
      v = 128;
      foreach (var tone in tones.Values)
      {
        v += tone.Calc(mc);
      }
      foreach (var tone in echoTones.Values)
      {
        v += tone.Calc(mc);
      }

      if (v < 0) v = 0;
      if (v > 255) v = 255;
      return (byte)v;
    }

    void ReadWave(byte[] buffer)
    {
      lock (tones)
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

        xaSv.Start();
        while (mainThread.IsAlive)
        {
          lock (tones)
          {
            MidiUpdate();
          }
          Thread.Sleep(0);
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
      if (e.Delta < 0) zoom = Math.Max(1, zoom / 2);
      if (e.Delta > 0) zoom = Math.Min(8192, zoom * 2);
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
        if (sig == 8) sig = 0;
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

      { 0, 45 },         // A2 (capslock - german)
      { Keys.A, 46 },    // A#2
      { (Keys)226, 47 }, // B2 (less than - german)
      { Keys.Y, 48 },    // C3
      { Keys.S, 49 },    // C#3
      { Keys.X, 50 },    // D3
      { Keys.D, 51 },    // D#3
      { Keys.C, 52 },    // E3
      { Keys.V, 53 },    // F3
      { Keys.G, 54 },    // F#3
      { Keys.B, 55 },    // G3
      { Keys.H, 56 },    // G#3
      { Keys.N, 57 },    // A3
      { Keys.J, 58 },    // A#3
      { Keys.M, 59 },    // B3
      { (Keys)188, 60 }, // C4 (comma - german)
      { Keys.L, 61 },    // C#4
      { (Keys)190, 62 }, // D4 (period - german)
      { (Keys)192, 63 }, // D#4 (ö - german)
      { (Keys)189, 64 }, // E4 (hyphen - german)
      { (Keys)16, 65 },  // F4 (shift)
      { (Keys)191, 66 }, // F#4 (# - german)

      // --- oben ---

      { (Keys)220, 58 }, // A#3  (tilde - german)
      { Keys.Tab, 59 },  // B3
      { Keys.Q, 60 },    // C4
      { Keys.D2, 61 },   // C#4
      { Keys.W, 62 },    // D4
      { Keys.D3, 63 },   // D#4
      { Keys.E, 64 },    // E4
      { Keys.R, 65 },    // F4
      { Keys.D5, 66 },   // F#4
      { Keys.T, 67 },    // G4
      { Keys.D6, 68 },   // G#4
      { Keys.Z, 69 },    // A4
      { Keys.D7, 70 },   // A#4
      { Keys.U, 71 },    // B4
      { Keys.I, 72 },    // C5
      { Keys.D9, 73 },   // C#5
      { Keys.O, 74 },    // D5
      { Keys.D0, 75 },   // D#5
      { Keys.P, 76 },    // E5
      { (Keys)186, 77 }, // F5  (Ü - german)
      { (Keys)221, 78 }, // F#5 (` - german)
      { (Keys)187, 79 }, // G5  (+ - german)
      { (Keys)8, 80 },   // G#5 (backspace)
      { (Keys)13, 81 },  // A5  (return)
      { (Keys)45, 82 },  // A#5 (insert)
      { (Keys)46, 83 },  // B5  (del)
      { (Keys)35, 84 },  // C6  (end)
      { (Keys)33, 85 },  // C#6 (pageUp)
      { (Keys)34, 86 },  // D6  (pageDown)
    };

    #endregion
  }
}
