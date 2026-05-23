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
    /// Lógica de interacción para DemarcaView.xaml
    /// </summary>
    public partial class DemarcaView : Window
    {
        private readonly ProductoService _productoService;
        private readonly UbicacionService _ubicacionService;
        private readonly DemarcaService _demarcaService;

        public DemarcaView()
        {
            InitializeComponent();

            _productoService = new ProductoService();
            _ubicacionService = new UbicacionService();
            _demarcaService = new DemarcaService();

            Loaded += DemarcaView_Loaded;
        }

        private async void DemarcaView_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarDatos();
        }

        private async Task CargarDatos()
        {
            TxtMensaje.Text = "Cargando datos...";

            List<ProductoDTO> productos = await _productoService.GetProductosAsync();
            productos = productos.Where(p => p.Activo && p.Habilitado).ToList();

            List<UbicacionDTO> ubicaciones = await _ubicacionService.GetUbicacionesPorTipoAsync(2);
            ubicaciones = ubicaciones.Where(u => u.Activa).ToList();

            CmbProducto.ItemsSource = productos;
            CmbUbicacion.ItemsSource = ubicaciones;

            TxtMensaje.Text = "Datos cargados.";
        }

        private async void BtnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarFormulario())
            {
                return;
            }

            CreateDemarcaDTO demarca = new CreateDemarcaDTO();

            demarca.ProductoId = (int)CmbProducto.SelectedValue;
            demarca.UbicacionId = (int)CmbUbicacion.SelectedValue;
            demarca.Cantidad = int.Parse(TxtCantidad.Text);
            demarca.Motivo = TxtMotivo.Text.Trim();

            string mensaje = await _demarcaService.CrearDemarcaAsync(demarca);

            TxtMensaje.Text = mensaje;
        }

        private bool ValidarFormulario()
        {
            if (CmbProducto.SelectedValue == null)
            {
                TxtMensaje.Text = "Selecciona un producto.";
                return false;
            }

            if (CmbUbicacion.SelectedValue == null)
            {
                TxtMensaje.Text = "Selecciona una ubicación.";
                return false;
            }

            if (!int.TryParse(TxtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                TxtMensaje.Text = "La cantidad debe ser mayor que cero.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtMotivo.Text))
            {
                TxtMensaje.Text = "El motivo es obligatorio.";
                return false;
            }

            return true;
        }
    }
}