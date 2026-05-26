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
        }

        private async void BtnCargarProductos_Click(object sender, RoutedEventArgs e)
        {
            TxtMensajeProductos.Text = "Cargando productos...";

            BtnCargarProductos.IsEnabled = false;

            try{
                _productos = await _productoService.GetProductosAsync();

                TablaProductos.ItemsSource = _productos;

                if (_productos.Count == 0){

                    TxtMensajeProductos.Text = "No se encontraron productos";

                }else{

                    TxtMensajeProductos.Text = "Productos cargados: " + _productos.Count;

                }

            }finally {

                BtnCargarProductos.IsEnabled = true;

            }
        }

        private async void BtnBuscarProducto_Click(object sender, RoutedEventArgs e)
        {
            string texto = TxtBuscarProducto.Text.Trim();

            if (string.IsNullOrWhiteSpace(texto)) {

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

            TxtMensajeProductos.Text = "Búsqueda limpiada.";
        }
    }


}
 