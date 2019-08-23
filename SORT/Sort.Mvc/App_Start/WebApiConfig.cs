using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Cors;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Sort.Mvc.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //config.EnableCors();
            //config.SetCorsPolicyProviderFactory(new DynamicPolicyProviderFactory());

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

        public class DynamicPolicyProviderFactory : ICorsPolicyProviderFactory
        {
            public ICorsPolicyProvider GetCorsPolicyProvider(
                HttpRequestMessage request)
            {
                var route = request.GetRouteData();
                var controller = (string)route.Values["controller"];
                var corsRequestContext = request.GetCorsRequestContext();
                var originRequested = corsRequestContext.Origin;
                var policy = GetPolicyForControllerAndOrigin(controller, originRequested);
                return new CustomPolicyProvider(policy);
            }

            private CorsPolicy GetPolicyForControllerAndOrigin(string controller, string originRequested)
            {
                var policy = new CorsPolicy()
                {
                    AllowAnyHeader = true,
                    AllowAnyMethod = true,
                    SupportsCredentials = true
                };
                policy.Origins.Add(originRequested);

                return policy;
            }
        }
        public class CustomPolicyProvider : ICorsPolicyProvider
        {
            CorsPolicy policy;
            public CustomPolicyProvider(CorsPolicy policy)
            {
                this.policy = policy;
            }
            public Task<CorsPolicy> GetCorsPolicyAsync(
                HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(this.policy);
            }
        }
    }
}