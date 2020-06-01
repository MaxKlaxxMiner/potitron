using System.Runtime.InteropServices;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace arduino_audio
{
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
    public MidiValue(byte control, byte note, byte velocity = 100, uint timeStamp = 0xffffffff, byte undefined = 0)
    {
      this.undefined = undefined;
      this.velocity = velocity;
      this.note = note;
      this.control = control;
      this.timeStamp = timeStamp;
    }
    public MidiValue(bool onOff, byte note, byte velocity = 100)
    {
      undefined = 0;
      this.velocity = velocity;
      this.note = note;
      control = onOff ? (byte)144 : (byte)128;
      timeStamp = 0xffffffff;
    }
    public override string ToString()
    {
      return (new { control, note, velocity, timeStamp }).ToString();
    }
    public bool Valid { get { return control != 0; } }
  }
}
