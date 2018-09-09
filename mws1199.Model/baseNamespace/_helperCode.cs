using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace LEADBase
{
	#region GetCollectionConfiguration
	[Serializable]
	public class GetCollectionConfiguration
	{
		private string _whereClause = string.Empty;
		private bool _getChildren = false;
		private int _getChildrenNestingLevel = 2;
		private bool _pageRecords = false;
		private int _pageSize = 500;
		private bool _includeUpdateDateInWhereClause = false;
		private long _topRecords = -1;
		private string _topSort = string.Empty;
		private Dictionary<string, object> _parameters = new Dictionary<string, object>();

		public GetCollectionConfiguration()
		{
		}

		public string WhereClause
		{
			get { return _whereClause; }
			set { _whereClause = value; }
		}

		public Dictionary<string, object> Parameters
		{
			get { return _parameters; }
			set { _parameters = value; }
		}

		public bool GetChildren
		{
			get { return _getChildren; }
			set { _getChildren = value; }
		}

		public int GetChildrenNestingLevel
		{
			get { return _getChildrenNestingLevel; }
			set { _getChildrenNestingLevel = value; }
		}

		public bool PageRecords
		{
			get { return _pageRecords; }
			set { _pageRecords = value; }
		}

		public int PageSize
		{
			get { return _pageSize; }
			set { _pageSize = value; }
		}

		public bool IncludeUpdateDateInWhereClause
		{
			get { return _includeUpdateDateInWhereClause; }
			set { _includeUpdateDateInWhereClause = value; }
		}

		public long TopRecords
		{
			get { return _topRecords; }
			set { _topRecords = value; }
		}
		
		public string TopSort
		{
			get { return _topSort; }
			set { _topSort = value; }
		}
	}
	#endregion GetCollectionConfiguration

	#region ClassGenException Class
	[Serializable]
	public class ClassGenException
	{
		private ClassGenExceptionIconType _exceptionIconType = ClassGenExceptionIconType.None;
		private ClassGenExceptionType _exceptionType = ClassGenExceptionType.Empty;
		private string _propName = string.Empty;
		private int _recordIndex = 0;
		private string _recordKey = string.Empty;

		private DateTime _errorDate = DateTime.Now;
		private string _errorKey = System.Guid.NewGuid().ToString();

		private BrokenRuleType _ruleType = BrokenRuleType.Empty;

		[NonSerialized, System.Xml.Serialization.XmlIgnore]
		private System.Collections.IDictionary _data = null;
		
		private string _helpLink = string.Empty;

		[NonSerialized, System.Xml.Serialization.XmlIgnore]
		private Exception _innerException = null;

		private string _message = string.Empty;
		private string _source = string.Empty;
		private string _stackTrace = string.Empty;
		private byte _class; // = byte.Parse(@"\0");
		private byte _state; // = byte.Parse(@"\0");
		private int _errorCode = 0;

		[NonSerialized, System.Xml.Serialization.XmlIgnore]
		private System.Data.SqlClient.SqlErrorCollection _sqlErrors = null;

		private int _lineNumber = 0;
		private int _number = 0;
		private string _procedure = string.Empty;
		private string _server = string.Empty;

		#region Constructors
		public ClassGenException(string description)
		{
			this._errorDate = DateTime.Now;
			this._errorKey = System.Guid.NewGuid().ToString();

			this.ClassGenExceptionType = ClassGenExceptionType.TextOnly;
			this.ClassGenExceptionIconType = ClassGenExceptionIconType.Critical;
			this.InnerException = new Exception(description);
			this.Message = description;
			this.StackTrace = System.Environment.StackTrace;
		}
		
		public ClassGenException(string description, ClassGenExceptionIconType exceptionIconType)
		{
			this._errorDate = DateTime.Now;
			this._errorKey = System.Guid.NewGuid().ToString();

			this.ClassGenExceptionType = ClassGenExceptionType.TextOnly;
			this.ClassGenExceptionIconType = exceptionIconType;
			this.InnerException = new Exception(description);
			this.Message = description;
			this.StackTrace = System.Environment.StackTrace;
		}

		public ClassGenException(string description, ClassGenExceptionIconType exceptionIconType, string extendedDescription)
		{
			this._errorDate = DateTime.Now;
			this._errorKey = System.Guid.NewGuid().ToString();

			this.ClassGenExceptionType = ClassGenExceptionType.TextOnly;
			this.ClassGenExceptionIconType = exceptionIconType;
			this.InnerException = new Exception(extendedDescription);
			this.Message = description;
			this.StackTrace = System.Environment.StackTrace;
		}

		public ClassGenException(string description, int recordIndex)
		{
			this._errorDate = DateTime.Now;
			this._errorKey = System.Guid.NewGuid().ToString();

			this.ClassGenExceptionType = ClassGenExceptionType.TextOnly;
			this.ClassGenExceptionIconType = ClassGenExceptionIconType.Critical;
			this.InnerException = new Exception(description);
			this.Message = description;
			this.StackTrace = System.Environment.StackTrace;
			this.RecordIndex = recordIndex;
		}

		public ClassGenException(string description, int recordIndex, ClassGenExceptionIconType exceptionIconType)
		{
			this._errorDate = DateTime.Now;
			this._errorKey = System.Guid.NewGuid().ToString();

			this.ClassGenExceptionType = ClassGenExceptionType.TextOnly;
			this.ClassGenExceptionIconType = exceptionIconType;
			this.InnerException = new Exception(description);
			this.Message = description;
			this.StackTrace = System.Environment.StackTrace;
			this.RecordIndex = recordIndex;
		}

		public ClassGenException(string description, int recordIndex, ClassGenExceptionIconType exceptionIconType, string extendedDescription)
		{
			this._errorDate = DateTime.Now;
			this._errorKey = System.Guid.NewGuid().ToString();

			this.ClassGenExceptionType = ClassGenExceptionType.TextOnly;
			this.ClassGenExceptionIconType = exceptionIconType;
			this.InnerException = new Exception(extendedDescription);
			this.Message = description;
			this.StackTrace = System.Environment.StackTrace;
			this.RecordIndex = recordIndex;
		}

		public ClassGenException(Exception e)
		{
			this._errorDate = DateTime.Now;
			this._errorKey = System.Guid.NewGuid().ToString();

			this.ClassGenExceptionType = ClassGenExceptionType.Exception;
			this.ClassGenExceptionIconType = ClassGenExceptionIconType.Critical;
			this.Data = e.Data;
			this.HelpLink = e.HelpLink;
			this.InnerException = e.InnerException;
			this.Message = e.Message;
			this.Source = e.Source;
			this.StackTrace = e.StackTrace;
		}

		public ClassGenException(System.Data.SqlClient.SqlException e)
	    {
	        this._errorDate = DateTime.Now;
			this._errorKey = System.Guid.NewGuid().ToString();

			this.ClassGenExceptionType = ClassGenExceptionType.SQLException;
			this.ClassGenExceptionIconType = ClassGenExceptionIconType.Critical;
	        this.Data = e.Data;
	        this.HelpLink = e.HelpLink;
	        this.InnerException = e.InnerException;
	        this.Message = e.Message;
	        this.Source = e.Source;
	        this.StackTrace = e.StackTrace;

	        this.Class = e.Class;
	        this.ErrorCode = e.ErrorCode;
	        this.SQLErrors = e.Errors;
	        this.LineNumber = e.LineNumber;
	        this.Number = e.Number;
	        this.Procedure = e.Procedure;
	        this.Server = e.Server;
	        this.State = e.State;
	    }

		public ClassGenException()
		{
		}
		#endregion Constructors

		#region Properties
		public ClassGenExceptionIconType ClassGenExceptionIconType
		{
			get { return _exceptionIconType; }
			set { _exceptionIconType = value; }
		}

		public ClassGenExceptionType ClassGenExceptionType
		{
			get { return _exceptionType; }
			set { _exceptionType = value; }
		}

		public string Description
		{
			get { return _message; }
			set { _message = value; }
		}

		public string DescriptionWithException
		{
			get { return this.Description + 
				(!String.IsNullOrEmpty(ExtendedDescription) && 
				ExtendedDescription.Trim().ToLower() != Description.Trim().ToLower() ? Environment.NewLine + ExtendedDescription : ""); }
		}

		public string ExtendedDescription
		{
			get { return (_innerException != null ? _innerException.Message : string.Empty); }
			set { _innerException = new Exception(value); }
		}

		public string PropertyName
		{
			get { return _propName; }
			set { _propName = value; }
		}

		public int RecordIndex
		{
			get { return _recordIndex; }
			set { _recordIndex = value; }
		}

		public string RecordKey
		{
			get { return _recordKey; }
			set { _recordKey = value; }
		}

		public BrokenRuleType BrokenRuleType
		{
			get { return _ruleType; }
			set { _ruleType = value; }
		}
		
		public DateTime ErrorDate
		{
			get { return _errorDate; }
		}

		public string ErrorKey
		{
			get { return _errorKey; }
		}

		[System.Xml.Serialization.XmlIgnore]
		public System.Collections.IDictionary Data
		{
			get { return _data; }
			set { _data = value; }
		}

		public string HelpLink
		{
			get { return _helpLink; }
			set { _helpLink = value; }
		}

		[System.Xml.Serialization.XmlIgnore]
		public Exception InnerException
		{
			get { return _innerException; }
			set { _innerException = value; }
		}

		public string Message
		{
			get { return _message; }
			set { _message = value; }
		}

		public string Source
		{
			get { return _source; }
			set { _source = value; }
		}

		public string StackTrace
		{
			get { return _stackTrace; }
			set { _stackTrace = value; }
		}

		public byte Class
		{
			get { return _class; }
			set { _class = value; }
		}

		public byte State
		{
			get { return _state; }
			set { _state = value; }
		}

		[System.Xml.Serialization.XmlIgnore]
		public System.Data.SqlClient.SqlErrorCollection SQLErrors
		{
			get { return _sqlErrors; }
			set { _sqlErrors = value; }
		}

		public int ErrorCode
		{
			get { return _errorCode; }
			set { _errorCode = value; }
		}

		public int LineNumber
		{
			get { return _lineNumber; }
			set { _lineNumber = value; }
		}

		public int Number
		{
			get { return _number; }
			set { _number = value; }
		}

		public string Procedure
		{
			get { return _procedure; }
			set { _procedure = value; }
		}

		public string Server
		{
			get { return _server; }
			set { _server = value; }
		}
		#endregion Properties
	}

	[Serializable]
	public class ClassGenExceptionCollection : List<ClassGenException>
	{
		public new void Add(ClassGenException ex) 
		{
			if (!KeyExists(ex.ErrorKey)) { base.Add(ex); }
		}

		public bool KeyExists(string key)
		{
			bool rtv = false;
			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].ErrorKey.ToLower() == key.ToLower())
				{
					rtv = true;
					break;
				}
			}
			return rtv;
		}

		/// <summary>
		/// The critical exception or warning collection
		/// </summary>
		public ClassGenExceptionCollection CriticalOrWarningExceptionCollection
		{
			get
			{
				ClassGenExceptionCollection coll = new ClassGenExceptionCollection();
				for (int i = 0; i < this.Count; i++)
				{
					if (this[i].ClassGenExceptionIconType == ClassGenExceptionIconType.Critical ||
						this[i].ClassGenExceptionIconType == ClassGenExceptionIconType.Warning)
					{
						coll.Add(this[i]);
					}
				}
				return coll;
			}
		}

		/// <summary>
		/// Get the critical exception or warning count
		/// </summary>
		/// <returns>The count of critical errors and warnings</returns>
		public int CriticalOrWarningExceptionCount
		{
			get
			{
				int count = 0;
				for (int i = 0; i < this.Count; i++)
				{
					if (this[i].ClassGenExceptionIconType == ClassGenExceptionIconType.Critical ||
						this[i].ClassGenExceptionIconType == ClassGenExceptionIconType.Warning)
					{
						count++;
					}
				}
				return count;
			}
		}

		/// <summary>
		/// The critical exception collection
		/// </summary>
		public ClassGenExceptionCollection CriticalExceptionCollection
		{
			get
			{
				ClassGenExceptionCollection coll = new ClassGenExceptionCollection();
				for (int i = 0; i < this.Count; i++)
				{
					if (this[i].ClassGenExceptionIconType == ClassGenExceptionIconType.Critical)
					{
						coll.Add(this[i]);
					}
				}
				return coll;
			}
		}

		/// <summary>
		/// Get the critical exception count
		/// </summary>
		/// <returns>The count of critical errors</returns>
		public int CriticalExceptionCount
		{
			get
			{
				int count = 0;
				for (int i = 0; i < this.Count; i++)
				{
					if (this[i].ClassGenExceptionIconType == ClassGenExceptionIconType.Critical)
					{
						count++;
					}
				}
				return count;
			}
		}

		/// <summary>
		/// The warning exception collection
		/// </summary>
		public ClassGenExceptionCollection WarningExceptionCollection
		{
			get
			{
				ClassGenExceptionCollection coll = new ClassGenExceptionCollection();
				for (int i = 0; i < this.Count; i++)
				{
					if (this[i].ClassGenExceptionIconType == ClassGenExceptionIconType.Warning)
					{
						coll.Add(this[i]);
					}
				}
				return coll;
			}
		}

		/// <summary>
		/// Get the Warning exception count
		/// </summary>
		/// <returns>The count of Warning errors</returns>
		public int WarningExceptionCount
		{
			get
			{
				int count = 0;
				for (int i = 0; i < this.Count; i++)
				{
					if (this[i].ClassGenExceptionIconType == ClassGenExceptionIconType.Warning)
					{
						count++;
					}
				}
				return count;
			}
		}

		/// <summary>
		/// The information exception collection
		/// </summary>
		public ClassGenExceptionCollection InformationExceptionCollection
		{
			get
			{
				ClassGenExceptionCollection coll = new ClassGenExceptionCollection();
				for (int i = 0; i < this.Count; i++)
				{
					if (this[i].ClassGenExceptionIconType == ClassGenExceptionIconType.Information)
					{
						coll.Add(this[i]);
					}
				}
				return coll;
			}
		}
		
		/// <summary>
		/// Get the Information exception count
		/// </summary>
		/// <returns>The count of Information errors</returns>
		public int InformationExceptionCount
		{
			get
			{
				int count = 0;
				for (int i = 0; i < this.Count; i++)
				{
					if (this[i].ClassGenExceptionIconType == ClassGenExceptionIconType.Information)
					{
						count++;
					}
				}
				return count;
			}
		}

		/// <summary>
		/// The system exception collection
		/// </summary>
		public ClassGenExceptionCollection SystemExceptionCollection
		{
			get
			{
				ClassGenExceptionCollection coll = new ClassGenExceptionCollection();
				for (int i = 0; i < this.Count; i++)
				{
					if (this[i].ClassGenExceptionIconType == ClassGenExceptionIconType.System)
					{
						coll.Add(this[i]);
					}
				}
				return coll;
			}
		}
		
		/// <summary>
		/// Get the System exception count
		/// </summary>
		/// <returns>The count of System errors</returns>
		public int SystemExceptionCount
		{
			get
			{
				int count = 0;
				for (int i = 0; i < this.Count; i++)
				{
					if (this[i].ClassGenExceptionIconType == ClassGenExceptionIconType.System)
					{
						count++;
					}
				}
				return count;
			}
		}

		/// <summary>
		/// The viewable exception collection
		/// </summary>
		public ClassGenExceptionCollection ViewableExceptionCollection
		{
			get
			{
				ClassGenExceptionCollection coll = new ClassGenExceptionCollection();
				for (int i = 0; i < this.Count; i++)
				{
					if (this[i].ClassGenExceptionIconType == ClassGenExceptionIconType.Critical ||
						this[i].ClassGenExceptionIconType == ClassGenExceptionIconType.Warning ||
						this[i].ClassGenExceptionIconType == ClassGenExceptionIconType.Information)
					{
						coll.Add(this[i]);
					}
				}
				return coll;
			}
		}

		/// <summary>
		/// Get the Information, Warning and Critical exception counts all in one
		/// </summary>
		public int ViewableExceptionCount
		{
			get
			{
				int count = 0;
				for (int i = 0; i < this.Count; i++)
				{
					if (this[i].ClassGenExceptionIconType == ClassGenExceptionIconType.Critical ||
						this[i].ClassGenExceptionIconType == ClassGenExceptionIconType.Warning ||
						this[i].ClassGenExceptionIconType == ClassGenExceptionIconType.Information)
					{
						count++;
					}
				}
				return count;
			}
		}

		/// <summary>
		/// Messages relating to how many records were updated or 
		/// deleted are put in as informational messages - get them by
		/// using this method
		/// </summary>
		/// <returns>The number of records that have been updated</returns>
		public int UpdatedCount
		{
			get
			{
				int rtv = -1;
				for (int i = 0; i < this.Count; i++)
				{
					if (this[i].ClassGenExceptionIconType == ClassGenExceptionIconType.System &&
						this[i].Message.ToLower().Contains("records affected by the update"))
					{
						if (rtv < 0) { rtv = 0; }
						rtv += int.Parse(this[i].ExtendedDescription);
					}
				}
				return rtv;
			}
		}

		/// <summary>
		/// Messages relating to how many records were updated or 
		/// deleted are put in as informational messages - get them by
		/// using this method
		/// </summary>
		/// <returns>The number of records that have been deleted</returns>
		public int DeletedCount
		{
			get
			{
				int rtv = -1;
				for (int i = 0; i < this.Count; i++)
				{
					if (this[i].ClassGenExceptionIconType == ClassGenExceptionIconType.System &&
						this[i].Message.ToLower().Contains("records affected by the delete"))
					{
						if (rtv < 0) { rtv = 0; }
						rtv += int.Parse(this[i].ExtendedDescription);
					}
				}
				return rtv;
			}
		}
	}
	#endregion ClassGenException Class

    #region Public Variables
    public delegate T ConstructorParams<T>(params object[] parameters);
    public delegate T Constructor<T>();
    public delegate T Constructor<T, V1>(V1 arg1);
    public delegate T Constructor<T, V1, V2>(V1 arg1, V2 arg2);
    public delegate T Constructor<T, V1, V2, V3>(V1 arg1, V2 arg2, V3 arg3);
    public delegate T Constructor<T, V1, V2, V3, V4>(V1 arg1, V2 arg2, V3 arg3, V4 arg4);
    public delegate T Constructor<T, V1, V2, V3, V4, V5>(V1 arg1, V2 arg2, V3 arg3, V4 arg4, V5 arg5);
    public delegate T Constructor<T, V1, V2, V3, V4, V5, V6>(V1 arg1, V2 arg2, V3 arg3, V4 arg4, V5 arg5, V6 arg6);
    public delegate T Constructor<T, V1, V2, V3, V4, V5, V6, V7>(V1 arg1, V2 arg2, V3 arg3, V4 arg4, V5 arg5, V6 arg6, V7 arg7);
    public delegate T Constructor<T, V1, V2, V3, V4, V5, V6, V7, V8>(V1 arg1, V2 arg2, V3 arg3, V4 arg4, V5 arg5, V6 arg6, V7 arg7, V8 arg8);
    public delegate T Constructor<T, V1, V2, V3, V4, V5, V6, V7, V8, V9>(V1 arg1, V2 arg2, V3 arg3, V4 arg4, V5 arg5, V6 arg6, V7 arg7, V8 arg8, V9 arg9);

    public delegate void ProcParams<T>(T target, params object[] parameters);

    public delegate void Proc<T>(T target);
    public delegate void Proc<T, V1>(T target, V1 arg1);
    public delegate void Proc<T, V1, V2>(T target, V1 arg1, V2 arg2);
    public delegate void Proc<T, V1, V2, V3>(T target, V1 arg1, V2 arg2, V3 arg3);
    public delegate void Proc<T, V1, V2, V3, V4>(T target, V1 arg1, V2 arg2, V3 arg3, V4 arg4);
    public delegate void Proc<T, V1, V2, V3, V4, V5>(T target, V1 arg1, V2 arg2, V3 arg3, V4 arg4, V5 arg5);
    public delegate void Proc<T, V1, V2, V3, V4, V5, V6>(T target, V1 arg1, V2 arg2, V3 arg3, V4 arg4, V5 arg5, V6 arg6);
    public delegate void Proc<T, V1, V2, V3, V4, V5, V6, V7>(T target, V1 arg1, V2 arg2, V3 arg3, V4 arg4, V5 arg5, V6 arg6, V7 arg7);
    public delegate void Proc<T, V1, V2, V3, V4, V5, V6, V7, V8>(T target, V1 arg1, V2 arg2, V3 arg3, V4 arg4, V5 arg5, V6 arg6, V7 arg7, V8 arg8);
    public delegate void Proc<T, V1, V2, V3, V4, V5, V6, V7, V8, V9>(T target, V1 arg1, V2 arg2, V3 arg3, V4 arg4, V5 arg5, V6 arg6, V7 arg7, V8 arg8, V9 arg9);

    public delegate void StaticProcParams<T>(params object[] parameters);

    public delegate void StaticProc<T>();
    public delegate void StaticProc<T, V1>(V1 arg1);
    public delegate void StaticProc<T, V1, V2>(V1 arg1, V2 arg2);
    public delegate void StaticProc<T, V1, V2, V3>(V1 arg1, V2 arg2, V3 arg3);
    public delegate void StaticProc<T, V1, V2, V3, V4>(V1 arg1, V2 arg2, V3 arg3, V4 arg4);
    public delegate void StaticProc<T, V1, V2, V3, V4, V5>(V1 arg1, V2 arg2, V3 arg3, V4 arg4, V5 arg5);
    public delegate void StaticProc<T, V1, V2, V3, V4, V5, V6>(V1 arg1, V2 arg2, V3 arg3, V4 arg4, V5 arg5, V6 arg6);
    public delegate void StaticProc<T, V1, V2, V3, V4, V5, V6, V7>(V1 arg1, V2 arg2, V3 arg3, V4 arg4, V5 arg5, V6 arg6, V7 arg7);
    public delegate void StaticProc<T, V1, V2, V3, V4, V5, V6, V7, V8>(V1 arg1, V2 arg2, V3 arg3, V4 arg4, V5 arg5, V6 arg6, V7 arg7, V8 arg8);
    public delegate void StaticProc<T, V1, V2, V3, V4, V5, V6, V7, V8, V9>(V1 arg1, V2 arg2, V3 arg3, V4 arg4, V5 arg5, V6 arg6, V7 arg7, V8 arg8, V9 arg9);

    public delegate TRet HelperFuncParams<T, TRet>(T target, params object[] parameters);

    public delegate TRet HelperFunc<T, TRet>(T target);
    public delegate TRet HelperFunc<T, TRet, V1>(T target, V1 arg1);
    public delegate TRet HelperFunc<T, TRet, V1, V2>(T target, V1 arg1, V2 arg2);
    public delegate TRet HelperFunc<T, TRet, V1, V2, V3>(T target, V1 arg1, V2 arg2, V3 arg3);
    public delegate TRet HelperFunc<T, TRet, V1, V2, V3, V4>(T target, V1 arg1, V2 arg2, V3 arg3, V4 arg4);
    public delegate TRet HelperFunc<T, TRet, V1, V2, V3, V4, V5>(T target, V1 arg1, V2 arg2, V3 arg3, V4 arg4, V5 arg5);
    public delegate TRet HelperFunc<T, TRet, V1, V2, V3, V4, V5, V6>(T target, V1 arg1, V2 arg2, V3 arg3, V4 arg4, V5 arg5, V6 arg6);
    public delegate TRet HelperFunc<T, TRet, V1, V2, V3, V4, V5, V6, V7>(T target, V1 arg1, V2 arg2, V3 arg3, V4 arg4, V5 arg5, V6 arg6, V7 arg7);
    public delegate TRet HelperFunc<T, TRet, V1, V2, V3, V4, V5, V6, V7, V8>(T target, V1 arg1, V2 arg2, V3 arg3, V4 arg4, V5 arg5, V6 arg6, V7 arg7, V8 arg8);
    public delegate TRet HelperFunc<T, TRet, V1, V2, V3, V4, V5, V6, V7, V8, V9>(T target, V1 arg1, V2 arg2, V3 arg3, V4 arg4, V5 arg5, V6 arg6, V7 arg7, V8 arg8, V9 arg9);

    public delegate TRet StaticFuncParams<T, TRet>(params object[] parameters);

    public delegate TRet StaticFunc<T, TRet>();
    public delegate TRet StaticFunc<T, TRet, V1>(V1 arg1);
    public delegate TRet StaticFunc<T, TRet, V1, V2>(V1 arg1, V2 arg2);
    public delegate TRet StaticFunc<T, TRet, V1, V2, V3>(V1 arg1, V2 arg2, V3 arg3);
    public delegate TRet StaticFunc<T, TRet, V1, V2, V3, V4>(V1 arg1, V2 arg2, V3 arg3, V4 arg4);
    public delegate TRet StaticFunc<T, TRet, V1, V2, V3, V4, V5>(V1 arg1, V2 arg2, V3 arg3, V4 arg4, V5 arg5);
    public delegate TRet StaticFunc<T, TRet, V1, V2, V3, V4, V5, V6>(V1 arg1, V2 arg2, V3 arg3, V4 arg4, V5 arg5, V6 arg6);
    public delegate TRet StaticFunc<T, TRet, V1, V2, V3, V4, V5, V6, V7>(V1 arg1, V2 arg2, V3 arg3, V4 arg4, V5 arg5, V6 arg6, V7 arg7);
    public delegate TRet StaticFunc<T, TRet, V1, V2, V3, V4, V5, V6, V7, V8>(V1 arg1, V2 arg2, V3 arg3, V4 arg4, V5 arg5, V6 arg6, V7 arg7, V8 arg8);
    public delegate TRet StaticFunc<T, TRet, V1, V2, V3, V4, V5, V6, V7, V8, V9>(V1 arg1, V2 arg2, V3 arg3, V4 arg4, V5 arg5, V6 arg6, V7 arg7, V8 arg8, V9 arg9);

    public enum ParameterList
    {
        Auto
    }

	public class PublicStatics
	{
		//public static string UpdatingUserName = string.Empty;

		//private static int _keyedValueIndex = 0;

		///// <summary>
		///// Get the next key in the sequence
		///// </summary>
		///// <returns>The next downward key in the sequence</returns>
		//public static int GetNextKey()
		//{
		//    _keyedValueIndex--;
		//    return _keyedValueIndex;
		//}
	}
    #endregion Public Variables

    #region Class: ParameterCountMismatchException
    public class ParameterCountMismatchException : ArgumentException
    {
        private const string ErrorFormat = "Method {0}.{1} takes {2} arguments, passed {3}.";
        public static void CheckAndThrow(Type type, string methodName, int expectedParameterCount, int actualArgumentCount)
        {
            if (expectedParameterCount != actualArgumentCount)
                throw new ParameterCountMismatchException(type, methodName, expectedParameterCount, actualArgumentCount);
        }
        public ParameterCountMismatchException(Type type, string methodName, int expectedParameterCount, int actualArgumentCount)
            : base(string.Format(ErrorFormat, type, methodName, expectedParameterCount, actualArgumentCount))
        {
        }
        static MethodInfo s_CheckAndThrow;
        internal static MethodInfo CheckAndThrowMethodInfo
        {
            get
            {
                if (s_CheckAndThrow == null)
                {
                    s_CheckAndThrow = typeof(ParameterCountMismatchException).GetMethod("CheckAndThrow", BindingFlags.Public | BindingFlags.Static);
                }

                return s_CheckAndThrow;
            }
        }
    }
    #endregion Class: ParameterCountMismatchException

    #region Class: Dynamic
    public abstract class Dynamic<T>
    {
        public class Constructor
        {
            public class Params
                : DynamicBase<ConstructorParams<T>>.Params.Constructor
            {
            }

            #region Explicit arguments
            public class Explicit
                : DynamicBase<Constructor<T>>.Explicit.Constructor
            {
            }

            public class Explicit<V1>
                : DynamicBase<Constructor<T, V1>>.Explicit.Constructor
            {
            }

            public class Explicit<V1, V2>
                : DynamicBase<Constructor<T, V1, V2>>.Explicit.Constructor
            {
            }

            public class Explicit<V1, V2, V3>
                : DynamicBase<Constructor<T, V1, V2, V3>>.Explicit.Constructor
            {
            }

            public class Explicit<V1, V2, V3, V4>
                : DynamicBase<Constructor<T, V1, V2, V3, V4>>.Explicit.Constructor
            {
            }

            public class Explicit<V1, V2, V3, V4, V5>
                : DynamicBase<Constructor<T, V1, V2, V3, V4, V5>>.Explicit.Constructor
            {
            }

            public class Explicit<V1, V2, V3, V4, V5, V6>
                : DynamicBase<Constructor<T, V1, V2, V3, V4, V5, V6>>.Explicit.Constructor
            {
            }

            public class Explicit<V1, V2, V3, V4, V5, V6, V7>
                : DynamicBase<Constructor<T, V1, V2, V3, V4, V5, V6, V7>>.Explicit.Constructor
            {
            }

            public class Explicit<V1, V2, V3, V4, V5, V6, V7, V8>
                : DynamicBase<Constructor<T, V1, V2, V3, V4, V5, V6, V7, V8>>.Explicit.Constructor
            {
            }

            public class Explicit<V1, V2, V3, V4, V5, V6, V7, V8, V9>
                : DynamicBase<Constructor<T, V1, V2, V3, V4, V5, V6, V7, V8, V9>>.Explicit.Constructor
            {
            }
            #endregion
        }

        public abstract class Static
        {
            public abstract class Field<VF>
            {
                public abstract class Getter
                    : DynamicBase<StaticFunc<T, VF>>.Explicit.Static.Field<VF>.Getter
                {
                }

                public abstract class Setter
                    : DynamicBase<StaticProc<T, VF>>.Explicit.Static.Field<VF>.Setter
                {
                }
            }

            public abstract class Property<VP>
            {
                public abstract class Params
                {
                    public abstract class Getter
                        : DynamicBase<StaticFuncParams<T, VP>>.Params.Static.Property.Getter<VP>
                    {
                    }

                    public abstract class Setter
                        : DynamicBase<StaticProcParams<T>>.Params.Static.Property.Setter
                    {
                    }
                }

                public abstract class Explicit
                {
                    public abstract class Getter
                        : DynamicBase<StaticFunc<T, VP>>.Explicit.Static.Property<VP>.Getter
                    {
                    }

                    public abstract class Setter
                        : DynamicBase<StaticProc<T, VP>>.Explicit.Static.Property<VP>.Setter
                    {
                    }
                }

                public class Explicit<V1>
                {
                    public abstract class Getter
                        : DynamicBase<StaticFunc<T, VP, V1>>.Explicit.Static.Property<VP>.Getter
                    {
                    }

                    public abstract class Setter
                        : DynamicBase<StaticProc<T, VP, V1>>.Explicit.Static.Property<VP>.Setter
                    {
                    }
                }

                public class Explicit<V1, V2>
                {
                    public abstract class Getter
                        : DynamicBase<StaticFunc<T, VP, V1, V2>>.Explicit.Static.Property<VP>.Getter
                    {
                    }

                    public abstract class Setter
                        : DynamicBase<StaticProc<T, VP, V1, V2>>.Explicit.Static.Property<VP>.Setter
                    {
                    }
                }

                public class Explicit<V1, V2, V3>
                {
                    public abstract class Getter
                        : DynamicBase<StaticFunc<T, VP, V1, V2, V3>>.Explicit.Static.Property<VP>.Getter
                    {
                    }

                    public abstract class Setter
                        : DynamicBase<StaticProc<T, VP, V1, V2, V3>>.Explicit.Static.Property<VP>.Setter
                    {
                    }
                }

                public class Explicit<V1, V2, V3, V4>
                {
                    public abstract class Getter
                        : DynamicBase<StaticFunc<T, VP, V1, V2, V3, V4>>.Explicit.Static.Property<VP>.Getter
                    {
                    }

                    public abstract class Setter
                        : DynamicBase<StaticProc<T, VP, V1, V2, V3, V4>>.Explicit.Static.Property<VP>.Setter
                    {
                    }
                }

                public class Explicit<V1, V2, V3, V4, V5>
                {
                    public abstract class Getter
                        : DynamicBase<StaticFunc<T, VP, V1, V2, V3, V4, V5>>.Explicit.Static.Property<VP>.Getter
                    {
                    }

                    public abstract class Setter
                        : DynamicBase<StaticProc<T, VP, V1, V2, V3, V4, V5>>.Explicit.Static.Property<VP>.Setter
                    {
                    }
                }

                public class Explicit<V1, V2, V3, V4, V5, V6>
                {
                    public abstract class Getter
                        : DynamicBase<StaticFunc<T, VP, V1, V2, V3, V4, V5, V6>>.Explicit.Static.Property<VP>.Getter
                    {
                    }

                    public abstract class Setter
                        : DynamicBase<StaticProc<T, VP, V1, V2, V3, V4, V5, V6>>.Explicit.Static.Property<VP>.Setter
                    {
                    }
                }

                public class Explicit<V1, V2, V3, V4, V5, V6, V7>
                {
                    public abstract class Getter
                        : DynamicBase<StaticFunc<T, VP, V1, V2, V3, V4, V5, V6, V7>>.Explicit.Static.Property<VP>.Getter
                    {
                    }

                    public abstract class Setter
                        : DynamicBase<StaticProc<T, VP, V1, V2, V3, V4, V5, V6, V7>>.Explicit.Static.Property<VP>.Setter
                    {
                    }
                }

                public class Explicit<V1, V2, V3, V4, V5, V6, V7, V8>
                {
                    public abstract class Getter
                        : DynamicBase<StaticFunc<T, VP, V1, V2, V3, V4, V5, V6, V7, V8>>.Explicit.Static.Property<VP>.Getter
                    {
                    }

                    public abstract class Setter
                        : DynamicBase<StaticProc<T, VP, V1, V2, V3, V4, V5, V6, V7, V8>>.Explicit.Static.Property<VP>.Setter
                    {
                    }
                }
            }

            public abstract class Procedure
            {
                public class Params
                    : DynamicBase<StaticProcParams<T>>.Params.Static.Procedure
                {
                }

                public class Explicit
                    : DynamicBase<StaticProc<T>>.Explicit.Static.Procedure
                {
                }

                public class Explicit<V1>
                    : DynamicBase<StaticProc<T, V1>>.Explicit.Static.Procedure
                {
                }

                public class Explicit<V1, V2>
                    : DynamicBase<StaticProc<T, V1, V2>>.Explicit.Static.Procedure
                {
                }

                public class Explicit<V1, V2, V3>
                    : DynamicBase<StaticProc<T, V1, V2, V3>>.Explicit.Static.Procedure
                {
                }

                public class Explicit<V1, V2, V3, V4>
                    : DynamicBase<StaticProc<T, V1, V2, V3, V4>>.Explicit.Static.Procedure
                {
                }

                public class Explicit<V1, V2, V3, V4, V5>
                    : DynamicBase<StaticProc<T, V1, V2, V3, V4, V5>>.Explicit.Static.Procedure
                {
                }

                public class Explicit<V1, V2, V3, V4, V5, V6>
                    : DynamicBase<StaticProc<T, V1, V2, V3, V4, V5, V6>>.Explicit.Static.Procedure
                {
                }

                public class Explicit<V1, V2, V3, V4, V5, V6, V7>
                    : DynamicBase<StaticProc<T, V1, V2, V3, V4, V5, V6, V7>>.Explicit.Static.Procedure
                {
                }

                public class Explicit<V1, V2, V3, V4, V5, V6, V7, V8>
                    : DynamicBase<StaticProc<T, V1, V2, V3, V4, V5, V6, V7, V8>>.Explicit.Static.Procedure
                {
                }

                public class Explicit<V1, V2, V3, V4, V5, V6, V7, V8, V9>
                    : DynamicBase<StaticProc<T, V1, V2, V3, V4, V5, V6, V7, V8, V9>>.Explicit.Static.Procedure
                {
                }
            }

            public abstract class Function<TRet>
            {
                public class Params
                    : DynamicBase<StaticFuncParams<T, TRet>>.Params.Static.Function<TRet>
                {
                }

                public class Explicit
                    : DynamicBase<StaticFunc<T, TRet>>.Explicit.Static.Function<TRet>
                {
                }

                public class Explicit<V1>
                    : DynamicBase<StaticFunc<T, TRet, V1>>.Explicit.Static.Function<TRet>
                {
                }

                public class Explicit<V1, V2>
                    : DynamicBase<StaticFunc<T, TRet, V1, V2>>.Explicit.Static.Function<TRet>
                {
                }

                public class Explicit<V1, V2, V3>
                    : DynamicBase<StaticFunc<T, TRet, V1, V2, V3>>.Explicit.Static.Function<TRet>
                {
                }

                public class Explicit<V1, V2, V3, V4>
                    : DynamicBase<StaticFunc<T, TRet, V1, V2, V3, V4>>.Explicit.Static.Function<TRet>
                {
                }

                public class Explicit<V1, V2, V3, V4, V5>
                    : DynamicBase<StaticFunc<T, TRet, V1, V2, V3, V4, V5>>.Explicit.Static.Function<TRet>
                {
                }

                public class Explicit<V1, V2, V3, V4, V5, V6>
                    : DynamicBase<StaticFunc<T, TRet, V1, V2, V3, V4, V5, V6>>.Explicit.Static.Function<TRet>
                {
                }

                public class Explicit<V1, V2, V3, V4, V5, V6, V7>
                    : DynamicBase<StaticFunc<T, TRet, V1, V2, V3, V4, V5, V6, V7>>.Explicit.Static.Function<TRet>
                {
                }

                public class Explicit<V1, V2, V3, V4, V5, V6, V7, V8>
                    : DynamicBase<StaticFunc<T, TRet, V1, V2, V3, V4, V5, V6, V7, V8>>.Explicit.Static.Function<TRet>
                {
                }

                public class Explicit<V1, V2, V3, V4, V5, V6, V7, V8, V9>
                    : DynamicBase<StaticFunc<T, TRet, V1, V2, V3, V4, V5, V6, V7, V8, V9>>.Explicit.Static.Function<TRet>
                {
                }
            }
        }

        public abstract class Instance
        {
            public abstract class Field<VF>
            {
                public abstract class Getter
                    : DynamicBase<HelperFunc<T, VF>>.Explicit.Instance.Field<VF>.Getter
                {
                }

                public abstract class Setter
                    : DynamicBase<Proc<T, VF>>.Explicit.Instance.Field<VF>.Setter
                {
                }
            }

            public abstract class Property<VP>
            {
                public abstract class Params
                {
                    public abstract class Getter
                        : DynamicBase<HelperFuncParams<T, VP>>.Params.Instance.Property<VP>.Getter
                    {
                    }

                    public abstract class Setter
                        : DynamicBase<ProcParams<T>>.Params.Instance.Property<VP>.Setter
                    {
                    }
                }

                public abstract class Explicit
                {
                    public abstract class Getter
                        : DynamicBase<HelperFunc<T, VP>>.Explicit.Instance.Property<VP>.Getter
                    {
                    }

                    public abstract class Setter
                        : DynamicBase<Proc<T, VP>>.Explicit.Instance.Property<VP>.Setter
                    {
                    }
                }

                public class Explicit<V1>
                {
                    public abstract class Getter
                        : DynamicBase<HelperFunc<T, VP, V1>>.Explicit.Instance.Property<VP>.Getter
                    {
                    }

                    public abstract class Setter
                        : DynamicBase<Proc<T, VP, V1>>.Explicit.Instance.Property<VP>.Setter
                    {
                    }
                }

                public class Explicit<V1, V2>
                {
                    public abstract class Getter
                        : DynamicBase<HelperFunc<T, VP, V1, V2>>.Explicit.Instance.Property<VP>.Getter
                    {
                    }

                    public abstract class Setter
                        : DynamicBase<Proc<T, VP, V1, V2>>.Explicit.Instance.Property<VP>.Setter
                    {
                    }
                }

                public class Explicit<V1, V2, V3>
                {
                    public abstract class Getter
                        : DynamicBase<HelperFunc<T, VP, V1, V2, V3>>.Explicit.Instance.Property<VP>.Getter
                    {
                    }

                    public abstract class Setter
                        : DynamicBase<Proc<T, VP, V1, V2, V3>>.Explicit.Instance.Property<VP>.Setter
                    {
                    }
                }

                public class Explicit<V1, V2, V3, V4>
                {
                    public abstract class Getter
                        : DynamicBase<HelperFunc<T, VP, V1, V2, V3, V4>>.Explicit.Instance.Property<VP>.Getter
                    {
                    }

                    public abstract class Setter
                        : DynamicBase<Proc<T, VP, V1, V2, V3, V4>>.Explicit.Instance.Property<VP>.Setter
                    {
                    }
                }

                public class Explicit<V1, V2, V3, V4, V5>
                {
                    public abstract class Getter
                        : DynamicBase<HelperFunc<T, VP, V1, V2, V3, V4, V5>>.Explicit.Instance.Property<VP>.Getter
                    {
                    }

                    public abstract class Setter
                        : DynamicBase<Proc<T, VP, V1, V2, V3, V4, V5>>.Explicit.Instance.Property<VP>.Setter
                    {
                    }
                }

                public class Explicit<V1, V2, V3, V4, V5, V6>
                {
                    public abstract class Getter
                        : DynamicBase<HelperFunc<T, VP, V1, V2, V3, V4, V5, V6>>.Explicit.Instance.Property<VP>.Getter
                    {
                    }

                    public abstract class Setter
                        : DynamicBase<Proc<T, VP, V1, V2, V3, V4, V5, V6>>.Explicit.Instance.Property<VP>.Setter
                    {
                    }
                }

                public class Explicit<V1, V2, V3, V4, V5, V6, V7>
                {
                    public abstract class Getter
                        : DynamicBase<HelperFunc<T, VP, V1, V2, V3, V4, V5, V6, V7>>.Explicit.Instance.Property<VP>.Getter
                    {
                    }

                    public abstract class Setter
                        : DynamicBase<Proc<T, VP, V1, V2, V3, V4, V5, V6, V7>>.Explicit.Instance.Property<VP>.Setter
                    {
                    }
                }

                public class Explicit<V1, V2, V3, V4, V5, V6, V7, V8>
                {
                    public abstract class Getter
                        : DynamicBase<HelperFunc<T, VP, V1, V2, V3, V4, V5, V6, V7, V8>>.Explicit.Instance.Property<VP>.Getter
                    {
                    }

                    public abstract class Setter
                        : DynamicBase<Proc<T, VP, V1, V2, V3, V4, V5, V6, V7, V8>>.Explicit.Instance.Property<VP>.Setter
                    {
                    }
                }
            }

            public abstract class Procedure
            {
                public class Params
                    : DynamicBase<ProcParams<T>>.Params.Instance.Procedure
                {
                }

                public class Explicit
                    : DynamicBase<Proc<T>>.Explicit.Instance.Procedure
                {
                }

                public class Explicit<V1>
                    : DynamicBase<Proc<T, V1>>.Explicit.Instance.Procedure
                {
                }

                public class Explicit<V1, V2>
                    : DynamicBase<Proc<T, V1, V2>>.Explicit.Instance.Procedure
                {
                }

                public class Explicit<V1, V2, V3>
                    : DynamicBase<Proc<T, V1, V2, V3>>.Explicit.Instance.Procedure
                {
                }

                public class Explicit<V1, V2, V3, V4>
                    : DynamicBase<Proc<T, V1, V2, V3, V4>>.Explicit.Instance.Procedure
                {
                }

                public class Explicit<V1, V2, V3, V4, V5>
                    : DynamicBase<Proc<T, V1, V2, V3, V4, V5>>.Explicit.Instance.Procedure
                {
                }

                public class Explicit<V1, V2, V3, V4, V5, V6>
                    : DynamicBase<Proc<T, V1, V2, V3, V4, V5, V6>>.Explicit.Instance.Procedure
                {
                }

                public class Explicit<V1, V2, V3, V4, V5, V6, V7>
                    : DynamicBase<Proc<T, V1, V2, V3, V4, V5, V6, V7>>.Explicit.Instance.Procedure
                {
                }

                public class Explicit<V1, V2, V3, V4, V5, V6, V7, V8>
                    : DynamicBase<Proc<T, V1, V2, V3, V4, V5, V6, V7, V8>>.Explicit.Instance.Procedure
                {
                }

                public class Explicit<V1, V2, V3, V4, V5, V6, V7, V8, V9>
                    : DynamicBase<Proc<T, V1, V2, V3, V4, V5, V6, V7, V8, V9>>.Explicit.Instance.Procedure
                {
                }
            }

            public abstract class Function<TRet>
            {
                public class Params
                    : DynamicBase<HelperFuncParams<T, TRet>>.Params.Instance.Function<TRet>
                {
                }

                public class Explicit
                    : DynamicBase<HelperFunc<T, TRet>>.Explicit.Instance.Function<TRet>
                {
                }

                public class Explicit<V1>
                    : DynamicBase<HelperFunc<T, TRet, V1>>.Explicit.Instance.Function<TRet>
                {
                }

                public class Explicit<V1, V2>
                    : DynamicBase<HelperFunc<T, TRet, V1, V2>>.Explicit.Instance.Function<TRet>
                {
                }

                public class Explicit<V1, V2, V3>
                    : DynamicBase<HelperFunc<T, TRet, V1, V2, V3>>.Explicit.Instance.Function<TRet>
                {
                }

                public class Explicit<V1, V2, V3, V4>
                    : DynamicBase<HelperFunc<T, TRet, V1, V2, V3, V4>>.Explicit.Instance.Function<TRet>
                {
                }

                public class Explicit<V1, V2, V3, V4, V5>
                    : DynamicBase<HelperFunc<T, TRet, V1, V2, V3, V4, V5>>.Explicit.Instance.Function<TRet>
                {
                }

                public class Explicit<V1, V2, V3, V4, V5, V6>
                    : DynamicBase<HelperFunc<T, TRet, V1, V2, V3, V4, V5, V6>>.Explicit.Instance.Function<TRet>
                {
                }

                public class Explicit<V1, V2, V3, V4, V5, V6, V7>
                    : DynamicBase<HelperFunc<T, TRet, V1, V2, V3, V4, V5, V6, V7>>.Explicit.Instance.Function<TRet>
                {
                }

                public class Explicit<V1, V2, V3, V4, V5, V6, V7, V8>
                    : DynamicBase<HelperFunc<T, TRet, V1, V2, V3, V4, V5, V6, V7, V8>>.Explicit.Instance.Function<TRet>
                {
                }

                public class Explicit<V1, V2, V3, V4, V5, V6, V7, V8, V9>
                    : DynamicBase<HelperFunc<T, TRet, V1, V2, V3, V4, V5, V6, V7, V8, V9>>.Explicit.Instance.Function<TRet>
                {
                }
            }
        }

        public abstract class DynamicBase<D> where D : class
        {
            private const BindingFlags DefaultFlags = BindingFlags.Public | BindingFlags.NonPublic;

            protected const BindingFlags StaticFlags = DefaultFlags | BindingFlags.Static;

            protected const BindingFlags InstanceFlags = DefaultFlags | BindingFlags.Instance;

            private static void EmitLoadThis(DynamicEmit de, MemberInfo memberInfo)
            {
                de.LoadArgument(typeof(T).IsValueType, 0);  // get the "this" pointer we were passed

                if (memberInfo.DeclaringType != null && !memberInfo.DeclaringType.IsAssignableFrom(typeof(T)))
                {
                    de.CastTo(typeof(T), memberInfo.DeclaringType);
                }
            }

            private static void EmitGetField(DynamicEmit de, FieldInfo fieldInfo, Type memberType)
            {
                de.LoadField(fieldInfo);
                de.CastTo(fieldInfo.FieldType, memberType);
            }

            private static void EmitSetField(DynamicEmit de, int argumentOffset, FieldInfo fieldInfo, Type knownArgumentType)
            {
                de.LoadArgument(argumentOffset);

                if (knownArgumentType == null)
                {
                    de.LoadLiteral(0);
                    de.LoadElementReference();
                    de.CastTo(fieldInfo.FieldType);
                }
                else
                {
                    de.CastTo(knownArgumentType, fieldInfo.FieldType);
                }

                de.StoreField(fieldInfo);
            }

            public abstract class Params
            {
                private static void EmitArgumentCountCheck(DynamicEmit de, int argumentOffset, int parameterLength, string methodName)
                {
                    de.LoadType<T>();
                    de.LoadLiteral(methodName);
                    de.LoadLiteral(parameterLength);    // expected parameter length
                    de.LoadArgument(argumentOffset);
                    de.LoadArrayLength();
                    de.Convert<int>();                  // actual parameters supplied length
                    de.Call(ParameterCountMismatchException.CheckAndThrowMethodInfo);
                }

                private static LocalBuilder[] EmitCoerceArguments(DynamicEmit de, int argumentOffset, ParameterInfo[] parameterInfos)
                {
                    LocalBuilder[] locals = new LocalBuilder[parameterInfos.Length];
                    for (int i = 0; i < locals.Length; i++)
                    {
                        Type parameterType = parameterInfos[i].ParameterType;
                        de.LoadArgument(argumentOffset);
                        de.LoadLiteral(i);
                        de.LoadElementReference();
                        de.CastTo(parameterType);
                        locals[i] = de.DeclareLocal(parameterType);
                        de.StoreLocal(locals[i].LocalIndex);
                    }
                    return locals;
                }

                private static LocalBuilder[] EmitArgumentCheckAndCoerce(DynamicEmit de, int argumentOffset, ParameterInfo[] parameterInfos, MemberInfo member)
                {
                    EmitArgumentCountCheck(de, argumentOffset, parameterInfos.Length, member.Name);
                    return EmitCoerceArguments(de, argumentOffset, parameterInfos);
                }

                private static D Build(ConstructorInfo constructorInfo)
                {
                    if (constructorInfo == null)
                        throw new ArgumentNullException("constructorInfo");

                    Type[] argumentTypes = new Type[] { typeof(object[]) };

                    DynamicMethod dm = new DynamicMethod(constructorInfo.Name, MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard
                                                        , typeof(T), argumentTypes, typeof(T), true);
                    dm.InitLocals = true;
                    DynamicEmit de = new DynamicEmit(dm);

                    ParameterInfo[] parameterInfos = constructorInfo.GetParameters();
                    LocalBuilder[] locals = EmitArgumentCheckAndCoerce(de, 0, parameterInfos, constructorInfo);

                    for (int i = 0; i < locals.Length; i++)
                    {
                        de.LoadLocal(locals[i].LocalIndex); // get all of the now-correctly-typed arguments
                    }

                    de.Call(constructorInfo);

                    de.Return();
                    return dm.CreateDelegate(typeof(D)) as D;
                }

                private static D Build(Type memberType, FieldInfo fieldInfo, bool wantGet)
                {
                    if (memberType == null)
                        throw new ArgumentNullException("memberType");

                    if (fieldInfo == null)
                        throw new ArgumentNullException("fieldInfo");

                    if (memberType != typeof(void) && !memberType.IsAssignableFrom(fieldInfo.FieldType))
                        throw new NotSupportedException(string.Format("{0} cannot be assigned from actual  type {1} of field {2}.{3}"
                                                        , memberType, fieldInfo.FieldType, typeof(T), fieldInfo.Name));

                    int argumentOffset = fieldInfo.IsStatic ? 0 : 1;
                    Type[] argumentTypes = fieldInfo.IsStatic ? new Type[] { typeof(object[]) } : new Type[] { typeof(T), typeof(object[]) };

                    DynamicMethod dm = new DynamicMethod(fieldInfo.Name, MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard
                                                        , memberType, argumentTypes, typeof(T), true);
                    dm.InitLocals = !wantGet;
                    DynamicEmit de = new DynamicEmit(dm);

                    int parameterLength = wantGet ? 0 : 1;

                    EmitArgumentCountCheck(de, argumentOffset, parameterLength, fieldInfo.Name);

                    if (!fieldInfo.IsStatic)
                    {
                        EmitLoadThis(de, fieldInfo);
                    }

                    if (wantGet)
                        EmitGetField(de, fieldInfo, memberType);
                    else
                        EmitSetField(de, argumentOffset, fieldInfo, null);

                    de.Return();
                    return dm.CreateDelegate(typeof(D)) as D;
                }

                private static D Build(Type returnType, MethodInfo methodInfo)
                {
                    if (returnType == null)
                        throw new ArgumentNullException("returnType");

                    if (methodInfo == null)
                        throw new ArgumentNullException("methodInfo");

                    if (returnType != typeof(void) && !returnType.IsAssignableFrom(methodInfo.ReturnType))
                        throw new NotSupportedException(string.Format("{0} cannot be assigned from actual return type {1} of method {2}.{3}"
                                                        , returnType, methodInfo.ReturnType, typeof(T), methodInfo.Name));

                    int argumentOffset = methodInfo.IsStatic ? 0 : 1;
                    Type[] argumentTypes = methodInfo.IsStatic ? new Type[] { typeof(object[]) } : new Type[] { typeof(T), typeof(object[]) };

                    DynamicMethod dm = new DynamicMethod(methodInfo.Name, MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard
                                                        , returnType, argumentTypes, typeof(T), true);
                    dm.InitLocals = true;
                    DynamicEmit de = new DynamicEmit(dm);

                    ParameterInfo[] parameterInfos = methodInfo.GetParameters();
                    LocalBuilder[] locals = EmitArgumentCheckAndCoerce(de, argumentOffset, parameterInfos, methodInfo);

                    // now build up the call stack...
                    if (!methodInfo.IsStatic)
                    {
                        EmitLoadThis(de, methodInfo);
                    }

                    for (int i = 0; i < locals.Length; i++)
                    {
                        de.LoadLocal(locals[i].LocalIndex); // get all of the now-correctly-typed arguments
                    }

                    de.Call(methodInfo);
                    de.CastTo(methodInfo.ReturnType, returnType);

                    de.Return();
                    return dm.CreateDelegate(typeof(D)) as D;
                }

                public abstract class Constructor
                {
                    public static D CreateDelegate(ConstructorInfo constructorInfo)
                    {
                        return Build(constructorInfo);
                    }

                    public static D CreateDelegate(RuntimeMethodHandle methodHandle)
                    {
                        ConstructorInfo constructorInfo = (ConstructorInfo)ConstructorInfo.GetMethodFromHandle(methodHandle);
                        return CreateDelegate(constructorInfo);
                    }

                    public static D CreateDelegate()
                    {
                        return CreateDelegate(typeof(T).GetConstructor(InstanceFlags, null, Type.EmptyTypes, null));
                    }

                    public static D CreateDelegate(Type[] parameterTypes)
                    {
                        return CreateDelegate(typeof(T).GetConstructor(InstanceFlags, null, parameterTypes, null));
                    }
                }

                public abstract class Static
                {
                    public abstract class Field
                    {
                        public abstract class Getter<TRet>
                        {
                            public static D CreateDelegate(FieldInfo fieldInfo)
                            {
                                return Build(typeof(TRet), fieldInfo, true);
                            }

                            public static D CreateDelegate(RuntimeFieldHandle fieldHandle)
                            {
                                FieldInfo fieldInfo = (FieldInfo)FieldInfo.GetFieldFromHandle(fieldHandle);
                                return CreateDelegate(fieldInfo);
                            }

                            public static D CreateDelegate(string fieldName)
                            {
                                return CreateDelegate(typeof(T).GetField(fieldName, StaticFlags | BindingFlags.GetField));
                            }
                        }

                        public abstract class Setter
                        {
                            public static D CreateDelegate(FieldInfo fieldInfo)
                            {
                                return Build(typeof(void), fieldInfo, false);
                            }

                            public static D CreateDelegate(RuntimeFieldHandle fieldHandle)
                            {
                                FieldInfo fieldInfo = (FieldInfo)FieldInfo.GetFieldFromHandle(fieldHandle);
                                return CreateDelegate(fieldInfo);
                            }

                            public static D CreateDelegate(string fieldName)
                            {
                                return CreateDelegate(typeof(T).GetField(fieldName, StaticFlags | BindingFlags.SetField));
                            }
                        }
                    }

                    public abstract class Property
                    {
                        public abstract class Getter<TRet>
                        {
                            public static D CreateDelegate(PropertyInfo propertyInfo)
                            {
                                if (propertyInfo == null)
                                    throw new ArgumentNullException("propertyInfo");

                                return Build(typeof(TRet), propertyInfo.GetGetMethod(true));
                            }

                            public static D CreateDelegate(string propertyName)
                            {
                                return CreateDelegate(typeof(T).GetProperty(propertyName, StaticFlags | BindingFlags.GetProperty));
                            }

                            public static D CreateDelegate(string propertyName, Type[] parameterTypes)
                            {
                                return CreateDelegate(typeof(T).GetProperty(propertyName, StaticFlags | BindingFlags.GetProperty
                                                                            , null, typeof(TRet), parameterTypes, null));
                            }
                        }

                        public abstract class Setter
                        {
                            public static D CreateDelegate(PropertyInfo propertyInfo)
                            {
                                if (propertyInfo == null)
                                    throw new ArgumentNullException("propertyInfo");

                                return Build(typeof(void), propertyInfo.GetSetMethod(true));
                            }

                            public static D CreateDelegate(string propertyName)
                            {
                                return CreateDelegate(typeof(T).GetProperty(propertyName, StaticFlags | BindingFlags.SetProperty));
                            }

                            public static D CreateDelegate(string propertyName, Type[] parameterTypes)
                            {
                                return CreateDelegate(typeof(T).GetProperty(propertyName, StaticFlags | BindingFlags.SetProperty
                                                                            , null, typeof(void), parameterTypes, null));
                            }
                        }
                    }

                    public abstract class Procedure
                    {
                        public static D CreateDelegate(MethodInfo methodInfo)
                        {
                            return Build(typeof(void), methodInfo);
                        }

                        public static D CreateDelegate(RuntimeMethodHandle methodHandle)
                        {
                            MethodInfo methodInfo = (MethodInfo)MethodInfo.GetMethodFromHandle(methodHandle);
                            return CreateDelegate(methodInfo);
                        }

                        public static D CreateDelegate(string methodName)
                        {
                            return CreateDelegate(typeof(T).GetMethod(methodName, StaticFlags));
                        }

                        public static D CreateDelegate(string methodName, Type[] parameterTypes)
                        {
                            return CreateDelegate(typeof(T).GetMethod(methodName, StaticFlags, null, parameterTypes, null));
                        }
                    }

                    public abstract class Function<TRet>
                    {
                        public static D CreateDelegate(MethodInfo methodInfo)
                        {
                            return Build(typeof(TRet), methodInfo);
                        }

                        public static D CreateDelegate(RuntimeMethodHandle methodHandle)
                        {
                            MethodInfo methodInfo = (MethodInfo)MethodInfo.GetMethodFromHandle(methodHandle);
                            return CreateDelegate(methodInfo);
                        }

                        public static D CreateDelegate(string methodName)
                        {
                            return CreateDelegate(typeof(T).GetMethod(methodName, StaticFlags));
                        }

                        public static D CreateDelegate(string methodName, Type[] parameterTypes)
                        {
                            return CreateDelegate(typeof(T).GetMethod(methodName, StaticFlags, null, parameterTypes, null));
                        }
                    }
                }

                public abstract class Instance
                {
                    public abstract class Field<VF>
                    {
                        public abstract class Getter
                        {
                            public static D CreateDelegate(FieldInfo fieldInfo)
                            {
                                return Build(typeof(VF), fieldInfo, true);
                            }

                            public static D CreateDelegate(RuntimeFieldHandle fieldHandle)
                            {
                                FieldInfo fieldInfo = (FieldInfo)FieldInfo.GetFieldFromHandle(fieldHandle);
                                return CreateDelegate(fieldInfo);
                            }

                            public static D CreateDelegate(string fieldName)
                            {
                                return CreateDelegate(typeof(T).GetField(fieldName, InstanceFlags | BindingFlags.GetField));
                            }
                        }

                        public abstract class Setter
                        {
                            public static D CreateDelegate(FieldInfo fieldInfo)
                            {
                                return Build(typeof(void), fieldInfo, false);
                            }

                            public static D CreateDelegate(RuntimeFieldHandle fieldHandle)
                            {
                                FieldInfo fieldInfo = (FieldInfo)FieldInfo.GetFieldFromHandle(fieldHandle);
                                return CreateDelegate(fieldInfo);
                            }

                            public static D CreateDelegate(string fieldName)
                            {
                                return CreateDelegate(typeof(T).GetField(fieldName, InstanceFlags | BindingFlags.SetField));
                            }
                        }
                    }

                    public abstract class Property<VP>
                    {
                        public abstract class Getter
                        {
                            public static D CreateDelegate(PropertyInfo propertyInfo)
                            {
                                if (propertyInfo == null)
                                    throw new ArgumentNullException("propertyInfo");

                                return Build(typeof(VP), propertyInfo.GetGetMethod(true));
                            }

                            public static D CreateDelegate(string propertyName)
                            {
                                return CreateDelegate(typeof(T).GetProperty(propertyName, InstanceFlags | BindingFlags.GetProperty));
                            }

                            public static D CreateDelegate(string propertyName, Type[] parameterTypes)
                            {
                                return CreateDelegate(typeof(T).GetProperty(propertyName, InstanceFlags | BindingFlags.GetProperty
                                                                            , null, typeof(VP), parameterTypes, null));
                            }
                        }

                        public abstract class Setter
                        {
                            public static D CreateDelegate(PropertyInfo propertyInfo)
                            {
                                if (propertyInfo == null)
                                    throw new ArgumentNullException("propertyInfo");

                                return Build(typeof(void), propertyInfo.GetSetMethod(true));
                            }

                            public static D CreateDelegate(string propertyName)
                            {
                                return CreateDelegate(typeof(T).GetProperty(propertyName, InstanceFlags | BindingFlags.SetProperty));
                            }

                            public static D CreateDelegate(string propertyName, Type[] parameterTypes)
                            {
                                return CreateDelegate(typeof(T).GetProperty(propertyName, InstanceFlags | BindingFlags.SetProperty
                                                                            , null, typeof(void), parameterTypes, null));
                            }
                        }
                    }

                    public abstract class Procedure
                    {
                        public static D CreateDelegate(MethodInfo methodInfo)
                        {
                            return Build(typeof(void), methodInfo);
                        }

                        public static D CreateDelegate(RuntimeMethodHandle methodHandle)
                        {
                            MethodInfo methodInfo = (MethodInfo)MethodInfo.GetMethodFromHandle(methodHandle);
                            return CreateDelegate(methodInfo);
                        }

                        public static D CreateDelegate(string methodName)
                        {
                            return CreateDelegate(typeof(T).GetMethod(methodName, InstanceFlags));
                        }

                        public static D CreateDelegate(string methodName, Type[] parameterTypes)
                        {
                            return CreateDelegate(typeof(T).GetMethod(methodName, InstanceFlags, null, parameterTypes, null));
                        }
                    }

                    public abstract class Function<TRet>
                    {
                        public static D CreateDelegate(MethodInfo methodInfo)
                        {
                            return Build(typeof(TRet), methodInfo);
                        }

                        public static D CreateDelegate(RuntimeMethodHandle methodHandle)
                        {
                            MethodInfo methodInfo = (MethodInfo)MethodInfo.GetMethodFromHandle(methodHandle);
                            return CreateDelegate(methodInfo);
                        }

                        public static D CreateDelegate(string methodName)
                        {
                            return CreateDelegate(typeof(T).GetMethod(methodName, InstanceFlags));
                        }

                        public static D CreateDelegate(string methodName, Type[] parameterTypes)
                        {
                            return CreateDelegate(typeof(T).GetMethod(methodName, InstanceFlags, null, parameterTypes, null));
                        }
                    }
                }
            }

            public abstract class Explicit
            {
                private static D Build(ConstructorInfo constructorInfo, Type[] knownArgumentTypes)
                {
                    if (constructorInfo == null)
                        throw new ArgumentNullException("constructorInfo");

                    if (knownArgumentTypes == null)
                        throw new ArgumentNullException("argumentTypes");

                    ParameterInfo[] parameterInfos = constructorInfo.GetParameters();

                    if (parameterInfos.Length != knownArgumentTypes.Length)
                        throw new ParameterCountMismatchException(typeof(T), constructorInfo.Name, parameterInfos.Length, knownArgumentTypes.Length);

                    DynamicMethod dm = new DynamicMethod(constructorInfo.Name, MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard
                                                        , typeof(T), knownArgumentTypes, typeof(T), true);
                    dm.InitLocals = false; // we have no locals
                    DynamicEmit de = new DynamicEmit(dm);

                    for (int i = 0; i < parameterInfos.Length; i++)
                    {
                        de.LoadArgument(i);
                        de.CastTo(knownArgumentTypes[i], parameterInfos[i].ParameterType);
                    }

                    de.Call(constructorInfo);

                    de.Return();
                    return dm.CreateDelegate(typeof(D)) as D;
                }

                private static D Build(Type memberType, FieldInfo fieldInfo, bool wantGet, Type[] knownArgumentTypes)
                {
                    if (memberType == null)
                        throw new ArgumentNullException("memberType");

                    if (fieldInfo == null)
                        throw new ArgumentNullException("fieldInfo");

                    if (knownArgumentTypes == null)
                        throw new ArgumentNullException("knownArgumentTypes");

                    if (memberType != typeof(void) && !memberType.IsAssignableFrom(fieldInfo.FieldType))
                        throw new NotSupportedException(string.Format("{0} cannot be assigned from actual type {1} of field {2}.{3}"
                                                        , memberType, fieldInfo.FieldType, typeof(T), fieldInfo.Name));

                    int argumentOffset = fieldInfo.IsStatic ? 0 : 1;
                    int argumentCount = (knownArgumentTypes.Length - argumentOffset);

                    int parameterLength = wantGet ? 0 : 1;

                    if (argumentCount != parameterLength)
                        throw new ParameterCountMismatchException(typeof(T), fieldInfo.Name, parameterLength, argumentCount);

                    DynamicMethod dm = new DynamicMethod(fieldInfo.Name, MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard
                                                        , memberType, knownArgumentTypes, typeof(T), true);
                    dm.InitLocals = false;
                    DynamicEmit de = new DynamicEmit(dm);

                    // now build up the call stack...
                    if (!fieldInfo.IsStatic)
                    {
                        EmitLoadThis(de, fieldInfo);
                    }

                    if (wantGet)
                        EmitGetField(de, fieldInfo, memberType);
                    else
                        EmitSetField(de, argumentOffset, fieldInfo, knownArgumentTypes[argumentOffset]);

                    de.Return();
                    return dm.CreateDelegate(typeof(D)) as D;
                }

                private static D Build(Type returnType, MethodInfo methodInfo, Type[] knownArgumentTypes)
                {
                    if (returnType == null)
                        throw new ArgumentNullException("returnType");

                    if (methodInfo == null)
                        throw new ArgumentNullException("methodInfo");

                    if (knownArgumentTypes == null)
                        throw new ArgumentNullException("argumentTypes");

                    if (returnType != typeof(void) && !returnType.IsAssignableFrom(methodInfo.ReturnType))
                        throw new NotSupportedException(string.Format("{0} cannot be assigned from actual return type {1} of method {2}.{3}"
                                                        , returnType, methodInfo.ReturnType, typeof(T), methodInfo.Name));

                    int argumentOffset = methodInfo.IsStatic ? 0 : 1;
                    int argumentCount = (knownArgumentTypes.Length - argumentOffset);

                    ParameterInfo[] parameterInfos = methodInfo.GetParameters();

                    if (argumentCount != parameterInfos.Length)
                        throw new ParameterCountMismatchException(typeof(T), methodInfo.Name, parameterInfos.Length, argumentCount);

                    DynamicMethod dm = new DynamicMethod(methodInfo.Name, MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard
                                                        , returnType, knownArgumentTypes, typeof(T), true);
                    dm.InitLocals = false; // we have no locals
                    DynamicEmit de = new DynamicEmit(dm);

                    if (!methodInfo.IsStatic)
                    {
                        EmitLoadThis(de, methodInfo);
                    }

                    for (int i = 0; i < parameterInfos.Length; i++)
                    {
                        de.LoadArgument(i + argumentOffset);
                        de.CastTo(knownArgumentTypes[i + argumentOffset], parameterInfos[i].ParameterType);
                    }

                    de.Call(methodInfo);
                    de.CastTo(methodInfo.ReturnType, returnType);

                    de.Return();
                    return dm.CreateDelegate(typeof(D)) as D;
                }

                private static Type[] KnownArgumentTypes(int discardFirstCount)
                {
                    return KnownArgumentTypes(0, discardFirstCount);
                }

                private static Type[] KnownArgumentTypes(int discardFirst, int discardCount)
                {
                    Type[] delegateTypes = typeof(D).GetGenericArguments();

                    if (discardCount == 0)
                        return delegateTypes;

                    Type[] knownArgumentTypes = new Type[delegateTypes.Length - discardCount];

                    if (discardFirst + discardCount <= delegateTypes.Length)
                        Array.Copy(delegateTypes, discardFirst + discardCount, knownArgumentTypes, discardFirst, knownArgumentTypes.Length - discardFirst);

                    if (discardFirst > 0)
                        Array.Copy(delegateTypes, 0, knownArgumentTypes, 0, discardFirst);

                    return knownArgumentTypes;
                }

                public abstract class Constructor
                {
                    public static D CreateDelegate(ConstructorInfo constructorInfo)
                    {
                        return Build(constructorInfo, KnownArgumentTypes(1));
                    }

                    public static D CreateDelegate(RuntimeMethodHandle methodHandle)
                    {
                        ConstructorInfo constructorInfo = (ConstructorInfo)ConstructorInfo.GetMethodFromHandle(methodHandle);
                        return CreateDelegate(constructorInfo);
                    }

                    public static D CreateDelegate()
                    {
                        return CreateDelegate(typeof(T).GetConstructor(InstanceFlags, null, Type.EmptyTypes, null));
                    }

                    public static D CreateDelegate(ParameterList flag)
                    {
                        return CreateDelegate(KnownArgumentTypes(1));
                    }

                    public static D CreateDelegate(Type[] parameterTypes)
                    {
                        return CreateDelegate(typeof(T).GetConstructor(InstanceFlags, null, parameterTypes, null));
                    }
                }

                public abstract class Static
                {
                    public abstract class Field<VF>
                    {
                        public abstract class Getter
                        {
                            public static D CreateDelegate(FieldInfo fieldInfo)
                            {
                                return Build(typeof(VF), fieldInfo, true, Type.EmptyTypes);
                            }

                            public static D CreateDelegate(RuntimeFieldHandle fieldHandle)
                            {
                                FieldInfo fieldInfo = (FieldInfo)FieldInfo.GetFieldFromHandle(fieldHandle);
                                return CreateDelegate(fieldInfo);
                            }

                            public static D CreateDelegate(string fieldName)
                            {
                                return CreateDelegate(typeof(T).GetField(fieldName, StaticFlags | BindingFlags.GetField));
                            }
                        }

                        public abstract class Setter
                        {
                            public static D CreateDelegate(FieldInfo fieldInfo)
                            {
                                return Build(typeof(void), fieldInfo, false, new Type[] { typeof(VF) });
                            }

                            public static D CreateDelegate(RuntimeFieldHandle fieldHandle)
                            {
                                FieldInfo fieldInfo = (FieldInfo)FieldInfo.GetFieldFromHandle(fieldHandle);
                                return CreateDelegate(fieldInfo);
                            }

                            public static D CreateDelegate(string fieldName)
                            {
                                return CreateDelegate(typeof(T).GetField(fieldName, StaticFlags | BindingFlags.SetField));
                            }
                        }
                    }

                    public abstract class Property<VP>
                    {
                        public abstract class Getter
                        {
                            public static D CreateDelegate(PropertyInfo propertyInfo)
                            {
                                if (propertyInfo == null)
                                    throw new ArgumentNullException("propertyInfo");

                                return Build(typeof(VP), propertyInfo.GetGetMethod(true), KnownArgumentTypes(2));
                            }

                            public static D CreateDelegate(string propertyName)
                            {
                                return CreateDelegate(typeof(T).GetProperty(propertyName, StaticFlags | BindingFlags.GetProperty));
                            }

                            public static D CreateDelegate(string propertyName, ParameterList flag)
                            {
                                return CreateDelegate(propertyName, KnownArgumentTypes(2));
                            }

                            public static D CreateDelegate(string propertyName, Type[] parameterTypes)
                            {
                                return CreateDelegate(typeof(T).GetProperty(propertyName, StaticFlags | BindingFlags.GetProperty
                                                                            , null, typeof(VP), parameterTypes, null));
                            }
                        }

                        public abstract class Setter
                        {
                            public static D CreateDelegate(PropertyInfo propertyInfo)
                            {
                                if (propertyInfo == null)
                                    throw new ArgumentNullException("propertyInfo");

                                return Build(typeof(void), propertyInfo.GetSetMethod(true), KnownArgumentTypes(1));
                            }

                            public static D CreateDelegate(string propertyName)
                            {
                                return CreateDelegate(typeof(T).GetProperty(propertyName, StaticFlags | BindingFlags.SetProperty));
                            }

                            public static D CreateDelegate(string propertyName, ParameterList flag)
                            {
                                return CreateDelegate(propertyName, KnownArgumentTypes(1));
                            }

                            public static D CreateDelegate(string propertyName, Type[] parameterTypes)
                            {
                                return CreateDelegate(typeof(T).GetProperty(propertyName, StaticFlags | BindingFlags.SetProperty
                                                                            , null, typeof(void), parameterTypes, null));
                            }
                        }
                    }

                    public abstract class Procedure
                    {
                        public static D CreateDelegate(MethodInfo methodInfo)
                        {
                            return Build(typeof(void), methodInfo, KnownArgumentTypes(1));
                        }

                        public static D CreateDelegate(RuntimeMethodHandle methodHandle)
                        {
                            MethodInfo methodInfo = (MethodInfo)MethodInfo.GetMethodFromHandle(methodHandle);
                            return CreateDelegate(methodInfo);
                        }

                        public static D CreateDelegate(string methodName)
                        {
                            return CreateDelegate(typeof(T).GetMethod(methodName, StaticFlags));
                        }

                        public static D CreateDelegate(string methodName, ParameterList flag)
                        {
                            return CreateDelegate(methodName, KnownArgumentTypes(1));
                        }

                        public static D CreateDelegate(string methodName, Type[] parameterTypes)
                        {
                            return CreateDelegate(typeof(T).GetMethod(methodName, StaticFlags, null, parameterTypes, null));
                        }
                    }

                    public abstract class Function<TRet>
                    {
                        public static D CreateDelegate(MethodInfo methodInfo)
                        {
                            return Build(typeof(TRet), methodInfo, KnownArgumentTypes(2));
                        }

                        public static D CreateDelegate(RuntimeMethodHandle methodHandle)
                        {
                            MethodInfo methodInfo = (MethodInfo)MethodInfo.GetMethodFromHandle(methodHandle);
                            return CreateDelegate(methodInfo);
                        }

                        public static D CreateDelegate(string methodName)
                        {
                            return CreateDelegate(typeof(T).GetMethod(methodName, StaticFlags));
                        }

                        public static D CreateDelegate(string methodName, ParameterList flag)
                        {
                            return CreateDelegate(methodName, KnownArgumentTypes(2));
                        }

                        public static D CreateDelegate(string methodName, Type[] parameterTypes)
                        {
                            return CreateDelegate(typeof(T).GetMethod(methodName, StaticFlags, null, parameterTypes, null));
                        }
                    }
                }

                public abstract class Instance
                {
                    public abstract class Field<VF>
                    {
                        public abstract class Getter
                        {
                            public static D CreateDelegate(FieldInfo fieldInfo)
                            {
                                return Build(typeof(VF), fieldInfo, true, new Type[] { typeof(T) });
                            }

                            public static D CreateDelegate(RuntimeFieldHandle fieldHandle)
                            {
                                FieldInfo fieldInfo = (FieldInfo)FieldInfo.GetFieldFromHandle(fieldHandle);
                                return CreateDelegate(fieldInfo);
                            }

                            public static D CreateDelegate(string fieldName)
                            {
                                return CreateDelegate(typeof(T).GetField(fieldName, InstanceFlags | BindingFlags.GetField));
                            }
                        }

                        public abstract class Setter
                        {
                            public static D CreateDelegate(FieldInfo fieldInfo)
                            {
                                return Build(typeof(void), fieldInfo, false, new Type[] { typeof(T), typeof(VF) });
                            }

                            public static D CreateDelegate(RuntimeFieldHandle fieldHandle)
                            {
                                FieldInfo fieldInfo = (FieldInfo)FieldInfo.GetFieldFromHandle(fieldHandle);
                                return CreateDelegate(fieldInfo);
                            }

                            public static D CreateDelegate(string fieldName)
                            {
                                return CreateDelegate(typeof(T).GetField(fieldName, InstanceFlags | BindingFlags.SetField));
                            }
                        }
                    }

                    public abstract class Property<VP>
                    {
                        public abstract class Getter
                        {
                            public static D CreateDelegate(PropertyInfo propertyInfo)
                            {
                                if (propertyInfo == null)
                                    throw new ArgumentNullException("propertyInfo");

                                return Build(typeof(VP), propertyInfo.GetGetMethod(true), KnownArgumentTypes(1, 1));
                            }

                            public static D CreateDelegate(string propertyName)
                            {
                                return CreateDelegate(typeof(T).GetProperty(propertyName, InstanceFlags | BindingFlags.GetProperty));
                            }

                            public static D CreateDelegate(string propertyName, ParameterList flag)
                            {
                                return CreateDelegate(propertyName, KnownArgumentTypes(2));
                            }

                            public static D CreateDelegate(string propertyName, Type[] parameterTypes)
                            {
                                return CreateDelegate(typeof(T).GetProperty(propertyName, InstanceFlags | BindingFlags.GetProperty
                                                                            , null, typeof(VP), parameterTypes, null));
                            }
                        }

                        public abstract class Setter
                        {
                            public static D CreateDelegate(PropertyInfo propertyInfo)
                            {
                                if (propertyInfo == null)
                                    throw new ArgumentNullException("propertyInfo");

                                return Build(typeof(void), propertyInfo.GetSetMethod(true), KnownArgumentTypes(0));
                            }

                            public static D CreateDelegate(string propertyName)
                            {
                                return CreateDelegate(typeof(T).GetProperty(propertyName, InstanceFlags | BindingFlags.SetProperty));
                            }

                            public static D CreateDelegate(string propertyName, ParameterList flag)
                            {
                                return CreateDelegate(propertyName, KnownArgumentTypes(1));
                            }

                            public static D CreateDelegate(string propertyName, Type[] parameterTypes)
                            {
                                return CreateDelegate(typeof(T).GetProperty(propertyName, InstanceFlags | BindingFlags.SetProperty
                                                                            , null, typeof(void), parameterTypes, null));
                            }
                        }
                    }

                    public abstract class Procedure
                    {
                        public static D CreateDelegate(MethodInfo methodInfo)
                        {
                            return Build(typeof(void), methodInfo, KnownArgumentTypes(0));
                        }

                        public static D CreateDelegate(RuntimeMethodHandle methodHandle)
                        {
                            MethodInfo methodInfo = (MethodInfo)MethodInfo.GetMethodFromHandle(methodHandle);
                            return CreateDelegate(methodInfo);
                        }

                        public static D CreateDelegate(string methodName)
                        {
                            return CreateDelegate(typeof(T).GetMethod(methodName, InstanceFlags));
                        }

                        public static D CreateDelegate(string methodName, ParameterList flag)
                        {
                            return CreateDelegate(methodName, KnownArgumentTypes(1));
                        }

                        public static D CreateDelegate(string methodName, Type[] parameterTypes)
                        {
                            return CreateDelegate(typeof(T).GetMethod(methodName, InstanceFlags, null, parameterTypes, null));
                        }
                    }

                    public abstract class Function<TRet>
                    {
                        public static D CreateDelegate(MethodInfo methodInfo)
                        {
                            return Build(typeof(TRet), methodInfo, KnownArgumentTypes(1, 1));
                        }

                        public static D CreateDelegate(RuntimeMethodHandle methodHandle)
                        {
                            MethodInfo methodInfo = (MethodInfo)MethodInfo.GetMethodFromHandle(methodHandle);
                            return CreateDelegate(methodInfo);
                        }

                        public static D CreateDelegate(string methodName)
                        {
                            return CreateDelegate(typeof(T).GetMethod(methodName, InstanceFlags));
                        }

                        public static D CreateDelegate(string methodName, ParameterList flag)
                        {
                            return CreateDelegate(methodName, KnownArgumentTypes(2));
                        }

                        public static D CreateDelegate(string methodName, Type[] parameterTypes)
                        {
                            return CreateDelegate(typeof(T).GetMethod(methodName, InstanceFlags, null, parameterTypes, null));
                        }
                    }
                }
            }
        }
    }
    #endregion Class: Dynamic

    #region Class: DynamicEmit
    public class DynamicEmit
    {
        private static readonly Dictionary<Type, OpCode> s_Converts;

        static DynamicEmit()
        {
            s_Converts = new Dictionary<Type, OpCode>();
            s_Converts.Add(typeof(sbyte), OpCodes.Conv_I1);
            s_Converts.Add(typeof(short), OpCodes.Conv_I2);
            s_Converts.Add(typeof(int), OpCodes.Conv_I4);
            s_Converts.Add(typeof(long), OpCodes.Conv_I8);

            s_Converts.Add(typeof(byte), OpCodes.Conv_U1);
            s_Converts.Add(typeof(ushort), OpCodes.Conv_U2);
            s_Converts.Add(typeof(uint), OpCodes.Conv_U4);
            s_Converts.Add(typeof(ulong), OpCodes.Conv_U8);

            s_Converts.Add(typeof(float), OpCodes.Conv_R4);
            s_Converts.Add(typeof(double), OpCodes.Conv_R8);

            s_Converts.Add(typeof(bool), OpCodes.Conv_I1);
            s_Converts.Add(typeof(char), OpCodes.Conv_U2);
        }
        private ILGenerator _ilGen;

        public DynamicEmit(DynamicMethod dm)
        {
            _ilGen = dm.GetILGenerator();
        }

        public DynamicEmit(ILGenerator ilGen)
        {
            _ilGen = ilGen;
        }

        public LocalBuilder DeclareLocal(Type type)
        {
            return _ilGen.DeclareLocal(type);
        }

        public Label DefineLabel()
        {
            return _ilGen.DefineLabel();
        }

        public void MarkLabel(Label loc)
        {
            _ilGen.MarkLabel(loc);
        }

        public void LoadElementReference()
        {
            _ilGen.Emit(OpCodes.Ldelem_Ref);
        }

        public void LoadLiteral(int value)
        {
            switch (value)
            {
                case -1:
                    _ilGen.Emit(OpCodes.Ldc_I4_M1);
                    return;
                case 0:
                    _ilGen.Emit(OpCodes.Ldc_I4_0);
                    return;
                case 1:
                    _ilGen.Emit(OpCodes.Ldc_I4_1);
                    return;
                case 2:
                    _ilGen.Emit(OpCodes.Ldc_I4_2);
                    return;
                case 3:
                    _ilGen.Emit(OpCodes.Ldc_I4_3);
                    return;
                case 4:
                    _ilGen.Emit(OpCodes.Ldc_I4_4);
                    return;
                case 5:
                    _ilGen.Emit(OpCodes.Ldc_I4_5);
                    return;
                case 6:
                    _ilGen.Emit(OpCodes.Ldc_I4_6);
                    return;
                case 7:
                    _ilGen.Emit(OpCodes.Ldc_I4_7);
                    return;
                case 8:
                    _ilGen.Emit(OpCodes.Ldc_I4_8);
                    return;
            }

            if (value > -129 && value < 128)
            {
                _ilGen.Emit(OpCodes.Ldc_I4_S, (SByte)value);
            }
            else
            {
                _ilGen.Emit(OpCodes.Ldc_I4, value);
            }
        }

        public void LoadLiteral(long value)
        {
            _ilGen.Emit(OpCodes.Ldc_I8, value);
        }

        public void LoadLiteral(float value)
        {
            _ilGen.Emit(OpCodes.Ldc_R4, value);
        }

        public void LoadLiteral(double value)
        {
            _ilGen.Emit(OpCodes.Ldc_R8, value);
        }

        public void LoadLiteral(string value)
        {
            _ilGen.Emit(OpCodes.Ldstr, value);
        }

        public void LoadArrayLength()
        {
            _ilGen.Emit(OpCodes.Ldlen);
        }

        public void LoadToken(Type type)
        {
            _ilGen.Emit(OpCodes.Ldtoken, type);
        }

        public void LoadToken<T>()
        {
            LoadToken(typeof(T));
        }

        public void TypeFromHandle()
        {
            MethodInfo translator = typeof(Type).GetMethod("GetTypeFromHandle", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(RuntimeTypeHandle) }, null);
            Call(translator);
        }

        public void LoadType(Type type)
        {
            LoadToken(type);
            TypeFromHandle();
        }

        public void LoadType<T>()
        {
            LoadType(typeof(T));
        }

        public void StringFormat()
        {
            MethodInfo formatter = typeof(String).GetMethod("Format", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), typeof(object[]) }, null);
            Call(formatter);
        }

        public void NewArray<T>(int length)
        {
            LoadLiteral(length);
            _ilGen.Emit(OpCodes.Newarr, typeof(T));
        }

        public void StoreElement()
        {
            _ilGen.Emit(OpCodes.Stelem);
        }

        public void StoreElementReference()
        {
            _ilGen.Emit(OpCodes.Stelem_Ref);
        }

        public void NewObject(ConstructorInfo constructor)
        {
            _ilGen.Emit(OpCodes.Newobj, constructor);
        }

        public void Throw()
        {
            _ilGen.Emit(OpCodes.Throw);
        }

        public void Convert(Type toType)
        {
            _ilGen.Emit(s_Converts[toType]);
        }

        public void Convert<T>()
        {
            Convert(typeof(T));
        }

        public void CastTo(Type toType)
        {
            if (toType.IsValueType)
            {
                _ilGen.Emit(OpCodes.Unbox_Any, toType);
            }
            else
            {
                _ilGen.Emit(OpCodes.Castclass, toType);
            }
        }

        public void CastTo(Type fromType, Type toType)
        {
            if (fromType == toType)
                return;

            if (toType == typeof(void))
            {
                if (fromType != typeof(void))
                    this.Pop();
            }
            else
            {
                if (fromType.IsValueType)
                {
                    if (toType.IsValueType)
                    {
                        Convert(toType);
                        return;
                    }

                    _ilGen.Emit(OpCodes.Box, fromType);
                }

                CastTo(toType);
            }
        }

        public void LoadArgumentAddress(int argumentIndex)
        {
            if (argumentIndex < 256)
                _ilGen.Emit(OpCodes.Ldarga_S, (byte)argumentIndex);
            else
                _ilGen.Emit(OpCodes.Ldarga, argumentIndex);
        }

        public void LoadArgument(int argumentIndex)
        {
            switch (argumentIndex)
            {
                case 0:
                    _ilGen.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    _ilGen.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    _ilGen.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    _ilGen.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    if (argumentIndex < 256)
                        _ilGen.Emit(OpCodes.Ldarg_S, (byte)argumentIndex);
                    else
                        _ilGen.Emit(OpCodes.Ldarg, argumentIndex);
                    break;
            }
        }

        public void LoadLocalAddress(int localIndex)
        {
            if (localIndex < 256)
                _ilGen.Emit(OpCodes.Ldloca_S, (byte)localIndex);
            else
                _ilGen.Emit(OpCodes.Ldloca, localIndex);
        }

        public void LoadLocal(int localIndex)
        {
            switch (localIndex)
            {
                case 0:
                    _ilGen.Emit(OpCodes.Ldloc_0);
                    break;
                case 1:
                    _ilGen.Emit(OpCodes.Ldloc_1);
                    break;
                case 2:
                    _ilGen.Emit(OpCodes.Ldloc_2);
                    break;
                case 3:
                    _ilGen.Emit(OpCodes.Ldloc_3);
                    break;
                default:
                    if (localIndex < 256)
                        _ilGen.Emit(OpCodes.Ldloc_S, (byte)localIndex);
                    else
                        _ilGen.Emit(OpCodes.Ldloc, localIndex);
                    break;
            }
        }

        public void StoreField(FieldInfo field)
        {
            if (field.IsStatic)
            {
                _ilGen.Emit(OpCodes.Stsfld, field);
            }
            else
            {
                _ilGen.Emit(OpCodes.Stfld, field);
            }
        }

        public void StoreLocal(int localIndex)
        {
            switch (localIndex)
            {
                case 0:
                    _ilGen.Emit(OpCodes.Stloc_0);
                    break;
                case 1:
                    _ilGen.Emit(OpCodes.Stloc_1);
                    break;
                case 2:
                    _ilGen.Emit(OpCodes.Stloc_2);
                    break;
                case 3:
                    _ilGen.Emit(OpCodes.Stloc_3);
                    break;
                default:
                    if (localIndex < 256)
                        _ilGen.Emit(OpCodes.Stloc_S, (byte)localIndex);
                    else
                        _ilGen.Emit(OpCodes.Stloc, localIndex);
                    break;
            }
        }

        public void LoadNull()
        {
            _ilGen.Emit(OpCodes.Ldnull);
        }

        public void Return()
        {
            _ilGen.Emit(OpCodes.Ret);
        }

        public void Call(ConstructorInfo constructor)
        {
            _ilGen.Emit(OpCodes.Newobj, constructor);
        }

        public void Call(MethodInfo method)
        {
            if (method.IsFinal || !method.IsVirtual)
            {
                _ilGen.EmitCall(OpCodes.Call, method, null);
            }
            else
            {
                _ilGen.EmitCall(OpCodes.Callvirt, method, null);
            }
        }

        public void LoadArgument(bool targetIsValueType, int argumentIndex)
        {
            if (targetIsValueType)
            {
                LoadArgumentAddress(argumentIndex);
            }
            else
            {
                LoadArgument(argumentIndex);
            }
        }

        public void LoadField(FieldInfo field)
        {
            if (field.IsStatic)
            {
                _ilGen.Emit(OpCodes.Ldsfld, field);
            }
            else
            {
                _ilGen.Emit(OpCodes.Ldfld, field);
            }
        }

        public void BoxIfNeeded(Type type)
        {
            if (type.IsValueType || type.IsEnum)
            {
                _ilGen.Emit(OpCodes.Box, type);
            }
        }

        public void BoxIfNeeded<T>()
        {
            BoxIfNeeded(typeof(T));
        }

        public void Duplicate()
        {
            _ilGen.Emit(OpCodes.Dup);
        }

        public void Pop()
        {
            _ilGen.Emit(OpCodes.Pop);
        }

        public void Branch(Label label, bool isShort)
        {
            if (isShort)
                _ilGen.Emit(OpCodes.Br_S, label);
            else
                _ilGen.Emit(OpCodes.Br, label);
        }

        public void BranchEqual(Label label, bool isShort)
        {
            if (isShort)
                _ilGen.Emit(OpCodes.Beq_S, label);
            else
                _ilGen.Emit(OpCodes.Beq, label);
        }

        public void BranchEqual(Label label, bool isShort, int value)
        {
            LoadLiteral(value);

            if (isShort)
                _ilGen.Emit(OpCodes.Beq_S, label);
            else
                _ilGen.Emit(OpCodes.Beq, label);
        }

        public void BranchEqual(Label label, bool isShort, long value)
        {
            LoadLiteral(value);

            if (isShort)
                _ilGen.Emit(OpCodes.Beq_S, label);
            else
                _ilGen.Emit(OpCodes.Beq, label);
        }

        public void BranchEqual(Label label, bool isShort, float value)
        {
            LoadLiteral(value);

            if (isShort)
                _ilGen.Emit(OpCodes.Beq_S, label);
            else
                _ilGen.Emit(OpCodes.Beq, label);
        }

        public void BranchEqual(Label label, bool isShort, double value)
        {
            LoadLiteral(value);

            if (isShort)
                _ilGen.Emit(OpCodes.Beq_S, label);
            else
                _ilGen.Emit(OpCodes.Beq, label);
        }

        public void BranchLess(Label label, bool isShort)
        {
            if (isShort)
                _ilGen.Emit(OpCodes.Blt_S, label);
            else
                _ilGen.Emit(OpCodes.Blt, label);
        }

        public void BranchLessEqual(Label label, bool isShort)
        {
            if (isShort)
                _ilGen.Emit(OpCodes.Ble_S, label);
            else
                _ilGen.Emit(OpCodes.Ble, label);
        }

        public void BranchGreater(Label label, bool isShort)
        {
            if (isShort)
                _ilGen.Emit(OpCodes.Bgt_S, label);
            else
                _ilGen.Emit(OpCodes.Bgt, label);
        }

        public void BranchGreaterEqual(Label label, bool isShort)
        {
            if (isShort)
                _ilGen.Emit(OpCodes.Bge_S, label);
            else
                _ilGen.Emit(OpCodes.Bge, label);
        }

        public void BranchIfTrue(Label label, bool isShort)
        {
            if (isShort)
                _ilGen.Emit(OpCodes.Brtrue_S, label);
            else
                _ilGen.Emit(OpCodes.Brtrue, label);
        }

        // synonym for clearer code
        public void BranchIfNotNull(Label label, bool isShort)
        {
            BranchIfTrue(label, isShort);
        }

        // synonym for clearer code
        public void BranchIfNonZero(Label label, bool isShort)
        {
            BranchIfTrue(label, isShort);
        }

        public void BranchIfFalse(Label label, bool isShort)
        {
            if (isShort)
                _ilGen.Emit(OpCodes.Brfalse_S, label);
            else
                _ilGen.Emit(OpCodes.Brfalse, label);
        }

        // synonym for clearer code
        public void BranchIfNull(Label label, bool isShort)
        {
            BranchIfFalse(label, isShort);
        }

        // synonym for clearer code
        public void BranchIfZero(Label label, bool isShort)
        {
            BranchIfFalse(label, isShort);
        }

        public void Negate()
        {
            _ilGen.Emit(OpCodes.Neg);
        }

        public void Get(bool targetIsValueType, int argumentIndex, SortProperty property)
        {
            LoadArgument(targetIsValueType, argumentIndex);

            if (property.Get != null)
                Call(property.Get); // Get property value.
            else
                _ilGen.Emit(OpCodes.Ldfld, property.Field);  // Get field value
        }
    }
    #endregion Class: DynamicEmit

    #region Class: DynamicComparer
    public sealed class DynamicComparer<T> : System.Collections.Generic.IComparer<T>
    {
        private DynamicMethod method;
        private Comparison<T> comparer;

        public Comparison<T> Comparer
        {
            get
            {
                return comparer;
            }
        }

        public DynamicComparer(string orderBy)
        {
            Initialize(orderBy);
        }

        public DynamicComparer(SortProperty[] sortProperties)
        {
            Initialize(sortProperties);
        }

        public void Initialize(string orderBy)
        {
            Initialize(SortProperty.ParseOrderBy(orderBy));
        }

        public void Initialize(SortProperty[] sortProperties)
        {
            SortProperty.BindSortProperties(sortProperties, typeof(T));
            method = CreateDynamicCompareMethod(sortProperties);
            comparer = (Comparison<T>)method.CreateDelegate(typeof(Comparison<T>));
        }

        public int Compare(T x, T y)
        {
            return comparer.Invoke(x, y);
        }

        private DynamicMethod CreateDynamicCompareMethod(SortProperty[] sortProperties)
        {
            // at this time, the inner loop is (worst case) 39 IL bytes per property with short branches
            const int BytesPerProperty = 39;
            const int PropertiesPerShortBranch = 128 / BytesPerProperty;

            DynamicMethod dm = new DynamicMethod("DynamicCompare"
                , MethodAttributes.Static | MethodAttributes.Public, CallingConventions.Standard,
                typeof(int), new Type[] { typeof(T), typeof(T) }, typeof(T), false);
            dm.InitLocals = false;
            DynamicEmit de = new DynamicEmit(dm);

            #region Generate IL for dynamic method
            Dictionary<Type, LocalBuilder> localVariables = new Dictionary<Type, LocalBuilder>();
            bool isValueType = typeof(T).IsValueType;

            if (sortProperties.Length > 0)
            {
                Label breakLabel = de.DefineLabel();

                // For each of the properties we want to check inject the following.
                int numberLeft = sortProperties.Length;
                foreach (SortProperty property in sortProperties)
                {
                    Label continueLabel = de.DefineLabel();
                    Type propertyType = property.ValueType;

                    // Load argument at position 0.
                    de.Get(isValueType, 0, property);

                    // If the type is an Enum, then we need to box it...
                    if (propertyType.IsEnum)
                    {
                        de.BoxIfNeeded(propertyType);
                    }
                    else if (propertyType.IsValueType)
                    {
                        if (!property.IsNullable)
                        {
                            // If the type is a ValueType then we need to inject code to store
                            // it in a local variable, this insures it doesn't get boxed.
                            // Do we have a local variable for this type?
                            LocalBuilder localBuilder;

                            if (!localVariables.TryGetValue(propertyType, out localBuilder))
                            {
                                // Adds a local variable of type x and remember it
                                localBuilder = de.DeclareLocal(propertyType);
                                localVariables.Add(propertyType, localBuilder);
                            }

                            // This local variable is for handling value types of type x.
                            int localIndex = localBuilder.LocalIndex;

                            de.StoreLocal(localIndex);       // Store the value in the local var at position x.
                            de.LoadLocalAddress(localIndex); // Load the address of the local
                        }
                    }
                    else
                    {
                        // value is an reference type
                        Label leftNotNull = de.DefineLabel();
                        Label rightNotNull = de.DefineLabel();
                        de.Duplicate(); // left is now on stack twice.
                        de.BranchIfNotNull(leftNotNull, true);

                        // Left is null
                        de.Pop(); // discard second copy of left

                        // Get right...
                        de.Get(isValueType, 1, property);

                        // and check if right is not null
                        de.BranchIfNotNull(rightNotNull, true);

                        // We know that right is null too, thus they are equal
                        de.LoadLiteral(0);
                        de.Branch(continueLabel, numberLeft <= PropertiesPerShortBranch);

                        // Okay, right is NOT null, left is less
                        de.MarkLabel(rightNotNull);
                        de.LoadLiteral(-1);
                        de.Branch(continueLabel, numberLeft <= PropertiesPerShortBranch);

                        de.MarkLabel(leftNotNull);
                    }

                    // Load argument at position 1.
                    de.Get(isValueType, 1, property);

                    // If the type is an Enum, then we need to box it...
                    if (propertyType.IsEnum)
                    {
                        de.BoxIfNeeded(propertyType);
                    }

                    // Compare the top 2 items in the evaluation stack and push the return value onto the stack.
                    if (property.IsNullable)
                    {
                        // use Nullable's Compare method
                        MethodInfo elementCompare = typeof(Nullable).GetMethod("Compare");
                        elementCompare = elementCompare.MakeGenericMethod(propertyType.GetGenericArguments()[0]);
                        de.Call(elementCompare);
                    }
                    else
                    {
                        //Comparer<T>.Default;
                        // use propertyType's CompareTo method
                        MethodInfo elementCompare = propertyType.GetMethod("CompareTo", new Type[] { propertyType });
                        de.Call(elementCompare);
                    }

                    de.MarkLabel(continueLabel);

                    // If the sort should be descending we need to flip the result of the comparison.
                    if (property.Descending)
                        de.Negate();

                    if (--numberLeft > 0)
                    {
                        de.Duplicate();     // save a copy of the return value
                        // Is the result is not zero, we're done so break out of the loop.
                        de.BranchIfNonZero(breakLabel, numberLeft <= PropertiesPerShortBranch);
                        de.Pop();           // discard the (known 0) copy of the return value
                    }
                }

                de.MarkLabel(breakLabel); // This is the spot where the label we created earlier should be added.
            }
            else
            {
                // if there are no properties, call object comparer directly...
                de.LoadArgument(isValueType, 0);    // Load argument at position 0.
                de.LoadArgument(1);                 // Load argument at position 1.
                MethodInfo instanceCompare = typeof(T).GetMethod("CompareTo", new Type[] { typeof(T) });
                de.Call(instanceCompare);
            }

            de.Return(); // Return the value.
            #endregion

            return dm;
        }
    }
    #endregion Class: DynamicComparer

    #region Struct: SortProperty
    /// <summary>
    /// Internal struct to carry the sorting properties.
    /// </summary>
    public struct SortProperty
    {
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentException("A property cannot have an empty name.", "value");

                name = value.Trim();
            }
        }

        private bool descending;
        public bool Descending
        {
            get
            {
                return descending;
            }
            set
            {
                descending = value;
            }
        }

        public static bool IsComparable(Type valueType)
        {
            bool isNullable;
            return IsComparable(valueType, out isNullable);
        }

        public static bool IsComparable(Type valueType, out bool isNullable)
        {
            isNullable = valueType.IsGenericType
                    && !valueType.IsGenericTypeDefinition
                    && valueType.IsAssignableFrom(typeof(Nullable<>).MakeGenericType(valueType.GetGenericArguments()[0]));

            return (typeof(IComparable).IsAssignableFrom(valueType)
                    || typeof(IComparable<>).MakeGenericType(valueType).IsAssignableFrom(valueType)
                    || isNullable);
        }

        private Type valueType;
        internal Type ValueType
        {
            get
            {
                return valueType;
            }
            private set
            {
                valueType = value;

                if (!IsComparable(value, out isNullable))
                {
                    throw new NotSupportedException("The type \""
                        + value.FullName
                        + "\" of the property \""
                        + this.Name
                        + "\" does not implement IComparable, IComparible<T> or is Nullable<T>.");
                }
            }
        }

        private MethodInfo get;
        internal MethodInfo Get
        {
            get
            {
                return get;
            }
            private set
            {
                get = value;
            }
        }

        private FieldInfo field;
        internal FieldInfo Field
        {
            get
            {
                return field;
            }
            private set
            {
                field = value;
            }
        }

        private bool isNullable;
        internal bool IsNullable
        {
            get
            {
                return isNullable;
            }
        }

        public static SortProperty[] ParseOrderBy(string orderBy)
        {
            if (orderBy == null)
                throw new ArgumentException("The orderBy clause may not be null.", "orderBy");

            string[] properties = orderBy.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            SortProperty[] sortProperties = new SortProperty[properties.Length];

            for (int i = 0; i < properties.Length; i++)
            {
                bool descending = false;
                string property = properties[i].Trim();
                string[] propertyElements = property.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (propertyElements.Length > 1)
                {
                    if (propertyElements[1].Equals("DESC", StringComparison.OrdinalIgnoreCase))
                    {
                        descending = true;
                    }
                    else if (propertyElements[1].Equals("ASC", StringComparison.OrdinalIgnoreCase))
                    {
                        // already set to descending = false;
                    }
                    else
                    {
                        throw new ArgumentException("Unexpected sort order type \"" + propertyElements[1] + "\" for \"" + propertyElements[0] + "\"", "orderBy");
                    }
                }

                sortProperties[i] = new SortProperty(propertyElements[0], descending);
            }

            return sortProperties;
        }

        public SortProperty(string propertyName, bool sortDescending)
        {
            if (String.IsNullOrEmpty(propertyName))
                throw new ArgumentException("A property cannot have an empty name.", "propertyName");

            name = propertyName;
            descending = sortDescending;

            // we set these when accessor validated
            valueType = null;
            get = null;
            field = null;
            isNullable = false;
        }

        internal static void BindSortProperties(SortProperty[] sortProperties, Type instanceType)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            if (sortProperties == null)
                sortProperties = new SortProperty[0];

            if (sortProperties.Length > 0)
            {
                for (int index = 0; index < sortProperties.Length; index++)
                {
                    string propertyName = sortProperties[index].Name;
                    PropertyInfo propertyInfo = instanceType.GetProperty(propertyName, BindingFlags.GetProperty | flags);

                    if (propertyInfo != null)
                    {
                        sortProperties[index].ValueType = propertyInfo.PropertyType;
                        sortProperties[index].Get = propertyInfo.GetGetMethod(true);
                    }
                    else
                    {
                        FieldInfo fieldInfo = instanceType.GetField(propertyName, BindingFlags.GetField | flags);

                        if (fieldInfo != null)
                        {
                            sortProperties[index].ValueType = fieldInfo.FieldType;
                            sortProperties[index].Field = fieldInfo;
                        }
                        else
                        {
                            throw new ArgumentException("No public property or field named \""
                                + propertyName
                                + "\" was found in type \""
                                + instanceType.FullName
                                + "\".");
                        }
                    }
                }
            }
            else
            {
                if (!IsComparable(instanceType))
                    throw new NotSupportedException("The type \""
                        + instanceType.FullName
                        + "\" does not implement IComparable, IComparable<T> nor is a Nullable<T>.");
            }
        }
    }
    #endregion Struct: SortProperty

	#region Custom event handler for populate/update/delete
	/// <summary>
	/// Custom event handler for populate/update/delete
	/// </summary>
	[Serializable]
	public class PopulateUpdateDelete_EventArgs : EventArgs
	{
		private string _name = string.Empty;
		private string _description = string.Empty;
		private string _errors = string.Empty;
		private int _totalCount = 0;
		private int _currentIndex = 0;
		
		public PopulateUpdateDelete_EventArgs(string name, string description, string errors, int totalCount, int currentIndex)
		{
			_name = name;
			_description = description;
			_errors = errors;
			_totalCount = totalCount;
			_currentIndex = currentIndex;
		}
		
		public PopulateUpdateDelete_EventArgs()
		{
		}
		
		public string Name
		{
			get { return _name; }
		}
		
		public string Description
		{
			get { return _description; }
		}
		
		public string Errors
		{
			get { return _errors; }
		}
		
		public int TotalCount
		{
			get { return _totalCount; }
		}
		
		public int CurrentIndex
		{
			get { return _currentIndex; }
		}
	}
	#endregion Custom event handler for populate/update/delete

	#region Custom EventHandler for PropertyChanged/PropertyChanging
	public delegate void ClassGenPropertyChangingEventHandler(object sender, ClassGenPropertyChangingEventArgs e);
	public class ClassGenPropertyChangingEventArgs
	{
		private string _propName = string.Empty;
		private bool _cancel = false;
		private object _oldValue = null;
		private object _newValue = null;

		public ClassGenPropertyChangingEventArgs(string propName, bool cancel, object oldValue, object newValue)
		{
			_propName = propName;
			_cancel = cancel;
			_oldValue = oldValue;
			_newValue = newValue;
		}

		public ClassGenPropertyChangingEventArgs(string propName, object oldValue, object newValue)
		{
			_propName = propName;
			_oldValue = oldValue;
			_newValue = newValue;
		}

		public ClassGenPropertyChangingEventArgs(string propName)
		{
			_propName = propName;
		}

		public ClassGenPropertyChangingEventArgs(string propName, bool cancel)
		{
			_propName = propName;
			_cancel = cancel;
		}

		public string PropertyName
		{
			get { return _propName; }
		}

		public bool Cancel
		{
			get { return _cancel; }
			set { _cancel = value; }
		}

		public object OldValue
		{
			get { return _oldValue; }
		}

		public object NewValue
		{
			get { return _newValue; }
		}
	}

	public interface IClassGenPropertyChanging
	{
		event ClassGenPropertyChangingEventHandler ClassGenPropertyChanging;
	}

	public delegate void ClassGenPropertyChangedEventHandler(object sender, ClassGenPropertyChangedEventArgs e);
	public class ClassGenPropertyChangedEventArgs
	{
		private string _propName = string.Empty;
		private object _newValue = null;

		public ClassGenPropertyChangedEventArgs(string propName, object newValue)
		{
			_propName = propName;
			_newValue = newValue;
		}

		public ClassGenPropertyChangedEventArgs(string propName)
		{
			_propName = propName;
		}

		public string PropertyName
		{
			get { return _propName; }
		}

		public object NewValue
		{
			get { return _newValue; }
		}
	}

	public interface IClassGenPropertyChanged
	{
		event ClassGenPropertyChangedEventHandler ClassGenPropertyChanged;
	}
	#endregion Custom EventHandler for PropertyChanged/PropertyChanging

	#region Sort Comparer
	public class SortComparer<T> : IComparer<T>
	{
		private ListSortDescriptionCollection m_SortCollection = null;
		private PropertyDescriptor m_PropDesc = null;
		private ListSortDirection m_Direction = ListSortDirection.Ascending;

		public SortComparer(PropertyDescriptor propDesc, ListSortDirection direction)
		{
			m_PropDesc = propDesc;
			m_Direction = direction;
		}

		public SortComparer(ListSortDescriptionCollection sortCollection)
		{
			m_SortCollection = sortCollection;
		}

		#region IComparer<T> Members

		int IComparer<T>.Compare(T x, T y)
		{
			if (m_PropDesc != null) // Simple sort 
			{
				object xValue = m_PropDesc.GetValue(x);
				object yValue = m_PropDesc.GetValue(y);
				return CompareValues(xValue, yValue, m_Direction);
			}
			else if (m_SortCollection != null && m_SortCollection.Count > 0)
			{
				return RecursiveCompareInternal(x, y, 0);
			}
			else return 0;
		}
		#endregion

		private int CompareValues(object xValue, object yValue, ListSortDirection direction)
		{

			int retValue = 0;
			if (xValue is IComparable) // Can ask the x value
			{
				retValue = ((IComparable)xValue).CompareTo(yValue);
			}
			else if (yValue is IComparable) //Can ask the y value
			{
				retValue = ((IComparable)yValue).CompareTo(xValue);
			}
			else if (!xValue.Equals(yValue)) // not comparable, compare String representations
			{
				retValue = xValue.ToString().CompareTo(yValue.ToString());
			}
			if (direction == ListSortDirection.Ascending)
			{
				return retValue;
			}
			else
			{
				return retValue * -1;
			}
		}

		private int RecursiveCompareInternal(T x, T y, int index)
		{
			if (index >= m_SortCollection.Count)
				return 0; // termination condition

			ListSortDescription listSortDesc = m_SortCollection[index];
			object xValue = listSortDesc.PropertyDescriptor.GetValue(x);
			object yValue = listSortDesc.PropertyDescriptor.GetValue(y);

			int retValue = CompareValues(xValue, yValue, listSortDesc.SortDirection);
			if (retValue == 0)
			{
				return RecursiveCompareInternal(x, y, ++index);
			}
			else
			{
				return retValue;
			}
		}
	}
	#endregion Sort Comparer
}
