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
    /// Lógica de interacción para OrdenesReposicionView.xaml
    /// </summary>
    public partial class OrdenesReposicionView : Window
    {
        private readonly OrdenReposicionService _ordenService;

        private OrdenReposicionDTO? _ordenSeleccionada;

        private List<OrdenReposicionDTO> _ordenes = new List<OrdenReposicionDTO>();

        public OrdenesReposicionView()
        {
            InitializeComponent();

            _ordenService = new OrdenReposicionService();
        }

        private async void BtnCargarOrdenes_Click(object sender, RoutedEventArgs e)
        {
            await CargarOrdenes();
        }

        private async void BtnCargarPendientes_Click(object sender, RoutedEventArgs e)
        {
            TxtMensaje.Text = "Cargando reposiciones pendientes...";

            _ordenes = await _ordenService.GetPendientesAsync();

            TablaOrdenes.ItemsSource = _ordenes;

            TablaLineas.ItemsSource = null;

            TxtMensaje.Text = "Reposiciones pendientes cargadas: " + _ordenes.Count;
        }

        private async Task CargarOrdenes()
        {
            TxtMensaje.Text = "Cargando reposiciones...";

            _ordenes = await _ordenService.GetOrdenesAsync();

            TablaOrdenes.ItemsSource = _ordenes;

            TablaLineas.ItemsSource = null;

            TxtMensaje.Text = "Reposiciones cargadas: " + _ordenes.Count;
        }

        private void TablaOrdenes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _ordenSeleccionada = TablaOrdenes.SelectedItem as OrdenReposicionDTO;

            if (_ordenSeleccionada == null){

                TablaLineas.ItemsSource = null;
                return;
            }

            TablaLineas.ItemsSource = _ordenSeleccionada.Lineas;
        }

        private async void BtnAsignar_Click(object sender, RoutedEventArgs e)
        {
            if (_ordenSeleccionada == null){

                TxtMensaje.Text = "Selecciona una reposición";
                return;
            }

            string mensaje = await _ordenService.AsignarAsync(_ordenSeleccionada.Id);

            await CargarOrdenes();

            TxtMensaje.Text = mensaje;
        }

        private async void BtnFinalizar_Click(object sender, RoutedEventArgs e)
        {
            if (_ordenSeleccionada == null){

                TxtMensaje.Text = "Selecciona una reposición";
                return;
            }

            if (_ordenSeleccionada.EstadoNombre != "Asignada"){

                TxtMensaje.Text = "Solo se puede finalizar una reposición asignada";
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
                TxtMensaje.Text = "Selecciona una reposición";
                return;
            }

            if (_ordenSeleccionada.EstadoNombre != "Asignada") {

                TxtMensaje.Text = "La orden no está disponible para preparación o ya está finalizada";
                return;
            }

            PreparacionReposicionDTO? preparacion = await _ordenService.GetPreparacionAsync(_ordenSeleccionada.Id);

            if (preparacion == null){
                TxtMensaje.Text = "La orden no está disponible para preparación o ya está finalizada";
                return;
            }

            PreparacionReposicionView view = new PreparacionReposicionView(preparacion);

            view.ShowDialog();
        }


        private async void BtnCancelarOrden_Click(object sender, RoutedEventArgs e)
        {
            if (_ordenSeleccionada == null){

                TxtMensaje.Text = "Selecciona una reposición";
                return;
            }

            if (_ordenSeleccionada.EstadoNombre != "Asignada" && _ordenSeleccionada.EstadoNombre != "EnPreparacion"){

                TxtMensaje.Text = "Solo se pueden cancelar reposiciones asignadas o en preparación";
                return;
            }

            CancelarOrdenView cancelarView = new CancelarOrdenView();

            bool? resultado = cancelarView.ShowDialog();

            if (resultado != true) {
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
                TxtMensaje.Text = "Reposiciones cargadas: " + _ordenes.Count;
                return;
            }

            List<OrdenReposicionDTO> ordenesFiltradas = _ordenes
                .Where(o =>
                    o.Id.ToString().Contains(texto) ||
                    (o.EstadoNombre != null && o.EstadoNombre.ToLower().Contains(texto)) ||
                    (o.SolicitanteTexto != null && o.SolicitanteTexto.ToLower().Contains(texto)) ||
                    (o.PreparadorTexto != null && o.PreparadorTexto.ToLower().Contains(texto)))
                .ToList();

            TablaOrdenes.ItemsSource = ordenesFiltradas;

            TxtMensaje.Text = "Resultados encontrados: " + ordenesFiltradas.Count;
        }

        private void BtnLimpiarBusquedaOrden_Click(object sender, RoutedEventArgs e)
        {
            TxtBuscarOrden.Text = "";

            TablaOrdenes.ItemsSource = _ordenes;

            TxtMensaje.Text = "Búsqueda limpia";
        }
    }
}