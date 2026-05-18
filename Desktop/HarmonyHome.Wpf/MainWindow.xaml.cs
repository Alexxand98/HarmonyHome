using HarmonyHome.Wpf.Helpers;
using HarmonyHome.Wpf.Views;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HarmonyHome.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            CargarDatosUsuario();
        }

        private void CargarDatosUsuario()
        {
            TxtUsuario.Text = SessionManager.Email;

            TxtRol.Text = SessionManager.Rol;


            if (SessionManager.Rol == "Logistico")
            {
                BtnGestionProductos.Visibility = Visibility.Collapsed;
            }
            else
            {
                BtnGestionProductos.Visibility = Visibility.Visible;
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



        private void BtnServerInfo_Click(object sender, RoutedEventArgs e)
        {
            ServidorView servidorView = new ServidorView();


            servidorView.ShowDialog();
        }


        private void BtnGestionProductos_Click(object sender, RoutedEventArgs e)
        {
            GestionProductosView gestionProductosView = new GestionProductosView();

            gestionProductosView.ShowDialog();
        }


    }



}