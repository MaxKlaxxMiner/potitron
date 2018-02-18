namespace ResistorHelper
{
  sealed partial class Form1
  {
    /// <summary>
    /// Erforderliche Designervariable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Verwendete Ressourcen bereinigen.
    /// </summary>
    /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Vom Windows Form-Designer generierter Code

    /// <summary>
    /// Erforderliche Methode für die Designerunterstützung.
    /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
    /// </summary>
    private void InitializeComponent()
    {
      this.listBox1 = new System.Windows.Forms.ListBox();
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.tabPageAdd = new System.Windows.Forms.TabPage();
      this.label9 = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this.textBoxIdent = new System.Windows.Forms.TextBox();
      this.label7 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.textBoxValue = new System.Windows.Forms.TextBox();
      this.tabControl1.SuspendLayout();
      this.tabPageAdd.SuspendLayout();
      this.SuspendLayout();
      // 
      // listBox1
      // 
      this.listBox1.Dock = System.Windows.Forms.DockStyle.Left;
      this.listBox1.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.listBox1.FormattingEnabled = true;
      this.listBox1.Location = new System.Drawing.Point(0, 0);
      this.listBox1.Name = "listBox1";
      this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
      this.listBox1.Size = new System.Drawing.Size(216, 548);
      this.listBox1.TabIndex = 0;
      // 
      // tabControl1
      // 
      this.tabControl1.Controls.Add(this.tabPageAdd);
      this.tabControl1.Location = new System.Drawing.Point(217, 2);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(649, 546);
      this.tabControl1.TabIndex = 1;
      // 
      // tabPageAdd
      // 
      this.tabPageAdd.Controls.Add(this.label9);
      this.tabPageAdd.Controls.Add(this.label8);
      this.tabPageAdd.Controls.Add(this.textBoxIdent);
      this.tabPageAdd.Controls.Add(this.label7);
      this.tabPageAdd.Controls.Add(this.label6);
      this.tabPageAdd.Controls.Add(this.label5);
      this.tabPageAdd.Controls.Add(this.label4);
      this.tabPageAdd.Controls.Add(this.label3);
      this.tabPageAdd.Controls.Add(this.label2);
      this.tabPageAdd.Controls.Add(this.label1);
      this.tabPageAdd.Controls.Add(this.textBoxValue);
      this.tabPageAdd.Location = new System.Drawing.Point(4, 22);
      this.tabPageAdd.Name = "tabPageAdd";
      this.tabPageAdd.Padding = new System.Windows.Forms.Padding(3);
      this.tabPageAdd.Size = new System.Drawing.Size(641, 520);
      this.tabPageAdd.TabIndex = 0;
      this.tabPageAdd.Text = "Add";
      this.tabPageAdd.UseVisualStyleBackColor = true;
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(246, 15);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(31, 13);
      this.label9.TabIndex = 11;
      this.label9.Text = "Ident";
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(6, 15);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(59, 13);
      this.label8.TabIndex = 10;
      this.label8.Text = "Ohm-Value";
      // 
      // textBoxIdent
      // 
      this.textBoxIdent.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxIdent.Location = new System.Drawing.Point(249, 31);
      this.textBoxIdent.Name = "textBoxIdent";
      this.textBoxIdent.Size = new System.Drawing.Size(157, 20);
      this.textBoxIdent.TabIndex = 8;
      this.textBoxIdent.Text = "Type 1 - Nr. 1";
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label7.Location = new System.Drawing.Point(58, 144);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(121, 13);
      this.label7.TabIndex = 7;
      this.label7.Text = "219,77  (= 219.8 Ω)";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label6.Location = new System.Drawing.Point(58, 126);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(121, 13);
      this.label6.TabIndex = 6;
      this.label6.Text = "219.77  (= 219.8 Ω)";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label5.Location = new System.Drawing.Point(58, 108);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(127, 13);
      this.label5.TabIndex = 5;
      this.label5.Text = "10,01M  (= 10.01 MΩ)";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.Location = new System.Drawing.Point(58, 90);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(127, 13);
      this.label4.TabIndex = 4;
      this.label4.Text = "5,5k    (= 5.500 kΩ)";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(58, 72);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(121, 13);
      this.label3.TabIndex = 3;
      this.label3.Text = "220     (= 220.0 Ω)";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(6, 72);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(55, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Examples:";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(6, 54);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(237, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "Enter the value and press Enter to add to the list.";
      // 
      // textBoxValue
      // 
      this.textBoxValue.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxValue.Location = new System.Drawing.Point(6, 31);
      this.textBoxValue.Name = "textBoxValue";
      this.textBoxValue.Size = new System.Drawing.Size(237, 20);
      this.textBoxValue.TabIndex = 0;
      this.textBoxValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(865, 548);
      this.Controls.Add(this.tabControl1);
      this.Controls.Add(this.listBox1);
      this.Name = "Form1";
      this.Text = "ResistorHelper 0.01";
      this.tabControl1.ResumeLayout(false);
      this.tabPageAdd.ResumeLayout(false);
      this.tabPageAdd.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ListBox listBox1;
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabPageAdd;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox textBoxValue;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.TextBox textBoxIdent;
  }
}

