using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace LEAD
{
    /// <summary>
    /// Summary description for DAL.
    /// </summary>
    public partial class DAL //: DALBase
    {
		/// <summary>
        ///   The DB.ConnString property allows us to modify and retrieve the database connection string.
        /// </summary>
        /// <returns>The SQL database connection string.</returns>
        /// <param name="value">Value to set the SQL database connection string to.</param> 
        public static string ConnString
        {
            get
			{
				if (String.IsNullOrEmpty(m_sConn))
				{
					//m_sConn = "Integrated Security=SSPI;Persist Security Info=False;User ID=LEADUser;Initial Catalog=LEAD_Small;Data Source=(local)";
					m_sConn = "Integrated Security=SSPI;Persist Security Info=False;User ID=LEADUser;Initial Catalog=LEAD_Small;Data Source=(local)\\MGER_2014";
				}
				return m_sConn;
			}
            set { m_sConn = value; }
        }
	}
}
