using System;
using System.Collections.Generic;
using System.IO;
using HyperGaleria.Models;

namespace HyperGaleria.Services
{
    public class ExploradorArchivos
    {
        public List<ArchivoItem> ObtenerArchivos(string rutaCarpeta)
        {
            List<ArchivoItem> archivos = new List<ArchivoItem>();

            if (!Directory.Exists(rutaCarpeta))
                return archivos;

            try
            {
                foreach (string carpeta in Directory.GetDirectories(rutaCarpeta))
                {
                    try
                    {
                        var dirInfo = new DirectoryInfo(carpeta);
                        if ((dirInfo.Attributes & FileAttributes.Hidden) != 0) continue;
                        archivos.Add(new ArchivoItem(carpeta));
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                }

                string[] rutas = Directory.GetFiles(
                    rutaCarpeta,
                    "*.*",
                    SearchOption.TopDirectoryOnly
                );

                foreach (string ruta in rutas)
                {
                    try
                    {
                        var fileInfo = new FileInfo(ruta);
                        if ((fileInfo.Attributes & FileAttributes.Hidden) != 0) continue;

                        archivos.Add(new ArchivoItem(ruta));
                    }
                    catch (IOException)
                    {
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (IOException)
            {
            }

            return archivos;
        }
    }
}
