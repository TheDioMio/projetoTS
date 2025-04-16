using EI.SI;
using MessagePack;
using Shared;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        private const int PORT = 10000;
        private TcpClient client;
        private NetworkStream networkStream;
        private ProtocolSI protocolSI;

        public Form1()
        {
            InitializeComponent();

            try
            {
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Loopback, PORT);
                client = new TcpClient();
                client.Connect(endpoint);
                networkStream = client.GetStream();
                protocolSI = new ProtocolSI();

                Console.WriteLine("Conexão estabelecida com o servidor.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao conectar ao servidor: {ex.Message}");
                this.Close();
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            string name = textBoxMessage.Text;
            //verifica se o campo de texto está preenchido
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Por favor, insira um nome válido para a sala.");
                return;
            }
            //cria e define a mensagem a ser enviada para o servidor para criar a Room
            var roomCreateMessage = new MessageRoomCreate
            {
                Action = "roomcreate",
                Name = name
            };
            //Cria a mensagem Geral para enviar para o servidor com o typo a ser igual a função que vai ser chamada no server, e o Body a mensagem para criar a sala incriptada
            var generalMessage = new GeneralMessage
            {
                Type = "roomcreate",
                Body = MessagePack.MessagePackSerializer.Serialize(roomCreateMessage)
            };

            try
            {
                // converte a mensagem geral e envia para o servidor
                byte[] serializedGeneralMessage = MessagePack.MessagePackSerializer.Serialize(generalMessage);
                byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedGeneralMessage);
                networkStream.Write(packet, 0, packet.Length);

                //recebe do server a mensagem, descerializa e mostra o que vem na messagem box
                //futuramente é para tratar os dados que recebe em vez de mandar uma MSG
                //MessageBox apenas para efeito de DEBUG
                networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                if (protocolSI.GetCmdType() == ProtocolSICmdType.DATA)
                {
                    byte[] receivedData = protocolSI.GetData();
                    var serverResponse = MessagePack.MessagePackSerializer.Deserialize<ServerResponse>(receivedData);

                    MessageBox.Show($"Servidor respondeu: {serverResponse.Message}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao enviar mensagem: {ex.Message}");
            }
        }


        //escreve na consola que o cliente terminor a sessão, e termina as ligações
        private void CloseClient()
        {
            try
            {
                byte[] eot = protocolSI.Make(ProtocolSICmdType.EOT);
                networkStream.Write(eot, 0, eot.Length);
                networkStream.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao fechar conexão: {ex.Message}");
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
    }
}

// estruturas partilhadas 
// ou seja estas estruturas são transmitidas cliente<>servidor para se saber a estrutura a ser serializada
// GeneralMessage é a primeira e é sempre igual depois consoante o que vai no Body é realizada a tarefa

namespace Shared
{
    [MessagePackObject]
    public struct GeneralMessage
    {
        [Key(0)]
        public string Type { get; set; } // Identifica o tipo da mensagem

        [Key(1)]
        public byte[] Body { get; set; } // Dados serializados da mensagem específica
    }

    [MessagePackObject]
    public struct MessageRoomCreate
    {
        [Key(0)]
        public string Action { get; set; }
        //é a acção que se pretende fazer, e vai chamar uma função com o mesmo nome 
        [Key(1)]
        public string Name { get; set; }
    }


    //estrutura da mensagem de retorno para o cliente
    [MessagePackObject]
    public struct ServerResponse
    {
        [Key(0)]
        public bool Success { get; set; }

        [Key(1)]
        public string Message { get; set; }
    }
}
