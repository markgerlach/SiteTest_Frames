using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.IO;

using LEADBase;

namespace LEAD
{
	public class Security
	{
		#region static events
		public delegate void BeforeEncrypting(int recordCount);
		public delegate void EncryptionProgress(int progress);
		public delegate void AfterEncrypting(int recordCount);

		public static event BeforeEncrypting BeforeEncryptingRecs;
		public static event EncryptionProgress EncryptionProgressRecs;
		public static event AfterEncrypting AfterEncryptingRecs;

		public static void OnBeforeEncrypting(int recordCount)
		{
			if (BeforeEncryptingRecs != null)
			{
				BeforeEncryptingRecs(recordCount);
			}
		}
		public static void OnEncryptionProgress(int progress)
		{
			if (EncryptionProgressRecs != null)
			{
				EncryptionProgressRecs(progress);
			}
		}
		public static void OnAfterEncrypting(int recordCount)
		{
			if (AfterEncryptingRecs != null)
			{
				AfterEncryptingRecs(recordCount);
			}
		}
		#endregion static events

		#region static member variables
		public static User CurrentUser = new User();

		private static string _password = "13FC1A64EBAC4089A98330AD4314725D";
		private static string _fieldEncSupportListName = "SecurityEncryptedField";
		#endregion

		#region constructors
		public Security()
		{
		}
		#endregion constructors

		public static BeforeEncrypting beforeEncrypting;

		/// <summary>
		/// Encrypts cleartext and returns the cipherText
		/// </summary>
		/// <param name="clearText"></param>
		/// <returns>Encrypted String</returns>
		public static string EncryptString(string clearText, ref ClassGenExceptionCollection errors)
		{
			string rtv = string.Empty;
			try
			{
				rtv = mws1199.Model.Crypto.EncryptDecrypt.Encrypt(clearText, _password);
			}
			catch (Exception ex) { errors.Add(new ClassGenException(ex)); }

			return rtv;
		}

		/// <summary>
		/// Decrypts cipherText and returns a clearText string
		/// </summary>
		/// <param name="cipherText"></param>
		/// <returns>Decrypted string</returns>
		public static string DecryptString(string cipherText, ref ClassGenExceptionCollection errors)
		{
			string rtv = string.Empty;
			if (!String.IsNullOrEmpty(cipherText))
			{
				try
				{
					rtv = mws1199.Model.Crypto.EncryptDecrypt.Decrypt(cipherText, _password);
				}
				catch (Exception ex) { errors.Add(new ClassGenException(ex)); }
			}

			return rtv;
		}

		/// <summary>
		/// Returns true if the specified field name is encrypted.  Throws error if the field name is invalid.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public static bool FieldIsEncrypted(string fieldName, ref ClassGenExceptionCollection errors)
		{
			bool rtv = false;
			try
			{
				//SystemSettingCollection encFields = GlobalCollections.AgencySettings.GetByAgencyCategory(AgencySetting.SecurityEncryptedField);
				SystemSettingCollection encFields = new SystemSettingCollection("sCategory = 'SecurityEncryptedField'");
				bool found = false;
				foreach (SystemSetting encField in encFields)
				{
					if (encField.Key.ToLower() == fieldName.ToLower())
					{
						found = true;
						rtv = (encField.DefaultStringValueDesc.ToLower() == "true");
						break;
					}
				}
				if (!found)
				{
					throw new Exception("The specified field is not available for encryption.");
				}
			}
			catch (Exception err) { errors.Add(new ClassGenException(err)); }

			return rtv;
		}

		///// <summary>
		///// Applies the update to a single field.  Value can be cleartext or encrypted.  Database will be updated to that value.
		///// </summary>
		///// <param name="fieldName"></param>
		///// <param name="key"></param>
		///// <param name="value"></param>
		//public static ClassGenExceptionCollection UpdateEncryptedField(string fieldName, 
		//    System.Collections.Specialized.NameValueCollection itemList)
		//{
		//    SqlTransaction trans = null;
		//    SqlConnection con = new SqlConnection(DAL.ConnString);
		//    SqlCommand cmd = con.CreateCommand();
		//    ClassGenExceptionCollection errors = new ClassGenExceptionCollection();

		//    try
		//    {
		//        con.Open();
		//        cmd.CommandType = CommandType.StoredProcedure;
		//        cmd.CommandText = "[dbo].[spr_SecEncUpdateItem]";
		//        cmd.Parameters.Add("@psFieldName", SqlDbType.VarChar, 30).Value = fieldName;
		//        cmd.Parameters.Add("@psKey", SqlDbType.UniqueIdentifier);
		//        cmd.Parameters.Add("@psValue", SqlDbType.VarChar, 100);
		//        trans = con.BeginTransaction(IsolationLevel.Serializable, "EncryptedFieldUpdate");
		//        cmd.Connection = con;
		//        cmd.Transaction = trans;

		//        OnBeforeEncrypting(itemList.Count);

		//        for (int i = 0; i < itemList.Count; i++)
		//        {
		//            OnEncryptionProgress(i);
		//            cmd.Parameters["@psKey"].Value = new Guid(itemList.GetKey(i));
		//            cmd.Parameters["@psValue"].Value = itemList.Get(i);
		//            cmd.ExecuteNonQuery();
		//        }

		//        // FLIP THE ENCRYPTION FLAG FOR THIS FIELD NAME
		//        cmd.Parameters.Clear();
		//        cmd.CommandText = "[dbo].[spr_SecEncUpdateFieldStatus]";
		//        cmd.Parameters.Add("@psListName", SqlDbType.VarChar, 100).Value = _fieldEncSupportListName;
		//        cmd.Parameters.Add("@psFieldName", SqlDbType.VarChar, 100).Value = fieldName;
		//        cmd.Parameters.Add("@psValueDesc", SqlDbType.VarChar, 100).Value = !Security.FieldIsEncrypted(fieldName, ref errors);
		//        cmd.ExecuteNonQuery();

		//        OnAfterEncrypting(itemList.Count);

		//        trans.Commit();
		//        trans.Dispose();
		//        con.Close();
		//        con.Dispose();
		//        cmd.Dispose();
		//    }
		//    catch (Exception ex)
		//    {

		//        errors.Add(new ClassGenException(ex));

		//        try
		//        {
		//            trans.Rollback("EncryptedFieldUpdate");
		//            trans.Dispose();
		//            con.Close();
		//            con.FDispose();
		//            cmd.Dispose();
		//        }
		//        catch (Exception ex2)
		//        {
		//            errors.Add(new ClassGenException(ex2));
		//        }
		//    }

		//    return errors;		// Return the error collection
		//}

		/// <summary>
		/// Encrypt or Decrypt the field specified
		/// </summary>
		/// <param name="fld">The field to encrypt/decrypt</param>
		/// <param name="action">The action to encrypt/decrypt</param>
		public ClassGenExceptionCollection EncryptDecryptField(EncryptedField fld, EncryptAction action)
		{
			ClassGenExceptionCollection errors = new ClassGenExceptionCollection();
			int count = 0;

			try
			{
				switch (fld)
				{
					case EncryptedField.Password:
						UserCollection users = new UserCollection("sPassword IS NOT NULL");
						if (action == EncryptAction.Decrypt)
						{
							// Decrypt the field in the table
							foreach (User u in users)
							{
								if (!String.IsNullOrEmpty(u.Password) &&
									u.Password.EndsWith("="))
								{
									// Field is encrypted and is valid - decrypt it
									u.Password = Security.DecryptString(u.Password, ref errors);
								}

								// Send out an event
								this.OnDetailPopulate("User", "Decrypt User Password", string.Empty, users.Count, count + 1);
								count++;
							}
						}
						if (action == EncryptAction.Encrypt)
						{
							// Encrypt the field in the table
							foreach (User u in users)
							{
								if (!String.IsNullOrEmpty(u.Password))
								{
									// Field is not encrypted
									u.Password = Security.EncryptString(u.Password, ref errors);
								}

								// Send out an event
								this.OnDetailPopulate("User", "Encrypt User Password", string.Empty, users.Count, count + 1);
								count++;
							}
						}

						// Send the updates off to the database
						errors.AddRange(users.AddUpdateAll());

						break;
				}
			}
			catch (Exception ex)
			{
				errors.Add(new ClassGenException(ex));
			}

			return errors;		// Return the errors collection
		}

		private void contacts_DetailUpdate(object sender, PopulateUpdateDelete_EventArgs e)
		{
			this.OnDetailPopulate(e.Name, e.Description, e.Errors, e.TotalCount * 2, e.CurrentIndex + e.TotalCount);
		}

		public delegate void DetailPopulateEventHandler(object sender, PopulateUpdateDelete_EventArgs e);
		public event DetailPopulateEventHandler DetailPopulate;
		protected void OnDetailPopulate(string name, string description, string errors, int totalCount, int currentIndex)
		{
			if (DetailPopulate != null)
			{
				PopulateUpdateDelete_EventArgs e = new PopulateUpdateDelete_EventArgs(name, description, errors, totalCount, currentIndex);
				DetailPopulate(this, e);
			}
		}

		///// <summary>
		///// Returns a field for encryption or decryption
		///// </summary>
		///// <param name="fieldName"></param>
		///// <returns></returns>
		//public static DataTable GetEncryptedField(string fieldName, ref ClassGenExceptionCollection errors)
		//{
		//    DataTable dt = new DataTable("EncryptedFieldValues");
		//    try
		//    {
		//        SqlCommand cmd = new SqlCommand("[dbo].[spr_SecEncListitems]");
		//        cmd.Parameters.Add("@psFieldName", SqlDbType.VarChar, 30).Value = fieldName;

		//        dt = DAL.SQLExecDataTable(cmd);
		//        cmd.Dispose();
		//    }
		//    catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
		//    catch (Exception err) { errors.Add(new ClassGenException(err)); }

		//    return dt;
		//}

		/// <summary>
		/// Check to see if anyone exists with this username.  Overloaded.
		/// </summary>
		/// <param name="userName">The username to check</param>
		/// <returns>True if at least one name exists, otherwise, false</returns>
		public static bool UserNameExists(string userNameToCheck, ref ClassGenExceptionCollection errors)
		{
			//DataTable dt = null;
			//SqlCommand cmd = null;
			bool rtv = true;

			try
			{
				UserCollection users = new UserCollection("sUserName = '" + userNameToCheck.Replace("'", "''") + "'");
				rtv = (users.Count > 0);
				//cmd = new SqlCommand("[dbo].[spr_UserCheckLoginExists]");
				//cmd.Parameters.Add("@psUserNameToCheck", SqlDbType.VarChar, 50).Value = userNameToCheck;
				//dt = DAL.SQLExecDT(cmd);

				//if (dt.Rows.Count > 0 && dt.Rows[0][0] != DBNull.Value)
				//{
				//    rtv = bool.Parse(dt.Rows[0][0].ToString());
				//}
				//dt.Dispose();
			}
			catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
			catch (Exception err) { errors.Add(new ClassGenException(err)); }

			return rtv;
		}

		/// <summary>
		/// Check to see if anyone exists with this username, excluding the specified userGUID.  Overloaded.
		/// </summary>
		/// <param name="userNameToCheck">The username to check</param>
		/// <param name="userGUID">Exclude this user from the username check.</param>
		/// <returns>bool</returns>
		public static bool UserNameExists(string userNameToCheck, string userGUID, ref ClassGenExceptionCollection errors)
		{
			//DataTable dt = null;
			//SqlCommand cmd = null;
			bool rtv = true;

			try
			{
				UserCollection users = new UserCollection("sUserGUID = '" + userGUID + "' AND sUserName = '" + userNameToCheck.Replace("'", "''") + "'");
				rtv = (users.Count > 0);
				//cmd = new SqlCommand("[dbo].[spr_UserCheckLoginExists]");
				//cmd.Parameters.Add("@psUserGUID", SqlDbType.UniqueIdentifier).Value = new Guid(userGUID);
				//cmd.Parameters.Add("@psUserNameToCheck", SqlDbType.VarChar, 50).Value = userNameToCheck;
				//dt = DAL.SQLExecDT(cmd);

				//if (dt.Rows.Count > 0 && dt.Rows[0][0] != DBNull.Value)
				//{
				//    rtv = bool.Parse(dt.Rows[0][0].ToString());
				//}
				//dt.Dispose();
			}
			catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
			catch (Exception err) { errors.Add(new ClassGenException(err)); }

			return rtv;
		}

		/// <summary>
		/// Checks whether a security group already exists
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		public static bool SecGroupExists(string groupName, ref ClassGenExceptionCollection errors)
		{
			DataTable dt = null;
			SqlCommand cmd = null;
			string sql = string.Empty;
			int count = 0;

			try
			{
				sql = "SELECT COUNT(*) FROM tSecGroup " +
					"WHERE sDesc = @psDesc";
				cmd = new SqlCommand(sql);
				cmd.Parameters.Add("@psDesc", SqlDbType.VarChar, 400).Value = groupName;
				dt = DAL.SQLExecDT(cmd);
				if (dt.Rows.Count > 0 && dt.Rows[0][0] != DBNull.Value)
				{
					count = int.Parse(dt.Rows[0][0].ToString());
				}
				dt.Dispose();
			}
			catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
			catch (Exception err) { errors.Add(new ClassGenException(err)); }

			return (count > 0);
		}

		///// <summary>
		///// Get the user information from the database
		///// </summary>
		///// <param name="userGUID">The userGUID to find</param>
		///// <returns>A user object that has the information</returns>
		//public static User GetUser(string userGUID)
		//{
		//    User user = new User();
		//    user.Get(userGUID);
		//    return user;
		//}

		///// <summary>
		///// Get the user information from the database
		///// </summary>
		///// <param name="userName">The userName to find</param>
		///// <param name="userPass">The userPass to find</param>
		///// <returns>A user object that has the information</returns>
		//public static User GetUser(string userName, string userPass, ref ClassGenExceptionCollection errors)
		//{
		//    User user = new User();
		//    string p = userPass;
		//    if (Security.FieldIsEncrypted("Password", ref errors))
		//    {
		//        p = Security.EncryptString(userPass, ref errors);
		//    }
		//    user.Get(userName, p);
		//    return user;
		//}

		///// <summary>
		///// Get the groups from the database
		///// </summary>
		///// <returns>A datatable containing the groups</returns>
		//public static DataTable GetGroups(ref ClassGenExceptionCollection errors)
		//{
		//    SqlCommand cmd = null;
		//    DataTable dt = null;

		//    try
		//    {
		//        cmd = new SqlCommand("[dbo].[spr_SecGroupList]");
		//        #region commented code
		//        //				sql = "SELECT sGroupGUID, sDesc, ISNULL(iSortOrder, 99999) AS iSortOrder, bRowversion "  +
		//        //					"FROM tSecGroup ORDER BY iSortOrder, sDesc, sGroupGUID";
		//        #endregion commented code
		//        dt = DAL.SQLExecDataTable(cmd);
		//        cmd.Dispose();
		//    }
		//    catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
		//    catch (Exception err) { errors.Add(new ClassGenException(err)); }

		//    return dt;
		//}

		/// <summary>
		/// Delete the user at the attached GUID.
		/// Physically removes the user from the database.
		/// </summary>
		/// <param name="userGUID">The user GUID to delete</param>
		public static ClassGenExceptionCollection UserDelete(string userGUID)
		{
			// Delete the user
			ClassGenExceptionCollection errors = new ClassGenExceptionCollection();
			try
			{
				UserCollection.Delete("sUserGUID = '" + userGUID + "'", ref errors);
			}
			catch (Exception err) { errors.Add(new ClassGenException(err)); }
			return errors;
		}

		/// <summary>
		/// Delete all users from the specified group
		/// </summary>
		/// <param name="groupGUID">The group guid to use</param>
		/// <returns>The number of users who were deleted from the group</returns>
		public static int DeleteAllUsersFromGroup(string groupGUID, ref ClassGenExceptionCollection errors)
		{
			string sql = string.Empty;
			int recsAffected = 0;

			try
			{
				sql = "DELETE FROM tSecUserToGroup WHERE sGroupGUID = '" + groupGUID + "'";
				recsAffected = DAL.SQLExecNonQuery(sql);
			}
			catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
			catch (Exception err) { errors.Add(new ClassGenException(err)); }

			return recsAffected;
		}

		/// <summary>
		/// Add all users to the specified group
		/// </summary>
		/// <param name="groupGUID">The group to add the users to</param>
		/// <returns>The number of users added</returns>
		public static int AddAllUsersToGroup(string groupGUID, ref ClassGenExceptionCollection errors)
		{
			string sql = string.Empty;
			int recsAffected = 0;

			try
			{
				sql = "INSERT INTO tSecUserToGroup (sUserGUID, sGroupGUID, sUpdatedByUser) " +
					"SELECT sUserGUID, '" + groupGUID + "', '" +
					Security.CurrentUser.UserName.Replace("'", "''") + "' " +
					"FROM tUser ORDER BY sUserName";
				recsAffected = DAL.SQLExecNonQuery(sql);
			}
			catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
			catch (Exception err) { errors.Add(new ClassGenException(err)); }

			return recsAffected;
		}

		/// <summary>
		/// Add the user(s) to the specified group
		/// </summary>
		/// <param name="groupGUID">The group to add the user to</param>
		/// <param name="aryUsers">An Arraylist of user guids to add</param>
		/// <returns>The number of records affected</returns>
		public static int AddUsersToGroup(string groupGUID, ArrayList aryUsers, ref ClassGenExceptionCollection errors)
		{
			string sql = string.Empty;
			int recsAffected = 0;

			try
			{
				for (int i = 0; i < aryUsers.Count; i++)
				{
					sql = "INSERT INTO tSecUserToGroup (sUserGUID, sGroupGUID, sUpdatedByUser) " +
						"SELECT '" + aryUsers[i].ToString() + "', '" + groupGUID + "', '" +
						Security.CurrentUser.UserName.Replace("'", "''") + "' ";
					int count = DAL.SQLExecNonQuery(sql);
					recsAffected++;
				}
			}
			catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
			catch (Exception err) { errors.Add(new ClassGenException(err)); }

			return recsAffected;
		}

		/// <summary>
		/// Delete the user(s) from the specified group
		/// </summary>
		/// <param name="groupGUID">The group to delete the user from</param>
		/// <param name="aryUsers">An Arraylist of user guids to delete</param>
		/// <returns>The number of records affected</returns>
		public static int DeleteUsersFromGroup(string groupGUID, ArrayList aryUsers, ref ClassGenExceptionCollection errors)
		{
			string sql = string.Empty;
			int recsAffected = 0;

			try
			{
				for (int i = 0; i < aryUsers.Count; i++)
				{
					sql = "DELETE FROM tSecUserToGroup WHERE sGroupGUID = '" + groupGUID +
						"' AND sUserGUID = '" + aryUsers[i].ToString() + "'";
					int count = DAL.SQLExecNonQuery(sql);
					recsAffected++;
				}
			}
			catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
			catch (Exception err) { errors.Add(new ClassGenException(err)); }

			return recsAffected;
		}

		/// <summary>
		/// Delete all groups from the user
		/// </summary>
		/// <param name="userGUID">The user to delete all the groups for</param>
		/// <returns>The number of records affected</returns>
		public static int DeleteAllGroupsFromUser(string userGUID, ref ClassGenExceptionCollection errors)
		{
			string sql = string.Empty;
			int recsAffected = 0;

			try
			{
				sql = "DELETE FROM tSecUserToGroup WHERE sUserGUID = '" + userGUID + "'";
				recsAffected = DAL.SQLExecNonQuery(sql);
			}
			catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
			catch (Exception err) { errors.Add(new ClassGenException(err)); }

			return recsAffected;
		}

		/// <summary>
		/// Add all groups to the user
		/// </summary>
		/// <param name="userGUID">The user to add all the groups for</param>
		/// <returns>The number of records affected</returns>
		public static int AddAllGroupsToUser(string userGUID, ref ClassGenExceptionCollection errors)
		{
			string sql = string.Empty;
			int recsAffected = 0;

			try
			{
				sql = "INSERT INTO tSecUserToGroup (sGroupGUID, sUserGUID, sUpdatedByUser) " +
					"SELECT sGroupGUID, '" + userGUID + "', '" +
					Security.CurrentUser.UserName.Replace("'", "''") + "' " +
					"FROM tSecGroup ORDER BY iSortOrder, sDesc, sGroupGUID";
				recsAffected = DAL.SQLExecNonQuery(sql);
			}
			catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
			catch (Exception err) { errors.Add(new ClassGenException(err)); }

			return recsAffected;
		}

		/// <summary>
		/// Add specific groups to the user.
		/// Overridden.
		/// </summary>
		/// <param name="userGUID">The user to add the groups for</param>
		/// <param name="aryGroups">The array of group guids to use</param>
		/// <returns>The number of records affected</returns>
		public static int AddGroupsToUser(string userGUID, ArrayList aryGroups, ref ClassGenExceptionCollection errors)
		{
			string sql = string.Empty;
			int recsAffected = 0;

			try
			{
				for (int i = 0; i < aryGroups.Count; i++)
				{
					sql = "INSERT INTO tSecUserToGroup (sGroupGUID, sUserGUID, sUpdatedByUser) " +
						"SELECT '" + aryGroups[i].ToString() + "', '" + userGUID + "', '" +
						Security.CurrentUser.UserName.Replace("'", "''") + "' ";
					int count = DAL.SQLExecNonQuery(sql);
					recsAffected++;
				}
			}
			catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
			catch (Exception err) { errors.Add(new ClassGenException(err)); }

			return recsAffected;
		}

		/// <summary>
		/// Delete specific groups from the user
		/// </summary>
		/// <param name="userGUID">The user to delete the groups for</param>
		/// <param name="aryGroups">The array of group guids to use</param>
		/// <returns>The number of records affected</returns>
		public static int DeleteGroupsFromUser(string userGUID, ArrayList aryGroups, ref ClassGenExceptionCollection errors)
		{
			string sql = string.Empty;
			int recsAffected = 0;

			try
			{
				for (int i = 0; i < aryGroups.Count; i++)
				{
					sql = "DELETE FROM tSecUserToGroup WHERE sUserGUID = '" + userGUID +
						"' AND sGroupGUID = '" + aryGroups[i].ToString() + "'";
					int count = DAL.SQLExecNonQuery(sql);
					recsAffected++;
				}
			}
			catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
			catch (Exception err) { errors.Add(new ClassGenException(err)); }

			return recsAffected;
		}

		///// <summary>
		///// Get the assigned users based on the group
		///// </summary>
		///// <param name="groupGUID">The group GUID to look up</param>
		///// <returns>Get the listing of assigned users</returns>
		//public static DataTable GetAssignedUsers(string groupGUID, ref ClassGenExceptionCollection errors)
		//{
		//    string sql = string.Empty;
		//    DataTable dt = null;
		//    SqlCommand cmd = null;

		//    try
		//    {
		//        // Assigned first
		//        cmd = new SqlCommand("[dbo].[spr_SecGroupListAssignedUsers]");
		//        cmd.Parameters.Add("@psGroupGUID", SqlDbType.UniqueIdentifier).Value = new Guid(groupGUID);
		//        dt = DAL.SQLExecDataTable(cmd);
		//        cmd.Dispose();
		//        #region commented code
		//        //				sql = "SELECT sUserGUID, sLastName + ', ' + sFirstName + ' (' + sUserName + ')' AS sName " + 
		//        //					"FROM tUser INNER JOIN tContact ON tUser.sContactGUID = tContact.sContactGUID WHERE sUserGUID IN " +
		//        //					"(SELECT sUserGUID FROM tSecUserToGroup WHERE sGroupGUID IN " + 
		//        //					"('" + groupGUID + "')) ORDER BY sLastName, sFirstName, sUserName";
		//        //				dt = DAL.SQLExecDataTable(sql);
		//        #endregion commented code
		//    }
		//    catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
		//    catch (Exception err) { errors.Add(new ClassGenException(err)); }

		//    return dt;
		//}

		///// <summary>
		///// Get the available users based on the group
		///// </summary>
		///// <param name="groupGUID">The group GUID to look up</param>
		///// <param name="nameFilter">An optional filter value for Last Name / User Name</param>
		///// <returns>Get a listing of available users</returns>
		//public static DataTable GetAvailableUsers(string groupGUID, string nameFilter, ref ClassGenExceptionCollection errors)
		//{
		//    //string sql = string.Empty;
		//    SqlCommand cmd = null;
		//    DataTable dt = null;

		//    try
		//    {
		//        // Now, available
		//        cmd = new SqlCommand("[dbo].[spSecGroupListAvailableUsers]");
		//        cmd.Parameters.Add("@psGroupGUID", SqlDbType.UniqueIdentifier).Value = new Guid(groupGUID);
		//        if (!String.IsNullOrEmpty(nameFilter))
		//        {
		//            cmd.Parameters.Add("@psNameFilter", SqlDbType.VarChar, 15).Value = nameFilter;
		//        }
		//        dt = DAL.SQLExecDataTable(cmd);
		//        cmd.Dispose();
		//        #region commented code
		//        //				sql = "SELECT sUserGUID, sLastName + ', ' + sFirstName + ' (' + sUserName + ')' AS sName " + 
		//        //					"FROM tUser INNER JOIN tContact ON tUser.sContactGUID = tContact.sContactGUID WHERE sUserGUID NOT IN " +
		//        //					"(SELECT sUserGUID FROM tSecUserToGroup WHERE sGroupGUID IN " + 
		//        //					"('" + groupGUID + "')) ORDER BY sLastName, sFirstName, sUserName";
		//        //				dt = DAL.SQLExecDataTable(sql);
		//        #endregion commented code
		//    }
		//    catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
		//    catch (Exception err) { errors.Add(new ClassGenException(err)); }

		//    return dt;
		//}

		/// <summary>
		/// Returns security table for a specified object
		/// </summary>
		/// <param name="Type"></param>
		/// <param name="Show"></param>
		/// <param name="ObjectGUID"></param>
		/// <returns>Single datatable</returns>
		public static DataTable GetSecurityForObject(string type, 
			string show, 
			string objectGUID, 
			string filterByGroupOrUser, 
			ref ClassGenExceptionCollection errors)
		{
			DataTable dt = null;
			SqlCommand cmd = null;

			try
			{
				cmd = new SqlCommand("[dbo].[spGetSecurityForObject]");
				cmd.Parameters.Add("@psType", SqlDbType.VarChar, 100).Value = type;
				cmd.Parameters.Add("@psObjectGUID", SqlDbType.UniqueIdentifier).Value = new Guid(objectGUID);
				cmd.Parameters.Add("@psShow", SqlDbType.VarChar, 100).Value = show;
				cmd.Parameters.Add("@psFilterByGroupOrUser", SqlDbType.VarChar, 200).Value = filterByGroupOrUser;
				dt = DAL.SQLExecDataTable(cmd);
				cmd.Dispose();
			}
			catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
			catch (Exception err) { errors.Add(new ClassGenException(err)); }

			return dt;
		}

		/// <summary>
		/// Returns security table for a specified object
		/// </summary>
		/// <param name="Type"></param>
		/// <param name="Show"></param>
		/// <param name="GroupGUID"></param>
		/// <returns>Single datatable</returns>
		public static DataTable GetSecurityForGroup(string type, 
			string show, 
			string groupGUID, 
			string filterByGroupOrUser,
			ref ClassGenExceptionCollection errors)
		{
			DataTable dt = null;
			SqlCommand cmd = null;

			try
			{
				cmd = new SqlCommand("[dbo].[spGetSecurityForObject]");
				cmd.Parameters.Add("@psType", SqlDbType.VarChar, 100).Value = type;
				cmd.Parameters.Add("@psGroupGUID", SqlDbType.UniqueIdentifier).Value = new Guid(groupGUID);
				//				cmd.Parameters.Add("@psShow", SqlDbType.VarChar, 100).Value = show;
				cmd.Parameters.Add("@psFilterByGroupOrUser", SqlDbType.VarChar, 200).Value = filterByGroupOrUser;
				dt = DAL.SQLExecDataTable(cmd);
				cmd.Dispose();
			}
			catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
			catch (Exception err) { errors.Add(new ClassGenException(err)); }

			return dt;
		}

		/// <summary>
		/// Returns security table for a specified object
		/// </summary>
		/// <param name="Type"></param>
		/// <param name="Show"></param>
		/// <param name="UserGUID"></param>
		/// <returns>Single datatable</returns>
		public static DataTable GetSecurityForUser(string type, 
			string show, 
			string userGUID, 
			string filterByGroupOrUser,
			ref ClassGenExceptionCollection errors)
		{
			DataTable dt = null;
			SqlCommand cmd = null;

			try
			{
				cmd = new SqlCommand("[dbo].[spGetSecurityForObject]");
				cmd.Parameters.Add("@psType", SqlDbType.VarChar, 100).Value = type;
				cmd.Parameters.Add("@psUserGUID", SqlDbType.UniqueIdentifier).Value = new Guid(userGUID);
				//				cmd.Parameters.Add("@psShow", SqlDbType.VarChar, 100).Value = show;
				cmd.Parameters.Add("@psFilterByGroupOrUser", SqlDbType.VarChar, 200).Value = filterByGroupOrUser;
				dt = DAL.SQLExecDataTable(cmd);
				cmd.Dispose();
			}
			catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
			catch (Exception err) { errors.Add(new ClassGenException(err)); }

			return dt;
		}

		/// <summary>
		/// Get the security users and/or groups depending on the value passed
		/// </summary>
		/// <param name="type">user = Users, group = Groups, lname = Last names</param>
		/// <returns>A data table containing the users/groups </returns>
		public static DataTable GetSecurityUsersGroups(string type, ref ClassGenExceptionCollection errors)
		{
			string sql = string.Empty;
			DataTable dt = null;

			try
			{
				// Check to make sure the group doesn't exist in the database yet
				if (type.ToLower().StartsWith("user"))
				{
					sql = "SELECT sUserGUID, sUserName " +
						"FROM tUser " +
						"WHERE sContactGUID NOT IN " +
						"	(SELECT sContactGUID FROM tContact WHERE iStatusCode = 1) " +
						"ORDER BY sUserName";
				}
				if (type.ToLower().StartsWith("group"))
				{
					sql = "SELECT sGroupGUID, sDesc " +
						"FROM tSecGroup " +
						"WHERE bSystemGroup = 0 " +
						"ORDER BY sDesc";
				}
				if (type.ToLower().StartsWith("lname"))
				{
					sql = "SELECT sUserGUID, sLastName + ', ' + sFirstName + ' (' + sUserName + ')' " +
						"FROM tContact " +
						"INNER JOIN tUser ON tContact.sContactGUID = tUser.sContactGUID " +
						"WHERE tContact.iStatusCode <> 1 " +
						"ORDER BY sLastName, sFirstName, sUserName";
				}

				//TODO: Change code so that a SqlCommand and parameterized query is used instead of text.
				//NOTE: A stored procdedure should be used where possible.
				dt = DAL.SQLExecDataTable(sql);
			}
			catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
			catch (Exception err) { errors.Add(new ClassGenException(err)); }

			return dt;
		}

		/// <summary>
		/// Returns a list of navbuttons for a single screen.
		/// </summary>
		/// <param name="objectGUID">The objectGUID (parent) to load the buttons for</param>
		/// <returns>SecObjectCollection</returns>
		public static SecObjectCollection GetNavButtons(string objectGUID, ref ClassGenExceptionCollection errors)
		{
			SecObjectCollection coll = null;

			try
			{
				coll = new SecObjectCollection("sType = 'NavBar' AND bInactive = 0 AND sParentObjectGUID = '" + objectGUID + "'");
				if (coll.Count == 0) { coll = null; }
			}
			catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
			catch (Exception err) { errors.Add(new ClassGenException(err)); }

			return coll;
		}

		///// <summary>
		///// Returns a single Navbutton
		///// </summary>
		///// <param name="screenName"></param>
		///// <param name="keyName"></param>
		///// <returns>DataRow</returns>
		//public static DataRow GetNavButton(string screenName, string keyName, ref ClassGenExceptionCollection errors)
		//{
		//    SqlCommand cmd = null;
		//    DataRow row = null;

		//    try
		//    {
		//        cmd = new SqlCommand("[dbo].[spr_SecNavButtonListOne]");
		//        cmd.Parameters.Add("@psScreenName", SqlDbType.VarChar, 200).Value = screenName;
		//        cmd.Parameters.Add("@psKeyName", SqlDbType.VarChar, 400).Value = keyName;
		//        row = DAL.SQLExecDataRow(cmd);
		//        cmd.Dispose();
		//    }
		//    catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
		//    catch (Exception err) { errors.Add(new ClassGenException(err)); }

		//    return row;
		//}

		///// <summary>
		///// Gets the ObjectGUID, Screen Name and Key Name (Screen + Control Name)
		///// of the NavBar Objects from tSecObjects
		///// </summary>
		///// <returns>A DataTable with all the information</returns>
		//public static DataTable GetNavBarElements(ref ClassGenExceptionCollection errors)
		//{
		//    return GetNavBarElements(string.Empty, ref errors);
		//}

		///// <summary>
		///// Gets the ObjectGUID, Screen Name and Key Name (Screen + Control Name)
		///// of the NavBar Objects from tSecObjects
		///// </summary>
		///// <returns>A DataTable with all the information</returns>
		//public static DataTable GetNavBarElements(string bandKeyName, ref ClassGenExceptionCollection errors)
		//{
		//    SqlCommand cmd = null;
		//    DataTable dt = null;

		//    try
		//    {
		//        cmd = new SqlCommand("[dbo].[spr_SecNavBarList]");
		//        cmd.Parameters.Add("@psNavBarBandKeyName", SqlDbType.VarChar, 100).Value = bandKeyName;
		//        dt = DAL.SQLExecDataTable(cmd);
		//        cmd.Dispose();
		//    }
		//    catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
		//    catch (Exception err) { errors.Add(new ClassGenException(err)); }

		//    return dt;
		//}

		///// <summary>
		///// Returns a single NavBar element
		///// </summary>
		///// <param name="key"></param>
		///// <returns></returns>
		//public static DataRow GetNavBarElement(string navExplorerName, ref ClassGenExceptionCollection errors)
		//{
		//    SqlCommand cmd = null;
		//    DataRow row = null;

		//    try
		//    {
		//        cmd = new SqlCommand("[dbo].[spr_SecNavBarListOne]");
		//        cmd.Parameters.Add("@psNavExplorerName", SqlDbType.VarChar, 100).Value = navExplorerName;
		//        row = DAL.SQLExecDataRow(cmd);
		//        cmd.Dispose();
		//    }
		//    catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
		//    catch (Exception err) { errors.Add(new ClassGenException(err)); }

		//    return row;
		//}

		///// <summary>
		///// Get the object GUID based on the current menu item
		///// </summary>
		//public static string GetObjectGUIDFromMenu(string menuName, ref ClassGenExceptionCollection errors)
		//{
		//	//string sql = string.Empty;
		//	//SqlCommand cmd = null;
		//	//DataTable dt = null;
		//	string objectGUID = string.Empty;

		//	try
		//	{
		//		NavBarViewCollection coll = new NavBarViewCollection("sMenuName = '" + menuName.Replace("'", "''") + "'");
		//		if (coll.Count > 0)
		//		{
		//			objectGUID = coll[0].ObjectGUID;
		//		}
		//		//sql = "SELECT sObjectGUID FROM tSecObject WHERE sMenuName = @psMenuName";
		//		//cmd = new SqlCommand(sql);
		//		//cmd.Parameters.Add("@psMenuName", SqlDbType.VarChar, 200).Value = menuName;
		//		//dt = DAL.SQLExecDT(cmd);
		//		//if (dt.Rows.Count > 0 &&
		//		//    dt.Rows[0][0] != DBNull.Value)
		//		//{
		//		//    objectGUID = dt.Rows[0][0].ToString();
		//		//}
		//		//dt.Dispose();
		//	}
		//	catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
		//	catch (Exception err) { errors.Add(new ClassGenException(err)); }

		//	return objectGUID;
		//}

		///// <summary>
		///// Get the object GUID based on the current navbar item
		///// </summary>
		//public static string GetObjectGUIDFromNavBar(string navbarName, ref ClassGenExceptionCollection errors)
		//{
		//	//string sql = string.Empty;
		//	//SqlCommand cmd = null;
		//	//DataTable dt = null;
		//	string objectGUID = string.Empty;

		//	try
		//	{
		//		NavBarViewCollection coll = new NavBarViewCollection("sNavBarKey = '" + navbarName.Replace("'", "''") + "'");
		//		if (coll.Count > 0)
		//		{
		//			objectGUID = coll[0].ObjectGUID;
		//		}
		//		//sql = "SELECT sObjectGUID FROM tSecObject WHERE sNavExplorerName = @psNavBarName";
		//		//cmd = new SqlCommand(sql);
		//		//cmd.Parameters.Add("@psNavBarName", SqlDbType.VarChar, 200).Value = navbarName;
		//		//dt = DAL.SQLExecDT(cmd);
		//		//if (dt.Rows.Count > 0 &&
		//		//    dt.Rows[0][0] != DBNull.Value)
		//		//{
		//		//    objectGUID = dt.Rows[0][0].ToString();
		//		//}
		//		//dt.Dispose();
		//	}
		//	catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
		//	catch (Exception err) { errors.Add(new ClassGenException(err)); }

		//	return objectGUID;
		//}

		///// <summary>
		///// Update the access level on the object
		///// </summary>
		///// <param name="accessLevel">The access level to update </param>
		///// <param name="linkGUID">The link GUID to look for</param>
		///// <returns>The number of records affected</returns>
		//public static int SecGroupToObjectAddUpdateAccessLevel(int accessLevel, string linkGUID, ref ClassGenExceptionCollection errors)
		//{
		//	string sql = string.Empty;
		//	int recsAffected = 0;

		//	try
		//	{
		//		sql = "UPDATE tSecGroupToObject SET iAccessLevel = " +
		//			accessLevel + " WHERE sLinkGUID = '" + linkGUID + "'";
		//		recsAffected = DAL.SQLExecNonQuery(sql);
		//	}
		//	catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
		//	catch (Exception err) { errors.Add(new ClassGenException(err)); }

		//	return recsAffected;
		//}

		///// <summary>
		///// Delete from SecGroupToObject the values that have an accessLevel less than 0
		///// </summary>
		///// <returns>The number of records affected</returns>
		//public static int SecGroupToObjectDeleteInvalid(ref ClassGenExceptionCollection errors)
		//{
		//	string sql = string.Empty;
		//	int recsAffected = 0;

		//	try
		//	{
		//		sql = "DELETE FROM tSecGroupToObject WHERE iAccessLevel < 0";
		//		recsAffected = DAL.SQLExecNonQuery(sql);
		//	}
		//	catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
		//	catch (Exception err) { errors.Add(new ClassGenException(err)); }

		//	return recsAffected;
		//}

		///// <summary>
		///// See if the label control exists on the current form
		///// </summary>
		///// <param name="controlName">The control to find the associated label for</param>
		///// <returns>The label control</returns>
		//public static Control FindControl(System.Windows.Forms.Control.ControlCollection cntrls, string controlName)
		//{
		//    bool found = false;
		//    Control retVal = null;

		//    // Get the target control
		//    string targetControlName = string.Empty;
		//    for (int i = 0; i < controlName.Length; i++)
		//    {
		//        int asciiValue = (int)Convert.ToChar(controlName.Substring(i, 1));
		//        if (asciiValue >= 65 &&
		//            asciiValue <= 90)
		//        {
		//            targetControlName = controlName.Substring(i);
		//            break;
		//        }
		//    }

		//    // Find the control
		//    foreach (Control cntrl in cntrls)
		//    {
		//        // Get the control name
		//        if (cntrl.Name.ToLower() == "lbl" + targetControlName.ToLower())
		//        {
		//            found = true;
		//            retVal = cntrl;
		//            break;
		//        }

		//        if (cntrl.HasChildren)
		//        {
		//            retVal = FindControl(cntrl.Controls, controlName);
		//            if (retVal != null) { found = true; }
		//        }

		//        if (found) { break; }
		//    }

		//    return retVal;		// Return the final value
		//}

		/// <summary>
		/// Check's the user's login and sends back a confirmation of the login
		/// </summary>
		/// <param name="userName">The username to validate</param>
		/// <param name="userPass">The password to validate</param>
		/// <param name="createLoginIfNotExists">Create the login if it doesn't exist - default in the proc is false</param>
		/// <param name="retVal">The return value for the function</param>
		/// <param name="loginType">The login type (Standard or Limited)</param>
		/// <param name="userGUID">The User GUID that's found in the DB and passed back</param>
		/// <returns>A string containing the error if there was a problem with 
		/// the login, or a zero-length string if there was no problem</returns>
		public static string CheckLoginConsolidated(string userName,
			string userPass,
			bool createLoginIfNotExists,
			out int retVal,
			//out string loginType,
			out string userGUID,
			ref ClassGenExceptionCollection errors)
		{
			string error = string.Empty;
			DataTable dt = null;
			SqlCommand cmd = null;

			retVal = -1;
			//loginType = string.Empty;
			userGUID = string.Empty;

			int accountLockoutLimit = 4;

			try
			{
				string p = string.Empty;

				//// Add the accounts for max security
				//Dictionary<string, string> bdadmins = new Dictionary<string, string>();
				//bdadmins.Add("mgerlachadmin", "e$2435");
				//bdadmins.Add("jeropkinadmin", "j9*90j");
				//bdadmins.Add("jblohmadmin", "shasta1");
				//bdadmins.Add("bdadmin", "u76ty&");
				//bdadmins.Add("rribbadmin", "ta38tE");

				//if (Utils.DictionaryContainsStringKey<string>(bdadmins, userName))
				//{
				//    // Get the value
				//    foreach (KeyValuePair<string, string> kvp in bdadmins)
				//    {
				//        if (kvp.Key.ToLower() == userName)
				//        {
				//            p = kvp.Value;
				//            break;
				//        }
				//    }

				//    // Check the pwd
				//    if (!String.IsNullOrEmpty(p) &&
				//        p == userPass)
				//    {
				//        // We're good - they've got an admin pwd here
				//        retVal = 0;
				//        loginType = "Standard";
				//        userGUID = "ABCDEABC-DEAB-CDEA-BCDE-ABCDEABCDEAB";

				//        // Dump out of the function
				//        return string.Empty;
				//    }
				//    else
				//    {
				//        retVal = 2;
				//        error += "Invalid Password - Please try again...";
				//        return error;
				//    }
				//}

				// Go out and check out the username and password and verify that they're a valid user
				if (Security.UserNameExists(userName, ref errors))
				{
					p = (Security.FieldIsEncrypted("Password", ref errors) ? Security.EncryptString(userPass, ref errors) : userPass);
				}
				else
				{
					// They're trying the limited login function
					//if (InitConfig.InitListGetDefaultValue("SecurityLimitedLoginUserFieldType").ToLower() == "ssn")
					//if (GlobalCollections.AgencySettings.GetDefaultStringByCategoryKeyAsString("SecurityLimitedLoginUserFieldType",
					//    "SecurityLimitedLoginUserFieldType").ToLower() == "ssn")
					//{
					//    p = (Security.FieldIsEncrypted("SSN") ? Security.EncryptString(userPass) : userPass);
					//}
					//else
					//{
					//    p = userPass;
					//}
				}

				// GET THE ADMIN CONFIGURED LOCKOUT LIMIT
				//int.TryParse(InitConfig.InitListGetDefaultValue("SecurityLoginAccountLockoutLimit"), out accountLockoutLimit);
				accountLockoutLimit = Utils.SecurityLoginAccountLockoutLimit;
				if (accountLockoutLimit <= 0) { accountLockoutLimit = 4; }  // SET IT TO DEFAULT = 4 IF <= ZERO OR NOT FOUND

				cmd = new SqlCommand("[dbo].[spSecUserLoginConsolidated]");
				cmd.Parameters.Add("@psUserName", SqlDbType.VarChar, 100).Value = userName;
				cmd.Parameters.Add("@psPassword", SqlDbType.VarChar, 100).Value = p;
				//cmd.Parameters.Add("@piMaxLoginAttempts", SqlDbType.Int).Value = accountLockoutLimit;
				cmd.Parameters.Add("@pbCreateUserAcct", SqlDbType.Bit).Value = createLoginIfNotExists;
				//cmd.Parameters.Add("@psLimitedLoginType", SqlDbType.VarChar, 50).Value =
				//    InitConfig.InitListGetDefaultValue("SecurityLimitedLoginUserFieldType").ToLower();
				//if (GlobalCollections.AgencySettings.Count > 0)
				//{
				//    cmd.Parameters.Add("@psLimitedLoginType", SqlDbType.VarChar, 50).Value =
				//        GlobalCollections.AgencySettings.GetDefaultStringByCategoryKeyAsString("SecurityLimitedLoginUserFieldType",
				//        "SecurityLimitedLoginUserFieldType").ToLower();
				//}
				//else
				//{
				//    SystemSettingCollection coll = new SystemSettingCollection("sCategory = 'SecurityLimitedLoginUserFieldType' and sKey = 'SecurityLimitedLoginUserFieldType'");
				//    if (coll.Count > 0)
				//    {
				//        cmd.Parameters.Add("@psLimitedLoginType", SqlDbType.VarChar, 50).Value = coll[0].DefaultStringValue.ToLower();
				//    }
				//}
				dt = DAL.SQLExecDT(cmd);

				retVal = -1;
				if (dt.Rows.Count > 0)
				{
					DataRow row = dt.Rows[0];

					// Set the User GUID
					userGUID = (row["sUserGUID"] != DBNull.Value ? row["sUserGUID"].ToString() : "");

					//if (row["sLoginMethod"] != DBNull.Value &&
					//    row["sLoginMethod"].ToString().ToLower().StartsWith("standard"))
					//{
					//    loginType = "Standard";
					//}
					//else
					//{
					//    loginType = "Limited";
					//}

					// Get the return value
					retVal = int.Parse(row["iRetVal"].ToString());
					switch (retVal)
					{
						case 0:		// Successful login
							// RESET BACKDOOR ADMINISTRATORS PERMISSIONSs
							//ResetBDAPermissions();
							break;
						case 1:		// Invalid UserName
							error += "Invalid UserName - Please try again...";
							break;
						case 2:		// Invalid Password
							error += "Invalid Password - Please try again...";
							break;
						case 3:		// Account Locked
							error += "Your account has been locked out.  This can either be due to excessive logins " +
								"or an administrative lockout.\n\nPlease contact your system administrator as soon " +
								"as possible.";
							break;
						case 4:		// Account Inactive
							error += "Your account has been deactivated.  Please contact your system administrator to " +
								"reactivate your account.";
							break;
					}
				}
				dt.Dispose();
			}
			catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
			catch (Exception err) { errors.Add(new ClassGenException(err)); }

			return error;		// Return the error string
		}

		///// <summary>
		///// Check's the user's limited login (SSN and DOB) and sends back a confirmation of the login
		///// </summary>
		///// <param name="ssn">The SSN to validate</param>
		///// <param name="dob">The DOB to validate</param>
		///// <param name="userGUID">The UserGUID to Look up</param>
		///// <returns>A string containing the error if there was a problem with 
		///// the login, or a zero-length string if there was no problem</returns>
		//public static string CheckLimitedLogin(string ssn, string dob, out string userGUID, ref ClassGenExceptionCollection errors)
		//{
		//    string error = string.Empty;
		//    string sql = string.Empty;
		//    DataTable dt = null;
		//    //int recsAffected = 0;
		//    int accountLockoutLimit = 4;

		//    userGUID = string.Empty;

		//    try
		//    {
		//        // GET THE ADMIN CONFIGURED LOCKOUT LIMIT
		//        //int.TryParse(InitConfig.InitListGetDefaultValue("SecurityLoginAccountLockoutLimit"), out accountLockoutLimit);
		//        accountLockoutLimit = Utils.SecurityLoginAccountLockoutLimit;
		//        if (accountLockoutLimit <= 0) { accountLockoutLimit = 4; }  // SET IT TO DEFAULT = 4 IF <= ZERO OR NOT FOUND


		//        // Check to see if they have a valid date here
		//        DateTime dateDOB;

		//        if (!DateTime.TryParse(dob, out dateDOB) || dateDOB < DateTime.Parse("1900-01-01"))
		//        {
		//            error += "Invalid Password - Please try again..."; // Invalid password
		//            return error;
		//        }

		//        string s = Validation.FormatSSN(ssn);

		//        if (Security.FieldIsEncrypted("SSN"))
		//        {
		//            s = Security.EncryptString(s);
		//        }

		//        SqlCommand cmd = new SqlCommand("[dbo].[spr_UserLoginLimited]");
		//        cmd.Parameters.Add("@psSSN", SqlDbType.VarChar, 100).Value = s;
		//        cmd.Parameters.Add("@pdtDOB", SqlDbType.DateTime).Value = dateDOB;

		//        dt = DAL.SQLExecDT(cmd);

		//        if (dt.Rows.Count > 0)
		//        {
		//            DataRow row = dt.Rows[0];

		//            // Set the User GUID
		//            userGUID = (row["sUserGUID"] != DBNull.Value ? row["sUserGUID"].ToString() : "");
		//            string userName = (row["sUserName"].ToString());

		//            if (bool.Parse(row[0].ToString()))
		//            {

		//                // Check to make sure the account is active and hasn't exceeded the maximum login attempts
		//                if (int.Parse(row["iLoginAttempts"].ToString()) > accountLockoutLimit ||
		//                    bool.Parse(row["bInactive"].ToString()))
		//                {
		//                    error += "This account has been locked. This can due to excessive failed login attempts or an administrative lockout.\nPlease contact your system administrator to unlock your account.";
		//                }
		//                else
		//                {
		//                    // Password invalid
		//                    if (!bool.Parse(row[1].ToString()))
		//                    {
		//                        error += "Invalid Password - Please try again...";
		//                    }
		//                    else
		//                    {
		//                        // WERE GOOD WITH PWD, ACTIVE USER, NOT LOCKED OUT
		//                        ResetLoginAttempts(userName, "System Process");
		//                        // RESET BACKDOOR ADMINISTRATORS PERMISSIONSs
		//                        //ResetBDAPermissions();
		//                    }
		//                }
		//            }
		//            else	// There was no one with that username
		//            {
		//                error += "Invalid UserName - Please try again...";
		//            }
		//        }
		//        dt.Dispose();
		//    }
		//    catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
		//    catch (Exception err) { errors.Add(new ClassGenException(err)); }

		//    return error;		// Return the error string
		//}

		/// <summary>
		/// Resets the logincount to 0
		/// </summary>
		/// <param name="userName">UserName to reset</param>
		public static ClassGenExceptionCollection ResetLoginAttempts(string userName, string updatedByUser)
		{
			SqlCommand cmd = null;
			ClassGenExceptionCollection errors = new ClassGenExceptionCollection();

			try
			{
				cmd = new SqlCommand("[dbo].[spSecUserResetLoginAttempts]");
				cmd.Parameters.Add("@psUserName", SqlDbType.VarChar, 50).Value = userName;

				int recsAffected = DAL.SQLExecNonQuery(cmd);
				cmd.Dispose();
			}
			catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
			catch (Exception err) { errors.Add(new ClassGenException(err)); }

			return errors;
		}

		///// <summary>
		///// Return a datatable with menu items based on the current user
		///// </summary>
		///// <returns>A datatable with menu items and access levels</returns>
		//public static DataTable AggregateMenu(ref ClassGenExceptionCollection errors)
		//{
		//	SqlCommand cmd = null;
		//	DataTable dt = null;

		//	try
		//	{
		//		//cmd = new SqlCommand("[dbo].[spGetSecurityAggregateMenu]");
		//		cmd = new SqlCommand("[dbo].[spSecAggregateList]");
		//		cmd.Parameters.Add("@psType", SqlDbType.VarChar, 50).Value = "Menu";
		//		cmd.Parameters.Add("@psUserGUID", SqlDbType.UniqueIdentifier).Value = new Guid(Security.CurrentUser.UserGUID);
		//		//				sql = "EXEC spGetSecurityAggregateMenu " + 
		//		//					"@psUserGUID='" + Security.CurrentUser.UserGUID + "'";
		//		dt = DAL.SQLExecDataTable(cmd);
		//		cmd.Dispose();
		//	}
		//	catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
		//	catch (Exception err) { errors.Add(new ClassGenException(err)); }

		//	return dt;
		//}

		///// <summary>
		///// Return a datatable with nav bar elements based on the current user
		///// </summary>
		///// <returns>A datatable with nav bar elements and access levels</returns>
		//public static DataTable AggregateNavBar(ref ClassGenExceptionCollection errors)
		//{
		//	//string sql = string.Empty;
		//	SqlCommand cmd = null;
		//	DataTable dt = null;

		//	try
		//	{
		//		cmd = new SqlCommand("[dbo].[spSecAggregateList]");
		//		cmd.Parameters.Add("@psType", SqlDbType.VarChar, 50).Value = "NavBar";
		//		cmd.Parameters.Add("@psUserGUID", SqlDbType.UniqueIdentifier).Value = new Guid(Security.CurrentUser.UserGUID);
		//		cmd.Parameters.Add("@pbForWeb", SqlDbType.Bit).Value = false;
		//		//sql = "EXEC spGetSecurityAggregateNavBar " + 
		//		//"@psUserGUID='" + Security.CurrentUser.UserGUID + "'";
		//		dt = DAL.SQLExecDataTable(cmd);
		//		cmd.Dispose();
		//	}
		//	catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
		//	catch (Exception err) { errors.Add(new ClassGenException(err)); }

		//	return dt;
		//}

		///// <summary>
		///// Return a datatable with screen elements based on the current user
		///// </summary>
		///// <returns>A datatable with screen elements and access levels</returns>
		//public static DataTable AggregateScreens(ref ClassGenExceptionCollection errors)
		//{
		//	//string sql = string.Empty;
		//	SqlCommand cmd = null;
		//	DataTable dt = null;

		//	try
		//	{
		//		cmd = new SqlCommand("[dbo].[spSecAggregateList]");
		//		cmd.Parameters.Add("@psType", SqlDbType.VarChar, 50).Value = "Screen";
		//		cmd.Parameters.Add("@psUserGUID", SqlDbType.UniqueIdentifier).Value = new Guid(Security.CurrentUser.UserGUID);
		//		cmd.Parameters.Add("@pbForWeb", SqlDbType.Bit).Value = false;
		//		dt = DAL.SQLExecDataTable(cmd);
		//		cmd.Dispose();
		//	}
		//	catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
		//	catch (Exception err) { errors.Add(new ClassGenException(err)); }

		//	return dt;
		//}

		///// <summary>
		///// Return a datatable with nav button elements based on the current user
		///// </summary>
		///// <returns>A datatable with nav button elements and access levels</returns>
		//public static DataTable AggregateNavButtons(ref ClassGenExceptionCollection errors)
		//{
		//	//string sql = string.Empty;
		//	SqlCommand cmd = null;
		//	DataTable dt = null;

		//	try
		//	{
		//		cmd = new SqlCommand("[dbo].[spSecAggregateList]");
		//		cmd.Parameters.Add("@psType", SqlDbType.VarChar, 50).Value = "NavBar";
		//		cmd.Parameters.Add("@psUserGUID", SqlDbType.UniqueIdentifier).Value = new Guid(Security.CurrentUser.UserGUID);
		//		cmd.Parameters.Add("@pbForWeb", SqlDbType.Bit).Value = false;
		//		dt = DAL.SQLExecDataTable(cmd);
		//		cmd.Dispose();
		//	}
		//	catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
		//	catch (Exception err) { errors.Add(new ClassGenException(err)); }

		//	return dt;
		//}

		/// <summary>
		/// Decrypts the specified fieldname in the referenced DataTable
		/// </summary>
		/// <param name="dt"></param>
		/// <param name="fieldName"></param>
		public static void DecryptField(ref DataTable dt, string fieldName)
		{
			ClassGenExceptionCollection errors = new ClassGenExceptionCollection();
			DataColumn dc = dt.Columns[fieldName];
			if (dc != null)
			{
				// DO SOME DECRYPTING!
				foreach (DataRow row in dt.Rows)
				{
					if (!String.IsNullOrEmpty(row[dc].ToString()))
					{

						row[dc] = Security.DecryptString(row[dc].ToString(), ref errors);
					}
				}
			}
		}
	}
}
