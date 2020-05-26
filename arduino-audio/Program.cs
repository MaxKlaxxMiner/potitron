#region # using *.*
// ReSharper disable RedundantUsingDirective
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SlimDX.Multimedia;
using XA = SlimDX.XAudio2;
#endregion

namespace arduino_audio
{
  static unsafe class Program
  {
    const int Samples = 16;
    const int SampleRate = 48000;

    static Thread audioThread;

    static byte NextL()
    {
      return 128;
    }

    static byte NextR()
    {
      return 128;
    }

    static void ReadWave(byte[] buffer)
    {
      fixed (byte* bufferP = buffer)
      {
        var samples = (short*)bufferP;
        for (int i = 0; i < Samples * 2; i += 2)
        {
          int l = (NextL() - 128) * 100;
          int r = (NextR() - 128) * 100;

          //todo: filter l
          //todo: mix r und l

          samples[i + 0] = (short)l;
          samples[i + 1] = (short)r;
        }
      }
    }

    static void Main(string[] args)
    {
      var mainThread = Thread.CurrentThread;
      audioThread = new Thread(() =>
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

      for (; ; )
      {
        if (Console.KeyAvailable)
        {
          Console.ReadKey(true).Key
          switch (Console.ReadKey(true).Key)
          {
            case ConsoleKey.Escape: return;
            case ConsoleKey.Q: 
          }
        }
      }
    }
  }
}
