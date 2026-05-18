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

        public ProductosView()
        {

            InitializeComponent();


            _productoService = new ProductoService();
        }

        private async void BtnCargarProductos_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Boton pulsado");

            TxtMensajeProductos.Text = "Cargando productos...";

            BtnCargarProductos.IsEnabled = false;

            List<ProductoDTO> productos = await _productoService.GetProductosAsync();

            TablaProductos.ItemsSource = productos;

            if (productos.Count == 0)  {

                TxtMensajeProductos.Text = "No se encontraron productos";
            }else {


                TxtMensajeProductos.Text = "Productos cargados " + productos.Count;
            }

            BtnCargarProductos.IsEnabled = true;
        }
    }
}
 