using HarmonyHome.Wpf.Models.DTOs;
using HarmonyHome.Wpf.Services;
using System.Collections.Generic;
using System.Windows;

namespace HarmonyHome.Wpf.Views
{
    /// <summary>
    /// Lógica de interacción para ProductosView.xaml
    /// </summary>
    public partial class ProductosView : Window
    {
        private readonly ProductoService _productoService;

        private List<ProductoDTO> _productos = new List<ProductoDTO>();

        public ProductosView()
        {
            InitializeComponent();

            _productoService = new ProductoService();

            Loaded += ProductosView_Loaded;
        }

        private async void ProductosView_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarProductos();
        }

        private async void BtnActualizarProductos_Click(object sender, RoutedEventArgs e)
        {
            await CargarProductos();
        }

        private async Task CargarProductos()
        {
            TxtMensajeProductos.Text = "Cargando productos...";

            BtnActualizarProductos.IsEnabled = false;

            try
            {
                _productos = await _productoService.GetProductosAsync();

                TablaProductos.ItemsSource = _productos;

                if (_productos.Count == 0)
                {

                    TxtMensajeProductos.Text = "No se encontraron productos";

                }
                else
                {

                    TxtMensajeProductos.Text = "Productos cargados: " + _productos.Count;
                }

            }
            finally
            {

                BtnActualizarProductos.IsEnabled = true;
            }
        }

        private async void BtnBuscarProducto_Click(object sender, RoutedEventArgs e)
        {
            string texto = TxtBuscarProducto.Text.Trim();

            if (string.IsNullOrWhiteSpace(texto))
            {

                TablaProductos.ItemsSource = _productos;

                TxtMensajeProductos.Text = "Productos cargados: " + _productos.Count;

                return;
            }

            List<ProductoDTO> productosFiltrados = await _productoService.BuscarProductosAsync(texto);

            TablaProductos.ItemsSource = productosFiltrados;

            TxtMensajeProductos.Text = "Resultados encontrados: " + productosFiltrados.Count;
        }

        private void BtnLimpiarBusquedaProducto_Click(object sender, RoutedEventArgs e)
        {
            TxtBuscarProducto.Text = "";

            TablaProductos.ItemsSource = _productos;

            TxtMensajeProductos.Text = "Búsqueda limpia";
        }

        private void TablaProductos_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ProductoDTO? producto = TablaProductos.SelectedItem as ProductoDTO;

            if (producto == null)
            {
                return;
            }

            ProductoDetalleView detalleView = new ProductoDetalleView(producto);

            detalleView.ShowDialog();
        }
    }
}