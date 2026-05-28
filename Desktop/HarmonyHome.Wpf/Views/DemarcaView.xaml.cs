using HarmonyHome.Wpf.Models.DTOs;
using HarmonyHome.Wpf.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HarmonyHome.Wpf.Views
{
    /// <summary>
    /// Lógica de interacción para DemarcaView.xaml
    /// </summary>
    public partial class DemarcaView : Window
    {
        private readonly ProductoService _productoService;
        private readonly UbicacionService _ubicacionService;
        private readonly StockService _stockService;
        private readonly DemarcaService _demarcaService;

        private List<ProductoDTO> _productos = new List<ProductoDTO>();
        private List<UbicacionDTO> _ubicaciones = new List<UbicacionDTO>();
        private List<StockUbicacionDTO> _stock = new List<StockUbicacionDTO>();

        public DemarcaView()
        {
            InitializeComponent();

            _productoService = new ProductoService();
            _ubicacionService = new UbicacionService();
            _stockService = new StockService();
            _demarcaService = new DemarcaService();

            Loaded += DemarcaView_Loaded;
        }

        private async void DemarcaView_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarDatos();
        }

        private async Task CargarDatos()
        {
            TxtMensaje.Text = "Cargando datos...";

            _productos = await _productoService.GetProductosAsync();
            _productos = _productos.Where(p => p.Activo && p.Habilitado).ToList();

            _ubicaciones = await _ubicacionService.GetUbicacionesPorTipoAsync(2);
            _ubicaciones = _ubicaciones.Where(u => u.Activa).ToList();

            _stock = await _stockService.GetStockAsync();

            CmbProducto.ItemsSource = _productos;
            CmbUbicacion.ItemsSource = null;

            LimpiarFormulario();

            TxtMensaje.Text = "Datos cargados.";
        }

        private void CmbProducto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarUbicacionesPorProducto();
        }

        private void CmbUbicacion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MostrarStockDisponible();
        }

        private void CargarUbicacionesPorProducto()
        {
            if (CmbProducto.SelectedValue is not int productoId)
            {
                CmbUbicacion.ItemsSource = null;
                CmbUbicacion.SelectedIndex = -1;
                CmbUbicacion.Text = "Seleccione una ubicación";
                TxtStockDisponible.Text = "Stock disponible: -";
                return;
            }

            List<int> ubicacionesConStock = _stock
                .Where(s => s.ProductoId == productoId && s.Cantidad > 0)
                .Select(s => s.UbicacionId)
                .Distinct()
                .ToList();

            List<UbicacionDTO> ubicacionesProducto = _ubicaciones
                .Where(u => ubicacionesConStock.Contains(u.Id))
                .ToList();

            CmbUbicacion.ItemsSource = ubicacionesProducto;
            CmbUbicacion.SelectedIndex = -1;
            CmbUbicacion.Text = "Seleccione una ubicación";

            TxtStockDisponible.Text = "Stock disponible: -";

            if (ubicacionesProducto.Count == 0) {
                TxtMensaje.Text = "El producto seleccionado no tiene stock disponible en almacén.";
            }else{
                TxtMensaje.Text = "Selecciona la ubicación de la demarca.";
            }
        }

        private void MostrarStockDisponible()
        {
            if (CmbProducto.SelectedValue is not int productoId || CmbUbicacion.SelectedValue is not int ubicacionId){
                TxtStockDisponible.Text = "Stock disponible: -";
                return;
            }

            StockUbicacionDTO? stockProducto = _stock
                .FirstOrDefault(s => s.ProductoId == productoId && s.UbicacionId == ubicacionId);

            if (stockProducto == null) {
                TxtStockDisponible.Text = "Stock disponible: 0 unidades";
                return;
            }

            TxtStockDisponible.Text = "Stock disponible: " + stockProducto.Cantidad + " unidades";
        }

        private async void BtnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarFormulario()) {
                return;
            }

            BtnRegistrar.IsEnabled = false;
            TxtMensaje.Text = "Registrando demarca...";

            try {
                CreateDemarcaDTO demarca = new CreateDemarcaDTO();

                demarca.ProductoId = (int)CmbProducto.SelectedValue;
                demarca.UbicacionId = (int)CmbUbicacion.SelectedValue;
                demarca.Cantidad = int.Parse(TxtCantidad.Text);
                demarca.Motivo = TxtMotivo.Text.Trim();

                string mensaje = await _demarcaService.CrearDemarcaAsync(demarca);

                _stock = await _stockService.GetStockAsync();

                LimpiarFormulario();

                TxtMensaje.Text = mensaje;

            } finally{

                BtnRegistrar.IsEnabled = true;
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

            if (string.IsNullOrWhiteSpace(TxtMotivo.Text)) {
                TxtMensaje.Text = "El motivo es obligatorio.";
                return false;
            }

            if (CmbProducto.SelectedValue is int productoId && CmbUbicacion.SelectedValue is int ubicacionId){
                StockUbicacionDTO? stockProducto = _stock
                    .FirstOrDefault(s => s.ProductoId == productoId && s.UbicacionId == ubicacionId);

                if (stockProducto == null || stockProducto.Cantidad < cantidad) {
                    TxtMensaje.Text = "No hay stock suficiente en la ubicación seleccionada.";
                    return false;
                }
            }

            return true;
        }

        private void LimpiarFormulario()
        {
            CmbProducto.SelectedIndex = -1;

            CmbUbicacion.ItemsSource = null;
            CmbUbicacion.SelectedIndex = -1;

            CmbProducto.Text = "Seleccione un producto";
            CmbUbicacion.Text = "Seleccione una ubicación";

            TxtStockDisponible.Text = "Stock disponible: -";

            TxtCantidad.Text = "";
            TxtMotivo.Text = "";
        }
    }
}