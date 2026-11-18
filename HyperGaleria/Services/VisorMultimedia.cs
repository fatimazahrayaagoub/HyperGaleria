using System.Diagnostics;

namespace HyperGaleria.Services
{
    public static class VisorMultimedia
    {
        public static void AbrirConAplicacionAsociada(string rutaArchivo)
        {
            Process.Start(new ProcessStartInfo(rutaArchivo)
            {
                UseShellExecute = true
            });
        }
    }
}
