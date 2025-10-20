namespace CNPM
{
    partial class Messenger
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Messenger));
            this.name_user = new System.Windows.Forms.Label();
            this.text_input = new System.Windows.Forms.TextBox();
            this.flowLayoutPanelMessages = new System.Windows.Forms.FlowLayoutPanel();
            this.pb_reset = new System.Windows.Forms.PictureBox();
            this.close = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.send_message = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pb_reset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.close)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.send_message)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // name_user
            // 
            this.name_user.AutoSize = true;
            this.name_user.BackColor = System.Drawing.Color.Transparent;
            this.name_user.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.name_user.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(164)))), ((int)(((byte)(255)))));
            this.name_user.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.name_user.Location = new System.Drawing.Point(142, 42);
            this.name_user.Name = "name_user";
            this.name_user.Size = new System.Drawing.Size(98, 32);
            this.name_user.TabIndex = 1;
            this.name_user.Text = "label1";
            // 
            // text_input
            // 
            this.text_input.Location = new System.Drawing.Point(14, 624);
            this.text_input.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.text_input.Multiline = true;
            this.text_input.Name = "text_input";
            this.text_input.Size = new System.Drawing.Size(432, 50);
            this.text_input.TabIndex = 3;
            this.text_input.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_input_KeyDown);
            // 
            // flowLayoutPanelMessages
            // 
            this.flowLayoutPanelMessages.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(164)))), ((int)(((byte)(255)))));
            this.flowLayoutPanelMessages.Location = new System.Drawing.Point(0, 111);
            this.flowLayoutPanelMessages.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.flowLayoutPanelMessages.Name = "flowLayoutPanelMessages";
            this.flowLayoutPanelMessages.Size = new System.Drawing.Size(566, 504);
            this.flowLayoutPanelMessages.TabIndex = 5;
            this.flowLayoutPanelMessages.Paint += new System.Windows.Forms.PaintEventHandler(this.flowLayoutPanelMessages_Paint);
            // 
            // pb_reset
            // 
            this.pb_reset.BackColor = System.Drawing.Color.White;
            this.pb_reset.Image = ((System.Drawing.Image)(resources.GetObject("pb_reset.Image")));
            this.pb_reset.Location = new System.Drawing.Point(459, 0);
            this.pb_reset.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pb_reset.Name = "pb_reset";
            this.pb_reset.Size = new System.Drawing.Size(50, 41);
            this.pb_reset.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_reset.TabIndex = 0;
            this.pb_reset.TabStop = false;
            this.pb_reset.Click += new System.EventHandler(this.pb_reset_Click);
            // 
            // close
            // 
            this.close.BackColor = System.Drawing.Color.White;
            this.close.Image = ((System.Drawing.Image)(resources.GetObject("close.Image")));
            this.close.Location = new System.Drawing.Point(514, 0);
            this.close.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(52, 41);
            this.close.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.close.TabIndex = 6;
            this.close.TabStop = false;
            this.close.Click += new System.EventHandler(this.close_Click);
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.Color.White;
            //this.pictureBox3.Image = global::QuanLySinhVien.Properties.Resources.usercut1;
            this.pictureBox3.Location = new System.Drawing.Point(14, 11);
            this.pictureBox3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(100, 88);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 0;
            this.pictureBox3.TabStop = false;
            // 
            // send_message
            // 
            this.send_message.Image = ((System.Drawing.Image)(resources.GetObject("send_message.Image")));
            this.send_message.Location = new System.Drawing.Point(473, 619);
            this.send_message.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.send_message.Name = "send_message";
            this.send_message.Size = new System.Drawing.Size(93, 72);
            this.send_message.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.send_message.TabIndex = 4;
            this.send_message.TabStop = false;
            this.send_message.Click += new System.EventHandler(this.send_message_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(566, 115);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // Messenger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(561, 688);
            this.Controls.Add(this.pb_reset);
            this.Controls.Add(this.close);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.flowLayoutPanelMessages);
            this.Controls.Add(this.send_message);
            this.Controls.Add(this.text_input);
            this.Controls.Add(this.name_user);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Messenger";
            this.Text = "Messenger";
            ((System.ComponentModel.ISupportInitialize)(this.pb_reset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.close)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.send_message)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label name_user;
        private System.Windows.Forms.TextBox text_input;
        private System.Windows.Forms.PictureBox send_message;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelMessages;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox close;
        private System.Windows.Forms.PictureBox pb_reset;
    }
}