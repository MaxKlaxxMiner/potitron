using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ResistorHelper
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    void textBox1_KeyPress(object sender, KeyPressEventArgs e)
    {
      if (e.KeyChar == '\r')
      {
        listBox1.Items.Add(textBox1.Text);
      }
    }
  }
}
