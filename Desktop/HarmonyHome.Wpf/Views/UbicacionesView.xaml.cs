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
    /// Lógica de interacción para UbicacionesView.xaml
    /// </summary>
    public partial class UbicacionesView : Window
    {
        private readonly UbicacionService _ubicacionService;

        public UbicacionesView()
        {
            InitializeComponent();
            _ubicacionService = new UbicacionService();
        }

        private async void BtnCargarUbicaciones_Click(object sender, RoutedEventArgs e)
        {
            TxtMensajeUbicaciones.Text = "Cargando ubicaciones...";
            BtnCargarUbicaciones.IsEnabled = false;

            List<UbicacionDTO> ubicaciones = await _ubicacionService.GetUbicacionesAsync();

            TablaUbicaciones.ItemsSource = ubicaciones;

            if (ubicaciones.Count == 0)
            {
                TxtMensajeUbicaciones.Text = "No se encontraron ubicaciones";
            }
            else
            {
                TxtMensajeUbicaciones.Text = "Ubicaciones cargadas " + ubicaciones.Count;
            }

            BtnCargarUbicaciones.IsEnabled = true;
        }
    }
}