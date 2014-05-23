using CompreAqui.Modelos;
using CompreAqui.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CompreAqui.Resources
{
    public class BancoDados
    {
        private const string DATA_FILE = "data.JSON";

        private BancoDados()
        { }

        private static BancoDados instancia;
        public static BancoDados Instancia
        {
            get
            {
                return CriarOuObterInstancia();
            }
            set
            {
                instancia = value;
            }
        }

        public static BancoDados CriarOuObterInstancia()
        {
            if (instancia == null)
            {
                instancia = new BancoDados();
                instancia.Usuarios = new List<Usuario>();
            }

            return instancia;
        }

        

        public async Task BuscarDados()
        {
            StorageFolder pasta = ApplicationData.Current.LocalFolder;
            StorageFile arquivo = await pasta.CreateFileAsync(DATA_FILE, CreationCollisionOption.OpenIfExists);
            string resultado = await FileIO.ReadTextAsync(arquivo);
            if (string.IsNullOrEmpty(resultado))
            {
                Usuario admin = new Usuario();
                admin.NomeUsuario = "admin";
                admin.Senha = "admin";
                admin.Email = "admin@compreaqui.com.br";
                admin.Id = 1;

                Usuarios.Add(admin);
                await GravarDados();
                await BuscarDados();
            }
            else
                Usuarios = JsonConvert.DeserializeObject<List<Usuario>>(resultado);
        }

        public async Task GravarDados()
        {
            StorageFolder pasta = ApplicationData.Current.LocalFolder;
            StorageFile arquivo = await pasta.CreateFileAsync(DATA_FILE, CreationCollisionOption.OpenIfExists);
            await FileIO.WriteTextAsync(arquivo, JsonConvert.SerializeObject(Usuarios), Windows.Storage.Streams.UnicodeEncoding.Utf8);
        }

        public async void AdicionarUsuario(UsuarioVM usuarioVM, bool Autenticar = false)
        {
            Usuario novoUsuario = new Usuario();
            novoUsuario.Email = usuarioVM.Email;
            novoUsuario.NomeUsuario = usuarioVM.Nome;
            novoUsuario.Senha = usuarioVM.Senha;
            novoUsuario.EntrarAutomaticamente = usuarioVM.EntrarAutomaticamente;


            novoUsuario.Id = Usuarios.Count + 1;
            Usuarios.Add(novoUsuario);
            await GravarDados();

            if (Autenticar)
                novoUsuario.Autenticar();
        }

        public List<Usuario> Usuarios { get; private set; }
    }
}
