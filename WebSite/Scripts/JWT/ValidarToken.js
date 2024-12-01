function validateToken() {
    // Recuperar el token de sessionStorage
    let token = sessionStorage.getItem('authToken');

    // Verificar si estamos en la p�gina de Login y si ya hay un token
    if (window.location.pathname.toLowerCase().includes('login')  && token) {
        // Si el token existe y estamos en la p�gina de login, redirigir a Home/Index
        window.location.href = '/Home/Index';
        return;  // Salir de la funci�n, ya que no necesitamos hacer m�s validaciones
    }

    // Si no estamos en Login, validamos el token
    if (!token && !window.location.pathname.toLowerCase().includes('login')) {
        // Si no existe el token, redirigir a la p�gina de login
        window.location.href = '/Login/Index';
        return;
    } else if (!window.location.pathname.toLowerCase().includes('login')) {
        // Usar jwt-decode para decodificar el token y verificar si ha expirado
        try {
            const decodedToken = jwt_decode(token);  // Decodificar el token
            const currentTime = Math.floor(Date.now() / 1000); // Tiempo actual en segundos

            // Verificar si el token ha expirado
            if (decodedToken.exp < currentTime) {
                console.log('Token valido');
                // El token ha expirado, redirigir a la p�gina de login
                window.location.href = '/Login/Index';
            }
        } catch (e) {
            if (e.message.includes("invalid token")) {
                console.log("Token inv�lido");
                //window.location.href = '/Login/Index';  // Redirigir al login
            } else {
                console.log('Erro - [' + e.name + '] - [' + e.message + ']');
            }
        }
    }

    
}