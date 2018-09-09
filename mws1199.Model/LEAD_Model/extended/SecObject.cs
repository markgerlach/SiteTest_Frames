using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

using LEADBase;

namespace LEAD
{
	/// <summary>
	/// Class Name: SecObject
	/// This class can be modified by the user and will not be written over if the class is already found.
	/// Generated by GenClasses on 04/06/2017 01:40 PM
	/// </summary>
	public partial class SecObject
	{
	}

	public partial class SecObjectCollection : ClassGenBindingList<SecObject, long>
	{
		/// <summary>
		/// Get the text from Sec Object where the key is the value passed
		/// </summary>
		/// <param name="key">The key to look up</param>
		/// <returns>The value returned</returns>
		public static string GetTextByKey(string key)
		{
			string rtv = string.Empty;
			SecObjectCollection objColl = new LEAD.SecObjectCollection("sKeyName = '" + key + "'");
			if (objColl.Count > 0) { rtv = objColl[0].Text ; }
			return rtv;
		}
	}
}
