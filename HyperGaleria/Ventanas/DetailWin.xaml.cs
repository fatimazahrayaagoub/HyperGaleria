using HyperGaleria.Models;
using HyperGaleria.Services;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HyperGaleria.Ventanas
{
    public partial class DetailWin : Window
    {
        private readonly ArchivoItem? _archivo;
        private ScaleTransform? _imageScale;
        private MediaElement? _mediaElement;
        private TextBox? _textBox;

        public DetailWin()
        {
            InitializeComponent();
        }

        public DetailWin(ArchivoItem archivo)
        {
            InitializeComponent();
            _archivo = archivo;
            this.DataContext = archivo;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Owner is NaveWin naveWin)
            {
                bool noche = naveWin.EsModoNoche();
                var bgKey = noche ? "NightBackgroundBrush" : "NormalBackgroundBrush";
                var fgKey = noche ? "NightForegroundBrush" : "NormalForegroundBrush";
                Background = (Brush)Application.Current.Resources[bgKey];
                Foreground = (Brush)Application.Current.Resources[fgKey];
            }

            CargarContenido();
        }

        private void CargarContenido()
        {
            if (_archivo == null || string.IsNullOrWhiteSpace(_archivo.Ruta))
                return;

            switch (_archivo.Tipo)
            {
                case TipoArchivo.Imagen:
                    MostrarImagen();
                    break;
                case TipoArchivo.Audio:
                case TipoArchivo.Video:
                    MostrarMultimedia();
                    break;
                case TipoArchivo.Texto:
                    MostrarTexto();
                    break;
                case TipoArchivo.Markdown:
                    MostrarMarkdown();
                    break;
                default:
                    AbrirArchivoExterno();
                    break;
            }
        }

        private void MostrarImagen()
        {
            _imageScale = new ScaleTransform(1.0, 1.0);
            var image = new Image
            {
                Source = new BitmapImage(new Uri(_archivo!.Ruta)),
                Style = (Style)FindResource("ImageViewerStyle"),
                LayoutTransform = _imageScale
            };

            var scroll = new ScrollViewer
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = image
            };
            ContenedorPrincipal.Children.Add(scroll);

            var btnZoomIn = new Button { Content = "Zoom +" };
            btnZoomIn.Click += (_, _) => AjustarZoom(0.2);

            var btnZoomOut = new Button { Content = "Zoom -" };
            btnZoomOut.Click += (_, _) => AjustarZoom(-0.2);

            PanelControles.Children.Add(btnZoomIn);
            PanelControles.Children.Add(btnZoomOut);
        }

        private void AjustarZoom(double delta)
        {
            if (_imageScale == null) return;
            double nuevaEscala = Math.Max(0.2, Math.Min(5.0, _imageScale.ScaleX + delta));
            _imageScale.ScaleX = nuevaEscala;
            _imageScale.ScaleY = nuevaEscala;
        }

        private void MostrarMultimedia()
        {
            _mediaElement = new MediaElement
            {
                Source = new Uri(_archivo!.Ruta),
                Style = (Style)FindResource("MediaViewerStyle")
            };
            ContenedorPrincipal.Children.Add(_mediaElement);

            var btnPlay = new Button { Content = "Play" };
            btnPlay.Click += (_, _) => _mediaElement?.Play();

            var btnPause = new Button { Content = "Pause" };
            btnPause.Click += (_, _) => _mediaElement?.Pause();

            PanelControles.Children.Add(btnPlay);
            PanelControles.Children.Add(btnPause);

            _mediaElement.Play();
        }

        private void MostrarTexto()
        {
            _textBox = new TextBox
            {
                Text = File.ReadAllText(_archivo!.Ruta),
                Style = (Style)FindResource("TextViewerStyle")
            };
            ContenedorPrincipal.Children.Add(_textBox);

            var cmbFont = new ComboBox
            {
                Width = 150,
                Margin = new Thickness(4),
                VerticalContentAlignment = VerticalAlignment.Center
            };
            string[] fuentes = { "Segoe UI", "Consolas", "Arial", "Times New Roman", "Courier New" };
            foreach (var f in fuentes)
                cmbFont.Items.Add(f);

            cmbFont.SelectedIndex = 0;
            cmbFont.SelectionChanged += (_, _) =>
            {
                if (_textBox != null && cmbFont.SelectedItem is string nombreFuente)
                    _textBox.FontFamily = new FontFamily(nombreFuente);
            };

            var btnFontMas = new Button { Content = "FontSize +" };
            btnFontMas.Click += (_, _) =>
            {
                if (_textBox != null) _textBox.FontSize = Math.Min(48, _textBox.FontSize + 2);
            };

            var btnFontMenos = new Button { Content = "FontSize -" };
            btnFontMenos.Click += (_, _) =>
            {
                if (_textBox != null) _textBox.FontSize = Math.Max(8, _textBox.FontSize - 2);
            };

            PanelControles.Children.Add(cmbFont);
            PanelControles.Children.Add(btnFontMas);
            PanelControles.Children.Add(btnFontMenos);
        }

        private void MostrarMarkdown()
        {
            MdViewer.Markdown = MarkdownService.LeerMarkdown(_archivo!.Ruta);
            ScrollMd.Visibility = Visibility.Visible;
        }

        private void AbrirArchivoExterno()
        {
            var lbl = new Label
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 14
            };

            try
            {
                VisorMultimedia.AbrirConAplicacionAsociada(_archivo!.Ruta);
                lbl.Content = $"Archivo abierto con la aplicación asociada del sistema:\n{_archivo.Ruta}";
            }
            catch (Exception ex)
            {
                lbl.Content = $"No se pudo abrir el archivo con una aplicación asociada:\n{ex.Message}";
            }

            ContenedorPrincipal.Children.Add(lbl);
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_mediaElement != null)
            {
                _mediaElement.Stop();
                _mediaElement.Close();
            }
        }
    }
}
