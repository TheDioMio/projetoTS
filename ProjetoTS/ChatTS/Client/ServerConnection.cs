using EI.SI;
using MessagePack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

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
        public static int RoomSelected;

        public ServerConnection()
        {
            client = new TcpClient();
        }

        public bool IsConnected
        {
            get { return client != null && client.Connected; }
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

        //public byte[] ReceiveMessage()
        //{
        //    try
        //    {
        //        networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
        //        return protocolSI.GetData();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Erro ao receber mensagem: {ex.Message}");
        //    }
        //}

        //public byte[] ReceiveMessage()
        //{
        //    try
        //    {
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            while (true)
        //            {
        //                // Lê o pacote recebido
        //                int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
        //                if (bytesRead == 0)
        //                {
        //                    throw new Exception("Conexão fechada pelo servidor.");
        //                }

        //                var cmd = protocolSI.GetCmdType();

        //                if (cmd == ProtocolSICmdType.EOT) // Se for o fim da transmissão
        //                {
        //                    // Envia ACK para o servidor
        //                    byte[] ack = protocolSI.Make(ProtocolSICmdType.ACK);
        //                    networkStream.Write(ack, 0, ack.Length);
        //                    break;
        //                }
        //                else if (cmd == ProtocolSICmdType.DATA) // Se for um pacote de dados
        //                {
        //                    // Adiciona os dados ao stream
        //                    byte[] data = protocolSI.GetData();
        //                    ms.Write(data, 0, data.Length);

        //                    // Envia ACK para o servidor, confirmando o recebimento do pacote
        //                    byte[] ack = protocolSI.Make(ProtocolSICmdType.ACK);
        //                    networkStream.Write(ack, 0, ack.Length);
        //                }
        //                else
        //                {
        //                    throw new Exception($"Comando inesperado recebido: {cmd}");
        //                }
        //            }

        //            return ms.ToArray(); // Retorna os dados recebidos
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Erro ao receber mensagem: {ex.Message}");
        //    }
        //}

        public byte[] ReceiveMessage()
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    while (true)
                    {
                        // Lê o pacote recebido
                        int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                        if (bytesRead == 0)
                        {
                            throw new Exception("Conexão fechada pelo servidor.");
                        }

                        var cmd = protocolSI.GetCmdType();

                        if (cmd == ProtocolSICmdType.EOT) // Se for o fim da transmissão
                        {
                            // Envia ACK para o servidor
                            byte[] ack = protocolSI.Make(ProtocolSICmdType.ACK);
                            networkStream.Write(ack, 0, ack.Length);
                            break;
                        }
                        else if (cmd == ProtocolSICmdType.DATA) // Se for um pacote de dados
                        {
                            // Adiciona os dados ao stream
                            byte[] data = protocolSI.GetData();
                            ms.Write(data, 0, data.Length);

                            // Envia ACK para o servidor, confirmando o recebimento do pacote
                            byte[] ack = protocolSI.Make(ProtocolSICmdType.ACK);
                            networkStream.Write(ack, 0, ack.Length);
                        }
                        else
                        {
                            throw new Exception($"Comando inesperado recebido: {cmd}");
                        }
                    }

                    return ms.ToArray(); // Retorna os dados recebidos
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao receber mensagem: {ex.Message}");
            }
        }



        //public byte[] ReceiveMessage()
        //{
        //    try
        //    {
        //        MemoryStream ms = new MemoryStream();

        //        while (true)
        //        {
        //            // Lê os dados que chegaram
        //            networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);

        //            // Se for EOT   , acabou
        //            if (protocolSI.GetCmdType() == ProtocolSICmdType.EOT)
        //            {
        //                break;
        //            }
        //            // Se for DATA, adiciona os dados
        //            else if (protocolSI.GetCmdType() == ProtocolSICmdType.DATA)
        //            {
        //                byte[] data = protocolSI.GetData();
        //                ms.Write(data, 0, data.Length);
        //            }
        //            else
        //            {
        //                throw new Exception($"Comando inesperado recebido: {protocolSI.GetCmdType()}");
        //            }
        //        }

        //        return ms.ToArray(); // Retorna os dados recebidos
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Erro ao receber mensagem: {ex.Message}");
        //    }
        //}


        //public byte[] ReceiveMessage()
        //{
        //    try
        //    {
        //        MemoryStream ms = new MemoryStream();
        //        int bytesRead;

        //        while (true)
        //        {
        //            bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);

        //            if (bytesRead == 0)
        //            {
        //                // No more data to read, possibly end of stream
        //                break;
        //            }

        //            // Se for EOT, fim de transmissão
        //            if (protocolSI.GetCmdType() == ProtocolSICmdType.EOT)
        //            {
        //                break;
        //            }
        //            // Se for DATA, adiciona os dados ao buffer
        //            else if (protocolSI.GetCmdType() == ProtocolSICmdType.DATA)
        //            {
        //                byte[] data = protocolSI.GetData();
        //                ms.Write(data, 0, data.Length);
        //            }
        //            else
        //            {
        //                throw new Exception($"Comando inesperado recebido: {protocolSI.GetCmdType()}");
        //            }
        //        }

        //        return ms.ToArray();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Erro ao receber mensagem: {ex.Message}");
        //    }
        //}

        //public byte[] ReceiveMessage()
        //{
        //    try
        //    {
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            while (true)
        //            {
        //                int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
        //                if (bytesRead == 0)
        //                {
        //                    throw new Exception("Conexão fechada pelo servidor.");
        //                }

        //                var cmd = protocolSI.GetCmdType();

        //                if (cmd == ProtocolSICmdType.EOT)
        //                {
        //                    break;
        //                }
        //                else if (cmd == ProtocolSICmdType.DATA)
        //                {
        //                    byte[] data = protocolSI.GetData();
        //                    ms.Write(data, 0, data.Length);
        //                }
        //                else
        //                {
        //                    throw new Exception($"Comando inesperado recebido: {cmd}");
        //                }
        //            }

        //            return ms.ToArray();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Erro ao receber mensagem: {ex.Message}");
        //    }
        //}







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
    public struct UpdateRequest
    {
        [Key(0)]
        public int IdRoom { get; set; }

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
    }


    [MessagePackObject]
    public class usersAddRoomFormat
    {
        [Key(0)]
        public int RoomId { get; set; }
        [Key(1)]
        public int UserId { get; set; }
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

        public override string ToString()
        {
            return Text;
        }
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

        public override string ToString()
        {
            return Name;
        }
    }

}
