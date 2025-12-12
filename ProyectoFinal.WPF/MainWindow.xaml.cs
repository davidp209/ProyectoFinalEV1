using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Windows;

namespace ProyectoFinal.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            lblError.Text = "Conectando...";
            var usuario = txtUsuario.Text;
            var password = txtPass.Password;

            var loginData = new { NombreUsuario = usuario, Password = password };
            var json = JsonConvert.SerializeObject(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await ClienteHttp.Client.PostAsync("Auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var datos = JObject.Parse(result);
                    string token = datos["token"].ToString();

                    // Configuramos el token global
                    ClienteHttp.ConfigurarToken(token);

                    // Abrimos la principal
                    var ventana = new VentanaPrincipal();
                    ventana.Show();
                    this.Close();
                }
                else
                {
                    lblError.Text = "Usuario o contraseña incorrectos.";
                }
            }
            catch
            {
                lblError.Text = "Error: La API no responde.";
            }
        }
    }
}