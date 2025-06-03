using EI.SI;
using MessagePack;
using Server.Models;
using System;
using System.Data.Entity.Core.Mapping;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Shared;
using Server.models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Collections.Concurrent;
using Server;
using System.Data.Entity.Core.Metadata.Edm;
using System.Runtime.Remoting.Contexts;

// a estrutura base do programa é a base da ficha 3 das aulas

// de momento tem algumas mensagens a trabalhar para efeitos de debug
namespace Server
{
    class Program
    {
        private const int PORT = 10000;
        public static ConcurrentDictionary<int, UserConnection> ConnectedUsers = new ConcurrentDictionary<int, UserConnection>();

        private Dictionary<int, TcpClient> users = new Dictionary<int, TcpClient>();
        static void Main(string[] args)
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, PORT);
            TcpListener listener = new TcpListener(endpoint);

            listener.Start();
            Console.WriteLine("SERVER READY");
            int clientCounter = 0;

            //criar uma lista de clientes
            while (true)
            {
                Console.WriteLine("Aguardando cliente...");
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Cliente conectado.");
               
                clientCounter++;
                //Console.WriteLine("Client {0} connected", clientCounter);

                ClientHandler clientHandler = new ClientHandler(client, clientCounter);
                clientHandler.Handle();
            }
        }
    }

 


    public class UserConnection
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public TcpClient Client { get; set; }
        public NetworkStream Stream => Client.GetStream();
    }

    class ClientHandler
    {
        private TcpClient client;
        private int clientID;
        
        

        public ClientHandler(TcpClient client, int clientID)
        {
            this.client = client;
            this.clientID = clientID;
        }

        public void Handle()
        {
            Thread thread = new Thread(ThreadHandler);
            thread.Start();
        }

        private void ThreadHandler()
        {
            NetworkStream networkStream = this.client.GetStream();
            ProtocolSI protocolSI = new ProtocolSI();

            while (true)
            {
                //Console.WriteLine("Esta a postos para ler a mensagem...");
                int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);

                if (bytesRead == 0)
                {
                    string logMessage = $"Cliente desconectador.";
                    LogToFile(logMessage);
                    Console.WriteLine(logMessage);

                    break;
                }

                switch (protocolSI.GetCmdType())
                {
                    case ProtocolSICmdType.DATA:
                        byte[] receivedData = protocolSI.GetData();
                        try
                        {
                            // Desserializar a mensagem geral
                            var generalMessage = MessagePack.MessagePackSerializer.Deserialize<GeneralMessage>(receivedData);
                            string logMessage;

                            //Console.WriteLine("Recebeu a mensagem do tipo: " + generalMessage.Type);
                            switch (generalMessage.Type)
                            {
                                //analisa o Type que vem na mensagem 
                                //para criar novas funções é só enviar a mensagem com o type que se deseja e criar o case e a função


                                case "roomCreate":
                                    // Desserializar para MessageRoomCreate o BODY da mensagem
                                    var messageRoomCreate = MessagePack.MessagePackSerializer.Deserialize<MessageRoomCreate>(generalMessage.Body);
                                    var responseRoomCreate = new ServerResponse { };
                                    
                                    //tratamento para ver se o campo nome vem correto
                                    // podemos e devemos implementar a ver se a sala já existe na base de dados
                                    if (string.IsNullOrEmpty(messageRoomCreate.Name))
                                    {
                                        responseRoomCreate.Success = false;
                                        responseRoomCreate.Message = "Erro: Nome da sala é inválido.";
                                         logMessage = $"Utilizador {messageRoomCreate.NameUser} criou a sala {messageRoomCreate.Name}.";
                                        LogToFile(logMessage);
                                        Console.WriteLine(logMessage);
                                    }
                                    else
                                    {
                                        // chama afunção CreateRoom para criar a Room que recebeu do cliente
                                        CreateRoom(messageRoomCreate);

                                        //formata a mensagem de retorno para o cliente
                                        responseRoomCreate.Success = true;
                                        responseRoomCreate.Message = $"Sala '{messageRoomCreate.Name}' criada com sucesso.";
                                        logMessage = $"Utilizador {messageRoomCreate.NameUser} criou a sala {messageRoomCreate.Name}.";
                                        LogToFile(logMessage);
                                        Console.WriteLine(logMessage);
                                    }
                                    SendMessageToClient(networkStream, protocolSI, responseRoomCreate, "roomCreate");
                                    break;


                                case "usersAddRoom":
                                    // Desserializar para MessageRoomCreate o BODY da mensagem
                                   
                                    var messageUsersAddRoom = MessagePack.MessagePackSerializer.Deserialize<usersAddRoomFormat>(generalMessage.Body);
                                    //Console.WriteLine($"UserId recebido no servidor: {messageUsersAddRoom.UserId}");
                                    var responseUserAddRoom = new ServerResponse { };

                                    if ((messageUsersAddRoom.UserId < 0) || (messageUsersAddRoom.RoomId < 0))
                                    {
                                        responseUserAddRoom.Success = false;
                                        responseUserAddRoom.Message = "Erro: Não foi possivel adicionar o utilizador";
                                        logMessage = $"Não foi possivel adicionar o utilizador {messageUsersAddRoom.UserName} a sala {messageUsersAddRoom.RoomName}.";
                                        LogToFile(logMessage);
                                        Console.WriteLine(logMessage);
                                    }
                                    else
                                    {
                                        // chama afunção CreateRoom para criar a Room que recebeu do cliente
                                        usersAddRoom(messageUsersAddRoom.RoomId, messageUsersAddRoom.UserId);
                                        responseUserAddRoom.Success = true;
                                        responseUserAddRoom.Message = $"O utilizador '{messageUsersAddRoom.UserId}' foi relacionado com a sala'{messageUsersAddRoom.RoomId}' com sucesso.";
                                        logMessage = $"Utilizador {messageUsersAddRoom.UserName} foi adicionado a sala {messageUsersAddRoom.RoomName}.";
                                        LogToFile(logMessage);
                                        Console.WriteLine(logMessage);
                                    }
                                    SendMessageToClient(networkStream, protocolSI, responseUserAddRoom, "userAddRoom");
                                    break;


                                case "logout":
                                    //vamos ter de apagar o user da lista 

                                    var messageLogout = MessagePack.MessagePackSerializer.Deserialize<LogoutRequest>(generalMessage.Body);

                                    if (messageLogout.IdUser >= 0)
                                    {
                                        Program.ConnectedUsers.TryRemove(messageLogout.IdUser, out _);
                                        logMessage = $"Utilizador {messageLogout.NameUser} fez logout.";
                                        LogToFile(logMessage);
                                        Console.WriteLine(logMessage);
                                    }
                                    break;



                                case "login":
                                    
                                    var messageLogin = MessagePack.MessagePackSerializer.Deserialize<LoginRequest>(generalMessage.Body);

                                    if (string.IsNullOrEmpty(messageLogin.Username) || string.IsNullOrEmpty(messageLogin.Password))
                                    {
                                        var errorResponse = new ServerResponse
                                        {
                                            Success = false,
                                            Message = "Erro: Credênciais inválidas."
                                        };
                                        SendMessageToClient(networkStream, protocolSI, errorResponse, "errorResponse");

                                        logMessage = $"Utilizador {messageLogin.Username} tentou fazer login com credenciais inválidas.";
                                    }
                                    else
                                    {
                                        using (var dbContext = new ChatContext())
                                        {

                                            var user = dbContext.Users.FirstOrDefault(u => u.Username == messageLogin.Username && u.Password == messageLogin.Password);

                                            if (user == null)
                                            {
                                                // Usuário não encontrado ou credenciais incorretas
                                                var errorResponse = new ServerResponse
                                                {
                                                    Success = false,
                                                    Message = "Erro: Credenciais inválidas."
                                                };
                                                SendMessageToClient(networkStream, protocolSI, errorResponse, "errorResponse");

                                                logMessage = $"Utilizador {messageLogin.Username} tentou fazer login mas não foi encontrado na BD ou tem credenciais inválidas.";
                                            }
                                            else
                                            {
                                                // User encontrado - login bem-sucedido
                                                var successResponse = new ServerResponse
                                                {
                                                    Success = true,
                                                    Message = user.Name,
                                                    IdUser = user.Id
                                                };

                                                Program.ConnectedUsers.TryAdd(user.Id, new UserConnection
                                                {
                                                    UserId = user.Id,
                                                    UserName = user.Name,
                                                    Client = this.client
                                                });
                                                SendMessageToClient(networkStream, protocolSI, successResponse, "login");
                                                logMessage = $"Utilizador {user.Name} fez login.";
                                            }
                                        }
                                    }
                                    LogToFile(logMessage);
                                    Console.WriteLine(logMessage);
                                    break;

                                case "allUsers":
                                    
                                    // Obtém a lista de usuários do banco de dados.
                                    using (var dbContext = new ChatContext())
                                    {
                                        List<UserListFormat> usersList = dbContext.Users
                                             .Select(u => new UserListFormat
                                             {
                                                 Id = u.Id,
                                                 Name = u.Name,
                                                 State = u.State
                                             })
                                             .ToList();
                                        SendMessageToClient(networkStream, protocolSI, usersList, "allUsers");
                                    }
                                    break;

                                    



                                case "userLeaveRoom":
                                    logMessage = "";
                                    try
                                    {
                                        // Desserializa para obter os dados 
                                        var userLeaveRoomRequest = MessagePack.MessagePackSerializer.Deserialize<UserLeaveRoomFormat>(generalMessage.Body);

                                        if (userLeaveRoomRequest.RoomId < 0)
                                        {
                                            // Se o ID for inválido, envia um erro de resposta
                                            var errorResponse = new ServerResponse
                                            {
                                                Success = false,
                                                Message = "Erro: ID da sala inválido."
                                            };
                                            SendMessageToClient(networkStream, protocolSI, errorResponse, "errorResponse");
                                            logMessage = $"Utilizador {userLeaveRoomRequest.NameUser} tentou abandonar a sala {userLeaveRoomRequest.NameRoom} sem secesso.";
                                        }
                                        else if (userLeaveRoomRequest.UserId < 0)
                                        {
                                            // Se o ID for inválido, envia um erro de resposta
                                            var errorResponse = new ServerResponse
                                            {
                                                Success = false,
                                                Message = "Erro: ID do Utilizador inválido."
                                            };
                                            SendMessageToClient(networkStream, protocolSI, errorResponse, "errorResponse");
                                            logMessage = $"Utilizador {userLeaveRoomRequest.NameUser} tentou abandonar a sala {userLeaveRoomRequest.NameRoom} sem secesso.";
                                        }
                                        else
                                        {
                                            using (var dbContext = new ChatContext())
                                            {
                                                var userRoom = dbContext.UserRooms.FirstOrDefault(ur => ur.IdUser == userLeaveRoomRequest.UserId &&
                                                                                                        ur.IdRoom == userLeaveRoomRequest.RoomId);
                                                if (userRoom != null)
                                                {
                                                    // Guarda todos os utilizadores daquela sala
                                                    var usersInRoom = dbContext.UserRooms
                                                                               .Where(ur => ur.IdRoom == userLeaveRoomRequest.RoomId)
                                                                               .Select(ur => ur.IdUser)
                                                                               .ToList();

                                                    // remove a relaçao do utilizador com a sala 
                                                    dbContext.UserRooms.Remove(userRoom);
                                                    dbContext.SaveChanges();
                                                    
                                                    //Envia mensagem para todos os users que pertenciam a sala para atualizarem a lista de salas
                                                    foreach (var IdUser in usersInRoom)
                                                    {
                                                        if (Program.ConnectedUsers.ContainsKey(IdUser))
                                                        {
                                                            var connectedUser = Program.ConnectedUsers[IdUser];
                                                            //neste body vamos colocar mensagens para os utilizadores saberem o que se passa
                                                            var body = "";

                                                            var updateRooms = new GeneralMessage
                                                            {
                                                                Type = "updateRooms",
                                                                Body = MessagePack.MessagePackSerializer.Serialize(body)
                                                            };

                                                            byte[] serializedUpdate = MessagePack.MessagePackSerializer.Serialize(updateRooms);
                                                            byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedUpdate);
                                                            byte[] eofPacket = protocolSI.Make(ProtocolSICmdType.EOF);

                                                            try
                                                            {
                                                                connectedUser.Stream.Write(packet, 0, packet.Length);
                                                                connectedUser.Stream.Write(eofPacket, 0, eofPacket.Length);
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                
                                                                logMessage = $"Erro ao enviar update para o usuário {IdUser}: {ex.Message}";
                                                                LogToFile(logMessage);
                                                                Console.WriteLine(logMessage);
                                                            }

                                                        }
                                                    }
                                                    logMessage = $"Utilizador {userLeaveRoomRequest.NameUser} abandonou a sala {userLeaveRoomRequest.NameRoom}.";
                                                }
                                                else
                                                {
                                                    var errorResponse = new ServerResponse
                                                    {
                                                        Success = false,
                                                        Message = "Erro: Sala ou utilizador não encontrado."
                                                    };
                                                    SendMessageToClient(networkStream, protocolSI, errorResponse, "errorResponse");
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    { 
                                        var errorResponse = new ServerResponse
                                        {
                                            Success = false,
                                            Message = "Erro no processamento da exclusão."
                                        };
                                        SendMessageToClient(networkStream, protocolSI, errorResponse, "errorResponse");
                                        logMessage = $"Erro! Não foi possivel ao user abandonar a sala: {ex.Message}";
                                    }
                                    LogToFile(logMessage);
                                    Console.WriteLine(logMessage);
                                    break;


                                case "banUser":
                                    logMessage = "";
                                    try
                                    {
                                        // Desserializa para obter o ID da sala que será apagada
                                        var banUserRequest = MessagePack.MessagePackSerializer.Deserialize<UserBanFormat>(generalMessage.Body);

                                        if (banUserRequest.RoomId < 0)
                                        {
                                            // Se o ID for inválido, envia um erro de resposta
                                            var errorResponse = new ServerResponse
                                            {
                                                Success = false,
                                                Message = "Erro: ID da sala inválido."
                                            };
                                            SendMessageToClient(networkStream, protocolSI, errorResponse, "errorResponse");
                                            logMessage = $"Erro! Não foi possivel excluir o user {banUserRequest.NameUser} da sala {banUserRequest.NameRoom}: ID da sala inválido.";
                                        }
                                        else if (banUserRequest.UserId < 0)
                                        {
                                            // Se o ID for inválido, envia um erro de resposta
                                            var errorResponse = new ServerResponse
                                            {
                                                Success = false,
                                                Message = "Erro: ID de Utilizador inválido."
                                            };
                                            SendMessageToClient(networkStream, protocolSI, errorResponse, "errorResponse");
                                            logMessage = $"Erro! Não foi possivel excluir o user {banUserRequest.NameUser} da sala {banUserRequest.NameRoom}: ID de Utilizador inválido.";
                                        }
                                        else
                                        {
                                            using (var dbContext = new ChatContext())
                                            {


                                                var userRoom = dbContext.UserRooms.FirstOrDefault(ur => ur.IdUser == banUserRequest.UserId &&
                                                                                                        ur.IdRoom == banUserRequest.RoomId);

                                                if (userRoom != null)
                                                {
                                                                                                  
                                                    // Guarda todos os utilizadores daquela sala
                                                    var usersInRoom = dbContext.UserRooms
                                                                               .Where(ur => ur.IdRoom == banUserRequest.RoomId)
                                                                               .Select(ur => ur.IdUser)
                                                                               .ToList();

                                                    // Elimina a sala da base de dados
                                                    dbContext.UserRooms.Remove(userRoom);
                                                    dbContext.SaveChanges();
                                                    //Envia mensagem para todos os users que pertenciam a sala para atualizarem a lista de salas
                                                    foreach (var IdUser in usersInRoom)
                                                    {
                                                        if (Program.ConnectedUsers.ContainsKey(IdUser))
                                                        {
                                                            var connectedUser = Program.ConnectedUsers[IdUser];

                                                            var body = "";

                                                            var updateRooms = new GeneralMessage
                                                            {
                                                                Type = "updateRooms",
                                                                Body = MessagePack.MessagePackSerializer.Serialize(body)
                                                            };

                                                            byte[] serializedUpdate = MessagePack.MessagePackSerializer.Serialize(updateRooms);
                                                            byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedUpdate);
                                                            byte[] eofPacket = protocolSI.Make(ProtocolSICmdType.EOF);

                                                            try
                                                            {
                                                                connectedUser.Stream.Write(packet, 0, packet.Length);
                                                                connectedUser.Stream.Write(eofPacket, 0, eofPacket.Length);
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                logMessage = $"Erro ao enviar update para o usuário {IdUser}: {ex.Message}";
                                                                LogToFile(logMessage);
                                                                Console.WriteLine(logMessage);
                                                            }
                                                        }
                                                    }
                                                    logMessage = $"Utilizador {banUserRequest.NameUser} foi excluido da sala {banUserRequest.NameRoom} pelo administrador.";
                                                }
                                                else
                                                {
                                                    var errorResponse = new ServerResponse
                                                    {
                                                        Success = false,
                                                        Message = "Erro: Sala ou utilizador não encontrado."
                                                    };
                                                    SendMessageToClient(networkStream, protocolSI, errorResponse, "errorResponse");
                                                    logMessage = $"Erro: Não foi possivel excluir o user { banUserRequest.NameUser} da sala {banUserRequest.NameRoom} (Sala ou utilizador não encontrado).";
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {   
                                        var errorResponse = new ServerResponse
                                        {
                                            Success = false,
                                            Message = "Erro no processamento da exclusão."
                                        };
                                        SendMessageToClient(networkStream, protocolSI, errorResponse, "errorResponse");
                                        logMessage = $"Erro! Não foi possivel excluir o user da sala: {ex.Message}";
                                    }
                                    LogToFile(logMessage);
                                    Console.WriteLine(logMessage);
                                    break;



                                case "renameRoom":
                                    logMessage = "";
                                    try
                                    {
                                        // Desserializa para obter os dados da sala 
                                        var roomUpdateRequest = MessagePack.MessagePackSerializer.Deserialize<updateRoom>(generalMessage.Body);

                                        if (roomUpdateRequest.RoomId < 0)
                                        {
                                            // Se o ID for inválido, envia um erro de resposta
                                            var errorResponse = new ServerResponse
                                            {
                                                Success = false,
                                                Message = "Erro: ID da sala inválido."
                                            };
                                            SendMessageToClient(networkStream, protocolSI, errorResponse, "errorResponse");
                                            logMessage = $"Erro! Não foi possivel renomear a sala: Id da sala inválido";
                                        }
                                        else
                                        {
                                            using (var dbContext = new ChatContext())
                                            {
                                                // Buscar a sala para update
                                                var room = dbContext.Rooms.Find(roomUpdateRequest.RoomId);
                                                string oldName = room.Name;
                                                if (room != null)
                                                {
                                                    // Guarda todos os utilizadores daquela sala
                                                    var usersInRoom = dbContext.UserRooms
                                                                               .Where(ur => ur.IdRoom == roomUpdateRequest.RoomId)
                                                                               .Select(ur => ur.IdUser)
                                                                               .ToList();

                                                    // Elimina a sala da base de dados
                                                    room.Name = roomUpdateRequest.NewName;
                                                    dbContext.SaveChanges();
                                                    //Envia mensagem para todos os users que pertencem a sala para atualizarem a lista de salas
                                                    foreach (var IdUser in usersInRoom)
                                                    {
                                                        if (Program.ConnectedUsers.ContainsKey(IdUser))
                                                        {
                                                            var connectedUser = Program.ConnectedUsers[IdUser];

                                                            var body = "";

                                                            var updateRooms = new GeneralMessage
                                                            {
                                                                Type = "updateRooms",
                                                                Body = MessagePack.MessagePackSerializer.Serialize(body)
                                                            };

                                                            byte[] serializedUpdate = MessagePack.MessagePackSerializer.Serialize(updateRooms);
                                                            byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedUpdate);
                                                            byte[] eofPacket = protocolSI.Make(ProtocolSICmdType.EOF);

                                                            try
                                                            {
                                                                connectedUser.Stream.Write(packet, 0, packet.Length);
                                                                connectedUser.Stream.Write(eofPacket, 0, eofPacket.Length);
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                logMessage = $"Erro ao enviar update para o usuário {IdUser}: {ex.Message}";
                                                                LogToFile(logMessage);
                                                                Console.WriteLine(logMessage);
                                                            }
                                                        }
                                                    }
                                                    logMessage = $"A sala {oldName} foi renomeada para {roomUpdateRequest.NewName}.";
                                                }
                                                else
                                                {
                                                    var errorResponse = new ServerResponse
                                                    {
                                                        Success = false,
                                                        Message = "Erro: Sala não encontrada."
                                                    };
                                                    SendMessageToClient(networkStream, protocolSI, errorResponse, "errorResponse");
                                                    logMessage = $"Erro! Não foi possivel renomear a sala: sala não encontrada";
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        var errorResponse = new ServerResponse
                                        {
                                            Success = false,
                                            Message = "Erro no processamento da exclusão."
                                        };
                                        SendMessageToClient(networkStream, protocolSI, errorResponse, "errorResponse");
                                        logMessage = $"Erro! Não foi possivel renomear a sala: {ex.Message}";
                                    }
                                    LogToFile(logMessage);
                                    Console.WriteLine(logMessage);
                                    break;


                                case "deleteRoom":
                                    logMessage = "";
                                    try
                                    {
                                        // Desserializa para obter o ID da sala que será apagada
                                        var roomDeleteRequest = MessagePack.MessagePackSerializer.Deserialize<deleteRoom>(generalMessage.Body);
                                        //Console.WriteLine($"Solicitação para excluir a sala ID: {roomDeleteRequest.RoomId}");

                                        if (roomDeleteRequest.RoomId < 0)
                                        {
                                            // Se o ID for inválido, envia um erro de resposta
                                            var errorResponse = new ServerResponse
                                            {
                                                Success = false,
                                                Message = "Erro: ID da sala inválido."
                                            };
                                            SendMessageToClient(networkStream, protocolSI, errorResponse, "errorResponse");
                                            logMessage = $"Erro! Não foi possivel apagar a sala: Id da sala inválido";
                                        }
                                        else
                                        {
                                            using (var dbContext = new ChatContext())
                                            {
                                                // Buscar a sala para exclusão
                                                var room = dbContext.Rooms.Find(roomDeleteRequest.RoomId);

                                                if (room != null)
                                                {
                                                    // Guarda todos os utilizadores daquela sala
                                                    var usersInRoom = dbContext.UserRooms
                                                                               .Where(ur => ur.IdRoom == roomDeleteRequest.RoomId)
                                                                               .Select(ur => ur.IdUser)
                                                                               .ToList();

                                                    // Elimina a sala da base de dados
                                                    dbContext.Rooms.Remove(room);
                                                    dbContext.SaveChanges();
                                                    //Envia mensagem para todos os users que pertenciam a sala para atualizarem a lista de salas
                                                    foreach (var IdUser in usersInRoom)
                                                    {
                                                        if (Program.ConnectedUsers.ContainsKey(IdUser))
                                                        {
                                                            var connectedUser = Program.ConnectedUsers[IdUser];

                                                            var body = "";

                                                            var updateRooms = new GeneralMessage
                                                            {
                                                                Type = "updateRooms",
                                                                Body = MessagePack.MessagePackSerializer.Serialize(body)
                                                            };

                                                            byte[] serializedUpdate = MessagePack.MessagePackSerializer.Serialize(updateRooms);
                                                            byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedUpdate);
                                                            byte[] eofPacket = protocolSI.Make(ProtocolSICmdType.EOF);

                                                            try
                                                            {
                                                                connectedUser.Stream.Write(packet, 0, packet.Length);
                                                                connectedUser.Stream.Write(eofPacket, 0, eofPacket.Length);
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                logMessage = $"Erro ao enviar update para o usuário {IdUser}: {ex.Message}";
                                                                LogToFile(logMessage);
                                                                Console.WriteLine(logMessage);
                                                            }
                                                        }
                                                    }
                                                    logMessage= $"Sala {roomDeleteRequest.RoomName} foi apagada pelo administrador!";
                                                }
                                                else
                                                {
                                                    var errorResponse = new ServerResponse
                                                    {
                                                        Success = false,
                                                        Message = "Erro: Sala não encontrada."
                                                    };
                                                    SendMessageToClient(networkStream, protocolSI, errorResponse, "errorResponse");
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        var errorResponse = new ServerResponse
                                        {
                                            Success = false,
                                            Message = "Erro no processamento da exclusão."
                                        };
                                        SendMessageToClient(networkStream, protocolSI, errorResponse, "errorResponse");
                                        logMessage = $"Erro! Não foi possivel apagar a sala: {ex.Message}";
                                    }
                                    LogToFile(logMessage);
                                    Console.WriteLine(logMessage);
                                    break;


                                case "sendMessage":
                                    logMessage = "";
                                    try
                                    {
                                        // Desserializa para messageFormat o BODY da mensagem
                                        var messageAdd = MessagePack.MessagePackSerializer.Deserialize<messageFormat>(generalMessage.Body);
                                        //Console.WriteLine($"Mensagem recebida: {messageAdd.Text}");

                                        if ((messageAdd.UserId < 0) || (messageAdd.RoomId < 0) || (string.IsNullOrEmpty(messageAdd.Text)))
                                        {
                                            // Se os parâmetros forem inválidos, envia um erro de resposta
                                            var errorResponse = new ServerResponse
                                            {
                                                Success = false,
                                                Message = "Erro: Parametros inválidos."
                                            };
                                            SendMessageToClient(networkStream, protocolSI, errorResponse, "errorResponse");
                                            logMessage = $"Erro! Mensagem não processada: Parametros inválidos";
                                        }
                                        else
                                        {
                                            using (var dbContext = new ChatContext())
                                            {
                                                // Registra a mensagem no banco de dados
                                                Message Message = new Message(messageAdd.UserId, messageAdd.RoomId, messageAdd.Text);
                                                dbContext.Messages.Add(Message);
                                                dbContext.SaveChanges();
                                            }

                                            // Formata a resposta de sucesso para o cliente
                                            var successResponse = new ServerResponse
                                            {
                                                Success = true,
                                                Message = $"Mensagem enviada com sucesso."
                                            };
                                            SendMessageToClient(networkStream, protocolSI, successResponse, "sendMessage");

                                            

                                            // Buscar da base de dados os utilizadores daquela sala
                                            using (var dbContext = new ChatContext())
                                            {
                                                var usersInRoom = dbContext.UserRooms
                                                                            .Where(ur => ur.IdRoom == messageAdd.RoomId)
                                                                            .Select(ur => ur.IdUser)
                                                                            .ToList();

                                                foreach (var IdUser in usersInRoom)
                                                {
                                                    if (Program.ConnectedUsers.ContainsKey(IdUser))
                                                    {
                                                        var connectedUser = Program.ConnectedUsers[IdUser];
                                                        
                                                        var updateMessageBody = new messageFormat
                                                        {
                                                            RoomId = messageAdd.RoomId,
                                                            UserId = messageAdd.UserId,
                                                            UserName = messageAdd.UserName,
                                                            Text = messageAdd.Text
                                                        };

                                                        var updateMessage = new GeneralMessage
                                                        {
                                                            Type = "updateMessages",
                                                            Body = MessagePack.MessagePackSerializer.Serialize(updateMessageBody)
                                                        };

                                                        byte[] serializedUpdate = MessagePack.MessagePackSerializer.Serialize(updateMessage);
                                                        byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedUpdate);
                                                        byte[] eofPacket = protocolSI.Make(ProtocolSICmdType.EOF);

                                                        try
                                                        {
                                                            connectedUser.Stream.Write(packet, 0, packet.Length);
                                                            connectedUser.Stream.Write(eofPacket, 0, eofPacket.Length);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            logMessage=$"Erro ao enviar update para o usuário {IdUser}: {ex.Message}";
                                                            LogToFile(logMessage);
                                                            Console.WriteLine(logMessage);
                                                        }
                                                        
                                                    }
                                                }
                                                logMessage = $"Mensagem enviada de user {messageAdd.UserName} para a sala {messageAdd.RoomName} com o texto {messageAdd.Text}.";
                                            }

                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        // Em caso de erro, envie uma resposta de erro
                                        var errorResponse = new ServerResponse
                                        {
                                            Success = false,
                                            Message = "Erro no processamento da mensagem."
                                        };
                                        
                                        SendMessageToClient(networkStream, protocolSI, errorResponse, "errorResponse");
                                        logMessage = $"Erro! Não foi possivel enviar a mensagem: {ex.Message}";
                                    }
                                    LogToFile(logMessage);
                                    Console.WriteLine(logMessage);
                                    break;

                                case "usersInRoom":
                                    // Desserializa o corpo da mensagem usando a classe usersInRoomFormat para obter o RoomId
                                    usersInRoomFormat roomRequest = MessagePack.MessagePackSerializer.Deserialize<usersInRoomFormat>(generalMessage.Body);
                                    int roomId = roomRequest.RoomId;

                                    using (var dbContext = new ChatContext())
                                    {
                                        // Primeiro buscar os dados brutos
                                        var rawUsersList = (from ur in dbContext.UserRooms
                                                            join u in dbContext.Users on ur.IdUser equals u.Id
                                                            where ur.IdRoom == roomId
                                                            select new
                                                            {
                                                                Id = u.Id,
                                                                Name = u.Name,
                                                                State = u.State,
                                                                UserType = ur.UserType
                                                            }).ToList();

                                        // Depois tratar o nome no C#
                                        List<UserListFormat> usersList = rawUsersList.Select(u => new UserListFormat
                                        {
                                            Id = u.Id,
                                            Name = (u.UserType == "A") ? $"{u.Name} (Admin)" : u.Name,
                                            State = u.State
                                        }).ToList();

                                        SendMessageToClient(networkStream, protocolSI, usersList, "usersInRoom");
                                    }
                                    break;



                                case "messagesInRoom":
                                    // Desserializa o Body da mensagem para obter o RoomId
                                    messagesInRoomFormat messageRequest = MessagePack.MessagePackSerializer.Deserialize<messagesInRoomFormat>(generalMessage.Body);
                                    int idRoom = messageRequest.RoomId;

                                    using (var dbContext = new ChatContext())
                                    {
                                        // Realiza um join entre as tabelas Messages e Users para obter o nome do usuário
                                        var messagesList = (from m in dbContext.Messages
                                                            join u in dbContext.Users on m.IdUser equals u.Id
                                                            where m.IdRoom == idRoom
                                                            select new messageFormat
                                                            {
                                                                RoomId = m.IdRoom,
                                                                UserId = m.IdUser,
                                                                Text = u.Name + ": \n " + m.Text,  // Concatenação de strings
                                                                Date = m.Date
                                                            }).ToList();

                                        // Envia a lista de mensagens formatadas para o cliente
                                        SendMessageToClient(networkStream, protocolSI, messagesList, "messagesInRoom");
                                    }
                                    break;
                                


                                case "roomsOfUser":
                                    roomsOfUserFormat requestData = MessagePack.MessagePackSerializer.Deserialize<roomsOfUserFormat>(generalMessage.Body);
                                    int userId = requestData.UserId;

                                    using (var dbContext = new ChatContext())
                                    {
                                        // Realiza o join entre as tabelas UserRooms, Rooms e Users
                                        var roomsList = (from ur in dbContext.UserRooms
                                                            join r in dbContext.Rooms on ur.IdRoom equals r.Id
                                                            join u in dbContext.Users on ur.IdUser equals u.Id
                                                            where ur.IdUser == userId // Filtra pelas salas em que o usuário está
                                                            select new
                                                            {
                                                                RoomId = r.Id,
                                                                RoomName = r.Name,
                                                                AdminId = (from urAdmin in dbContext.UserRooms
                                                                        join uAdmin in dbContext.Users on urAdmin.IdUser equals uAdmin.Id
                                                                        where urAdmin.IdRoom == r.Id && urAdmin.UserType == "A"
                                                                        select uAdmin.Id).FirstOrDefault(), // Nome do administrador da sala
                                                                AdminName = (from urAdmin in dbContext.UserRooms
                                                                        join uAdmin in dbContext.Users on urAdmin.IdUser equals uAdmin.Id
                                                                        where urAdmin.IdRoom == r.Id && urAdmin.UserType == "A"
                                                                        select uAdmin.Name).FirstOrDefault() // Nome do administrador da sala
                                                            }).ToList();

                                        // Formata a lista para enviar ao cliente, incluindo o nome da sala e o nome do administrador
                                        var formattedRoomsList = roomsList.Select(room => new RoomListFormat
                                        {
                                            Id = room.RoomId,
                                            Name = $"{room.RoomName} ({room.AdminName})", // Nome da sala + Nome do administrador
                                            IdAdmin = room.AdminId
                                        }).ToList();

                                        // Envia a resposta ao cliente
                                        SendMessageToClient(networkStream, protocolSI, formattedRoomsList, "roomsOfUser");
                                    }
                                    break;
                                


                                case "register":
                                    var messageRegister = MessagePackSerializer.Deserialize<RegisterRequest>(generalMessage.Body);
                                    logMessage = "";
                                    if (string.IsNullOrEmpty(messageRegister.Username) ||
                                        string.IsNullOrEmpty(messageRegister.Password) ||
                                        string.IsNullOrEmpty(messageRegister.Name))
                                    {
                                        var errorResponse = new ServerResponse
                                        {
                                            Success = false,
                                            Message = "Erro: Todos os campos de registo são obrigatórios."
                                        };
                                        SendMessageToClient(networkStream, protocolSI, errorResponse, "errorResponse");
                                        logMessage = $"Erro! Tentativa de registo com campos inválidos.";
                                    }
                                    else
                                    {
                                        using (var dbContext = new ChatContext())
                                        {
                                            var existingUser = dbContext.Users.FirstOrDefault(u => u.Username == messageRegister.Username);

                                            if (existingUser != null)
                                            {
                                                var errorResponse = new ServerResponse
                                                {
                                                    Success = false,
                                                    Message = "Erro: Já existe um utilizador com o mesmo Username."
                                                };
                                                SendMessageToClient(networkStream, protocolSI, errorResponse, "errorResponse");
                                                logMessage = $"Erro! O utilizador {messageRegister.Name} tentou registar-se com um UserName já existente.";
                                            }
                                            else
                                            {
                                                var newUser = new User
                                                {
                                                    Username = messageRegister.Username,
                                                    Password = messageRegister.Password,
                                                    Name = messageRegister.Name
                                                };

                                                dbContext.Users.Add(newUser);
                                                dbContext.SaveChanges();

                                                var successResponse = new ServerResponse
                                                {
                                                    Success = true,
                                                    Message = "Registo efetuado com sucesso!"
                                                };
                                                SendMessageToClient(networkStream, protocolSI, successResponse, "register");
                                                logMessage = $"O utilizador {messageRegister.Name} registou-se com sucesso com o UserName {messageRegister.Username}.";
                                            }
                                        }
                                    }
                                    LogToFile(logMessage);
                                    Console.WriteLine(logMessage);
                                    break;

                                default:
                                    // caso o TYPE que vem na mensagem não esteja tratado ou seja invalido
                                    var unknownResponse = new ServerResponse
                                    {
                                        Success = false,
                                        Message = "Erro: Tipo de mensagem desconhecido."
                                    };
                                    SendMessageToClient(networkStream, protocolSI, unknownResponse, "unknownResponse");
                                    logMessage = "Erro: Tipo de mensagem desconhecido.";
                                    LogToFile(logMessage);
                                    Console.WriteLine(logMessage);
                                    break;
                            }

                        }
                        catch (Exception ex)
                        {
                            //tratamento para caso alguma coisa corra mal
                            var errorResponse = new ServerResponse
                            {
                                Success = false,
                                Message = $"Erro ao processar a mensagem: {ex.Message}"
                            };
                            SendMessageToClient(networkStream, protocolSI, errorResponse, "errorResponse");

                            string logMessage = $"Erro ao processar a mensagem: {ex.Message}";
                            LogToFile(logMessage);
                            Console.WriteLine(logMessage);
                            break;
                        }
                        break;



                    //quando o cliente termina a transmissão escreve na consola do servidor uma mensagem
                    case ProtocolSICmdType.EOT:
                        //Console.WriteLine($"Finalizando cliente {clientID}");

                        if (client != null && client.Connected)
                        {
                            client.Close(); // Fecha a conexão TCP
                            string logMessage = $"Finalizando cliente {clientID}.";
                            LogToFile(logMessage);
                            Console.WriteLine(logMessage);
                        }
                        break;
                }
            }
        }


        private void SendMessageToClient(NetworkStream stream, ProtocolSI protocol, object body, string type)
        {
            //Console.WriteLine("Estas no Send Message do Servidor");
            // Monta a mensagem geral
            var message = new GeneralMessage
            {
                Type = type,
                Body = MessagePack.MessagePackSerializer.Serialize(body)
            };

            // Serializa a mensagem completa
            byte[] serializedMessage = MessagePack.MessagePackSerializer.Serialize(message);

            
            int chunkSize = 64;
            int totalLength = serializedMessage.Length;
            //Console.WriteLine("Vou enviar uma mensagem (tamanho)= " + totalLength.ToString());
            for (int i = 0; i < totalLength; i += chunkSize)
            {
                // Determina o tamanho do chunk atual (no caso do último, pode ser menor que chunkSize)
                int currentChunkSize = Math.Min(chunkSize, totalLength - i);

                // Cria um novo array para armazenar o chunk atual
                byte[] chunk = new byte[currentChunkSize];
                Array.Copy(serializedMessage, i, chunk, 0, currentChunkSize);

                // Cria a mensagem p/ o protocolo SI usando o chunk atual
                byte[] packet = protocol.Make(ProtocolSICmdType.DATA, chunk);
                stream.Write(packet, 0, packet.Length);
                //Console.WriteLine("pacotes enviados = " + packet.Length.ToString());


                // Aguardar ACK para garantir que o pacote foi recebido
                int x = 0;
                while (true)
                {
                    
                    protocol.Buffer = new byte[protocol.Buffer.Length];
                    int bytesRead = stream.Read(protocol.Buffer, 0, protocol.Buffer.Length);
                    if (bytesRead == 0)
                    {
                        throw new Exception("Conexão fechada pelo servidor.");
                    }

                    // Verifica o tipo de comando recebido
                    var cmd = protocol.GetCmdType();
                    if (cmd == ProtocolSICmdType.ACK) // Espera um ACK
                    {
                        //Console.WriteLine("Recebi ACK");
                        break;
                    }
                    x++;
                    //Console.WriteLine("pacotes enviados = "+ x.ToString());
                }
            }

            // Envia uma mensagem de EOF
            byte[] eofPacket = protocol.Make(ProtocolSICmdType.EOF, new byte[0]);
            stream.Write(eofPacket, 0, eofPacket.Length);
            //Console.WriteLine("Enviei EOF");
        }

        private void LogToFile(string message)
        {
            string logFilePath = "server_log.txt";

            // Garante que o ficheiro existe; se não existir, cria-o
            if (!File.Exists(logFilePath))
            {
                using (File.Create(logFilePath)) { }
            }

            // Formata a mensagem com data e hora
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}";

            // Escreve no final do ficheiro
            File.AppendAllText(logFilePath, logEntry);
        }


        //função para criar uma nova Room
        private void CreateRoom(MessageRoomCreate message)
        {
            //Console.WriteLine($"Criando sala: {message.Name}");
            

            using (var dbContext = new ChatContext())
            { 
            
                Room Room = new Room(message.Name);
                dbContext.Rooms.Add(Room);
                dbContext.SaveChanges();
                int roomId = Room.Id;


                //depois de criada a sala agora temos de criar a associacao de user com a sala

                UserRoom UserRoom = new UserRoom { 
                    IdUser = message.IdUser,
                    IdRoom = roomId,
                    UserType = "A",
                    DateCreated = DateTime.Now,
                    UserState = "Active"
                };
                
                dbContext.UserRooms.Add(UserRoom);
                dbContext.SaveChanges();
                //mensagem para o servidor e guardar no ficheiro LOG
            }
        }

        private void usersAddRoom(int idRoom, int idUser)
        {
            using (var dbContext = new ChatContext())
            {
                bool duplicated = dbContext.UserRooms.Any(ur => ur.IdUser == idUser && ur.IdRoom == idRoom);
                // Se não existir, cria um novo registro
                if (!duplicated)
                {
                    
                    UserRoom userRoom = new UserRoom(idUser, idRoom, "G", "Active");
                    dbContext.UserRooms.Add(userRoom);
                    dbContext.SaveChanges();
                }

            }
            
        }
    }
}



//estruturas partilhadas 
// ou seja estas estruturas são transmitidas cliente<>servidor para se saber a estrutura a ser serializada
// GeneralMessage é a primeira e é sempre igual depois consoante o que vai no Body é realizada a tarefa
namespace Shared
{
   

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

    //[MessagePackObject]
    //public class UserRoomListFormat
    //{
    //    [Key(0)]
    //    public int Id { get; set; }

    //    [Key(1)]
    //    public string Name { get; set; }

    //    [Key(2)]
    //    public bool State { get; set; }

    //    public override string ToString()
    //    {
    //        return Name;
    //    }
    //}
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


