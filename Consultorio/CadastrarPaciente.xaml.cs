using Biblioteca.ClassesBasicas;
using Biblioteca.Negocio;
using Consultorio.ServiceReference1;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Owner.IsEnabled = false;
            this.Closing += CadastrarPaciente_Closing;
        }

        private void CadastrarPaciente_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Owner.IsEnabled = true;
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


        private void Bt_Confirmar_Click(object sender, EventArgs e)
        {
            try
            {
                ValidarInclusao();
                MessageBoxResult Resultado = MessageBox.Show("Deseja confirmar o cadastro?", "Mensagem", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (MessageBoxResult.Yes.Equals(Resultado))
                {
                    CadastroPaciente();
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Mensagem", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CadastroPaciente()
        {
            try
            {
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
            bool retorno = true;

            SiteUtil.ValidacaoTextBoxReset(txtNomePaciente);
            if (string.Empty.Equals(txtNomePaciente.Text.Trim()))
            {
                SiteUtil.ValidacaoTextBox(txtNomePaciente);
                retorno = false;
            }

            SiteUtil.ValidacaoTextBoxReset(txtCpf);
            if (string.Empty.Equals(txtCpf.Text))
            {
                SiteUtil.ValidacaoTextBox(txtCpf);
                retorno = false;
            }

            SiteUtil.ValidacaoTextBoxReset(txtTelefone);
            if (string.Empty.Equals(txtTelefone.Text))
            {
                SiteUtil.ValidacaoTextBox(txtTelefone);
                retorno = false;
            }

            dataNascimento.BorderBrush = SiteUtil.BorderBrushPadrao();
            if (dataNascimento.SelectedDate.Equals(null) || SiteUtil.FormatarData(DateTime.Now).Equals(SiteUtil.FormatarData((DateTime)dataNascimento.SelectedDate)))
            {
                dataNascimento.BorderBrush = Brushes.Red;
                retorno = false;
            }

            if (!retorno)
            {
                throw new Exception(SiteUtil.CAMPOOBRIGATORIO);
            }

            SiteUtil.ValidacaoTextBoxReset(txtCpf);
            if (!string.Empty.Equals(txtCpf.Text) && !SiteUtil.IsValidCPF(txtCpf.Text))
            {
                SiteUtil.ValidacaoTextBox(txtCpf);
                throw new Exception("Favor informa um CPF valido!");
            }

            SiteUtil.ValidacaoTextBoxReset(txtNumero);
            if (!string.Empty.Equals(txt_Logradouro.Text) && string.Empty.Equals(txtNumero.Text))
            {
                SiteUtil.ValidacaoTextBox(txtNumero);
                throw new Exception("Favor informar um número para o endereço");
            }
            
            if (string.Empty.Equals(txt_Logradouro.Text))
            {
                LimparEndereco();
                txtNumero.Text = string.Empty;
                txtComplemento.Text = string.Empty;
            }
        }

        private void PreencherInclusao()
        {
            pPaciente = new Paciente
            {
                Nome = txtNomePaciente.Text,
                Date = (DateTime)dataNascimento.SelectedDate,
                Sexo = (bool)rb_feminino.IsChecked ? Sexo.FEMININO : Sexo.MASCULINO,
                Logradouro = txt_Logradouro.Text,
                Numero = SiteUtil.ConverterStringParaLong(txtNumero.Text),
                Complemento = txtComplemento.Text,
                Estado = txt_Estado.Text,
                Cidade = txt_Cidade.Text,
                Bairro = txt_Bairro.Text
            };

            string retornoCPF = SiteUtil.RemoverCaracteresEspecial(txtCpf.Text);
            pPaciente.Cpf = SiteUtil.ConverterStringParaLong(retornoCPF);

            string retornoTelefone = SiteUtil.RemoverCaracteresEspecial(txtTelefone.Text);
            pPaciente.Telefone = SiteUtil.ConverterStringParaLong(retornoTelefone);

            string retornoCEP = SiteUtil.RemoverCaracteresEspecial(txtCEP.Text);
            pPaciente.Cep = SiteUtil.ConverterStringParaLong(retornoCEP);

        }

        private void Tb_CPF_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            SiteUtil.FormatarCPFPreviewTextInput((TextBox)sender, e);
        }


        private void Tb_CPF_LostFocus(object sender, RoutedEventArgs e)
        {
            SiteUtil.FormatarCPFLostFocus((TextBox)sender);
        }

        private void TxtCEP_LostFocus(object sender, EventArgs e)
        {
            ConsultarEndereco();
        }


        private void TxtCEP_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ConsultarEndereco();
            }
        }

        private void TxtCEP_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            SiteUtil.TxtCEPPreviewTextInput((TextBox)sender, e);
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

        private void TxtTelefone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            SiteUtil.FormatarCampoTelefonePreviewTextInput((TextBox)sender, e);
        }

        private void TxtNumero_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            SiteUtil.CampoNumericoPreviewTextInput(e);
        }
    }
}
