using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProyectoFinal.Dominio.Interfaces
{
    public interface IRepositorio<T>
    {
        // 1. Obtener todos los datos (devuelve una lista)
        Task<List<T>> ObtenerTodosAsync();

        // 2. Obtener solo uno por su ID
        Task<T> ObtenerPorIdAsync(int id);

        // 3. Guardar un dato nuevo
        Task AgregarAsync(T entidad);

        // 4. Borrar un dato
        Task EliminarAsync(int id);

        // 5. OBLIGATORIO POR RÚBRICA: Cargar desde el CSV de Kaggle
        Task CargarDesdeCSV(string rutaArchivo);

        // 6. OPCIONAL POR RÚBRICA: Actualizar un dato existente
        Task ActualizarAsync(T entidad); 
    }
}