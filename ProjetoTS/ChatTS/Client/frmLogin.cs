using EI.SI;
using MessagePack;
using Shared;
using System;
using System.Windows.Forms;

namespace Client
{
    public partial class frmLogin : Form
    {
        private ServerConnection serverConnection; // Instância de conexão com o servidor

        public frmLogin()
        {
            InitializeComponent();
            serverConnection = new ServerConnection(); // Inicializa a conexão
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
                serverConnection.Connect();

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
                var serializedMessage = MessagePack.MessagePackSerializer.Serialize(generalMessage);
                var protocolSI = new ProtocolSI();
                var packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedMessage);
                serverConnection.SendMessage(packet);

                // Recebe a resposta do servidor
                byte[] response = serverConnection.ReceiveMessage();

                // Desserializa a resposta
                var serverResponse = MessagePack.MessagePackSerializer.Deserialize<ServerResponse>(response);

                if (serverResponse.Success)
                {
                    MessageBox.Show("Login bem-sucedido!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Abre o Form1 e passa a instância de conexão
                    Form1 form1 = new Form1(serverConnection);
                    form1.Show();
                    this.Hide(); // Esconde o formulário de login
                }
                else
                {
                    MessageBox.Show($"Falha no login: {serverResponse.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
                serverConnection.Connect();

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
                var serializedMessage = MessagePack.MessagePackSerializer.Serialize(generalMessage);
                var protocolSI = new ProtocolSI();
                var packet = protocolSI.Make(ProtocolSICmdType.DATA, serializedMessage);
                serverConnection.SendMessage(packet);

                // Recebe a resposta do servidor
                byte[] response = serverConnection.ReceiveMessage();

                // Desserializa a resposta
                var serverResponse = MessagePack.MessagePackSerializer.Deserialize<ServerResponse>(response);

                if (serverResponse.Success)
                {
                    MessageBox.Show("Registro realizado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Esconde os campos de registro e mostra a mensagem de agradecimento
                    EsconderCamposRegistro();
                }
                else
                {
                    MessageBox.Show($"Erro no registro: {serverResponse.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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










//using Shared;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace Client
//{
//    public partial class frmLogin: Form
//    {
//        private ServerConnection serverConnection;

//        public frmLogin()
//        {
//            InitializeComponent();
//            serverConnection = new ServerConnection();
//        }



//        private void btLogin_Click(object sender, EventArgs e)
//        {
//            string user = txtUsername.Text;
//            string pass = txtPassword.Text;


//            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(pass))
//            {
//                MessageBox.Show("Por favor preencha todos os campos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);

//            }
//            else
//            {
//                try
//                {
//                    serverConnection.Connect();

//                    // Simular envio de mensagem de login
//                    var message = "LOGIN_REQUEST"; // Modifique conforme necessário
//                    var protocolSI = new EI.SI.ProtocolSI();
//                    var packet = protocolSI.Make(EI.SI.ProtocolSICmdType.DATA, message);

//                    serverConnection.SendMessage(packet);


//                    // Receber resposta do servidor
//                    var response = serverConnection.ReceiveMessage();
//                    if (response != null) // Simular validação de login
//                    {
//                        MessageBox.Show("Login bem-sucedido!");

//                        // Abrir Form1 e passar a conexão
//                        Form1 form1 = new Form1(serverConnection);
//                        form1.Show();
//                        this.Hide();
//                    }
//                    else
//                    {
//                        MessageBox.Show("Falha no login!");
//                    }
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show($"Erro: {ex.Message}");
//                }





//            }






//        }

//        private void button1_Click(object sender, EventArgs e)
//        {
//            this.Height = 450;
//            txtUsername.Enabled = false;
//            txtPassword.Enabled = false;
//            btLogin.Enabled = false;

//        }

//        private void btSubmit_Click(object sender, EventArgs e)
//        {
//            string username = txtUserRgst.Text.Trim();
//            string pass = txtPassRgst.Text;
//            string confirm = txtConfirmPassRgst.Text;
//            string nome = txtNomeRgst.Text;

//            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(pass) || string.IsNullOrEmpty(confirm) || string.IsNullOrEmpty(nome))
//            {
//                MessageBox.Show("Por favor preencha todos os campos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                return;
//            }

//            if (pass != confirm)
//            {
//                MessageBox.Show("As passwords não coincidem.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                txtPassRgst.Clear();
//                txtConfirmPassRgst.Clear();
//                txtPassRgst.Focus();
//                return;
//            }

//            /************ ESCONDE CAMPOS DE REGISTO ********/
//            txtNomeRgst.Visible = false;
//            txtUserRgst.Visible = false;
//            txtPassRgst.Visible = false;
//            txtConfirmPassRgst.Visible = false;
//            labelNomeRgst.Visible = false;
//            labelUserRgst.Visible = false;
//            labelPassRgst.Visible = false;
//            labelPassConfirm.Visible = false;
//            /************ FIM ESCONDE CAMPOS DE REGISTO ********/

//            labelThankYou.Visible = true;  //MOSTRA LABEL OBRIGADO

//            btSubmit.Visible = false; // Esconde Botao Submeter

//            btVoltar.Visible = true; // Coloca Visivel o botao voltar

//        }

//        private void btVoltar_Click(object sender, EventArgs e)
//        {


//            txtUsername.Enabled = true;
//            txtPassword.Enabled = true;
//            btLogin.Enabled = true;

//            /************ MOSTRA CAMPOS DE REGISTO ********/
//            txtUserRgst.Visible = true;
//            txtPassRgst.Visible = true;
//            txtConfirmPassRgst.Visible = true;
//            labelUserRgst.Visible = true;
//            labelPassRgst.Visible = true;
//            labelPassConfirm.Visible = true;

//            /************ FIM MOSTRA CAMPOS DE REGISTO ********/

//            btSubmit.Visible = true; // Mostra Botao Submeter

//            btVoltar.Visible = false; // Esconde o botao voltar

//            labelThankYou.Visible = false;  //ESCONDE LABEL OBRIGADO

//            this.Height = 220;

//        }
//    }
//}
