using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

class Program
{
    // 1. EL CLIENTE GLOBAL
    static readonly HttpClient client = new HttpClient();

    static string UrlBase = "https://localhost:7155/api/";

    static async Task Main(string[] args)
    {
        // 2. CONFIGURACIÓN INICIAL DEL CLIENTE
        client.BaseAddress = new Uri(UrlBase);

        // Intentamos Login
        bool logueado = await PantallaLogin();

        if (logueado)
        {
            bool salir = false;
            while (!salir)
            {
                Console.Clear();
                Console.WriteLine("=== GESTIÓN DE COCHES (VERSIÓN OPTIMIZADA) ===");
                Console.WriteLine("1. Ver todos los coches");
                Console.WriteLine("2. BUSCAR coche por ID");
                Console.WriteLine("3. Crear un coche nuevo");
                Console.WriteLine("4. Borrar un coche");
                Console.WriteLine("5. MODIFICAR un coche");
                Console.WriteLine("6. Salir");

                Console.Write("\nElige una opción: ");
                var opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1": await ListarCoches(); break;
                    case "2": await BuscarCoche(); break;
                    case "3": await CrearCoche(); break;
                    case "4": await BorrarCoche(); break;
                    case "5": await ModificarCoche(); break;
                    case "6": salir = true; break;
                }

                if (!salir)
                {
                    Console.WriteLine("\nPulsa ENTER para continuar...");
                    Console.ReadLine();
                }
            }
        }
    }

    static async Task<bool> PantallaLogin()
    {
        int intentos = 0;
        while (intentos < 3)
        {
            Console.Clear();
            Console.WriteLine($"=== LOGIN (Intento {intentos + 1}/3) ===");
            Console.Write("Usuario (admin): "); string usuario = Console.ReadLine();
            Console.Write("Password (1234): "); string pass = Console.ReadLine();

            var loginData = new { NombreUsuario = usuario, Password = pass };
            var content = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("Auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    dynamic datos = JsonConvert.DeserializeObject(result);
                    string token = datos.token;

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    Console.WriteLine("\n✅ Login Correcto.");
                    Thread.Sleep(1000); 
                    return true;
                }
                else
                {
                    Console.WriteLine("\n❌ Datos incorrectos.");
                    intentos++;
                    Console.ReadLine();
                }
            }
            catch
            {
                Console.WriteLine("\n❌ Error: La API no responde.");
                return false;
            }
        }
        return false;
    }

    static async Task ListarCoches()
    {
        var response = await client.GetAsync("Coches");
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            dynamic coches = JsonConvert.DeserializeObject(json);

            Console.WriteLine($"\n--- LISTADO ({coches.Count}) ---");
            foreach (var c in coches) Console.WriteLine($"ID: {c.id} | {c.marca} {c.modelo} - ${c.precio}");
        }
    }

    static async Task BuscarCoche()
    {
        Console.Write("\nID a buscar: ");
        string id = Console.ReadLine();

        var response = await client.GetAsync($"Coches/{id}");
        if (response.IsSuccessStatusCode)
        {

            //descarga de la API conviertelo en un objt sin forma definida para poder usar los datos
            dynamic c = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
            Console.WriteLine($"\n🚗 {c.marca} {c.modelo} ({c.anio}) - {c.caballos}CV - ${c.precio}");
        }
        else Console.WriteLine("❌ No encontrado.");
    }

    static async Task CrearCoche()
    {
        Console.WriteLine("\n--- NUEVO ---");

        Console.Write("Marca: "); string marca = Console.ReadLine();
        Console.Write("Modelo: "); string modelo = Console.ReadLine();

        // Usamos int.TryParse para que no explote si ponen letras
        Console.Write("Año (ej: 2024): ");
        int.TryParse(Console.ReadLine(), out int anio);

        Console.Write("Caballos (HP): ");
        int.TryParse(Console.ReadLine(), out int caballos);

        Console.Write("Tiempo 0-60 (ej: 4,5): ");
        double.TryParse(Console.ReadLine(), out double tiempo);

        Console.Write("Precio: ");
        decimal.TryParse(Console.ReadLine(), out decimal precio);

        var nuevoCoche = new { Marca = marca, Modelo = modelo, Anio = anio, Caballos = caballos, Tiempo0a60 = tiempo, Precio = precio };
        var content = new StringContent(JsonConvert.SerializeObject(nuevoCoche), Encoding.UTF8, "application/json");

        var response = await client.PostAsync("Coches", content);
        if (response.IsSuccessStatusCode) Console.WriteLine("✅ Creado.");
        else Console.WriteLine("❌ Error al crear.");
    }

    static async Task BorrarCoche()
    {
        Console.Write("\nID a borrar: ");
        string id = Console.ReadLine();

        var response = await client.DeleteAsync($"Coches/{id}");
        if (response.IsSuccessStatusCode) Console.WriteLine("✅ Eliminado.");
        else Console.WriteLine("❌ Fallo al borrar.");
    }

    static async Task ModificarCoche()
    {
        Console.WriteLine("\n--- MODIFICAR COCHE COMPLETO (Edición Inteligente) ---");
        Console.Write("Escribe el ID del coche que quieres cambiar: ");
        string id = Console.ReadLine();

        var responseGet = await client.GetAsync($"Coches/{id}");

        if (!responseGet.IsSuccessStatusCode)
        {
            Console.WriteLine("❌ No se encuentra ese coche.");
            return;
        }

        var jsonGet = await responseGet.Content.ReadAsStringAsync();
        dynamic coche = JsonConvert.DeserializeObject(jsonGet);

        Console.WriteLine($"\nHas seleccionado: {coche.marca} {coche.modelo}");
        Console.WriteLine("INSTRUCCIONES: Escribe el nuevo valor o PULSA ENTER para dejarlo igual.\n");

        // --- MARCA ---
        Console.Write($"Marca actual ({coche.marca}): ");
        string marca = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(marca)) coche.marca = marca;

        // --- MODELO ---
        Console.Write($"Modelo actual ({coche.modelo}): ");
        string modelo = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(modelo)) coche.modelo = modelo;

        // --- AÑO ---
        Console.Write($"Año actual ({coche.anio}): ");
        string anioTxt = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(anioTxt) && int.TryParse(anioTxt, out int nuevoAnio)) coche.anio = nuevoAnio;

        // --- CABALLOS ---
        Console.Write($"Caballos actual ({coche.caballos}): ");
        string hpTxt = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(hpTxt) && int.TryParse(hpTxt, out int nuevosHp)) coche.caballos = nuevosHp;

        // --- TIEMPO ---
        Console.Write($"Tiempo 0-60 actual ({coche.tiempo0a60}): ");
        string tiempoTxt = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(tiempoTxt) && double.TryParse(tiempoTxt, out double nuevoTiempo)) coche.tiempo0a60 = nuevoTiempo;

        // --- PRECIO ---
        Console.Write($"Precio actual ({coche.precio}): ");
        string precioTxt = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(precioTxt) && decimal.TryParse(precioTxt, out decimal nuevoPrecio)) coche.precio = nuevoPrecio;

        var content = new StringContent(JsonConvert.SerializeObject(coche), Encoding.UTF8, "application/json");
        var responsePut = await client.PutAsync($"Coches/{id}", content);

        if (responsePut.IsSuccessStatusCode)
            Console.WriteLine("\n✅ ¡Coche actualizado correctamente!");
        else
            Console.WriteLine($"\n❌ Error al guardar: {responsePut.StatusCode}");
    }
} 