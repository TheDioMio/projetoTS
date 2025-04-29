using EI.SI;
using MessagePack;
using Server.Models;
using Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Sockets;
using System.Threading;
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

            // Inicia a escuta de mensagens do servidor
            StartListening();

        }

        public void StartListening()
        {
            Thread listenThread = new Thread(ListenForMessages);
            listenThread.IsBackground = true;
            Console.WriteLine("Thread de escuta iniciada!");
            listenThread.Start();
        }

        private void ListenForMessages()
        {
            try
            {
                while (serverConnection.IsConnected)
                {
                   
                    // Recebe a resposta do servidor
                    byte[] responsePacket = serverConnection.ReceiveMessage();

                    if (responsePacket.Length == 0)
                    {
                        Console.WriteLine("Nenhum dado recebido. Finalizando escuta...");
                        break;
                    }
                    Console.WriteLine("Mensagem recebida do servidor!");
                    Console.WriteLine("Dados recebidos do servidor: " + BitConverter.ToString(responsePacket));
                    // Desserializa a resposta para um objeto do tipo GeneralMessage
                    GeneralMessage responseMessage = MessagePack.MessagePackSerializer.Deserialize<GeneralMessage>(responsePacket);
                    if (responsePacket.Length > 0)
                    {

                        switch (responseMessage.Type)
                        {
                            case "sendmessage":
                                Console.WriteLine("Estas no sendMessage do Form1");

                                try
                                {
                                    Console.WriteLine("Body recebido (Base64): " + Convert.ToBase64String(responseMessage.Body));

                                    ServerResponse sendMessageResponse = MessagePack.MessagePackSerializer.Deserialize<ServerResponse>(responseMessage.Body);

                                    if (sendMessageResponse.Success)
                                    {
                                        this.Invoke((MethodInvoker)delegate
                                        {
                                            Console.WriteLine("Mensagem enviada com sucesso.");
                                            //messagesInRoom(ServerConnection.RoomSelected);

                                        });
                                }
                                    else
                                    {
                                        Console.WriteLine("Erro ao enviar mensagem: " + sendMessageResponse.Message);
                                    }
                                    messagesInRoom(ServerConnection.RoomSelected);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Erro ao desserializar ServerResponse: " + ex.Message);
                                }

                                break;

                            case "UpdateMessages":
                                try
                                {
                                    Console.WriteLine("Estas no updateMenssages do Form1");
                                    Console.WriteLine("Body recebido (Base64): " + Convert.ToBase64String(responseMessage.Body));

                                    UpdateRequest updateMessageResponse = MessagePack.MessagePackSerializer.Deserialize<UpdateRequest>(responseMessage.Body);
                                    //var updateMessageResponse = MessagePack.MessagePackSerializer.Deserialize<UpdateRequest>(responseMessage.Body);
                                    if (updateMessageResponse.IdRoom == ServerConnection.RoomSelected)
                                    {
                                        this.Invoke((MethodInvoker)delegate
                                        {
                                            messagesInRoom(ServerConnection.RoomSelected);
                                        });
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Erro ao desserializar UpdateRequest: " + ex.Message);
                                }

                                break;


                            case "errorResponse":
                                ServerResponse message = MessagePack.MessagePackSerializer.Deserialize<ServerResponse>(responseMessage.Body);
                                if (message.Success == false)
                                {
                                    this.Invoke((MethodInvoker)delegate
                                    {
                                        MessageBox.Show(message.Message);
                                    });
                                }

                                break;


                            case "roomsOfUser":
                                // Desserializa o Body para uma lista de salas (RoomListFormat)
                                List<RoomListFormat> roomsList = MessagePack.MessagePackSerializer.Deserialize<List<RoomListFormat>>(responseMessage.Body);

                                // Atualiza a interface: Exemplo usando ListBox (listBoxRooms)
                                if (roomsList.Count>0)
                                {
                                    this.Invoke((MethodInvoker)delegate
                                    {
                                        listBoxRooms.DataSource = null;
                                        listBoxRooms.DataSource = roomsList;
                                        listBoxRooms.DisplayMember = "Name";
                                    });
                                }
                                


                                break;
                                
                            case "messagesInRoom":
                                // Desserializa o Body para uma lista de mensagens
                                List<messageFormat> messagesList = MessagePack.MessagePackSerializer.Deserialize<List<messageFormat>>(responseMessage.Body);
                                if ((messagesList != null)&&(messagesList.Count >0))
                                {
                                    //this.Invoke((MethodInvoker)delegate
                                    //{
                                    //    //listBoxMessages_old.DataSource = null;
                                    //    //listBoxMessages_old.DataSource = messagesList;
                                    //    //listBoxMessages_old.DisplayMember = "Text";
                                    //    listBoxMessages.Clear();
                                    //    foreach (var messageLine in messagesList)
                                    //    {
                                    //        // Formata e adiciona as mensagens ao RichTextBox, com o nome do usuário seguido pela mensagem
                                    //        string formattedMessage = $"{messageLine.Text}\n";
                                    //        listBoxMessages.AppendText(formattedMessage);  // Adiciona a mensagem com quebra de linha
                                    //    }
                                    //    listBoxMessages.ScrollToCaret();
                                    //});

                                    //this.Invoke((MethodInvoker)delegate
                                    //{
                                    //    listBoxMessages.Clear();  // Limpa o RichTextBox antes de adicionar novas mensagens

                                    //    foreach (var messageLine in messagesList)
                                    //    {
                                    //        // Verifica se a mensagem é do usuário logado
                                    //        bool isOwnMessage = messageLine.UserId == ServerConnection.UserSelected;  // loggedUserId é o ID do usuário logado

                                    //        // Se for a mensagem do usuário logado, alinha à direita
                                    //        if (isOwnMessage)
                                    //        {
                                    //            listBoxMessages.SelectionAlignment = HorizontalAlignment.Right;  // Alinha a mensagem à direita
                                    //            listBoxMessages.SelectionColor = Color.LightGreen;
                                    //        }
                                    //        else
                                    //        {
                                    //            listBoxMessages.SelectionAlignment = HorizontalAlignment.Left;  // Deixa as outras mensagens alinhadas à esquerda
                                    //            listBoxMessages.SelectionColor = Color.Black;
                                    //        }

                                    //        // Formata a mensagem e adiciona ao RichTextBox
                                    //        string formattedMessage = $"{messageLine.Text}\n";
                                    //        listBoxMessages.AppendText(formattedMessage);

                                    //        // Rolagem automática para o final
                                    //        listBoxMessages.ScrollToCaret();
                                    //    }
                                    //});

                                    this.Invoke((MethodInvoker)delegate
                                    {
                                        listBoxMessages.Clear();  // Limpa o RichTextBox antes de adicionar novas mensagens

                                        foreach (var messageLine in messagesList)
                                        {
                                            // Verifica se a mensagem é do usuário logado
                                            bool isOwnMessage = messageLine.UserId == ServerConnection.UserSelected;  // loggedUserId é o ID do usuário logado

                                            // Se for a mensagem do usuário logado, alinha à direita e define as cores e a fonte
                                            if (isOwnMessage)
                                            {
                                                listBoxMessages.SelectionAlignment = HorizontalAlignment.Right;  // Alinha a mensagem à direita
                                                listBoxMessages.SelectionColor = Color.Yellow;  // Letras amarelas
                                                listBoxMessages.SelectionBackColor = Color.Black;  // Fundo preto
                                            }
                                            else
                                            {
                                                listBoxMessages.SelectionAlignment = HorizontalAlignment.Left;  // Alinha à esquerda
                                                listBoxMessages.SelectionColor = Color.White;  // Letras brancas
                                                listBoxMessages.SelectionBackColor = Color.Red;  // Fundo vermelho
                                            }

                                            // Definir a fonte para algo mais "antigo" como "Courier New" ou "Lucida Console"
                                            listBoxMessages.SelectionFont = new Font("Courier New", 12, FontStyle.Regular);  // Fonte "Courier New", tamanho 12, estilo regular

                                            // Formata a mensagem e adiciona ao RichTextBox
                                            string formattedMessage = $"{messageLine.Text}\n";

                                            // Adiciona a mensagem ao RichTextBox
                                            listBoxMessages.AppendText(formattedMessage);

                                            // Rolagem automática para o final
                                            listBoxMessages.ScrollToCaret();
                                            
                                        }
                                        usersInRoom(ServerConnection.RoomSelected);
                                    });

                                    //fazer um else para esconder a lista de mensagens quando não tem valores para apresentar

                                }
                                break;

                            case "allUsers":
                                if (responseMessage.Body != null && responseMessage.Body.Length > 0)
                                {
                                    // Desserializa o Body para a lista de usuarios
                                    List<UserListFormat> usersList = MessagePack.MessagePackSerializer.Deserialize<List<UserListFormat>>(responseMessage.Body);

                                    // Atualiza a interface: Exemplo usando ListBox
                                    this.Invoke((MethodInvoker)delegate
                                    {
                                        listBoxUsers.DataSource = null;
                                        listBoxUsers.DataSource = usersList;
                                    });
                                }
                                break;

                            case "userAddRoom":
                                ServerResponse messageAddRoom = MessagePack.MessagePackSerializer.Deserialize<ServerResponse>(responseMessage.Body);
                                if (messageAddRoom.Success)
                                {
                                    //chama a funcao para atualizar a vista
                                    usersInRoom(ServerConnection.RoomSelected);
                                }
                                break;

                            case "usersInRoom":
                                // Desserializa o Body para a lista de usuários associados à sala
                                List<UserListFormat> usersInRoomList = MessagePack.MessagePackSerializer.Deserialize<List<UserListFormat>>(responseMessage.Body);

                                // Atualiza a interface: Exemplo usando ListBox

                                this.Invoke((MethodInvoker)delegate
                                {
                                    listBoxUserRoom.DataSource = null;
                                    listBoxUserRoom.DataSource = usersInRoomList;
                                });
                                break;

                            default:
                                break;
                        }

                        Thread.Sleep(100);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na escuta de mensagens: {ex.Message}");
            }
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao enviar mensagem: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //aqui implementar a logica do que queremos ver
            //ou selecionamos logo uma sala ou escondemos as mensagens e os usersInRoom
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao enviar mensagem: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //usersInRoom(ServerConnection.RoomSelected);
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
                roomsOfUser(ServerConnection.UserSelected);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao enviar mensagem: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void buttonQuit_Click(object sender, EventArgs e)
        {
            try
            {
                serverConnection.Disconnect();  // Chama a função de desconexão
                MessageBox.Show("Desconectado com sucesso!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao desconectar: {ex.Message}");
            }
            Application.Exit();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

 

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            GetAllUsers();
            Thread.Sleep(100);
            roomsOfUser(ServerConnection.UserSelected);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
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

            // Cria uma mensagem geral 
            GeneralMessage generalMessage = new GeneralMessage
            {
                Type = "sendmessage",
                Body = MessagePack.MessagePackSerializer.Serialize(requestData)
            };

            try
            {
                byte[] serializedGeneralMessage = MessagePack.MessagePackSerializer.Serialize(generalMessage);
                byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedGeneralMessage);
                // Envia o pacote para o servidor
                serverConnection.SendMessage(packet);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao enviar mensagem: {ex.Message}",
                                "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //limpa o campo da mensagem
            textBoxMessage.Text = "";

            ////atualiza a vista das mensagens
            //messagesInRoom(ServerConnection.RoomSelected);

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
            ////atualiza a lista de Users registados na sala
            ///
            
            //usersInRoom(ServerConnection.RoomSelected);
            //messagesInRoom(selectedRoom.Id);

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                serverConnection.Disconnect();  // Garante que o cliente seja desconectado
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao desconectar durante o fechamento: {ex.Message}");
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            roomsOfUser(ServerConnection.UserSelected);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            messagesInRoom(ServerConnection.RoomSelected);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            GetAllUsers();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            usersInRoom(ServerConnection.RoomSelected);
        }
    }
}
