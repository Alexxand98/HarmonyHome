using HarmonyHome.Wpf.Models.DTOs;
using HarmonyHome.Wpf.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace HarmonyHome.Wpf.Views
{
    /// <summary>
    /// Lógica de interacción para ProductosBajoStockView.xaml
    /// </summary>
    public partial class ProductosBajoStockView : Window
    {
        private readonly StockService _stockService;

        public ProductosBajoStockView()
        {
            InitializeComponent();

            _stockService = new StockService();

            Loaded += ProductosBajoStockView_Loaded;
        }

        private async void ProductosBajoStockView_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarProductosBajoStock();
        }

        private async void BtnActualizar_Click(object sender, RoutedEventArgs e)
        {
            await CargarProductosBajoStock();
        }

        private async Task CargarProductosBajoStock()
        {
            TxtMensaje.Text = "Cargando productos bajo stock...";
            BtnActualizar.IsEnabled = false;

            try
            {
                List<ProductoBajoStockDTO> productos = await _stockService.GetProductosBajoStockGeneralAsync();

                TablaProductosBajoStock.ItemsSource = productos;

                TxtResumen.Text = "Productos encontrados: " + productos.Count;

                if (productos.Count == 0)
                {
                    TxtMensaje.Text = "No hay productos bajo stock.";
                }
                else
                {
                    TxtMensaje.Text = "Consulta realizada correctamente.";
                }

            }
            finally
            {

                BtnActualizar.IsEnabled = true;
            }
        }
    }
}