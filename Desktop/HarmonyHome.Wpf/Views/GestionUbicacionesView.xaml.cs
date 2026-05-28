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
        private readonly StockService _stockService;

        private UbicacionDTO? _ubicacionSeleccionada;
        private List<UbicacionDTO> _ubicaciones = new List<UbicacionDTO>();

        public GestionUbicacionesView()
        {
            InitializeComponent();

            _ubicacionService = new UbicacionService();
            _stockService = new StockService();

            CargarTiposUbicacion();

            Loaded += GestionUbicacionesView_Loaded;
        }

        private async void GestionUbicacionesView_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarUbicaciones();
        }

        private void CargarTiposUbicacion()
        {
            List<OpcionComboDTO> tipos = new List<OpcionComboDTO>
            {
                new OpcionComboDTO { Id = 1, Nombre = "Tienda" },
                new OpcionComboDTO { Id = 2, Nombre = "Almacén" },
                new OpcionComboDTO { Id = 3, Nombre = "Recepción" },
                new OpcionComboDTO { Id = 4, Nombre = "Preparación" },
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
            BtnCargarUbicaciones.IsEnabled = false;

            try  {
                _ubicaciones = await _ubicacionService.GetUbicacionesAsync();

                TablaUbicaciones.ItemsSource = _ubicaciones;

                if (_ubicaciones.Count == 0) {

                    TxtMensajeGestionUbicaciones.Text = "No se encontraron ubicaciones.";
                } else{

                    TxtMensajeGestionUbicaciones.Text = "Ubicaciones cargadas: " + _ubicaciones.Count;
                }

            } finally {

                BtnCargarUbicaciones.IsEnabled = true;
            }
        }

        private void BtnBuscarUbicacion_Click(object sender, RoutedEventArgs e)
        {
            string texto = TxtBuscarUbicacion.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(texto)){

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

            if (_ubicacionSeleccionada == null) {

                return;
            }

            TxtCodigo.Text = _ubicacionSeleccionada.Codigo;
            TxtNombre.Text = _ubicacionSeleccionada.Nombre;
            CmbTipoUbicacion.SelectedValue = _ubicacionSeleccionada.TipoUbicacion;
            ChkActiva.IsChecked = _ubicacionSeleccionada.Activa;

            ConfigurarCheckActiva();
        }

        private void ConfigurarCheckActiva()
        {
            if (_ubicacionSeleccionada == null) {

                ChkActiva.IsChecked = true;
                ChkActiva.IsEnabled = false;
                ChkActiva.Content = "Ubicación activa";
                return;
            }

            if (_ubicacionSeleccionada.Activa) {

                ChkActiva.IsChecked = true;
                ChkActiva.IsEnabled = false;
                ChkActiva.Content = "Ubicación activa";
            }  else  {

                ChkActiva.IsChecked = false;
                ChkActiva.IsEnabled = true;
                ChkActiva.Content = "Reactivar ubicación";
            }
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

            ConfigurarCheckActiva();

            TxtMensajeGestionUbicaciones.Text = "Formulario limpio.";
        }

        private async void BtnGuardarUbicacion_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarFormulario()) {

                return;
            }

            BtnGuardarUbicacion.IsEnabled = false;
            TxtMensajeGestionUbicaciones.Text = "Guardando ubicación...";

            try {

                if (_ubicacionSeleccionada == null) {

                    CreateUbicacionDTO nuevaUbicacion = CrearDtoUbicacion();

                    bool creada = await _ubicacionService.CrearUbicacionAsync(nuevaUbicacion);

                    if (creada)  {

                        TxtMensajeGestionUbicaciones.Text = "Ubicación creada correctamente.";
                        LimpiarFormulario();

                        await CargarUbicaciones();
                    }  else {

                        TxtMensajeGestionUbicaciones.Text = "No se pudo crear la ubicación.";
                    }

                }  else {

                    UpdateUbicacionDTO ubicacionEditada = CrearDtoUpdateUbicacion();

                    bool actualizada = await _ubicacionService.ActualizarUbicacionAsync(_ubicacionSeleccionada.Id, ubicacionEditada);

                    if (actualizada) {

                        TxtMensajeGestionUbicaciones.Text = "Ubicación actualizada correctamente.";

                        LimpiarFormulario();

                        await CargarUbicaciones();
                    }  else  {
                        TxtMensajeGestionUbicaciones.Text = "No se pudo actualizar la ubicación.";
                    }
                }

            }finally {

                BtnGuardarUbicacion.IsEnabled = true;
            }
        }

        private async void BtnEliminarUbicacion_Click(object sender, RoutedEventArgs e)
        {
            if (_ubicacionSeleccionada == null) {

                TxtMensajeGestionUbicaciones.Text = "Selecciona una ubicación.";
                return;
            }

            List<StockUbicacionDTO> stockUbicacion = await _stockService.GetStockByUbicacionAsync(_ubicacionSeleccionada.Id);

            bool tieneStockActivo = stockUbicacion.Any(s => s.Cantidad > 0);

            if (tieneStockActivo) {

                TxtMensajeGestionUbicaciones.Text = "No se puede eliminar la ubicación porque tiene stock activo.";
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                "¿Seguro que quieres eliminar esta ubicación?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) {

                return;
            }

            BtnEliminarUbicacion.IsEnabled = false;
            TxtMensajeGestionUbicaciones.Text = "Eliminando ubicación...";

            try {
                string mensaje = await _ubicacionService.EliminarUbicacionAsync(_ubicacionSeleccionada.Id);

                LimpiarFormulario();

                await CargarUbicaciones();

                TxtMensajeGestionUbicaciones.Text = mensaje;

            }finally   {

                BtnEliminarUbicacion.IsEnabled = true;
            }
        }

        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(TxtCodigo.Text))
            {
                TxtMensajeGestionUbicaciones.Text = "El código es obligatorio.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtNombre.Text))
            {
                TxtMensajeGestionUbicaciones.Text = "El nombre es obligatorio.";
                return false;
            }

            if (CmbTipoUbicacion.SelectedValue == null)
            {
                TxtMensajeGestionUbicaciones.Text = "Selecciona tipo de ubicación.";
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
            ubicacion.Activa = true;

            return ubicacion;
        }

        private UpdateUbicacionDTO CrearDtoUpdateUbicacion()
        {
            UpdateUbicacionDTO ubicacion = new UpdateUbicacionDTO();

            ubicacion.Codigo = TxtCodigo.Text.Trim();
            ubicacion.Nombre = TxtNombre.Text.Trim();
            ubicacion.TipoUbicacion = (int)CmbTipoUbicacion.SelectedValue;

            if (_ubicacionSeleccionada != null && _ubicacionSeleccionada.Activa)
            {
                ubicacion.Activa = true;
            }
            else
            {
                ubicacion.Activa = ChkActiva.IsChecked == true;
            }

            return ubicacion;
        }
    }
}