using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using HarmonyHome.Wpf.Services;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HarmonyHome.Wpf.Models.DTOs;


namespace HarmonyHome.Wpf.Views
{
    /// <summary>
    /// Lógica de interacción para GestionLogisticosView.xaml
    /// </summary>
    public partial class GestionLogisticosView : Window
    {
        private readonly UsuarioLogisticoService _usuarioService;
        private UsuarioLogisticoDTO? _usuarioSeleccionado;

        public GestionLogisticosView()
        {
            InitializeComponent();

            _usuarioService = new UsuarioLogisticoService();
        }

        private async void BtnCargarLogisticos_Click(object sender, RoutedEventArgs e)
        {
            await CargarLogisticos();
        }

        private async Task CargarLogisticos()
        {
            TxtMensajeGestionLogisticos.Text = "Cargando logisticos...";

            List<UsuarioLogisticoDTO> logisticos = await _usuarioService.GetLogisticosAsync();

            TablaLogisticos.ItemsSource = logisticos;

            TxtMensajeGestionLogisticos.Text = "Logisticos cargados: " + logisticos.Count;
        }

        private void TablaLogisticos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _usuarioSeleccionado = TablaLogisticos.SelectedItem as UsuarioLogisticoDTO;

            if (_usuarioSeleccionado == null) {
                return;
            }

            TxtUserName.Text = _usuarioSeleccionado.UserName;
            TxtEmail.Text = _usuarioSeleccionado.Email;
            TxtNombreCompleto.Text = _usuarioSeleccionado.NombreCompleto;
            TxtPassword.Password = "";
            ChkActivo.IsChecked = _usuarioSeleccionado.Activo;
        }

        private void BtnNuevoLogistico_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario();
        }

        private async void BtnGuardarLogistico_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarFormulario())  {
                return;
            }

            if (_usuarioSeleccionado == null) {
                CreateUsuarioLogisticoDTO nuevoUsuario = CrearDtoUsuario();

                bool creado = await _usuarioService.CrearLogisticoAsync(nuevoUsuario);

                if (creado) {
                    TxtMensajeGestionLogisticos.Text = "Logístico creado correctamente.";

                    LimpiarFormulario();
                    await CargarLogisticos();
                } else{
                    TxtMensajeGestionLogisticos.Text = "No se pudo crear el logístico.";
                }
            } else{
                UpdateUsuarioLogisticoDTO usuarioEditado = CrearDtoUpdateUsuario();

                bool actualizado = await _usuarioService.ActualizarLogisticoAsync(_usuarioSeleccionado.Id, usuarioEditado);

                if (actualizado){
                    TxtMensajeGestionLogisticos.Text = "Logístico actualizado correctamente.";
                    LimpiarFormulario();
                    await CargarLogisticos();
                } else {
                    TxtMensajeGestionLogisticos.Text = "No se pudo actualizar el logístico.";
                }
            }
        }

        private async void BtnActivarLogistico_Click(object sender, RoutedEventArgs e)
        {
            if (_usuarioSeleccionado == null){
                TxtMensajeGestionLogisticos.Text = "Selecciona un logístico.";
                return;
            }

            bool activado = await _usuarioService.ActivarLogisticoAsync(_usuarioSeleccionado.Id);

            if (activado) {
                await CargarLogisticos();
                TxtMensajeGestionLogisticos.Text = "Logístico activado correctamente.";
            } else{
                TxtMensajeGestionLogisticos.Text = "No se pudo activar el logístico.";
            }
        }

        private async void BtnDesactivarLogistico_Click(object sender, RoutedEventArgs e)
        {
            if (_usuarioSeleccionado == null){
                TxtMensajeGestionLogisticos.Text = "Selecciona un logístico.";
                return;
            }

            bool desactivado = await _usuarioService.DesactivarLogisticoAsync(_usuarioSeleccionado.Id);

            if (desactivado){
                await CargarLogisticos();
                TxtMensajeGestionLogisticos.Text = "Logistico desactivado correctamente.";
            } else{
                TxtMensajeGestionLogisticos.Text = "No se pudo desactivar el logístico.";
            }
        }

        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(TxtUserName.Text)) {
                TxtMensajeGestionLogisticos.Text = "El usuario es obligatorio";
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtEmail.Text)){
                TxtMensajeGestionLogisticos.Text = "El email es obligatorio";
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtNombreCompleto.Text)) {
                TxtMensajeGestionLogisticos.Text = "El nombre completo es obligatorio";
                return false;
            }

            if (_usuarioSeleccionado == null && string.IsNullOrWhiteSpace(TxtPassword.Password)){
                TxtMensajeGestionLogisticos.Text = "La contraseña es obligatoria al crear";
                return false;
            }

            if (_usuarioSeleccionado == null && TxtPassword.Password.Length < 6) {
                TxtMensajeGestionLogisticos.Text = "La contraseña debe tener al menos 6 caracteres";
                return false;
            }

            return true;
        }

        private CreateUsuarioLogisticoDTO CrearDtoUsuario()
        {
            CreateUsuarioLogisticoDTO usuario = new CreateUsuarioLogisticoDTO();

            usuario.UserName = TxtUserName.Text.Trim();
            usuario.Email = TxtEmail.Text.Trim();
            usuario.NombreCompleto = TxtNombreCompleto.Text.Trim();
            usuario.Password = TxtPassword.Password;

            return usuario;
        }

        private UpdateUsuarioLogisticoDTO CrearDtoUpdateUsuario()
        {
            UpdateUsuarioLogisticoDTO usuario = new UpdateUsuarioLogisticoDTO();

            usuario.UserName = TxtUserName.Text.Trim();
            usuario.Email = TxtEmail.Text.Trim();
            usuario.NombreCompleto = TxtNombreCompleto.Text.Trim();

            return usuario;
        }

        private void LimpiarFormulario()
        {
            _usuarioSeleccionado = null;

            TxtUserName.Text = "";
            TxtEmail.Text = "";
            TxtNombreCompleto.Text = "";
            TxtPassword.Password = "";
            ChkActivo.IsChecked = false;

            TablaLogisticos.SelectedItem = null;

            TxtMensajeGestionLogisticos.Text = "Formulario limpio.";
        }
    }
}