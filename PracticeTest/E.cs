#region
using Spxus.Calture.Service;
using Spxus.Core.Calture;
using Spxus.Core.Email;
using Spxus.Email;
using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;
using System.Linq;
using System.Web;

#endregion

namespace System
{
    public static class E
    {
        public static Dictionary<EnvKey, dynamic> Config { get; set; }
        public static ServiceContext Services { get; set; }
        public static ICaltureService Calture { get; set; }
        public static Func<string,string,string> CaltureCnEn
        {
            get
            {
                return (string cnText,string enText)=> 
                {
                    return E.Calture.Text(new Dictionary<CaltureType, string>()
                    {
                        [CaltureType.Chinese] = cnText,
                        [CaltureType.English] = enText
                    });
                };
            }
        }
        public static GetAllContentByType AllContentByType;
        public static GetContentValue<string> ContentValueString;
        public static GetContentValue<int> ContentValueInt;
        public static GetContentValue<bool> ContentValueBool;
        public static IEmailServer EmailService()
        {
            IContentBase home = Services.ContentService.GetById(Config[EnvKey.HomeId]);
            var host = (string)home.GetValue("emailHost");
            var ports = G.Int32((string)home.GetValue("emailPort"), 0);
            var user = (string)home.GetValue("emailUser");
            var password = (string)home.GetValue("emailPassword");
            var ssl = G.ToBool(home.GetValue("emailSsl"));
            IEmailServer es = new SPXEmailServer(host, ports, user, password, ssl);
            return es;
        }
        public static UmbracoHelper Helper
        {
            get
            {
                return new UmbracoHelper(UmbracoContext.Current);
            }
        }
        public static IContent HomePage
        {
            get
            {
                return Services.ContentService.GetById((int)E.Config[EnvKey.HomeId]);
            }
        }
        public static string HomePageString
        {
            get
            {
                return HomePage.ConvertToContentTypeView(false, false, false, E.CurrentUmbracoAccess).ConvertToJson();
            }
        }
        public static Func<IEnumerable<string>, AllowUserAccess> CreatePublicAccessCheck
        {
            get
            {
                return UmbracoContentExtend.CheckPublicAccessFunction(E.Services.PublicAccessService, E.Services.ContentService);
            }
        }
        public static IEnumerable<string> CurrentUserUmbracoRoles
        {
            get
            {
                var umbracoRoles = E.Services.MemberGroupService.GetAll().Select(b => b.Name);
                var currentUser = HttpContext.Current.User;
                if (currentUser == null || HttpContext.Current.User.Identity.IsAuthenticated == false)
                {
                    return new List<string>();
                }
                return umbracoRoles.Where(b => currentUser.IsInRole(b));
            }
        }

        public static AllowUserAccess NoGroupUserAccess
        {
            get
            {
                return E.CreatePublicAccessCheck(new List<string>());
            }
        }
        public static AllowUserAccess CurrentUmbracoAccess
        {
            get
            {
                return E.CreatePublicAccessCheck(E.CurrentUserUmbracoRoles);
            }
        }
        public static void init(int homeID, CaltureType calture)
        {
            Config = new Dictionary<EnvKey, dynamic>()
            {
                [EnvKey.HomeId] = homeID
            };
            Services = ApplicationContext.Current.Services;
            Calture = new CaltureService();
            Calture.DefaultCalture = calture;
            AllContentByType = UmbracoContentExtend.GetAllContentTypeFunction(Services.ContentTypeService, Services.ContentService);
            ContentValueString = UmbracoContentExtend.GetContentValue<string>(Services.ContentService);
            ContentValueInt = UmbracoContentExtend.GetContentValue<int>(Services.ContentService);
            ContentValueBool = UmbracoContentExtend.GetContentValue<bool>(Services.ContentService);
        }

    }
    public enum EnvKey
    {
        HomeId = 1,
    }
}
