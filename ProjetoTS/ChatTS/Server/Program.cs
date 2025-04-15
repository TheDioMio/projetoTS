using EI.SI;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    
    class Program
    {
       
       
        //Criar novamente uma constante, tal como feito do lado do cliente.
        private const int PORT = 10000;

        static void Main(string[] args)
        {
            // Definição das variáveis na função principal.
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, PORT);
            TcpListener listener = new TcpListener(endpoint);

            // Iniciar o listener; apresentação da primeira mensagem na linha de comandos e inicialização do contador.
            listener.Start();
            Console.WriteLine("SERVER READY");
            int clientCounter = 0;

            //Criação do ciclo infinito de forma a que este esteja sempre em execução até ordem em contrário
            while (true)
            {
                // Definição da variável client do tipo TcpClient
                TcpClient client = listener.AcceptTcpClient();

                // Incrementação do contador, de forma a que vá sempre somando 1 (+1)
                clientCounter++;

                // Apresentação da mensagem indicative do nº do client na linha de comandos 
                Console.WriteLine("Client {0} connected", clientCounter);

                // Definição da variável clientHandler do tipo TcpClient
                ClientHandler clientHandler = new ClientHandler(client, clientCounter);
                clientHandler.Handle();
            }
        }





    }

    class ClientHandler
    {
        // Definição das variáveis client e clientID.
        private TcpClient client;
        private int clientID;
        public ClientHandler(TcpClient client, int clientID)
        {
            this.client = client;
            this.clientID = clientID;
        }
        public void Handle()
        {
            // Definição da variável thread e arranque da mesma
            // Para relembrar: Threads são unidades de execução dentro de um processo; um conjunto de instruções.
            Thread thread = new Thread(threadHandler);
            thread.Start();
        }
        private void threadHandler()
        {
            // Definição das variáveis networkStream e protocolSI
            NetworkStream networkStream = this.client.GetStream();
            ProtocolSI protocolSI = new ProtocolSI();

            // Ciclo a ser executado até ao fim da transmissão.
            while (protocolSI.GetCmdType() != ProtocolSICmdType.EOT)
            {
                int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                byte[] ack;

                // "Alteração"/mudança entre a apresentação da mensagem e o fim da tranmissão.
                switch (protocolSI.GetCmdType())
                {
                    //Dica do ALT
                    case ProtocolSICmdType.DATA:
                        Console.WriteLine("Client " + clientID + ": " + protocolSI.GetStringFromData());
                        ack = protocolSI.Make(ProtocolSICmdType.ACK);
                        networkStream.Write(ack, 0, ack.Length);
                        break;

                    case ProtocolSICmdType.EOT:
                        Console.WriteLine("Ending Thread from Client {0}", clientID);
                        ack = protocolSI.Make(ProtocolSICmdType.ACK);
                        networkStream.Write(ack, 0, ack.Length);
                        break;
                }
            }

            // Fecho do networkStream e do cliente (TcpClient)
            networkStream.Close();
            client.Close();
        }
    }
}
