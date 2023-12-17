namespace VoiceAUTH
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            panel1 = new Panel();
            label2 = new Label();
            button4 = new Button();
            label1 = new Label();
            progressBar1 = new ProgressBar();
            timer1 = new System.Windows.Forms.Timer(components);
            formsPlot2 = new ScottPlot.FormsPlot();
            timer2 = new System.Windows.Forms.Timer(components);
            formsPlot1 = new ScottPlot.FormsPlot();
            formsPlot3 = new ScottPlot.FormsPlot();
            button5 = new Button();
            button6 = new Button();
            button7 = new Button();
            formsPlot4 = new ScottPlot.FormsPlot();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(356, 15);
            button1.Name = "button1";
            button1.Size = new Size(103, 43);
            button1.TabIndex = 0;
            button1.Text = "Запись";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Enabled = false;
            button2.Location = new Point(465, 15);
            button2.Name = "button2";
            button2.Size = new Size(103, 43);
            button2.TabIndex = 1;
            button2.Text = "Стоп";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Enabled = false;
            button3.Location = new Point(685, 15);
            button3.Name = "button3";
            button3.Size = new Size(103, 43);
            button3.TabIndex = 2;
            button3.Text = "Слушать";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(label2);
            panel1.Controls.Add(button4);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(progressBar1);
            panel1.Controls.Add(button3);
            panel1.Controls.Add(button1);
            panel1.Controls.Add(button2);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1216, 69);
            panel1.TabIndex = 4;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(871, 26);
            label2.Name = "label2";
            label2.Size = new Size(50, 20);
            label2.TabIndex = 7;
            label2.Text = "label2";
            label2.Visible = false;
            // 
            // button4
            // 
            button4.Enabled = false;
            button4.Location = new Point(574, 15);
            button4.Name = "button4";
            button4.Size = new Size(103, 43);
            button4.TabIndex = 6;
            button4.Text = "Сравнить";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(205, 26);
            label1.Name = "label1";
            label1.Size = new Size(54, 20);
            label1.TabIndex = 5;
            label1.Text = "Время";
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(22, 21);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(177, 29);
            progressBar1.TabIndex = 4;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 20;
            timer1.Tick += timer1_Tick;
            // 
            // formsPlot2
            // 
            formsPlot2.Dock = DockStyle.Top;
            formsPlot2.Location = new Point(0, 69);
            formsPlot2.Margin = new Padding(5, 4, 5, 4);
            formsPlot2.Name = "formsPlot2";
            formsPlot2.Size = new Size(1216, 282);
            formsPlot2.TabIndex = 5;
            // 
            // timer2
            // 
            timer2.Enabled = true;
            timer2.Interval = 1000;
            timer2.Tick += timer2_Tick;
            // 
            // formsPlot1
            // 
            formsPlot1.Location = new Point(388, 392);
            formsPlot1.Margin = new Padding(5, 4, 5, 4);
            formsPlot1.Name = "formsPlot1";
            formsPlot1.Size = new Size(400, 351);
            formsPlot1.TabIndex = 6;
            // 
            // formsPlot3
            // 
            formsPlot3.Location = new Point(798, 392);
            formsPlot3.Margin = new Padding(5, 4, 5, 4);
            formsPlot3.Name = "formsPlot3";
            formsPlot3.Size = new Size(400, 351);
            formsPlot3.TabIndex = 7;
            // 
            // button5
            // 
            button5.Enabled = false;
            button5.Location = new Point(142, 358);
            button5.Name = "button5";
            button5.Size = new Size(103, 43);
            button5.TabIndex = 8;
            button5.Text = "Запись 1";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // button6
            // 
            button6.Enabled = false;
            button6.Location = new Point(545, 358);
            button6.Name = "button6";
            button6.Size = new Size(103, 43);
            button6.TabIndex = 9;
            button6.Text = "Запись 2";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // button7
            // 
            button7.Enabled = false;
            button7.Location = new Point(956, 358);
            button7.Name = "button7";
            button7.Size = new Size(103, 43);
            button7.TabIndex = 10;
            button7.Text = "Запись 3";
            button7.UseVisualStyleBackColor = true;
            button7.Click += button7_Click;
            // 
            // formsPlot4
            // 
            formsPlot4.Location = new Point(4, 392);
            formsPlot4.Margin = new Padding(5, 4, 5, 4);
            formsPlot4.Name = "formsPlot4";
            formsPlot4.Size = new Size(374, 351);
            formsPlot4.TabIndex = 11;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1216, 744);
            Controls.Add(button5);
            Controls.Add(formsPlot4);
            Controls.Add(button7);
            Controls.Add(button6);
            Controls.Add(formsPlot3);
            Controls.Add(formsPlot1);
            Controls.Add(formsPlot2);
            Controls.Add(panel1);
            Name = "MainForm";
            Text = "VoiceAUTH";
            FormClosed += Form1_FormClosed;
            Load += Form1_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
        private Button button2;
        private Button button3;
        private Panel panel1;
        
        private System.Windows.Forms.Timer timer1;
        private ProgressBar progressBar1;
        private ScottPlot.FormsPlot formsPlot2;
        private System.Windows.Forms.Timer timer2;
        private Label label1;
        private Button button4;
        private ScottPlot.FormsPlot formsPlot1;
        private ScottPlot.FormsPlot formsPlot3;
        private Button button5;
        private Button button6;
        private Button button7;
        private ScottPlot.FormsPlot formsPlot4;
        private Label label2;
    }
}
