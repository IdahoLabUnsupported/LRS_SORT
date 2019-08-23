using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.WsFederation;
using Owin;
using System.Configuration;
using System.IdentityModel.Claims;
using System.Threading.Tasks;

namespace LRS.Mvc
{
    public partial class Startup
    {
        private static string realm = ConfigurationManager.AppSettings["ida:Wtrealm"];
        private static string adfsMetadata = ConfigurationManager.AppSettings["ida:ADFSMetadata"];
        private static string adfsReply = ConfigurationManager.AppSettings["ida:ADFSReply"];

        /// <summary>
        /// ConfigureAuth
        /// </summary>
        /// <param name="app"></param>
        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseKentorOwinCookieSaver();

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseWsFederationAuthentication(
                new WsFederationAuthenticationOptions
                {
                    SignInAsAuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                    Wtrealm = realm,
                    MetadataAddress = adfsMetadata,
                    Wreply = adfsReply,
                    SignOutWreply = adfsReply + "/Account/SignOutCallback",
                    Notifications = new WsFederationAuthenticationNotifications
                    {
                        SecurityTokenValidated = context =>
                        {
                            string accountName = "";
                            System.Security.Claims.Claim nameClaim = null;
                            foreach (var claim in context.AuthenticationTicket.Identity.Claims)
                            {
                                if (claim.Type == ClaimTypes.Upn)
                                    accountName = claim.Value;
                                else if (claim.Type == ClaimTypes.Name)
                                    nameClaim = claim;
                            }

                            if (accountName.Contains("@"))
                            {
                                var parts = accountName.Split(new char[] { '@' });
                                accountName = $"{parts[1]}\\{parts[0]}";
                            }

                            if (nameClaim != null)
                                context.AuthenticationTicket.Identity.RemoveClaim(nameClaim);
                            context.AuthenticationTicket.Identity.AddClaim(new System.Security.Claims.Claim(System.IdentityModel.Claims.ClaimTypes.Name, accountName));
                            context.AuthenticationTicket.Identity.AddClaim(new System.Security.Claims.Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", ClaimTypes.Name));
                            context.AuthenticationTicket.Identity.AddClaim(new System.Security.Claims.Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", adfsMetadata));

                            return Task.FromResult(0);
                        }
                    },
                    UseTokenLifetime = false
                });
        }
    }

}