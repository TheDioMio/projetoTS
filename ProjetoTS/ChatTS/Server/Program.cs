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

// a estrutura base do programa é a base da ficha 3 das aulas

// de momento tem algumas mensagens a trabalhar para efeitos de debug
namespace Server
{
    class Program
    {
        private const int PORT = 10000;
        
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
                //TcpClient client = listener.AcceptTcpClient();
                clientCounter++;
                Console.WriteLine("Client {0} connected", clientCounter);

                ClientHandler clientHandler = new ClientHandler(client, clientCounter);
                clientHandler.Handle();
            }
        }
    }
    class ClientHandler
    {
        private TcpClient client;
        private int clientID;
        private Dictionary<int, TcpClient> users = new Dictionary<int, TcpClient>();

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
                                    //tratamento para ver se o campo nome vem correto
                                    // podemos e devemos implementar a ver se a sala já existe na base de dados
                                    if (string.IsNullOrEmpty(messageRoomCreate.Name))
                                    {
                                        var errorResponse = new ServerResponse
                                        {
                                            Success = false,
                                            Message = "Erro: Nome da sala é inválido."
                                        };
                                        SendMessageToClient(networkStream, protocolSI, errorResponse);
                                    }
                                    else
                                    {
                                        // chama afunção CreateRoom para criar a Room que recebeu do cliente
                                        CreateRoom(messageRoomCreate.Name, messageRoomCreate.IdUser);

                                        //formata a mensagem de retorno para o cliente
                                        var successResponse = new ServerResponse
                                        {
                                            Success = true,
                                            Message = $"Sala '{messageRoomCreate.Name}' criada com sucesso."
                                        };
                                        SendMessageToClient(networkStream, protocolSI, successResponse);
                                    }
                                    break;


                                case "usersaddroom":
                                    // Desserializar para MessageRoomCreate o BODY da mensagem
                                    var messageUsersAddRoom = MessagePack.MessagePackSerializer.Deserialize<usersAddRoomFormat>(generalMessage.Body);
                                    Console.WriteLine($"UserId recebido no servidor: {messageUsersAddRoom.UserId}");
                                    
                                    
                                    if ((messageUsersAddRoom.UserId<0)|| (messageUsersAddRoom.RoomId < 0))
                                    {
                                        var errorResponse = new ServerResponse
                                        {
                                            Success = false,
                                            Message = "Erro: Valores dos indices inválidos."
                                        };
                                        SendMessageToClient(networkStream, protocolSI, errorResponse);
                                    }
                                    else
                                    {
                                        // chama afunção CreateRoom para criar a Room que recebeu do cliente
                                        usersAddRoom(messageUsersAddRoom.RoomId, messageUsersAddRoom.UserId);

                                        //formata a mensagem de retorno para o cliente
                                        var successResponse = new ServerResponse
                                        {
                                            Success = true,
                                            Message = $"O utilizador '{messageUsersAddRoom.UserId}' foi relacionado com a sala'{messageUsersAddRoom.RoomId}' com sucesso."
                                        };
                                        SendMessageToClient(networkStream, protocolSI, successResponse);
                                    }
                                    break;


                                case "logout":

                                    //vamos ter de apagar o user da lista 
                                    
                                    var messageLogout = MessagePack.MessagePackSerializer.Deserialize<LogoutRequest>(generalMessage.Body);

                                    if (messageLogout.IdUser <0)
                                    {
                                        break;
                                    }
                                    users.Remove(messageLogout.IdUser);



                                    break;



                                case "login":
                                    var messageLogin = MessagePack.MessagePackSerializer.Deserialize<LoginRequest>(generalMessage.Body);

                                    if(string.IsNullOrEmpty(messageLogin.Username)|| string.IsNullOrEmpty(messageLogin.Password))
                                    {
                                        var errorResponse = new ServerResponse
                                        {
                                            Success = false,
                                            Message = "Erro: Credênciais inválidas."
                                        };
                                        SendMessageToClient(networkStream, protocolSI, errorResponse);
                                    }
                                    else
                                    {
                                        using (var dbContext = new ChatContext()) // Substitua "YourDbContext" pelo nome do seu DbContext
                                        {
                                            // Procurar o usuário na tabela Users com base no username e password
                                            var user = dbContext.Users.FirstOrDefault(u => u.Username == messageLogin.Username && u.Password == messageLogin.Password);

                                            if (user == null)
                                            {
                                                // Usuário não encontrado ou credenciais incorretas
                                                var errorResponse = new ServerResponse
                                                {
                                                    Success = false,
                                                    Message = "Erro: Credenciais inválidas."
                                                };
                                                SendMessageToClient(networkStream, protocolSI, errorResponse);
                                            }
                                            else
                                            {
                                                // User encontrado - login bem-sucedido
                                                var successResponse = new ServerResponse
                                                {
                                                    Success = true,
                                                    Message = "Bem vindo, "+user.Name,
                                                    IdUser = user.Id
                                                };
                                                users.Add(user.Id, client);
                                                SendMessageToClient(networkStream, protocolSI, successResponse);
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
                                        // Serializa a lista de usuários usando MessagePack.
                                        byte[] serializedUsers = MessagePack.MessagePackSerializer.Serialize(usersList);

                                        // Cria um objeto de mensagem geral para encapsular a lista.
                                        GeneralMessage Message = new GeneralMessage
                                        {
                                            Type = "allusers", // Tipo para identificar que esta mensagem contém todos os usuários.
                                            Body = serializedUsers
                                        };

                                        // Serializa a mensagem geral.
                                        byte[] serializedGeneralMessage = MessagePack.MessagePackSerializer.Serialize(Message);

                                        
                                        byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedGeneralMessage);

                                        // Envia o pacote para o cliente via NetworkStream.
                                        networkStream.Write(packet, 0, packet.Length);
                                    }
                                    break;


                                case "sendmessage":

                                    // Desserializar para messageFormat o BODY da mensagem
                                    var messageAdd = MessagePack.MessagePackSerializer.Deserialize<messageFormat>(generalMessage.Body);
                                    Console.WriteLine($"Mensagem recebida: {messageAdd.Text}");


                                    if ((messageAdd.UserId < 0) || (messageAdd.RoomId < 0)||(string.IsNullOrEmpty(messageAdd.Text)))
                                    {
                                        var errorResponse = new ServerResponse
                                        {
                                            Success = false,
                                            Message = "Erro: Parametros inválidos."
                                        };
                                        SendMessageToClient(networkStream, protocolSI, errorResponse);
                                    }
                                    else
                                    {
                                        

                                        using (var dbContext = new ChatContext())
                                        {
                                            // regista a mensagem
                                            Message Message = new Message(messageAdd.UserId, messageAdd.RoomId, messageAdd.Text);
                                            dbContext.Messages.Add(Message);
                                            dbContext.SaveChanges();
                                        }

                                        //formata a mensagem de retorno para o cliente
                                        var successResponse = new ServerResponse
                                            {
                                                Success = true,
                                                Message = $"Mensagem enviada com sucesso."
                                            };
                                        SendMessageToClient(networkStream, protocolSI, successResponse);
                                    }


                                    break;

                                case "usersinroom":
                                    // Desserializa o corpo da mensagem usando a classe usersInRoomFormat para obter o RoomId
                                    usersInRoomFormat roomRequest = MessagePack.MessagePackSerializer.Deserialize<usersInRoomFormat>(generalMessage.Body);
                                    int roomId = roomRequest.RoomId;

                                    using (var dbContext = new ChatContext())
                                    {
                                        // Realiza um join entre UserRooms e Users para obter os usuários associados à sala com o RoomId informado
                                        List<UserListFormat> usersList = (from ur in dbContext.UserRooms
                                                                          join u in dbContext.Users on ur.IdUser equals u.Id
                                                                          where ur.IdRoom == roomId
                                                                          select new UserListFormat
                                                                          {
                                                                              Id = u.Id,
                                                                              Name = u.Name,
                                                                              State = u.State
                                                                          }).ToList();

                                        // Serializa a lista de usuários usando MessagePack
                                        byte[] serializedUsers = MessagePack.MessagePackSerializer.Serialize(usersList);

                                        // Cria um objeto de mensagem geral para encapsular a lista serializada e define o Type como "usersInRoom"
                                        GeneralMessage responseMessage = new GeneralMessage
                                        {
                                            Type = "usersInRoom",
                                            Body = serializedUsers
                                        };

                                        // Serializa a mensagem geral
                                        byte[] serializedGeneralMessage = MessagePack.MessagePackSerializer.Serialize(responseMessage);

                                        // Cria o pacote com o protocolo customizado e envia ao cliente via networkStream
                                        byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedGeneralMessage);
                                        networkStream.Write(packet, 0, packet.Length);
                                    }
                                    break;

                                case "messagesinroom":
                                    {
                                        // Desserializa o Body da mensagem para obter o RoomId
                                        messagesInRoomFormat messageRequest = MessagePack.MessagePackSerializer.Deserialize<messagesInRoomFormat>(generalMessage.Body);
                                        int idRoom = messageRequest.RoomId;

                                        using (var dbContext = new ChatContext())
                                        {
                                            // Consulta todos os registros da tabela Messages que possuem o IdRoom igual ao informado
                                            List<messageFormat> messagesList = dbContext.Messages
                                                .Where(m => m.IdRoom == idRoom)
                                                .Select(m => new messageFormat
                                                {
                                                    RoomId = m.IdRoom,
                                                    UserId = m.IdUser,
                                                    Text = m.Text,
                                                    Date = m.Date  
                                                }).ToList();

                                            // Serializa a lista de mensagens usando MessagePack
                                            byte[] serializedMessages = MessagePack.MessagePackSerializer.Serialize(messagesList);

                                            // Cria um objeto GeneralMessage encapsulando a lista serializada e define o tipo como "messagesInRoom"
                                            GeneralMessage responseMessage = new GeneralMessage
                                            {
                                                Type = "messagesInRoom",
                                                Body = serializedMessages
                                            };

                                            // Serializa a mensagem geral
                                            byte[] serializedGeneralMessage = MessagePack.MessagePackSerializer.Serialize(responseMessage);

                                            // Monta o pacote com o protocolo customizado e envia para o cliente via networkStream
                                            byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedGeneralMessage);
                                            networkStream.Write(packet, 0, packet.Length);
                                        }
                                        break;
                                    }


                                case "roomsofuser":
                                    {
                                        // Desserializa o Body da mensagem usando a classe roomsOfUserFormat para obter o UserId
                                        roomsOfUserFormat requestData = MessagePack.MessagePackSerializer.Deserialize<roomsOfUserFormat>(generalMessage.Body);
                                        int userId = requestData.UserId;

                                        using (var dbContext = new ChatContext())
                                        {
                                            // Realiza uma consulta que une a tabela de associação UserRooms com a tabela de Room
                                            // para obter as salas associadas ao usuário cujo ID foi enviado
                                            List<RoomListFormat> roomsList = (from ur in dbContext.UserRooms
                                                                              join r in dbContext.Rooms on ur.IdRoom equals r.Id
                                                                              where ur.IdUser == userId
                                                                              select new RoomListFormat
                                                                              {
                                                                                  Id = r.Id,
                                                                                  Name = r.Name
                                                                              }).ToList();

                                            // Serializa a lista de salas utilizando MessagePack
                                            byte[] serializedRooms = MessagePack.MessagePackSerializer.Serialize(roomsList);

                                            // Cria um objeto de resposta encapsulando a lista serializada e define o tipo como "roomsOfUser"
                                            GeneralMessage responseMessage = new GeneralMessage
                                            {
                                                Type = "roomsOfUser",
                                                Body = serializedRooms
                                            };

                                            // Serializa o objeto de resposta geral
                                            byte[] serializedGeneralMessage = MessagePack.MessagePackSerializer.Serialize(responseMessage);

                                            // Monta o pacote com o protocolo customizado e envia para o cliente via networkStream
                                            byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedGeneralMessage);
                                            networkStream.Write(packet, 0, packet.Length);
                                        }
                                        break;
                                    }

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

                                default:
                                    // caso o TYPE que vem na mensagem não esteja tratado ou seja invalido
                                    var unknownResponse = new ServerResponse
                                    {
                                        Success = false,
                                        Message = "Erro: Tipo de mensagem desconhecido."
                                    };
                                    SendMessageToClient(networkStream, protocolSI, unknownResponse);
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
                            SendMessageToClient(networkStream, protocolSI, errorResponse);
                        }

                        break;

                        //quando o cliente termina a transmissão escreve na consola do servidor uma mensagem
                    case ProtocolSICmdType.EOT:
                        Console.WriteLine($"Finalizando cliente {clientID}");
                        break;
                }
            }
        }


        //função para enviar as mensagens de retorno para o cliente
        private void SendMessageToClient(NetworkStream networkStream, ProtocolSI protocolSI, ServerResponse response)
        {
            byte[] serializedResponse = MessagePack.MessagePackSerializer.Serialize(response);
            byte[] responsePacket = protocolSI.Make(ProtocolSICmdType.DATA, serializedResponse);
            networkStream.Write(responsePacket, 0, responsePacket.Length);
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
    public class ServerConnection
    {
        private const int PORT = 10000;
        private const string IP = "127.0.0.1";
        private TcpClient client;
        private NetworkStream networkStream;
        private ProtocolSI protocolSI;

        public ServerConnection()
        {
            client = new TcpClient();
        }

        public void Connect()
        {
            try
            {
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
    public struct LogoutRequest
    {
        [Key(0)]
        public int IdUser { get; set; }
 
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
            return Name + "(" + State + ")";
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
            return Name + "(" + State + ")";
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


