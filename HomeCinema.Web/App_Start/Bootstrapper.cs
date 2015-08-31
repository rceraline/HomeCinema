using System.Web.Http;

namespace HomeCinema.Web
{
    public class Bootstrapper
    {
        public static void Run()
        {
            AutofacWebApiConfig.Initialize(GlobalConfiguration.Configuration);
        }
    }
}