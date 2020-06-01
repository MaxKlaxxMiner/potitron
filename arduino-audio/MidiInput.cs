using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace arduino_audio
{
  /// <summary>
  /// Klasse zum einlesen von MIDI-Signalen
  /// </summary>
  public sealed class MidiInput : IDisposable
  {
    /// <summary>
    /// internere Delegate für die Callback-Funktionen
    /// </summary>
    public delegate void MidiInProc(IntPtr hMidiIn, int wMsg, IntPtr dwInstance, uint dwParam1, uint dwParam2);

    /// <summary>
    /// gibt die Anzahl der MIDI-Input Geräte zurück
    /// </summary>
    /// <returns>Anzahl der MIDI-Geräte</returns>
    [DllImport("winmm.dll")]
    static extern int midiInGetNumDevs();

    /// <summary>
    /// beendet ein MIDI-Input Objekt
    /// </summary>
    /// <param name="hMidiIn">Handle die MIDI-Input Objektes</param>
    /// <returns>0 = erfolgreich</returns>
    [DllImport("winmm.dll")]
    static extern int midiInClose(IntPtr hMidiIn);

    /// <summary>
    /// öffnet ein MIDI-Input Gerät
    /// </summary>
    /// <param name="lphMidiIn">erstelltes Handle, welches zurückgegeben wird</param>
    /// <param name="uDeviceId">Nummer des Gerätes (siehe: <see cref="midiInGetNumDevs"/>)</param>
    /// <param name="dwCallback">Rückrufmethode, wenn MIDI-Nachrichten eintreffen (siehe: <see cref="MidiInProc"/>)</param>
    /// <param name="dwCallbackInstance">optionale eigene Instanz</param>
    /// <param name="dwFlags">zugehörige Flags</param>
    /// <returns>0 = erfolgreich</returns>
    [DllImport("winmm.dll")]
    static extern int midiInOpen(out IntPtr lphMidiIn, int uDeviceId, MidiInProc dwCallback, IntPtr dwCallbackInstance, int dwFlags);

    /// <summary>
    /// startet ein MIDI-Input Gerät
    /// </summary>
    /// <param name="hMidiIn">Handle auf das bereits erstellte MIDI-Gerät (siehe: <see cref="midiInOpen"/>)</param>
    /// <returns>0 = erfolgreich</returns>
    [DllImport("winmm.dll")]
    static extern int midiInStart(IntPtr hMidiIn);

    /// <summary>
    /// stoppt ein wieder MIDI-Input Gerät
    /// </summary>
    /// <param name="hMidiIn">Handle auf das bereits erstellte MIDI-Gerät (siehe: <see cref="midiInOpen"/>)</param>
    /// <returns>0 = erfolgreich</returns>
    [DllImport("winmm.dll")]
    static extern int midiInStop(IntPtr hMidiIn);

    /// <summary>
    /// merkt sich die Handles aller erstellten MIDI-Geräte
    /// </summary>
    readonly List<IntPtr> handles = new List<IntPtr>();

    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    /// <summary>
    /// Rückruf-Methode als Field (damit der Garbage-Collector die Methode nicht zu früh löscht)
    /// </summary>
    readonly MidiInProc callBackFunc;

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="id">optionale Nummer des MIDI-Input Gerätes (siehe: <see cref="DeviceInputCount"/>), Default: -1 = alle gefundenen Geräte als Input nutzen</param>
    public MidiInput(int id = -1)
    {
      callBackFunc = MidiCallBack;
      IntPtr handle;

      if (id >= 0)
      {
        if (midiInOpen(out handle, id, callBackFunc, IntPtr.Zero, 0x30000) == 0 && midiInStart(handle) == 0) handles.Add(handle);
      }
      else
      {
        int count = DeviceInputCount;
        for (int i = 0; i < count; i++)
        {
          if (midiInOpen(out handle, i, callBackFunc, IntPtr.Zero, 0x30000) == 0 && midiInStart(handle) == 0) handles.Add(handle);
        }
      }
    }

    /// <summary>
    /// merkt sich alle MIDI-Nachrichten im Buffer
    /// </summary>
    readonly Queue<MidiValue> midiBuffer = new Queue<MidiValue>();

    /// <summary>
    /// interne Rückruf-Methode um die MIDI-Nachrichten zu empfangen
    /// </summary>
    void MidiCallBack(IntPtr hMidiIn, int wMsg, IntPtr dwInstance, uint dwParam1, uint dwParam2)
    {
      lock (midiBuffer)
      {
        midiBuffer.Enqueue(new MidiValue(dwParam1, dwParam2));
      }
    }

    /// <summary>
    /// gibt an, wieviel MIDI-Nachrichten sich momentan im Buffer befinden
    /// </summary>
    public int Avail
    {
      get
      {
        lock (midiBuffer)
        {
          return midiBuffer.Count;
        }
      }
    }

    /// <summary>
    /// liest eine MIDI-Nachricht aus (sofern vorhanden), siehe <see cref="MidiValue.Valid"/>
    /// </summary>
    /// <returns>MIDI-Nachricht</returns>
    public MidiValue ReadValue()
    {
      lock (midiBuffer)
      {
        return midiBuffer.Count > 0 ? midiBuffer.Dequeue() : new MidiValue();
      }
    }

    /// <summary>
    /// fügt eine eigene Simulierte MIDI-Nachricht in den Buffer hinzu
    /// </summary>
    /// <param name="value">MIDI-Nachricht, welche hinzugefügt werden soll</param>
    public void SimulateValue(MidiValue value)
    {
      lock (midiBuffer)
      {
        midiBuffer.Enqueue(value);
      }
    }

    /// <summary>
    /// gibt die Anzahl der MIDI-Input Geräte zurück
    /// </summary>
    public static int DeviceInputCount
    {
      get { return midiInGetNumDevs(); }
    }

    /// <summary>
    /// schließt die geöffneten MIDI-Geräte und gibt alle Ressourcen wieder frei
    /// </summary>
    public void Dispose()
    {
      foreach (var handle in handles)
      {
        try
        {
          midiInStop(handle);
          midiInClose(handle);
        }
        catch
        {
          // ignored
        }
      }
      handles.Clear();
    }
  }
}
