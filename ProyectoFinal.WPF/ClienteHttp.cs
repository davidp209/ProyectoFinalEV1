using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ProyectoFinal.WPF
{
    // "static" significa que esta clase es única y global para toda la app
    public static class ClienteHttp
    {
        // 1. EL CLIENTE ÚNICO
        public static HttpClient Client = new HttpClient();

        // 2. CONFIGURACIÓN INICIAL (Constructor estático)
        static ClienteHttp()
        {
            // AJUSTA AQUÍ TU PUERTO (El de Swagger)
            Client.BaseAddress = new Uri("https://localhost:7155/api/");

            // Le decimos que esperamos JSON siempre
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // 3. MÉTODO PARA GUARDAR EL TOKEN UNA VEZ Y OLVIDARSE
        public static void ConfigurarToken(string token)
        {
            // Esto mete la llave en la cabecera del cliente para siempre
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}