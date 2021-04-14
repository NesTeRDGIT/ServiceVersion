namespace ClientServise.Launcher
{
    partial class Launcher
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Launcher));
            this.listView1 = new System.Windows.Forms.ListView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.progressBarLevel1 = new System.Windows.Forms.ProgressBar();
            this.progressBarMain = new System.Windows.Forms.ProgressBar();
            this.textBlock1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.FullRowSelect = true;
            this.listView1.LabelWrap = false;
            this.listView1.Location = new System.Drawing.Point(12, 12);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(737, 166);
            this.listView1.SmallImageList = this.imageList1;
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.List;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "agt_update_recommended1_2655.png");
            this.imageList1.Images.SetKeyName(1, "tick_circle_8579.png");
            // 
            // progressBarLevel1
            // 
            this.progressBarLevel1.Location = new System.Drawing.Point(12, 216);
            this.progressBarLevel1.Name = "progressBarLevel1";
            this.progressBarLevel1.Size = new System.Drawing.Size(737, 23);
            this.progressBarLevel1.TabIndex = 1;
            // 
            // progressBarMain
            // 
            this.progressBarMain.Location = new System.Drawing.Point(12, 266);
            this.progressBarMain.Name = "progressBarMain";
            this.progressBarMain.Size = new System.Drawing.Size(737, 23);
            this.progressBarMain.TabIndex = 2;
            // 
            // textBlock1
            // 
            this.textBlock1.Location = new System.Drawing.Point(12, 292);
            this.textBlock1.Name = "textBlock1";
            this.textBlock1.Size = new System.Drawing.Size(737, 19);
            this.textBlock1.TabIndex = 3;
            this.textBlock1.Text = "Статус";
            this.textBlock1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Launcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.PaleGreen;
            this.ClientSize = new System.Drawing.Size(761, 353);
            this.Controls.Add(this.textBlock1);
            this.Controls.Add(this.progressBarMain);
            this.Controls.Add(this.progressBarLevel1);
            this.Controls.Add(this.listView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Launcher";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.Launcher_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBarLevel1;
        private System.Windows.Forms.ProgressBar progressBarMain;
        private System.Windows.Forms.Label textBlock1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ListView listView1;
    }
}