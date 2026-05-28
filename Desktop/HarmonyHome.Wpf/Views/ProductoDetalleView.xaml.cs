using HarmonyHome.Wpf.Helpers;
using HarmonyHome.Wpf.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HarmonyHome.Wpf.Views
{
    /// <summary>
    /// Lógica de interacción para ProductoDetalleView.xaml
    /// </summary>
    public partial class ProductoDetalleView : Window
    {
        private readonly ProductoDTO _producto;

        public ProductoDetalleView(ProductoDTO producto)
        {
            InitializeComponent();

            _producto = producto;

            CargarDatos();
        }

        private void CargarDatos()
        {
            TxtNombre.Text = _producto.Nombre;
            TxtReferencia.Text = "Referencia: " + _producto.Referencia;

            TxtDescripcion.Text = string.IsNullOrWhiteSpace(_producto.Descripcion) ? "Sin descripción registrada" : _producto.Descripcion;

            TxtCategoria.Text = _producto.Categoria;
            TxtStockMinimo.Text = _producto.StockMinimo.ToString();

            TxtPrecioCoste.Text = _producto.PrecioCoste.ToString("0.00") + " €";
            TxtPrecioVenta.Text = _producto.PrecioVenta.ToString("0.00") + " €";

            TxtTrazabilidad.Text = _producto.TipoTrazabilidadNombre;

            string habilitado = _producto.Habilitado ? "Sí" : "No";
            string activo = _producto.Activo ? "Sí" : "No";

            TxtEstado.Text = "Habilitado: " + habilitado + " | Activo: " + activo;

            TxtObservaciones.Text = string.IsNullOrWhiteSpace(_producto.Observaciones) ? "Sin observaciones registradas" : _producto.Observaciones;

            CargarImagen();
        }

        private void CargarImagen()
        {
            string urlImagen = ImageHelper.GetProductoImageUrl(_producto.ImagenUrl);

            if (string.IsNullOrWhiteSpace(urlImagen)){
                return;
            }

            try {
                BitmapImage imagen = new BitmapImage();

                imagen.BeginInit();
                imagen.UriSource = new Uri(urlImagen, UriKind.Absolute);
                imagen.CacheOption = BitmapCacheOption.OnLoad;
                imagen.EndInit();

                ImgProducto.Source = imagen;

            }catch{
                ImgProducto.Source = null;
            }
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}