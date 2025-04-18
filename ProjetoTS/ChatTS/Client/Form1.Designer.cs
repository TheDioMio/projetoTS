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
            this.listBoxMessages = new System.Windows.Forms.ListBox();
            this.listBoxUserRoom = new System.Windows.Forms.ListBox();
            this.listBoxUsers = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.buttonSendMessage = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.adicionarASalaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enviarMensagemPrivadaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button1 = new System.Windows.Forms.Button();
            this.contextMenuStrip1.SuspendLayout();
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
            this.buttonSend.Location = new System.Drawing.Point(305, 12);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(114, 23);
            this.buttonSend.TabIndex = 6;
            this.buttonSend.Text = "Criar Nova Sala";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // textBoxRoomName
            // 
            this.textBoxRoomName.Location = new System.Drawing.Point(12, 12);
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
            // 
            // listBoxMessages
            // 
            this.listBoxMessages.FormattingEnabled = true;
            this.listBoxMessages.Location = new System.Drawing.Point(208, 69);
            this.listBoxMessages.Name = "listBoxMessages";
            this.listBoxMessages.Size = new System.Drawing.Size(370, 290);
            this.listBoxMessages.TabIndex = 9;
            // 
            // listBoxUserRoom
            // 
            this.listBoxUserRoom.FormattingEnabled = true;
            this.listBoxUserRoom.Location = new System.Drawing.Point(586, 70);
            this.listBoxUserRoom.Name = "listBoxUserRoom";
            this.listBoxUserRoom.Size = new System.Drawing.Size(191, 108);
            this.listBoxUserRoom.TabIndex = 10;
            // 
            // listBoxUsers
            // 
            this.listBoxUsers.FormattingEnabled = true;
            this.listBoxUsers.Location = new System.Drawing.Point(586, 208);
            this.listBoxUsers.Name = "listBoxUsers";
            this.listBoxUsers.Size = new System.Drawing.Size(187, 147);
            this.listBoxUsers.TabIndex = 11;
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
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(214, 387);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(364, 21);
            this.textBox1.TabIndex = 15;
            // 
            // buttonSendMessage
            // 
            this.buttonSendMessage.Location = new System.Drawing.Point(474, 413);
            this.buttonSendMessage.Name = "buttonSendMessage";
            this.buttonSendMessage.Size = new System.Drawing.Size(102, 24);
            this.buttonSendMessage.TabIndex = 16;
            this.buttonSendMessage.Text = "Enviar";
            this.buttonSendMessage.UseVisualStyleBackColor = true;
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
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.adicionarASalaToolStripMenuItem,
            this.enviarMensagemPrivadaToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(237, 52);
            // 
            // adicionarASalaToolStripMenuItem
            // 
            this.adicionarASalaToolStripMenuItem.Name = "adicionarASalaToolStripMenuItem";
            this.adicionarASalaToolStripMenuItem.Size = new System.Drawing.Size(236, 24);
            this.adicionarASalaToolStripMenuItem.Text = "Adicionar a Sala";
            // 
            // enviarMensagemPrivadaToolStripMenuItem
            // 
            this.enviarMensagemPrivadaToolStripMenuItem.Name = "enviarMensagemPrivadaToolStripMenuItem";
            this.enviarMensagemPrivadaToolStripMenuItem.Size = new System.Drawing.Size(236, 24);
            this.enviarMensagemPrivadaToolStripMenuItem.Text = "Enviar Mensagem Privada";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(500, 19);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 27);
            this.button1.TabIndex = 19;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(785, 447);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonSendMessage);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBoxUsers);
            this.Controls.Add(this.listBoxUserRoom);
            this.Controls.Add(this.listBoxMessages);
            this.Controls.Add(this.listBoxRooms);
            this.Controls.Add(this.buttonQuit);
            this.Controls.Add(this.buttonSend);
            this.Controls.Add(this.textBoxRoomName);
            this.Controls.Add(this.labelMessage);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonQuit;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.TextBox textBoxRoomName;
        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.ListBox listBoxRooms;
        private System.Windows.Forms.ListBox listBoxMessages;
        private System.Windows.Forms.ListBox listBoxUserRoom;
        private System.Windows.Forms.ListBox listBoxUsers;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button buttonSendMessage;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem adicionarASalaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enviarMensagemPrivadaToolStripMenuItem;
        private System.Windows.Forms.Button button1;
    }
}

