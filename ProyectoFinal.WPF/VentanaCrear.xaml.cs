using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Windows;

namespace ProyectoFinal.WPF
{
    public partial class VentanaCrear : Window
    {
        public VentanaCrear()
        {
            InitializeComponent();
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // 1. Validación básica: Que no haya campos vacíos
            if (string.IsNullOrWhiteSpace(txtMarca.Text) ||
                string.IsNullOrWhiteSpace(txtModelo.Text) ||
                string.IsNullOrWhiteSpace(txtPrecio.Text))
            {
                MessageBox.Show("Por favor, rellena todos los campos.");
                return;
            }

            try
            {
                // 2. Convertimos los textos a números
                // Si alguien escribe "hola" en el precio, saltará al 'catch'
                int anio = int.Parse(txtAnio.Text);
                int caballos = int.Parse(txtCaballos.Text);
                double tiempo = double.Parse(txtTiempo0a60.Text); // Usa coma o punto según tu PC
                decimal precio = decimal.Parse(txtPrecio.Text);

                // 3. Creamos el objeto con TODOS los datos reales
                var nuevoCoche = new
                {
                    Marca = txtMarca.Text,
                    Modelo = txtModelo.Text,
                    Anio = anio,
                    Caballos = caballos,
                    Tiempo0a60 = tiempo,
                    Precio = precio
                };

                // 4. Enviar a la API
                var json = JsonConvert.SerializeObject(nuevoCoche);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Usamos el cliente global
                var response = await ClienteHttp.Client.PostAsync("Coches", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("¡Coche guardado correctamente!");
                    this.Close(); // Cerramos la ventana
                }
                else
                {
                    MessageBox.Show("Error al guardar en el servidor.");
                }
            }
            catch
            {
                MessageBox.Show("Error: Revisa que el Precio, Año, Caballos y Tiempo sean números válidos.\n(Si usas decimales, prueba con coma ',' o punto '.')");
            }
        }
    }
}