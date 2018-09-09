using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace LEAD_Log
{
    /// <summary>
    /// Summary description for DAL.
    /// </summary>
    public partial class DAL // : IDisposable
    {
		private static string m_sConn = string.Empty;	// SQL server connection string.
		private static int _numSecondsLocalComputerBehindServer = 0;

		/// <summary>
        ///   Class constructor with no arguments.
        /// </summary>
        public DAL()
        {
        }

		#region Database Connection Methods
		public static void EvalNumSecondsLocalMachineBehindServer(DateTime serverDateTime)
		{
			_numSecondsLocalComputerBehindServer = (int)((TimeSpan)serverDateTime.Subtract(DateTime.Now)).TotalSeconds;
		}

		public static DateTime GetSQLServerDateTime()
		{
			DateTime date = DateTime.Now;
			if (_numSecondsLocalComputerBehindServer != 0)
			{
				date.AddSeconds(_numSecondsLocalComputerBehindServer);
			}
			return date;
		}

		/// <summary>
		/// Open a connection that can be used on the database
		/// </summary>
		/// <returns>The Connection object to use</returns>
		public static SqlConnection OpenConnection()
		{
			SqlConnection oConn = null;
			string connString = DAL.ConnString;
			if (String.IsNullOrEmpty(connString))
			{
				throw new Exception("Connection string not set on DAL.");
			}
			else
			{
				oConn = new SqlConnection(connString);
				oConn.Open();
			}
			return oConn;
		}

		/// <summary>
		/// Close the connection 
		/// </summary>
		public static void CloseConnection(SqlConnection oConn)
		{
			if (oConn != null)
			{
				oConn.Close();
				oConn.Dispose();
			}
		}
		#endregion Database Connection Methods

		#region Aliased Methods

		#region DataReaders
		/// <summary>
		/// Return a datareader based on the SQL string that's been passed
		/// </summary>
		/// <param name="SQL">The SQL String to process</param>
		/// <returns>A datareader with the retrieved information - remember to close it</returns>
		public static SqlDataReader SQLExecDataReader(string SQL)
		{
			return SQLExecDR(SQL);
		}

		/// <summary>
		/// Return a datareader based on the SQL string that's been passed
		/// </summary>
		/// <param name="SQL">The SQL String to process</param>
		/// <param name="cb">Command Behavior Object</param>
		/// <returns>A datareader with the retrieved information - remember to close it</returns>
		public static SqlDataReader SQLExecDataReader(string SQL, CommandBehavior cb)
		{
			return SQLExecDR(SQL, cb);
		}

		/// <summary>
		/// Return a datareader based on the SQL string that's been passed
		/// </summary>
		/// <param name="SQL">The SQL String to process</param>
		/// <returns>A datareader with the retrieved information - remember to close it</returns>
		public static SqlDataReader SQLExecDataReader(SqlCommand cmd)
		{
			return SQLExecDR(cmd);
		}

		/// <summary>
		/// Return a datareader based on the SQL string that's been passed
		/// </summary>
		/// <param name="SQL">The SQL String to process</param>
		/// <param name="cb">Command Behavior Object</param>
		/// <returns>A datareader with the retrieved information - remember to close it</returns>
		public static SqlDataReader SQLExecDataReader(SqlCommand cmd, CommandBehavior cb)
		{
			return SQLExecDR(cmd, cb);
		}
		#endregion DataReaders

		#region DataSets
		/// <summary>
		/// Return a datatable based on the command object that's been passed
		/// </summary>
		/// <param name="cmd">The command object to process</param>
		/// <returns>A DataSet with the given information</returns>
		public static DataSet SQLExecDataSet(SqlCommand cmd)
		{
			return SQLExecDS(cmd);
		}

		/// <summary>
		/// Return a datatable based on the sql string that's been passed
		/// </summary>
		/// <param name="psSql">The array of sql strings to process against</param>
		/// <returns>A DataSet with the given information</returns>
		public static DataSet SQLExecDataSet(string[,] psSql)
		{
			return SQLExecDS(psSql);
		}
		#endregion DataSets

		#region DataTables
		/// <summary>
        /// Return a datatable based on the SQL string that's been passed
        /// </summary>
        /// <param name="SQL">The SQL string to process</param>
        /// <returns>A datatable with the given information</returns>
		public static DataTable SQLExecDataTable(string SQL)
        {
            return SQLExecDT(SQL);
        }

		/// <summary>
		/// Return a datatable based on the SQL string that's been passed
		/// </summary>
		/// <param name="SQL">The SQL string to process</param>
		/// <returns>A datatable with the given information</returns>
		public static DataTable SQLExecDataTable(string SQL, SqlConnection oConn)
		{
			return SQLExecDT(SQL, oConn);
		}

        /// <summary>
        /// Returns a datatable from a SQLCommand.  The SqlCommand cannot be a SPROC, and may or may not have params.
        /// </summary>
        /// <param name="cmd">The command object to execute and return</param>
        /// <returns>A datatable from the SqlCommand</returns>
		public static DataTable SQLExecDataTable(SqlCommand cmd)
        {
            return SQLExecDT(cmd);
        }

		/// <summary>
		/// Returns a datatable from a SQLCommand.  The SqlCommand cannot be a SPROC, and may or may not have params.
		/// </summary>
		/// <param name="cmd">The command object to execute and return</param>
		/// <returns>A datatable from the SqlCommand</returns>
		public static DataTable SQLExecDataTable(SqlCommand cmd, SqlConnection oConn)
		{
			return SQLExecDT(cmd, oConn);
		}

        /// <summary>
        /// Returns a datatable from a SQLCommand.  The SqlCommand must be a SPROC, and may or may not have params.
        /// </summary>
        /// <param name="cmd">The command object to execute and return</param>
        /// <returns>A datatable from the SqlCommand</returns>
		public static DataTable SQLExecDataTableSP(SqlCommand cmd)
        {
            return SQLExecDTSP(cmd);
        }

		///// <summary>
		///// Returns a datatable from a SQLCommand.  The SqlCommand cannot be a SPROC, and may or may not have params.
		///// </summary>
		///// <param name="cmd">The command object to execute and return</param>
		///// <returns>A datatable from the SqlCommand</returns>
		//public DataTable SQLExecDataTableSP(SqlCommand cmd, SqlConnection oConn)
		//{
		//    return SQLExecDTSP(cmd, oConn);
		//}
		#endregion DataTables

		#endregion Aliased Methods

		#region Standard Methods

		#region DataRow
		/// <summary>
		/// Return a datatable based on the SQL string that's been passed
		/// </summary>
		/// <param name="SQL">The SQL string to process</param>
		/// <returns>A datatable with the given information</returns>
		public static DataRow SQLExecDataRow(string SQL)
		{
			DataTable oRS = null;
			SqlConnection oConn = DAL.OpenConnection();
			if (oConn == null) { return null; }

			SqlDataAdapter oDA = new SqlDataAdapter(SQL, oConn);
			oRS = new DataTable();

			oDA.Fill(oRS);

			oDA.Dispose();
			DAL.CloseConnection(oConn);

			return (oRS.Rows.Count > 0 ? oRS.Rows[0] : null);
		}

		/// <summary>
		/// Returns a DataRow from the SqlCommand passsed in. The SqlCommand myst be a SPROC, and may or may not have params.
		/// </summary>
		/// <param name="cmd">The SqlCommand object to execute</param>
		/// <returns>A single Datarow from the SqlCommand</returns>
		public static DataRow SQLExecDataRow(SqlCommand cmd)
		{
			DataTable dt = null;
			SqlConnection oConn = DAL.OpenConnection();
			if (oConn == null) { return null; }

			cmd.CommandType = GetCommandType(cmd);

			cmd.Connection = oConn;
			SqlDataAdapter oDA = new SqlDataAdapter(cmd);
			dt = new DataTable();

			oDA.Fill(dt);

			oDA.Dispose();
			DAL.CloseConnection(oConn);

			return (dt.Rows.Count > 0 ? dt.Rows[0] : null);
		}
		#endregion DataRow

		#region DataReaders
		/// <summary>
		/// Return a datareader based on the SQL string that's been passed
		/// </summary>
		/// <param name="SQL">The SQL String to process</param>
		/// <returns>A datareader with the retrieved information - remember to close it</returns>
		public static SqlDataReader SQLExecDR(string SQL)
		{
			SqlDataReader dr = null;
			SqlConnection oConn = DAL.OpenConnection();
			if (oConn == null) { return null; }

			SqlCommand cmd = new SqlCommand(SQL, oConn);
			cmd.CommandType = GetCommandType(cmd);
			dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

			cmd.Dispose();
			cmd = null;

			return dr;
		}

		/// <summary>
		/// Return a datareader based on the SQL string that's been passed
		/// </summary>
		/// <param name="SQL">The SQL String to process</param>
		/// <param name="cb">Command Behavior Object</param>
		/// <returns>A datareader with the retrieved information - remember to close it</returns>
		public static SqlDataReader SQLExecDR(string SQL, CommandBehavior cb)
		{
			SqlDataReader dr = null;
			SqlConnection oConn = DAL.OpenConnection();
			if (oConn == null) { return null; }

			SqlCommand cmd = new SqlCommand(SQL, oConn);
			cmd.CommandType = GetCommandType(cmd);

			dr = cmd.ExecuteReader(cb);

			return dr;
		}

		/// <summary>
		/// Returns a DataReader from a passed-in Command object.  The Command text must be a SPROC.
		/// </summary>
		/// <param name="cmd">The command object to extract a datareader.</param>
		/// <returns></returns>
		public static SqlDataReader SQLExecDR(SqlCommand cmd)
		{
			SqlDataReader dr = null;
			SqlConnection oConn = DAL.OpenConnection();
			if (oConn == null) { return null; }

			cmd.CommandType = GetCommandType(cmd);
			cmd.Connection = oConn;

			dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

			cmd.Dispose();
			cmd = null;

			return dr;
		}

		/// <summary>
		/// Returns a DataReader from the SqlCommand object passed in.
		/// Overloaded.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="cmdBehavior">CommandBehavior enumerations for the .ExecuteReader() method.
		/// Be sure to include the CommandBehavior.CloseConnection if necessary</param>
		/// <returns></returns>
		public static SqlDataReader SQLExecDR(SqlCommand cmd, CommandBehavior cmdBehavior)
		{
			SqlDataReader dr = null;
			SqlConnection oConn = DAL.OpenConnection();
			if (oConn == null) { return null; }

			cmd.CommandType = GetCommandType(cmd);

			cmd.Connection = oConn;

			dr = cmd.ExecuteReader(cmdBehavior);

			cmd.Dispose();
			cmd = null;

			return dr;
		}
		#endregion DataReaders

		#region DataTables
		/// <summary>
        /// Return a datatable based on the SQL string that's been passed
        /// </summary>
        /// <param name="SQL">The SQL string to process</param>
        /// <returns>A datatable with the given information</returns>
		public static DataTable SQLExecDT(string SQL)
        {
            DataTable oRS = null;
			SqlConnection oConn = DAL.OpenConnection();
			if (oConn == null) { return null; }

			SqlCommand cmd = new SqlCommand(SQL);
			cmd.CommandTimeout = 300;
			cmd.CommandType = GetCommandType(cmd);
			cmd.Connection = oConn;

			SqlDataAdapter oDA = new SqlDataAdapter(cmd);
            oRS = new DataTable();

            oDA.Fill(oRS);

            oDA.Dispose();
			DAL.CloseConnection(oConn);

            return oRS;
        }

		/// <summary>
		/// Return a datatable based on the SQL string that's been passed
		/// </summary>
		/// <param name="SQL">The SQL string to process</param>
		/// <returns>A datatable with the given information</returns>
		public static DataTable SQLExecDT(string SQL, SqlConnection oConn)
		{
			DataTable oRS = null;

			SqlCommand cmd = new SqlCommand(SQL, oConn);
			cmd.CommandTimeout = 300;
			cmd.CommandType = GetCommandType(cmd);

			SqlDataAdapter oDA = new SqlDataAdapter(cmd);
			oRS = new DataTable();

			oDA.Fill(oRS);

			oDA.Dispose();
			cmd.Dispose();

			return oRS;
		}

        /// <summary>
        /// Returns a datatable from a SQLCommand.  The SqlCommand cannot be a SPROC, and may or may not have params.
        /// </summary>
        /// <param name="cmd">The command object to execute and return</param>
        /// <returns>A datatable from the SqlCommand</returns>
		public static DataTable SQLExecDT(SqlCommand cmd)
        {
            DataTable dt = null;
			SqlConnection oConn = DAL.OpenConnection();
			if (oConn == null) { return null; }

            cmd.Connection = oConn;
			cmd.CommandType = GetCommandType(cmd);

            //oConn.Open();
            SqlDataAdapter oDA = new SqlDataAdapter(cmd);
            dt = new DataTable();

            oDA.Fill(dt);

            oDA.Dispose();
			DAL.CloseConnection(oConn);

            return dt;
        }

		/// <summary>
		/// Returns a datatable from a SQLCommand.  The SqlCommand cannot be a SPROC, and may or may not have params.
		/// </summary>
		/// <param name="cmd">The command object to execute and return</param>
		/// <returns>A datatable from the SqlCommand</returns>
		public static DataTable SQLExecDT(SqlCommand cmd, SqlConnection oConn)
		{
			DataTable dt = null;

			cmd.Connection = oConn;
			cmd.CommandType = GetCommandType(cmd);
			SqlDataAdapter oDA = new SqlDataAdapter(cmd);
			dt = new DataTable();

			oDA.Fill(dt);

			oDA.Dispose();
			cmd.Dispose();

			return dt;
		}

        /// <summary>
        /// Returns a datatable from a SQLCommand.  The SqlCommand must be a SPROC, and may or may not have params.
        /// </summary>
        /// <param name="cmd">The command object to execute and return</param>
        /// <returns>A datatable from the SqlCommand</returns>
		public static DataTable SQLExecDTSP(SqlCommand cmd)
        {
            DataTable dt = null;
			SqlConnection oConn = DAL.OpenConnection();
			if (oConn == null) { return null; }

            cmd.CommandType = CommandType.StoredProcedure;

			cmd.Connection = oConn;
            SqlDataAdapter oDA = new SqlDataAdapter(cmd);
            dt = new DataTable();

            oDA.Fill(dt);

            oDA.Dispose();
			DAL.CloseConnection(oConn);

            return dt;
        }
		#endregion DataTables

		#region DataSets
		/// <summary>
		/// Returns a dataset from SqlCommand.
		/// </summary>
		/// <param name="cmd"></param>
		/// <returns>DataSet</returns>
		public static DataSet SQLExecDS(SqlCommand cmd)
		{
			DataSet ds = new DataSet();
			SqlConnection oConn = DAL.OpenConnection();
			if (oConn == null) { return null; }

			cmd.CommandType = GetCommandType(cmd);

			cmd.Connection = oConn;
			SqlDataAdapter oDA = new SqlDataAdapter(cmd);

			oDA.Fill(ds);

			oDA.Dispose();
			DAL.CloseConnection(oConn);

			return ds;
		}

		/// <summary>
		/// Opens Data Set and Populates Tables
		/// </summary>
		/// <param name="psSql">
		/// <list type="1st Element">Qualified Sql String</list>
		/// <list type="2nd Element">Table Name</list></param>
		/// <returns>DataSet</returns>
		public static DataSet SQLExecDS(string[,] psSql)
		{
			DataSet oData = new DataSet();
			SqlConnection oConn = DAL.OpenConnection();
			if (oConn == null) { return null; }

			for (int i = psSql.GetLowerBound(0); i < psSql.GetUpperBound(0) + 1; i++)
			{
				if (psSql[i, 0].Length > 0)
				{
					SqlDataAdapter oSqlDA = new SqlDataAdapter(psSql[i, 0], oConn);
					oSqlDA.Fill(oData, psSql[i, 1]);
				}
			}
			DAL.CloseConnection(oConn);

			return oData;
		}
		#endregion DataSets

		#region Non-Queries
		/// <summary>
        /// Run a "quick-query" against the database
        /// Return the number of rows affected by the query
        /// </summary>
        /// <param name="SQL">The SQL string to process</param>
        /// <returns>An integer value with the number of rows affected</returns>
		public static int SQLExecNonQuery(string SQL)
        {
            int nRows = 0;
			SqlConnection oConn = DAL.OpenConnection();
			if (oConn == null) { return 0; }

			SqlCommand cmd = new SqlCommand(SQL, oConn);
			cmd.CommandTimeout = 300;
			cmd.CommandType = GetCommandType(cmd);

			nRows = cmd.ExecuteNonQuery();	// Get the number of rows effected by the action query.

			DAL.CloseConnection(oConn);

            return nRows;
        }

        /// <summary>
        /// Run a "quick-query" against the database
        /// Return the number of rows affected by the query
        /// </summary>
        /// <param name="SQL">The SQL string to process</param>
        /// <returns>An integer value with the number of rows affected</returns>
		public static int SQLExecNonQuery(SqlCommand cmd)
        {
            int nRows = 0;
			SqlConnection oConn = DAL.OpenConnection();
			if (oConn == null) { return 0; }

            cmd.CommandTimeout = 300;
            cmd.Connection = oConn;
			cmd.CommandType = GetCommandType(cmd);

            nRows = cmd.ExecuteNonQuery();	// Get the number of rows effected by the action query.

			DAL.CloseConnection(oConn);

            return nRows;
        }

		/// <summary>
		/// Run a "quick-query" against the database
		/// Return the number of rows affected by the query
		/// </summary>
		/// <param name="SQL">The SQL string to process</param>
		/// <returns>An integer value with the number of rows affected</returns>
		public static int SQLExecNonQuery(SqlCommand cmd, SqlConnection oConn)
		{
			int nRows = 0;

			cmd.CommandTimeout = 300;
			cmd.Connection = oConn;
			cmd.CommandType = GetCommandType(cmd);

			nRows = cmd.ExecuteNonQuery();	// Get the number of rows effected by the action query.

			//oConn.Close();
			//oConn.Dispose();
			cmd.Dispose();

			return nRows;
		}

        /// <summary>
        /// Run a "quick-query" against the database
        /// Return the number of rows affected by the query
        /// </summary>
        /// <param name="SQL">The SQL string to process</param>
        /// <returns>An integer value with the number of rows affected</returns>
		public static int SQLExecNonQuerySP(SqlCommand cmd)
        {
            int nRows = 0;
			SqlConnection oConn = DAL.OpenConnection();
			if (oConn == null) { return 0; }
            
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.CommandTimeout = 300;
            cmd.Connection = oConn;

            nRows = cmd.ExecuteNonQuery();	// Get the number of rows effected by the action query.

			DAL.CloseConnection(oConn);

            return nRows;
        }
		#endregion Non-Queries

		#region Scalar
		/// <summary>
		/// Execute the scalar command - even though it's spelled wrong
		/// </summary>
		public static object SQLExecScaler(SqlCommand cmd)
        {
			return SQLExecScalar(cmd);		// Call the other object
		}

		/// <summary>
		/// Execute a scalar method and have it return an object 
		/// with the single item from the database
		/// </summary>
		/// <param name="cmd">The command object to call</param>
		/// <returns>The object returned from the database</returns>
		public static object SQLExecScalar(SqlCommand cmd)
        {
            SqlConnection oConn = DAL.OpenConnection();
            if (oConn == null) { return null; }
			object rtv = null;
            
            cmd.Connection = oConn;
            rtv = cmd.ExecuteScalar();

            DAL.CloseConnection(oConn);
            cmd.Dispose();
            cmd = null;

            return rtv;		// Return the object
        }
		#endregion Scalar
		
		#region Common Methods
		/// <summary>
		/// Get the command type for the specified object
		/// </summary>
		/// <param name="cmd">The command object to analyze</param>
		/// <returns>The type of command for the object</returns>
		public static CommandType GetCommandType(SqlCommand cmd)
		{
			CommandType type = CommandType.StoredProcedure;

			if (cmd.CommandText.Contains("\t")) { cmd.CommandText = cmd.CommandText.Replace("\t", " "); }
			if (cmd.CommandText.Contains("\r")) { cmd.CommandText = cmd.CommandText.Replace("\r", " "); }
			if (cmd.CommandText.Contains("\n")) { cmd.CommandText = cmd.CommandText.Replace("\n", " "); }

			if (cmd.CommandText.Substring(0, 2).ToUpper() == "--" ||
				cmd.CommandText.Substring(0, 5).ToUpper() == "EXEC " ||
				cmd.CommandText.Substring(0, 7).ToUpper() == "CREATE " ||
				cmd.CommandText.Substring(0, 3).ToUpper() == "IF " ||
				cmd.CommandText.Substring(0, 7).ToUpper() == "SELECT " ||
				cmd.CommandText.Substring(0, 7).ToUpper() == "DELETE " ||
				cmd.CommandText.Substring(0, 7).ToUpper() == "INSERT " ||
				cmd.CommandText.Substring(0, 7).ToUpper() == "UPDATE " ||
				cmd.CommandText.Substring(0, 8).ToUpper() == "DECLARE " ||
				cmd.CommandText.Substring(0, 4).ToUpper() == "SET " ||
				(cmd.CommandText.Length > 10 && cmd.CommandText.Substring(0, 10).ToUpper() == "IF EXISTS ") ||
				(cmd.CommandText.Length > 14 && cmd.CommandText.Substring(0, 14).ToUpper() == "IF NOT EXISTS "))
			{
				type = CommandType.Text;
			}
			return type;
		}

		public static DateTime GetCurrentDateTimeFromSQLServer()
        {
            DataTable dt = null;
            SqlCommand cmd = null;
            SqlConnection oConn = DAL.OpenConnection();
			if (oConn == null) { return DateTime.MinValue; }
            DateTime rtv = DateTime.MinValue;

            cmd = new SqlCommand("SELECT GETDATE() AS dtDateTime");

			cmd.Connection = oConn;
			cmd.CommandType = GetCommandType(cmd);
            SqlDataAdapter oDA = new SqlDataAdapter(cmd);
            dt = new DataTable();

            oDA.Fill(dt);

            // Get the time from the record
            if (dt.Rows.Count > 0)
            {
                rtv = (DateTime)dt.Rows[0][0];
            }

            oDA.Dispose();
			DAL.CloseConnection(oConn);

            return rtv;
		}

		/// <summary>
		/// Get the time stamp from the table
		/// </summary>
		public static byte[] GetTimeStampFromTable(string tableName, string timeStampFieldName, string whereClause)
		{
			byte[] rtv = null;
			DataTable dt = null;
			SqlCommand cmd = null;
			string sql = string.Empty;

			sql = "SELECT " + timeStampFieldName + " FROM " + tableName + " WHERE " + whereClause + "";
			cmd = new SqlCommand(sql);
			dt = DAL.SQLExecDataTable(cmd);

			if (dt.Rows.Count > 0) { rtv = (byte[])(dt.Rows[0][0]); }
			
			return rtv;
		}
		#endregion Common Methods

		#endregion Standard Methods

		#region GetValueForSQL Section

		#region Static Empty Values
		private static readonly Guid _EmptyGuid = Guid.Empty;
		private static readonly int _EmptyInt = -1;
        private static readonly double _EmptyDouble = -1;
        private static readonly decimal _EmptyDecimal = -1;
        private static readonly DateTime _EmptyDateTime = DateTime.Parse("1/1/1900");
        private static readonly string _EmptyString = string.Empty;

        private static readonly string _DateFormat = "MM/dd/yyyy";
        private static readonly string _TimeFormat = "hh:mm:ss";
        private static readonly string _DateTimeFormat = _DateFormat + " " + _TimeFormat;
		#endregion Empty Values

		public static string GetValueForSQL(object In)
		{
			if (In is Guid)
				return GetValueForSQL((Guid)In);
			else if (In is int)
				return GetValueForSQL((int)In);
			else if (In is double)
				return GetValueForSQL((decimal)In);
			else if (In is decimal)
				return GetValueForSQL((decimal)In);
			else if (In is bool)
				return GetValueForSQL((bool)In);
			else if (In is DateTime)
				return GetValueForSQL((DateTime)In);
			else if (In is byte[])
				return GetValueForSQL((byte[])In);
			else
				if (In == null || In.ToString() == _EmptyString)
					return "NULL";
				else
					return "'" + FixForSQL(In.ToString()) + "'";
		}


		public static string GetValueForSQL(Guid In)
		{
			if (In == _EmptyGuid)
				return "NULL";
			else
				return "'" + In.ToString() + "'";
		}


		public static string GetValueForSQL(int In)
		{
			if (In == _EmptyInt)
				return "NULL";
			else
				return In.ToString();
		}


		public static string GetValueForSQL(double In)
		{
			if (In == _EmptyDouble)
				return "NULL";
			else
				return In.ToString();
		}


		public static string GetValueForSQL(decimal In)
		{
			if (In == _EmptyDecimal)
				return "NULL";
			else
				return In.ToString();
		}


		public static string GetValueForSQL(bool In)
		{
			if (In)
				return "1";
			else
				return "0";
		}


		public static string GetValueForSQL(DateTime In)
		{
			if (In <= _EmptyDateTime || In == DateTime.MinValue)
				return "NULL";
			else
				return "'" + In.ToString() + "'";
		}


		public static string GetValueForSQL(byte[] In)
		{
			if (In == null || In.Length == 0)
				return "NULL";
			else
			{
				StringBuilder Ret = new StringBuilder();
				Ret.Append("0x");
				foreach (byte ByteItem in In)
				{
					Ret.Append(ByteItem.ToString("X2"));
				}

				return Ret.ToString();
			}
		}


		/// <summary>
		/// Fixes a string for safe passage to SQL.  This is intended for values not full SQL strings.
		/// </summary>
		/// <param name="Val">Value to fixed</param>
		/// <returns>Safe SQL string.</returns>
		public static string FixForSQL(string Val)
		{
			if (Val == null) return string.Empty;

			string sRet = Val;
			if (Val.IndexOf("'") > -1) { sRet = Val.Replace("'", "''"); }	// Replace "'" with "''".
			if (Val.IndexOf("<<") > -1) { sRet = Val.Replace("<<", "<"); }	// Replace "<<" with "<".

			return sRet;	// Return the fixed SQL query string.
		}

		/// <summary>
		/// Check to see if the database is online
		/// </summary>
		/// <returns>True if there, otherwise, false</returns>
		public static bool IsOnline(bool resetConnStr)
		{
			bool rtv = false;

			//DAL dal = new DAL();
			if (resetConnStr) { DAL.ConnString = string.Empty; }

			System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
			if (!String.IsNullOrEmpty(DAL.ConnString))
			{
				conn.ConnectionString = DAL.ConnString;
				conn.Open();
				conn.Close();
				conn.Dispose();

				rtv = true;
			}

			return rtv;
		}
		#endregion GetValueForSQL Section 
    }
}

