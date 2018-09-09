using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SiteTest_Frames
{
	public partial class Frames1 : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Personnel.ucListEmployees le = (Personnel.ucListEmployees)Page.LoadControl("~/Personnel/ucListEmployees.ascx");
			//TopPane.InnerHtml = 
			splitCenter.Panes["TopPane"].Controls.Add(le);
			// Panel.Controls.Add(uc) where uc is UserControl1 uc = Page.LoadControl("~/UserControl1.ascx").
		}
	}
}