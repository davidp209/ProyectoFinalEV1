using Newtonsoft.Json;
using ProyectoFinal.WPF.Modelos;
using System.Net.Http;
using System.Text;
using System.Windows;

namespace ProyectoFinal.WPF
{
    public partial class VentanaEditar : Window
    {
        private Coche _coche;

        public VentanaEditar(Coche coche)
        {
            InitializeComponent();
            _coche = coche;

            // 1. CARGAR DATOS EXISTENTES EN LAS CAJAS
            // Es importante usar .ToString() porque las cajas solo entienden texto
            txtMarca.Text = _coche.Marca;
            txtModelo.Text = _coche.Modelo;
            txtAnio.Text = _coche.Anio.ToString();
            txtCaballos.Text = _coche.Caballos.ToString();
            txtTiempo0a60.Text = _coche.Tiempo0a60.ToString();
            txtPrecio.Text = _coche.Precio.ToString();
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 2. LEER Y ACTUALIZAR EL OBJETO
                _coche.Marca = txtMarca.Text;
                _coche.Modelo = txtModelo.Text;

                // Convertimos el texto a números (Esto puede fallar si escriben letras, por eso el try-catch)
                _coche.Anio = int.Parse(txtAnio.Text);
                _coche.Caballos = int.Parse(txtCaballos.Text);
                _coche.Tiempo0a60 = double.Parse(txtTiempo0a60.Text);
                _coche.Precio = decimal.Parse(txtPrecio.Text);

                // 3. ENVIAR A LA API (PUT)
                var json = JsonConvert.SerializeObject(_coche);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Usamos el cliente global optimizado
                var response = await ClienteHttp.Client.PutAsync($"Coches/{_coche.Id}", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("¡Coche actualizado correctamente!");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Error al actualizar en el servidor.");
                }
            }
            catch
            {
                MessageBox.Show("Error: Revisa que Año, Caballos, Tiempo y Precio sean números válidos.");
            }
        }
    }
}