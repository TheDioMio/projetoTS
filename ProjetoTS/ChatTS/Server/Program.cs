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
                Console.WriteLine("Client {0} connected", clientCounter);

                ClientHandler clientHandler = new ClientHandler(client, clientCounter);
                clientHandler.Handle();
            }
        }
    }

    public class UserConnection
    {
        public int UserId { get; set; }
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

            while (protocolSI.GetCmdType() != ProtocolSICmdType.EOT)
            {
                int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);


                switch (protocolSI.GetCmdType())
                {
                    case ProtocolSICmdType.DATA:
                        byte[] receivedData = protocolSI.GetData();
                        try
                        {
                            // Desserializar a mensagem geral
                            var generalMessage = MessagePack.MessagePackSerializer.Deserialize<GeneralMessage>(receivedData);



                            switch (generalMessage.Type.ToLower())
                            {
                                //analisa o Type que vem na mensagem 
                                //para criar novas funções é só enviar a mensagem com o type que se deseja e criar o case e a função


                                case "roomcreate":
                                    // Desserializar para MessageRoomCreate o BODY da mensagem
                                    var messageRoomCreate = MessagePack.MessagePackSerializer.Deserialize<MessageRoomCreate>(generalMessage.Body);
                                    Console.WriteLine($"UserId recebido no servidor: {messageRoomCreate.IdUser}");

                                    var responseRoomCreate = new ServerResponse { };
                                    //tratamento para ver se o campo nome vem correto
                                    // podemos e devemos implementar a ver se a sala já existe na base de dados
                                    if (string.IsNullOrEmpty(messageRoomCreate.Name))
                                    {
                                        responseRoomCreate.Success = false;
                                        responseRoomCreate.Message = "Erro: Nome da sala é inválido.";
                                    }
                                    else
                                    {
                                        // chama afunção CreateRoom para criar a Room que recebeu do cliente
                                        CreateRoom(messageRoomCreate.Name, messageRoomCreate.IdUser);

                                        //formata a mensagem de retorno para o cliente
                                        responseRoomCreate.Success = true;
                                        responseRoomCreate.Message = $"Sala '{messageRoomCreate.Name}' criada com sucesso.";
                                    }
                                    SendMessageToClient(networkStream, protocolSI, responseRoomCreate, "roomCreate");
                                    break;


                                case "usersaddroom":
                                    // Desserializar para MessageRoomCreate o BODY da mensagem
                                    var messageUsersAddRoom = MessagePack.MessagePackSerializer.Deserialize<usersAddRoomFormat>(generalMessage.Body);
                                    Console.WriteLine($"UserId recebido no servidor: {messageUsersAddRoom.UserId}");
                                    var responseUserAddRoom = new ServerResponse { };

                                    if ((messageUsersAddRoom.UserId < 0) || (messageUsersAddRoom.RoomId < 0))
                                    {
                                        responseUserAddRoom.Success = false;
                                        responseUserAddRoom.Message = "Erro: Não foi possivel adicionar o utilizador";
                                    }
                                    else
                                    {
                                        // chama afunção CreateRoom para criar a Room que recebeu do cliente
                                        usersAddRoom(messageUsersAddRoom.RoomId, messageUsersAddRoom.UserId);
                                        responseUserAddRoom.Success = true;
                                        responseUserAddRoom.Message = $"O utilizador '{messageUsersAddRoom.UserId}' foi relacionado com a sala'{messageUsersAddRoom.RoomId}' com sucesso.";
                                    }
                                    SendMessageToClient(networkStream, protocolSI, responseUserAddRoom, "userAddRoom");
                                    break;


                                case "logout":

                                    //vamos ter de apagar o user da lista 

                                    var messageLogout = MessagePack.MessagePackSerializer.Deserialize<LogoutRequest>(generalMessage.Body);

                                    if (messageLogout.IdUser >= 0)
                                    {
                                        Program.ConnectedUsers.TryRemove(messageLogout.IdUser, out _);
                                    }
                                    break;


                                case "register":
                                    var messageRegister = MessagePackSerializer.Deserialize<RegisterRequest>(generalMessage.Body);

                                    if (string.IsNullOrEmpty(messageRegister.Username) ||
                                        string.IsNullOrEmpty(messageRegister.Password) ||
                                        string.IsNullOrEmpty(messageRegister.Name))
                                    {
                                        var errorResponse = new ServerResponse
                                        {
                                            Success = false,
                                            Message = "Erro: Todos os campos de registo são obrigatórios."
                                        };
                                        SendMessageToClient(networkStream, protocolSI, errorResponse);
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
                                                    Message = "Erro: Nome de utilizador já existe."
                                                };
                                                SendMessageToClient(networkStream, protocolSI, errorResponse);
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
                                                SendMessageToClient(networkStream, protocolSI, successResponse);
                                            }
                                        }
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
                                        SendMessageToClient(networkStream, protocolSI, errorResponse, "login");
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
                                                SendMessageToClient(networkStream, protocolSI, errorResponse, "login");
                                            }
                                            else
                                            {
                                                // User encontrado - login bem-sucedido
                                                var successResponse = new ServerResponse
                                                {
                                                    Success = true,
                                                    Message = "Bem vindo, " + user.Name,
                                                    IdUser = user.Id
                                                };

                                                Program.ConnectedUsers.TryAdd(user.Id, new UserConnection
                                                {
                                                    UserId = user.Id,
                                                    Client = this.client
                                                });

                                                SendMessageToClient(networkStream, protocolSI, successResponse, "login");
                                            }
                                        }
                                    }
                                    break;

                                case "allusers":

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


                                case "sendmessage":
                                    try
                                    {
                                        // Desserializa para messageFormat o BODY da mensagem
                                        var messageAdd = MessagePack.MessagePackSerializer.Deserialize<messageFormat>(generalMessage.Body);
                                        Console.WriteLine($"Mensagem recebida: {messageAdd.Text}");

                                        if ((messageAdd.UserId < 0) || (messageAdd.RoomId < 0) || (string.IsNullOrEmpty(messageAdd.Text)))
                                        {
                                            // Se os parâmetros forem inválidos, envia um erro de resposta
                                            var errorResponse = new ServerResponse
                                            {
                                                Success = false,
                                                Message = "Erro: Parametros inválidos."
                                            };
                                            SendMessageToClient(networkStream, protocolSI, errorResponse, "sendmessage");
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
                                            SendMessageToClient(networkStream, protocolSI, successResponse, "sendmessage");

                                            Console.WriteLine("Users logados:");

                                            foreach (var connectedUser in Program.ConnectedUsers.Values)
                                            {
                                                Console.WriteLine($"UserId: {connectedUser.UserId}");
                                            }

                                            // Buscar da base de dados os utilizadores daquela sala
                                            //using (var dbContext = new ChatContext())
                                            //{
                                            //    var usersInRoom = dbContext.UserRooms
                                            //                                .Where(ur => ur.IdRoom == messageAdd.RoomId)
                                            //                                .Select(ur => ur.IdUser)
                                            //                                .ToList();

                                            //    foreach (var userId in usersInRoom)
                                            //    {
                                            //        if (Program.ConnectedUsers.ContainsKey(userId))
                                            //        {
                                            //            var connectedUser = Program.ConnectedUsers[userId];
                                            //            if (userId != messageAdd.UserId) // Não enviar para quem mandou
                                            //            {
                                            //                var updateMessageBody = new UpdateRequest
                                            //                {
                                            //                    IdRoom = messageAdd.RoomId
                                            //                };

                                            //                var updateMessage = new GeneralMessage
                                            //                {
                                            //                    Type = "UpdateMessages",
                                            //                    Body = MessagePack.MessagePackSerializer.Serialize(updateMessageBody)
                                            //                };

                                            //                byte[] serializedUpdate = MessagePack.MessagePackSerializer.Serialize(updateMessage);
                                            //                byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedUpdate);
                                            //                byte[] eotPacket = protocolSI.Make(ProtocolSICmdType.EOT);

                                            //                try
                                            //                {
                                            //                    connectedUser.Stream.Write(packet, 0, packet.Length);
                                            //                    connectedUser.Stream.Write(eotPacket, 0, eotPacket.Length);
                                            //                }
                                            //                catch (Exception ex)
                                            //                {
                                            //                    Console.WriteLine($"Erro ao enviar update para o usuário {userId}: {ex.Message}");
                                            //                }
                                            //            }
                                            //        }
                                            //    }
                                            //}


                                            // Notificação para todos os usuários conectados
                                            var updateMessageBody = new UpdateRequest
                                            {
                                                IdRoom = messageAdd.RoomId
                                            };  // Corpo da mensagem com o RoomId
                                            var updateMessage = new GeneralMessage
                                            {
                                                Type = "UpdateMessages",
                                                Body = MessagePack.MessagePackSerializer.Serialize(updateMessageBody)

                                            };

                                            byte[] serializedUpdate = MessagePack.MessagePackSerializer.Serialize(updateMessage);
                                            byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedUpdate);
                                            byte[] eotPacket = protocolSI.Make(ProtocolSICmdType.EOT);

                                            // Envia a mensagem para todos os usuários conectados
                                            foreach (var user in Program.ConnectedUsers.Values)
                                            {
                                                try
                                                {
                                                    user.Stream.Write(packet, 0, packet.Length);
                                                    user.Stream.Write(eotPacket, 0, eotPacket.Length);
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine($"Erro ao enviar para o usuário {user.UserId}: {ex.Message}");
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Erro ao processar a mensagem: {ex.Message}");
                                        // Em caso de erro, envie uma resposta de erro
                                        var errorResponse = new ServerResponse
                                        {
                                            Success = false,
                                            Message = "Erro no processamento da mensagem."
                                        };
                                        Thread.Sleep(100);
                                        SendMessageToClient(networkStream, protocolSI, errorResponse, "sendmessage");
                                    }
                                    break;

                                case "usersinroom":
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




                                //case "usersinroom":
                                //    // Desserializa o corpo da mensagem usando a classe usersInRoomFormat para obter o RoomId
                                //    usersInRoomFormat roomRequest = MessagePack.MessagePackSerializer.Deserialize<usersInRoomFormat>(generalMessage.Body);
                                //    int roomId = roomRequest.RoomId;

                                //    using (var dbContext = new ChatContext())
                                //    {
                                //        // Realiza um join entre UserRooms e Users para obter os usuários associados à sala com o RoomId informado
                                //        List<UserListFormat> usersList = (from ur in dbContext.UserRooms
                                //                                          join u in dbContext.Users on ur.IdUser equals u.Id
                                //                                          where ur.IdRoom == roomId
                                //                                          select new UserListFormat
                                //                                          {
                                //                                              Id = u.Id,
                                //                                              Name = u.Name,
                                //                                              State = u.State
                                //                                          }).ToList();

                                //        SendMessageToClient(networkStream, protocolSI, usersList, "usersInRoom");

                                //    }
                                //    break;



                                case "messagesinroom":
                                    {
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
                                    }



                                //case "messagesinroom":
                                //    {
                                //        // Desserializa o Body da mensagem para obter o RoomId
                                //        messagesInRoomFormat messageRequest = MessagePack.MessagePackSerializer.Deserialize<messagesInRoomFormat>(generalMessage.Body);
                                //        int idRoom = messageRequest.RoomId;

                                //        using (var dbContext = new ChatContext())
                                //        {
                                //            // Consulta todos os registros da tabela Messages que possuem o IdRoom igual ao informado
                                //            List<messageFormat> messagesList = dbContext.Messages
                                //                .Where(m => m.IdRoom == idRoom)
                                //                .Select(m => new messageFormat
                                //                {
                                //                    RoomId = m.IdRoom,
                                //                    UserId = m.IdUser,
                                //                    Text = m.Text,
                                //                    Date = m.Date
                                //                }).ToList();
                                //            SendMessageToClient(networkStream, protocolSI, messagesList, "messagesInRoom");
                                //        }
                                //        break;
                                //    }

                                case "roomsofuser":
                                    {
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
                                                                            select uAdmin.Name).FirstOrDefault() // Nome do administrador da sala
                                                             }).ToList();

                                            // Formata a lista para enviar ao cliente, incluindo o nome da sala e o nome do administrador
                                            var formattedRoomsList = roomsList.Select(room => new RoomListFormat
                                            {
                                                Id = room.RoomId,
                                                Name = $"{room.RoomName} ({room.AdminId})" // Nome da sala + Nome do administrador
                                            }).ToList();

                                            // Envia a resposta ao cliente
                                            SendMessageToClient(networkStream, protocolSI, formattedRoomsList, "roomsOfUser");
                                        }
                                        break;
                                    }


                                //case "roomsofuser":
                                //    {
                                //        // Desserializa o Body da mensagem usando a classe roomsOfUserFormat para obter o UserId
                                //        roomsOfUserFormat requestData = MessagePack.MessagePackSerializer.Deserialize<roomsOfUserFormat>(generalMessage.Body);
                                //        int userId = requestData.UserId;

                                //        using (var dbContext = new ChatContext())
                                //        {
                                //            // Realiza uma consulta que une a tabela de associação UserRooms com a tabela de Room
                                //            // para obter as salas associadas ao usuário cujo ID foi enviado
                                //            List<RoomListFormat> roomsList = (from ur in dbContext.UserRooms
                                //                                              join r in dbContext.Rooms on ur.IdRoom equals r.Id
                                //                                              where ur.IdUser == userId
                                //                                              select new RoomListFormat
                                //                                              {
                                //                                                  Id = r.Id,
                                //                                                  Name = r.Name
                                //                                              }).ToList();

                                //            SendMessageToClient(networkStream, protocolSI, roomsList, "roomsOfUser");
                                //        }
                                //        break;
                                //    }

                                case "register":
                                    var messageRegister = MessagePackSerializer.Deserialize<RegisterRequest>(generalMessage.Body);

                                    if (string.IsNullOrEmpty(messageRegister.Username) ||
                                        string.IsNullOrEmpty(messageRegister.Password) ||
                                        string.IsNullOrEmpty(messageRegister.Name))
                                    {
                                        var errorResponse = new ServerResponse
                                        {
                                            Success = false,
                                            Message = "Erro: Todos os campos de registo são obrigatórios."
                                        };
                                        SendMessageToClient(networkStream, protocolSI, errorResponse, "register");
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
                                                    Message = "Erro: Nome de utilizador já existe."
                                                };
                                                SendMessageToClient(networkStream, protocolSI, errorResponse, "register");
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
                                            }
                                        }
                                    }
                                    break;

                                default:
                                    // caso o TYPE que vem na mensagem não esteja tratado ou seja invalido
                                    var unknownResponse = new ServerResponse
                                    {
                                        Success = false,
                                        Message = "Erro: Tipo de mensagem desconhecido."
                                    };
                                    SendMessageToClient(networkStream, protocolSI, unknownResponse, "unknownResponse");
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
                        }
                        break;



                    //quando o cliente termina a transmissão escreve na consola do servidor uma mensagem
                    case ProtocolSICmdType.EOT:
                        Console.WriteLine($"Finalizando cliente {clientID}");
                        if (client != null && client.Connected)
                        {
                            client.Close(); // Fecha a conexão TCP
                        }
                        break;
                }
            }
        }


        //função para enviar as mensagens de retorno para o cliente
        //private void SendMessageToClient(NetworkStream stream, ProtocolSI protocol, object body, string type)
        //{
        //    // Monta a mensagem geral
        //    var message = new GeneralMessage
        //    {
        //        Type = type,
        //        Body = MessagePack.MessagePackSerializer.Serialize(body)
        //    };

        //    // Serializa a mensagem completa
        //    byte[] serializedMessage = MessagePack.MessagePackSerializer.Serialize(message);

        //    // Empacota com o protocolo
        //    byte[] packet = protocol.Make(ProtocolSICmdType.DATA, serializedMessage);

        //    // Envia pelo stream
        //    stream.Write(packet, 0, packet.Length);

        //    byte[] eotPacket = protocol.Make(ProtocolSICmdType.EOT);
        //    stream.Write(eotPacket, 0, eotPacket.Length);
        //    stream.Flush();
        //}

        //private void SendMessageToClient(NetworkStream stream, ProtocolSI protocol, object body, string type)
        //{
        //    // Monta a mensagem geral
        //    var message = new GeneralMessage
        //    {
        //        Type = type,
        //        Body = MessagePack.MessagePackSerializer.Serialize(body)
        //    };

        //    // Serializa a mensagem completa
        //    byte[] serializedMessage = MessagePack.MessagePackSerializer.Serialize(message);

        //    // Empacota com o protocolo
        //    byte[] packet = protocol.Make(ProtocolSICmdType.DATA, serializedMessage);

        //    // Envia o pacote de dados
        //    stream.Write(packet, 0, packet.Length);

        //    // Aguardar ACK para garantir que o pacote foi recebido
        //    while (true)
        //    {
        //        // Lê o próximo pacote do stream
        //        protocol.Buffer = new byte[protocol.Buffer.Length];
        //        int bytesRead = stream.Read(protocol.Buffer, 0, protocol.Buffer.Length);
        //        if (bytesRead == 0)
        //        {
        //            throw new Exception("Conexão fechada pelo servidor.");
        //        }

        //        // Verifica o tipo de comando recebido
        //        var cmd = protocol.GetCmdType();

        //        // Se for ACK, significa que o pacote foi recebido corretamente
        //        if (cmd == ProtocolSICmdType.ACK)
        //        {
        //            break;
        //        }
        //    }

        //    // Envia o pacote de fim de transmissão (EOT)
        //    byte[] eotPacket = protocol.Make(ProtocolSICmdType.EOT);
        //    stream.Write(eotPacket, 0, eotPacket.Length);
        //    stream.Flush(); // Garante que todos os dados foram enviados
        //}

        private void SendMessageToClient(NetworkStream stream, ProtocolSI protocol, object body, string type)
        {
            // Monta a mensagem geral
            var message = new GeneralMessage
            {
                Type = type,
                Body = MessagePack.MessagePackSerializer.Serialize(body)
            };

            // Serializa a mensagem completa
            byte[] serializedMessage = MessagePack.MessagePackSerializer.Serialize(message);

            // Verifica se o tamanho da mensagem excede o limite
            const int MAX_DATA_LENGTH = 1400;

            int totalLength = serializedMessage.Length;
            int offset = 0;

            while (offset < totalLength)
            {
                int chunkSize = Math.Min(MAX_DATA_LENGTH, totalLength - offset); // Determina o tamanho do fragmento
                byte[] chunk = new byte[chunkSize];
                Array.Copy(serializedMessage, offset, chunk, 0, chunkSize);

                // Empacota o fragmento com o protocolo
                byte[] packet = protocol.Make(ProtocolSICmdType.DATA, chunk);

                // Envia o fragmento
                stream.Write(packet, 0, packet.Length);

                // Aguardar ACK para garantir que o pacote foi recebido
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
                        break;
                    }
                }

                offset += chunkSize; // Atualiza o offset para o próximo fragmento
            }

            // Envia o pacote de fim de transmissão (EOT)
            byte[] eotPacket = protocol.Make(ProtocolSICmdType.EOT);
            stream.Write(eotPacket, 0, eotPacket.Length);
            stream.Flush(); // Garante que todos os dados foram enviados
        }




        //função para criar uma nova Room
        private void CreateRoom(string name, int idUser)
        {
            Console.WriteLine($"Criando sala: {name}");
            

            using (var dbContext = new ChatContext())
            { 
            
                Room Room = new Room(name);
                dbContext.Rooms.Add(Room);
                dbContext.SaveChanges();
                int roomId = Room.Id;


                //depois de criada a sala agora temos de criar a associacao de user com a sala

                UserRoom UserRoom = new UserRoom { 
                    IdUser = idUser,
                    IdRoom = roomId,
                    UserType = "A",
                    DateCreated = DateTime.Now,
                    UserState = "Active"
                };
                
                dbContext.UserRooms.Add(UserRoom);
                dbContext.SaveChanges();
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
                    // Cria o registro com os valores desejados: "Guest" e "Active"
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


