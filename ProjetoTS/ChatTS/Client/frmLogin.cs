using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class frmLogin: Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }


        private void btLogin_Click(object sender, EventArgs e)
        {
            string user = txtUsername.Text;
            string pass = txtPassword.Text;


            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Por favor preencha todos os campos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Height = 450;
            txtUsername.Enabled = false;
            txtPassword.Enabled = false;
            btLogin.Enabled = false;

        }

        private void btSubmit_Click(object sender, EventArgs e)
        {
            string username = txtUserRgst.Text.Trim();
            string pass = txtPassRgst.Text;
            string confirm = txtConfirmPassRgst.Text;
            string nome = txtNomeRgst.Text;

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

            /************ ESCONDE CAMPOS DE REGISTO ********/
            txtNomeRgst.Visible = false;
            txtUserRgst.Visible = false;
            txtPassRgst.Visible = false;
            txtConfirmPassRgst.Visible = false;
            labelNomeRgst.Visible = false;
            labelUserRgst.Visible = false;
            labelPassRgst.Visible = false;
            labelPassConfirm.Visible = false;
            /************ FIM ESCONDE CAMPOS DE REGISTO ********/

            labelThankYou.Visible = true;  //MOSTRA LABEL OBRIGADO

            btSubmit.Visible = false; // Esconde Botao Submeter

            btVoltar.Visible = true; // Coloca Visivel o botao voltar

        }

        private void btVoltar_Click(object sender, EventArgs e)
        {


            txtUsername.Enabled = true;
            txtPassword.Enabled = true;
            btLogin.Enabled = true;

            /************ MOSTRA CAMPOS DE REGISTO ********/
            txtUserRgst.Visible = true;
            txtPassRgst.Visible = true;
            txtConfirmPassRgst.Visible = true;
            labelUserRgst.Visible = true;
            labelPassRgst.Visible = true;
            labelPassConfirm.Visible = true;

            /************ FIM MOSTRA CAMPOS DE REGISTO ********/

            btSubmit.Visible = true; // Mostra Botao Submeter

            btVoltar.Visible = false; // Esconde o botao voltar

            labelThankYou.Visible = false;  //ESCONDE LABEL OBRIGADO

            this.Height = 220;

        }
    }
}
