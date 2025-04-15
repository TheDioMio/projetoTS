using System;
using EI.SI;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1: Form
    {
        private const int PORT = 10000;
        NetworkStream networkStream;
        ProtocolSI protocolSI;
        TcpClient client;

        public Form1()
        {
            InitializeComponent();
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Loopback, PORT);
            client = new TcpClient();
            client.Connect(endpoint);
            networkStream = client.GetStream();
            protocolSI = new ProtocolSI();
        }

        // Método do botão enviar
        private void buttonSend_Click(object sender, EventArgs e)
        {
            string msg = textBoxMessage.Text;
            textBoxMessage.Clear();
            byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, msg); //cria uma mensagem/pacote de um tipo específico
            networkStream.Write(packet, 0, packet.Length);
            while (protocolSI.GetCmdType() != ProtocolSICmdType.ACK)
            {
                networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
            }
        }
        // O envio de bits/bytes é trivial recorrendo ao ProtocolSI.

        //Método para fechar o Client
        private void CloseClient()
            {
                try
                {
                    // Definição da variável eot (End of Transmission) do tipo array de byte.
                    // Utilização do método Make. ProtocolSICmdType serve para enviar dados
                    byte[] eot = protocolSI.Make(ProtocolSICmdType.EOT);

                    // A classe NetworkStream disponibiliza métodos para enviar/receber dados através de socket Stream
                    // O Socket de rede é um endpoint interno para envio e recepção de dados com um nó/computador presente na rede.
                    networkStream.Write(eot, 0, eot.Length);
                    networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                    networkStream.Close();
                    client.Close();
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            //Método para fechar o formulário
            private void Client_FormClosing(object sender, FormClosingEventArgs e)
            {
                // Chamar a função para fechar o Client
                CloseClient();
            }

            //Método para o Botão para sair
            private void buttonQuit_Click(object sender, EventArgs e)
            {
                // Chamar a função para fechar o Client e associar a este próprio botão
                CloseClient();
                this.Close();
            }

    }
}
