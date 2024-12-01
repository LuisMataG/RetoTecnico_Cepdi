using System;
using System.Configuration;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Unity;
using Unity.AspNet.Mvc;
using Unity.Lifetime;

namespace WebSite
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Crear el contenedor de Unity
            // ------------------------------------------------
            // Se crea una nueva instancia del contenedor de dependencias Unity.
            // UnityContainer es el objeto responsable de gestionar las dependencias en la aplicación.
            // Usando este contenedor, se registran los servicios y las dependencias que necesitamos 
            // para que sean inyectados automáticamente donde sea necesario.
            var container = new UnityContainer();

            // Registrar los servicios necesarios con Unity
            // ------------------------------------------------
            // En este paso, le indicamos al contenedor qué servicios (clases, interfaces, etc.) queremos que 
            // administre y que esté disponible para inyectarse en otros objetos, como los controladores.
            // Por ejemplo, si tenemos un servicio que interactúa con una API (como un servicio `HttpClient`),
            // lo registramos en Unity para que esté disponible para cualquier clase que lo necesite.
            RegisterServices(container);

            // Establecer el resolver de dependencias de MVC para usar Unity
            // ------------------------------------------------------------
            // Aquí le decimos a ASP.NET MVC que use Unity como su proveedor de dependencias para 
            // resolver automáticamente las dependencias de los controladores.
            // El DependencyResolver es un objeto que gestiona la creación de controladores y otras clases 
            // necesarias, y UnityDependencyResolver es un adaptador que permite que Unity trabaje con MVC.
            // Con esto, MVC utilizará el contenedor de Unity para crear instancias de controladores y 
            // automáticamente inyectar las dependencias necesarias (como servicios o repositorios).
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            CultureInfo cultureInfo = new CultureInfo("es-MX");
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }


        // Método para registrar los servicios con Unity
        // ------------------------------------------------
        // Este método se utiliza para registrar los servicios o dependencias en el contenedor de Unity.
        // El contenedor de Unity será el responsable de administrar la creación y ciclo de vida de estos servicios.
        private void RegisterServices(IUnityContainer container)
        {
            // Registra HttpClient usando RegisterFactory en lugar de InjectionFactory
            // ------------------------------------------------
            // Aquí estamos utilizando el método RegisterFactory para registrar el servicio de HttpClient.
            // RegisterFactory es útil cuando se necesita crear una instancia de un objeto con lógica personalizada, 
            // como es el caso de la creación de HttpClient, en la cual configuramos el `BaseAddress` con la URL base desde web.config.
            container.RegisterFactory<HttpClient>(c =>
            {
                // Lee la URL base desde el archivo de configuración
                // -------------------------------------------------
                // Utilizamos ConfigurationManager para acceder a los valores almacenados en el archivo de configuración web.config.
                var apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];

                if (string.IsNullOrEmpty(apiBaseUrl))
                {
                    throw new InvalidOperationException("La URL base de la API no está configurada en el archivo web.config.");
                }

                // Devuelve una nueva instancia de HttpClient con la base address configurada
                // ------------------------------------------------
                // En este paso, creamos una nueva instancia de HttpClient y configuramos su propiedad `BaseAddress`
                // con la URL base que hemos obtenido de web.config. Esta URL se utilizará para todas las solicitudes HTTP 
                // que se realicen mediante este HttpClient.
                return new HttpClient
                {
                    BaseAddress = new Uri(apiBaseUrl)  // Configuración de la URL base
                };
            },
            // Usar Singleton (contener la misma instancia de HttpClient)
            // ------------------------------------------------
            // Usamos `ContainerControlledLifetimeManager` para asegurarnos de que Unity cree una única instancia de HttpClient.
            // Esto es útil porque HttpClient debe ser reutilizado y no se recomienda crear nuevas instancias repetidamente.
            // El `ContainerControlledLifetimeManager` asegura que la misma instancia se utilice durante toda la vida de la aplicación
            // (es decir, se comporta como un Singleton).
            new ContainerControlledLifetimeManager());
        }

    }
}
