using HarmonyHome.Wpf.Models.DTOs;
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
    /// Lógica de interacción para PreparacionRecogidaView.xaml
    /// </summary>
    public partial class PreparacionRecogidaView : Window
    {
        public PreparacionRecogidaView(PreparacionRecogidaDTO preparacion)
        {
            InitializeComponent();

            TxtInfo.Text = "Orden: " + preparacion.OrdenRecogidaId
                + " | Pedido: " + preparacion.PedidoVentaId
                + " | Cliente: " + preparacion.ClienteNombreCompleto
                + " | Estado: " + preparacion.EstadoOrden;

            List<PreparacionRecogidaFila> filas = new List<PreparacionRecogidaFila>();

            foreach (LineaPreparacionRecogidaDTO linea in preparacion.Lineas){

                foreach (UbicacionPreparacionRecogidaDTO ubicacion in linea.Ubicaciones){

                    filas.Add(new PreparacionRecogidaFila
                    {
                        ProductoNombre = linea.ProductoNombre,
                        CantidadSolicitada = linea.CantidadSolicitada,
                        UbicacionNombre = ubicacion.UbicacionNombre,
                        UbicacionCodigo = ubicacion.UbicacionCodigo,
                        CantidadDisponible = ubicacion.CantidadDisponible,
                        CantidadARecoger = ubicacion.CantidadARecoger
                    });

                }
            }

            TablaPreparacion.ItemsSource = filas;
        }
    }

    public class PreparacionRecogidaFila
    {
        public string ProductoNombre { get; set; } = string.Empty;

        public int CantidadSolicitada { get; set; }

        public string UbicacionNombre { get; set; } = string.Empty;

        public string UbicacionCodigo { get; set; } = string.Empty;

        public int CantidadDisponible { get; set; }

        public int CantidadARecoger { get; set; }
    }
}