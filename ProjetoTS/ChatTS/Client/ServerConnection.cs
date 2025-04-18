using EI.SI;
using MessagePack;
using System;
using System.Net.Sockets;

namespace Shared
{
    

    public class ServerConnection
    {
        private const int PORT = 10000;
        private const string IP = "127.0.0.1";
        private TcpClient client;
        private NetworkStream networkStream;
        private ProtocolSI protocolSI;

        public static int UserSelected;

        public ServerConnection()
        {
            client = new TcpClient();
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
                networkStream.Write(message, 0, message.Length);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao enviar mensagem: {ex.Message}");
            }
        }

        public byte[] ReceiveMessage()
        {
            try
            {
                networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                return protocolSI.GetData();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao receber mensagem: {ex.Message}");
            }
        }

        public void Disconnect()
        {
            networkStream.Close();
            client.Close();
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
        public string Action { get; set; }
        //é a acção que se pretende fazer, e vai chamar uma função com o mesmo nome 
        [Key(1)]
        public string Name { get; set; }
        [Key(2)]
        public int IdUser { get; set; }
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
    }

}
