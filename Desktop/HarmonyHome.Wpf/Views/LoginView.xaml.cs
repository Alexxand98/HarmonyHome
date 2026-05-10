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
    /// Lógica de interacción para LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        private readonly AuthService _authService;

        public LoginView()
        {
            InitializeComponent();

            _authService = new AuthService();
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            TxtError.Text = "";

            string email = TxtEmail.Text.Trim();

            string password = TxtPassword.Password.Trim();

            if (string.IsNullOrWhiteSpace(email)){

                TxtError.Text = "Introduce el email";
                return;
            }


            if (string.IsNullOrWhiteSpace(password)){

                TxtError.Text = "Introduce la contraseña.";
                return;
            }

            BtnEnterLogin.IsEnabled = false;

            BtnEnterLogin.Content = "Entrando...";

            bool loginOk = await _authService.LoginAsync(email, password);

            if (loginOk){

                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();

                Close();

            }else{
                TxtError.Text = "No se pudo iniciar sesion";

                BtnEnterLogin.IsEnabled = true;


                BtnEnterLogin.Content = "Iniciar sesion";
            }
        }



        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}

