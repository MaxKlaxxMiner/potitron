using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace arduino_audio
{
  public sealed class MidiInput : IDisposable
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

    readonly List<IntPtr> handles = new List<IntPtr>();

    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    readonly MidiInProc callBackFunc;
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

    public void SimulateValue(MidiValue value)
    {
      lock (midiBuffer)
      {
        midiBuffer.Enqueue(value);
      }
    }

    public static int DeviceInputCount
    {
      get { return midiInGetNumDevs(); }
    }

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
