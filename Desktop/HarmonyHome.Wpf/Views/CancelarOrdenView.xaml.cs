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
    /// Lógica de interacción para CancelarOrdenView.xaml
    /// </summary>
    public partial class CancelarOrdenView : Window
    {
        public string MotivoCancelacion { get; private set; } = string.Empty;

        public CancelarOrdenView()
        {
            InitializeComponent();
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtMotivo.Text))  {
                TxtMensaje.Text = "El motivo es obligatorio";
                return;
            }

            MotivoCancelacion = TxtMotivo.Text.Trim();

            DialogResult = true;

            Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            Close();
        }
    }
}