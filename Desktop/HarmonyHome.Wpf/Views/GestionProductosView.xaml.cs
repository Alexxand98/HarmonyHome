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
    /// Lógica de interacción para GestionProductosView.xaml
    /// </summary>
    public partial class GestionProductosView : Window
    {
        private readonly ProductoService _productoService;
        private ProductoDTO? _productoSeleccionado;

        public GestionProductosView()
        {
            InitializeComponent();

            _productoService = new ProductoService();

            CargarTiposTrazabilidad();
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

        private async System.Threading.Tasks.Task CargarProductos()
        {
            TxtMensajeGestionProductos.Text = "Cargando productos...";

            List<ProductoDTO> productos = await _productoService.GetProductosAsync();

            TablaProductosGestion.ItemsSource = productos;

            TxtMensajeGestionProductos.Text = "Productos cargados: " + productos.Count;
        }

        private void TablaProductosGestion_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _productoSeleccionado = TablaProductosGestion.SelectedItem as ProductoDTO;

            if (_productoSeleccionado == null)
            {
                return;
            }

            TxtReferencia.Text = _productoSeleccionado.Referencia;
            TxtNombre.Text = _productoSeleccionado.Nombre;
            TxtDescripcion.Text = _productoSeleccionado.Descripcion;
            TxtCategoria.Text = _productoSeleccionado.Categoria;
            TxtPrecioCoste.Text = _productoSeleccionado.PrecioCoste.ToString();
            TxtPrecioVenta.Text = _productoSeleccionado.PrecioVenta.ToString();
            TxtStockMinimo.Text = _productoSeleccionado.StockMinimo.ToString();
            CmbTipoTrazabilidad.SelectedValue = _productoSeleccionado.TipoTrazabilidad;
            ChkHabilitado.IsChecked = _productoSeleccionado.Habilitado;
            ChkActivo.IsChecked = _productoSeleccionado.Activo;
            TxtImagenUrl.Text = _productoSeleccionado.ImagenUrl;
            TxtObservaciones.Text = _productoSeleccionado.Observaciones;
        }

        private void BtnNuevoProducto_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario();
        }

        private async void BtnGuardarProducto_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarFormulario())
            {
                return;
            }

            if (_productoSeleccionado == null)
            {
                CreateProductoDTO producto = CrearDtoProducto();

                bool creado = await _productoService.CrearProductoAsync(producto);

                if (creado)
                {
                    TxtMensajeGestionProductos.Text = "Producto creado correctamente.";
                    LimpiarFormulario();
                    await CargarProductos();
                }
                else
                {
                    TxtMensajeGestionProductos.Text = "No se pudo crear el producto.";
                }
            }
            else
            {
                TxtMensajeGestionProductos.Text = "Más adelante actualizaremos el producto seleccionado.";
            }
        }


        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(TxtReferencia.Text))
            {
                TxtMensajeGestionProductos.Text = "La referencia es obligatoria.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtNombre.Text))
            {
                TxtMensajeGestionProductos.Text = "El nombre es obligatorio.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtCategoria.Text))
            {
                TxtMensajeGestionProductos.Text = "La categoría es obligatoria.";
                return false;
            }

            if (!decimal.TryParse(TxtPrecioCoste.Text, out decimal precioCoste) || precioCoste < 0)
            {
                TxtMensajeGestionProductos.Text = "El precio de coste no es válido.";
                return false;
            }

            if (!decimal.TryParse(TxtPrecioVenta.Text, out decimal precioVenta) || precioVenta < 0)
            {
                TxtMensajeGestionProductos.Text = "El precio de venta no es válido.";
                return false;
            }

            if (!int.TryParse(TxtStockMinimo.Text, out int stockMinimo) || stockMinimo < 0)
            {
                TxtMensajeGestionProductos.Text = "El stock mínimo no es válido.";
                return false;
            }

            if (CmbTipoTrazabilidad.SelectedValue == null)
            {
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

            TxtMensajeGestionProductos.Text = "Formulario limpio.";
        }



    }

    public class OpcionComboDTO
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;
    }

}