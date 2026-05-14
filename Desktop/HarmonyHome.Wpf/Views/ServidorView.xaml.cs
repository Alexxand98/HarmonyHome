using HarmonyHome.Wpf.Helpers;
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
    /// Lógica de interacción para ServidorView.xaml
    /// </summary>
    public partial class ServidorView : Window
    {
        public ServidorView()
        {
            InitializeComponent();

            CargarDatos();
        }

        private void CargarDatos()
        {
            TxtNombreApp.Text = "Aplicacion: " + AppInfo.NombreApp;
            TxtVersion.Text = "Version: " + AppInfo.Version;
            TxtUsuarioServidor.Text = "Usuario: " + SessionManager.Email;
            TxtRolServidor.Text = "Rol: " + SessionManager.Rol;

            if (SessionManager.IsLoggedIn){
                TxtSesion.Text = "Sesion: activa";
            } else{
                TxtSesion.Text = "Sesion: No activa";
            }
        }

        private void BtnCerrarInfo_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}