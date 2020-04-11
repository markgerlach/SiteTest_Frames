using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SiteTest_Frames.controls
{
	public partial class ucRibbonControl : DevExpress.Web.ASPxRibbon
	{
		//private ;

		protected void Page_Init(object sender, EventArgs e)
		{
			// When the button is created, set up some of the properties
			this.Tabs.Clear();
			this.Tabs.Add(new RibbonTab("_Base", "_Base"));

			// Add in some of the buttons depending on the type
		}

		protected void Page_Load(object sender, EventArgs e)
		{

		}
	}
}