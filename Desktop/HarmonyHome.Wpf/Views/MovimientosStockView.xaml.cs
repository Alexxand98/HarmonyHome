using HarmonyHome.Wpf.Models.DTOs;
using HarmonyHome.Wpf.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

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

            Loaded += MovimientosStockView_Loaded;
        }

        private async void MovimientosStockView_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarMovimientos();
        }

        private async void BtnActualizarMovimientos_Click(object sender, RoutedEventArgs e)
        {
            await CargarMovimientos();
        }

        private async Task CargarMovimientos()
        {
            TxtMensajeMovimientos.Text = "Cargando movimientos...";
            BtnActualizarMovimientos.IsEnabled = false;

            try{
                List<MovimientoStockDTO> movimientos = await _movimientoService.GetMovimientosAsync();

                TablaMovimientos.ItemsSource = movimientos;

                if (movimientos.Count == 0){
                    TxtMensajeMovimientos.Text = "No se encontraron movimientos.";
                }else{
                    TxtMensajeMovimientos.Text = "Movimientos cargados: " + movimientos.Count;
                }

            }
            finally
            {

                BtnActualizarMovimientos.IsEnabled = true;
            }
        }
    }
}