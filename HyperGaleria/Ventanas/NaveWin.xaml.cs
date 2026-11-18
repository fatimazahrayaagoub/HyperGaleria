using HyperGaleria.Models;
using HyperGaleria.Services;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HyperGaleria.Ventanas
{
    public partial class NaveWin : Window
    {
        private readonly ExploradorArchivos explorador;

        private bool modoNoche = false;
        private string rutaActual = string.Empty;

        public NaveWin()
        {
            InitializeComponent();

            explorador = new ExploradorArchivos();

            string misImagenes = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            CargarCarpeta(misImagenes);
        }

        public bool EsModoNoche() => modoNoche;

        private void CargarCarpeta(string ruta)
        {
            if (string.IsNullOrWhiteSpace(ruta) || !Directory.Exists(ruta))
                return;

            rutaActual = ruta;
            txtRuta.Text = ruta;
            btnSubir.IsEnabled = Directory.GetParent(ruta) != null;
            listaArchivos.ItemsSource = explorador.ObtenerArchivos(ruta);
        }

        private void SeleccionarCarpeta_Click(object sender, RoutedEventArgs e)
        {
            var dialogo = new Microsoft.Win32.OpenFolderDialog
            {
                Title = "Selecciona una carpeta para explorar",
                Multiselect = false
            };

            if (dialogo.ShowDialog() == true)
            {
                CargarCarpeta(dialogo.FolderName);
            }
        }

        private void SubirCarpeta_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(rutaActual))
                return;

            DirectoryInfo? padre = Directory.GetParent(rutaActual);
            if (padre != null)
            {
                CargarCarpeta(padre.FullName);
            }
        }

        private void ListaArchivos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listaArchivos.SelectedItem is ArchivoItem archivo)
            {
                if (archivo.Tipo == TipoArchivo.Carpeta)
                {
                    CargarCarpeta(archivo.Ruta);
                    return;
                }

                listaArchivos.SelectedItem = null;

                DetailWin ventana = new DetailWin(archivo);
                ventana.Owner = this;
                ventana.Show();
            }
        }

        private void CambiarTema_Click(object sender, RoutedEventArgs e)
        {
            modoNoche = !modoNoche;

            var bgKey = modoNoche ? "NightBackgroundBrush" : "NormalBackgroundBrush";
            var fgKey = modoNoche ? "NightForegroundBrush" : "NormalForegroundBrush";
            Application.Current.Resources["AppBackground"] = (Brush)Application.Current.Resources[bgKey];
            Application.Current.Resources["AppForeground"] = (Brush)Application.Current.Resources[fgKey];

            if (modoNoche)
            {
                PanelPrincipal.Background = Brushes.DarkSlateGray;
                listaArchivos.Background = Brushes.DimGray;
                listaArchivos.Foreground = Brushes.White;
            }
            else
            {
                PanelPrincipal.Background = Brushes.WhiteSmoke;
                listaArchivos.Background = Brushes.White;
                listaArchivos.Foreground = Brushes.Black;
            }
        }
    }
}
