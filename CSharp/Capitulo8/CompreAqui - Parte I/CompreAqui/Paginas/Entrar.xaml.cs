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
    public sealed partial class Entrar : Page
    {
        private UsuarioVM _usuarioVM;

        public Entrar()
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
        }

        private async void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string validacoes = _usuarioVM.ValidarCamposAutenticar();
            if (string.IsNullOrEmpty(validacoes))
            {
                Usuario usuario = await Usuario.ValidarAutenticacao(_usuarioVM.Nome, _usuarioVM.Senha);
                if (usuario != null)
                {
                    if (_usuarioVM.EntrarAutomaticamente)
                    {
                        usuario.EntrarAutomaticamente = true;
                        await usuario.AtualizarDados();
                    }

                    usuario.Autenticar();
                    this.Frame.Navigate(typeof(ProdutosHub));
                    this.Frame.BackStack.Clear();
                }
                else
                {
                    await ExibirMensagem("- Não foi encontrado um usuário com este nome e senha");
                }
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
    }
}
