namespace Client
{
    partial class Form1
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.buttonQuit = new System.Windows.Forms.Button();
            this.buttonSend = new System.Windows.Forms.Button();
            this.textBoxRoomName = new System.Windows.Forms.TextBox();
            this.labelMessage = new System.Windows.Forms.Label();
            this.listBoxRooms = new System.Windows.Forms.ListBox();
            this.listBoxUserRoom = new System.Windows.Forms.ListBox();
            this.menuListBoxUsersInRoom = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.excluirUtilizadorDaSalaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listBoxUsers = new System.Windows.Forms.ListBox();
            this.menuListBoxUsers = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.adicionarASalaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enviarMensagemPrivadaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            this.buttonSendMessage = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.listBoxMessages = new System.Windows.Forms.RichTextBox();
            this.labelNew = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.btnAlterarRoom = new System.Windows.Forms.Button();
            this.textBoxNewNameRoom = new System.Windows.Forms.TextBox();
            this.button7 = new System.Windows.Forms.Button();
            this.menuListBoxUsersInRoom.SuspendLayout();
            this.menuListBoxUsers.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonQuit
            // 
            this.buttonQuit.Location = new System.Drawing.Point(698, 414);
            this.buttonQuit.Name = "buttonQuit";
            this.buttonQuit.Size = new System.Drawing.Size(75, 23);
            this.buttonQuit.TabIndex = 7;
            this.buttonQuit.Text = "Sair";
            this.buttonQuit.UseVisualStyleBackColor = true;
            this.buttonQuit.Click += new System.EventHandler(this.buttonQuit_Click);
            // 
            // buttonSend
            // 
            this.buttonSend.Location = new System.Drawing.Point(12, 474);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(114, 23);
            this.buttonSend.TabIndex = 6;
            this.buttonSend.Text = "Criar Nova Sala";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // textBoxRoomName
            // 
            this.textBoxRoomName.Location = new System.Drawing.Point(144, 473);
            this.textBoxRoomName.Multiline = true;
            this.textBoxRoomName.Name = "textBoxRoomName";
            this.textBoxRoomName.Size = new System.Drawing.Size(287, 23);
            this.textBoxRoomName.TabIndex = 5;
            // 
            // labelMessage
            // 
            this.labelMessage.AutoSize = true;
            this.labelMessage.Location = new System.Drawing.Point(12, 51);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(33, 13);
            this.labelMessage.TabIndex = 4;
            this.labelMessage.Text = "Salas";
            // 
            // listBoxRooms
            // 
            this.listBoxRooms.FormattingEnabled = true;
            this.listBoxRooms.Location = new System.Drawing.Point(12, 67);
            this.listBoxRooms.Name = "listBoxRooms";
            this.listBoxRooms.Size = new System.Drawing.Size(186, 342);
            this.listBoxRooms.TabIndex = 8;
            this.listBoxRooms.SelectedIndexChanged += new System.EventHandler(this.listBoxRooms_SelectedIndexChanged);
            // 
            // listBoxUserRoom
            // 
            this.listBoxUserRoom.ContextMenuStrip = this.menuListBoxUsersInRoom;
            this.listBoxUserRoom.FormattingEnabled = true;
            this.listBoxUserRoom.Location = new System.Drawing.Point(586, 70);
            this.listBoxUserRoom.Name = "listBoxUserRoom";
            this.listBoxUserRoom.Size = new System.Drawing.Size(191, 108);
            this.listBoxUserRoom.TabIndex = 10;
            // 
            // menuListBoxUsersInRoom
            // 
            this.menuListBoxUsersInRoom.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.excluirUtilizadorDaSalaToolStripMenuItem});
            this.menuListBoxUsersInRoom.Name = "menuListBoxUsersInRoom";
            this.menuListBoxUsersInRoom.Size = new System.Drawing.Size(224, 28);
            this.menuListBoxUsersInRoom.Opening += new System.ComponentModel.CancelEventHandler(this.menuListBoxUsersInRoom_Opening);
            // 
            // excluirUtilizadorDaSalaToolStripMenuItem
            // 
            this.excluirUtilizadorDaSalaToolStripMenuItem.Name = "excluirUtilizadorDaSalaToolStripMenuItem";
            this.excluirUtilizadorDaSalaToolStripMenuItem.Size = new System.Drawing.Size(223, 24);
            this.excluirUtilizadorDaSalaToolStripMenuItem.Text = "Excluir utilizador da Sala";
            this.excluirUtilizadorDaSalaToolStripMenuItem.Click += new System.EventHandler(this.excluirUtilizadorDaSalaToolStripMenuItem_Click);
            // 
            // listBoxUsers
            // 
            this.listBoxUsers.ContextMenuStrip = this.menuListBoxUsers;
            this.listBoxUsers.FormattingEnabled = true;
            this.listBoxUsers.Location = new System.Drawing.Point(586, 208);
            this.listBoxUsers.Name = "listBoxUsers";
            this.listBoxUsers.Size = new System.Drawing.Size(187, 147);
            this.listBoxUsers.TabIndex = 11;
            // 
            // menuListBoxUsers
            // 
            this.menuListBoxUsers.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.adicionarASalaToolStripMenuItem,
            this.enviarMensagemPrivadaToolStripMenuItem});
            this.menuListBoxUsers.Name = "contextMenuStrip1";
            this.menuListBoxUsers.Size = new System.Drawing.Size(237, 52);
            // 
            // adicionarASalaToolStripMenuItem
            // 
            this.adicionarASalaToolStripMenuItem.Name = "adicionarASalaToolStripMenuItem";
            this.adicionarASalaToolStripMenuItem.Size = new System.Drawing.Size(236, 24);
            this.adicionarASalaToolStripMenuItem.Text = "Adicionar utilizador a Sala";
            this.adicionarASalaToolStripMenuItem.Click += new System.EventHandler(this.adicionarASalaToolStripMenuItem_Click);
            // 
            // enviarMensagemPrivadaToolStripMenuItem
            // 
            this.enviarMensagemPrivadaToolStripMenuItem.Name = "enviarMensagemPrivadaToolStripMenuItem";
            this.enviarMensagemPrivadaToolStripMenuItem.Size = new System.Drawing.Size(236, 24);
            this.enviarMensagemPrivadaToolStripMenuItem.Text = "Enviar Mensagem Privada";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(208, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Mensagens";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(583, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Amigos nesta Sala";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(584, 192);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Todos os Amigos";
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.Location = new System.Drawing.Point(214, 387);
            this.textBoxMessage.Multiline = true;
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.Size = new System.Drawing.Size(364, 21);
            this.textBoxMessage.TabIndex = 15;
            // 
            // buttonSendMessage
            // 
            this.buttonSendMessage.Location = new System.Drawing.Point(474, 413);
            this.buttonSendMessage.Name = "buttonSendMessage";
            this.buttonSendMessage.Size = new System.Drawing.Size(102, 24);
            this.buttonSendMessage.TabIndex = 16;
            this.buttonSendMessage.Text = "Enviar";
            this.buttonSendMessage.UseVisualStyleBackColor = true;
            this.buttonSendMessage.Click += new System.EventHandler(this.buttonSendMessage_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(214, 371);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Mensagem";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(814, 67);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 18;
            this.button1.Text = "Rooms";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(814, 108);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 19;
            this.button2.Text = "Messages";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(814, 155);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 20;
            this.button3.Text = "All Users";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(814, 192);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(91, 23);
            this.button4.TabIndex = 21;
            this.button4.Text = "Users in Room";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // listBoxMessages
            // 
            this.listBoxMessages.BackColor = System.Drawing.SystemColors.InfoText;
            this.listBoxMessages.Location = new System.Drawing.Point(200, 69);
            this.listBoxMessages.Name = "listBoxMessages";
            this.listBoxMessages.Size = new System.Drawing.Size(376, 299);
            this.listBoxMessages.TabIndex = 22;
            this.listBoxMessages.Text = "";
            // 
            // labelNew
            // 
            this.labelNew.AutoSize = true;
            this.labelNew.Location = new System.Drawing.Point(593, 491);
            this.labelNew.Name = "labelNew";
            this.labelNew.Size = new System.Drawing.Size(59, 13);
            this.labelNew.TabIndex = 24;
            this.labelNew.Text = "Mensagem";
            this.labelNew.Visible = false;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(12, 415);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(102, 24);
            this.button5.TabIndex = 25;
            this.button5.Text = "Apagar Sala";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(814, 221);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(75, 23);
            this.button6.TabIndex = 26;
            this.button6.Text = "Banir";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // btnAlterarRoom
            // 
            this.btnAlterarRoom.Location = new System.Drawing.Point(12, 445);
            this.btnAlterarRoom.Name = "btnAlterarRoom";
            this.btnAlterarRoom.Size = new System.Drawing.Size(126, 23);
            this.btnAlterarRoom.TabIndex = 27;
            this.btnAlterarRoom.Text = "Alterar Nome da Sala";
            this.btnAlterarRoom.UseVisualStyleBackColor = true;
            this.btnAlterarRoom.Click += new System.EventHandler(this.btnAlterarRoom_Click);
            // 
            // textBoxNewNameRoom
            // 
            this.textBoxNewNameRoom.Location = new System.Drawing.Point(144, 447);
            this.textBoxNewNameRoom.Name = "textBoxNewNameRoom";
            this.textBoxNewNameRoom.Size = new System.Drawing.Size(158, 20);
            this.textBoxNewNameRoom.TabIndex = 28;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(120, 415);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(89, 24);
            this.button7.TabIndex = 29;
            this.button7.Text = "Sair da Sala";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(917, 513);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.textBoxNewNameRoom);
            this.Controls.Add(this.btnAlterarRoom);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.labelNew);
            this.Controls.Add(this.listBoxMessages);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonSendMessage);
            this.Controls.Add(this.textBoxMessage);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBoxUsers);
            this.Controls.Add(this.listBoxUserRoom);
            this.Controls.Add(this.listBoxRooms);
            this.Controls.Add(this.buttonQuit);
            this.Controls.Add(this.buttonSend);
            this.Controls.Add(this.textBoxRoomName);
            this.Controls.Add(this.labelMessage);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed_1);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.menuListBoxUsersInRoom.ResumeLayout(false);
            this.menuListBoxUsers.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonQuit;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.TextBox textBoxRoomName;
        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.ListBox listBoxRooms;
        private System.Windows.Forms.ListBox listBoxUserRoom;
        private System.Windows.Forms.ListBox listBoxUsers;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxMessage;
        private System.Windows.Forms.Button buttonSendMessage;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ContextMenuStrip menuListBoxUsers;
        private System.Windows.Forms.ToolStripMenuItem adicionarASalaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enviarMensagemPrivadaToolStripMenuItem;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.RichTextBox listBoxMessages;
        private System.Windows.Forms.ContextMenuStrip menuListBoxUsersInRoom;
        private System.Windows.Forms.ToolStripMenuItem excluirUtilizadorDaSalaToolStripMenuItem;
        private System.Windows.Forms.Label labelNew;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button btnAlterarRoom;
        private System.Windows.Forms.TextBox textBoxNewNameRoom;
        private System.Windows.Forms.Button button7;
    }
}

