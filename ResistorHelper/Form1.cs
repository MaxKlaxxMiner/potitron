using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ResistorHelper
{
  public sealed partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();

      if (File.Exists("autosave.txt"))
      {
        listBox1.Items.AddRange(File.ReadLines("autosave.txt").Select(Resistor.FromTsv).ToArray());
      }
    }

    static void AutoSave(Resistor[] resistors)
    {
      File.WriteAllLines("autosave.txt", resistors.Select(x => x.ToTsv()));
    }

    void textBox1_KeyPress(object sender, KeyPressEventArgs e)
    {
      if (e.KeyChar == '\r')
      {
        var p = Resistor.Parse(textBox1.Text);
        if (p != null)
        {
          int insertPos = 0;
          while (insertPos < listBox1.Items.Count && ((Resistor)listBox1.Items[insertPos]).valueMilliOhm < p.valueMilliOhm) insertPos++;
          listBox1.Items.Insert(insertPos, p);
          AutoSave(listBox1.Items.Cast<Resistor>().ToArray());
        }
      }
    }
  }
}
