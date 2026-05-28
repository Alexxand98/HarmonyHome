using HarmonyHome.Wpf.Models.DTOs;
using HarmonyHome.Wpf.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

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

            LimpiarFormulario();

            TxtMensaje.Text = "Datos cargados.";
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarFormulario()){
                return;
            }

            BtnGuardar.IsEnabled = false;
            TxtMensaje.Text = "Guardando stock...";

            try{
                CreateStockUbicacionDTO stock = new CreateStockUbicacionDTO();

                stock.ProductoId = (int)CmbProducto.SelectedValue;
                stock.UbicacionId = (int)CmbUbicacion.SelectedValue;
                stock.Cantidad = int.Parse(TxtCantidad.Text);

                string mensaje = await _stockService.CrearStockAsync(stock);

                LimpiarFormulario();

                TxtMensaje.Text = mensaje;

            }finally{

                BtnGuardar.IsEnabled = true;
            }
        }

        private bool ValidarFormulario()
        {
            if (CmbProducto.SelectedValue == null){
                TxtMensaje.Text = "Selecciona un producto.";
                return false;
            }

            if (CmbUbicacion.SelectedValue == null){
                TxtMensaje.Text = "Selecciona una ubicación.";
                return false;
            }

            if (!int.TryParse(TxtCantidad.Text, out int cantidad) || cantidad <= 0){
                TxtMensaje.Text = "La cantidad debe ser mayor que cero.";
                return false;
            }

            return true;
        }

        private void LimpiarFormulario()
        {
            CmbProducto.SelectedIndex = -1;
            CmbUbicacion.SelectedIndex = -1;

            CmbProducto.Text = "Seleccione un producto";
            CmbUbicacion.Text = "Seleccione una ubicación";

            TxtCantidad.Text = "";
        }
    }
}