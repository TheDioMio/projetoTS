using EI.SI;
using MessagePack;
using Server.Models;
using Shared;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace Client
{
    public partial class frmLogin : Form
    {
        //private ServerConnection serverConnection; // Instância de conexão com o servidor
        //private ProtocolSI protocolSI;
        private Shared.ServerConnection serverConnection;

        public frmLogin()
        {
            InitializeComponent();
            serverConnection = Program.GlobalServerConnection;
            //serverConnection = new ServerConnection(); // Inicializa a conexão

            //serverConnection = connection ?? throw new ArgumentNullException(nameof(connection));
            //protocolSI = new ProtocolSI();

            // Inicia a escuta de mensagens do servidor
            //StartListening();
        }



        //public void StartListening()
        //{
        //    Thread listenThread = new Thread(ListenForMessages);
        //    listenThread.IsBackground = true;
        //    Console.WriteLine("Thread de escuta iniciada!");
        //    listenThread.Start();
        //}

        //private void ListenForMessages()
        //{
        //    try
        //    {
        //        //while (serverConnection.IsConnected)
        //        //{
        //            // Recebe a resposta do servidor
        //            serverConnection.ReceiveMessage();
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Erro na escuta de mensagens: {ex.Message}");
        //    }
        //}

        public void login(GeneralMessage serverResponse)
        {
            // Desserializa a resposta

            var serverResponseBody = MessagePack.MessagePackSerializer.Deserialize<ServerResponse>(serverResponse.Body);
            if (serverResponseBody.Success)
            {
                //MessageBox.Show("Login bem-sucedido!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ServerConnection.UserSelected = serverResponseBody.IdUser;
                ServerConnection.UserSelectedName = serverResponseBody.Message;

                //MessageBox.Show(serverResponse.IdUser.ToString(), "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Abre o Form1 e passa a instância de conexão
                Form1 form1 = new Form1(serverConnection);
                form1.Text = serverResponseBody.Message;
                form1.Show();
                this.Hide(); // Esconde o formulário de login
            }
            else
            {
                MessageBox.Show($"Falha no login: {serverResponseBody.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } 
        }


        public void register(GeneralMessage serverResponse)
        {

                var serverResponseBody = MessagePack.MessagePackSerializer.Deserialize<ServerResponse>(serverResponse.Body);

                if (serverResponseBody.Success)
                {
                    MessageBox.Show("Registro realizado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ServerConnection.UserSelected = serverResponseBody.IdUser;

                    MessageBox.Show(serverResponseBody.IdUser.ToString(), "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Esconde os campos de registro e mostra a mensagem de agradecimento
                    EsconderCamposRegistro();
                }
                else
                {
                    MessageBox.Show($"Erro no registro: {serverResponseBody.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

        }

        private void btLogin_Click(object sender, EventArgs e)
        {
            string user = txtUsername.Text;
            string pass = txtPassword.Text;

            // Valida os campos de entrada
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Por favor preencha todos os campos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Conecta ao servidor
                //serverConnection.Connect();

                // Cria a mensagem de login
                var loginRequest = new LoginRequest
                {
                    Username = user,
                    Password = pass
                };

                // Cria a mensagem geral encapsulando a mensagem de login
                var generalMessage = new GeneralMessage
                {
                    Type = "login",
                    Body = MessagePack.MessagePackSerializer.Serialize(loginRequest)
                };

                // Serializa e envia a mensagem geral ao servidor
                var packet = MessagePack.MessagePackSerializer.Serialize(generalMessage);
                //var protocolSI = new ProtocolSI();
                //var packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedMessage);
                serverConnection.SendMessage(packet);            
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao conectar: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btSubmit_Click(object sender, EventArgs e)
        {
            string username = txtUserRgst.Text.Trim();
            string pass = txtPassRgst.Text;
            string confirm = txtConfirmPassRgst.Text;
            string nome = txtNomeRgst.Text;

            // Valida os campos de registro
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(pass) || string.IsNullOrEmpty(confirm) || string.IsNullOrEmpty(nome))
            {
                MessageBox.Show("Por favor preencha todos os campos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (pass != confirm)
            {
                MessageBox.Show("As passwords não coincidem.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassRgst.Clear();
                txtConfirmPassRgst.Clear();
                txtPassRgst.Focus();
                return;
            }

            try
            {
                // Conecta ao servidor
                //serverConnection.Connect();

                // Cria a mensagem de registro
                var registerRequest = new RegisterRequest
                {
                    Username = username,
                    Password = pass,
                    Name = nome
                };

                // Cria a mensagem geral encapsulando a mensagem de registro
                var generalMessage = new GeneralMessage
                {
                    Type = "register",
                    Body = MessagePack.MessagePackSerializer.Serialize(registerRequest)
                };

                // Serializa e envia a mensagem geral ao servidor
                var packet = MessagePack.MessagePackSerializer.Serialize(generalMessage);
                //var protocolSI = new ProtocolSI();
                //var packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedMessage);
                serverConnection.SendMessage(packet);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao conectar: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btVoltar_Click(object sender, EventArgs e)
        {
            // Reseta os campos de registro e volta para o formulário de login
            MostrarCamposRegistro();
        }

        private void button1_Click(object sender, EventArgs e)
        {


        }

        

        private void EsconderCamposRegistro()
        {
            txtNomeRgst.Visible = false;
            txtUserRgst.Visible = false;
            txtPassRgst.Visible = false;
            txtConfirmPassRgst.Visible = false;
            labelNomeRgst.Visible = false;
            labelUserRgst.Visible = false;
            labelPassRgst.Visible = false;
            labelPassConfirm.Visible = false;

            labelThankYou.Visible = true; // Mostra a mensagem de agradecimento
            btSubmit.Visible = false; // Esconde o botão de registrar
            btVoltar.Visible = true;  // Mostra o botão de voltar
        }

        private void MostrarCamposRegistro()
        {
            txtUsername.Enabled = true;
            txtPassword.Enabled = true;
            btLogin.Enabled = true;

            txtUserRgst.Visible = true;
            txtPassRgst.Visible = true;
            txtConfirmPassRgst.Visible = true;
            labelUserRgst.Visible = true;
            labelPassRgst.Visible = true;
            labelPassConfirm.Visible = true;

            btSubmit.Visible = true;
            btVoltar.Visible = false;
            labelThankYou.Visible = false;
        }

        private void btRegist_Click(object sender, EventArgs e)
        {
            this.Height = 450;
            txtUsername.Enabled = false;
            txtPassword.Enabled = false;
            btLogin.Enabled = false;
        }
    }
}

