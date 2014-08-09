using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ToDoService.Startup))]
namespace ToDoService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
