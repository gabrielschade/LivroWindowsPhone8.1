using CompreAqui.Modelos;
using CompreAqui.Resources;
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
    public sealed partial class SuaConta : Page
    {
        private UsuarioVM _usuarioVM;
        private Usuario _usuarioAtual;

        public SuaConta()
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
            if (_usuarioVM == null)
                _usuarioVM = new UsuarioVM();
            this.DataContext = _usuarioVM;

            Windows.Storage.ApplicationDataContainer configuracoes =
                Windows.Storage.ApplicationData.Current.LocalSettings;

            _usuarioAtual = BancoDados.Instancia
                                  .Usuarios
                                  .FirstOrDefault
                                  (usuario => usuario.Id == Convert.ToInt32(configuracoes.Values["usuarioId"]));
            if (_usuarioAtual != null)
            {
                _usuarioVM.Nome = _usuarioAtual.NomeUsuario;
                _usuarioVM.Email = _usuarioAtual.Email;
            }
        }

        private async void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string validacoes = _usuarioVM.ValidarCamposTrocaSenha();
            if (string.IsNullOrEmpty(validacoes))
            {
                if (_usuarioAtual.Senha == _usuarioVM.SenhaAntesAlteracao)
                {
                    _usuarioAtual.Senha = _usuarioVM.Senha;
                    await _usuarioAtual.AtualizarDados();

                    await ExibirMensagem("Sua senha foi atualizada com sucesso!");
                    _usuarioVM.SenhaAntesAlteracao = string.Empty;
                    _usuarioVM.Senha = string.Empty;
                    _usuarioVM.ConfirmacaoSenha = string.Empty;
                }
                else
                    await ExibirMensagem("- O campo Senha atual não está com a senha correta");
            }
            else
            {
                await ExibirMensagem(validacoes);
            }
        }

        private async Task ExibirMensagem(string mensagem)
        {
            MessageDialog messageDialog = new MessageDialog(mensagem);
            await messageDialog.ShowAsync();
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog mensagem = new MessageDialog("Tem certeza que deseja desconectar sua conta do aplicativo?", "Confirmação");
            mensagem.Commands.Add(new UICommand("Sim", delegate(IUICommand command)
            {
                Windows.Storage.ApplicationDataContainer configuracoes =
            Windows.Storage.ApplicationData.Current.LocalSettings;

                configuracoes.Values["usuarioId"] = 0;
                this.Frame.Navigate(typeof(MainPage));
                this.Frame.BackStack.Clear();
            }));

            mensagem.Commands.Add(new UICommand("Não"));

            await mensagem.ShowAsync();
        }

    }
}
