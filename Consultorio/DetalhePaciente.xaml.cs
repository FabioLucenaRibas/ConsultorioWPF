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
    /// Lógica interna para DetalhePaciente.xaml
    /// </summary>
    public partial class DetalhePaciente : Window
    {
        private Paciente paciente;

        public DetalhePaciente(Paciente pPaciente)
        {
            InitializeComponent();
            paciente = pPaciente;
            CarregarInformacoesPaciente();
        }

        private void CarregarInformacoesPaciente()
        {
            txtNomePaciente.Text = paciente.Nome;
            txtCpf.Text = SiteUtil.FormatarCPF(paciente.Cpf);
            txtTelefone.Text = SiteUtil.FormatarTelefone(paciente.Telefone);
            dataNascimento.SelectedDate = paciente.Date;
            if (Sexo.FEMININO.Equals(paciente.Sexo))
            {
                rb_feminino.IsChecked = true;
                rb_masculino.IsChecked = false;
            }
            txtCEP.Text = SiteUtil.FormatarCEP(paciente.Cep);
            txt_Logradouro.Text = paciente.Logradouro;
            txtComplemento.Text = paciente.Complemento;
            txt_Estado.Text = paciente.Estado;
            txt_Cidade.Text = paciente.Cidade;
            txt_Bairro.Text = paciente.Bairro;
            if (!0L.Equals(paciente.Numero))
            {
                txtNumero.Text = Convert.ToString(paciente.Numero);
            }
            else
            {
                txtNumero.Text = string.Empty;
            }
        }

        private void Bt_Confirmar_Alterar_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidarAlteracao())
                    {
                    MessageBoxResult Resultado = MessageBox.Show("Deseja confirmar a alteração?", "Mensagem", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (MessageBoxResult.Yes.Equals(Resultado))
                    {
                        Alterar();
                        if (this.Owner.GetType().Equals(typeof (MainWindow)))
                        {
                            ((MainWindow)this.Owner).ConsultarPacientes();
                        }
                    }
                }
                else
                {
                    throw new Exception("Campos obrigatórios não preenchidos!");
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Mensagem", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        private void Alterar()
        {
            try
            {
                PreencherPaciente();
                new Service1Client().AtualizarPaciente(paciente);

            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Mensagem", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private bool ValidarAlteracao()
        {
            bool retorno = true;

            SiteUtil.ValidacaoTextBoxReset(txtTelefone);
            if (string.Empty.Equals(txtTelefone.Text.Trim()))
            {
                SiteUtil.ValidacaoTextBox(txtTelefone);
                retorno = false;
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

            return retorno;
        }

        private void PreencherPaciente()
        {
            paciente = new Paciente
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
            paciente.Cpf = SiteUtil.ConverterStringParaLong(retornoCPF);

            string retornoTelefone = SiteUtil.RemoverCaracteresEspecial(txtTelefone.Text);
            paciente.Telefone = SiteUtil.ConverterStringParaLong(retornoTelefone);

            string retornoCEP = SiteUtil.RemoverCaracteresEspecial(txtCEP.Text);
            paciente.Cep = SiteUtil.ConverterStringParaLong(retornoCEP);

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
                    SiteUtil.ValidacaoTextBox(txtCEP, "CEP não encontrado");
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

        private void TxtCEP_LostFocus(object sender, EventArgs e)
        {
            SiteUtil.ValidacaoTextBoxReset(txtCEP);
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
                SiteUtil.ValidacaoTextBoxReset(txtTelefone);
                txtTelefone.Text = SiteUtil.FormatarTelefone(txtTelefone.Text);
            }
        }

        private void TxtTelefone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            SiteUtil.FormatarCampoTelefonePreviewTextInput((TextBox)sender,e);
        }

         private void TxtNumero_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            SiteUtil.CampoNumericoPreviewTextInput(e);
        }
    }
}
