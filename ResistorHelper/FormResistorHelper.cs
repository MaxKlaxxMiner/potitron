using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ResistorHelper
{
  public sealed partial class FormResistorHelper : Form
  {
    public FormResistorHelper()
    {
      InitializeComponent();

      if (File.Exists("autosave.txt"))
      {
        listBoxResistors.Items.AddRange(File.ReadLines("autosave.txt").Select(Resistor.FromTsv).ToArray());
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

    /// <summary>
    /// Enter-Taste, wenn ein neuer Widerstandwert hinzugefügt werden soll
    /// </summary>
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
          while (insertPos < listBoxResistors.Items.Count && ((Resistor)listBoxResistors.Items[insertPos]).valueMilliOhm < p.valueMilliOhm) insertPos++;
          listBoxResistors.Items.Insert(insertPos, p);
          AutoSave(listBoxResistors.Items.Cast<Resistor>().ToArray());
        }
      }
    }

    /// <summary>
    /// löscht ein oder mehrere Widerstände aus der Liste
    /// </summary>
    void removeToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (listBoxResistors.SelectedItems.Count > 0)
      {
        var toKill = listBoxResistors.SelectedItems.Cast<Resistor>().ToArray();
        if (MessageBox.Show("Remove Resistors: " + toKill.Length + "\r\n" + string.Join("\r\n", toKill.Select(x => x.ToString())), "Remove Resitors", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
        {
          listBoxSearchResults.BeginUpdate();
          foreach (var r in toKill) listBoxResistors.Items.Remove(r);
          listBoxSearchResults.EndUpdate();
          AutoSave(listBoxResistors.Items.Cast<Resistor>().ToArray());
        }
      }
    }

    /// <summary>
    /// Änderung der Sucheingabe
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void textBoxSearch_TextChanged(object sender, EventArgs e)
    {
      var val = Resistor.Parse(textBoxSearch.Text);
      if (val == null) return;

      int searchType = radioButtonSearchType1.Checked ? 1 : radioButtonSearchType2.Checked ? 2 : radioButtonSearchType3.Checked ? 3 : radioButtonSearchType4.Checked ? 4 : radioButtonSearchType5.Checked ? 5 : radioButtonSearchType6.Checked ? 6 : 0;
      if (searchType == 0) throw new ArgumentException("searchType");

      const int TargetLimit = 1000;

      ResistorResult[] results = null;
      foreach (double searchLevel in new[] { 1.000001, 1.00001, 1.0001, 1.0002, 1.0005, 1.001, 1.002, 1.005, 1.01, 1.02, 1.05, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9, 9999999.9 })
      {
        results = Resistor.Search(listBoxResistors.Items.Cast<Resistor>().ToArray(), val, searchType, searchLevel).Take(TargetLimit).ToArray();
        if (results.Length >= TargetLimit / 2) break; // mindestens die Hälfte an Ergebnissen erreicht?
      }

      Array.Sort(results, (x, y) => x.errorMilliOhm.CompareTo(y.errorMilliOhm));

      listBoxSearchResults.BeginUpdate();
      listBoxSearchResults.Items.Clear();
      listBoxSearchResults.Items.AddRange(results);
      listBoxSearchResults.EndUpdate();
    }
  }
}
