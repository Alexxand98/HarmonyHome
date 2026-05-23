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
    /// Lógica de interacción para AltaStockView.xaml
    /// </summary>
    public partial class AltaStockView : Window
    {
        private readonly ProductoService _productoService;
        private readonly UbicacionService _ubicacionService;
        private readonly StockService _stockService;

        public AltaStockView()
        {
            InitializeComponent();

            _productoService = new ProductoService();

            _ubicacionService = new UbicacionService();

            _stockService = new StockService();

            Loaded += AltaStockView_Loaded;
        }

        private async void AltaStockView_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarDatos();
        }

        private async Task CargarDatos()
        {
            TxtMensaje.Text = "Cargando datos...";

            List<ProductoDTO> productos = await _productoService.GetProductosAsync();
            productos = productos.Where(p => p.Activo && p.Habilitado).ToList();

            List<UbicacionDTO> ubicaciones = await _ubicacionService.GetUbicacionesPorTipoAsync(2);
            ubicaciones = ubicaciones.Where(u => u.Activa).ToList();

            CmbProducto.ItemsSource = productos;
            CmbUbicacion.ItemsSource = ubicaciones;

            TxtMensaje.Text = "Datos cargados.";
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarFormulario())
            {
                return;
            }

            CreateStockUbicacionDTO stock = new CreateStockUbicacionDTO();

            stock.ProductoId = (int)CmbProducto.SelectedValue;
            stock.UbicacionId = (int)CmbUbicacion.SelectedValue;
            stock.Cantidad = int.Parse(TxtCantidad.Text);

            string mensaje = await _stockService.CrearStockAsync(stock);

            TxtMensaje.Text = mensaje;
        }

        private bool ValidarFormulario()
        {
            if (CmbProducto.SelectedValue == null)
            {
                TxtMensaje.Text = "Selecciona un producto";
                return false;
            }

            if (CmbUbicacion.SelectedValue == null)
            {
                TxtMensaje.Text = "Selecciona una ubicación";
                return false;
            }

            if (!int.TryParse(TxtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                TxtMensaje.Text = "La cantidad debe ser mayor que cero";
                return false;
            }

            return true;
        }
    }
}