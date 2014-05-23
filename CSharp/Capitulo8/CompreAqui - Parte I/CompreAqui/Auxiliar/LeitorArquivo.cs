using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Streams;

namespace CompreAqui.Auxiliar
{
    public static class LeitorArquivo
    {
        public async static Task<string> LerAsync()
        {
            try
            {
                var arquivo = await Package.Current.InstalledLocation.GetFileAsync(@"Resources\dados.txt");
                return await FileIO.ReadTextAsync(arquivo);
            }
            catch
            {
                throw;
            }
        }
    }
}
