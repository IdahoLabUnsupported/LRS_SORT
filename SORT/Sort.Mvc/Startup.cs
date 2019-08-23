using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Owin;

namespace Sort.Mvc
{
    /// <summary>
    /// Startup
    /// </summary>
    public partial class Startup
    {
        /// <summary>
        /// Configuration
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }

}