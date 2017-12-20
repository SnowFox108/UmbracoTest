using Spxus.Core.Calture;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core;
using Umbraco.Web;

namespace Umbraco.Web
{
    public class CustomUmbracoApplication : UmbracoApplication
    {
        protected override IBootManager GetBootManager()
        {
            return new CustomWebBootManager(this);
        }
        public class CustomWebBootManager : WebBootManager
        {

            public CustomWebBootManager(UmbracoApplicationBase umbracoApplication) : base(umbracoApplication)
            {

            }

            public override IBootManager Complete(Action<ApplicationContext> afterComplete)
            {
                var rootPath = HostingEnvironment.ApplicationPhysicalPath;
                var config = File.OpenRead($@"{rootPath}\Web.config");
                G.setSettingPath(config);
                config.Close();
                var rootid = G.AppSettings("homeID");
                var calture = (CaltureType)G.AppSettings("caltureId").Int32();
                E.init(
                    rootid.Int32(),
                    calture
                );
                AreaRegistration.RegisterAllAreas();
                return base.Complete(afterComplete);
            }

        }
    }
}
