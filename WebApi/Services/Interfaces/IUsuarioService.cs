using WebApi.Models;

namespace WebApi.Services.Interfaces
{
    // Interface que define los métodos relacionados con la gestión de usuarios.
    // Se utiliza para obtener información sobre los usuarios de la aplicación.
    public interface IUsuarioService
    {
        // Obtiene un usuario en función del nombre de usuario proporcionado.
        // Este método es asincrónico y devuelve un Task que contiene el objeto Usuario
        // correspondiente al nombre de usuario dado, o null si no se encuentra el usuario.        
        Task<Usuario> GetUsuarioByUsernameAsync(string username);
    }
}
