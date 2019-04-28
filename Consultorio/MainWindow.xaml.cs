using Biblioteca.Negocio;
using Biblioteca.ViewModel;
using Consultorio.ServiceReference1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Consultorio
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        public ConsultaFiltro filtro;

        CollectionViewSource view = new CollectionViewSource();
        ObservableCollection<ConsultaViewModel> GridAtendimentoSource = new ObservableCollection<ConsultaViewModel>();
        int currentPageIndex = 0;
        int itemPerPage = 20;
        int totalPage = 0;
        
        public MainWindow()
        {
            InitializeComponent();
            dataInicio.DisplayDateEnd = DateTime.Now;
            dataFim.DisplayDateEnd = DateTime.Now;
            //ConsultarPacientes();
            Limpar();
        }

        private void Bt_pesquisar_Click(object sender, EventArgs e)
        {
            try
            {
                ValidarConsulta();
                PreencherFiltro();
                List<Consulta> resultado = new Service1Client().Consultar(filtro).ToList();
                gridAtendimento.ItemsSource = null;
                if (resultado.Any())
                {
                    CreateDataGridConsulta(resultado);
                    CarregarTreeView(resultado);
                }
                else
                {
                    throw new Exception("Não existem dados para essa pesquisa.");
                }

            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Mensagem", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CreateDataGridConsulta(List<Consulta> resultado)
        {
            GridAtendimentoSource = new ObservableCollection<ConsultaViewModel>();
            foreach (Consulta item in resultado)
            {
                ConsultaViewModel itemViewModel = new ConsultaViewModel
                {
                    DataConsulta = SiteUtil.formatarDataHora(item.DataConsulta),
                    NomePaciente = item.Paciente.Nome,
                    Cpf = SiteUtil.formatarCPF(item.Paciente.Cpf),
                    Telefone = SiteUtil.formatarTelefone(item.Paciente.Telefone),
                    DataNascimento = SiteUtil.formatarData(item.Paciente.Date),
                    Tratamento = item.Tratamento.Nome,
                    Situacao = item.Situacao.Descricao
                };

                GridAtendimentoSource.Add(itemViewModel);
            }
            int itemcount = resultado.Count;
            if (itemcount >= itemPerPage)
            {
                botoesPaginacaoAtendimento.Visibility = Visibility;
            }

            totalPage = itemcount / itemPerPage;
            if (itemcount % itemPerPage != 0)
            {
                totalPage += 1;
            }

            view.Source = GridAtendimentoSource;
            view.Filter += new FilterEventHandler(View_Filter);
           // this.gridAtendimento.DataContext = view;
            gridAtendimento.ItemsSource = view.View;
            ShowCurrentPageIndex();
            this.lb_TotalPaginas.Content = totalPage.ToString();
            gridAtendimento.Visibility = Visibility;
        }

        private void ShowCurrentPageIndex()
        {
            this.lb_PaginaAtual.Content = (currentPageIndex + 1).ToString();
        }
   
        void View_Filter(object sender, FilterEventArgs e)
        {
            int index = GridAtendimentoSource.IndexOf((ConsultaViewModel)e.Item);
            if (index >= itemPerPage * currentPageIndex && index < itemPerPage * (currentPageIndex + 1))
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }

        private void BtnFirst_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageIndex != 0)
            {
                currentPageIndex = 0;
                view.View.Refresh();
            }
            ShowCurrentPageIndex();
        }

        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageIndex > 0)
            {
                currentPageIndex--;
                view.View.Refresh();
            }
            ShowCurrentPageIndex();
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageIndex < totalPage - 1)
            {
                currentPageIndex++;
                view.View.Refresh();
            }
            ShowCurrentPageIndex();
        }

        private void BtnLast_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageIndex != totalPage - 1)
            {
                currentPageIndex = totalPage - 1;
                view.View.Refresh();
            }
            ShowCurrentPageIndex();
        }


        private void ValidarConsulta()
        {
            if (DateTime.Compare(dataInicio.DisplayDate, dataFim.DisplayDate) > 0)
            {
                throw new Exception("'Data' inicial maior que a 'Data' final");
            }

            if (tb_Nome.Text.Length > 0 && tb_Nome.Text.Length < 3)
            {
                throw new Exception("Você deve informar ao menos 3 caracteres para pesquisar por nome do paciente.");
            }

            if (!string.Empty.Equals(tb_CPF.Text) && !SiteUtil.isValidCPF(tb_CPF.Text))
            {
                throw new Exception("CPF invalido.");
            }
        }

        private void PreencherFiltro()
        {
            filtro = new ConsultaFiltro
            {
                DataInicio = (DateTime)dataInicio.SelectedDate,
                DataFim = (DateTime)dataFim.SelectedDate,
                NomePaciente = tb_Nome.Text
            };

            String retorno = SiteUtil.removerCaracteresEspecial(tb_CPF.Text);
            if (!string.Empty.Equals(retorno))
            {
                filtro.Cpf = Convert.ToInt64(retorno);
            }
        }

        private void Bt_limpar_Click(object sender, EventArgs e)
        {
            Limpar();
        }

        private void Limpar()
        {
            filtro = new ConsultaFiltro();
            gridAtendimento.ItemsSource = null;
            treeViewConsultaSimplificada.Items.Clear();
            tb_Nome.Text = string.Empty;
            tb_CPF.Text = string.Empty;
            DateTime dataAtual = DateTime.Now;
            dataInicio.SelectedDate = dataAtual;
            dataFim.SelectedDate = dataAtual;
            gridAtendimento.Visibility = Visibility.Hidden;
            botoesPaginacaoAtendimento.Visibility = Visibility.Hidden;
        }

        private void CarregarTreeView(List<Consulta> resultado)
        {
            treeViewConsultaSimplificada.Items.Clear();

            String data = string.Empty;
            TreeView rootView = treeViewConsultaSimplificada;
            TreeViewItem rootNode = null;
            TreeViewItem rootItem = null;
            foreach (Consulta item in resultado)
            {
                if (data.Equals(SiteUtil.formatarData(item.DataConsulta)))
                {
                    PreencherItemNodeTreeView(rootNode, rootItem, item);
                }
                else
                {
                    data = SiteUtil.formatarData(item.DataConsulta);
                    rootNode = new TreeViewItem
                    {
                        Header = data
                    };
                    rootView.Items.Add(rootNode);

                    PreencherItemNodeTreeView(rootNode, rootItem, item);
                }
            }
        }

        private void PreencherItemNodeTreeView(TreeViewItem rootNode, TreeViewItem rootItem, Consulta item)
        {
            rootItem = new TreeViewItem
            {
                Header = SiteUtil.formatarHora(item.DataConsulta) + " - " + item.Paciente.Nome
            };
            rootNode.Items.Add(rootItem);
        }

        //private void Bt_Cadastrar_Paciente_Click(object sender, EventArgs e)
        //{
        //    CadastroPaciente formCadastroPaciente = new CadastroPaciente
        //    {
        //        Owner = this
        //    };
        //    formCadastroPaciente.Show();
        //}

        //private void Txt_CPF_Leave(object sender, EventArgs e)
        //{
        //    FormatarCPF_Leave(tb_CPF);
        //}


        // PACIENTE
        //private void Txt_Nome_Paciente_Search_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Enter)
        //    {
        //        ConsultarPacientePorParametro();
        //    }
        //}

        //private void TxtCPF_Paciente_Search_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Enter)
        //    {
        //        ConsultarPacientePorParametro();
        //    }
        //}

        //private void TxtCPF_Paciente_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    FormatarCPF_KeyPress(txtCPF_Paciente_Search, e);
        //}

        //private void TxtCPF_Paciente_Leave(object sender, EventArgs e)
        //{
        //    FormatarCPF_Leave(txtCPF_Paciente_Search);
        //}

        //private void ConsultarPacientePorParametro()
        //{
        //    Paciente pFiltro = new Paciente
        //    {
        //        Nome = txt_Nome_Paciente_Search.Text
        //    };
        //    String retorno = SiteUtil.removerCaracteresEspecial(txtCPF_Paciente_Search.Text);
        //    if (!retorno.Equals(""))
        //    {
        //        pFiltro.Cpf = Convert.ToInt64(retorno);
        //    }
        //    MontarGrid(new Service1Client().ConsultarPaciente(pFiltro).ToList());
        //}

        //public void ConsultarPacientes()
        //{
        //    MontarGrid(new Service1Client().ConsultarPaciente(new Paciente()).ToList());
        //}

        //private void MontarGrid(List<Paciente> resultado)
        //{
        //    materialListView1.Items.Clear();
        //    foreach (Paciente item2 in resultado)
        //    {
        //        ListViewItem itListView = materialListView1.Items.Add("");
        //        itListView.SubItems.Add(item2.Nome);
        //        itListView.SubItems.Add(SiteUtil.formatarCPF(item2.Cpf));
        //        itListView.SubItems.Add(SiteUtil.formatarTelefone(item2.Telefone));
        //        itListView.SubItems.Add(SiteUtil.obterDescricaoEnum(item2.Sexo));
        //        itListView.SubItems.Add(SiteUtil.formatarData(item2.Date));
        //        itListView.SubItems.Add(SiteUtil.formatarCEP(item2.Cep));
        //        itListView.SubItems.Add(item2.Logradouro);
        //        if (!0L.Equals(item2.Numero))
        //        {
        //            itListView.SubItems.Add(Convert.ToString(item2.Numero));
        //        }
        //        else
        //        {
        //            itListView.SubItems.Add("");
        //        }
        //        itListView.SubItems.Add(item2.Complemento);
        //        itListView.SubItems.Add(item2.Estado);
        //        itListView.SubItems.Add(item2.Cidade);
        //        itListView.SubItems.Add(item2.Bairro);
        //        itListView.SubItems.Add("");
        //    }
        //}

        //private void materialListView1_DoubleClick(object sender, EventArgs e)
        //{
        //    if (materialListView1.SelectedItems.Count == 1)
        //    {
        //        Paciente pFiltro = new Paciente();
        //        String retorno = SiteUtil.removerCaracteresEspecial(materialListView1.SelectedItems[0].SubItems[2].Text);
        //        if (!retorno.Equals(""))
        //        {
        //            pFiltro.Cpf = Convert.ToInt64(retorno);
        //        }
        //        Paciente resultado = new Service1Client().ConsultarPaciente(pFiltro).ToList()[0];
        //        DetalhePaciente formDetalhePaciente = new DetalhePaciente(resultado)
        //        {
        //            Owner = this
        //        };
        //        formDetalhePaciente.Show();
        //    }
        //}

        //// COMUM
        //private void Timer1_Tick(object sender, EventArgs e)
        //{
        //    date_time = DateTime.Now;
        //    lb_date_time.Text = "Data : " + date_time.ToLongDateString() + "  Hora: " + date_time.ToLongTimeString();
        //}

        public void Bt_Sobre_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Consutorio - Jéssyka ● Lucena\nVersão: 1.1.0\n\nDesenvolvedor: Fábio Lucena Ribas\nLinkedIn: fabiolucenaribas", "Sobre", MessageBoxButton.OK, MessageBoxImage.Information);
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
                        campoCPF.Text = campoCPF.Text + ".";
                        campoCPF.SelectionStart = 5;
                        break;
                    case 7:
                        campoCPF.Text = campoCPF.Text + ".";
                        campoCPF.SelectionStart = 9;
                        break;
                    case 11:
                        campoCPF.Text = campoCPF.Text + "-";
                        campoCPF.SelectionStart = 13;
                        break;
                }
            }
        }

        private void Tb_CPF_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            FormatarCPF_KeyPress((TextBox)sender, e);
        }


        private void Tb_CPF_LostFocus(object sender, RoutedEventArgs e)
        {
            FormatarCPF_Leave((TextBox)sender);
        }

        private void FormatarCPF_Leave(TextBox campoCPF)
        {
            if (!14.Equals(campoCPF.Text.Length) && !11.Equals(campoCPF.Text.Length) && !10.Equals(campoCPF.Text.Length))
            {
                campoCPF.Text = string.Empty;
            }
            else
            {
                campoCPF.Text = SiteUtil.formatarCPF(Convert .ToString(campoCPF.Text));
           }
        }

        private void Bt_Prev_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
