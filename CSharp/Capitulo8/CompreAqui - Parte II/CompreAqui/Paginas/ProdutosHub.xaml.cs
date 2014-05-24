using CompreAqui.Auxiliar;
using CompreAqui.Common;
using CompreAqui.Modelos;
using CompreAqui.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace CompreAqui.Paginas
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProdutosHub : Page
    {
        private ListView listaCategorias;
        private ListView listaProdutos;
        private ListView listaPromocoes;

        public ProdutosHub()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }
        private void btnSuaConta_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SuaConta));
        }

        public async void VincularDados()
        {
            listaCategorias.ItemsSource = (from produtos in Loja.Dados.Produtos
                                           select new CategoriaVM
                                           {
                                               Id = produtos.Categoria.Id,
                                               Descricao = produtos.Categoria.Descricao
                                           }).Distinct().ToList();

            listaPromocoes.ItemsSource = (from produtos in Loja.Dados.Produtos
                                          where produtos.PrecoPromocao != 0
                                          select new ProdutoVM
                                          {
                                              Id = produtos.Id,
                                              Descricao = produtos.Descricao,
                                              Preco = produtos.Preco,
                                              PrecoPromocao = produtos.PrecoPromocao,
                                              Icone = produtos.Icone
                                          })
                                 .OrderByDescending(produto => produto.Desconto)
                                 .ToList();

            listaProdutos.ItemsSource = (from produtos in Loja.Dados.Produtos
                                         where produtos.Id == 3 || produtos.Id == 4
                                         select new ProdutoVM
                                         {
                                             Id = produtos.Id,
                                             Descricao = produtos.Descricao,
                                             Icone = produtos.Icone,
                                             Preco = produtos.Preco,
                                             PrecoPromocao = produtos.PrecoPromocao
                                         })
                                 .ToList();

        }

        public async void CarregarDadosAsync()
        {
            if (listaCategorias != null && listaPromocoes != null && listaProdutos != null)
            {
                if (Loja.Dados.Produtos == null)
                {
                    await Loja.Dados.CarregarDadosAsync();
                    VincularDados();
                }
                else
                {
                    VincularDados();
                }

                pnlCarregando.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private void ListaCategoria_Loaded(object sender, RoutedEventArgs e)
        {
            listaCategorias = sender as ListView;
            CarregarDadosAsync();
        }

        private void ListaPromocoes_Loaded(object sender, RoutedEventArgs e)
        {
            listaPromocoes = sender as ListView;
            CarregarDadosAsync();
        }

        private void ListaProdutos_Loaded(object sender, RoutedEventArgs e)
        {
            listaProdutos = sender as ListView;

            CarregarDadosAsync();
        }

        private void TodosProdutos_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Produtos));
        }

        private void Categoria_Tapped(object sender, TappedRoutedEventArgs e)
        {
            TextBlock categoriaClicada = sender as TextBlock;

            Pesquisa pesquisa = new Pesquisa();
            pesquisa.CategoriaId = Convert.ToInt32(categoriaClicada.Tag);

            this.Frame.Navigate(typeof(Produtos), pesquisa);
        }

        private void Produto_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Grid componentePressionado = sender as Grid;
            if (componentePressionado != null)
            {
                int id = Convert.ToInt32(componentePressionado.Tag);

                this.Frame.Navigate(typeof(ProdutoDetalhe), id);
            }
        }

        private void CampoPesquisa_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                ExecutarPesquisa();
            }
        }

        private void ExecutarPesquisa()
        {
            Pesquisa pesquisa = new Pesquisa();
            pesquisa.DescricaoProduto = CampoPesquisa.Text;

            this.Frame.Navigate(typeof(Produtos), pesquisa);
            DesaparecerPainelPesquisa();
        }

        private void CampoPesquisa_LostFocus(object sender, RoutedEventArgs e)
        {
            if (btnAppBar.FocusState == Windows.UI.Xaml.FocusState.Unfocused)
                DesaparecerPainelPesquisa();
        }

        private void DesaparecerPainelPesquisa()
        {
            CampoPesquisa.Text = string.Empty;
            PainelPesquisa.Visibility = Visibility.Collapsed;
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (PainelPesquisa.Visibility == Visibility.Collapsed)
            {
                PainelPesquisa.Visibility = Visibility.Visible;
                CampoPesquisa.Focus(Windows.UI.Xaml.FocusState.Programmatic);
            }
            else
            {
                ExecutarPesquisa();
            }
        }


    }
}
