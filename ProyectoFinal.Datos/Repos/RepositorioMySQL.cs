using MySql.Data.MySqlClient;
using ProyectoFinal.Dominio.Interfaces;
using ProyectoFinal.Dominio.Modelos;
using System.Globalization;
using System.Data;

namespace ProyectoFinal.Datos.Repositorios
{
    public class RepositorioMySQL : IRepositorio<Coche>
    {
        private string _cadenaConexion;

        public RepositorioMySQL(string cadenaConexion)
        {
            _cadenaConexion = cadenaConexion;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(_cadenaConexion);
        }

        public async Task<List<Coche>> ObtenerTodosAsync()
        {
            var lista = new List<Coche>();
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var query = "SELECT * FROM Coches";
                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(MapearCoche(reader));
                    }
                }
            }
            return lista;
        }

        // --- NUEVO: Obtener solo uno por ID ---
        public async Task<Coche> ObtenerPorIdAsync(int id)
        {
            Coche coche = null;
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var query = "SELECT * FROM Coches WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            coche = MapearCoche(reader);
                        }
                    }
                }
            }
            return coche;
        }

        public async Task AgregarAsync(Coche c)
        {
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var query = @"INSERT INTO Coches (Marca, Modelo, Anio, Caballos, Tiempo0a60, Precio) 
                              VALUES (@Marca, @Modelo, @Anio, @Caballos, @Tiempo0a60, @Precio)";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Marca", c.Marca);
                    cmd.Parameters.AddWithValue("@Modelo", c.Modelo);
                    cmd.Parameters.AddWithValue("@Anio", c.Anio);
                    cmd.Parameters.AddWithValue("@Caballos", c.Caballos);
                    cmd.Parameters.AddWithValue("@Tiempo0a60", c.Tiempo0a60);
                    cmd.Parameters.AddWithValue("@Precio", c.Precio);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        // --- NUEVO: Eliminar de la base de datos ---
        public async Task EliminarAsync(int id)
        {
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var query = "DELETE FROM Coches WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task CargarDesdeCSV(string rutaArchivo)
        {
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var cmd = new MySqlCommand("SELECT COUNT(*) FROM Coches", conn);
                long count = (long)await cmd.ExecuteScalarAsync();
                if (count > 0) return;
            }

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

                        await AgregarAsync(c);
                    }
                    catch { }
                }
            }
        }

        // Helper para no repetir código al leer
        private Coche MapearCoche(System.Data.Common.DbDataReader reader)
        {
            return new Coche
            {
                Id = reader.GetInt32("Id"),
                Marca = reader.GetString("Marca"),
                Modelo = reader.GetString("Modelo"),
                Anio = reader.GetInt32("Anio"),
                Caballos = reader.GetInt32("Caballos"),
                Tiempo0a60 = reader.GetDouble("Tiempo0a60"),
                Precio = reader.GetDecimal("Precio")
            };
        }
        public async Task ActualizarAsync(Coche c)
        {
          
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();

                var query = @"UPDATE Coches 
                      SET Marca = @Marca, 
                          Modelo = @Modelo, 
                          Anio = @Anio, 
                          Caballos = @Caballos, 
                          Tiempo0a60 = @Tiempo0a60, 
                          Precio = @Precio 
                      WHERE Id = @Id";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", c.Id);
                    cmd.Parameters.AddWithValue("@Marca", c.Marca);
                    cmd.Parameters.AddWithValue("@Modelo", c.Modelo);
                    cmd.Parameters.AddWithValue("@Anio", c.Anio);
                    cmd.Parameters.AddWithValue("@Caballos", c.Caballos);
                    cmd.Parameters.AddWithValue("@Tiempo0a60", c.Tiempo0a60);

                    cmd.Parameters.AddWithValue("@Precio", c.Precio);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }


    }

}