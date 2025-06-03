using Client;
using EI.SI;
using MessagePack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace Shared
{
    

    public class ServerConnection
    {
        private const int PORT = 10000;
        private const string IP = "127.0.0.1";
        private TcpClient client;
        private NetworkStream networkStream;
        private ProtocolSI protocolSI;
        //private ServerConnection serverConnection; // Reutiliza a conexão

        public static int UserSelected;
        public static string UserSelectedName;
        public static int RoomSelected;
        public static string RoomSelectedName;

        //constantes utilizadas no Salt
        private const int SALTSIZE = 8;
        private const int NUMBER_OF_ITERATIONS = 50000;

        public ServerConnection()
        {
            client = new TcpClient();
        }

        public bool IsConnected
        {
            get { return client != null && client.Connected; }
        }


        //funções para incriptar

        private static byte[] GenerateSalt(int size)
        {
            //Generate a cryptographic random number.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);
            return buff;
        }

        private static byte[] GenerateSaltedHash(string plainText, byte[] salt)
        {
            Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(plainText, salt, NUMBER_OF_ITERATIONS);
            return rfc2898.GetBytes(32);
        }

        

        public void Connect()
        {
            try
            {
                if (client.Connected)
                    return; // Já está ligado, não faz nada

                client.Connect(IP, PORT);
                networkStream = client.GetStream();
                protocolSI = new ProtocolSI();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao conectar ao servidor: {ex.Message}");
            }
        }

        

        public void SendMessage(byte[] message)
        {
            try
            {
                Console.WriteLine("Estas no send message do cliente...");
                byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, message);
                networkStream.Write(packet, 0, packet.Length);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao enviar mensagem: {ex.Message}");
            }
        }

    

        public void ReceiveMessage()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("Estas no Receive Message do cliente...");
                    using (MemoryStream ms = new MemoryStream())
                    {
                        int x = 0;
                        while (true)
                        {
                            // Lê o pacote recebido
                            int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                            Console.WriteLine("Pacotes recebidos (bytes)= " + bytesRead.ToString());
                            //Console.WriteLine("Estas no Receive Message do cliente dentro do WHILE...");
                            if (bytesRead == 0)
                            {
                                throw new Exception("Conexão fechada pelo servidor.");
                            }

                            var cmd = protocolSI.GetCmdType();

                            if (cmd == ProtocolSICmdType.EOF) // Se for o fim da transmissão
                            {
                                //Console.WriteLine("Recebi EOF");
                                // Envia ACK para o servidor
                                //byte[] ack = protocolSI.Make(ProtocolSICmdType.ACK);
                                //networkStream.Write(ack, 0, ack.Length);
                                //Console.WriteLine("Enviei ACK");
                                break;
                            }
                            else if (cmd == ProtocolSICmdType.DATA) // Se for um pacote de dados
                            {
                                // Adiciona os dados ao stream
                                byte[] data = protocolSI.GetData();
                                ms.Write(data, 0, data.Length);

                                //Console.WriteLine("Recebi pacote com "+ data.Length.ToString());

                                // Envia ACK para o servidor, confirmando o recebimento do pacote
                                byte[] ack = protocolSI.Make(ProtocolSICmdType.ACK);
                                networkStream.Write(ack, 0, ack.Length);
                                //Console.WriteLine("Enviei ACK");
                            }
                            else
                            {
                                    Console.WriteLine($"Comando inesperado recebido: {cmd}. Ignorando.");
                                    continue;
                                
                            }
                            x++;
                            
                        }

                        // Executa a função para processar a mensagem recebida
                        executeMessage(ms.ToArray());

                        // Limpa as variáveis para aguardar a próxima mensagem
                        Console.WriteLine("Recebi mensagem com " + ms.Length.ToString());
                        ms.SetLength(0);
                        
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao receber mensagem: {ex.Message}");
            }
        }


        public void executeMessage(byte[] message)
        {
            try
            {
                GeneralMessage responseMessage = MessagePack.MessagePackSerializer.Deserialize<GeneralMessage>(message);
                Console.WriteLine("Estas no Execute Message do cliente e a mensagem é do tipo " + responseMessage.Type);
                Form targetForm = null;

                switch (responseMessage.Type)
                {
                    case "login":
                    case "register":
                        targetForm = Application.OpenForms.OfType<frmLogin>().FirstOrDefault();
                        if (targetForm == null)
                        {
                            targetForm = new Client.frmLogin();
                            targetForm.Show();
                        }
                        break;

                    case "roomCreate":
                    case "updateRooms":
                    case "sendMessage":
                    case "updateMessages":
                    case "errorResponse":
                    case "roomsOfUser":
                    case "messagesInRoom":
                    case "allUsers":
                    case "userAddRoom":
                    case "usersInRoom":
                        targetForm = Application.OpenForms.OfType<Form1>().FirstOrDefault();
                        if (targetForm == null)
                        {
                            targetForm = new Client.Form1(Program.GlobalServerConnection); // ou a instância que estiver usando
                            targetForm.Show();
                        }
                        break;

                    default:
                        MessageBox.Show("Comando desconhecido ou formulário não encontrado...", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                }

                // Executa a ação dentro do formulário correto
                targetForm.Invoke(new Action(() =>
                {
                    switch (responseMessage.Type)
                    {
                        case "login":
                            ((frmLogin)targetForm).login(responseMessage);
                            break;
                        case "register":
                            ((frmLogin)targetForm).register(responseMessage);
                            break;
                        case "sendMessage":
                            ((Form1)targetForm).sendMessage(responseMessage);
                            break;
                        case "updateRooms":
                            ((Form1)targetForm).updateRooms();
                            break;
                        case "roomCreate":
                            ((Form1)targetForm).updateRooms();
                            break;
                        case "updateMessages":
                            ((Form1)targetForm).updateMessages(responseMessage);
                            break;
                        case "errorResponse":
                            ((Form1)targetForm).errorResponse(responseMessage);
                            break;
                        case "roomsOfUser":
                            ((Form1)targetForm).roomsOfUser(responseMessage);
                            break;
                        case "messagesInRoom":
                            ((Form1)targetForm).messagesInRoom(responseMessage);
                            break;
                        case "allUsers":
                            ((Form1)targetForm).allUsers(responseMessage);
                            break;
                        case "userAddRoom":
                            ((Form1)targetForm).userAddRoom(responseMessage);
                            break;
                        case "usersInRoom":
                            ((Form1)targetForm).usersInRoom(responseMessage);
                            break;

                    }
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao processar mensagem: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }





        public void Disconnect()
        {
            try
            {
                if (networkStream != null)
                {
                    networkStream.Close(); // Fecha o stream de rede
                }

                if (client != null && client.Connected)
                {
                    client.Close(); // Fecha a conexão TCP
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao desconectar: {ex.Message}");
            }
        }

    }

    //estruturas gerais 


    [MessagePackObject]
    public struct LoginRequest
    {
        [Key(0)]
        public string Username { get; set; }

        [Key(1)]
        public string Password { get; set; }
    }

    [MessagePackObject]
    public struct LogoutRequest
    {
        [Key(0)]
        public int IdUser { get; set; }
        [Key(1)]
        public string NameUser { get; set; }

    }



    [MessagePackObject]
    public struct RegisterRequest
    {
        [Key(0)]
        public string Username { get; set; }

        [Key(1)]
        public string Password { get; set; }

        [Key(2)]
        public string Name { get; set; }
    }

    [MessagePackObject]
    public struct GeneralMessage
    {
        [Key(0)]
        public string Type { get; set; }

        [Key(1)]
        public byte[] Body { get; set; }
    }

    [MessagePackObject]
    public struct ServerResponse
    {
        [Key(0)]
        public bool Success { get; set; }

        [Key(1)]
        public string Message { get; set; }

        [Key(2)]
        public int IdUser { get; set; }

    }

    [MessagePackObject]
    public struct MessageRoomCreate
    {

        [Key(0)]
        public string Name { get; set; }
        [Key(1)]
        public int IdUser { get; set; }
        [Key(2)]
        public string NameUser { get; set; }
    }

    [MessagePackObject]
    public class UserListFormat
    {
        [Key(0)]
        public int Id { get; set; }

        [Key(1)]
        public string Name { get; set; }

        [Key(2)]
        public bool State { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    [MessagePackObject]
    public class UserRoomListFormat
    {
        [Key(0)]
        public int Id { get; set; }

        [Key(1)]
        public string Name { get; set; }

        [Key(2)]
        public bool State { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
    [MessagePackObject]
    public class usersInRoomFormat
    {
        [Key(0)]
        public int RoomId { get; set; }
        [Key(1)]
        public string RoomName { get; set; }
        [Key(2)]
        public string UserName { get; set; }
        [Key(3)]
        public int UserId { get; set; }
    }

    [MessagePackObject]
    public class usersAddRoomFormat
    {
        [Key(0)]
        public int RoomId { get; set; }
        [Key(1)]
        public int UserId { get; set; }
        [Key(2)]
        public string RoomName { get; set; }
        [Key(3)]
        public string UserName { get; set; }
    }

    [MessagePackObject]
    public class deleteRoom
    {
        [Key(0)]
        public int RoomId { get; set; }
        [Key(1)]
        public int UserId { get; set; }
        [Key(2)]
        public string RoomName { get; set; }
        [Key(3)]
        public string UserName { get; set; }
    }



    [MessagePackObject]
    public class updateRoom
    {
        [Key(0)]
        public int RoomId { get; set; }
        [Key(1)]
        public int UserId { get; set; }

        [Key(2)]
        public string NewName { get; set; }
        [Key(3)]
        public string UserName { get; set; }
        [Key(4)]
        public string OldName { get; set; }

    }


    [MessagePackObject]
    public class messageFormat
    {
        [Key(0)]
        public int RoomId { get; set; }
        [Key(1)]
        public int UserId { get; set; }
        [Key(2)]
        public string Text { get; set; }
        [Key(3)]
        public DateTime Date { get; set; }
        [Key(4)]
        public string UserName { get; set; }
        [Key(5)]
        public string RoomName { get; set; }
    }

    [MessagePackObject]
    public class messagesInRoomFormat
    {
        [Key(0)]
        public int RoomId { get; set; }
    }

    [MessagePackObject]
    public class roomsOfUserFormat
    {
        [Key(0)]
        public int UserId { get; set; }
    }

    [MessagePackObject]
    public class RoomListFormat
    {
        [Key(0)]
        public int Id { get; set; }

        [Key(1)]
        public string Name { get; set; }

        [Key(2)]
        public int IdAdmin { get; set; }

        [Key(3)]
        public string NameAdmin { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    [MessagePackObject]
    public class UserBanFormat
    {
        [Key(0)]
        public int UserId { get; set; }

        [Key(1)]
        public string NameRoom { get; set; }

        [Key(2)]
        public int RoomId { get; set; }

        [Key(3)]
        public string NameUser { get; set; }
    }


    [MessagePackObject]
    public class UserLeaveRoomFormat
    {
        [Key(0)]
        public int UserId { get; set; }

        [Key(1)]
        public string NameRoom { get; set; }

        [Key(2)]
        public int RoomId { get; set; }

        [Key(3)]
        public string NameUser { get; set; }

    }

}
