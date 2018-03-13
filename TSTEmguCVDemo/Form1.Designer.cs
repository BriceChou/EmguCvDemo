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
            this.Open = new System.Windows.Forms.Button();
            this.button_Draw = new System.Windows.Forms.Button();
            this.button_Zoom = new System.Windows.Forms.Button();
            this.button_Irregular = new System.Windows.Forms.Button();
            this.button_Contour = new System.Windows.Forms.Button();
            this.button_Move = new System.Windows.Forms.Button();
            this.button_Display = new System.Windows.Forms.Button();
            this.cvControlEx1 = new TST.Vision.Thirdparty.EmguCVControlEx(437, 477);
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
            this.button_Draw.TabIndex = 2;
            this.button_Draw.Text = "Draw ROI";
            this.button_Draw.UseVisualStyleBackColor = true;
            this.button_Draw.Click += new System.EventHandler(this.button_Draw_Click);
            // 
            // button_Zoom
            // 
            this.button_Zoom.Location = new System.Drawing.Point(571, 287);
            this.button_Zoom.Name = "button_Zoom";
            this.button_Zoom.Size = new System.Drawing.Size(143, 60);
            this.button_Zoom.TabIndex = 3;
            this.button_Zoom.Text = "ZOOM";
            this.button_Zoom.UseVisualStyleBackColor = true;
            this.button_Zoom.Click += new System.EventHandler(this.button_Zoom_Click);
            // 
            // button_Draw_irregular ROI
            // 
            this.button_Irregular.Location = new System.Drawing.Point(571, 400);
            this.button_Irregular.Name = "button_Irregular";
            this.button_Irregular.Size = new System.Drawing.Size(143, 60);
            this.button_Irregular.TabIndex = 3;
            this.button_Irregular.Text = "Irregular ROI";
            this.button_Irregular.UseVisualStyleBackColor = true;
            this.button_Irregular.Click += new System.EventHandler(this.button_Irregular_Click);
            // 
            // button_Draw_contour
            // 
            this.button_Contour.Location = new System.Drawing.Point(571, 500);
            this.button_Contour.Name = "button_Contour";
            this.button_Contour.Size = new System.Drawing.Size(143, 60);
            this.button_Contour.TabIndex = 3;
            this.button_Contour.Text = "Draw Contour";
            this.button_Contour.UseVisualStyleBackColor = true;
            this.button_Contour.Click += new System.EventHandler(this.button_Contour_Click);
            //
            // button_Move
            // 
            this.button_Move.Location = new System.Drawing.Point(762, 74);
            this.button_Move.Name = "button_Move";
            this.button_Move.Size = new System.Drawing.Size(129, 60);
            this.button_Move.TabIndex = 4;
            this.button_Move.Text = "Move";
            this.button_Move.UseVisualStyleBackColor = true;
            this.button_Move.Click += new System.EventHandler(this.button_Move_Click);
            // 
            // button_Display
            // 
            this.button_Display.Location = new System.Drawing.Point(762, 180);
            this.button_Display.Name = "button_Display";
            this.button_Display.Size = new System.Drawing.Size(129, 60);
            this.button_Display.TabIndex = 5;
            this.button_Display.Text = "Display message";
            this.button_Display.UseVisualStyleBackColor = true;
            this.button_Display.Click += new System.EventHandler(this.button_Display_Click);
            // 
            // cvControlEx1
            // 
            this.cvControlEx1.BackColor = System.Drawing.Color.Black;
            this.cvControlEx1.Location = new System.Drawing.Point(41, 65);
            this.cvControlEx1.Name = "cvControlEx1";
            this.cvControlEx1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(927, 697);
            this.Controls.Add(this.button_Display);
            this.Controls.Add(this.button_Move);
            this.Controls.Add(this.Open);
            this.Controls.Add(this.button_Draw);
            this.Controls.Add(this.button_Zoom);
            this.Controls.Add(this.button_Irregular);
            this.Controls.Add(this.button_Contour);
            this.Controls.Add(this.cvControlEx1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Open;
        private System.Windows.Forms.Button button_Draw;
        private System.Windows.Forms.Button button_Zoom;
        private System.Windows.Forms.Button button_Irregular;
        private System.Windows.Forms.Button button_Contour;
        private TST.Vision.Thirdparty.EmguCVControlEx cvControlEx1;
        private System.Windows.Forms.Button button_Move;
        private System.Windows.Forms.Button button_Display;
    }
}

