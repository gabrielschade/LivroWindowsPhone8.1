using CompreAqui.Common;
using CompreAqui.Modelos;
using CompreAqui.Resources;
using CompreAqui.ViewModels;
using NotificationsExtensions.TileContent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
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
    public sealed partial class CriarConta : Page
    {
        private UsuarioVM _usuarioVM;
        private NavigationHelper _navigationHelper;

        public CriarConta()
        {
            this.InitializeComponent();

            this._navigationHelper = new NavigationHelper(this);
            this._navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this._navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this._navigationHelper.OnNavigatedTo(e);

            if (_usuarioVM == null)
                _usuarioVM = new UsuarioVM();
            this.DataContext = _usuarioVM;


        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this._navigationHelper.OnNavigatedFrom(e);
        }


        private async void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string validacoes = _usuarioVM.ValidarCamposCadastro();
            if (!string.IsNullOrEmpty(validacoes))
            {
                string mensagem = string.Concat(
                    "Não foi possível gravar esta conta por um ou mais motivos abaixo:", Environment.NewLine, validacoes);

                await ExibirMensagem(mensagem);
            }
            else
            {
                try
                {
                    BancoDados.Instancia.AdicionarUsuario(_usuarioVM, true);
                    this.Frame.Navigate(typeof(ProdutosHub));
                    this.Frame.BackStack.Clear();
                    AtualizarLiveTile(_usuarioVM.Nome, _usuarioVM.Email);
                }
                catch
                {
                    string mensagem = "Houve um problema ao tentar criar sua conta,tente novamente mais tarde.";
                    ExibirMensagem(mensagem);
                }
            }
        }

        private async Task ExibirMensagem(string mensagem)
        {
            MessageDialog messageDialog = new MessageDialog(mensagem);
            await messageDialog.ShowAsync();
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (e.PageState != null)
            {
                _usuarioVM = new UsuarioVM();

                if (e.PageState.ContainsKey("email"))
                    _usuarioVM.Email = e.PageState["email"] as string;

                if (e.PageState.ContainsKey("usuario"))
                    _usuarioVM.Nome = e.PageState["usuario"] as string;

                if (e.PageState.ContainsKey("senha"))
                    _usuarioVM.Senha = e.PageState["senha"] as string;

                if (e.PageState.ContainsKey("confirmacao"))
                    _usuarioVM.ConfirmacaoSenha = e.PageState["confirmacao"] as string;

                if (e.PageState.ContainsKey("lembrarme"))
                    _usuarioVM.EntrarAutomaticamente =
                     Convert.ToBoolean(e.PageState["lembrarme"]);
            }
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            e.PageState["email"] = _usuarioVM.Email;
            e.PageState["usuario"] = _usuarioVM.Nome;
            e.PageState["senha"] = _usuarioVM.Senha;
            e.PageState["confirmacao"] = _usuarioVM.ConfirmacaoSenha;
            e.PageState["lembrarme"] = _usuarioVM.EntrarAutomaticamente;
        }

        private void AtualizarLiveTile(string usuario, string email)
        {
            ITileWide310x150ImageAndText01 wideTile = TileContentFactory.CreateTileWide310x150ImageAndText01();
            wideTile.TextCaptionWrap.Text = string.Concat(usuario, " - ",email);
            wideTile.Image.Src = @"ms-appx:/Assets/WideLogo.scale-240.png";

            ITileSquare150x150PeekImageAndText01 squareTile = TileContentFactory.CreateTileSquare150x150PeekImageAndText01();
            squareTile.TextBody1.Text = usuario;
            squareTile.TextBody2.Text = email;
            squareTile.Image.Src = @"ms-appx:/Assets/Square71x71Logo.scale-240.png";

            wideTile.Square150x150Content = squareTile;

            TileUpdateManager.CreateTileUpdaterForApplication().Update(wideTile.CreateNotification());
        }
    }
}
