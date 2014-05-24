using CompreAqui.Auxiliar;
using CompreAqui.Modelos;
using CompreAqui.ViewModels;
using NotificationsExtensions.TileContent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
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
    public sealed partial class Produtos : Page
    {
        private Pesquisa _parametrosPesquisa;
        private List<ProdutoVM> _produtos;
        public Produtos()
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
            if (e.Parameter is Pesquisa)
                _parametrosPesquisa = e.Parameter as Pesquisa;
        }

        private async void ListaProdutos_Loaded(object sender, RoutedEventArgs e)
        {
            if (Loja.Dados.Produtos == null)
                await Loja.Dados.CarregarDadosAsync();

            ListView lista = sender as ListView;
            _produtos = (from produto in Loja.Dados.Produtos
                         select new ProdutoVM
                         {
                             Id = produto.Id,
                             Descricao = produto.Descricao,
                             Preco = produto.Preco,
                             PrecoPromocao = produto.PrecoPromocao,
                             AvaliacaoMedia = produto.AvaliacaoMedia,
                             CategoriaId = produto.Categoria.Id,
                             CategoriaDescricao = produto.Categoria.Descricao,
                             Icone = produto.Icone
                         }).ToList();


            if (_parametrosPesquisa != null)
            {
                if (_parametrosPesquisa.CategoriaId != 0)
                {
                    _produtos = _produtos.Where(produto => produto.CategoriaId == _parametrosPesquisa.CategoriaId).ToList();
                    Titulo.Text = _produtos.FirstOrDefault().CategoriaDescricao;
                }
                else if (!string.IsNullOrEmpty(_parametrosPesquisa.DescricaoProduto))
                {
                    _produtos = _produtos.Where(produto =>
                                            produto.Descricao.ToLower()
                                                    .Contains(_parametrosPesquisa.DescricaoProduto.ToLower()))
                                                    .ToList();

                    Titulo.Text = "resultados";
                }
            }

            lista.ItemsSource = _produtos;
            if (_produtos.Count == 0)
            {
                CampoMensagem.Visibility = Visibility.Visible;
            }

            pnlCarregando.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void Fixar_Click(object sender, RoutedEventArgs e)
        {
            CriarTileSecundaria();
        }

        public async void CriarTileSecundaria()
        {
            SecondaryTile tileSecundaria = new SecondaryTile("pesquisa");

            tileSecundaria.DisplayName = string.Concat("Pesquisa: ", Titulo.Text);
            tileSecundaria.Arguments = string.Concat("Produtos:", _parametrosPesquisa.CategoriaId, ":", _parametrosPesquisa.DescricaoProduto);
            tileSecundaria.VisualElements.ShowNameOnSquare150x150Logo = true;
            tileSecundaria.VisualElements.Square150x150Logo = new Uri("ms-appx:///Assets/Logo.scale-240.png");

            if (SecondaryTile.Exists(tileSecundaria.TileId))
            {
                await tileSecundaria.UpdateAsync();
            }
            else
            {
                await tileSecundaria.RequestCreateAsync();
            }
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Grid componentePressionado = sender as Grid;
            if (componentePressionado != null)
            {
                int id = Convert.ToInt32(componentePressionado.Tag);

                this.Frame.Navigate(typeof(ProdutoDetalhe), id);
            }
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.ApplicationDataContainer configuracoes =
                            Windows.Storage.ApplicationData.Current.LocalSettings;

            if (configuracoes.Values.ContainsKey("usuarioId") &&
                Convert.ToInt32(configuracoes.Values["usuarioId"]) != 0)
                this.Frame.Navigate(typeof(ProdutosHub));
            else
                this.Frame.Navigate(typeof(MainPage));
            this.Frame.BackStack.Clear();
        }

    }
}
