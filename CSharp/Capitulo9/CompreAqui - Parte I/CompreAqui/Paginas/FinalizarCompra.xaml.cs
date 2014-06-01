using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
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
    public sealed partial class FinalizarCompra : Page
    {
        private Geopoint localizacaoUsuario;
        public FinalizarCompra()
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
            ObterPosicaoAtual();
        }

        private void MarcarPosicaoNoMapa(Geopoint posicao, string texto)
        {
            MapIcon marcacao = new MapIcon();
            marcacao.Title = texto;
            marcacao.Location = posicao;

            Mapa.MapElements.Add(marcacao);
        }

        private async void ObterPosicaoAtual()
        {
            Geolocator localizador = new Geolocator();
            localizador.DesiredAccuracy = PositionAccuracy.Default;

            try
            {
                Geoposition posicaoAtual = await localizador.GetGeopositionAsync();
                BasicGeoposition posicao = new BasicGeoposition();
                posicao.Latitude = posicaoAtual.Coordinate.Point.Position.Latitude;
                posicao.Longitude = posicaoAtual.Coordinate.Point.Position.Longitude;

                Geopoint posicaoEspacial = new Geopoint(posicao);

                MarcarPosicaoNoMapa(posicaoEspacial, "VOCÊ");
                Mapa.Center = posicaoEspacial;
                Mapa.ZoomLevel = 15.5;

                localizacaoUsuario = posicaoEspacial;
            }
            catch
            {
                MessageDialog dialogo = new MessageDialog("Não foi possível encontrar sua localização. Por favor, verifique se suas configurações de localização estão habilitadas.");
                dialogo.ShowAsync();
            }
        }

        private async void BuscarPorEndereco(string logradouro, string cidade)
        {
            MapLocationFinderResult resultado = await MapLocationFinder.FindLocationsAsync(string.Concat(logradouro,", " ,cidade), localizacaoUsuario, 1);

            foreach (MapLocation local in resultado.Locations)
                MarcarPosicaoNoMapa(local.Point, "ENDEREÇO BUSCADO");

            Logradouro.Text = string.Empty;
            Cidade.Text = string.Empty;
        }

        private void AlterarAcumulativoZoom(double zoomLevel)
        {
            double zoomLevelLimite = Mapa.ZoomLevel + zoomLevel;

            if (zoomLevel > 0)
                zoomLevelLimite = Math.Min(20, zoomLevelLimite);
            else
                zoomLevelLimite = Math.Max(1, zoomLevelLimite);

            Mapa.ZoomLevel = zoomLevelLimite;
        }

        private void AumentarZoom_Tapped(object sender, TappedRoutedEventArgs e)
        {
            AlterarAcumulativoZoom(3);
        }

        private void DiminuirZoom_Tapped(object sender, TappedRoutedEventArgs e)
        {
            AlterarAcumulativoZoom(-3);
        }

        private void AdicionarEndereco_Tapped(object sender, TappedRoutedEventArgs e)
        {
            BuscarPorEndereco(Logradouro.Text, Cidade.Text);
        }

        private async void FinalizarCompra_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MessageDialog dialogo = new MessageDialog("Compra efetuada com sucesso!");
            await dialogo.ShowAsync();
            this.Frame.GoBack();
        }

    }
}
