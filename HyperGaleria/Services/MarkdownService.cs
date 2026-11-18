using System.IO;

namespace HyperGaleria.Services
{
    public static class MarkdownService
    {
        public static string LeerMarkdown(string rutaArchivo)
        {
            return File.Exists(rutaArchivo) ? File.ReadAllText(rutaArchivo) : string.Empty;
        }
    }
}
