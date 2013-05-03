using System.ServiceModel.Activation;
using System.Web.Mvc;
using System.Web.Routing;
using DFSResolver.APIs;
using Microsoft.ApplicationServer.Http;
using Microsoft.ApplicationServer.Http.Activation;

namespace DFSResolver
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            var config = new HttpConfiguration();
            config.EnableTestClient = true;
            config.EnableHelpPage = true;
            routes.Add(new ServiceRoute("api/resolveDFS", new HttpServiceHostFactory() { Configuration = config }, typeof(DfsResolverApi)));


            routes.MapRoute(
                            "Default", // Route name
                            "{controller}/{action}/{id}", // URL with parameters
                             new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
                           );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
    }
}