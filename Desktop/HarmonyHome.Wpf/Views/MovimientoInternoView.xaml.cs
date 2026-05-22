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
    /// Lógica de interacción para MovimientoInternoView.xaml
    /// </summary>
    public partial class MovimientoInternoView : Window
    {
        private readonly ProductoService _productoService;
        private readonly UbicacionService _ubicacionService;
        private readonly StockService _stockService;

        public MovimientoInternoView()
        {
            InitializeComponent();

            _productoService = new ProductoService();

            _ubicacionService = new UbicacionService();

            _stockService = new StockService();

            Loaded += MovimientoInternoView_Loaded;
        }

        private async void MovimientoInternoView_Loaded(object sender, RoutedEventArgs e)
        {

            await CargarDatos();
        }

        private async Task CargarDatos()
        {
            TxtMensaje.Text = "Cargando datos...";

            List<ProductoDTO> productos = await _productoService.GetProductosAsync();

            List<UbicacionDTO> ubicaciones = await _ubicacionService.GetUbicacionesPorTipoAsync(2);

            CmbProducto.ItemsSource = productos;

            CmbOrigen.ItemsSource = ubicaciones;

            CmbDestino.ItemsSource = ubicaciones;

            TxtMensaje.Text = "Datos cargados.";
        }

        private async void BtnMover_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarFormulario()) {
                return;
            }

            CreateMovimientoInternoDTO movimiento = new CreateMovimientoInternoDTO();

            movimiento.ProductoId = (int)CmbProducto.SelectedValue;
            movimiento.UbicacionOrigenId = (int)CmbOrigen.SelectedValue;
            movimiento.UbicacionDestinoId = (int)CmbDestino.SelectedValue;

            movimiento.Cantidad = int.Parse(TxtCantidad.Text);
            movimiento.Observaciones = TxtObservaciones.Text.Trim();

            string mensaje = await _stockService.MoverStockAsync(movimiento);

            TxtMensaje.Text = mensaje;
        }

        private bool ValidarFormulario()
        {
            if (CmbProducto.SelectedValue == null) {
                TxtMensaje.Text = "Selecciona un producto.";
                return false;
            }

            if (CmbOrigen.SelectedValue == null) {
                TxtMensaje.Text = "Selecciona la ubicación origen.";
                return false;
            }

            if (CmbDestino.SelectedValue == null) {
                TxtMensaje.Text = "Selecciona la ubicación destino.";
                return false;
            }

            if ((int)CmbOrigen.SelectedValue == (int)CmbDestino.SelectedValue) {
                TxtMensaje.Text = "La ubicación origen y destino no pueden ser la misma.";
                return false;
            }

            if (!int.TryParse(TxtCantidad.Text, out int cantidad) || cantidad <= 0) {
                TxtMensaje.Text = "La cantidad debe ser mayor que cero.";
                return false;
            }

            return true;
        }
    }
}