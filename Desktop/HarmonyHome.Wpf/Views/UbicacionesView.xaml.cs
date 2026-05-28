using HarmonyHome.Wpf.Models.DTOs;
using HarmonyHome.Wpf.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace HarmonyHome.Wpf.Views
{
    public partial class UbicacionesView : Window
    {
        private readonly UbicacionService _ubicacionService;

        private List<UbicacionDTO> _ubicaciones = new List<UbicacionDTO>();

        public UbicacionesView()
        {
            InitializeComponent();

            _ubicacionService = new UbicacionService();

            Loaded += UbicacionesView_Loaded;
        }

        private async void UbicacionesView_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarUbicaciones();
        }

        private async void BtnActualizarUbicaciones_Click(object sender, RoutedEventArgs e)
        {
            await CargarUbicaciones();
        }

        private async Task CargarUbicaciones()
        {
            TxtMensajeUbicaciones.Text = "Cargando ubicaciones...";

            BtnActualizarUbicaciones.IsEnabled = false;

            try
            {
                _ubicaciones = await _ubicacionService.GetUbicacionesAsync();

                TablaUbicaciones.ItemsSource = _ubicaciones;

                if (_ubicaciones.Count == 0)
                {

                    TxtMensajeUbicaciones.Text = "No se encontraron ubicaciones";

                }
                else
                {

                    TxtMensajeUbicaciones.Text = "Ubicaciones cargadas: " + _ubicaciones.Count;
                }

            }
            finally
            {

                BtnActualizarUbicaciones.IsEnabled = true;
            }
        }

        private void BtnBuscarUbicacion_Click(object sender, RoutedEventArgs e)
        {
            string texto = TxtBuscarUbicacion.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(texto))
            {

                TablaUbicaciones.ItemsSource = _ubicaciones;

                TxtMensajeUbicaciones.Text = "Ubicaciones cargadas: " + _ubicaciones.Count;

                return;
            }

            List<UbicacionDTO> ubicacionesFiltradas = _ubicaciones.Where(u =>
                u.Codigo.ToLower().Contains(texto) ||
                u.Nombre.ToLower().Contains(texto) ||
                u.TipoUbicacionNombre.ToLower().Contains(texto)).ToList();

            TablaUbicaciones.ItemsSource = ubicacionesFiltradas;

            TxtMensajeUbicaciones.Text = "Resultados encontrados: " + ubicacionesFiltradas.Count;
        }

        private void BtnLimpiarBusquedaUbicacion_Click(object sender, RoutedEventArgs e)
        {
            TxtBuscarUbicacion.Text = "";

            TablaUbicaciones.ItemsSource = _ubicaciones;

            TxtMensajeUbicaciones.Text = "Búsqueda limpia";
        }
    }
}