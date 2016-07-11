using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FBIUCRDemo.Startup))]
namespace FBIUCRDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
