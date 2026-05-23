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
    /// Lógica de interacción para MovimientosStockView.xaml
    /// </summary>
    public partial class MovimientosStockView : Window
    {
        private readonly MovimientoStockService _movimientoService;

        public MovimientosStockView()
        {
            InitializeComponent();

            _movimientoService = new MovimientoStockService();
        }

        private async void BtnCargarMovimientos_Click(object sender, RoutedEventArgs e)
        {
            await CargarMovimientos();
        }

        private async Task CargarMovimientos()
        {
            TxtMensajeMovimientos.Text = "Cargando movimientos...";

            List<MovimientoStockDTO> movimientos = await _movimientoService.GetMovimientosAsync();

            TablaMovimientos.ItemsSource = movimientos;

            TxtMensajeMovimientos.Text = "Movimientos cargados: " + movimientos.Count;
        }
    }
}