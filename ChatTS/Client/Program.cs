using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    static class Program
    {

        // Conexão global com o servidor, compartilhada por todas as forms
        public static Shared.ServerConnection GlobalServerConnection { get; private set; }

        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            // Cria e conecta à instância global
            GlobalServerConnection = new Shared.ServerConnection();

            try
            {
                GlobalServerConnection.Connect();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao conectar com o servidor: " + ex.Message);
                return;
            }

            // Inicia a thread de recepção de mensagens de forma global
            Thread receiveThread = new Thread(() => GlobalServerConnection.ReceiveMessage())
            {
                IsBackground = true // Será encerrada automaticamente quando o processo terminar
            };
            receiveThread.Start();








            Application.Run(new frmLogin()); // INICIA PRIMEIRO A JANELA DE LOGIN
            
            
        }
    }
}
