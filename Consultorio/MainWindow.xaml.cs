using Biblioteca.ClassesBasicas;
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
        private ConsultaFiltro filtro;

        //# INICIO - CONSULTA
        private readonly CollectionViewSource AtendimentoViewSource = new CollectionViewSource();
        private ObservableCollection<ConsultaViewModel> GridAtendimentoCollection = new ObservableCollection<ConsultaViewModel>();
        private int AtendimentoCurrentPageIndex = 0;
        private readonly int AtendimentoItemPerPage = 20;
        private int AtendimentoTotalPage = 0;
        //# FIM - CONSULTA

        //# INICIO - PACIENTES
        private readonly CollectionViewSource PacientesViewSource = new CollectionViewSource();
        private ObservableCollection<PacientesViewModel> GridPacientesCollection = new ObservableCollection<PacientesViewModel>();
        private int PacientesCurrentPageIndex = 0;
        private readonly int PacientesItemPerPage = 20;
        private int PacientesTotalPage = 0;
        //# FIM - PACIENTES

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataInicio.DisplayDateEnd = DateTime.Now;
            dataFim.DisplayDateEnd = DateTime.Now;
            ConsultarPacientes();
            Limpar();
        }

        //# INICIO - CONSULTA
        private void Bt_pesquisar_Click(object sender, EventArgs e)
        {
            try
            {
                LimparResultado();
                ValidarConsulta();
                PreencherFiltro();
                List<Consulta> resultado = new Service1Client().Consultar(filtro).ToList();
                gridAtendimento.ItemsSource = null;
                if (resultado.Any())
                {
                    DataGridConsulta(resultado);
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

        private void DataGridConsulta(List<Consulta> resultado)
        {
            GridAtendimentoCollection = new ObservableCollection<ConsultaViewModel>();
            foreach (Consulta item in resultado)
            {
                ConsultaViewModel itemViewModel = new ConsultaViewModel
                {
                    DataConsulta = SiteUtil.FormatarDataHora(item.DataConsulta),
                    NomePaciente = item.Paciente.Nome,
                    Cpf = SiteUtil.FormatarCPF(item.Paciente.Cpf),
                    Telefone = SiteUtil.FormatarTelefone(item.Paciente.Telefone),
                    DataNascimento = SiteUtil.FormatarData(item.Paciente.Date),
                    Tratamento = item.Tratamento.Nome,
                    Situacao = item.Situacao.Descricao
                };

                GridAtendimentoCollection.Add(itemViewModel);
            }
            int itemcount = resultado.Count;
            AtendimentoCurrentPageIndex = 0;
            AtendimentoTotalPage = itemcount / AtendimentoItemPerPage;
            if (itemcount % AtendimentoItemPerPage != 0)
            {
                AtendimentoTotalPage += 1;
            }

            AtendimentoViewSource.Source = GridAtendimentoCollection;
            AtendimentoViewSource.Filter += new FilterEventHandler(View_Filter);
            // this.gridAtendimento.DataContext = ViewSource;
            gridAtendimento.ItemsSource = AtendimentoViewSource.View;
            ShowCurrentPageIndex();
            this.lb_TotalPaginas.Content = AtendimentoTotalPage.ToString();
            gridAtendimento.Visibility = Visibility;
            treeViewConsultaSimplificada.Visibility = Visibility;
            botoesPaginacaoAtendimento.Visibility = Visibility;
        }

        private void GridAtendimento_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (gridAtendimento.SelectedItems.Count == 1)
            {
                Paciente pFiltro = new Paciente();
                ConsultaViewModel value = (ConsultaViewModel)gridAtendimento.SelectedItem;
                String retorno = SiteUtil.RemoverCaracteresEspecial(value.Cpf);
                if (!string.Empty.Equals(retorno))
                {
                    pFiltro.Cpf = Convert.ToInt64(retorno);
                }
                Paciente resultado = new Service1Client().ConsultarPaciente(pFiltro).ToList()[0];
                DetalhePaciente formDetalhePaciente = new DetalhePaciente(resultado)
                {
                    Owner = this
                };
                formDetalhePaciente.Show();
            }
        }

        private void ShowCurrentPageIndex()
        {
            this.lb_PaginaAtual.Content = (AtendimentoCurrentPageIndex + 1).ToString();
        }

        void View_Filter(object sender, FilterEventArgs e)
        {
            int index = GridAtendimentoCollection.IndexOf((ConsultaViewModel)e.Item);
            e.Accepted = (index >= AtendimentoItemPerPage * AtendimentoCurrentPageIndex && index < AtendimentoItemPerPage * (AtendimentoCurrentPageIndex + 1));
        }

        private void BtnFirst_Click(object sender, RoutedEventArgs e)
        {
            if (AtendimentoCurrentPageIndex != 0)
            {
                AtendimentoCurrentPageIndex = 0;
                AtendimentoViewSource.View.Refresh();
            }
            ShowCurrentPageIndex();
        }

        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (AtendimentoCurrentPageIndex > 0)
            {
                AtendimentoCurrentPageIndex--;
                AtendimentoViewSource.View.Refresh();
            }
            ShowCurrentPageIndex();
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (AtendimentoCurrentPageIndex < AtendimentoTotalPage - 1)
            {
                AtendimentoCurrentPageIndex++;
                AtendimentoViewSource.View.Refresh();
            }
            ShowCurrentPageIndex();
        }

        private void BtnLast_Click(object sender, RoutedEventArgs e)
        {
            if (AtendimentoCurrentPageIndex != AtendimentoTotalPage - 1)
            {
                AtendimentoCurrentPageIndex = AtendimentoTotalPage - 1;
                AtendimentoViewSource.View.Refresh();
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

            if (!string.Empty.Equals(tb_CPF.Text) && !SiteUtil.IsValidCPF(tb_CPF.Text))
            {
                throw new Exception("Favor informa um CPF valido!");
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

            String retorno = SiteUtil.RemoverCaracteresEspecial(tb_CPF.Text);
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
            tb_Nome.Text = string.Empty;
            tb_CPF.Text = string.Empty;
            DateTime dataAtual = DateTime.Now;
            dataInicio.SelectedDate = dataAtual;
            dataFim.SelectedDate = dataAtual;
            LimparResultado();
        }

        private void LimparResultado()
        {
            gridAtendimento.ItemsSource = null;
            treeViewConsultaSimplificada.Items.Clear();
            treeViewConsultaSimplificada.Visibility = Visibility.Hidden;
            gridAtendimento.Visibility = Visibility.Hidden;
            botoesPaginacaoAtendimento.Visibility = Visibility.Hidden;
        }

        private void CarregarTreeView(List<Consulta> resultado)
        {
            treeViewConsultaSimplificada.Items.Clear();

            String data = string.Empty;
            TreeView rootView = treeViewConsultaSimplificada;
            TreeViewItem rootNode = null;
            foreach (Consulta item in resultado)
            {
                if (data.Equals(SiteUtil.FormatarData(item.DataConsulta)))
                {
                    PreencherItemNodeTreeView(rootNode, item);
                }
                else
                {
                    data = SiteUtil.FormatarData(item.DataConsulta);
                    rootNode = new TreeViewItem
                    {
                        Header = data
                    };
                    rootView.Items.Add(rootNode);

                    PreencherItemNodeTreeView(rootNode, item);
                }
            }
        }

        private void PreencherItemNodeTreeView(TreeViewItem rootNode, Consulta item)
        {
            TreeViewItem rootItem = new TreeViewItem
            {
                Header = SiteUtil.FormatarHora(item.DataConsulta) + " - " + item.Paciente.Nome
            };
            rootNode.Items.Add(rootItem);
        }

        //# FIM - CONSULTA

        //# INICIO - PACIENTES

        public void ConsultarPacientes()
        {
            DataGridPacientes(new Service1Client().ConsultarPaciente(new Paciente()).ToList());
        }

        private void ConsultarPacientePorParametro()
        {
            String retorno = SiteUtil.RemoverCaracteresEspecial(tb_Paciente_CPF.Text);
            Paciente pFiltro = new Paciente
            {
                Nome = tb_Paciente_Nome.Text,
                Cpf = !string.Empty.Equals(retorno) ? Convert.ToInt64(retorno) : 0L
            };
            DataGridPacientes(new Service1Client().ConsultarPaciente(pFiltro).ToList());
        }

        private void GridPacientes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (gridPacientes.SelectedItems.Count == 1)
            {
                Paciente pFiltro = new Paciente();
                PacientesViewModel value = (PacientesViewModel) gridPacientes.SelectedItem;
                String retorno = SiteUtil.RemoverCaracteresEspecial(value.Cpf);
                if (!string.Empty.Equals(retorno))
                {
                    pFiltro.Cpf = Convert.ToInt64(retorno);
                }
                Paciente resultado = new Service1Client().ConsultarPaciente(pFiltro).ToList()[0];
                DetalhePaciente formDetalhePaciente = new DetalhePaciente(resultado)
                {
                    Owner = this
                };
                formDetalhePaciente.Show();
            }
        }

        private void DataGridPacientes(List<Paciente> resultado)
        {
            GridPacientesCollection = new ObservableCollection<PacientesViewModel>();
            foreach (Paciente item in resultado)
            {
                PacientesViewModel itemViewModel = new PacientesViewModel
                {
                    NomeCompleto = item.Nome,
                    Cpf = SiteUtil.FormatarCPF(item.Cpf),
                    Telefone = SiteUtil.FormatarTelefone(item.Telefone),
                    Sexo = SiteUtil.ObterDescricaoEnum(item.Sexo),
                    DataNascimento = SiteUtil.FormatarData(item.Date),
                    Cep = SiteUtil.FormatarCEP(item.Cep),
                    Logradouro = item.Logradouro,
                    Numero = (!0L.Equals(item.Numero) ? Convert.ToString(item.Numero) : string.Empty),
                    Complemento = item.Complemento,
                    Estado = item.Estado,
                    Cidade = item.Cidade,
                    Bairro = item.Bairro
                };

                GridPacientesCollection.Add(itemViewModel);
            }
            int itemcount = resultado.Count;
            PacientesCurrentPageIndex = 0;
            PacientesTotalPage = itemcount / PacientesItemPerPage;
            if (itemcount % PacientesItemPerPage != 0)
            {
                PacientesTotalPage += 1;
            }

            PacientesViewSource.Source = GridPacientesCollection;
            PacientesViewSource.Filter += new FilterEventHandler(View_Filter_Pacientes);
            gridPacientes.ItemsSource = PacientesViewSource.View;
            ShowCurrentPageIndexPacientes();
            lb_TotalPaginasPacientes.Content = PacientesTotalPage.ToString();
            gridPacientes.Visibility = Visibility;
            botoesPaginacaoPacientes.Visibility = Visibility;
        }

        private void ShowCurrentPageIndexPacientes()
        {
            lb_PaginaAtualPacientes.Content = (PacientesCurrentPageIndex + 1).ToString();
        }

        void View_Filter_Pacientes(object sender, FilterEventArgs e)
        {
            int index = GridPacientesCollection.IndexOf((PacientesViewModel)e.Item);
            e.Accepted = (index >= PacientesItemPerPage * PacientesCurrentPageIndex && index < PacientesItemPerPage * (PacientesCurrentPageIndex + 1));
        }

        private void BtnFirstPacientes_Click(object sender, RoutedEventArgs e)
        {
            if (PacientesCurrentPageIndex != 0)
            {
                PacientesCurrentPageIndex = 0;
                PacientesViewSource.View.Refresh();
            }
            ShowCurrentPageIndexPacientes();
        }

        private void BtnPrevPacientes_Click(object sender, RoutedEventArgs e)
        {
            if (PacientesCurrentPageIndex > 0)
            {
                PacientesCurrentPageIndex--;
                PacientesViewSource.View.Refresh();
            }
            ShowCurrentPageIndexPacientes();
        }

        private void BtnNextPacientes_Click(object sender, RoutedEventArgs e)
        {
            if (PacientesCurrentPageIndex < PacientesTotalPage - 1)
            {
                PacientesCurrentPageIndex++;
                PacientesViewSource.View.Refresh();
            }
            ShowCurrentPageIndexPacientes();
        }

        private void BtnLastPacientes_Click(object sender, RoutedEventArgs e)
        {
            if (PacientesCurrentPageIndex != PacientesTotalPage - 1)
            {
                PacientesCurrentPageIndex = PacientesTotalPage - 1;
                PacientesViewSource.View.Refresh();
            }
            ShowCurrentPageIndexPacientes();
        }


        private void Bt_Cadastrar_Paciente_Click(object sender, EventArgs e)
        {
            CadastrarPaciente formCadastroPaciente = new CadastrarPaciente
            {
                Owner = this
            };
            formCadastroPaciente.Show();
        }

        private void TxtPaciente_Nome_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ConsultarPacientePorParametro();
            }
        }

        private void TxtPaciente_CPF_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ConsultarPacientePorParametro();
            }
        }

        private void Tb_Paciente_Nome_LostFocus(object sender, RoutedEventArgs e)
        {
            ConsultarPacientePorParametro();
        }

        private void Tb_Paciente_CPF_LostFocus(object sender, RoutedEventArgs e)
        {
            SiteUtil.FormatarCPFLostFocus((TextBox)sender);
            ConsultarPacientePorParametro();
        }

        //// COMUM
        public void Bt_Sobre_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Consutorio - Jéssyka ● Lucena\nVersão: 1.1.0\n\nDesenvolvedor: Fábio Lucena Ribas\nLinkedIn: fabiolucenaribas", "Sobre", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Tb_CPF_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            SiteUtil.FormatarCPFPreviewTextInput((TextBox)sender, e);
        }


        private void Tb_CPF_LostFocus(object sender, RoutedEventArgs e)
        {
            SiteUtil.FormatarCPFLostFocus((TextBox)sender);
        }
    }
}
