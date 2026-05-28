using HarmonyHome.Wpf.Models.DTOs;
using HarmonyHome.Wpf.Services;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

                } else {

                    TxtMensajeMovimientos.Text = "Movimientos cargados: " + movimientos.Count;
                }

            } finally {

                BtnActualizarMovimientos.IsEnabled = true;
            }
        }

        private void BtnExportarMovimientos_Click(object sender, RoutedEventArgs e)
        {
            List<MovimientoStockDTO> movimientos = TablaMovimientos.ItemsSource as List<MovimientoStockDTO>
                ?? TablaMovimientos.ItemsSource?.Cast<MovimientoStockDTO>().ToList()
                ?? new List<MovimientoStockDTO>();

            if (movimientos.Count == 0){

                TxtMensajeMovimientos.Text = "No hay movimientos para exportar.";
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Title = "Guardar reporte de movimientos";
            saveFileDialog.Filter = "Archivo CSV (*.csv)|*.csv";
            saveFileDialog.FileName = "reporte_movimientos_stock.csv";

            bool? resultado = saveFileDialog.ShowDialog();

            if (resultado != true) {
                return;
            }

            StringBuilder csv = new StringBuilder();

            csv.AppendLine("Fecha;Referencia;Producto;Origen;Destino;Cantidad;Tipo;Observaciones");

            foreach (MovimientoStockDTO movimiento in movimientos){

                csv.AppendLine(
                    LimpiarCsv(movimiento.Fecha.ToString()) + ";" +
                    LimpiarCsv(movimiento.ProductoReferencia) + ";" +
                    LimpiarCsv(movimiento.ProductoNombre) + ";" +
                    LimpiarCsv(movimiento.UbicacionOrigenCodigo) + ";" +
                    LimpiarCsv(movimiento.UbicacionDestinoCodigo) + ";" +
                    movimiento.Cantidad + ";" +
                    LimpiarCsv(movimiento.TipoMovimientoNombre) + ";" +
                    LimpiarCsv(movimiento.Observaciones)
                );
            }

            File.WriteAllText(saveFileDialog.FileName, csv.ToString(), Encoding.UTF8);

            TxtMensajeMovimientos.Text = "Reporte exportado correctamente.";
        }

        private string LimpiarCsv(string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) {

                return "";
            }

            return valor
                .Replace(";", ",")
                .Replace("\r", " ")
                .Replace("\n", " ")
                .Trim();
        }
    }
}