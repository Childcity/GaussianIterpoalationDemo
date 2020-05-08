namespace GaussianInterpolationResearch
{
	partial class Form1
	{
		/// <summary>
		/// Обязательная переменная конструктора.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Освободить все используемые ресурсы.
		/// </summary>
		/// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
				covidSett?.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Код, автоматически созданный конструктором форм Windows

		/// <summary>
		/// Требуемый метод для поддержки конструктора — не изменяйте 
		/// содержимое этого метода с помощью редактора кода.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			this.zedGraph = new ZedGraph.ZedGraphControl();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.methodChooser = new System.Windows.Forms.CheckedListBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.stepGb = new System.Windows.Forms.GroupBox();
			this.stepTb = new System.Windows.Forms.TextBox();
			this.stepFixedMode = new System.Windows.Forms.RadioButton();
			this.stepAutoMode = new System.Windows.Forms.RadioButton();
			this.covid19Mode = new System.Windows.Forms.RadioButton();
			this.standartFunctionMode = new System.Windows.Forms.RadioButton();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.scoreLagrageLb = new System.Windows.Forms.Label();
			this.scoreGausLb = new System.Windows.Forms.Label();
			this.exportToWordCB = new System.Windows.Forms.CheckBox();
			this.scoreParamNormLb = new System.Windows.Forms.Label();
			this.scoreParamSummLb = new System.Windows.Forms.Label();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.autoReportChBx = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.button2 = new System.Windows.Forms.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.alphaSummaryTrBar = new System.Windows.Forms.TrackBar();
			this.alphaSummaryTb = new System.Windows.Forms.TextBox();
			this.alphaParametricTrBar = new System.Windows.Forms.TrackBar();
			this.alphaParametricTb = new System.Windows.Forms.TextBox();
			this.alphaNonParametricTrBar = new System.Windows.Forms.TrackBar();
			this.alphaNonParametricTb = new System.Windows.Forms.TextBox();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.stepGb.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.alphaSummaryTrBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.alphaParametricTrBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.alphaNonParametricTrBar)).BeginInit();
			this.SuspendLayout();
			// 
			// zedGraph
			// 
			this.zedGraph.Dock = System.Windows.Forms.DockStyle.Fill;
			this.zedGraph.IsShowPointValues = true;
			this.zedGraph.IsZoomOnMouseCenter = true;
			this.zedGraph.Location = new System.Drawing.Point(4, 4);
			this.zedGraph.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.zedGraph.Name = "zedGraph";
			this.zedGraph.ScrollGrace = 0D;
			this.zedGraph.ScrollMaxX = 0D;
			this.zedGraph.ScrollMaxY = 0D;
			this.zedGraph.ScrollMaxY2 = 0D;
			this.zedGraph.ScrollMinX = 0D;
			this.zedGraph.ScrollMinY = 0D;
			this.zedGraph.ScrollMinY2 = 0D;
			this.zedGraph.Size = new System.Drawing.Size(847, 834);
			this.zedGraph.TabIndex = 0;
			this.zedGraph.UseExtendedPrintDialog = true;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 550F));
			this.tableLayoutPanel1.Controls.Add(this.zedGraph, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(1405, 842);
			this.tableLayoutPanel1.TabIndex = 1;
			// 
			// panel1
			// 
			this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel1.Controls.Add(this.methodChooser);
			this.panel1.Controls.Add(this.groupBox3);
			this.panel1.Controls.Add(this.groupBox2);
			this.panel1.Controls.Add(this.groupBox1);
			this.panel1.Controls.Add(this.label6);
			this.panel1.Controls.Add(this.label5);
			this.panel1.Controls.Add(this.label3);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.alphaSummaryTrBar);
			this.panel1.Controls.Add(this.alphaSummaryTb);
			this.panel1.Controls.Add(this.alphaParametricTrBar);
			this.panel1.Controls.Add(this.alphaParametricTb);
			this.panel1.Controls.Add(this.alphaNonParametricTrBar);
			this.panel1.Controls.Add(this.alphaNonParametricTb);
			this.panel1.Controls.Add(this.progressBar1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(858, 3);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(544, 836);
			this.panel1.TabIndex = 2;
			// 
			// methodChooser
			// 
			this.methodChooser.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.methodChooser.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.methodChooser.CheckOnClick = true;
			this.methodChooser.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.methodChooser.FormattingEnabled = true;
			this.methodChooser.Items.AddRange(new object[] {
            "Basis Points",
            "Lagrange",
            "Gaus Non Parametric",
            "Gaus Parametric Normal",
            "Gaus Parametric Summary"});
			this.methodChooser.Location = new System.Drawing.Point(22, 237);
			this.methodChooser.Name = "methodChooser";
			this.methodChooser.Size = new System.Drawing.Size(317, 145);
			this.methodChooser.TabIndex = 2;
			this.methodChooser.UseCompatibleTextRendering = true;
			// 
			// groupBox3
			// 
			this.groupBox3.BackColor = System.Drawing.Color.Cornsilk;
			this.groupBox3.Controls.Add(this.stepGb);
			this.groupBox3.Controls.Add(this.covid19Mode);
			this.groupBox3.Controls.Add(this.standartFunctionMode);
			this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
			this.groupBox3.ForeColor = System.Drawing.SystemColors.ControlText;
			this.groupBox3.Location = new System.Drawing.Point(12, 9);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(523, 148);
			this.groupBox3.TabIndex = 13;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Program Mode";
			this.groupBox3.UseCompatibleTextRendering = true;
			// 
			// stepGb
			// 
			this.stepGb.Controls.Add(this.stepTb);
			this.stepGb.Controls.Add(this.stepFixedMode);
			this.stepGb.Controls.Add(this.stepAutoMode);
			this.stepGb.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold);
			this.stepGb.Location = new System.Drawing.Point(32, 64);
			this.stepGb.Name = "stepGb";
			this.stepGb.Size = new System.Drawing.Size(209, 78);
			this.stepGb.TabIndex = 1;
			this.stepGb.TabStop = false;
			this.stepGb.Text = "Step";
			// 
			// stepTb
			// 
			this.stepTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
			this.stepTb.Location = new System.Drawing.Point(145, 46);
			this.stepTb.Name = "stepTb";
			this.stepTb.Size = new System.Drawing.Size(58, 26);
			this.stepTb.TabIndex = 2;
			this.stepTb.Text = "0.2";
			// 
			// stepFixedMode
			// 
			this.stepFixedMode.AutoSize = true;
			this.stepFixedMode.Location = new System.Drawing.Point(12, 49);
			this.stepFixedMode.Name = "stepFixedMode";
			this.stepFixedMode.Size = new System.Drawing.Size(67, 21);
			this.stepFixedMode.TabIndex = 1;
			this.stepFixedMode.Text = "Fixed";
			this.stepFixedMode.UseVisualStyleBackColor = true;
			// 
			// stepAutoMode
			// 
			this.stepAutoMode.AutoSize = true;
			this.stepAutoMode.Checked = true;
			this.stepAutoMode.Location = new System.Drawing.Point(12, 22);
			this.stepAutoMode.Name = "stepAutoMode";
			this.stepAutoMode.Size = new System.Drawing.Size(104, 21);
			this.stepAutoMode.TabIndex = 0;
			this.stepAutoMode.TabStop = true;
			this.stepAutoMode.Text = "Increasing";
			this.stepAutoMode.UseVisualStyleBackColor = true;
			// 
			// covid19Mode
			// 
			this.covid19Mode.AutoSize = true;
			this.covid19Mode.Cursor = System.Windows.Forms.Cursors.Hand;
			this.covid19Mode.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
			this.covid19Mode.Location = new System.Drawing.Point(283, 33);
			this.covid19Mode.Name = "covid19Mode";
			this.covid19Mode.Size = new System.Drawing.Size(186, 25);
			this.covid19Mode.TabIndex = 0;
			this.covid19Mode.Text = "COVID-19 Statistics";
			this.covid19Mode.UseCompatibleTextRendering = true;
			this.covid19Mode.UseVisualStyleBackColor = true;
			// 
			// standartFunctionMode
			// 
			this.standartFunctionMode.AutoSize = true;
			this.standartFunctionMode.Checked = true;
			this.standartFunctionMode.Cursor = System.Windows.Forms.Cursors.Hand;
			this.standartFunctionMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
			this.standartFunctionMode.Location = new System.Drawing.Point(32, 33);
			this.standartFunctionMode.Name = "standartFunctionMode";
			this.standartFunctionMode.Size = new System.Drawing.Size(177, 25);
			this.standartFunctionMode.TabIndex = 0;
			this.standartFunctionMode.TabStop = true;
			this.standartFunctionMode.Text = "Standart Functions";
			this.standartFunctionMode.UseCompatibleTextRendering = true;
			this.standartFunctionMode.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
			this.groupBox2.Controls.Add(this.scoreParamNormLb);
			this.groupBox2.Controls.Add(this.scoreGausLb);
			this.groupBox2.Controls.Add(this.scoreParamSummLb);
			this.groupBox2.Controls.Add(this.scoreLagrageLb);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.button1);
			this.groupBox2.Controls.Add(this.exportToWordCB);
			this.groupBox2.Controls.Add(this.checkBox1);
			this.groupBox2.Controls.Add(this.autoReportChBx);
			this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.groupBox2.Location = new System.Drawing.Point(12, 154);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(527, 285);
			this.groupBox2.TabIndex = 12;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Standart Functions";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label1.Location = new System.Drawing.Point(384, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(123, 59);
			this.label1.TabIndex = 3;
			this.label1.Text = "Algorithm\r\nScore";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.label1.UseCompatibleTextRendering = true;
			// 
			// button1
			// 
			this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.button1.Location = new System.Drawing.Point(5, 241);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(85, 36);
			this.button1.TabIndex = 1;
			this.button1.Text = "Rport";
			this.button1.UseCompatibleTextRendering = true;
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// scoreLagrageLb
			// 
			this.scoreLagrageLb.AutoSize = true;
			this.scoreLagrageLb.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)), true);
			this.scoreLagrageLb.Location = new System.Drawing.Point(328, 105);
			this.scoreLagrageLb.Name = "scoreLagrageLb";
			this.scoreLagrageLb.Size = new System.Drawing.Size(41, 34);
			this.scoreLagrageLb.TabIndex = 4;
			this.scoreLagrageLb.Text = "0.0";
			this.scoreLagrageLb.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.scoreLagrageLb.UseCompatibleTextRendering = true;
			this.scoreLagrageLb.Click += new System.EventHandler(this.scoreLagrageLb_Click);
			// 
			// scoreGausLb
			// 
			this.scoreGausLb.AutoSize = true;
			this.scoreGausLb.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)), true);
			this.scoreGausLb.Location = new System.Drawing.Point(328, 134);
			this.scoreGausLb.Name = "scoreGausLb";
			this.scoreGausLb.Size = new System.Drawing.Size(41, 34);
			this.scoreGausLb.TabIndex = 4;
			this.scoreGausLb.Text = "0.0";
			this.scoreGausLb.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.scoreGausLb.UseCompatibleTextRendering = true;
			// 
			// exportToWordCB
			// 
			this.exportToWordCB.AutoSize = true;
			this.exportToWordCB.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.exportToWordCB.Location = new System.Drawing.Point(363, 250);
			this.exportToWordCB.Name = "exportToWordCB";
			this.exportToWordCB.Size = new System.Drawing.Size(153, 23);
			this.exportToWordCB.TabIndex = 10;
			this.exportToWordCB.Text = "Export to MSWord";
			this.exportToWordCB.UseCompatibleTextRendering = true;
			this.exportToWordCB.UseVisualStyleBackColor = true;
			// 
			// scoreParamNormLb
			// 
			this.scoreParamNormLb.AutoSize = true;
			this.scoreParamNormLb.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)), true);
			this.scoreParamNormLb.Location = new System.Drawing.Point(328, 163);
			this.scoreParamNormLb.Name = "scoreParamNormLb";
			this.scoreParamNormLb.Size = new System.Drawing.Size(41, 34);
			this.scoreParamNormLb.TabIndex = 4;
			this.scoreParamNormLb.Text = "0.0";
			this.scoreParamNormLb.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.scoreParamNormLb.UseCompatibleTextRendering = true;
			// 
			// scoreParamSummLb
			// 
			this.scoreParamSummLb.AutoSize = true;
			this.scoreParamSummLb.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)), true);
			this.scoreParamSummLb.Location = new System.Drawing.Point(328, 194);
			this.scoreParamSummLb.Name = "scoreParamSummLb";
			this.scoreParamSummLb.Size = new System.Drawing.Size(41, 34);
			this.scoreParamSummLb.TabIndex = 4;
			this.scoreParamSummLb.Text = "0.0";
			this.scoreParamSummLb.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.scoreParamSummLb.UseCompatibleTextRendering = true;
			// 
			// checkBox1
			// 
			this.checkBox1.AutoSize = true;
			this.checkBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.checkBox1.Location = new System.Drawing.Point(97, 250);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(59, 23);
			this.checkBox1.TabIndex = 6;
			this.checkBox1.Text = "Next";
			this.checkBox1.UseCompatibleTextRendering = true;
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// autoReportChBx
			// 
			this.autoReportChBx.AutoSize = true;
			this.autoReportChBx.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.autoReportChBx.Location = new System.Drawing.Point(161, 250);
			this.autoReportChBx.Name = "autoReportChBx";
			this.autoReportChBx.Size = new System.Drawing.Size(59, 23);
			this.autoReportChBx.TabIndex = 6;
			this.autoReportChBx.Text = "Auto";
			this.autoReportChBx.UseCompatibleTextRendering = true;
			this.autoReportChBx.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.button2);
			this.groupBox1.Enabled = false;
			this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.groupBox1.Location = new System.Drawing.Point(12, 445);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(523, 82);
			this.groupBox1.TabIndex = 12;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "COVID-19 Statistics";
			this.groupBox1.UseCompatibleTextRendering = true;
			// 
			// button2
			// 
			this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.button2.Location = new System.Drawing.Point(196, 33);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(105, 39);
			this.button2.TabIndex = 0;
			this.button2.Text = "Settings";
			this.button2.UseCompatibleTextRendering = true;
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label6.Location = new System.Drawing.Point(19, 645);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(214, 27);
			this.label6.TabIndex = 11;
			this.label6.Text = "Gaus Parametric Normal";
			this.label6.UseCompatibleTextRendering = true;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label5.Location = new System.Drawing.Point(19, 570);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(188, 27);
			this.label5.TabIndex = 11;
			this.label5.Text = "Gaus Non Parametric";
			this.label5.UseCompatibleTextRendering = true;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label3.Location = new System.Drawing.Point(19, 712);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(234, 27);
			this.label3.TabIndex = 11;
			this.label3.Text = "Gaus Parametric Summary";
			this.label3.UseCompatibleTextRendering = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
			this.label2.Location = new System.Drawing.Point(7, 530);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(171, 29);
			this.label2.TabIndex = 9;
			this.label2.Text = "Alpha Control";
			// 
			// alphaSummaryTrBar
			// 
			this.alphaSummaryTrBar.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.alphaSummaryTrBar.LargeChange = 50;
			this.alphaSummaryTrBar.Location = new System.Drawing.Point(12, 739);
			this.alphaSummaryTrBar.Maximum = 20000;
			this.alphaSummaryTrBar.Name = "alphaSummaryTrBar";
			this.alphaSummaryTrBar.Size = new System.Drawing.Size(414, 56);
			this.alphaSummaryTrBar.TabIndex = 8;
			this.alphaSummaryTrBar.TickFrequency = 1000;
			this.alphaSummaryTrBar.TickStyle = System.Windows.Forms.TickStyle.Both;
			// 
			// alphaSummaryTb
			// 
			this.alphaSummaryTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
			this.alphaSummaryTb.Location = new System.Drawing.Point(432, 748);
			this.alphaSummaryTb.Name = "alphaSummaryTb";
			this.alphaSummaryTb.Size = new System.Drawing.Size(107, 28);
			this.alphaSummaryTb.TabIndex = 7;
			this.alphaSummaryTb.Text = "0.001";
			// 
			// alphaParametricTrBar
			// 
			this.alphaParametricTrBar.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.alphaParametricTrBar.LargeChange = 50;
			this.alphaParametricTrBar.Location = new System.Drawing.Point(12, 664);
			this.alphaParametricTrBar.Maximum = 20000;
			this.alphaParametricTrBar.Name = "alphaParametricTrBar";
			this.alphaParametricTrBar.Size = new System.Drawing.Size(414, 56);
			this.alphaParametricTrBar.TabIndex = 8;
			this.alphaParametricTrBar.TickFrequency = 1000;
			this.alphaParametricTrBar.TickStyle = System.Windows.Forms.TickStyle.Both;
			// 
			// alphaParametricTb
			// 
			this.alphaParametricTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
			this.alphaParametricTb.Location = new System.Drawing.Point(432, 673);
			this.alphaParametricTb.Name = "alphaParametricTb";
			this.alphaParametricTb.Size = new System.Drawing.Size(107, 28);
			this.alphaParametricTb.TabIndex = 7;
			this.alphaParametricTb.Text = "0.001";
			// 
			// alphaNonParametricTrBar
			// 
			this.alphaNonParametricTrBar.BackColor = System.Drawing.SystemColors.Control;
			this.alphaNonParametricTrBar.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.alphaNonParametricTrBar.LargeChange = 50;
			this.alphaNonParametricTrBar.Location = new System.Drawing.Point(12, 590);
			this.alphaNonParametricTrBar.Maximum = 20000;
			this.alphaNonParametricTrBar.Name = "alphaNonParametricTrBar";
			this.alphaNonParametricTrBar.Size = new System.Drawing.Size(414, 56);
			this.alphaNonParametricTrBar.TabIndex = 8;
			this.alphaNonParametricTrBar.TickFrequency = 1000;
			this.alphaNonParametricTrBar.TickStyle = System.Windows.Forms.TickStyle.Both;
			// 
			// alphaNonParametricTb
			// 
			this.alphaNonParametricTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
			this.alphaNonParametricTb.Location = new System.Drawing.Point(432, 599);
			this.alphaNonParametricTb.Name = "alphaNonParametricTb";
			this.alphaNonParametricTb.Size = new System.Drawing.Size(107, 28);
			this.alphaNonParametricTb.TabIndex = 7;
			this.alphaNonParametricTb.Text = "0.001";
			// 
			// progressBar1
			// 
			this.progressBar1.Location = new System.Drawing.Point(12, 804);
			this.progressBar1.MarqueeAnimationSpeed = 10;
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(523, 23);
			this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			this.progressBar1.TabIndex = 5;
			this.progressBar1.Value = 20;
			this.progressBar1.Visible = false;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1405, 842);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "Form1";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Gaussian Interpolation Research";
			this.tableLayoutPanel1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.stepGb.ResumeLayout(false);
			this.stepGb.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.alphaSummaryTrBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.alphaParametricTrBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.alphaNonParametricTrBar)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.ComponentModel.BackgroundWorker backgroundWorker1;
		private ZedGraph.ZedGraphControl zedGraph;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.CheckedListBox methodChooser;
		private System.Windows.Forms.Label scoreLagrageLb;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label scoreParamSummLb;
		private System.Windows.Forms.Label scoreParamNormLb;
		private System.Windows.Forms.Label scoreGausLb;
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.TrackBar alphaNonParametricTrBar;
		private System.Windows.Forms.TextBox alphaNonParametricTb;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox exportToWordCB;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TrackBar alphaSummaryTrBar;
		private System.Windows.Forms.TextBox alphaSummaryTb;
		private System.Windows.Forms.TrackBar alphaParametricTrBar;
		private System.Windows.Forms.TextBox alphaParametricTb;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox autoReportChBx;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.RadioButton covid19Mode;
		private System.Windows.Forms.RadioButton standartFunctionMode;
		private System.Windows.Forms.GroupBox stepGb;
		private System.Windows.Forms.RadioButton stepFixedMode;
		private System.Windows.Forms.RadioButton stepAutoMode;
		private System.Windows.Forms.TextBox stepTb;
	}
}

