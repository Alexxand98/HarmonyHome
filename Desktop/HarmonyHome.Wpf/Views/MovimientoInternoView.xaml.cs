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
    /// Lógica de interacción para MovimientoInternoView.xaml
    /// </summary>
    public partial class MovimientoInternoView : Window
    {
        private readonly ProductoService _productoService;
        private readonly UbicacionService _ubicacionService;
        private readonly StockService _stockService;

        private List<ProductoDTO> _productos = new List<ProductoDTO>();
        private List<UbicacionDTO> _ubicaciones = new List<UbicacionDTO>();
        private List<StockUbicacionDTO> _stock = new List<StockUbicacionDTO>();

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

            _productos = await _productoService.GetProductosAsync();
            _productos = _productos.Where(p => p.Activo && p.Habilitado).ToList();

            _ubicaciones = await _ubicacionService.GetUbicacionesPorTipoAsync(2);
            _stock = await _stockService.GetStockAsync();

            CmbProducto.ItemsSource = _productos;

            CmbOrigen.ItemsSource = null;
            CmbDestino.ItemsSource = _ubicaciones;

            LimpiarFormulario();

            TxtMensaje.Text = "Datos cargados.";
        }

        private void CmbProducto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarUbicacionesOrigenPorProducto();
        }

        private void CmbOrigen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MostrarStockDisponibleOrigen();
            FiltrarUbicacionesDestino();
        }

        private void CargarUbicacionesOrigenPorProducto()
        {
            if (CmbProducto.SelectedValue is not int productoId)
            {
                CmbOrigen.ItemsSource = null;
                CmbOrigen.SelectedIndex = -1;
                CmbOrigen.Text = "Seleccione ubicación de origen";

                CmbDestino.ItemsSource = _ubicaciones;
                CmbDestino.SelectedIndex = -1;
                CmbDestino.Text = "Seleccione ubicación de destino";

                TxtStockDisponible.Text = "Stock disponible en origen: -";
                return;
            }

            List<int> ubicacionesConStock = _stock
                .Where(s => s.ProductoId == productoId && s.Cantidad > 0)
                .Select(s => s.UbicacionId)
                .Distinct()
                .ToList();

            List<UbicacionDTO> ubicacionesOrigen = _ubicaciones
                .Where(u => ubicacionesConStock.Contains(u.Id))
                .ToList();

            CmbOrigen.ItemsSource = ubicacionesOrigen;
            CmbOrigen.SelectedIndex = -1;
            CmbOrigen.Text = "Seleccione ubicación de origen";

            CmbDestino.ItemsSource = _ubicaciones;
            CmbDestino.SelectedIndex = -1;
            CmbDestino.Text = "Seleccione ubicación de destino";

            TxtStockDisponible.Text = "Stock disponible en origen: -";

            if (ubicacionesOrigen.Count == 0)
            {
                TxtMensaje.Text = "El producto seleccionado no tiene stock disponible en almacén.";
            }
            else
            {
                TxtMensaje.Text = "Selecciona la ubicación de origen.";
            }
        }

        private void MostrarStockDisponibleOrigen()
        {
            if (CmbProducto.SelectedValue is not int productoId || CmbOrigen.SelectedValue is not int ubicacionOrigenId)
            {
                TxtStockDisponible.Text = "Stock disponible en origen: -";
                return;
            }

            StockUbicacionDTO? stockProducto = _stock
                .FirstOrDefault(s => s.ProductoId == productoId && s.UbicacionId == ubicacionOrigenId);

            if (stockProducto == null)
            {
                TxtStockDisponible.Text = "Stock disponible en origen: 0 unidades";
                return;
            }

            TxtStockDisponible.Text = "Stock disponible en origen: " + stockProducto.Cantidad + " unidades";
        }

        private void FiltrarUbicacionesDestino()
        {
            if (CmbOrigen.SelectedValue is not int ubicacionOrigenId)
            {
                CmbDestino.ItemsSource = _ubicaciones;
                CmbDestino.SelectedIndex = -1;
                CmbDestino.Text = "Seleccione ubicación de destino";
                return;
            }

            List<UbicacionDTO> ubicacionesDestino = _ubicaciones
                .Where(u => u.Id != ubicacionOrigenId)
                .ToList();

            CmbDestino.ItemsSource = ubicacionesDestino;
            CmbDestino.SelectedIndex = -1;
            CmbDestino.Text = "Seleccione ubicación de destino";
        }

        private async void BtnMover_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarFormulario())
            {
                return;
            }

            BtnMover.IsEnabled = false;
            TxtMensaje.Text = "Moviendo stock...";

            try
            {
                CreateMovimientoInternoDTO movimiento = new CreateMovimientoInternoDTO();

                movimiento.ProductoId = (int)CmbProducto.SelectedValue;
                movimiento.UbicacionOrigenId = (int)CmbOrigen.SelectedValue;
                movimiento.UbicacionDestinoId = (int)CmbDestino.SelectedValue;
                movimiento.Cantidad = int.Parse(TxtCantidad.Text);
                movimiento.Observaciones = TxtObservaciones.Text.Trim();

                string mensaje = await _stockService.MoverStockAsync(movimiento);

                _stock = await _stockService.GetStockAsync();

                LimpiarFormulario();

                TxtMensaje.Text = mensaje;

            }
            finally
            {

                BtnMover.IsEnabled = true;
            }
        }

        private bool ValidarFormulario()
        {
            if (CmbProducto.SelectedValue == null)
            {
                TxtMensaje.Text = "Selecciona un producto.";
                return false;
            }

            if (CmbOrigen.SelectedValue == null)
            {
                TxtMensaje.Text = "Selecciona la ubicación origen.";
                return false;
            }

            if (CmbDestino.SelectedValue == null)
            {
                TxtMensaje.Text = "Selecciona la ubicación destino.";
                return false;
            }

            if ((int)CmbOrigen.SelectedValue == (int)CmbDestino.SelectedValue)
            {
                TxtMensaje.Text = "La ubicación origen y destino no pueden ser la misma.";
                return false;
            }

            if (!int.TryParse(TxtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                TxtMensaje.Text = "La cantidad debe ser mayor que cero.";
                return false;
            }

            if (CmbProducto.SelectedValue is int productoId && CmbOrigen.SelectedValue is int ubicacionOrigenId)
            {
                StockUbicacionDTO? stockProducto = _stock
                    .FirstOrDefault(s => s.ProductoId == productoId && s.UbicacionId == ubicacionOrigenId);

                if (stockProducto == null || stockProducto.Cantidad < cantidad)
                {
                    TxtMensaje.Text = "No hay stock suficiente en la ubicación de origen.";
                    return false;
                }
            }

            return true;
        }

        private void LimpiarFormulario()
        {
            CmbProducto.SelectedIndex = -1;

            CmbOrigen.ItemsSource = null;
            CmbOrigen.SelectedIndex = -1;

            CmbDestino.ItemsSource = _ubicaciones;
            CmbDestino.SelectedIndex = -1;

            CmbProducto.Text = "Seleccione un producto";
            CmbOrigen.Text = "Seleccione ubicación de origen";
            CmbDestino.Text = "Seleccione ubicación de destino";

            TxtStockDisponible.Text = "Stock disponible en origen: -";

            TxtCantidad.Text = "";
            TxtObservaciones.Text = "";
        }
    }
}