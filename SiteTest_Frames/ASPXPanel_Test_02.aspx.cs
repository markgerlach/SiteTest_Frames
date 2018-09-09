using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using LEAD;
using LEADBase;
using SiteTest_Frames.classes;
using SiteTest_Frames.Personnel;

namespace SiteTest_Frames
{
	public partial class ASPXPanel_Test_02 : System.Web.UI.Page
	{
		protected void Page_Init(object sender, EventArgs e)
		{
			Personnel.ucListEmployees le = (Personnel.ucListEmployees)Page.LoadControl("~/Personnel/ucListEmployees.ascx");
			//TopPane.InnerHtml = 

			mainContent.Controls.Clear();
			le.ID = "ucContactList";
			mainContent.Controls.Add(le);
			//splitCenter.Panes["TopPane"].Controls.Add(le);
			// Panel.Controls.Add(uc) where uc 
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			//Personnel.ucListEmployees le = (Personnel.ucListEmployees)Page.LoadControl("~/Personnel/ucListEmployees.ascx");
			////TopPane.InnerHtml = 

			//mainContent.Controls.Clear();
			//mainContent.Controls.Add(le);
			////splitCenter.Panes["TopPane"].Controls.Add(le);
			//// Panel.Controls.Add(uc) where uc is UserControl1 uc = Page.LoadControl("~/UserControl1.ascx").
		}

		protected void mainContent_Callback(object sender, DevExpress.Web.CallbackEventArgsBase e)
		{
			// When the callback is called, reset just this frame
			mainContent.Controls.Clear();
			List<string> paramCollection = Utils.SplitStringToGenericList(e.Parameter, "||||");
			string param = e.Parameter;
			if (paramCollection.Count > 1) { param = paramCollection[0]; }
			//int test = 1;

			// Get the url that was passed and load the proper control
			string pageTarget = (param.EndsWith("/") ? param.Substring(0, param.Length - 1) : param).ToLower();
			pageTarget = (pageTarget.StartsWith("/") ? pageTarget.Substring(1) : pageTarget);
			List<string> targets = Utils.SplitStringToGenericList(pageTarget, "/");
			string keyVal = string.Empty;
			long? id = null;
			//List<Control> loadedControls = new List<Control>();
			switch (targets[0])
			{
				case "personnel":           // The Personnel functions
					switch (targets[1])
					{
						case "contact":     // /Personnel/Contact
							switch (targets[2])
							{
								case "list":        // /Personnel/Contact/List
									ucListEmployees cntrlList = ((ucListEmployees)UserControlHelper.LoadControl(Page, 
										"~/Personnel/ucListEmployees.ascx", new object[] { })); // Load the user control and place it
									cntrlList.ID = "ucContactList";
									mainContent.Controls.Add(cntrlList);
									break;
								case "edit":
									// Get the ID	// /Personnel/Contact/Edit/-9223372036854775767
									id = long.Parse(targets.Count > 3 ? targets[3] : null);

									// Find out if we're coming back around on an edit
									if (paramCollection.Count > 1)
									{
										keyVal = "ucContactEdit_";

										// Yup - this is saved from an edit
										Dictionary<string, string> kvp = new Dictionary<string, string>();
										List<string> nv = Utils.SplitStringToGenericList(paramCollection[1], "||");
										List<string> tmpSplit = new List<string>();
										foreach (string s in nv)
										{
											tmpSplit = Utils.SplitStringToGenericList(s, "|");      // Split the value
											kvp.Add(tmpSplit[0].Replace(keyVal, string.Empty), 
												(tmpSplit.Count > 1 ? tmpSplit[1] : string.Empty));
											//if (tmpSplit.Count > 1)
											//{
											//	kvp.Add(tmpSplit[0].Replace(keyVal, string.Empty), tmpSplit[1]);
											//}
											//else
											//{
											//	kvp.Add(tmpSplit[0].Replace(keyVal, string.Empty), string.Empty);
											//}
										}

										// Update the value in the db
										ContactCollection coll = new ContactCollection("iContactID = " + id.Value.ToString());
										ClassGenExceptionCollection errors = new ClassGenExceptionCollection();
										if (coll.Count > 0)
										{
											coll[0].FirstName = kvp["txtFirstName"];
											coll[0].LastName = kvp["txtLastName"];
											errors.AddRange(coll.AddUpdateAll());
										}
									}

									// Go out and get the new employee info
									ucEditEmployee cntrlEdit = ((ucEditEmployee)UserControlHelper.LoadControl(Page,
										"~/Personnel/ucEditEmployee.ascx", new object[] { id }));   // Load the user control and place it
									mainContent.Controls.Add(cntrlEdit);
									break;
							}
							break;
						default:
							break;
					}
				
					break;
				default:
					break;
			}
		}
	}
}