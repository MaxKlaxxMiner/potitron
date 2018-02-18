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

    /// <summary>
    /// zählt eine Nummer am Ende der Zeichenkette hoch oder fügt eine an (falls notwendig)
    /// </summary>
    /// <param name="val">Zeichenkette, welche inkrementiert werden soll</param>
    /// <returns>fertig inkrementierte Zeichenkette</returns>
    static string Increment(string val)
    {
      int digits = 0;
      while (digits < val.Length && char.IsDigit(val[val.Length - digits - 1])) digits++;
      if (digits == 0) return (val + " 1").Trim();
      long number = long.Parse(val.Substring(val.Length - digits, digits)) + 1;
      return val.Remove(val.Length - digits, digits) + number.ToString("D" + digits);
    }

    void textBox1_KeyPress(object sender, KeyPressEventArgs e)
    {
      if (e.KeyChar == '\r')
      {
        var p = Resistor.Parse(textBoxValue.Text);
        if (p != null)
        {
          p.ident = textBoxIdent.Text;
          textBoxIdent.Text = Increment(textBoxIdent.Text);
          int insertPos = 0;
          while (insertPos < listBox1.Items.Count && ((Resistor)listBox1.Items[insertPos]).valueMilliOhm < p.valueMilliOhm) insertPos++;
          listBox1.Items.Insert(insertPos, p);
          AutoSave(listBox1.Items.Cast<Resistor>().ToArray());
        }
      }
    }
  }
}
