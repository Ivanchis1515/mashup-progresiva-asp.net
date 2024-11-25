using System.Diagnostics;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Prod2Mashup.Helpers;
using Prod2Mashup.Models;

namespace Prod2Mashup.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger; //registra mensajes en el sistema de logging
        private readonly ApiHelper _apiHelper; //instanciacion del helper

        //constructor que inyecta el helper y el logger
        public HomeController(ILogger<HomeController> logger, ApiHelper apiHelper)
        {
            _logger = logger;
            _apiHelper = apiHelper;
        }

        //funcion de vista principal
        public IActionResult Index()
        {
            return View();
        }

        //accion para obtener datos del clima
        public async Task<IActionResult> GetWeatherData(string city, string countryCode)
        {
            _logger.LogInformation($"Solicitud recibida para ciudad: {city}, código de país: {countryCode}");

            //construye la URL para la API de geolocalizacion
            string geolocationUrl = $"https://nominatim.openstreetmap.org/search?city={city}&country={countryCode}&format=json";

            try
            {
                //consume la API de geolocalizacion y obtiene la respuesta en formato JSON
                string geolocationResponse = await _apiHelper.ConsumeApiAsync(geolocationUrl);
                _logger.LogInformation($"Respuesta de Nominatim: {geolocationResponse}");

                //deserializa la respuesta en una lista de objetos 
                var coordinates = JsonConvert.DeserializeObject<List<Geolocation>>(geolocationResponse);

                //verifica si no se encontraron coordenadas y retorna un error HTTP 400
                if (coordinates == null || !coordinates.Any())
                {
                    return BadRequest(new { message = "No se encontraron coordenadas para la ciudad proporcionada." });
                }

                //extrae latitud y longitud del primer resultados
                var lat = coordinates[0].lat;
                var lon = coordinates[0].lon;

                //URL para la API del clima usando latitud y longitud
                string weatherUrl = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid=67092dbddc839bf4c8a128a21a3c5094&units=metric&lang=es";
                //consume la API del clima y obtiene la respuesta en formato JSON
                string weatherResponse = await _apiHelper.ConsumeApiAsync(weatherUrl);
                _logger.LogInformation($"Respuesta de OpenWeatherMap: {weatherResponse}");

                //deserializa la respuesta del clima en un objeto DatosClima
                var weatherData = JsonConvert.DeserializeObject<DatosClima>(weatherResponse);

                if (weatherData == null || weatherData.weather == null || !weatherData.weather.Any())
                {
                    return BadRequest(new { message = "Los datos del clima no son válidos." });
                }

                //crea una lista de objetos DatosRequeridosClima con los datos necesarios
                var datosRequeridos = new List<DatosRequeridosClima>
                {
                    new DatosRequeridosClima
                    {
                        Ciudad = weatherData.name,//nombre ciudad
                        Pais = weatherData.sys.country, //pais
                        Temperatura = $"{weatherData.main.temp}°C", //temperatura en grados centrigrados
                        Imagen = weatherData.weather[0].icon, //icono
                        ClimaDescripcion = weatherData.weather[0].description,//descripcion del clima
                        PorcentajeNubes = $"{weatherData.clouds?.all}%", //porcentaje de nubes
                        PorcentajeHumedad = $"{weatherData.main.humidity}%", //porcentaje de humedad
                        Amanecer = UnixTimeToLocalTime(weatherData.sys.sunrise, weatherData.timezone), //hora amanecer
                        Anocher = UnixTimeToLocalTime(weatherData.sys.sunset, weatherData.timezone) //hora anochecer
                    }
                };

                //retornar como JSON
                return Json(datosRequeridos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar los datos del clima.");

                //retorna un codigo HTTP 500 con un mensaje de error
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        //metodo auxiliar que convierte tiempo UNIX a hora local
        private string UnixTimeToLocalTime(int unixTime, int timezoneOffset)
        {
            //convierte el tiempo UNIX a DateTimeOffset
            var dateTime = DateTimeOffset.FromUnixTimeSeconds(unixTime);

            //ajusta el tiempo segun el huso horario proporcionado
            var localTime = dateTime.AddSeconds(timezoneOffset);

            //retorna la hora en formato HH:mm (horas y minutos dia o noche)
            return localTime.ToString("hh:mm tt");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
