using Biblioteca.ClassesBasicas;
using Biblioteca.Negocio;
using Consultorio.ServiceReference1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Consultorio
{
    /// <summary>
    /// Lógica interna para CadastrarPaciente.xaml
    /// </summary>
    public partial class CadastrarPaciente : Window
    {

        private Paciente pPaciente;

        public CadastrarPaciente()
        {
            InitializeComponent();
        }

        private void Tb_CPF_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            FormatarCPF_KeyPress((TextBox)sender, e);
        }


        private void Tb_CPF_LostFocus(object sender, RoutedEventArgs e)
        {
            FormatarCPF_Leave((TextBox)sender);
        }

        private void FormatarCPF_KeyPress(TextBox campoCPF, TextCompositionEventArgs e)
        {

            if (!Char.IsDigit(Convert.ToChar(e.Text)) && Convert.ToChar(e.Text) != (char)8)
            {
                e.Handled = true;
            }
            if (char.IsNumber(Convert.ToChar(e.Text)) == true)
            {
                switch (campoCPF.Text.Length)
                {
                    case 0:
                        campoCPF.Text = string.Empty;
                        break;
                    case 3:
                        campoCPF.Text += ".";
                        campoCPF.SelectionStart = 5;
                        break;
                    case 7:
                        campoCPF.Text += ".";
                        campoCPF.SelectionStart = 9;
                        break;
                    case 11:
                        campoCPF.Text += "-";
                        campoCPF.SelectionStart = 13;
                        break;
                }
            }
        }

        private void FormatarCPF_Leave(TextBox campoCPF)
        {
            if (!14.Equals(campoCPF.Text.Length) && !11.Equals(campoCPF.Text.Length) && !10.Equals(campoCPF.Text.Length))
            {
                campoCPF.Text = string.Empty;
            }
            else
            {
                campoCPF.Text = SiteUtil.FormatarCPF(campoCPF.Text);
            }
        }

        private void TxtCEP_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ConsultarEndereco();
            }
        }

        private void ConsultarEndereco()
        {
            if (10.Equals(txtCEP.Text.Length) || 8.Equals(txtCEP.Text.Length))
            {
                txtCEP.Text = SiteUtil.FormatarCEP(txtCEP.Text);
                var modeloRetorno = new Service1Client().ConsultarCEP(txtCEP.Text);
                if ("1".Equals(modeloRetorno.Resultado) || "2".Equals(modeloRetorno.Resultado))
                {
                    txt_Logradouro.Text = modeloRetorno.TipoLogradouro + " " + modeloRetorno.Logradouro;
                    txt_Bairro.Text = modeloRetorno.Bairro;
                    txt_Cidade.Text = modeloRetorno.Cidade;
                    txt_Estado.Text = modeloRetorno.UF;
                }
                else
                {
                    LimparEndereco();
                    MessageBox.Show("Sua busca não retornou resultados!", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                LimparEndereco();
            }

        }

        private void LimparEndereco()
        {
            txt_Logradouro.Text = string.Empty;
            txt_Bairro.Text = string.Empty;
            txt_Cidade.Text = string.Empty;
            txt_Estado.Text = string.Empty;
        }

        private void TxtCEP_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(Convert.ToChar(e.Text)) && Convert.ToChar(e.Text) != (char)8)
            {
                e.Handled = true;
            }

            if (char.IsNumber(Convert.ToChar(e.Text)) == true)
            {
                switch (txtCEP.Text.Length)
                {
                    case 0:
                        txtCEP.Text = string.Empty;
                        break;
                    case 2:
                        txtCEP.Text += ".";
                        txtCEP.SelectionStart = 4;
                        break;
                    case 6:
                        txtCEP.Text += "-";
                        txtCEP.SelectionStart = 8;
                        break;
                }
            }
        }

        private void TxtTelefone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(Convert.ToChar(e.Text)) && Convert.ToChar(e.Text) != (char)8)
            {
                e.Handled = true;
            }

            if (char.IsNumber(Convert.ToChar(e.Text)) == true)
            {
                switch (txtTelefone.Text.Length)
                {
                    case 0:
                        txtTelefone.Text = "";
                        break;
                    case 1:
                        txtTelefone.Text = "(" + txtTelefone.Text;
                        txtTelefone.SelectionStart = 3;
                        break;
                    case 3:
                        txtTelefone.Text += ") ";
                        txtTelefone.SelectionStart = 7;
                        break;
                    case 6:
                        txtTelefone.Text += ".";
                        txtTelefone.SelectionStart = 8;
                        break;
                    case 11:
                        txtTelefone.Text += "-";
                        txtTelefone.SelectionStart = 13;
                        break;
                }
            }
        }

        private void TxtCEP_LostFocus(object sender, EventArgs e)
        {
            ConsultarEndereco();
        }

        private void Bt_Confirmar_Click(object sender, EventArgs e)
        {
            try
            {
                ValidarInclusao();
                PreencherInclusao();
                new Service1Client().InserirPaciente(pPaciente);
                ((MainWindow)this.Owner).ConsultarPacientes();
                this.Close();

            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Mensagem", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ValidarInclusao()
        {

            if (string.Empty.Equals(txtNomePaciente.Text) || string.Empty.Equals(txtCpf.Text) || string.Empty.Equals(txtTelefone.Text) || SiteUtil.FormatarData(DateTime.Now).Equals(SiteUtil.FormatarData((DateTime)dataNascimento.SelectedDate)))
            {
                throw new Exception("Campos obrigatórios não preenchidos!");
            }

            if (!SiteUtil.IsValidCPF(txtCpf.Text))
            {
                throw new Exception("CPF invalido.");
            }

            if (!string.Empty.Equals(txt_Logradouro.Text) && string.Empty.Equals(txtNumero.Text))
            {
                throw new Exception("Favor informar um número para o endereço");
            }
        }

        private void PreencherInclusao()
        {
            pPaciente = new Paciente
            {
                Nome = txtNomePaciente.Text
            };
            String retornoCPF = SiteUtil.RemoverCaracteresEspecial(txtCpf.Text);
            if (!string.Empty.Equals(retornoCPF))
            {
                pPaciente.Cpf = Convert.ToInt64(retornoCPF);
            }

            String retornoTelefone = SiteUtil.RemoverCaracteresEspecial(txtTelefone.Text);
            if (!string.Empty.Equals(retornoTelefone))
            {
                pPaciente.Telefone = Convert.ToInt64(retornoTelefone);
            }

            pPaciente.Date = (DateTime)dataNascimento.SelectedDate;
            pPaciente.Sexo = (bool)rb_feminino.IsChecked ? Sexo.FEMININO : Sexo.MASCULINO;
                        pPaciente.Cep = Convert.ToInt64(SiteUtil.RemoverCaracteresEspecial(txtCEP.Text));
            pPaciente.Logradouro = txt_Logradouro.Text;
            pPaciente.Numero = Convert.ToInt64(txtNumero.Text);
            pPaciente.Complemento = txtComplemento.Text;
            pPaciente.Estado = txt_Estado.Text;
            pPaciente.Cidade = txt_Cidade.Text;
            pPaciente.Bairro = txt_Bairro.Text;

        }

        private void TxtTelefone_LostFocus(object sender, EventArgs e)
        {
            if (!16.Equals(txtTelefone.Text.Length) && !11.Equals(txtTelefone.Text.Length))
            {
                txtTelefone.Text = string.Empty;
            }
            else
            {
                txtTelefone.Text = SiteUtil.FormatarTelefone(txtTelefone.Text);
            }
        }



    }
}
