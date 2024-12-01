using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebSite.Models;

namespace WebSite.Controllers
{
    public class MedicamentosController : Controller
    {
        private readonly HttpClient _httpClient;
        public MedicamentosController(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }
        #region Métodos Web (Acciones públicas)
        public ActionResult Index()
        {
            string baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
            ViewBag.BaseUrl = baseUrl;
            return View();
        }

        public async Task<ActionResult> ShowModal(int id)
        {
            string baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
            ViewBag.BaseUrl = baseUrl;
            if (id > 0)
            {
                var result = await ObtenerMedicamentoPorId(id);

                // Verificamos si el JSON contiene el campo 'success' y si es verdadero
                if (result != null && result.Data != null)
                {
                    // Si 'success' es true, procesamos los datos del medicamento
                    var jsonResponse = JsonConvert.SerializeObject(result.Data);

                    var jsonObject = JsonConvert.DeserializeObject<JObject>(jsonResponse);

                    if (jsonObject["success"].Value<bool>())
                    {
                        // Deserialización con configuración regional específica
                        var settings = new JsonSerializerSettings
                        {
                            Culture = new CultureInfo("es-US") 
                        };

                        var medicamento = JsonConvert.DeserializeObject<Medicamentos>(jsonObject["data"].ToString(), settings);

                        // Retornamos la vista parcial con el medicamento
                        return PartialView("_ModalMedicamentos", medicamento);
                    }
                    else
                    {
                        var errorMessage = jsonObject["message"]?.ToString() + " - " + jsonObject["errorDetails"]?.ToString() ?? "Error desconocido";
                        ViewData["message"] = errorMessage;
                        return View("Error");
                    }
                }
                else
                {
                    ViewData["message"] = "No se recibio más información.";
                    return View("Error");
                }
            }
            else
            {
                Medicamentos medicamentoNuevo = new Medicamentos
                {
                    // Inicialización con valores predeterminados
                    Idmedicamento = 0,
                    Nombre = string.Empty,
                    Concentracion = string.Empty,
                    Precio = 0.0m,
                    Stock = 0,
                    Presentacion = string.Empty,
                    FormaFarmaceuticaNombre = string.Empty,
                    Idformafarmaceutica = null
                };

                // Devolvemos la vista parcial para agregar un nuevo usuario
                return PartialView("_ModalMedicamentos", medicamentoNuevo);
            }
        }

        public async Task<ActionResult> Modal(Medicamentos medicamento)
        {
            if (ModelState.IsValid)
            {
                if (medicamento.Idmedicamento == 0)
                {
                    return await InsertarMedicamento(medicamento);
                }
                else
                {
                    return await ActualizarMedicamento(medicamento);
                }
            }
            string baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
            ViewBag.BaseUrl = baseUrl;
            return PartialView("_ModalMedicamentos", medicamento);
        }

        public async Task<ActionResult> Delete(int id)
        {
            var result = await EliminarMedicamento(id);

            if (result.Data != null)
            {
                // Si el contenido de result.Data es un JObject, lo procesamos directamente
                var jsonResponse = JsonConvert.SerializeObject(result.Data);

                // Convertimos el JSON string a un JObject para procesarlo
                var jsonObject = JsonConvert.DeserializeObject<JObject>(jsonResponse);

                if (jsonObject["success"].Value<bool>())
                {
                    return Json(new { success = true, message = "OK" });
                }
                else
                {
                    var errorMessage = jsonObject["message"]?.ToString() + " - " + jsonObject["errorDetails"]?.ToString() ?? "Error desconocido";
                    return Json(new { success = false, message = errorMessage });
                }
            }
            else
            {
                return Json(new { success = false, message = "No se recibieron datos." });
            }
        }
        #endregion

        #region Métodos Privados
        private async Task<JsonResult> ObtenerMedicamentoPorId(int id)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"api/medicamentos/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var medicamento = JsonConvert.DeserializeObject<Medicamentos>(json);

                    // Retornar el medicamento en caso de éxito
                    return Json(new { success = true, data = medicamento }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // Si la respuesta no es exitosa, retornamos un JSON con el mensaje de error
                    return Json(new { success = false, message = "No se pudo obtener el usuario." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (HttpRequestException httpEx)
            {
                return ManejarErrores(httpEx);
            }
            catch (TaskCanceledException timeoutEx)
            {
                return ManejarErrores(timeoutEx);
            }
            catch (JsonException jsonEx)
            {
                return ManejarErrores(jsonEx);
            }
            catch (Exception ex)
            {
                return ManejarErrores(ex);
            }
        }
        // Método para actualizar un usuario a través de la API
        private async Task<JsonResult> ActualizarMedicamento(Medicamentos medicamento)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(medicamento);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                
                HttpResponseMessage response = await _httpClient.PutAsync($"api/medicamentos/{medicamento.Idmedicamento}", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Medicamento actualizado correctamente.", redirectUrl = Url.Action("Index") });
                }
                else
                {
                    await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"Error durante la actualización: {response.ReasonPhrase}" });
                }
            }
            catch (HttpRequestException httpEx)
            {
                return ManejarErrores(httpEx);
            }
            catch (TaskCanceledException timeoutEx)
            {
                return ManejarErrores(timeoutEx);
            }
            catch (JsonException jsonEx)
            {
                return ManejarErrores(jsonEx);
            }
            catch (Exception ex)
            {
                return ManejarErrores(ex);
            }
        }
        private async Task<JsonResult> InsertarMedicamento(Medicamentos medicamento)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(medicamento);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync("api/medicamentos", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Se registro " + medicamento.Nombre + " correctamente.", redirectUrl = Url.Action("Index") });
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"Error durante la creación: {response.ReasonPhrase} - {errorMessage}" });
                }
            }
            catch (HttpRequestException httpEx)
            {
                return ManejarErrores(httpEx);
            }
            catch (TaskCanceledException timeoutEx)
            {
                return ManejarErrores(timeoutEx);
            }
            catch (JsonException jsonEx)
            {
                return ManejarErrores(jsonEx);
            }
            catch (Exception ex)
            {
                return ManejarErrores(ex);
            }
        }
        private async Task<JsonResult> EliminarMedicamento(int id)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.DeleteAsync($"api/medicamentos/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Medicamento eliminado correctamente.", redirectUrl = Url.Action("Index") });
                }
                else
                {
                    await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"Error durante la eliminación: {response.ReasonPhrase}" });
                }
            }
            catch (HttpRequestException httpEx)
            {
                return ManejarErrores(httpEx);
            }
            catch (TaskCanceledException timeoutEx)
            {
                return ManejarErrores(timeoutEx);
            }
            catch (JsonException jsonEx)
            {
                return ManejarErrores(jsonEx);
            }
            catch (Exception ex)
            {
                return ManejarErrores(ex);
            }
        }
        #endregion

        #region ManejoErrores
        private JsonResult ManejarErrores(Exception ex)
        {
            // Definimos valores por defecto para el mensaje y detalles de error
            string message = ex.Message;
            string errorDetails = ex.Message;
            string stackTrace = ex.StackTrace;

            // Identificamos el tipo de excepción para personalizar los mensajes
            if (ex is HttpRequestException)
            {
                message = "Problema de conexión con el servidor.";
            }
            else if (ex is TaskCanceledException)
            {
                message = "La solicitud ha excedido el tiempo de espera.";
            }
            else if (ex is JsonException)
            {
                message = "Error al procesar la respuesta del servidor.";
                stackTrace = (ex as JsonException)?.StackTrace; // Obtenemos stack trace si es JsonException
            }

            // Retornamos un Json con la estructura estandarizada
            return Json(new
            {
                success = false,
                message = message,
                errorDetails = errorDetails,
                stackTrace = stackTrace
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }


}