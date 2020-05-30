using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

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
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    readonly MidiInProc callBackFunc;
    public MidiInput(int id)
    {
      callBackFunc = MidiCallBack;
      if (midiInOpen(out handle, id, callBackFunc, IntPtr.Zero, 0x30000) != 0) throw new Exception("MIDI-Init Error");
      if (midiInStart(handle) != 0) throw new Exception("MIDI-Start Error");
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct MidiValue
    {
      public readonly byte undefined;
      public readonly byte velocity;
      public readonly byte note;
      public readonly byte control;
      public readonly uint timeStamp;
      public MidiValue(uint param1, uint param2)
      {
        undefined = (byte)(param1 >> 24);
        velocity = (byte)(param1 >> 16);
        note = (byte)(param1 >> 8);
        control = (byte)param1;
        timeStamp = param2;
      }
      public override string ToString()
      {
        return (new { control, note, velocity, timeStamp }).ToString();
      }
      public bool Valid { get { return control != 0; } }
    }

    readonly Queue<MidiValue> midiBuffer = new Queue<MidiValue>();

    void MidiCallBack(IntPtr hMidiIn, int wMsg, IntPtr dwInstance, uint dwParam1, uint dwParam2)
    {
      lock (midiBuffer)
      {
        midiBuffer.Enqueue(new MidiValue(dwParam1, dwParam2));
      }
    }

    public MidiValue ReadValue()
    {
      lock (midiBuffer)
      {
        return midiBuffer.Count > 0 ? midiBuffer.Dequeue() : new MidiValue();
      }
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
        midiInClose(handle);
        handle = IntPtr.Zero;
      }
    }
  }
}
