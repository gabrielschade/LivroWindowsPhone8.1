using CompreAqui.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompreAqui.Modelos
{
    public class Usuario
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string NomeUsuario { get; set; }

        public string Senha { get; set; }

        public bool EntrarAutomaticamente { get; set; }

        public void Autenticar()
        {
            Windows.Storage.ApplicationDataContainer configuracoes =
                Windows.Storage.ApplicationData.Current.LocalSettings;

            if (configuracoes.Values.ContainsKey("usuarioId"))
            {
                configuracoes.Values["usuarioId"] = this.Id;
            }
            else
            {
                configuracoes.Values.Add("usuarioId", this.Id);
            }
        }
        public async Task AtualizarDados()
        {
            if (BancoDados.Instancia.Usuarios == null)
                await BancoDados.Instancia.BuscarDados();

            {
                Usuario usuarioAntigo = BancoDados.Instancia.Usuarios.FirstOrDefault(usuario => usuario.Id == this.Id);

                usuarioAntigo.EntrarAutomaticamente = this.EntrarAutomaticamente;
                usuarioAntigo.Email = this.Email;
                usuarioAntigo.NomeUsuario = this.NomeUsuario;
                usuarioAntigo.Senha = this.Senha;

                await BancoDados.Instancia.GravarDados();
            }
        }

        public async static Task<Usuario> ValidarAutenticacao(string nomeUsuario, string senha)
        {
            if (BancoDados.Instancia.Usuarios == null)
                await BancoDados.Instancia.BuscarDados();


            return BancoDados.Instancia.Usuarios
                             .FirstOrDefault(usuario =>
                              usuario.NomeUsuario == nomeUsuario &&
                              usuario.Senha == senha);

        }

    }
}
