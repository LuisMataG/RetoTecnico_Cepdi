using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSite.Models;

public class UsuarioController : Controller
{
    private readonly HttpClient _httpClient;

    public UsuarioController(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    #region Métodos Web (Acciones públicas)

    // Acción que muestra la lista de usuarios
    public ActionResult Index()
    {
        string baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
        ViewBag.BaseUrl = baseUrl;
        return View();
    }

    // Acción para cargar la vista en un modal
    public async Task<ActionResult> EditModal(int id)
    {
        if (id > 0)
        {
            var result = await ObtenerUsuarioPorId(id);

            // Verificamos si el JSON contiene el campo 'success' y si es verdadero
            if (result != null && result.Data != null)
            {
                // Si 'success' es true, procesamos los datos del usuario
                var jsonResponse = JsonConvert.SerializeObject(result.Data);

                var jsonObject = JsonConvert.DeserializeObject<JObject>(jsonResponse);

                if (jsonObject["success"].Value<bool>())
                {
                    // Si es exitoso, obtenemos el usuario
                    var usuario = jsonObject["data"].ToObject<Usuario>();

                    // Retornamos la vista parcial con el usuario
                    return PartialView("_EditModal", usuario);
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
            Usuario usuarioNuevo = new Usuario
            {
                IdUsuario = 0,
                Nombre = string.Empty,
                Usuario1 = string.Empty,
                Password = string.Empty,
                FechaCreacion = DateTime.Now,
                IdPerfil = 0,
                Estatus = 2
            };

            // Devolvemos la vista parcial para agregar un nuevo usuario
            return PartialView("_EditModal", usuarioNuevo);
        }
    }

    // Acción para guardar o actualizar datos de usuario
    [HttpPost]
    public async Task<ActionResult> Edit(Usuario usuario)
    {
        if (ModelState.IsValid)
        {
            if (usuario.IdUsuario == 0)
            {
                return await InsertarUsuario(usuario);
            }
            else
            {
                return await ActualizarUsuario(usuario);
            }
        }

        return PartialView("_EditModal", usuario);
    }

    // Acción para eliminar un usuario.
    public async Task<ActionResult> Delete(int id)
    {
        var result = await EliminarUsuario(id);

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
    //Metodo para insertar nuevos usuarios mediante la API.
    private async Task<JsonResult> InsertarUsuario(Usuario usuario)
    {
        try
        {
            usuario.FechaCreacion = DateTime.Now;

            var jsonContent = JsonConvert.SerializeObject(usuario);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync("api/usuarios", content);

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Usuario creado correctamente.", redirectUrl = Url.Action("Index") });
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

    // Método para obtener los usuarios desde la API
    private async Task<JsonResult> ObtenerUsuarios(int page = 1, int pageSize = 10)
    {
        try
        {
            // Construir la URL con los parámetros de paginación
            string url = $"api/usuarios?page={page}&pageSize={pageSize}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                // Retornar la respuesta JSON tal como la recibimos, sin deserializar
                return Json(new { success = true, data = JsonConvert.DeserializeObject<object>(json) }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, message = "Error al obtener los usuarios." }, JsonRequestBehavior.AllowGet);
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


    // Método para obtener un usuario específico desde la API
    private async Task<JsonResult> ObtenerUsuarioPorId(int id)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"api/usuarios/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var usuario = JsonConvert.DeserializeObject<Usuario>(json);

                // Retornar el usuario en caso de éxito
                return Json(new { success = true, data = usuario }, JsonRequestBehavior.AllowGet);
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
    private async Task<JsonResult> ActualizarUsuario(Usuario usuario)
    {
        try
        {
            var jsonContent = JsonConvert.SerializeObject(usuario);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync($"api/usuarios/{usuario.IdUsuario}", content);

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Usuario actualizado correctamente.", redirectUrl = Url.Action("Index") });
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

    //Método para eliminar usuario mediante API
    private async Task<JsonResult> EliminarUsuario(int id)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"api/usuarios/{id}");

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Usuario eliminado correctamente.", redirectUrl = Url.Action("Index") });
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
