namespace Client
{
    partial class frmLogin
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
            this.btLogin = new System.Windows.Forms.Button();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.labelPassLogin = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.labelUserLogin = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btRegist = new System.Windows.Forms.Button();
            this.btSubmit = new System.Windows.Forms.Button();
            this.txtPassRgst = new System.Windows.Forms.TextBox();
            this.labelPassRgst = new System.Windows.Forms.Label();
            this.txtUserRgst = new System.Windows.Forms.TextBox();
            this.labelUserRgst = new System.Windows.Forms.Label();
            this.labelPassConfirm = new System.Windows.Forms.Label();
            this.txtConfirmPassRgst = new System.Windows.Forms.TextBox();
            this.labelThankYou = new System.Windows.Forms.Label();
            this.btVoltar = new System.Windows.Forms.Button();
            this.txtNomeRgst = new System.Windows.Forms.TextBox();
            this.labelNomeRgst = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btLogin
            // 
            this.btLogin.Location = new System.Drawing.Point(229, 122);
            this.btLogin.Name = "btLogin";
            this.btLogin.Size = new System.Drawing.Size(75, 23);
            this.btLogin.TabIndex = 9;
            this.btLogin.Text = "Login";
            this.btLogin.UseVisualStyleBackColor = true;
            this.btLogin.Click += new System.EventHandler(this.btLogin_Click);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(151, 84);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(138, 20);
            this.txtPassword.TabIndex = 8;
            // 
            // labelPassLogin
            // 
            this.labelPassLogin.AutoSize = true;
            this.labelPassLogin.Location = new System.Drawing.Point(86, 88);
            this.labelPassLogin.Name = "labelPassLogin";
            this.labelPassLogin.Size = new System.Drawing.Size(56, 13);
            this.labelPassLogin.TabIndex = 7;
            this.labelPassLogin.Text = "Password:";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(151, 50);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(138, 20);
            this.txtUsername.TabIndex = 6;
            // 
            // labelUserLogin
            // 
            this.labelUserLogin.AutoSize = true;
            this.labelUserLogin.Location = new System.Drawing.Point(86, 53);
            this.labelUserLogin.Name = "labelUserLogin";
            this.labelUserLogin.Size = new System.Drawing.Size(58, 13);
            this.labelUserLogin.TabIndex = 5;
            this.labelUserLogin.Text = "Username:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(169, 185);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Formulário de Registo";
            // 
            // btRegist
            // 
            this.btRegist.Location = new System.Drawing.Point(133, 122);
            this.btRegist.Name = "btRegist";
            this.btRegist.Size = new System.Drawing.Size(75, 23);
            this.btRegist.TabIndex = 11;
            this.btRegist.Text = "Registo";
            this.btRegist.UseVisualStyleBackColor = true;
            this.btRegist.Click += new System.EventHandler(this.btRegist_Click);
            // 
            // btSubmit
            // 
            this.btSubmit.Location = new System.Drawing.Point(171, 370);
            this.btSubmit.Name = "btSubmit";
            this.btSubmit.Size = new System.Drawing.Size(89, 23);
            this.btSubmit.TabIndex = 11;
            this.btSubmit.Text = "Submeter";
            this.btSubmit.UseVisualStyleBackColor = true;
            this.btSubmit.Click += new System.EventHandler(this.btSubmit_Click);
            // 
            // txtPassRgst
            // 
            this.txtPassRgst.Location = new System.Drawing.Point(151, 278);
            this.txtPassRgst.Name = "txtPassRgst";
            this.txtPassRgst.Size = new System.Drawing.Size(138, 20);
            this.txtPassRgst.TabIndex = 15;
            // 
            // labelPassRgst
            // 
            this.labelPassRgst.AutoSize = true;
            this.labelPassRgst.Location = new System.Drawing.Point(86, 281);
            this.labelPassRgst.Name = "labelPassRgst";
            this.labelPassRgst.Size = new System.Drawing.Size(56, 13);
            this.labelPassRgst.TabIndex = 14;
            this.labelPassRgst.Text = "Password:";
            // 
            // txtUserRgst
            // 
            this.txtUserRgst.Location = new System.Drawing.Point(151, 243);
            this.txtUserRgst.Name = "txtUserRgst";
            this.txtUserRgst.Size = new System.Drawing.Size(138, 20);
            this.txtUserRgst.TabIndex = 13;
            // 
            // labelUserRgst
            // 
            this.labelUserRgst.AutoSize = true;
            this.labelUserRgst.Location = new System.Drawing.Point(86, 246);
            this.labelUserRgst.Name = "labelUserRgst";
            this.labelUserRgst.Size = new System.Drawing.Size(58, 13);
            this.labelUserRgst.TabIndex = 12;
            this.labelUserRgst.Text = "Username:";
            // 
            // labelPassConfirm
            // 
            this.labelPassConfirm.AutoSize = true;
            this.labelPassConfirm.Location = new System.Drawing.Point(41, 317);
            this.labelPassConfirm.Name = "labelPassConfirm";
            this.labelPassConfirm.Size = new System.Drawing.Size(103, 13);
            this.labelPassConfirm.TabIndex = 14;
            this.labelPassConfirm.Text = "Confirmar Password:";
            // 
            // txtConfirmPassRgst
            // 
            this.txtConfirmPassRgst.Location = new System.Drawing.Point(151, 312);
            this.txtConfirmPassRgst.Name = "txtConfirmPassRgst";
            this.txtConfirmPassRgst.Size = new System.Drawing.Size(138, 20);
            this.txtConfirmPassRgst.TabIndex = 15;
            // 
            // labelThankYou
            // 
            this.labelThankYou.AutoSize = true;
            this.labelThankYou.Location = new System.Drawing.Point(148, 336);
            this.labelThankYou.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelThankYou.Name = "labelThankYou";
            this.labelThankYou.Size = new System.Drawing.Size(125, 13);
            this.labelThankYou.TabIndex = 16;
            this.labelThankYou.Text = "Obrigado Por se Registar";
            this.labelThankYou.Visible = false;
            // 
            // btVoltar
            // 
            this.btVoltar.Location = new System.Drawing.Point(171, 370);
            this.btVoltar.Name = "btVoltar";
            this.btVoltar.Size = new System.Drawing.Size(89, 23);
            this.btVoltar.TabIndex = 17;
            this.btVoltar.Text = "Voltar";
            this.btVoltar.UseVisualStyleBackColor = true;
            this.btVoltar.Visible = false;
            this.btVoltar.Click += new System.EventHandler(this.btVoltar_Click);
            // 
            // txtNomeRgst
            // 
            this.txtNomeRgst.Location = new System.Drawing.Point(151, 210);
            this.txtNomeRgst.Name = "txtNomeRgst";
            this.txtNomeRgst.Size = new System.Drawing.Size(138, 20);
            this.txtNomeRgst.TabIndex = 19;
            // 
            // labelNomeRgst
            // 
            this.labelNomeRgst.AutoSize = true;
            this.labelNomeRgst.Location = new System.Drawing.Point(86, 214);
            this.labelNomeRgst.Name = "labelNomeRgst";
            this.labelNomeRgst.Size = new System.Drawing.Size(38, 13);
            this.labelNomeRgst.TabIndex = 18;
            this.labelNomeRgst.Text = "Nome:";
            // 
            // frmLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 165);
            this.Controls.Add(this.txtNomeRgst);
            this.Controls.Add(this.labelNomeRgst);
            this.Controls.Add(this.btVoltar);
            this.Controls.Add(this.labelThankYou);
            this.Controls.Add(this.txtConfirmPassRgst);
            this.Controls.Add(this.txtPassRgst);
            this.Controls.Add(this.labelPassConfirm);
            this.Controls.Add(this.labelPassRgst);
            this.Controls.Add(this.txtUserRgst);
            this.Controls.Add(this.labelUserRgst);
            this.Controls.Add(this.btSubmit);
            this.Controls.Add(this.btRegist);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btLogin);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.labelPassLogin);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.labelUserLogin);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "frmLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btLogin;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label labelPassLogin;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label labelUserLogin;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btRegist;
        private System.Windows.Forms.Button btSubmit;
        private System.Windows.Forms.TextBox txtPassRgst;
        private System.Windows.Forms.Label labelPassRgst;
        private System.Windows.Forms.TextBox txtUserRgst;
        private System.Windows.Forms.Label labelUserRgst;
        private System.Windows.Forms.Label labelPassConfirm;
        private System.Windows.Forms.TextBox txtConfirmPassRgst;
        private System.Windows.Forms.Label labelThankYou;
        private System.Windows.Forms.Button btVoltar;
        private System.Windows.Forms.TextBox txtNomeRgst;
        private System.Windows.Forms.Label labelNomeRgst;
    }
}