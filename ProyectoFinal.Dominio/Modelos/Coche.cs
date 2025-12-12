using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinal.Dominio.Modelos
{
    public class Coche
    {
        public int Id { get; set; } // Nuestro ID interno
        public string Marca { get; set; }       // Car Make
        public string Modelo { get; set; }      // Car Model
        public int Anio { get; set; }           // Year
        public int Caballos { get; set; }       // Horsepower
        public double Tiempo0a60 { get; set; }  // 0-60 MPH Time
        public decimal Precio { get; set; }     // Price (in USD)
    }
}
