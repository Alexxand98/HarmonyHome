using HarmonyHome.Wpf.Helpers;
using HarmonyHome.Wpf.Models.DTOs;
using HarmonyHome.Wpf.Services;
using HarmonyHome.Wpf.Views;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System;
using System.Linq;

namespace HarmonyHome.Wpf
{
    public partial class MainWindow : Window
    {
        private readonly OrdenRecogidaService _ordenRecogidaService;
        private readonly OrdenReposicionService _ordenReposicionService;
        private readonly StockService _stockService;
        private readonly MovimientoStockService _movimientoStockService;

        public MainWindow()
        {
            InitializeComponent();

            _ordenRecogidaService = new OrdenRecogidaService();
            _ordenReposicionService = new OrdenReposicionService();
            _stockService = new StockService();
            _movimientoStockService = new MovimientoStockService();

            CargarDatosUsuario();

            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarDashboard();
        }

        private void CargarDatosUsuario()
        {
            TxtUsuario.Text = SessionManager.Email;
            TxtRol.Text = SessionManager.Rol;

            if (SessionManager.Rol == "Logistico") {

                BtnGestionProductos.Visibility = Visibility.Collapsed;
                BtnGestionUbicaciones.Visibility = Visibility.Collapsed;
                BtnGestionLogisticos.Visibility = Visibility.Collapsed;
            }else{

                BtnGestionProductos.Visibility = Visibility.Visible;
                BtnGestionUbicaciones.Visibility = Visibility.Visible;
                BtnGestionLogisticos.Visibility = Visibility.Visible;
            }

            TxtVersionApp.Text = AppInfo.NombreApp + " v" + AppInfo.Version;
            TxtModuloApp.Text = AppInfo.Modulo;
        }

        private async Task CargarDashboard()
        {
            try{

                List<OrdenRecogidaDTO> recogidas = await _ordenRecogidaService.GetOrdenesAsync();
                List<OrdenReposicionDTO> reposiciones = await _ordenReposicionService.GetOrdenesAsync();

                List<ProductoBajoStockDTO> productosBajoStock = await _stockService.GetProductosBajoStockGeneralAsync();
                List<MovimientoStockDTO> movimientos = await _movimientoStockService.GetMovimientosAsync();

                DateTime hoy = DateTime.Today;

                List<MovimientoStockDTO> movimientosHoy = movimientos
                    .Where(m => m.Fecha.Date == hoy)
                    .OrderByDescending(m => m.Fecha)
                    .ToList();

                List<MovimientoStockDTO> movimientosDashboard = movimientosHoy
                    .Take(14)
                    .ToList();

                TablaUltimosMovimientos.ItemsSource = movimientosDashboard;

                int recogidasPendientes = recogidas.Count(o => o.EstadoNombre == "Pendiente");
                int reposicionesPendientes = reposiciones.Count(o => o.EstadoNombre == "Pendiente");

                TxtRecogidasPendientes.Text = recogidasPendientes.ToString();
                TxtReposicionesPendientes.Text = reposicionesPendientes.ToString();
                TxtProductosBajoStock.Text = productosBajoStock.Count.ToString();
                TxtMovimientosRecientes.Text = movimientosDashboard.Count.ToString();

                CargarGraficoEstados(recogidas);

            }catch{

                TxtRecogidasPendientes.Text = "-";
                TxtReposicionesPendientes.Text = "-";
                TxtProductosBajoStock.Text = "-";
                TxtMovimientosRecientes.Text = "-";

                TablaUltimosMovimientos.ItemsSource = null;
            }
        }

        private void BtnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            SessionManager.ClearSession();

            LoginView loginView = new LoginView();

            loginView.Show();

            Close();
        }

        private void BtnProductos_Click(object sender, RoutedEventArgs e)
        {
            ProductosView productosView = new ProductosView();

            productosView.ShowDialog();
        }

        private void BtnUbicaciones_Click(object sender, RoutedEventArgs e)
        {
            UbicacionesView ubicacionesView = new UbicacionesView();

            ubicacionesView.ShowDialog();
        }

        private void BtnGestionProductos_Click(object sender, RoutedEventArgs e)
        {
            GestionProductosView gestionProductosView = new GestionProductosView();

            gestionProductosView.ShowDialog();
        }

        private void BtnGestionUbicaciones_Click(object sender, RoutedEventArgs e)
        {
            GestionUbicacionesView gestionUbicacionesView = new GestionUbicacionesView();

            gestionUbicacionesView.ShowDialog();
        }

        private void BtnGestionLogisticos_Click(object sender, RoutedEventArgs e)
        {
            GestionLogisticosView gestionLogisticosView = new GestionLogisticosView();

            gestionLogisticosView.ShowDialog();
        }

        private void BtnStock_Click(object sender, RoutedEventArgs e)
        {
            StockView stockView = new StockView();

            stockView.ShowDialog();
        }

        private void BtnMovimientos_Click(object sender, RoutedEventArgs e)
        {
            MovimientosStockView movimientosStockView = new MovimientosStockView();

            movimientosStockView.ShowDialog();
        }

        private void BtnDemarca_Click(object sender, RoutedEventArgs e)
        {
            DemarcaView demarcaView = new DemarcaView();

            demarcaView.ShowDialog();
        }

        private void BtnOrdenesRecogida_Click(object sender, RoutedEventArgs e)
        {
            OrdenesRecogidaView ordenesRecogidaView = new OrdenesRecogidaView();

            ordenesRecogidaView.ShowDialog();
        }

        private void BtnOrdenesReposicion_Click(object sender, RoutedEventArgs e)
        {
            OrdenesReposicionView ordenesReposicionView = new OrdenesReposicionView();

            ordenesReposicionView.ShowDialog();
        }

        private void CargarGraficoEstados(List<OrdenRecogidaDTO> recogidas)
        {
            int pendientes = recogidas.Count(o => o.EstadoNombre == "Pendiente");
            int asignadas = recogidas.Count(o => o.EstadoNombre == "Asignada");
            int enPreparacion = recogidas.Count(o => o.EstadoNombre == "EnPreparacion");
            int finalizadas = recogidas.Count(o => o.EstadoNombre == "Finalizada");
            int canceladas = recogidas.Count(o => o.EstadoNombre == "Cancelada");

            TxtGraficoPendientes.Text = pendientes.ToString();
            TxtGraficoAsignadas.Text = asignadas.ToString();
            TxtGraficoEnPreparacion.Text = enPreparacion.ToString();
            TxtGraficoFinalizadas.Text = finalizadas.ToString();
            TxtGraficoCanceladas.Text = canceladas.ToString();

            int total = pendientes + asignadas + enPreparacion + finalizadas + canceladas;

            if (total == 0){
                GraficoEstadosOrdenes.Series = new ISeries[]
                {
            new PieSeries<double>
            {
                Name = "Sin datos",
                Values = new double[] { 1 }
            }
                };

                return;
            }

            GraficoEstadosOrdenes.Series = new ISeries[]
            {
        new PieSeries<double>
        {
            Name = "Pendientes",
            Values = new double[] { pendientes }
        },
        new PieSeries<double>
        {
            Name = "Asignadas",
            Values = new double[] { asignadas }
        },
        new PieSeries<double>
        {
            Name = "En preparación",
            Values = new double[] { enPreparacion }
        },
        new PieSeries<double>
        {
            Name = "Finalizadas",
            Values = new double[] { finalizadas }
        },
        new PieSeries<double>
        {
            Name = "Canceladas",
            Values = new double[] { canceladas }
        }
            };
        }
    }
}