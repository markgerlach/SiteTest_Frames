using LEAD;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SiteTest_Frames.Personnel
{
	public partial class ucListEmployees : System.Web.UI.UserControl
	{
		private DataTable _dt;

		protected void Page_Load(object sender, EventArgs e)
		{
			// Load up the data table
			if (_dt == null) { LoadDataTable(); }

			// Load up the grid
			gridViewEmployees.DataSource = _dt;
			gridViewEmployees.DataBind();
		}

		// Load up the data table
		protected void LoadDataTable()
		{
			_dt = new DataTable();
			_dt.Columns.Add("ContactID", typeof(System.Int64));
			_dt.Columns.Add("LastName", typeof(System.String));
			_dt.Columns.Add("FirstName", typeof(System.String));

			// Get the contact collection
			ContactCollection coll = new ContactCollection(string.Empty);
			coll.Sort(LEAD.Contact.FN_FullName_LastFirst);

			// Add them into the local datatable
			foreach (LEAD.Contact c in coll)
			{
				_dt.Rows.Add(new object[]
				{
					c.ContactID,
					c.LastName,
					c.FirstName,
				});
			}

			// Code below on ToDataTable is really slow
			//DataTable dtContact = coll.ToDataTable();
			//for (int i = dtContact.Columns.Count - 1; i >= 0; i--)
			//{
			//	if (dtContact.Columns[i].ColumnName.ToLower().IndexOf("contactid") == -1 &&
			//		dtContact.Columns[i].ColumnName.ToLower().IndexOf("lastname") == -1 &&
			//		dtContact.Columns[i].ColumnName.ToLower().IndexOf("firstname") == -1)
			//	{
			//		dtContact.Columns.RemoveAt(i);
			//	}
			//}
			//_dt = dtContact;

			//for (int i = 0; i < 100; i++)
			//{
			//	_dt.Rows.Add(new object[]
			//	{
			//		i.ToString("##0"),
			//		String.Format("Last Name: {0}", i.ToString("##0")),
			//		String.Format("First Name: {0}", i.ToString("##0")),
			//	});
			//}
		}
	}
}