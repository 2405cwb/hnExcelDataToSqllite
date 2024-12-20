namespace ExcelDataToSqllite
{
    partial class Form1
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
            button4 = new Button();
            button3 = new Button();
            progressBar1 = new ProgressBar();
            toolTip1 = new ToolTip(components);
            textBox1 = new TextBox();
            button5 = new Button();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(31, 92);
            button1.Margin = new Padding(2, 3, 2, 3);
            button1.Name = "button1";
            button1.Size = new Size(447, 42);
            button1.TabIndex = 0;
            button1.Text = "资产表数据到新增数据库";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(31, 31);
            button2.Margin = new Padding(2, 3, 2, 3);
            button2.Name = "button2";
            button2.Size = new Size(447, 42);
            button2.TabIndex = 1;
            button2.Text = "打开样式表格";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button4
            // 
            button4.Location = new Point(31, 152);
            button4.Margin = new Padding(2, 3, 2, 3);
            button4.Name = "button4";
            button4.Size = new Size(447, 42);
            button4.TabIndex = 3;
            button4.Text = "添加资产表数据到数据库";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button3
            // 
            button3.Location = new Point(32, 209);
            button3.Margin = new Padding(2, 3, 2, 3);
            button3.Name = "button3";
            button3.Size = new Size(447, 42);
            button3.TabIndex = 4;
            button3.Text = "更新资产表数据到数据库";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(12, 310);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(500, 38);
            progressBar1.TabIndex = 5;
            // 
            // toolTip1
            // 
            toolTip1.Popup += toolTip1_Popup;
            // 
            // textBox1
            // 
            textBox1.BackColor = Color.IndianRed;
            textBox1.Font = new Font("Microsoft YaHei UI", 10.5F, FontStyle.Regular, GraphicsUnit.Point);
            textBox1.Location = new Point(12, 366);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(500, 72);
            textBox1.TabIndex = 6;
            textBox1.Text = "首先用户选择严格按照样式表格样式的数据资产表.xlsx\r\n之后选择需要处理的数据库文件,按照完成提示的文字对\r\n处理后的数据库文件进行名称修改放置到需要的位置";
            // 
            // button5
            // 
            button5.Location = new Point(32, 257);
            button5.Margin = new Padding(2, 3, 2, 3);
            button5.Name = "button5";
            button5.Size = new Size(447, 42);
            button5.TabIndex = 7;
            button5.Text = "查看是否有类似数据";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(524, 460);
            Controls.Add(button5);
            Controls.Add(textBox1);
            Controls.Add(progressBar1);
            Controls.Add(button3);
            Controls.Add(button4);
            Controls.Add(button2);
            Controls.Add(button1);
            Margin = new Padding(2, 3, 2, 3);
            Name = "Form1";
            Text = "资产表入库";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Button button2;
        private Button button4;
        private Button button3;
        private ProgressBar progressBar1;
        private ToolTip toolTip1;
        private TextBox textBox1;
        private Button button5;
    }
}