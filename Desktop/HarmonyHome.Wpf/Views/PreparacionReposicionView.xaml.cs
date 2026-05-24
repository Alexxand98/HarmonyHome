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
    /// Lógica de interacción para PreparacionReposicionView.xaml
    /// </summary>
    public partial class PreparacionReposicionView : Window
    {
        public PreparacionReposicionView(PreparacionReposicionDTO preparacion)
        {
            InitializeComponent();

            TxtInfo.Text = "Orden: " + preparacion.OrdenReposicionId + " | Estado: " + preparacion.EstadoOrden;

            List<PreparacionReposicionFila> filas = new List<PreparacionReposicionFila>();

            foreach (LineaPreparacionReposicionDTO linea in preparacion.Lineas) {

                foreach (UbicacionPreparacionReposicionDTO ubicacion in linea.Ubicaciones){

                    filas.Add(new PreparacionReposicionFila{

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

    public class PreparacionReposicionFila
    {
        public string ProductoNombre { get; set; } = string.Empty;

        public int CantidadSolicitada { get; set; }

        public string UbicacionNombre { get; set; } = string.Empty;

        public string UbicacionCodigo { get; set; } = string.Empty;

        public int CantidadDisponible { get; set; }

        public int CantidadARecoger { get; set; }
    }
}