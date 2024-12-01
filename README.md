# Proyecto de Desarrollo Web

¡Bienvenido a mi proyecto de desarrollo web! En esta plataforma he implementado tecnologías avanzadas y buenas prácticas de desarrollo, con un enfoque en la seguridad, escalabilidad y facilidad de mantenimiento.

Este proyecto incluye dos componentes principales:

- **Web API** robusta para gestionar los datos, con funciones de autenticación y generación de tokens para usuarios autenticados.
- **Website** interactivo que consume la API, con un diseño amigable y fácil de usar.

## Requisitos

Antes de comenzar, asegúrate de tener los siguientes requisitos instalados en tu entorno local:

- **JavaScript**
- **.NET Core 9**
- - **.NET 4.7.2**
- **Base de datos SQL SERVER 2019**

## Configuración de la Base de Datos

Este proyecto requiere una variable de entorno global para almacenar la cadena de conexión a la base de datos. Aquí te explico cómo configurarla en **Windows**.

### Instrucciones para configurar la variable de entorno en Windows:

1. **Abrir el Panel de Control**:
   - Haz clic derecho sobre el botón de **Inicio** y selecciona **Sistema**.
   - Luego, en la barra lateral, haz clic en **Configuración avanzada del sistema**.

2. **Acceder a las Variables de Entorno**:
   - En la ventana **Propiedades del sistema**, selecciona el botón **Variables de entorno**.

3. **Crear una nueva variable de entorno**:
   - En el cuadro de **Variables de usuario** (o **Variables del sistema** si quieres que sea global para todos los usuarios), haz clic en **Nueva**.
   - En el campo **Nombre de la variable**, escribe: `MY_CONNECTION_STRING`
   - En el campo **Valor de la variable**, ingresa la cadena de conexión a tu base de datos.
     
4. **Guardar los cambios**:
   - Haz clic en **Aceptar** en cada ventana para guardar la configuración.

5. **Reinicia tu terminal o IDE**:
   - Asegúrate de cerrar y volver a abrir cualquier ventana de terminal o entorno de desarrollo para que la nueva variable de entorno se cargue correctamente.

6. **Verificar que la variable se configuró correctamente**:
   - Abre una nueva ventana de **Símbolo del sistema** y escribe:

     ```bash
     echo %MY_CONNECTION_STRING%
     ```

   - Si todo está bien configurado, deberías ver la cadena de conexión que ingresaste.

## Funcionalidades

- **Autenticación y Autorización**: Se requiere un usuario y contraseña para acceder al sitio. El proyecto incluye un respaldo de SQL Server 2019.
- **Gestión de Datos**: La API permite realizar operaciones CRUD (Crear, Leer, Actualizar, Eliminar) sobre usuarios y medicamentos.
- **Generador PDF**: Incluye el consumo de un web service (url incluida en el web.config) que retorna un PDF.
- **API**: El proyecto incluye una API que controla toda la capa de datos.

---

¡Gracias por revisar mi proyecto! Si tienes alguna pregunta, no dudes en abrir un *issue* en el repositorio.
