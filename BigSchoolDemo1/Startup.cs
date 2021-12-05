using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BigSchoolDemo1.Startup))]
namespace BigSchoolDemo1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
