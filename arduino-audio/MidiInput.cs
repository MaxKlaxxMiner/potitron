using System;
using System.Runtime.InteropServices;
// ReSharper disable UnusedMember.Global

namespace arduino_audio
{
  public class MidiInput : IDisposable
  {
    public delegate void MidiInProc(IntPtr hMidiIn, int wMsg, IntPtr dwInstance, uint dwParam1, uint dwParam2);

    [DllImport("winmm.dll")]
    static extern int midiInGetNumDevs();

    [DllImport("winmm.dll")]
    static extern int midiInClose(IntPtr hMidiIn);

    [DllImport("winmm.dll")]
    static extern int midiInOpen(out IntPtr lphMidiIn, int uDeviceId, MidiInProc dwCallback, IntPtr dwCallbackInstance, int dwFlags);

    [DllImport("winmm.dll")]
    static extern int midiInStart(IntPtr hMidiIn);

    [DllImport("winmm.dll")]
    static extern int midiInStop(IntPtr hMidiIn);

    IntPtr handle;

    public MidiInput(int id, MidiInProc midiInProc)
    {
      if (midiInOpen(out handle, id, midiInProc, IntPtr.Zero, 0x30000) != 0) throw new Exception("MIDI-Init Error");
      if (midiInStart(handle) != 0) throw new Exception("MIDI-Start Error");
    }

    public static int InputCount
    {
      get { return midiInGetNumDevs(); }
    }

    public void Dispose()
    {
      if (handle != IntPtr.Zero)
      {
        midiInStop(handle);
        //midiInClose(handle);
        handle = IntPtr.Zero;
      }
    }
  }
}
