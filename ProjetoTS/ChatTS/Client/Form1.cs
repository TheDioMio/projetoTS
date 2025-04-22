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

        public void roomsOfUser(int userId)
        {
            // formata a mensagem com o userId que se deseja
            roomsOfUserFormat requestData = new roomsOfUserFormat
            {
                UserId = userId
            };

            // Cria uma mensagem geral com o tipo "roomsOfUser"
            // e embute o objeto de requisição no campo Body
            GeneralMessage requestMessage = new GeneralMessage
            {
                Type = "roomsOfUser",
                Body = MessagePack.MessagePackSerializer.Serialize(requestData)
            };

            try
            {
                // Serializa a mensagem geral e monta o pacote para envio
                byte[] serializedRequest = MessagePack.MessagePackSerializer.Serialize(requestMessage);
                byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedRequest);
                serverConnection.SendMessage(packet);

                // Recebe a resposta do servidor
                byte[] responsePacket = serverConnection.ReceiveMessage();

                // Desserializa a resposta para um objeto do tipo GeneralMessage
                GeneralMessage responseMessage = MessagePack.MessagePackSerializer.Deserialize<GeneralMessage>(responsePacket);

                // Verifica se o tipo da resposta é "roomsOfUser"
                if (responseMessage.Type == "roomsOfUser")
                {
                    // Desserializa o Body para uma lista de salas (RoomListFormat)
                    List<RoomListFormat> roomsList = MessagePack.MessagePackSerializer.Deserialize<List<RoomListFormat>>(responseMessage.Body);

                    // Atualiza a interface: Exemplo usando ListBox (listBoxRooms)
                    listBoxRooms.DataSource = null;
                    listBoxRooms.DataSource = roomsList;
                    listBoxRooms.DisplayMember = "Name"; // Exibe o nome da sala
                }
                else
                {
                    MessageBox.Show("Resposta do servidor não corresponde ao pedido de salas do usuário.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao enviar mensagem: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        public void messagesInRoom(int roomId)
        {
            // Cria o objeto de requisição com o RoomId desejado
            messagesInRoomFormat requestData = new messagesInRoomFormat
            {
                RoomId = roomId
            };

            // Cria uma mensagem geral com o tipo "messagesInRoom" e embute o objeto de requisição no campo Body
            GeneralMessage requestMessage = new GeneralMessage
            {
                Type = "messagesInRoom",
                Body = MessagePack.MessagePackSerializer.Serialize(requestData)
            };

            try
            {
                // Serializa a mensagem geral e monta o pacote para envio
                byte[] serializedRequest = MessagePack.MessagePackSerializer.Serialize(requestMessage);
                byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedRequest);
                serverConnection.SendMessage(packet);

                // Recebe a resposta do servidor
                byte[] responsePacket = serverConnection.ReceiveMessage();

                // Desserializa a resposta para um objeto do tipo GeneralMessage
                GeneralMessage responseMessage = MessagePack.MessagePackSerializer.Deserialize<GeneralMessage>(responsePacket);

                // Verifica se o tipo da resposta é "messagesInRoom"
                if (responseMessage.Type == "messagesInRoom")
                {
                    // Desserializa o Body para uma lista de mensagens
                    List<messageFormat> messagesList = MessagePack.MessagePackSerializer.Deserialize<List<messageFormat>>(responseMessage.Body);
                    if (messagesList == null)
                    {
                        listBoxMessages.DataSource = null;
                    }
                    else
                    {
                        // Atualiza a interface, por exemplo, atribuindo a lista a um ListBox (listBoxMessages)
                        listBoxMessages.DataSource = null;
                        listBoxMessages.DataSource = messagesList;
                        //listBoxMessages.DisplayMember = "Text"; // Ou, se preferir, sobrescreva ToString() na classe messageFormat para uma exibição personalizada
                    }
                }
                else
                {
                    MessageBox.Show("Resposta do servidor não corresponde ao pedido de mensagens da sala.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao enviar mensagem: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

                // Desserializa a resposta para um objeto do tipo GeneralMessage
                GeneralMessage responseMessage = MessagePack.MessagePackSerializer.Deserialize<GeneralMessage>(responsePacket);

                // Verifica se a resposta é do tipo "allusers"
                if (responseMessage.Type == "allusers")
                {
                    // Desserializa o Body para a lista de usuarios
                    List<UserListFormat> usersList = MessagePack.MessagePackSerializer.Deserialize<List<UserListFormat>>(responseMessage.Body);

                    // Atualiza a interface: Exemplo usando ListBox
                    listBoxUsers.DataSource = null;
                    listBoxUsers.DataSource = usersList;
                }
                else
                {
                    MessageBox.Show("Resposta do servidor não corresponde ao pedido de listagem de usuários.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao enviar mensagem: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        public void usersInRoom(int roomId)
        {
            // Vaia ao serverconnection e vai ver o ID da Room que está selecionado e pede ao servidor a lista de users naquela sala
            usersInRoomFormat idRoom = new usersInRoomFormat { RoomId = ServerConnection.RoomSelected };

            // Cria a mensagem geral, embutindo a requisição no campo Body
            var requestMessage = new GeneralMessage
            {
                Type = "usersInRoom",
                Body = MessagePack.MessagePackSerializer.Serialize(idRoom)
            };

            try
            {
                // Serializa a mensagem geral e envia para o servidor
                byte[] serializedRequest = MessagePack.MessagePackSerializer.Serialize(requestMessage);
                byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedRequest);
                serverConnection.SendMessage(packet); // Envia para o servidor

                // Recebe a resposta do servidor
                byte[] responsePacket = serverConnection.ReceiveMessage();

                // Desserializa a resposta para um objeto do tipo GeneralMessage
                GeneralMessage responseMessage = MessagePack.MessagePackSerializer.Deserialize<GeneralMessage>(responsePacket);

                
                // Verifica se a resposta é do tipo "allusers"
                if (responseMessage.Type == "usersInRoom")
                {
                    // Desserializa o Body para a lista de usuários associados à sala
                    List<UserListFormat> usersList = MessagePack.MessagePackSerializer.Deserialize<List<UserListFormat>>(responseMessage.Body);

                    // Atualiza a interface: Exemplo usando ListBox
                    listBoxUserRoom.DataSource = null;
                    listBoxUserRoom.DataSource = usersList;
                    
                }
                else
                {
                    MessageBox.Show("Resposta do servidor não corresponde ao pedido de listagem de usuários da sala.");
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

                //atualiza a lista de Rooms 
                roomsOfUser(ServerConnection.UserSelected);

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
            roomsOfUser(ServerConnection.UserSelected);
            GetAllUsers();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetAllUsers();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void adicionarASalaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Vamos adicionar um utilizador a uma sala
           
            // Obtém o índice selecionado na listBox de usuários
            int listBoxUserSelected = listBoxUsers.SelectedIndex;

            // Verifica se nenhum usuário foi selecionado ou se não há sala selecionada
            if (listBoxUserSelected < 0 || ServerConnection.RoomSelected == -1)
            {
                MessageBox.Show("Por favor, selecione um usuário e uma sala válida.",
                                "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Como a listBox está vinculada a uma lista de UserListFormat, obtemos o item selecionado
            UserListFormat selectedUser = (UserListFormat)listBoxUsers.SelectedItem;

            // Obtém os IDs relevantes: o ID do usuário a partir do item selecionado e o ID da sala
            int userId = selectedUser.Id;
            int roomId = ServerConnection.RoomSelected;

            // Cria o objeto de requisição para registrar o usuário na sala, usando a classe usersAddRoomFormat
            usersAddRoomFormat requestData = new usersAddRoomFormat
            {
                RoomId = roomId,
                UserId = userId
            };

            // Cria uma mensagem geral com o tipo "usersAddRoom", encapsulando o objeto de requisição no Body
            GeneralMessage generalMessage = new GeneralMessage
            {
                Type = "usersAddRoom",
                Body = MessagePack.MessagePackSerializer.Serialize(requestData)
            };

            try
            {
                // Serializa a mensagem geral e cria o pacote pelo protocolo customizado
                byte[] serializedGeneralMessage = MessagePack.MessagePackSerializer.Serialize(generalMessage);
                byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedGeneralMessage);

                // Envia o pacote para o servidor
                serverConnection.SendMessage(packet);

                // Aguarda e recebe a resposta do servidor
                byte[] responsePacket = serverConnection.ReceiveMessage();

                // Desserializa a resposta para um objeto ServerResponse
                ServerResponse response = MessagePack.MessagePackSerializer.Deserialize<ServerResponse>(responsePacket);

                // Exibe a resposta do servidor
                MessageBox.Show($"Servidor respondeu: {response.Message}",
                                response.Success ? "Sucesso" : "Erro",
                                MessageBoxButtons.OK,
                                response.Success ? MessageBoxIcon.Information : MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao enviar mensagem: {ex.Message}",
                                "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonSendMessage_Click(object sender, EventArgs e)
        {
            //funcao para enviar uma mensagem 

            string text = textBoxMessage.Text;
            int userId = ServerConnection.UserSelected;
            int roomId = ServerConnection.RoomSelected;

            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            // Verifica se nenhum usuário foi selecionado ou se não há sala selecionada
            if (roomId == -1)
            {
                MessageBox.Show("Por favor, selecione uma sala válida.","Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Cria o objeto de requisição para registrar o usuário na sala, usando a classe usersAddRoomFormat
            messageFormat requestData = new messageFormat
            {
                RoomId = roomId,
                UserId = userId,
                Text = text,
                Date = DateTime.Now

            };

            // Cria uma mensagem geral com o tipo "usersAddRoom", encapsulando o objeto de requisição no Body
            GeneralMessage generalMessage = new GeneralMessage
            {
                Type = "sendmessage",
                Body = MessagePack.MessagePackSerializer.Serialize(requestData)
            };

            try
            {
                // Serializa a mensagem geral e cria o pacote pelo protocolo customizado
                byte[] serializedGeneralMessage = MessagePack.MessagePackSerializer.Serialize(generalMessage);
                byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedGeneralMessage);

                // Envia o pacote para o servidor
                serverConnection.SendMessage(packet);

                // Aguarda e recebe a resposta do servidor
                byte[] responsePacket = serverConnection.ReceiveMessage();

                // Desserializa a resposta para um objeto ServerResponse
                ServerResponse response = MessagePack.MessagePackSerializer.Deserialize<ServerResponse>(responsePacket);

                // Exibe a resposta do servidor
                MessageBox.Show($"Servidor respondeu: {response.Message}",
                                response.Success ? "Sucesso" : "Erro",
                                MessageBoxButtons.OK,
                                response.Success ? MessageBoxIcon.Information : MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao enviar mensagem: {ex.Message}",
                                "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //limpa o campo da mensagem
            textBoxMessage.Text = "";
            //atualiza a vista das mensagens
            messagesInRoom(ServerConnection.RoomSelected);

        }

        private void listBoxRooms_SelectedIndexChanged(object sender, EventArgs e)
        {
            //atualiza a lista de mensagens
            int indexList = listBoxRooms.SelectedIndex;
            if (indexList < 0)
            {
                return;
            }
            RoomListFormat selectedRoom = (RoomListFormat)listBoxRooms.SelectedItem;
            ServerConnection.RoomSelected = selectedRoom.Id;
            messagesInRoom(selectedRoom.Id);
            //atualiza a lista de Users registados na sala
            usersInRoom(selectedRoom.Id);

        }
    }
}
