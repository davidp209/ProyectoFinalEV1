using Newtonsoft.Json;
using ProyectoFinal.WPF.Modelos;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;

namespace ProyectoFinal.WPF
{
    public partial class VentanaPrincipal : Window
    {
        public VentanaPrincipal()
        {
            InitializeComponent();
            CargarDatos();
        }

        private async void CargarDatos()
        {
            try
            {
                // Usamos el cliente global optimizado
                var response = await ClienteHttp.Client.GetAsync("Coches");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var listaCoches = JsonConvert.DeserializeObject<List<Coche>>(json);
                    gridCoches.ItemsSource = listaCoches;
                }
            }
            catch
            {
                MessageBox.Show("No se puede conectar con el servidor.", "Error de Conexión");
            }
        }

        // --- LOS BOTONES QUE TE DABAN ERROR ---

        private void BtnRefrescar_Click(object sender, RoutedEventArgs e)
        {
            CargarDatos();
        }

        private async void BtnBorrar_Click(object sender, RoutedEventArgs e)
        {
            var cocheSeleccionado = gridCoches.SelectedItem as Coche;

            if (cocheSeleccionado == null)
            {
                MessageBox.Show("Selecciona un coche de la lista para eliminarlo.");
                return;
            }

            if (MessageBox.Show("¿Seguro que quieres borrar este coche?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No) return;

            var response = await ClienteHttp.Client.DeleteAsync($"Coches/{cocheSeleccionado.Id}");

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Coche eliminado.");
                CargarDatos();
            }
            else
            {
                MessageBox.Show("Error al eliminar.");
            }
        }

        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            // Abrimos la ventana de crear y esperamos a que se cierre
            var ventana = new VentanaCrear();
            ventana.ShowDialog();
            CargarDatos(); // Recargamos la tabla al volver
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            var coche = gridCoches.SelectedItem as Coche;
            if (coche == null)
            {
                MessageBox.Show("Selecciona un coche para editar.");
                return;
            }

            // Pasamos el coche seleccionado a la ventana de edición
            var ventana = new VentanaEditar(coche);
            ventana.ShowDialog();
            CargarDatos();
        }
        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            // 1. Preguntar por si acaso le ha dado sin querer
            if (MessageBox.Show("¿Seguro que quieres cerrar sesión?", "Salir", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                return;

            // 2. Limpiar el token del cliente global (Seguridad)
            // Al ponerlo a null, si alguien intentara usar el cliente ahora, fallaría (que es lo que queremos)
            ClienteHttp.Client.DefaultRequestHeaders.Authorization = null;

            // 3. Abrir la ventana de Login otra vez
            var loginWindow = new MainWindow();
            loginWindow.Show();

            // 4. Cerrar la ventana actual (Principal)
            this.Close();
        }
    }


}