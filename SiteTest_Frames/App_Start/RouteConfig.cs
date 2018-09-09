using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Compilation;
using System.Web.Routing;
using System.Web.UI;
using Microsoft.AspNet.FriendlyUrls;

namespace SiteTest_Frames
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            var settings = new FriendlyUrlSettings();
            settings.AutoRedirectMode = RedirectMode.Permanent;
            routes.EnableFriendlyUrls(settings);

			// Add custom routes
			routes.Add("PersonnelListRoute", new Route
			(
			   "Personnel/{pageNum}",
			   new CustomRouteHandler("~/Personnel/ListEmployees.aspx")
			));
		}
    }

	public class CustomRouteHandler : IRouteHandler
	{
		public CustomRouteHandler(string virtualPath)
		{
			this.VirtualPath = virtualPath;
		}

		public string VirtualPath { get; private set; }

		public IHttpHandler GetHttpHandler(RequestContext
			  requestContext)
		{
			var page = BuildManager.CreateInstanceFromVirtualPath
				 (VirtualPath, typeof(Page)) as IHttpHandler;
			return page;
		}
	}
}
