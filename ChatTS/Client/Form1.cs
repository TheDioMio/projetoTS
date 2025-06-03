using EI.SI;
using MessagePack;
using Server.Models;
using Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Schema;

namespace Client
{
    public partial class Form1 : Form
    {
        private Shared.ServerConnection serverConnection;
        private List<UserListFormat> usersInRoomList;

        //private ProtocolSI protocolSI;

        //private ServerConnection serverConnection; // Reutiliza a conexão
        //private ProtocolSI protocolSI;

        // Recebe a instância do ServerConnection no construtor
        public Form1(ServerConnection connection)
        {
            InitializeComponent();
            serverConnection = Program.GlobalServerConnection;

        }


        public void roomsOfUser(GeneralMessage responseMessage)
        {
           
            // Desserializa o Body para uma lista de salas (RoomListFormat)
            List<RoomListFormat> roomsList = MessagePack.MessagePackSerializer.Deserialize<List<RoomListFormat>>(responseMessage.Body);

            // Atualiza a interface usando ListBox se houver salas na lista
            if (roomsList.Count > 0)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    RoomListFormat selected = (RoomListFormat)listBoxRooms.SelectedItem;
                    listBoxRooms.DataSource = null;
                    listBoxRooms.DataSource = roomsList;
                    listBoxRooms.DisplayMember = "Name";
                    try
                    {
                        listBoxRooms.SelectedItem = selected;
                    }
                    catch (Exception)
                    {
                        listBoxRooms.ClearSelected();                       
                    }    
                });
            }
        }

        public void addOneMessage(messageFormat message)
        {
            this.Invoke((MethodInvoker)delegate
            {
                
                // Verifica se a mensagem é do usuário logado
                bool isOwnMessage = message.UserId == ServerConnection.UserSelected;

                if (isOwnMessage)
                {
                    listBoxMessages.SelectionAlignment = HorizontalAlignment.Right;
                    listBoxMessages.SelectionColor = Color.Yellow;
                    listBoxMessages.SelectionBackColor = Color.Black;
                }
                else
                {
                    listBoxMessages.SelectionAlignment = HorizontalAlignment.Left;
                    listBoxMessages.SelectionColor = Color.White;
                    listBoxMessages.SelectionBackColor = Color.Red;
                }

                listBoxMessages.SelectionFont = new Font("Courier New", 12, FontStyle.Regular);
                string formattedMessage = $"{message.UserName}\n{message.Text}\n";
                listBoxMessages.AppendText(formattedMessage);
            });
        }


        public void messagesInRoom(GeneralMessage responseMessage)
        {
            // Desserializa o Body para uma lista de mensagens
            List<messageFormat> messagesList = MessagePack.MessagePackSerializer.Deserialize<List<messageFormat>>(responseMessage.Body);
            listBoxMessages.Clear();
            
            if (messagesList != null && messagesList.Count > 0)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    listBoxMessages.Visible = false;
                    
                    listBoxMessages.Clear();

                    foreach (var messageLine in messagesList)
                    {
                        addOneMessage(messageLine);
                    }

                    listBoxMessages.ScrollToCaret();
                    listBoxMessages.Visible = true;
                });
                usersInRoom(ServerConnection.RoomSelected);
            }
            else
            {
                this.Invoke((MethodInvoker)delegate {
                    listBoxMessages.Visible = false;
                });
            }
        }

        public void updateRooms()
        {
            roomsOfUserRequest(ServerConnection.UserSelected);
        }

        public void allUsers(GeneralMessage responseMessage)
        {
            // Verifica se o Body não está vazio
            if (responseMessage.Body != null && responseMessage.Body.Length > 0)
            {
                // Desserializa o Body para a lista de usuários
                List<UserListFormat> usersList = MessagePack.MessagePackSerializer.Deserialize<List<UserListFormat>>(responseMessage.Body);

                this.Invoke((MethodInvoker)delegate
                {
                    listBoxUsers.DataSource = null;
                    listBoxUsers.DataSource = usersList;
                });
            }
        }

        public void userAddRoom(GeneralMessage responseMessage)
        {
            // Desserializa uma resposta do servidor
            ServerResponse messageAddRoom = MessagePack.MessagePackSerializer.Deserialize<ServerResponse>(responseMessage.Body);
            if (messageAddRoom.Success)
            {
                // Se houver sucesso, você pode atualizar a interface ou chamar outro método
                // Exemplo: atualizar usuários da sala, se for o caso
                // OBS.: Se o método "usersInRoom" for chamado com um outro parâmetro (como o ID da sala),
                // considere ajustar a assinatura ou manter a atualização da lógica separada.
            }
        }

        public void usersInRoom(GeneralMessage responseMessage)
        {
            // Desserializa o Body para a lista de usuários que estão na sala
            usersInRoomList = MessagePack.MessagePackSerializer.Deserialize<List<UserListFormat>>(responseMessage.Body);

            this.Invoke((MethodInvoker)delegate
            {
                listBoxUserRoom.DataSource = null;
                listBoxUserRoom.DataSource = usersInRoomList;
            });
        }


        public void sendMessage(GeneralMessage responseMessage)
        {
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
                    });
                }
                else
                {
                    Console.WriteLine("Erro ao enviar mensagem: " + sendMessageResponse.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao desserializar ServerResponse: " + ex.Message);
            }

        }

        public void updateMessages(GeneralMessage responseMessage)
        {
            Console.WriteLine("Estas no UpdateMessages do Form1");

            try
            {
                Console.WriteLine("Estas no updateMenssages do Form1");
                Console.WriteLine("Body recebido (Base64): " + Convert.ToBase64String(responseMessage.Body));

                // Desserializa o Body para um objeto do tipo UpdateRequest
                messageFormat updateMessageResponse = MessagePack.MessagePackSerializer.Deserialize<messageFormat>(responseMessage.Body);

                // Se a mensagem for para a sala selecionada, atualiza as mensagens
                if (updateMessageResponse.RoomId == ServerConnection.RoomSelected)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        //messagesInRoom(ServerConnection.RoomSelected);
                        addOneMessage(updateMessageResponse);
                        listBoxMessages.ScrollToCaret();
                    });
                    listBoxMessages.Visible = true;
                }
                else
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        //messagesInRoom(ServerConnection.RoomSelected);
                        labelNew.Text = "Recebeu uma nova mensagem na Sala: "+updateMessageResponse.RoomId.ToString();
                        labelNew.Visible= true;

                        NotifyIcon notifyIcon = new NotifyIcon();
                        notifyIcon.Icon = SystemIcons.Information;
                        notifyIcon.Text = "Nova Mensagem";
                        notifyIcon.Visible = true;
                        notifyIcon.ShowBalloonTip(
                            5000, 
                            "Recebeu um nova mensagem", 
                            updateMessageResponse.Text, 
                            ToolTipIcon.Info 
                        );
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao desserializar UpdateRequest: " + ex.Message);
            }
        }

        public void errorResponse(GeneralMessage responseMessage)
        {
            // Desserializa o Body para um objeto do tipo ServerResponse
            ServerResponse messageResponse = MessagePack.MessagePackSerializer.Deserialize<ServerResponse>(responseMessage.Body);

            if (!messageResponse.Success)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show(messageResponse.Message);
                });
            }
        }




        public void roomsOfUserRequest(int userId)
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
                byte[] packet = MessagePack.MessagePackSerializer.Serialize(requestMessage);
                //byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedRequest);
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
                byte[] packet = MessagePack.MessagePackSerializer.Serialize(requestMessage);
                //byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedRequest);
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
                Type = "allUsers",
            };

            try
            {
                // Serializa a mensagem geral e envia para o servidor
                byte[] packet = MessagePack.MessagePackSerializer.Serialize(generalMessage);
                //byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedGeneralMessage);
                serverConnection.SendMessage(packet); // Usa o método da conexão
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao enviar mensagem: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        public void usersInRoom(int roomId)
        {
            // Vai ao serverconnection e vai ver o ID da Room que está selecionado e pede ao servidor a lista de users naquela sala
            usersInRoomFormat idRoom = new usersInRoomFormat { 
                RoomId = ServerConnection.RoomSelected 
            };

            // Cria a mensagem geral, embutindo a requisição no campo Body
            var requestMessage = new GeneralMessage
            {
                Type = "usersInRoom",
                Body = MessagePack.MessagePackSerializer.Serialize(idRoom)
            };

            try
            {
                // Serializa a mensagem geral e envia para o servidor
                byte[] packet = MessagePack.MessagePackSerializer.Serialize(requestMessage);
                //byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedRequest);
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
                Name = name,
                IdUser = ServerConnection.UserSelected,
                NameUser = ServerConnection.UserSelectedName


            };

            // Cria uma mensagem geral com a mensagem específica serializada
            var generalMessage = new GeneralMessage
            {
                Type = "roomCreate",
                Body = MessagePack.MessagePackSerializer.Serialize(roomCreateMessage)
            };

            try
            {
                // Serializa a mensagem geral e envia para o servidor
                byte[] packet = MessagePack.MessagePackSerializer.Serialize(generalMessage);
                //byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedGeneralMessage);
                serverConnection.SendMessage(packet); // Usa o método da conexão
                roomsOfUserRequest(ServerConnection.UserSelected);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao enviar mensagem: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void buttonQuit_Click(object sender, EventArgs e)
        {
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
            roomsOfUserRequest(ServerConnection.UserSelected);
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
                RoomName = ServerConnection.RoomSelectedName,
                UserId = userId,
                UserName= selectedUser.Name
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
                byte[] packet = MessagePack.MessagePackSerializer.Serialize(generalMessage);
                //byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedGeneralMessage);

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
                RoomName= ServerConnection.RoomSelectedName,
                UserName = ServerConnection.UserSelectedName,
                Text = text,
                Date = DateTime.Now

            };

            // Cria uma mensagem geral 
            GeneralMessage generalMessage = new GeneralMessage
            {
                Type = "sendMessage",
                Body = MessagePack.MessagePackSerializer.Serialize(requestData)
            };

            try
            {
                byte[] packet = MessagePack.MessagePackSerializer.Serialize(generalMessage);
                //byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedGeneralMessage);
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
            ServerConnection.RoomSelectedName = selectedRoom.Name;
            messagesInRoom(selectedRoom.Id);
            Thread.Sleep(100);
            usersInRoom(selectedRoom.Id);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                serverConnection.Disconnect();  // Garante que o cliente seja desconectado
                MessageBox.Show("Desconectado com sucesso!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao desconectar durante o fechamento: {ex.Message}");
            }

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            roomsOfUserRequest(ServerConnection.UserSelected);
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

        private void menuListBoxUsersInRoom_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void excluirUtilizadorDaSalaToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            //Aqui vai tentar apagar a sala
            // só deixa se for o admin da sala

            int indexList = listBoxRooms.SelectedIndex;
            if (indexList < 0)
            {
                return;
            }
            RoomListFormat selectedRoom = (RoomListFormat)listBoxRooms.SelectedItem;
            if (selectedRoom.IdAdmin == ServerConnection.UserSelected)
            {
                DialogResult resultado = MessageBox.Show(
                    "Apagar a sala de conversa irá apagar todas as mensagens permanentemente.\nDeseja continuar?",
                    "Confirmação",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (resultado == DialogResult.Yes)
                {
                    // Obtém os IDs relevantes: o ID do usuário a partir do item selecionado e o ID da sala
                    int userId = ServerConnection.UserSelected;
                    int roomId = ServerConnection.RoomSelected;

                    // Cria o objeto de requisição para registrar o usuário na sala, usando a classe usersAddRoomFormat
                    deleteRoom requestData = new deleteRoom
                    {
                        RoomId = roomId,
                        UserId = userId,
                        RoomName = ServerConnection.RoomSelectedName,
                        UserName = ServerConnection.UserSelectedName
                    };

                    // Cria uma mensagem geral com o tipo "usersAddRoom", encapsulando o objeto de requisição no Body
                    GeneralMessage generalMessage = new GeneralMessage
                    {
                        Type = "deleteRoom",
                        Body = MessagePack.MessagePackSerializer.Serialize(requestData)
                    };

                    try
                    {
                        // Serializa a mensagem geral e cria o pacote pelo protocolo customizado
                        byte[] packet = MessagePack.MessagePackSerializer.Serialize(generalMessage);
                        //byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedGeneralMessage);

                        // Envia o pacote para o servidor
                        serverConnection.SendMessage(packet);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao enviar mensagem: {ex.Message}",
                                        "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                MessageBox.Show("Não tem permissões de Administrador na Sala Selecionada.", "Falta de permissões", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAlterarRoom_Click(object sender, EventArgs e)
        {
            int indexList = listBoxRooms.SelectedIndex;
            if (indexList < 0)
            {
                return;
            }

            RoomListFormat selectedRoom = (RoomListFormat)listBoxRooms.SelectedItem;
            if (selectedRoom.IdAdmin == ServerConnection.UserSelected)
            {
                if (string.IsNullOrEmpty(textBoxNewNameRoom.Text))
                {
                    MessageBox.Show("O novo nome da Sala não é válido. ", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                DialogResult resultado = MessageBox.Show(
                    "Deseja alterar o nome da Sala?\n"+selectedRoom.Name+" => "+textBoxNewNameRoom.Text,
                    "Confirmação",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (resultado == DialogResult.Yes)
                {
                    int userId = ServerConnection.UserSelected;
                    int roomId = ServerConnection.RoomSelected;

                    // Cria o objeto de requisição para registrar o usuário na sala, usando a classe usersAddRoomFormat
                    updateRoom requestData = new updateRoom
                    {
                        RoomId = roomId,
                        UserId = userId,
                        NewName = textBoxNewNameRoom.Text,
                        UserName = ServerConnection.UserSelectedName
                    };

                    // Cria uma mensagem geral com o tipo "usersAddRoom", encapsulando o objeto de requisição no Body
                    GeneralMessage generalMessage = new GeneralMessage
                    {
                        Type = "renameRoom",
                        Body = MessagePack.MessagePackSerializer.Serialize(requestData)
                    };

                    try
                    {
                        // Serializa a mensagem geral e cria o pacote pelo protocolo customizado
                        byte[] packet = MessagePack.MessagePackSerializer.Serialize(generalMessage);
                        //byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedGeneralMessage);

                        // Envia o pacote para o servidor
                        serverConnection.SendMessage(packet);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao enviar mensagem: {ex.Message}",
                                        "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                MessageBox.Show("Não tem permissões de Administrador na Sala Selecionada.", "Falta de permissões", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //Aqui vai tentar banir um utilizador da sala
            // só deixa se for o admin da sala

            
            if ((listBoxRooms.SelectedIndex < 0)||(listBoxUserRoom.SelectedIndex < 0))
            {
                return;
            }

            
            RoomListFormat selectedRoom = (RoomListFormat)listBoxRooms.SelectedItem;
            if (selectedRoom.IdAdmin == ServerConnection.UserSelected)
            {
                if (selectedRoom.IdAdmin == usersInRoomList[listBoxUserRoom.SelectedIndex].Id)
                {
                    MessageBox.Show("Não é possivel excluir o Admin da Sala.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DialogResult resultado = MessageBox.Show(
                    "Deseja excluir o utilizador ("+listBoxUserRoom.SelectedItem.ToString()+") permanentemente desta Sala?",
                    "Confirmação",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (resultado == DialogResult.Yes)
                {
                    // Obtém os IDs relevantes: o ID do usuário a partir do item selecionado e o ID da sala
                    int userId = usersInRoomList[listBoxUserRoom.SelectedIndex].Id;
                    int roomId = ServerConnection.RoomSelected;
                    string nameRoom = selectedRoom.Name;
                    // Cria o objeto de requisição para registrar o usuário na sala, usando a classe usersAddRoomFormat
                    UserBanFormat requestData = new UserBanFormat
                    {
                        RoomId = roomId,
                        UserId = userId,
                        NameRoom = selectedRoom.Name,
                        NameUser = usersInRoomList[listBoxUserRoom.SelectedIndex].Name
                    };

                    GeneralMessage generalMessage = new GeneralMessage
                    {
                        Type = "banUser",
                        Body = MessagePack.MessagePackSerializer.Serialize(requestData)
                    };

                    try
                    {
                        // Serializa a mensagem geral e cria o pacote pelo protocolo customizado
                        byte[] packet = MessagePack.MessagePackSerializer.Serialize(generalMessage);
                        //byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedGeneralMessage);

                        // Envia o pacote para o servidor
                        serverConnection.SendMessage(packet);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao enviar mensagem: {ex.Message}",
                                        "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                MessageBox.Show("Não tem permissões de Administrador na Sala Selecionada.", "Falta de permissões", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //Aqui vai tentarsair da sala
            //Se for o admin da sala não deixa


            if (listBoxRooms.SelectedIndex < 0) 
            {
                return;
            }

            RoomListFormat selectedRoom = (RoomListFormat)listBoxRooms.SelectedItem;
            if (selectedRoom.IdAdmin != ServerConnection.UserSelected)
            {
                DialogResult resultado = MessageBox.Show(
                    "Deseja abandonar a Sala?",
                    "Confirmação",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (resultado == DialogResult.Yes)
                {
                    // Obtém os IDs relevantes: o ID do usuário a partir do item selecionado e o ID da sala
                    int userId = ServerConnection.UserSelected;
                    int roomId = ServerConnection.RoomSelected;
                    string nameRoom = selectedRoom.Name;
                    //utilizamos a mesma estrutura pois os campos serão os mesmos
                    UserLeaveRoomFormat requestData = new UserLeaveRoomFormat
                    {
                        RoomId = roomId,
                        UserId = userId,
                        NameRoom = selectedRoom.Name,
                        NameUser = ServerConnection.UserSelectedName
                    };

                    GeneralMessage generalMessage = new GeneralMessage
                    {
                        Type = "userLeaveRoom",
                        Body = MessagePack.MessagePackSerializer.Serialize(requestData)
                    };

                    try
                    {
                        // Serializa a mensagem geral e cria o pacote pelo protocolo customizado
                        byte[] packet = MessagePack.MessagePackSerializer.Serialize(generalMessage);
                        //byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedGeneralMessage);

                        // Envia o pacote para o servidor
                        serverConnection.SendMessage(packet);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao enviar mensagem: {ex.Message}",
                                        "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                MessageBox.Show("Sendo o administrador da sala, não é possivel sair! \nPara abandonar a sala deve apagar a mesma.", "Administrador", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnApagarMensagem_Click(object sender, EventArgs e)
        {
            
        }

        private void Form1_FormClosed_1(object sender, FormClosedEventArgs e)
        {

        }
    }
}
