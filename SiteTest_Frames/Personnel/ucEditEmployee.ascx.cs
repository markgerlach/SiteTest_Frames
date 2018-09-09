using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using LEAD;

namespace SiteTest_Frames.Personnel
{
	public partial class ucEditEmployee : System.Web.UI.UserControl
	{
		//private int? _contactID = null;
		private LEAD.Contact _contact = null;

		public ucEditEmployee() : base()
		{
			// Don't allow this control to load - there was no employee passed
			_contact = null;
		}

		public ucEditEmployee(long parameter) : this()
		{
			// Set the parameter ID, then load the fields
			ContactCollection contacts = new ContactCollection("iContactID = " + parameter);
			_contact = null;
			if (contacts.Count > 0) { _contact = contacts[0]; }
		}

		protected void Page_Init(object sender, EventArgs e)
		{

		}

		protected void Page_Load(object sender, EventArgs e)
		{
			// Set up the form
			if (_contact != null)
			{
				ucContactEdit_txtID.Text = _contact.ContactID.ToString();
				ucContactEdit_txtFirstName.Text = _contact.FirstName;
				ucContactEdit_txtLastName.Text = _contact.LastName;

				//txtID.valu
			}
			else
			{
				ucContactEdit_txtID.Text =
					ucContactEdit_txtFirstName.Text =
					ucContactEdit_txtLastName.Text =
					"<Value Not Found>";
			}
		}
	}
}