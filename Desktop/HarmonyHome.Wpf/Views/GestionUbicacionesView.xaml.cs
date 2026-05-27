using HarmonyHome.Wpf.Models.DTOs;
using HarmonyHome.Wpf.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HarmonyHome.Wpf.Views
{
    public partial class GestionUbicacionesView : Window
    {
        private readonly UbicacionService _ubicacionService;

        private UbicacionDTO? _ubicacionSeleccionada;

        private List<UbicacionDTO> _ubicaciones = new List<UbicacionDTO>();

        public GestionUbicacionesView()
        {
            InitializeComponent();

            _ubicacionService = new UbicacionService();

            CargarTiposUbicacion();
        }

        private void CargarTiposUbicacion()
        {
            List<OpcionComboDTO> tipos = new List<OpcionComboDTO>
            {
                new OpcionComboDTO { Id = 1, Nombre = "Tienda" },
                new OpcionComboDTO { Id = 2, Nombre = "Almacen" },
                new OpcionComboDTO { Id = 3, Nombre = "Recepcion" },
                new OpcionComboDTO { Id = 4, Nombre = "Preparacion" },
                new OpcionComboDTO { Id = 5, Nombre = "Recogida" },
                new OpcionComboDTO { Id = 6, Nombre = "Demarca" }
            };

            CmbTipoUbicacion.ItemsSource = tipos;

            CmbTipoUbicacion.SelectedValue = 2;
        }

        private async void BtnCargarUbicaciones_Click(object sender, RoutedEventArgs e)
        {
            await CargarUbicaciones();
        }

        private async Task CargarUbicaciones()
        {
            TxtMensajeGestionUbicaciones.Text = "Cargando ubicaciones...";

            _ubicaciones = await _ubicacionService.GetUbicacionesAsync();

            TablaUbicaciones.ItemsSource = _ubicaciones;

            TxtMensajeGestionUbicaciones.Text = "Ubicaciones cargadas: " + _ubicaciones.Count;
        }

        private void BtnBuscarUbicacion_Click(object sender, RoutedEventArgs e)
        {
            string texto = TxtBuscarUbicacion.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(texto))
            {
                TablaUbicaciones.ItemsSource = _ubicaciones;
                TxtMensajeGestionUbicaciones.Text = "Ubicaciones cargadas: " + _ubicaciones.Count;
                return;
            }

            List<UbicacionDTO> ubicacionesFiltradas = _ubicaciones
                .Where(u =>
                    u.Codigo.ToLower().Contains(texto) ||
                    u.Nombre.ToLower().Contains(texto) ||
                    u.TipoUbicacionNombre.ToLower().Contains(texto))
                .ToList();

            TablaUbicaciones.ItemsSource = ubicacionesFiltradas;

            TxtMensajeGestionUbicaciones.Text = "Resultados encontrados: " + ubicacionesFiltradas.Count;
        }

        private void BtnLimpiarBusquedaUbicacion_Click(object sender, RoutedEventArgs e)
        {
            TxtBuscarUbicacion.Text = "";

            TablaUbicaciones.ItemsSource = _ubicaciones;

            TxtMensajeGestionUbicaciones.Text = "Búsqueda limpiada.";
        }

        private void TablaUbicaciones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _ubicacionSeleccionada = TablaUbicaciones.SelectedItem as UbicacionDTO;

            if (_ubicacionSeleccionada == null)
            {
                return;
            }

            TxtCodigo.Text = _ubicacionSeleccionada.Codigo;
            TxtNombre.Text = _ubicacionSeleccionada.Nombre;
            CmbTipoUbicacion.SelectedValue = _ubicacionSeleccionada.TipoUbicacion;
            ChkActiva.IsChecked = _ubicacionSeleccionada.Activa;
        }

        private void BtnNuevaUbicacion_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario();
        }

        private void LimpiarFormulario()
        {
            _ubicacionSeleccionada = null;

            TxtCodigo.Text = "";
            TxtNombre.Text = "";

            CmbTipoUbicacion.SelectedValue = 2;
            ChkActiva.IsChecked = true;

            TablaUbicaciones.SelectedItem = null;

            TxtMensajeGestionUbicaciones.Text = "Formulario limpio.";
        }

        private async void BtnGuardarUbicacion_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarFormulario()){

                return;
            }

            if (_ubicacionSeleccionada == null){

                CreateUbicacionDTO nuevaUbicacion = CrearDtoUbicacion();

                bool creada = await _ubicacionService.CrearUbicacionAsync(nuevaUbicacion);

                if (creada){

                    TxtMensajeGestionUbicaciones.Text = "Ubicacion creada correctamente.";
                    LimpiarFormulario();

                    await CargarUbicaciones();

                } else{

                    TxtMensajeGestionUbicaciones.Text = "No se pudo crear la ubicacion.";
                }

            }else{

                UpdateUbicacionDTO ubicacionEditada = CrearDtoUpdateUbicacion();

                bool actualizada = await _ubicacionService.ActualizarUbicacionAsync(_ubicacionSeleccionada.Id, ubicacionEditada);

                if (actualizada){

                    TxtMensajeGestionUbicaciones.Text = "Ubicacion actualizada correctamente.";

                    LimpiarFormulario();

                    await CargarUbicaciones();

                }else{

                    TxtMensajeGestionUbicaciones.Text = "No se pudo actualizar la ubicacion.";
                }
            }
        }

        private async void BtnEliminarUbicacion_Click(object sender, RoutedEventArgs e)
        {
            if (_ubicacionSeleccionada == null){

                TxtMensajeGestionUbicaciones.Text = "Selecciona una ubicacion.";

                return;
            }

            MessageBoxResult result = MessageBox.Show(
                "¿Seguro que quieres eliminar esta ubicación?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result != MessageBoxResult.Yes){

                return;
            }

            string mensaje = await _ubicacionService.EliminarUbicacionAsync(_ubicacionSeleccionada.Id);

            LimpiarFormulario();

            await CargarUbicaciones();

            TxtMensajeGestionUbicaciones.Text = mensaje;
        }

        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(TxtCodigo.Text)){

                TxtMensajeGestionUbicaciones.Text = "El código es obligatorio.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtNombre.Text)) {

                TxtMensajeGestionUbicaciones.Text = "El nombre es obligatorio.";
                return false;
            }

            if (CmbTipoUbicacion.SelectedValue == null) {

                TxtMensajeGestionUbicaciones.Text = "Selecciona tipo de ubicacion.";
                return false;
            }

            return true;
        }

        private CreateUbicacionDTO CrearDtoUbicacion()
        {
            CreateUbicacionDTO ubicacion = new CreateUbicacionDTO();

            ubicacion.Codigo = TxtCodigo.Text.Trim();

            ubicacion.Nombre = TxtNombre.Text.Trim();

            ubicacion.TipoUbicacion = (int)CmbTipoUbicacion.SelectedValue;

            ubicacion.Activa = ChkActiva.IsChecked == true;

            return ubicacion;
        }

        private UpdateUbicacionDTO CrearDtoUpdateUbicacion()
        {
            UpdateUbicacionDTO ubicacion = new UpdateUbicacionDTO();

            ubicacion.Codigo = TxtCodigo.Text.Trim();

            ubicacion.Nombre = TxtNombre.Text.Trim();

            ubicacion.TipoUbicacion = (int)CmbTipoUbicacion.SelectedValue;

            ubicacion.Activa = ChkActiva.IsChecked == true;

            return ubicacion;
        }
    }
}