using HarmonyHome.Wpf.Models.DTOs;
using HarmonyHome.Wpf.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HarmonyHome.Wpf.Views
{
    /// <summary>
    /// Lógica de interacción para GestionProductosView.xaml
    /// </summary>
    public partial class GestionProductosView : Window
    {
        private readonly ProductoService _productoService;

        private ProductoDTO? _productoSeleccionado;
        private List<ProductoDTO> _productos = new List<ProductoDTO>();

        public GestionProductosView()
        {
            InitializeComponent();

            _productoService = new ProductoService();

            CargarTiposTrazabilidad();

            Loaded += GestionProductosView_Loaded;
        }

        private async void GestionProductosView_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarProductos();
        }

        private void CargarTiposTrazabilidad()
        {
            List<OpcionComboDTO> tipos = new List<OpcionComboDTO>
            {
                new OpcionComboDTO { Id = 1, Nombre = "Sin trazabilidad" },
                new OpcionComboDTO { Id = 2, Nombre = "Lote" },
                new OpcionComboDTO { Id = 3, Nombre = "Unitaria" }
            };

            CmbTipoTrazabilidad.ItemsSource = tipos;
            CmbTipoTrazabilidad.SelectedValue = 1;
        }

        private async void BtnCargarProductosGestion_Click(object sender, RoutedEventArgs e)
        {
            await CargarProductos();
        }

        private async Task CargarProductos()
        {
            TxtMensajeGestionProductos.Text = "Cargando productos...";
            BtnCargarProductosGestion.IsEnabled = false;

            try {

                _productos = await _productoService.GetProductosAsync();

                TablaProductosGestion.ItemsSource = _productos;

                if (_productos.Count == 0) {

                    TxtMensajeGestionProductos.Text = "No se encontraron productos.";

                }else{

                    TxtMensajeGestionProductos.Text = "Productos cargados: " + _productos.Count;
                }

            }finally{

                BtnCargarProductosGestion.IsEnabled = true;
            }
        }

        private void TablaProductosGestion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _productoSeleccionado = TablaProductosGestion.SelectedItem as ProductoDTO;

            if (_productoSeleccionado == null) {

                return;
            }

            TxtReferencia.Text = _productoSeleccionado.Referencia;
            TxtNombre.Text = _productoSeleccionado.Nombre;
            TxtDescripcion.Text = _productoSeleccionado.Descripcion;
            TxtCategoria.Text = _productoSeleccionado.Categoria;
            TxtPrecioCoste.Text = _productoSeleccionado.PrecioCoste.ToString("0.00");
            TxtPrecioVenta.Text = _productoSeleccionado.PrecioVenta.ToString("0.00");
            TxtStockMinimo.Text = _productoSeleccionado.StockMinimo.ToString();
            CmbTipoTrazabilidad.SelectedValue = _productoSeleccionado.TipoTrazabilidad;
            ChkHabilitado.IsChecked = _productoSeleccionado.Habilitado;
            ChkActivo.IsChecked = _productoSeleccionado.Activo;
            TxtImagenUrl.Text = _productoSeleccionado.ImagenUrl;
            TxtObservaciones.Text = _productoSeleccionado.Observaciones;

            ConfigurarCheckActivo();
        }

        private void ConfigurarCheckActivo()
        {
            if (_productoSeleccionado == null)  {

                ChkActivo.IsChecked = true;
                ChkActivo.IsEnabled = false;
                ChkActivo.Content = "Producto activo";
                return;
            }

            if (_productoSeleccionado.Activo){

                ChkActivo.IsChecked = true;
                ChkActivo.IsEnabled = false;
                ChkActivo.Content = "Producto activo";
            }else {

                ChkActivo.IsChecked = false;
                ChkActivo.IsEnabled = true;
                ChkActivo.Content = "Reactivar producto";
            }
        }

        private void BtnNuevoProducto_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario();
        }

        private async void BtnGuardarProducto_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarFormulario()) {

                return;
            }

            BtnGuardarProducto.IsEnabled = false;
            TxtMensajeGestionProductos.Text = "Guardando producto...";

            try {
                if (_productoSeleccionado == null){

                    CreateProductoDTO producto = CrearDtoProducto();

                    bool creado = await _productoService.CrearProductoAsync(producto);

                    if (creado) {

                        TxtMensajeGestionProductos.Text = "Producto creado correctamente.";
                        LimpiarFormulario();
                        await CargarProductos();

                    }else {

                        TxtMensajeGestionProductos.Text = "No se pudo crear el producto.";
                    }

                }else {

                    UpdateProductoDTO productoEditado = CrearDtoUpdateProducto();

                    bool actualizado = await _productoService.ActualizarProductoAsync(_productoSeleccionado.Id, productoEditado);

                    if (actualizado) {

                        TxtMensajeGestionProductos.Text = "Producto actualizado correctamente.";
                        LimpiarFormulario();
                        await CargarProductos();
                    }  else {

                        TxtMensajeGestionProductos.Text = "No se pudo actualizar el producto.";
                    }
                }

            } finally {

                BtnGuardarProducto.IsEnabled = true;
            }
        }

        private async void BtnEliminarProducto_Click(object sender, RoutedEventArgs e)
        {
            if (_productoSeleccionado == null){

                TxtMensajeGestionProductos.Text = "Selecciona un producto.";
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                "¿Seguro que quieres eliminar este producto?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes){

                return;
            }

            BtnEliminarProducto.IsEnabled = false;
            TxtMensajeGestionProductos.Text = "Eliminando producto...";

            try{

                string mensaje = await _productoService.EliminarProductoAsync(_productoSeleccionado.Id);

                TxtMensajeGestionProductos.Text = mensaje;

                LimpiarFormulario();

                await CargarProductos();

            } finally {

                BtnEliminarProducto.IsEnabled = true;
            }
        }

        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(TxtReferencia.Text)) {

                TxtMensajeGestionProductos.Text = "La referencia es obligatoria.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtNombre.Text)) {

                TxtMensajeGestionProductos.Text = "El nombre es obligatorio.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtCategoria.Text)){

                TxtMensajeGestionProductos.Text = "La categoría es obligatoria.";
                return false;
            }

            if (!decimal.TryParse(TxtPrecioCoste.Text, out decimal precioCoste) || precioCoste < 0) {

                TxtMensajeGestionProductos.Text = "El precio de coste no es válido.";
                return false;
            }

            if (!decimal.TryParse(TxtPrecioVenta.Text, out decimal precioVenta) || precioVenta < 0) {

                TxtMensajeGestionProductos.Text = "El precio de venta no es válido.";
                return false;
            }

            if (!int.TryParse(TxtStockMinimo.Text, out int stockMinimo) || stockMinimo < 0) {

                TxtMensajeGestionProductos.Text = "El stock mínimo no es válido.";
                return false;
            }

            if (CmbTipoTrazabilidad.SelectedValue == null) {

                TxtMensajeGestionProductos.Text = "Selecciona un tipo de trazabilidad.";
                return false;
            }

            return true;
        }

        private CreateProductoDTO CrearDtoProducto()
        {
            CreateProductoDTO producto = new CreateProductoDTO();

            producto.Referencia = TxtReferencia.Text.Trim();
            producto.Nombre = TxtNombre.Text.Trim();
            producto.Descripcion = TxtDescripcion.Text.Trim();
            producto.Categoria = TxtCategoria.Text.Trim();
            producto.PrecioCoste = decimal.Parse(TxtPrecioCoste.Text);
            producto.PrecioVenta = decimal.Parse(TxtPrecioVenta.Text);
            producto.StockMinimo = int.Parse(TxtStockMinimo.Text);
            producto.TipoTrazabilidad = (int)CmbTipoTrazabilidad.SelectedValue;
            producto.Habilitado = ChkHabilitado.IsChecked == true;
            producto.ImagenUrl = TxtImagenUrl.Text.Trim();
            producto.Observaciones = TxtObservaciones.Text.Trim();

            return producto;
        }

        private UpdateProductoDTO CrearDtoUpdateProducto()
        {
            UpdateProductoDTO producto = new UpdateProductoDTO();

            producto.Referencia = TxtReferencia.Text.Trim();
            producto.Nombre = TxtNombre.Text.Trim();
            producto.Descripcion = TxtDescripcion.Text.Trim();
            producto.Categoria = TxtCategoria.Text.Trim();
            producto.PrecioCoste = decimal.Parse(TxtPrecioCoste.Text);
            producto.PrecioVenta = decimal.Parse(TxtPrecioVenta.Text);
            producto.StockMinimo = int.Parse(TxtStockMinimo.Text);
            producto.TipoTrazabilidad = (int)CmbTipoTrazabilidad.SelectedValue;
            producto.Habilitado = ChkHabilitado.IsChecked == true;

            if (_productoSeleccionado != null && _productoSeleccionado.Activo) {

                producto.Activo = true;

            }else {

                producto.Activo = ChkActivo.IsChecked == true;
            }

            producto.ImagenUrl = TxtImagenUrl.Text.Trim();
            producto.Observaciones = TxtObservaciones.Text.Trim();

            return producto;
        }

        private void LimpiarFormulario()
        {
            _productoSeleccionado = null;

            TxtReferencia.Text = "";
            TxtNombre.Text = "";
            TxtDescripcion.Text = "";
            TxtCategoria.Text = "";
            TxtPrecioCoste.Text = "";
            TxtPrecioVenta.Text = "";
            TxtStockMinimo.Text = "";
            CmbTipoTrazabilidad.SelectedValue = 1;
            ChkHabilitado.IsChecked = true;
            ChkActivo.IsChecked = true;
            TxtImagenUrl.Text = "";
            TxtObservaciones.Text = "";

            TablaProductosGestion.SelectedItem = null;

            ConfigurarCheckActivo();

            TxtMensajeGestionProductos.Text = "Formulario limpio.";
        }

        private async void BtnBuscarProducto_Click(object sender, RoutedEventArgs e)
        {
            string texto = TxtBuscarProducto.Text.Trim();

            if (string.IsNullOrWhiteSpace(texto)){

                TablaProductosGestion.ItemsSource = _productos;

                TxtMensajeGestionProductos.Text = "Productos cargados: " + _productos.Count;
                return;
            }

            List<ProductoDTO> productosFiltrados = await _productoService.BuscarProductosAsync(texto);

            TablaProductosGestion.ItemsSource = productosFiltrados;

            TxtMensajeGestionProductos.Text = "Resultados encontrados: " + productosFiltrados.Count;
        }

        private void BtnLimpiarBusquedaProducto_Click(object sender, RoutedEventArgs e)
        {
            TxtBuscarProducto.Text = "";

            TablaProductosGestion.ItemsSource = _productos;

            TxtMensajeGestionProductos.Text = "Búsqueda limpiada.";
        }
    }

    public class OpcionComboDTO
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;
    }
}