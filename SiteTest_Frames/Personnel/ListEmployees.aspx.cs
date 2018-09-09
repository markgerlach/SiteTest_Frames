using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SiteTest_Frames.Personnel
{
	public partial class ListEmployees : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			RouteData rd = HttpContext.Current.Request.RequestContext.RouteData;
			foreach (var urlParm in rd.Values)
			{
				//_sb.Append(String.Format("{0}: {1}<br />", urlParm.Key, urlParm.Value));
				//Console.WriteLine(urlParm.Key + ": " + urlParm.Value);
				//requestContext.HttpContext.Items[urlParm.Key] = urlParm.Value;
			}
		}
	}
}