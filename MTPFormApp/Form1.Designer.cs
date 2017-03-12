namespace MTPFormApp
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.ReloadBtn = new System.Windows.Forms.Button();
            this.ListView = new System.Windows.Forms.ListView();
            this.UpdBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ReloadBtn
            // 
            this.ReloadBtn.Location = new System.Drawing.Point(13, 346);
            this.ReloadBtn.Name = "ReloadBtn";
            this.ReloadBtn.Size = new System.Drawing.Size(75, 23);
            this.ReloadBtn.TabIndex = 1;
            this.ReloadBtn.Text = "更新";
            this.ReloadBtn.UseVisualStyleBackColor = true;
            this.ReloadBtn.Click += new System.EventHandler(this.ReloadBtn_Clicked);
            // 
            // ListView
            // 
            this.ListView.Location = new System.Drawing.Point(13, 3);
            this.ListView.Name = "ListView";
            this.ListView.Size = new System.Drawing.Size(340, 337);
            this.ListView.TabIndex = 2;
            this.ListView.UseCompatibleStateImageBehavior = false;
            this.ListView.DoubleClick += new System.EventHandler(this.ListVIew_DoubleClicked);
            // 
            // UpdBtn
            // 
            this.UpdBtn.Location = new System.Drawing.Point(278, 346);
            this.UpdBtn.Name = "UpdBtn";
            this.UpdBtn.Size = new System.Drawing.Size(75, 23);
            this.UpdBtn.TabIndex = 3;
            this.UpdBtn.Text = "アップロード";
            this.UpdBtn.UseVisualStyleBackColor = true;
            this.UpdBtn.Click += new System.EventHandler(this.UpdBtn_Clicked);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(365, 381);
            this.Controls.Add(this.UpdBtn);
            this.Controls.Add(this.ListView);
            this.Controls.Add(this.ReloadBtn);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Activated += new System.EventHandler(this.Form1_Activated);
            this.Deactivate += new System.EventHandler(this.Form1_Deactived);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button ReloadBtn;
        private System.Windows.Forms.ListView ListView;
        private System.Windows.Forms.Button UpdBtn;
    }
}

