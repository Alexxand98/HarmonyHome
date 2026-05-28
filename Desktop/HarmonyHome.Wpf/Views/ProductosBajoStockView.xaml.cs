using HarmonyHome.Wpf.Models.DTOs;
using HarmonyHome.Wpf.Services;
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

        private async void BtnCargar_Click(object sender, RoutedEventArgs e)
        {
            await CargarProductosBajoStock();
        }

        private async Task CargarProductosBajoStock()
        {
            TxtMensaje.Text = "Cargando productos bajo stock...";

            List<ProductoBajoStockDTO> productos = await _stockService.GetProductosBajoStockGeneralAsync();

            TablaProductosBajoStock.ItemsSource = productos;

            TxtResumen.Text = "Productos encontrados: " + productos.Count;
            TxtMensaje.Text = "Consulta realizada correctamente.";
        }
    }
}