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
    /// Lógica de interacción para OrdenesRecogidaView.xaml
    /// </summary>
    public partial class OrdenesRecogidaView : Window
    {
        private readonly OrdenRecogidaService _ordenService;

        private OrdenRecogidaDTO? _ordenSeleccionada;

        private List<OrdenRecogidaDTO> _ordenes = new List<OrdenRecogidaDTO>();

        public OrdenesRecogidaView()
        {
            InitializeComponent();

            _ordenService = new OrdenRecogidaService();

            Loaded += OrdenesRecogidaView_Loaded;
        }

        private async void OrdenesRecogidaView_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarOrdenes();
        }

        private async void BtnActualizarOrdenes_Click(object sender, RoutedEventArgs e)
        {
            await CargarOrdenes();
        }

        private async Task CargarOrdenes()
        {
            TxtMensaje.Text = "Cargando órdenes...";
            BtnActualizarOrdenes.IsEnabled = false;

            try
            {
                _ordenes = await _ordenService.GetOrdenesAsync();

                _ordenSeleccionada = null;

                TablaOrdenes.ItemsSource = _ordenes;
                TablaLineas.ItemsSource = null;

                TxtBuscarOrden.Text = "";

                if (_ordenes.Count == 0)
                {
                    TxtMensaje.Text = "No se encontraron órdenes de recogida.";
                }
                else
                {
                    TxtMensaje.Text = "Órdenes cargadas: " + _ordenes.Count;
                }

            }
            finally
            {

                BtnActualizarOrdenes.IsEnabled = true;
            }
        }

        private void TablaOrdenes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _ordenSeleccionada = TablaOrdenes.SelectedItem as OrdenRecogidaDTO;

            if (_ordenSeleccionada == null)
            {
                TablaLineas.ItemsSource = null;
                return;
            }

            TablaLineas.ItemsSource = _ordenSeleccionada.LineasPedido;
        }

        private async void BtnAsignar_Click(object sender, RoutedEventArgs e)
        {
            if (_ordenSeleccionada == null)
            {
                TxtMensaje.Text = "Selecciona una orden.";
                return;
            }

            if (_ordenSeleccionada.EstadoNombre != "Pendiente")
            {
                TxtMensaje.Text = "Solo se puede asignar una orden pendiente.";
                return;
            }

            string mensaje = await _ordenService.AsignarAsync(_ordenSeleccionada.Id);

            await CargarOrdenes();

            TxtMensaje.Text = mensaje;
        }

        private async void BtnPreparacion_Click(object sender, RoutedEventArgs e)
        {
            if (_ordenSeleccionada == null)
            {
                TxtMensaje.Text = "Selecciona una orden.";
                return;
            }

            if (_ordenSeleccionada.EstadoNombre != "Asignada")
            {
                TxtMensaje.Text = "Solo se puede pasar a preparación una orden asignada.";
                return;
            }

            string mensaje = await _ordenService.IniciarPreparacionAsync(_ordenSeleccionada.Id);

            await CargarOrdenes();

            TxtMensaje.Text = mensaje;
        }

        private async void BtnFinalizar_Click(object sender, RoutedEventArgs e)
        {
            if (_ordenSeleccionada == null)
            {
                TxtMensaje.Text = "Selecciona una orden.";
                return;
            }

            if (_ordenSeleccionada.EstadoNombre != "EnPreparacion")
            {
                TxtMensaje.Text = "Solo se puede finalizar una orden en preparación.";
                return;
            }

            string mensaje = await _ordenService.FinalizarAsync(_ordenSeleccionada.Id);

            await CargarOrdenes();

            TxtMensaje.Text = mensaje;
        }

        private async void BtnVerPreparacion_Click(object sender, RoutedEventArgs e)
        {
            if (_ordenSeleccionada == null)
            {
                TxtMensaje.Text = "Selecciona una orden.";
                return;
            }

            if (_ordenSeleccionada.EstadoNombre != "EnPreparacion")
            {
                TxtMensaje.Text = "Solo se puede ver la preparación de una orden en preparación.";
                return;
            }

            PreparacionRecogidaDTO? preparacion = await _ordenService.GetPreparacionAsync(_ordenSeleccionada.Id);

            if (preparacion == null)
            {
                TxtMensaje.Text = "La orden no está disponible para preparación o ya está finalizada.";
                return;
            }

            PreparacionRecogidaView view = new PreparacionRecogidaView(preparacion);

            view.ShowDialog();
        }

        private async void BtnCancelarOrden_Click(object sender, RoutedEventArgs e)
        {
            if (_ordenSeleccionada == null)
            {
                TxtMensaje.Text = "Selecciona una orden.";
                return;
            }

            if (_ordenSeleccionada.EstadoNombre != "Asignada" && _ordenSeleccionada.EstadoNombre != "EnPreparacion")
            {
                TxtMensaje.Text = "Solo se pueden cancelar órdenes asignadas o en preparación.";
                return;
            }

            CancelarOrdenView cancelarView = new CancelarOrdenView();

            bool? resultado = cancelarView.ShowDialog();

            if (resultado != true)
            {
                return;
            }

            string mensaje = await _ordenService.CancelarAsync(_ordenSeleccionada.Id, cancelarView.MotivoCancelacion);

            await CargarOrdenes();

            TxtMensaje.Text = mensaje;
        }

        private void BtnBuscarOrden_Click(object sender, RoutedEventArgs e)
        {
            string texto = TxtBuscarOrden.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(texto))
            {
                TablaOrdenes.ItemsSource = _ordenes;
                TablaLineas.ItemsSource = null;
                _ordenSeleccionada = null;

                TxtMensaje.Text = "Órdenes cargadas: " + _ordenes.Count;

                return;
            }

            List<OrdenRecogidaDTO> ordenesFiltradas = _ordenes.Where(o =>
                o.Id.ToString().Contains(texto) ||
                o.PedidoVentaId.ToString().Contains(texto) ||
                (o.EstadoNombre != null && o.EstadoNombre.ToLower().Contains(texto)) ||
                (o.ClienteNombreCompleto != null && o.ClienteNombreCompleto.ToLower().Contains(texto)) ||
                (o.AsignadoTexto != null && o.AsignadoTexto.ToLower().Contains(texto))).ToList();

            TablaOrdenes.ItemsSource = ordenesFiltradas;
            TablaLineas.ItemsSource = null;
            _ordenSeleccionada = null;

            TxtMensaje.Text = "Resultados encontrados: " + ordenesFiltradas.Count;
        }

        private void BtnLimpiarBusquedaOrden_Click(object sender, RoutedEventArgs e)
        {
            TxtBuscarOrden.Text = "";

            TablaOrdenes.ItemsSource = _ordenes;
            TablaLineas.ItemsSource = null;
            _ordenSeleccionada = null;

            TxtMensaje.Text = "Búsqueda limpia.";
        }
    }
}