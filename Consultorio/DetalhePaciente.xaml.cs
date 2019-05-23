using Biblioteca.ClassesBasicas;
using Biblioteca.Negocio;
using Biblioteca.ViewModel;
using Consultorio.ServiceReference1;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Consultorio
{
    /// <summary>
    /// Lógica interna para DetalhePaciente.xaml
    /// </summary>
    public partial class DetalhePaciente : Window
    {
        private Paciente Paciente;
        private HistoricoPaciente HistoricoSelecionado;
        private List<HistoricoPaciente> HistoricoPacientes;
        private string DataConsulta = string.Empty;

        public DetalhePaciente(Paciente pPaciente)
        {
            InitializeComponent();
            Paciente = pPaciente;
        }

        public DetalhePaciente(ConsultaViewModel ConsultaViewModel)
        {
            InitializeComponent();
            Paciente pFiltro = new Paciente();
            String retorno = SiteUtil.RemoverCaracteresEspecial(ConsultaViewModel.Cpf);
            if (!string.Empty.Equals(retorno))
            {
                pFiltro.Cpf = Convert.ToInt64(retorno);
            }
            Paciente = new Service1Client().ConsultarPaciente(pFiltro).FirstOrDefault();
            DateTime data = Convert.ToDateTime(ConsultaViewModel.DataConsulta);
            DataConsulta = SiteUtil.FormatarData(data);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Owner.IsEnabled = false;
            this.Closing += DetalhePaciente_Closing;
            CarregarComboboxHistorico();
            CarregarInformacoesPaciente();
        }

        private void DetalhePaciente_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Owner.IsEnabled = true;
        }

        // ALTERAR CADASTRO
            private void CarregarInformacoesPaciente()
        {
            txtNomePaciente.Text = Paciente.Nome;
            txtCpf.Text = SiteUtil.FormatarCPF(Paciente.Cpf);
            txtTelefone.Text = SiteUtil.FormatarTelefone(Paciente.Telefone);
            dataNascimento.SelectedDate = Paciente.Date;
            if (Sexo.FEMININO.Equals(Paciente.Sexo))
            {
                rb_feminino.IsChecked = true;
                rb_masculino.IsChecked = false;
            }
            txtCEP.Text = SiteUtil.FormatarCEP(Paciente.Cep);
            txt_Logradouro.Text = Paciente.Logradouro;
            txtComplemento.Text = Paciente.Complemento;
            txt_Estado.Text = Paciente.Estado;
            txt_Cidade.Text = Paciente.Cidade;
            txt_Bairro.Text = Paciente.Bairro;
            if (!0L.Equals(Paciente.Numero))
            {
                txtNumero.Text = Convert.ToString(Paciente.Numero);
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
                new Service1Client().AtualizarPaciente(Paciente);

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
            Paciente = new Paciente
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
            Paciente.Cpf = SiteUtil.ConverterStringParaLong(retornoCPF);

            string retornoTelefone = SiteUtil.RemoverCaracteresEspecial(txtTelefone.Text);
            Paciente.Telefone = SiteUtil.ConverterStringParaLong(retornoTelefone);

            string retornoCEP = SiteUtil.RemoverCaracteresEspecial(txtCEP.Text);
            Paciente.Cep = SiteUtil.ConverterStringParaLong(retornoCEP);

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

        //HISTORICO

        private void CarregarComboboxHistorico()
        {
            HistoricoPacientes = new Service1Client().ConsultarHistorico(Paciente).ToList();
            int dataSelecionada = 0;
            foreach (HistoricoPaciente item in HistoricoPacientes)
            {
                string dataFormatada = SiteUtil.FormatarData(item.DataConsulta);
                dataConsultaHistorico.Items.Add(new KeyValuePair<HistoricoPaciente, string>(item, dataFormatada));
                if (DataConsulta.Equals(dataFormatada))
                {
                    dataSelecionada = dataConsultaHistorico.Items.Count - 1;
                }

            }

            dataConsultaHistorico.SelectedValuePath = "Key";
            dataConsultaHistorico.DisplayMemberPath = "Value";

            if (dataSelecionada > 0)
            {
                dataConsultaHistorico.SelectedIndex = dataSelecionada;
            }else if((int)dataConsultaHistorico.SelectedIndex < 0)
            {
                dataConsultaHistorico.SelectedIndex = 0;
            }
        }

        public void CarregarHistorico()
        {
            TxtHistorico.Text = string.Empty;
            if (null != dataConsultaHistorico.SelectedItem)
            {
                HistoricoSelecionado = ((KeyValuePair<HistoricoPaciente, string>)dataConsultaHistorico.SelectedItem).Key;
                TxtHistorico.Text = HistoricoSelecionado.DescricaoHistorico;
                TxtHistorico.IsEnabled = true;
            }
            else
            {
                TxtHistorico.IsEnabled = false;
            }
        }

        private void DataConsultaHistorico_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CarregarHistorico();
        }

        private void Chk_Alterar_Click(object sender, RoutedEventArgs e)
        {
            if (null == dataConsultaHistorico.SelectedItem)
            {
                MessageBoxResult result = MessageBox.Show("Favor selecionar uma data", "Mensagem", MessageBoxButton.OK, MessageBoxImage.Information);
                if (MessageBoxResult.OK.Equals(result))
                {
                    chk_Alterar.IsChecked = false;
                }
            }
            else
            {
                CarregarHistorico();
            }
            atualizarComponentes();
        }

        private void Bt_AlterarHistorio_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult Resultado = MessageBox.Show("Deseja confirmar a alteração?", "Mensagem", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (MessageBoxResult.Yes.Equals(Resultado))
                {
                    HistoricoSelecionado.DescricaoHistorico = TxtHistorico.Text;
                    new Service1Client().AtualizarHistorico(HistoricoSelecionado);
                    chk_Alterar.IsChecked = false;
                    atualizarComponentes();
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Mensagem", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void atualizarComponentes()
        {
            TxtHistorico.IsReadOnly = !(bool)chk_Alterar.IsChecked;
            bt_GerarPDF.IsEnabled = !(bool)chk_Alterar.IsChecked;
        }

        private void Bt_GerarPDF_Click(object sender, RoutedEventArgs e)
        {

            if (null != dataConsultaHistorico.SelectedItem)
            {
                Document doc = new Document(PageSize.A4);//criando e estipulando o tipo da folha usada
                doc.SetMargins(40, 40, 40, 80);//estibulando o espaçamento das margens que queremos
                doc.AddCreationDate();//adicionando as configuracoes

                //caminho onde sera criado o pdf + nome desejado
                //OBS: o nome sempre deve ser terminado com .pdf
                string caminho = @"C: \Users\FabioLucenaRibas\Desktop\teste.pdf";

                //criando o arquivo pdf embranco, passando como parametro a variavel doc criada acima e a variavel caminho 
                //tambem criada acima.
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(caminho, FileMode.Create));

                doc.Open();

                string dados = string.Empty;
                Paragraph paragrafo = new Paragraph(dados, new Font(Font.NORMAL, 12));
                paragrafo.Alignment = Element.ALIGN_LEFT;
                paragrafo.Add(SiteUtil.FormatarData(HistoricoSelecionado.DataConsulta));
                doc.Add(paragrafo);

                Paragraph paragrafo1 = new Paragraph(dados, new Font(Font.NORMAL, 12));
                paragrafo1.Alignment = Element.ALIGN_LEFT;
                paragrafo1.Add(HistoricoSelecionado.DescricaoHistorico);
                doc.Add(paragrafo1);
                //acidionado paragrafo ao documento

                //fechando documento para que seja salva as alteraçoes.
                doc.Close();


            }
            else
            {
                foreach (HistoricoPaciente item in HistoricoPacientes)
                {
                }
            }
            Process.Start(@"C:\Users\FabioLucenaRibas\Desktop\teste.pdf");
        }

    }
}
