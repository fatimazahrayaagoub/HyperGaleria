using System;
using System.IO;

namespace HyperGaleria.Models
{
    public class ArchivoItem
    {
        public string Nombre { get; set; }

        public string Ruta { get; set; }

        public string Extension { get; set; }

        public TipoArchivo Tipo { get; set; }

        public long Tamano { get; set; }

        public ArchivoItem(string ruta)
        {
            if (Directory.Exists(ruta))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(ruta);
                Nombre = dirInfo.Name;
                Ruta = dirInfo.FullName;
                Extension = "<Carpeta>";
                Tamano = 0;
                Tipo = TipoArchivo.Carpeta;
                return;
            }

            FileInfo informacion = new FileInfo(ruta);

            Nombre = informacion.Name;
            Ruta = informacion.FullName;
            Extension = informacion.Extension.ToLower();
            Tamano = informacion.Length;

            Tipo = ObtenerTipo(Extension);
        }

        private TipoArchivo ObtenerTipo(string extension)
        {
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".gif":
                case ".bmp":
                case ".webp":
                    return TipoArchivo.Imagen;

                case ".mp4":
                case ".avi":
                case ".mkv":
                case ".mov":
                    return TipoArchivo.Video;

                case ".mp3":
                case ".wav":
                case ".wma":
                case ".aac":
                    return TipoArchivo.Audio;

                case ".txt":
                    return TipoArchivo.Texto;

                case ".md":
                    return TipoArchivo.Markdown;

                case ".pdf":
                    return TipoArchivo.Pdf;

                default:
                    return TipoArchivo.Otro;
            }
        }
    }
}
