using System;
using System.Runtime.InteropServices;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable UnusedMember.Global

namespace arduino_audio
{
  /// <summary>
  /// Tone, welcher moentan gespielt wird
  /// </summary>
  [StructLayout(LayoutKind.Sequential, Pack = 0)]
  public struct Tone
  {
    /// <summary>
    /// Startzeitpunkt in Mikrosekunden
    /// </summary>
    public uint startMicros;

    /// <summary>
    /// Anteil der Frequenz pro Mikrosekunde - z.B.
    /// 0x80000000 = eine Welle aller 2 Mikrosekunden = 500 Khz,
    /// 0x10000000 = eine Welle aller 16 Mikrosekunden = 62.5 KHz,
    /// 0x00112559 = eine Welle aller 3822,26 Mikrosekunden = 261,6255 Hz = C4
    /// </summary>
    public uint waveFracPerMicro;

    /// <summary>
    /// die zu spielende MIDI-Note
    /// </summary>
    public byte midiNote;

    /// <summary>
    /// Lautstärke des Tones (0-127, empfohlen: 20)
    /// </summary>
    public byte volume;

    /// <summary>
    /// merkt sich den Typ des Tones
    /// </summary>
    public ToneType toneType;

    public Tone(uint startMicros, byte midiNote, ToneType toneType = ToneType.Square, byte volume = 20)
    {
      this.startMicros = startMicros;
      this.midiNote = midiNote;
      this.toneType = toneType;
      this.volume = volume;

      double freq = Math.Pow(2, 1.0 / 12.0 * (midiNote - 21)) * 13.75;
      waveFracPerMicro = (uint)Math.Round(Math.Pow(2.0, 32.0) / (1000000.0 / freq));
    }

    /// <summary>
    /// berechnet den Ton-Wert
    /// </summary>
    /// <param name="micros">aktueller Zeitpunkt</param>
    /// <returns>berechneter Ton-Wert</returns>
    public sbyte Calc(uint micros)
    {
      if (micros < startMicros) return 0;

      uint mOfs = micros - startMicros;
      double wavePos = ((ulong)mOfs * waveFracPerMicro & uint.MaxValue) / (double)(1UL << 32);

      switch (toneType)
      {
        case ToneType.Square: return (sbyte)(wavePos < 0.5 ? volume : -volume);
        case ToneType.Triangle: return (sbyte)Math.Round((wavePos < 0.5 ? wavePos - 0.25 : 0.75 - wavePos) * (volume * 4));
        case ToneType.Sine: return (sbyte)Math.Round(Math.Sin(wavePos * Math.PI * 2) * volume);
        case ToneType.Saw: return (sbyte)Math.Round((wavePos - 0.5) * (volume * 2));

        case ToneType.Square2Tune:
        {
          int v = volume / 2;
          double wavePos2 = (mOfs * 1067557526UL / 1073741824UL * waveFracPerMicro & uint.MaxValue) / (double)(1UL << 32);
          return (sbyte)((wavePos < 0.5 ? v : -v) + (wavePos2 < 0.5 ? v : -v));
        }

        case ToneType.Square2Double:
        {
          int v = volume / 2;
          double wavePos2 = (mOfs * 2 * waveFracPerMicro & uint.MaxValue) / (double)(1UL << 32);
          return (sbyte)((wavePos < 0.5 ? v : -v) + (wavePos2 < 0.5 ? v : -v));
        }

        case ToneType.Square2DoubleTune:
        {
          int v = volume / 2;
          double wavePos2 = (mOfs * 1067557526UL / (1073741824UL / 2) * waveFracPerMicro & uint.MaxValue) / (double)(1UL << 32);
          return (sbyte)((wavePos < 0.5 ? v : -v) + (wavePos2 < 0.5 ? v : -v));
        }

        case ToneType.Square3DoubleTune:
        {
          int v = volume / 2;
          double wavePos2 = (mOfs * 1070645210UL / (1073741824UL / 2) * waveFracPerMicro & uint.MaxValue) / (double)(1UL << 32);
          double wavePos3 = (mOfs * 1067557526UL / (1073741824UL / 4) * waveFracPerMicro & uint.MaxValue) / (double)(1UL << 32);
          return (sbyte)((wavePos < 0.5 ? v : -v) + (wavePos2 < 0.5 ? v : -v) + (wavePos3 < 0.5 ? v : -v));
        }

        default: return 0;
      }
    }
  }
}
