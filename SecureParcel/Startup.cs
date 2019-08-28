using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SecureParcel.Startup))]
namespace SecureParcel
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
