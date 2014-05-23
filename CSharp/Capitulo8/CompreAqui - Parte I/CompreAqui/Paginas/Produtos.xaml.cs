using CompreAqui.Auxiliar;
using CompreAqui.Modelos;
using CompreAqui.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

        private void ListaProdutos_Loaded(object sender, RoutedEventArgs e)
        {
            ListView lista = sender as ListView;
            List<ProdutoVM> produtos = (from produto in Loja.Dados.Produtos
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
                    produtos = produtos.Where(produto => produto.CategoriaId == _parametrosPesquisa.CategoriaId).ToList();
                    Titulo.Text = produtos.FirstOrDefault().CategoriaDescricao;
                }
                else if (!string.IsNullOrEmpty(_parametrosPesquisa.DescricaoProduto))
                {
                    produtos = produtos.Where(produto =>
                                            produto.Descricao.ToLower()
                                                    .Contains(_parametrosPesquisa.DescricaoProduto.ToLower()))
                                                    .ToList();

                    Titulo.Text = "resultados";
                }
            }

            lista.ItemsSource = produtos;
            if (produtos.Count == 0)
            {
                CampoMensagem.Visibility = Visibility.Visible;
            }
        }
    }
}
