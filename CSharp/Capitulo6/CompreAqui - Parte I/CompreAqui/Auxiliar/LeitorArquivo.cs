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
                var file = await Package.Current.InstalledLocation.GetFileAsync(@"Resources\dados.txt");

                using (var rdr = new StreamReader(await file.OpenStreamForReadAsync()))
                    return rdr.ReadToEnd();
            }
            catch
            {
                throw;
            }
        }
    }
}
