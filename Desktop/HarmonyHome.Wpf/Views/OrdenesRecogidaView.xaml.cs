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
    /// Lógica de interacción para OrdenesRecogidaView.xaml
    /// </summary>
    public partial class OrdenesRecogidaView : Window
    {
        private readonly OrdenRecogidaService _ordenService;
        private OrdenRecogidaDTO? _ordenSeleccionada;

        public OrdenesRecogidaView()
        {
            InitializeComponent();

            _ordenService = new OrdenRecogidaService();
        }

        private async void BtnCargarOrdenes_Click(object sender, RoutedEventArgs e)
        {
            await CargarOrdenes();
        }

        private async void BtnCargarPendientes_Click(object sender, RoutedEventArgs e)
        {
            TxtMensaje.Text = "Cargando ordenes pendientes...";

            List<OrdenRecogidaDTO> ordenes = await _ordenService.GetPendientesAsync();

            TablaOrdenes.ItemsSource = ordenes;

            TablaLineas.ItemsSource = null;

            TxtMensaje.Text = "Ordenes pendientes cargadas: " + ordenes.Count;
        }

        private async Task CargarOrdenes()
        {
            TxtMensaje.Text = "Cargando ordenes...";

            List<OrdenRecogidaDTO> ordenes = await _ordenService.GetOrdenesAsync();

            TablaOrdenes.ItemsSource = ordenes;

            TablaLineas.ItemsSource = null;

            TxtMensaje.Text = "Ordenes cargadas: " + ordenes.Count;
        }

        private void TablaOrdenes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _ordenSeleccionada = TablaOrdenes.SelectedItem as OrdenRecogidaDTO;

            if (_ordenSeleccionada == null){

                TablaLineas.ItemsSource = null;
                return;
            }

            TablaLineas.ItemsSource = _ordenSeleccionada.LineasPedido;
        }

        private async void BtnAsignar_Click(object sender, RoutedEventArgs e)
        {
            if (_ordenSeleccionada == null){

                TxtMensaje.Text = "Selecciona una orden";
                return;
            }

            string mensaje = await _ordenService.AsignarAsync(_ordenSeleccionada.Id);

            await CargarOrdenes();

            TxtMensaje.Text = mensaje;
        }

        private async void BtnPreparacion_Click(object sender, RoutedEventArgs e)
        {
            if (_ordenSeleccionada == null){

                TxtMensaje.Text = "Selecciona una orden";

                return;
            }

            string mensaje = await _ordenService.IniciarPreparacionAsync(_ordenSeleccionada.Id);

            await CargarOrdenes();

            TxtMensaje.Text = mensaje;
        }

        private async void BtnFinalizar_Click(object sender, RoutedEventArgs e)
        {
            if (_ordenSeleccionada == null){

                TxtMensaje.Text = "Selecciona una orden";
                return;
            }

            string mensaje = await _ordenService.FinalizarAsync(_ordenSeleccionada.Id);

            await CargarOrdenes();

            TxtMensaje.Text = mensaje;
        }
    }
}