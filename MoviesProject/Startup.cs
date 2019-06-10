using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MoviesProject.Startup))]
namespace MoviesProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
