using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Prod2Mashup.Helpers
{
    public class ApiHelper
    {
        private readonly HttpClient _httpClient; //cliente HTTP inyectado

        //inyeccion de dependencias
        public ApiHelper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        //metodo para consultar un endpoint
        public async Task<string> ConsumeApiAsync(string url)
        {
            try
            {
                //configuracion de cabeceras 
                _httpClient.DefaultRequestHeaders.Clear(); //limpia las cabeceras
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); //tipo de valores
                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; MyApp/1.0)"); //parseo

                //solicitud HTTP GET al endpoint
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                //valida si la respuesta fue exitosa
                if (response.IsSuccessStatusCode)
                {
                    //retorna la respuesta como una cadena JSON
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    //respuesta no exitosa
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error al consumir la API: {response.StatusCode} - Detalles: {errorMessage}");
                }
            }
            catch (HttpRequestException httpEx)
            {
                //captura errores especificos de la solicitud HTTP
                throw new Exception($"Error en la solicitud HTTP: {httpEx.Message}");
            }
            catch (TaskCanceledException)
            {
                //manejo especifico para tiempo de espera
                throw new Exception("La solicitud ha excedido el tiempo de espera.");
            }
            catch (Exception ex)
            {
                //manejo general de excepciones
                throw new Exception($"Error de conexión o validación: {ex.Message}");
            }
        }
    }
}
