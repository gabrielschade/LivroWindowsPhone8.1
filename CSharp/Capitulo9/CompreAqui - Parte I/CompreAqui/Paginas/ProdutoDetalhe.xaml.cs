using CompreAqui.Converter;
using CompreAqui.Modelos;
using CompreAqui.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
    public sealed partial class ProdutoDetalhe : Page
    {
        public ProdutoDetalhe()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            int produtoId = Convert.ToInt32(e.Parameter);
            ProdutoVM produto = null;

            if (produtoId != 0)
            {
                if (Loja.Dados.Produtos == null)
                    await Loja.Dados.CarregarDadosAsync();

                produto = (from produtos in Loja.Dados.Produtos
                           where produtos.Id == Convert.ToInt32(produtoId)
                           select new ProdutoVM
                           {
                               Id = produtos.Id,
                               AvaliacaoMedia = produtos.AvaliacaoMedia,
                               CategoriaDescricao = produtos.Categoria.Descricao,
                               Descricao = produtos.Descricao,
                               DescricaoDetalhada = produtos.DescricaoDetalhada,
                               Icone = produtos.Icone,
                               Preco = produtos.Preco,
                               PrecoPromocao = produtos.PrecoPromocao
                           }).FirstOrDefault();
            }

            if (produto != null)
            {
                DataContext = produto;
            }
            else
            {
                await ExibirMensagem("Não foi possível encontrar o produto.");
                this.Frame.GoBack();
            }

            pnlPrincipal.Visibility = Windows.UI.Xaml.Visibility.Visible;
            pnlCarregando.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            DataTransferManager.GetForCurrentView().DataRequested += OnDataRequested;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            DataTransferManager.GetForCurrentView().DataRequested -= OnDataRequested;
        }

        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            ProdutoVM dataContext = DataContext as ProdutoVM;
            DataRequest dataRequest = args.Request;

            dataRequest.Data.Properties.Title = txtTitulo.Text;
            dataRequest.Data.Properties.Description = string.Concat("Compartilhamento do produto ", dataContext.Descricao);
            string texto = string.Concat( "Confiram o produto ", dataContext.Descricao, " no aplicativo CompreAqui, está custando apenas ", dataContext.PrecoAPagar, " R$.");
            dataRequest.Data.SetText(texto);
        }





        private async Task ExibirMensagem(string mensagem)
        {
            MessageDialog messageDialog = new MessageDialog(mensagem);
            await messageDialog.ShowAsync();
        }

        private void Fixar_Click(object sender, RoutedEventArgs e)
        {
            CriarTileSecundaria();
        }

        private async void CriarTileSecundaria()
        {
            ProdutoVM produto = this.DataContext as ProdutoVM;
            SecondaryTile tileSecundaria = new SecondaryTile(produto.Id.ToString());

            tileSecundaria.DisplayName = produto.Descricao;
            tileSecundaria.Arguments = string.Concat("ProdutoDetalhe:", produto.Id);
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



        private void Compartilhar_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        private async void Comprar_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.ApplicationDataContainer configuracoes =
                            Windows.Storage.ApplicationData.Current.LocalSettings;

            if (configuracoes.Values.ContainsKey("usuarioId") &&
                Convert.ToInt32(configuracoes.Values["usuarioId"]) != 0)
            {
                this.Frame.Navigate(typeof(FinalizarCompra));
            }
            else
            {
                await ExibirMensagem("Ops, desculpe, mas você não pode efetuar uma compra sem estar autenticado no aplicativo.");
                this.Frame.Navigate(typeof(Entrar));
            }
        }


    }
}
