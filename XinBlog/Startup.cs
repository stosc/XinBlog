using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(XinBlog.Startup))]
namespace XinBlog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
