using ProyectoFinal.Dominio.Interfaces;
using ProyectoFinal.Dominio.Modelos;
using System.Globalization;

namespace ProyectoFinal.Datos.Repositorios
{
    public class RepositorioMemoria : IRepositorio<Coche>
    {
        // Usamos 'static' para que los datos no se borren entre peticiones
        private static List<Coche> _datos = new List<Coche>();

        // 1. LEER TODOS
        public async Task<List<Coche>> ObtenerTodosAsync()
        {
            return await Task.Run(() => _datos.ToList());
        }

        // 2. LEER UNO POR ID
        public async Task<Coche> ObtenerPorIdAsync(int id)
        {
            return await Task.Run(() => _datos.FirstOrDefault(c => c.Id == id));
        }

        // 3. INSERTAR
        public async Task AgregarAsync(Coche entidad)
        {
            await Task.Run(() =>
            {
                if (_datos.Any())
                    entidad.Id = _datos.Max(x => x.Id) + 1;
                else
                    entidad.Id = 1;

                _datos.Add(entidad);
            });
        }

        // 4. ELIMINAR
        public async Task EliminarAsync(int id)
        {
            await Task.Run(() =>
            {
                var item = _datos.FirstOrDefault(c => c.Id == id);
                if (item != null)
                {
                    _datos.Remove(item);
                }
            });
        }

        // 5. ACTUALIZAR
        public async Task ActualizarAsync(Coche entidad)
        {
            await Task.Run(() =>
            {
                var existente = _datos.FirstOrDefault(c => c.Id == entidad.Id);
                if (existente != null)
                {
                    existente.Marca = entidad.Marca;
                    existente.Modelo = entidad.Modelo;
                    existente.Precio = entidad.Precio;
                    existente.Anio = entidad.Anio;
                    existente.Caballos = entidad.Caballos;
                    existente.Tiempo0a60 = entidad.Tiempo0a60;
                }
            });
        }

        // 6. CARGA DESDE CSV
        public async Task CargarDesdeCSV(string rutaArchivo)
        {
            await Task.Run(() =>
            {
                if (_datos.Any()) return;
                if (!File.Exists(rutaArchivo)) return;

                var lineas = File.ReadAllLines(rutaArchivo);

                foreach (var linea in lineas.Skip(1))
                {
                    var col = linea.Split(',');

                    if (col.Length >= 8)
                    {
                        try
                        {
                            var c = new Coche();
                            c.Marca = col[0].Trim();
                            c.Modelo = col[1].Trim();
                            int.TryParse(col[2], out int anio); c.Anio = anio;
                            int.TryParse(col[4], out int hp); c.Caballos = hp;
                            double.TryParse(col[6].Replace(".", ","), out double t060); c.Tiempo0a60 = t060;

                            string precioLimpio = col[7].Replace("\"", "").Replace(",", "").Trim();
                            decimal.TryParse(precioLimpio, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal precio);
                            c.Precio = precio;

                            c.Id = _datos.Any() ? _datos.Max(x => x.Id) + 1 : 1;
                            _datos.Add(c);
                        }
                        catch { }
                    }
                }
            });
        }
    }
}