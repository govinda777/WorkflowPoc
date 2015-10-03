using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WorkflowPod.Startup))]
namespace WorkflowPod
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
