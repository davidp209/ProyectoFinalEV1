using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinal.Dominio.Modelos
{
    public class UsuarioLogin
    {
        public required string NombreUsuario { get; set; }
        public required string Password { get; set; }

    }
}

