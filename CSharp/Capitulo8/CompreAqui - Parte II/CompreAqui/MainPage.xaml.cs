using CompreAqui.Auxiliar;
using CompreAqui.Modelos;
using CompreAqui.Paginas;
using CompreAqui.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace CompreAqui
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await BancoDados.Instancia.BuscarDados();

            Windows.Storage.ApplicationDataContainer configuracoes =
                            Windows.Storage.ApplicationData.Current.LocalSettings;

            if (configuracoes.Values.ContainsKey("usuarioId") &&
                Convert.ToInt32(configuracoes.Values["usuarioId"]) != 0)
            {
                Usuario ultimoUsuario = BancoDados.Instancia
                                                  .Usuarios
                                                  .FirstOrDefault
                                                  (usuario => usuario.Id == Convert.ToInt32(configuracoes.Values["usuarioId"]));
                if (ultimoUsuario.EntrarAutomaticamente)
                {
                    this.Frame.Navigate(typeof(ProdutosHub));
                    this.Frame.BackStack.Clear();
                }
                else
                    configuracoes.Values["usuarioId"] = 0;
            }
        }

        private void btnAutenticar_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Entrar));
        }

        private void btnEntrar_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ProdutosHub));
        }

        private void btnCriarConta_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CriarConta));
        }
    }
}
