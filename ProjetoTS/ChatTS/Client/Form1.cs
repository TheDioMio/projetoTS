using EI.SI;
using MessagePack;
using Server.Models;
using Shared;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        private ServerConnection serverConnection; // Reutiliza a conexão
        private ProtocolSI protocolSI;
        
        // Recebe a instância do ServerConnection no construtor
        public Form1(ServerConnection connection)
        {
            InitializeComponent();

            serverConnection = connection ?? throw new ArgumentNullException(nameof(connection));
            protocolSI = new ProtocolSI();
        }


        public void GetAllUsers()
        {
            var generalMessage = new GeneralMessage
            {
                Type = "allusers",
            };

            try
            {
                // Serializa a mensagem geral e envia para o servidor
                byte[] serializedGeneralMessage = MessagePack.MessagePackSerializer.Serialize(generalMessage);
                byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedGeneralMessage);
                serverConnection.SendMessage(packet); // Usa o método da conexão

                // Recebe a resposta do servidor
                byte[] responsePacket = serverConnection.ReceiveMessage();

                // Desserializa a resposta do servidor
                var serverResponse = MessagePack.MessagePackSerializer.Deserialize<ServerResponse>(responsePacket);

                MessageBox.Show($"Servidor respondeu: {serverResponse.Message}");

                if (generalMessage.Type == "allusers")
                {
                    List<UserListFormat> usersList = MessagePack.MessagePackSerializer.Deserialize<List<UserListFormat>>(generalMessage.Body);
                    // Agora, a lista 'users' contém todos os usuários. actualiza a vista
                    listBoxUsers.DataSource = null;
                    listBoxUsers.DataSource = usersList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao enviar mensagem: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        private void buttonSend_Click(object sender, EventArgs e)
        {
            string name = textBoxRoomName.Text;

            // Verifica se o campo de texto está preenchido
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Por favor, insira um nome válido para a sala.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Cria a mensagem para criar uma sala
            var roomCreateMessage = new MessageRoomCreate
            {
                Action = "roomcreate",
                Name = name,
                IdUser = ServerConnection.UserSelected

            };

            // Cria uma mensagem geral com a mensagem específica serializada
            var generalMessage = new GeneralMessage
            {
                Type = "roomcreate",
                Body = MessagePack.MessagePackSerializer.Serialize(roomCreateMessage)
            };

            try
            {
                // Serializa a mensagem geral e envia para o servidor
                byte[] serializedGeneralMessage = MessagePack.MessagePackSerializer.Serialize(generalMessage);
                byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedGeneralMessage);
                serverConnection.SendMessage(packet); // Usa o método da conexão

                // Recebe a resposta do servidor
                byte[] responsePacket = serverConnection.ReceiveMessage();

                // Desserializa a resposta do servidor
                var serverResponse = MessagePack.MessagePackSerializer.Deserialize<ServerResponse>(responsePacket);

                MessageBox.Show($"Servidor respondeu: {serverResponse.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao enviar mensagem: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonQuit_Click(object sender, EventArgs e)
        {
            CloseClient();
            this.Close();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseClient();
        }

        private void CloseClient()
        {
            try
            {
                // Envia uma mensagem de término da sessão
                serverConnection.Disconnect();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao desconectar: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            //GetAllUsers();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetAllUsers();
        }
    }
}
