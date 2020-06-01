using System.Runtime.InteropServices;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace arduino_audio
{
  /// <summary>
  /// Struktur zum speichern einer MIDI-Nachricht
  /// </summary>
  [StructLayout(LayoutKind.Sequential, Pack = 0)]
  public struct MidiValue
  {
    /// <summary>
    /// reserviertes Platzhalter-Feld (momentan ohne Funktion und immer 0)
    /// </summary>
    public readonly byte undefined;
    /// <summary>
    /// Anschlagstärke der Note (0-127)
    /// </summary>
    public readonly byte velocity;
    /// <summary>
    /// die gespielte Note in Halbtönen (0-127, z.B.: C5 = 72)
    /// </summary>
    public readonly byte note;
    /// <summary>
    /// Steuerungs-Byte (z.B. 0x90 = note-on, 0x80 = note-off) + Channel: 0x00-0x0f
    /// </summary>
    public readonly byte control;
    /// <summary>
    /// optionaler Zeitstempel der MIDI-Nachricht
    /// </summary>
    public readonly uint timeStamp;
    /// <summary>
    /// Konstruktor mit den beiden Parametern, welche per <see cref="MidiInput.MidiInProc"/> übergeben werden
    /// </summary>
    /// <param name="param1">erster Parameter (MIDI-Daten)</param>
    /// <param name="param2">zweiter Parameter (TimeStamp in ms)</param>
    public MidiValue(uint param1, uint param2)
    {
      undefined = (byte)(param1 >> 24);
      velocity = (byte)(param1 >> 16);
      note = (byte)(param1 >> 8);
      control = (byte)param1;
      timeStamp = param2;
    }
    /// <summary>
    /// Konstruktor mit den entsprechenden Feldern
    /// </summary>
    /// <param name="control">Steuerungs-Byte (z.B. 0x90 = note-on, 0x80 = note-off) + Channel: 0x00-0x0f</param>
    /// <param name="note">die gespielte Note in Halbtönen (0-127, z.B.: C5 = 72)</param>
    /// <param name="velocity">Anschlagstärke der Note (0-127)</param>
    /// <param name="timeStamp">optionaler Zeitstempel der MIDI-Nachricht</param>
    /// <param name="undefined">reserviertes Platzhalter-Feld (momentan ohne Funktion und immer 0)</param>
    public MidiValue(byte control, byte note, byte velocity = 100, uint timeStamp = 0xffffffff, byte undefined = 0)
    {
      this.undefined = undefined;
      this.velocity = velocity;
      this.note = note;
      this.control = control;
      this.timeStamp = timeStamp;
    }

    /// <summary>
    /// Konstruktor mit einer bestimmten Note
    /// </summary>
    /// <param name="onOff">gibt an, ob die Note an- (true) oder ausgeschaltet wird (false)</param>
    /// <param name="note">die gespielte Note in Halbtönen (0-127, z.B.: C5 = 72)</param>
    /// <param name="velocity">Anschlagstärke der Note (0-127)</param>
    /// <param name="channel">optionaler Kanal, welcher verwendet werden soll 0-15 (Default: 0)</param>
    public MidiValue(bool onOff, byte note, byte velocity = 100, byte channel = 0)
    {
      undefined = 0;
      this.velocity = velocity;
      this.note = note;
      control = (byte)(onOff ? 0x90 : 0x80 + channel);
      timeStamp = 0xffffffff;
    }

    /// <summary>
    /// gibt den Inhalt als lesbare Zeichenkette zurück
    /// </summary>
    /// <returns>lesbare Zeichenkette</returns>
    public override string ToString()
    {
      return (new { control, note, velocity, timeStamp }).ToString();
    }

    /// <summary>
    /// gibt an, ob es ein gültiger Datensatz ist
    /// </summary>
    public bool Valid
    {
      get
      {
        return control != 0;
      }
    }
  }
}
