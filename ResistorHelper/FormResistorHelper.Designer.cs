namespace ResistorHelper
{
  sealed partial class FormResistorHelper
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
      this.components = new System.ComponentModel.Container();
      this.listBoxResistors = new System.Windows.Forms.ListBox();
      this.contextMenuListBox = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
      this.tabPageSearch = new System.Windows.Forms.TabPage();
      this.radioButtonSearchType6 = new System.Windows.Forms.RadioButton();
      this.radioButtonSearchType5 = new System.Windows.Forms.RadioButton();
      this.radioButtonSearchType4 = new System.Windows.Forms.RadioButton();
      this.radioButtonSearchType3 = new System.Windows.Forms.RadioButton();
      this.label11 = new System.Windows.Forms.Label();
      this.radioButtonSearchType2 = new System.Windows.Forms.RadioButton();
      this.radioButtonSearchType1 = new System.Windows.Forms.RadioButton();
      this.listBoxSearchResults = new System.Windows.Forms.ListBox();
      this.label10 = new System.Windows.Forms.Label();
      this.textBoxSearch = new System.Windows.Forms.TextBox();
      this.contextMenuListBox.SuspendLayout();
      this.tabControl1.SuspendLayout();
      this.tabPageAdd.SuspendLayout();
      this.tabPageSearch.SuspendLayout();
      this.SuspendLayout();
      // 
      // listBoxResistors
      // 
      this.listBoxResistors.ContextMenuStrip = this.contextMenuListBox;
      this.listBoxResistors.Dock = System.Windows.Forms.DockStyle.Left;
      this.listBoxResistors.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.listBoxResistors.FormattingEnabled = true;
      this.listBoxResistors.Location = new System.Drawing.Point(0, 0);
      this.listBoxResistors.Name = "listBoxResistors";
      this.listBoxResistors.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
      this.listBoxResistors.Size = new System.Drawing.Size(216, 548);
      this.listBoxResistors.TabIndex = 0;
      // 
      // contextMenuListBox
      // 
      this.contextMenuListBox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeToolStripMenuItem});
      this.contextMenuListBox.Name = "contextMenuListBox";
      this.contextMenuListBox.Size = new System.Drawing.Size(146, 26);
      // 
      // removeToolStripMenuItem
      // 
      this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
      this.removeToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
      this.removeToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
      this.removeToolStripMenuItem.Text = "&Remove";
      this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
      // 
      // tabControl1
      // 
      this.tabControl1.Controls.Add(this.tabPageAdd);
      this.tabControl1.Controls.Add(this.tabPageSearch);
      this.tabControl1.Location = new System.Drawing.Point(222, 2);
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
      this.label9.Location = new System.Drawing.Point(6, 15);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(31, 13);
      this.label9.TabIndex = 11;
      this.label9.Text = "Ident";
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(174, 15);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(59, 13);
      this.label8.TabIndex = 10;
      this.label8.Text = "Ohm-Value";
      // 
      // textBoxIdent
      // 
      this.textBoxIdent.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxIdent.Location = new System.Drawing.Point(9, 31);
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
      this.textBoxValue.Location = new System.Drawing.Point(174, 31);
      this.textBoxValue.Name = "textBoxValue";
      this.textBoxValue.Size = new System.Drawing.Size(100, 20);
      this.textBoxValue.TabIndex = 0;
      this.textBoxValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
      // 
      // tabPageSearch
      // 
      this.tabPageSearch.Controls.Add(this.radioButtonSearchType6);
      this.tabPageSearch.Controls.Add(this.radioButtonSearchType5);
      this.tabPageSearch.Controls.Add(this.radioButtonSearchType4);
      this.tabPageSearch.Controls.Add(this.radioButtonSearchType3);
      this.tabPageSearch.Controls.Add(this.label11);
      this.tabPageSearch.Controls.Add(this.radioButtonSearchType2);
      this.tabPageSearch.Controls.Add(this.radioButtonSearchType1);
      this.tabPageSearch.Controls.Add(this.listBoxSearchResults);
      this.tabPageSearch.Controls.Add(this.label10);
      this.tabPageSearch.Controls.Add(this.textBoxSearch);
      this.tabPageSearch.Location = new System.Drawing.Point(4, 22);
      this.tabPageSearch.Name = "tabPageSearch";
      this.tabPageSearch.Size = new System.Drawing.Size(641, 520);
      this.tabPageSearch.TabIndex = 1;
      this.tabPageSearch.Text = "Search";
      this.tabPageSearch.UseVisualStyleBackColor = true;
      // 
      // radioButtonSearchType6
      // 
      this.radioButtonSearchType6.AutoSize = true;
      this.radioButtonSearchType6.Location = new System.Drawing.Point(407, 31);
      this.radioButtonSearchType6.Name = "radioButtonSearchType6";
      this.radioButtonSearchType6.Size = new System.Drawing.Size(48, 17);
      this.radioButtonSearchType6.TabIndex = 20;
      this.radioButtonSearchType6.Text = "hexa";
      this.radioButtonSearchType6.UseVisualStyleBackColor = true;
      this.radioButtonSearchType6.CheckedChanged += new System.EventHandler(this.textBoxSearch_TextChanged);
      // 
      // radioButtonSearchType5
      // 
      this.radioButtonSearchType5.AutoSize = true;
      this.radioButtonSearchType5.Location = new System.Drawing.Point(349, 31);
      this.radioButtonSearchType5.Name = "radioButtonSearchType5";
      this.radioButtonSearchType5.Size = new System.Drawing.Size(52, 17);
      this.radioButtonSearchType5.TabIndex = 19;
      this.radioButtonSearchType5.Text = "penta";
      this.radioButtonSearchType5.UseVisualStyleBackColor = true;
      this.radioButtonSearchType5.CheckedChanged += new System.EventHandler(this.textBoxSearch_TextChanged);
      // 
      // radioButtonSearchType4
      // 
      this.radioButtonSearchType4.AutoSize = true;
      this.radioButtonSearchType4.Location = new System.Drawing.Point(294, 31);
      this.radioButtonSearchType4.Name = "radioButtonSearchType4";
      this.radioButtonSearchType4.Size = new System.Drawing.Size(49, 17);
      this.radioButtonSearchType4.TabIndex = 18;
      this.radioButtonSearchType4.Text = "quad";
      this.radioButtonSearchType4.UseVisualStyleBackColor = true;
      this.radioButtonSearchType4.CheckedChanged += new System.EventHandler(this.textBoxSearch_TextChanged);
      // 
      // radioButtonSearchType3
      // 
      this.radioButtonSearchType3.AutoSize = true;
      this.radioButtonSearchType3.Location = new System.Drawing.Point(235, 31);
      this.radioButtonSearchType3.Name = "radioButtonSearchType3";
      this.radioButtonSearchType3.Size = new System.Drawing.Size(47, 17);
      this.radioButtonSearchType3.TabIndex = 17;
      this.radioButtonSearchType3.Text = "triple";
      this.radioButtonSearchType3.UseVisualStyleBackColor = true;
      this.radioButtonSearchType3.CheckedChanged += new System.EventHandler(this.textBoxSearch_TextChanged);
      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.Location = new System.Drawing.Point(111, 15);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(145, 13);
      this.label11.TabIndex = 16;
      this.label11.Text = "Search-Type (max. Resistors)";
      // 
      // radioButtonSearchType2
      // 
      this.radioButtonSearchType2.AutoSize = true;
      this.radioButtonSearchType2.Checked = true;
      this.radioButtonSearchType2.Location = new System.Drawing.Point(172, 31);
      this.radioButtonSearchType2.Name = "radioButtonSearchType2";
      this.radioButtonSearchType2.Size = new System.Drawing.Size(57, 17);
      this.radioButtonSearchType2.TabIndex = 15;
      this.radioButtonSearchType2.TabStop = true;
      this.radioButtonSearchType2.Text = "double";
      this.radioButtonSearchType2.UseVisualStyleBackColor = true;
      this.radioButtonSearchType2.CheckedChanged += new System.EventHandler(this.textBoxSearch_TextChanged);
      // 
      // radioButtonSearchType1
      // 
      this.radioButtonSearchType1.AutoSize = true;
      this.radioButtonSearchType1.Location = new System.Drawing.Point(114, 31);
      this.radioButtonSearchType1.Name = "radioButtonSearchType1";
      this.radioButtonSearchType1.Size = new System.Drawing.Size(52, 17);
      this.radioButtonSearchType1.TabIndex = 14;
      this.radioButtonSearchType1.Text = "single";
      this.radioButtonSearchType1.UseVisualStyleBackColor = true;
      this.radioButtonSearchType1.CheckedChanged += new System.EventHandler(this.textBoxSearch_TextChanged);
      // 
      // listBoxSearchResults
      // 
      this.listBoxSearchResults.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.listBoxSearchResults.FormattingEnabled = true;
      this.listBoxSearchResults.Location = new System.Drawing.Point(8, 57);
      this.listBoxSearchResults.Name = "listBoxSearchResults";
      this.listBoxSearchResults.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
      this.listBoxSearchResults.Size = new System.Drawing.Size(623, 446);
      this.listBoxSearchResults.TabIndex = 13;
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Location = new System.Drawing.Point(8, 15);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(59, 13);
      this.label10.TabIndex = 12;
      this.label10.Text = "Ohm-Value";
      // 
      // textBoxSearch
      // 
      this.textBoxSearch.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxSearch.Location = new System.Drawing.Point(8, 31);
      this.textBoxSearch.Name = "textBoxSearch";
      this.textBoxSearch.Size = new System.Drawing.Size(100, 20);
      this.textBoxSearch.TabIndex = 11;
      this.textBoxSearch.TextChanged += new System.EventHandler(this.textBoxSearch_TextChanged);
      // 
      // FormResistorHelper
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(865, 548);
      this.Controls.Add(this.tabControl1);
      this.Controls.Add(this.listBoxResistors);
      this.Name = "FormResistorHelper";
      this.Text = "ResistorHelper 0.01";
      this.contextMenuListBox.ResumeLayout(false);
      this.tabControl1.ResumeLayout(false);
      this.tabPageAdd.ResumeLayout(false);
      this.tabPageAdd.PerformLayout();
      this.tabPageSearch.ResumeLayout(false);
      this.tabPageSearch.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ListBox listBoxResistors;
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
    private System.Windows.Forms.TabPage tabPageSearch;
    private System.Windows.Forms.ListBox listBoxSearchResults;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.TextBox textBoxSearch;
    private System.Windows.Forms.ContextMenuStrip contextMenuListBox;
    private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
    private System.Windows.Forms.RadioButton radioButtonSearchType2;
    private System.Windows.Forms.RadioButton radioButtonSearchType1;
    private System.Windows.Forms.RadioButton radioButtonSearchType4;
    private System.Windows.Forms.RadioButton radioButtonSearchType3;
    private System.Windows.Forms.Label label11;
    private System.Windows.Forms.RadioButton radioButtonSearchType6;
    private System.Windows.Forms.RadioButton radioButtonSearchType5;
  }
}

