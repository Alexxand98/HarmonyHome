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
using System.Linq;

namespace HarmonyHome.Wpf.Views
{
    /// <summary>
    /// Lógica de interacción para StockView.xaml
    /// </summary>
    public partial class StockView : Window
    {
        private readonly StockService _stockService;

        private List<StockUbicacionDTO> _stock = new List<StockUbicacionDTO>();

        public StockView()
        {
            InitializeComponent();

            _stockService = new StockService();
        }

        private async void BtnCargarTodo_Click(object sender, RoutedEventArgs e)
        {
            await CargarStock("todo");
        }

        private async void BtnCargarTienda_Click(object sender, RoutedEventArgs e)
        {
            await CargarStock("tienda");
        }

        private async void BtnCargarAlmacen_Click(object sender, RoutedEventArgs e)
        {
            await CargarStock("almacen");
        }

        private async Task CargarStock(string tipo)
        {
            TxtMensajeStock.Text = "Cargando stock...";

            List<StockUbicacionDTO> stock;

            if (tipo == "tienda") {
                stock = await _stockService.GetStockTiendaAsync();
            }else if (tipo == "almacen"){
                stock = await _stockService.GetStockAlmacenAsync();
            }  else{
                stock = await _stockService.GetStockAsync();
            }

            _stock = stock;

            TablaStock.ItemsSource = _stock;

            TxtMensajeStock.Text = "Registros cargados: " + stock.Count;
        }

        private async void BtnAltaStock_Click(object sender, RoutedEventArgs e)
        {
            AltaStockView altaStockView = new AltaStockView();

            altaStockView.ShowDialog();

            await CargarStock("todo");
        }

        private async void BtnMoverStock_Click(object sender, RoutedEventArgs e)
        {
            MovimientoInternoView movimientoInternoView = new MovimientoInternoView();

            movimientoInternoView.ShowDialog();

            await CargarStock("todo");
        }

        private void BtnBuscarStock_Click(object sender, RoutedEventArgs e)
        {
            string texto = TxtBuscarStock.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(texto)) {

                TablaStock.ItemsSource = _stock;

                TxtMensajeStock.Text = "Registros cargados: " + _stock.Count;

                return;
            }

            List<StockUbicacionDTO> stockFiltrado = _stock.Where(s =>(s.ProductoReferencia != null && s.ProductoReferencia.ToLower().Contains(texto)) || (s.ProductoNombre != null && s.ProductoNombre.ToLower().Contains(texto))).ToList();

            TablaStock.ItemsSource = stockFiltrado;

            TxtMensajeStock.Text = "Resultados encontrados: " + stockFiltrado.Count;
        }

        private void BtnLimpiarBusquedaStock_Click(object sender, RoutedEventArgs e)
        {
            TxtBuscarStock.Text = "";

            TablaStock.ItemsSource = _stock;

            TxtMensajeStock.Text = "Búsqueda limpia";
        }
    }
}