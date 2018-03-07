namespace TSTEmguCVDemo
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Open = new System.Windows.Forms.Button();
            this.button_Draw = new System.Windows.Forms.Button();
            this.cvControlEx1 = new TST.Vision.Thirdparty.EmguCVControlEx();
            this.SuspendLayout();
            // 
            // Open
            // 
            this.Open.Location = new System.Drawing.Point(571, 74);
            this.Open.Name = "Open";
            this.Open.Size = new System.Drawing.Size(143, 60);
            this.Open.TabIndex = 1;
            this.Open.Text = "Open File";
            this.Open.UseVisualStyleBackColor = true;
            this.Open.Click += new System.EventHandler(this.Open_Click);
            // 
            // button_Draw
            // 
            this.button_Draw.Location = new System.Drawing.Point(571, 180);
            this.button_Draw.Name = "button_Draw";
            this.button_Draw.Size = new System.Drawing.Size(143, 60);
            this.button_Draw.TabIndex = 1;
            this.button_Draw.Text = "Draw ROI";
            this.button_Draw.UseVisualStyleBackColor = true;
            this.button_Draw.Click += new System.EventHandler(this.button_Draw_Click);
            // 
            // EmguCVControlEx1
            // 
            this.cvControlEx1.BackColor = System.Drawing.Color.Black;
            this.cvControlEx1.Location = new System.Drawing.Point(41, 65);
            this.cvControlEx1.Name = "cvControlEx1";
            this.cvControlEx1.Size = new System.Drawing.Size(437, 477);
            this.cvControlEx1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(927, 697);
            this.Controls.Add(this.Open);
            this.Controls.Add(this.button_Draw);
            this.Controls.Add(this.cvControlEx1);
            this.Text = "Form1";
            this.Name = "Form1";
        }

        #endregion

        private System.Windows.Forms.Button Open;
        private System.Windows.Forms.Button button_Draw;
        private TST.Vision.Thirdparty.EmguCVControlEx cvControlEx1;
    }
}

