using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

using DevExpress.XtraEditors.DXErrorProvider;

using LEADBase;

namespace LEAD
{
	[Serializable]
    public class LEAD_BaseObject : IDisposable, 
		IClassGenCloneable, IClassGenPropertyChanged, INotifyPropertyChanged, IDXDataErrorInfo
    {
        #region Private Properties
        protected internal RecordStatus _recordStatus = RecordStatus.New;
        protected internal List<string> _changedProps = new List<string>();
        protected internal DateTime? _dateTimeObjectPopulated = null;
        protected internal bool _isLoaded = false;		// For use with parent and child collections
		protected internal RefreshFrequency _refreshRate = RefreshFrequency.ByDeveloperOnly;
		protected internal bool _checkedInGrid = false;
		protected internal bool _isDisposable = true;

		// Custom Fields
		protected internal string _gridCustom_0 = string.Empty;
		protected internal string _gridCustom_1 = string.Empty;
		protected internal string _gridCustom_2 = string.Empty;
		protected internal string _gridCustom_3 = string.Empty;
		protected internal string _gridCustom_4 = string.Empty;
		protected internal string _gridCustom_5 = string.Empty;
		protected internal string _gridCustom_6 = string.Empty;
		protected internal string _gridCustom_7 = string.Empty;
		protected internal string _gridCustom_8 = string.Empty;
		protected internal string _gridCustom_9 = string.Empty;

		private bool _disposed = false;
		private string _objectDisposedMessage = "The object has already been disposed.";
        #endregion Private Properties

		#region Public Static Properties

		//public static Dictionary<string, List<PropertyInfo>> PropInfoArray = new Dictionary<string, List<PropertyInfo>>();
		
		public static readonly string FN_CheckedInGrid = "CheckedInGrid";

		public static readonly string FN_GridCustom_0 = "GridCustom_0";
		public static readonly string FN_GridCustom_1 = "GridCustom_1";
		public static readonly string FN_GridCustom_2 = "GridCustom_2";
		public static readonly string FN_GridCustom_3 = "GridCustom_3";
		public static readonly string FN_GridCustom_4 = "GridCustom_4";

		public static readonly string FN_GridCustom_5 = "GridCustom_5";
		public static readonly string FN_GridCustom_6 = "GridCustom_6";
		public static readonly string FN_GridCustom_7 = "GridCustom_7";
		public static readonly string FN_GridCustom_8 = "GridCustom_8";
		public static readonly string FN_GridCustom_9 = "GridCustom_9";

		public static readonly int SplitOnXMLCharactersForLogging = 6000;
		public static readonly string SplitOnXMLHeader = @"<?xml version=""1.0"" standalone=""yes""?>";

		#endregion Public Static Properties

		public LEAD_BaseObject()
		{
			_changedProps = new List<string>();

			_rules = new ClassGenRules(this);		// Instantiate the collection
		}

        /// <summary>
		/// Tells if the object is valid or not based on the errors
		/// </summary>
		[Newtonsoft.Json.JsonIgnore]
		public bool IsValid
		{
			get 
			{ 
				if (this._disposed) { throw new ObjectDisposedException("LEAD_BaseObject", _objectDisposedMessage); }
				return this.BrokenRules.Count == 0; 
			}
		}

		#region Record Status
		public static Dictionary<Type, List<PropertyInfo>> RecordStatusParentHolders = new Dictionary<Type, List<PropertyInfo>>();

		/// <summary>
		/// The record status currently set on the object
		/// </summary>
        public RecordStatus RecordStatus
        {
            get 
			{ 
				if (this._disposed) { throw new ObjectDisposedException("LEAD_BaseObject", _objectDisposedMessage); }
				return _recordStatus; 
			}
            set 
			{ 
				if (this._disposed) { throw new ObjectDisposedException("LEAD_BaseObject", _objectDisposedMessage); }
				_recordStatus = value; 

				// See if there are parent properties
				if (value != RecordStatus.Current)
				{
					List<PropertyInfo> props = null;
					if (!RecordStatusParentHolders.TryGetValue(this.GetType(), out props))
					{
						props = new List<PropertyInfo>();
						foreach (PropertyInfo prop in this.GetType().GetProperties())
						{
							if (prop.Name.EndsWith("Item") &&
								prop.PropertyType.FullName.Contains("LEAD.") &&
								prop.PropertyType != this.GetType())
							{
								props.Add(prop);
							}
						}
						if (RecordStatusParentHolders.ContainsKey(this.GetType())) { RecordStatusParentHolders.Remove(this.GetType()); }
						RecordStatusParentHolders.Add(this.GetType(), props);
					}
					foreach (PropertyInfo prop in props)
					{
						if (prop.GetValue(this, null) != null &&
							prop is LEAD_BaseObject &&
							(LEAD_BaseObject)prop.GetValue(this, null) != null)
						{
							switch (((LEAD_BaseObject)prop.GetValue(this, null)).RecordStatus)
							{
								case RecordStatus.Current:
									((LEAD_BaseObject)prop.GetValue(this, null)).RecordStatus = RecordStatus.Modified;
									break;
								default:
									((LEAD_BaseObject)prop.GetValue(this, null)).RecordStatus =
										((LEAD_BaseObject)prop.GetValue(this, null)).RecordStatus;
									break;
							}
						}
					}
				}
			}
        }
		#endregion Record Status

		/// <summary>
		/// The list of the changed props on the object
		/// </summary>
        [Newtonsoft.Json.JsonIgnore]
		public List<string> ChangedProps
        {
            get 
			{ 
				if (this._disposed) { throw new ObjectDisposedException("LEAD_BaseObject", _objectDisposedMessage); }
				return _changedProps; 
			}
            set 
			{ 
				if (this._disposed) { throw new ObjectDisposedException("LEAD_BaseObject", _objectDisposedMessage); }
				_changedProps = value; 
			}
        }

		/// <summary>
		/// The date/time the object was populated
		/// </summary>
        [Newtonsoft.Json.JsonIgnore]
		public DateTime? DateTimeObjectPopulated
        {
            get 
			{ 
				if (this._disposed) { throw new ObjectDisposedException("LEAD_BaseObject", _objectDisposedMessage); }
				return _dateTimeObjectPopulated; 
			}
            set 
			{ 
				if (this._disposed) { throw new ObjectDisposedException("LEAD_BaseObject", _objectDisposedMessage); }
				_dateTimeObjectPopulated = value; 
			}
        }

		/// <summary>
		/// Tells whether the object is already loaded
		/// </summary>
        [Newtonsoft.Json.JsonIgnore]
		public bool IsLoaded
        {
            get 
			{ 
				if (this._disposed) { throw new ObjectDisposedException("LEAD_BaseObject", _objectDisposedMessage); }
				return _isLoaded; 
			}
            set 
			{ 
				if (this._disposed) { throw new ObjectDisposedException("LEAD_BaseObject", _objectDisposedMessage); }
				_isLoaded = value; 
			}
        }

		/// <summary>
		/// Sets the Refresh rate on the object and/or collection
		/// </summary>
		[Newtonsoft.Json.JsonIgnore]
		public RefreshFrequency RefreshRate
		{
			get 
			{ 
				if (this._disposed) { throw new ObjectDisposedException("LEAD_BaseObject", _objectDisposedMessage); }
				return _refreshRate; 
			}
			set 
			{ 
				if (this._disposed) { throw new ObjectDisposedException("LEAD_BaseObject", _objectDisposedMessage); }
				_refreshRate = value; 
			}
		}

        /// <summary>
		/// Tells if the record is checked in a grid
		/// </summary>
		[Newtonsoft.Json.JsonIgnore]
		public bool CheckedInGrid
		{
			get 
			{ 
				if (this._disposed) { throw new ObjectDisposedException("LEAD_BaseObject", _objectDisposedMessage); }
				return _checkedInGrid; 
			}
			set 
			{ 
				if (this._disposed) { throw new ObjectDisposedException("LEAD_BaseObject", _objectDisposedMessage); }
				_checkedInGrid = value; 
			}
		}

		/// <summary>
		/// Tells if the object can be disposed of
		/// </summary>
		[Newtonsoft.Json.JsonIgnore]
		public bool IsDisposable
		{
			get { return _isDisposable; }  
			set { _isDisposable = value; }  
		}

		/// <summary>
		/// Tells if the object is current
		/// </summary>
		public bool IsCurrent
		{
			get { return _recordStatus == RecordStatus.Current; }  
		}

		// Custom Fields
		[Newtonsoft.Json.JsonIgnore]
		public string GridCustom_0
		{
			get { return _gridCustom_0; }
			set
			{
				if (_gridCustom_0 != value)
				{
					_gridCustom_0 = value;
					NotifyPropertyChanged("GridCustom_0", value);
				}
			}
		}

		[Newtonsoft.Json.JsonIgnore]
		public string GridCustom_1
		{
			get { return _gridCustom_1; }
			set
			{
				if (_gridCustom_1 != value)
				{
					_gridCustom_1 = value;
					NotifyPropertyChanged("GridCustom_1", value);
				}
			}
		}

		[Newtonsoft.Json.JsonIgnore]
		public string GridCustom_2
		{
			get { return _gridCustom_2; }
			set
			{
				if (_gridCustom_2 != value)
				{
					_gridCustom_2 = value;
					NotifyPropertyChanged("GridCustom_2", value);
				}
			}
		}

		[Newtonsoft.Json.JsonIgnore]
		public string GridCustom_3
		{
			get { return _gridCustom_3; }
			set
			{
				if (_gridCustom_3 != value)
				{
					_gridCustom_3 = value;
					NotifyPropertyChanged("GridCustom_3", value);
				}
			}
		}

		[Newtonsoft.Json.JsonIgnore]
		public string GridCustom_4
		{
			get { return _gridCustom_4; }
			set
			{
				if (_gridCustom_4 != value)
				{
					_gridCustom_4 = value;
					NotifyPropertyChanged("GridCustom_4", value);
				}
			}
		}

		[Newtonsoft.Json.JsonIgnore]
		public string GridCustom_5
		{
			get { return _gridCustom_5; }
			set
			{
				if (_gridCustom_5 != value)
				{
					_gridCustom_5 = value;
					NotifyPropertyChanged("GridCustom_5", value);
				}
			}
		}

		[Newtonsoft.Json.JsonIgnore]
		public string GridCustom_6
		{
			get { return _gridCustom_6; }
			set
			{
				if (_gridCustom_6 != value)
				{
					_gridCustom_6 = value;
					NotifyPropertyChanged("GridCustom_6", value);
				}
			}
		}

		[Newtonsoft.Json.JsonIgnore]
		public string GridCustom_7
		{
			get { return _gridCustom_7; }
			set
			{
				if (_gridCustom_7 != value)
				{
					_gridCustom_7 = value;
					NotifyPropertyChanged("GridCustom_7", value);
				}
			}
		}

		[Newtonsoft.Json.JsonIgnore]
		public string GridCustom_8
		{
			get { return _gridCustom_8; }
			set
			{
				if (_gridCustom_8 != value)
				{
					_gridCustom_8 = value;
					NotifyPropertyChanged("GridCustom_8", value);
				}
			}
		}

		[Newtonsoft.Json.JsonIgnore]
		public string GridCustom_9
		{
			get { return _gridCustom_9; }
			set
			{
				if (_gridCustom_9 != value)
				{
					_gridCustom_9 = value;
					NotifyPropertyChanged("GridCustom_9", value);
				}
			}
		}

		/// <summary>
		/// Set the modified status on the current object
		/// </summary>
		public void SetModifiedStatus(string changedPropName)
		{
			//if (this._disposed) { throw new ObjectDisposedException("LEAD_BaseObject", _objectDisposedMessage); }
			if (_recordStatus != RecordStatus.New && _recordStatus != RecordStatus.Deleted)
			{
				if (!_changedProps.Contains(changedPropName)) { _changedProps.Add(changedPropName); }
				this.RecordStatus = RecordStatus.Modified;
			}
		}

		#region IClassGenPropertyChanged Implementation
		public event PropertyChangedEventHandler PropertyChanged;
		public event ClassGenPropertyChangedEventHandler ClassGenPropertyChanged;
		public void NotifyPropertyChanged(string info, object newValue)
		{
			if (ClassGenPropertyChanged != null)
			{
				ClassGenPropertyChanged(this, new ClassGenPropertyChangedEventArgs(info, newValue));
			}
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}
		#endregion IClassGenPropertyChanged Implementation

		#region IDisposable Implementation
		/// <summary>
		/// Implement IDisposable.
		/// Do not make this method virtual.
		/// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
			if (!this._isDisposable) { return; }

			Dispose(true);

			// Take yourself off the Finalization queue 
			// to prevent finalization code for this object
			// from executing a second time.
			GC.SuppressFinalize(this);
        }

		/// <summary>
		/// Dispose(bool disposing) executes in two distinct scenarios.
		/// If disposing equals true, the method has been called directly
		/// or indirectly by a user's code. Managed and unmanaged resources
		/// can be disposed.
		/// If disposing equals false, the method has been called by the 
		/// runtime from inside the finalizer and you should not reference 
		/// other objects. Only unmanaged resources can be disposed.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!this._isDisposable) { return; }

			// Check to see if Dispose has already been called.
			if (!this._disposed)
			{
				// Release unmanaged resources. If disposing is false, 
				// only the following code is executed.
				// <Enter Disposal here> 
				// Note that this is not thread safe.
				// Another thread could start disposing the object
				// after the managed resources are disposed,
				// but before the disposed flag is set to true.
				// If thread safety is necessary, it must be
				// implemented by the client.
				_disposed = true;

				// If disposing equals true, dispose all managed 
				// and unmanaged resources.
				if (disposing)
				{
					// Dispose managed resources.
					_changedProps = new List<string>();

					if (_rules != null) { _rules.Clear(); _rules = null; }
				}
			}
		}

		/// <summary>
		/// Use C# destructor syntax for finalization code.
		/// This destructor will run only if the Dispose method 
		/// does not get called.
		/// It gives your base class the opportunity to finalize.
		/// Do not provide destructors in types derived from this class.
		/// </summary>
		~LEAD_BaseObject()
		{
			// Do not re-create Dispose clean-up code here.
			// Calling Dispose(false) is optimal in terms of
			// readability and maintainability.
			Dispose(false);
		}
		#endregion IDisposable Implementation

		#region IDXDataErrorInfo Implementation
		public virtual void GetPropertyError(string propertyName, ErrorInfo info)
		{
			ClassGenExceptionCollection brokenRules = Rules.BrokenRules;
			info.ErrorText = string.Empty;
			foreach (ClassGenException ex in brokenRules)
			{
				if (ex.PropertyName == propertyName)
				{
					info.ErrorText +=
						(!String.IsNullOrEmpty(info.ErrorText) ? Environment.NewLine : "") +
						ex.Description;
					info.ErrorType = (ErrorType)Enum.Parse(typeof(ErrorType), ex.ClassGenExceptionIconType.ToString(), true);
				}
			}
		}
		
		public virtual void GetError(ErrorInfo info)
		{
			ClassGenExceptionCollection brokenRules = Rules.BrokenRules;
			info.ErrorText = string.Empty;
			foreach (ClassGenException ex in brokenRules)
			{
				info.ErrorText +=
					(!String.IsNullOrEmpty(info.ErrorText) ? Environment.NewLine : "") +
					ex.Description;
				info.ErrorType = (ErrorType)Enum.Parse(typeof(ErrorType), ex.ClassGenExceptionIconType.ToString(), true);
			}
		}
		#endregion IDXDataErrorInfo Implementation

		#region Static Handler for Math Functions
		/// <summary>
		/// Does a math function on the collection and field passed in
		/// </summary>
		/// <param name="collectionObject">The object to parse</param>
		/// <param name="propertyName">The property name to look at</param>
		/// <param name="type">The type of Math Function to do</param>
		/// <returns>A decimal value with the finished computation</returns>
		public static decimal Math<T>(List<T> collectionList, string propertyName, MathFunctionType type)
		{
			decimal rtv = 0;
			if (collectionList.Count == 0) { return rtv; }

			// Get the reflection information
			System.Reflection.PropertyInfo pi = collectionList[0].GetType().GetProperty(propertyName, 
				System.Reflection.BindingFlags.Instance |
				System.Reflection.BindingFlags.NonPublic |
				System.Reflection.BindingFlags.Public);

			switch (type)
			{
				case MathFunctionType.Count:
					rtv = collectionList.Count;
					break;
				case MathFunctionType.Average:
					foreach (object o in collectionList)
					{
						rtv += (pi.GetValue(o, null) != null ? decimal.Parse(pi.GetValue(o, null).ToString()) : 0);
					}
					rtv = rtv / collectionList.Count;
					break;
				case MathFunctionType.Sum:
					foreach (object o in collectionList)
					{
						rtv += (pi.GetValue(o, null) != null ? decimal.Parse(pi.GetValue(o, null).ToString()) : 0);
					}
					break;
				case MathFunctionType.Min:
					// Find the first value that's not null
					foreach (object o in collectionList)
					{
						if (pi.GetValue(o, null) != null)
						{
							rtv = Convert.ToDecimal(pi.GetValue(o, null));
							break;
						}
					}

					// Enum through the values to find the min
					foreach (object o in collectionList)
					{
						if (pi.GetValue(o, null) != null)
						{
							if (decimal.Parse(pi.GetValue(o, null).ToString()) < rtv)
							{ rtv = (pi.GetValue(o, null) != null ? decimal.Parse(pi.GetValue(o, null).ToString()) : 0); }
						}
					}
					break;
				case MathFunctionType.Max:
					// Find the first value that's not null
					foreach (object o in collectionList)
					{
						if (pi.GetValue(o, null) != null)
						{
							rtv = Convert.ToDecimal(pi.GetValue(o, null));
							break;
						}
					}
					
					// Enum through the values to find the max
					foreach (object o in collectionList)
					{
						if (pi.GetValue(o, null) != null)
						{
							if (decimal.Parse(pi.GetValue(o, null).ToString()) > rtv)
							{ rtv = (pi.GetValue(o, null) != null ? decimal.Parse(pi.GetValue(o, null).ToString()) : 0); }
						}
					}
					break;
			}

			return rtv;		// Return the value
		}

		public enum MathFunctionType : int
		{
			Sum = 0,
			Count,
			Average,
			Min,
			Max,
		}
		#endregion Static Handler for Math Functions

        #region Rules Implementation
        private ClassGenRules _rules = null;
        
		[Newtonsoft.Json.JsonIgnore]
		public ClassGenRules Rules
		{
			get 
			{ 
				if (this._disposed) { throw new ObjectDisposedException("LEAD_BaseObject", _objectDisposedMessage); }
				return _rules; 
			}
			set 
			{ 
				if (this._disposed) { throw new ObjectDisposedException("LEAD_BaseObject", _objectDisposedMessage); }
				_rules = value; 
			}
		}

		/// <summary>
		/// Retreive the broken rules collection
		/// </summary>
		[Newtonsoft.Json.JsonIgnore]
		public ClassGenExceptionCollection BrokenRules
		{
			get 
			{ 
				if (this._disposed) { throw new ObjectDisposedException("LEAD_BaseObject", _objectDisposedMessage); }
				return _rules.BrokenRules; 
			}
		}

		/// <summary>
		/// Use this method to set any additional rules
		/// </summary>
		protected virtual void SetAdditionalRules()
		{
		}
        #endregion Rules Implementation

		#region Class Methods
		/// <summary>
		/// Mark the item as deleted in the object, but don't touch it in the database
		/// </summary>
		public void Delete()
		{
			this.RecordStatus = RecordStatus.Deleted;
		}

		/// <summary>
		/// The default override for Undoing changes
		/// </summary>
		public virtual void UndoChanges()
		{
		}
		#endregion Class Methods

		#region IL Methods

		#region Delegate Handlers
		private delegate TResult CloneDelegate<T1, TResult>(T1 arg1);
		private delegate DataRow ToDataRow<T>(T arg1, DataTable dt);
		#endregion Delegate Handlers

		#region Static IL Holders
		public static Dictionary<Type, Delegate> CloneIL = new Dictionary<Type, Delegate>();
		public static Dictionary<Type, Delegate> ToDataRowIL = new Dictionary<Type, Delegate>();
		#endregion Static IL Holders

		#region Methods
		/// <summary>
		/// Convert the given object to a new data row in the format of the datatable
		/// </summary>
		/// <typeparam name="T">The type of object to convert</typeparam>
		/// <param name="myObject">The object to convert</param>
		/// <param name="dt">The data table object to use for the build</param>
		/// <returns>The populated datarow</returns>
		public DataRow ToDataRowWithIL<T>(T myObject, DataTable dt)
		{
			try
			{
				Delegate myExec = null;
				try
				{
					if (!ToDataRowIL.TryGetValue(typeof(T), out myExec))
					{
						// Create ILGenerator
						DynamicMethod dymMethod = new DynamicMethod("ToDataRow", typeof(System.Data.DataRow),
							new Type[2] { typeof(T), typeof(System.Data.DataTable) }, true);

						ILGenerator generator = dymMethod.GetILGenerator();

						LocalBuilder lbf = generator.DeclareLocal(typeof(System.Data.DataRow));
						Dictionary<Type, LocalBuilder> varList = new Dictionary<Type, LocalBuilder>();

						generator.Emit(OpCodes.Ldarg_1);
						generator.Emit(OpCodes.Callvirt, typeof(System.Data.DataTable).GetMethod("NewRow"));
						// callvirt   instance class [System.Data]System.Data.DataRow [System.Data]System.Data.DataTable::NewRow()
						generator.Emit(OpCodes.Stloc_0);

						PropertyInfo[] props = myObject.GetType().GetProperties(System.Reflection.BindingFlags.Instance |
							System.Reflection.BindingFlags.Public |
							System.Reflection.BindingFlags.NonPublic);

						foreach (PropertyInfo propInfo in props)
						{
							if (propInfo.CanRead)
							{
								generator.Emit(OpCodes.Ldloc_0);
								generator.Emit(OpCodes.Ldstr, propInfo.Name);
								generator.Emit(OpCodes.Ldarg_0);

								generator.Emit(OpCodes.Callvirt, myObject.GetType().GetMethod("get_" + propInfo.Name));
								// callvirt   instance string csCloneTest.KeyslotHistory::get_UserDef4()

								// Find out if we need to box this item
								// And handle the nulls
								System.Reflection.Emit.Label lblSetItem = generator.DefineLabel();
								System.Reflection.Emit.Label lblIsNotNull = generator.DefineLabel();
									
								if (propInfo.PropertyType != typeof(string))
								{
									if (propInfo.PropertyType.IsGenericType &&
										propInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
									{
										// Handle the dbnull.value type (if statement)
										// See if we need to declare a variable for this type or if it already exists
										if (!varList.ContainsKey(propInfo.PropertyType))
										{
											varList.Add(propInfo.PropertyType, generator.DeclareLocal(propInfo.PropertyType));
										}

										// Find out where to store the variable
										KeyValuePair<Type, LocalBuilder> kvpFound = new KeyValuePair<Type, LocalBuilder>();
										foreach (KeyValuePair<Type, LocalBuilder> kvp in varList)
										{
											if (kvp.Key == propInfo.PropertyType)
											{
												kvpFound = kvp;
												//generator.Emit(OpCodes.Stloc, kvp.Value);
												//generator.Emit(OpCodes.Ldloca_S, kvp.Value);
												break;
											}
										}

										generator.Emit(OpCodes.Stloc, kvpFound.Value);
										generator.Emit(OpCodes.Ldloca_S, kvpFound.Value);

										generator.Emit(OpCodes.Call, propInfo.PropertyType.GetMethod("get_HasValue"));
										generator.Emit(OpCodes.Brtrue_S, lblIsNotNull);
										generator.Emit(OpCodes.Ldsfld, typeof(System.DBNull).GetField("Value"));
										generator.Emit(OpCodes.Br_S, lblSetItem);

										generator.MarkLabel(lblIsNotNull);
										generator.Emit(OpCodes.Ldloca_S, kvpFound.Value);
										generator.Emit(OpCodes.Call, propInfo.PropertyType.GetMethod("GetValueOrDefault", new Type[] { }));
							
										//generator.Emit(OpCodes.Ldarg_0);
										//generator.Emit(OpCodes.Callvirt, myObject.GetType().GetMethod("get_" + propInfo.Name));
										//generator.Emit(OpCodes.Box, propInfo.PropertyType);
										generator.Emit(OpCodes.Box, System.Type.GetType(propInfo.PropertyType.GetGenericArguments()[0].FullName));
										//generator.MarkLabel(lblSetItem);
									}
									else
									{
										generator.Emit(OpCodes.Box, propInfo.PropertyType);
									}
								}
								else
								{
									// We have a string here - check for nulls
									generator.Emit(OpCodes.Dup);
									generator.Emit(OpCodes.Brtrue_S, lblSetItem);
									generator.Emit(OpCodes.Pop);
									generator.Emit(OpCodes.Ldsfld, typeof(System.DBNull).GetField("Value"));
									//generator.Emit(OpCodes.Br_S, lblSetItem);
									//generator.MarkLabel(lblIsNotNull);
									//generator.Emit(OpCodes.Ldarg_0);
									//generator.Emit(OpCodes.Callvirt, myObject.GetType().GetMethod("get_" + propInfo.Name));
								}

								generator.MarkLabel(lblSetItem);
								generator.Emit(OpCodes.Callvirt, typeof(System.Data.DataRow).GetMethod("set_Item", 
									new Type[2] { typeof(string), typeof(object) }));
								//  callvirt   instance void [System.Data]System.Data.DataRow::set_Item(string, object)
							}
						}

						// Load new constructed obj on eval stack -> 1 item on stack
						generator.Emit(OpCodes.Ldloc_0);
						// Return constructed object.   --> 0 items on stack
						generator.Emit(OpCodes.Ret);

						//myExec = dymMethod.CreateDelegate(typeof(Func<T, T>));
						myExec = dymMethod.CreateDelegate(typeof(ToDataRow<T>));
						ToDataRowIL.Add(typeof(T), myExec);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
				return ((ToDataRow<T>)myExec)(myObject, dt);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return null;
		}

		/// <summary>
		/// Generic cloning method that clones an object using IL.
		/// Only the first call of a certain type will hold back performance.
		/// After the first call, the compiled IL is executed.
		/// </summary>
		/// <typeparam name="T">Type of object to clone</typeparam>
		/// <param name="myObject">Object to clone</param>
		/// <returns>Cloned object</returns>
		public T CloneObjectWithIL<T>(T myObject)
		{
			Delegate myExec = null;
			if (!CloneIL.TryGetValue(typeof(T), out myExec))
			{
				// Create ILGenerator
				DynamicMethod dymMethod = new DynamicMethod("DoClone", typeof(T), new Type[] { typeof(T) }, true);
				ConstructorInfo cInfo = myObject.GetType().GetConstructor(new Type[] { });

				ILGenerator generator = dymMethod.GetILGenerator();

				LocalBuilder lbf = generator.DeclareLocal(typeof(T));

				generator.Emit(OpCodes.Newobj, cInfo);
				generator.Emit(OpCodes.Stloc_0);
				//foreach (FieldInfo field in myObject.GetType().GetFields(System.Reflection.BindingFlags.Instance | 
				//    System.Reflection.BindingFlags.Public | 
				//    System.Reflection.BindingFlags.NonPublic))
				foreach (PropertyInfo propInfo in myObject.GetType().GetProperties(System.Reflection.BindingFlags.Instance |
					System.Reflection.BindingFlags.Public |
					System.Reflection.BindingFlags.NonPublic))
				{
					if (propInfo.CanWrite &&
						(propInfo.PropertyType.IsValueType || 
						propInfo.PropertyType == typeof(string) || 
						propInfo.PropertyType == typeof(System.Byte[])))
					{
						// Load the new object on the eval stack... (currently 1 item on eval stack)
						generator.Emit(OpCodes.Ldloc_0);
						// Load initial object (parameter)          (currently 2 items on eval stack)
						generator.Emit(OpCodes.Ldarg_0);
						// Replace value by field value             (still currently 2 items on eval stack)
						//generator.Emit(OpCodes.Ldfld, field);
						//// Store the value of the top on the eval stack into the object underneath that value on the value stack.
						////  (0 items on eval stack)
						//generator.Emit(OpCodes.Stfld, field);

						generator.Emit(OpCodes.Callvirt, myObject.GetType().GetMethod("get_" + propInfo.Name));
						//callvirt   instance int32 csCloneTest.KeyslotHistory::get_KeyslotHistoryID()
						generator.Emit(OpCodes.Callvirt, myObject.GetType().GetMethod("set_" + propInfo.Name, 
							new Type[1] { propInfo.PropertyType }));
						//callvirt   instance void csCloneTest.KeyslotHistory::set_KeyslotHistoryID(int32)
					}
				}

				// Load new constructed obj on eval stack -> 1 item on stack
				generator.Emit(OpCodes.Ldloc_0);
				// Return constructed object.   --> 0 items on stack
				generator.Emit(OpCodes.Ret);

				myExec = dymMethod.CreateDelegate(typeof(CloneDelegate<T, T>));
				CloneIL.Add(typeof(T), myExec);
			}
			return ((CloneDelegate<T, T>)myExec)(myObject);
		}
		#endregion Methods

		#endregion IL Methods

		#region Static Methods
		///// <summary>
		///// Check the prop info array for the given type and get the params if you need to
		///// </summary>
		///// <param name="typeName">The type of object to get</param>
		//public static void CheckPropInfoArray(string typeName)
		//{
		//    if (!LEAD_BaseObject.PropInfoArray.ContainsKey(typeName))
		//    {
		//        // Load them up
		//        Type t = System.Type.GetType("LEAD." + typeName);
		//        PropertyInfo[] props = t.GetProperties(
		//            System.Reflection.BindingFlags.Instance |
		//            System.Reflection.BindingFlags.NonPublic |
		//            System.Reflection.BindingFlags.Public);
		//        List<PropertyInfo> newProps = new List<PropertyInfo>();

		//        //if (ignoreUnknownDBTypes)
		//        //{
		//        //    PropertyInfo pi = props[i];
		//        //    if (//!dt.Columns.Contains(pi.Name) &&
		//        //        pi.CanRead &&
		//        //        //!pi.PropertyType.IsGenericType &&
		//        //        //!pi.PropertyType.BaseType.IsGenericType &&
		//        //        (pi.PropertyType.ToString() != "System.Drawing.Image") &&
		//        //        (pi.PropertyType.IsPrimitive || pi.PropertyType.ToString().StartsWith("System.")))
		//        //    {
		//        //        newProps.Add(pi);
		//        //    }
		//        //}
		//        //else
		//        //{
		//            for (int i = props.Length - 1; i >= 0; i--)
		//            {
		//                newProps.Add(props[i]);
		//            }
		//        //}
		//        //((LEAD_BaseObject)o).Dispose();		// Dispose of the object
		//        LEAD_BaseObject.PropInfoArray.Add(typeName, newProps);		// Add the new collection
		//    }
		//}

		/// <summary>
		/// Delete records from the table based on a given criteria
		/// </summary>
		/// <param name="tableName">The name of the table to delete the record from</param>
		/// <param name="fieldName">The name of the field to key off for the delete - should be the primary key</param>
		/// <returns>The number of records affected by the delete</returns>
		public static int DeleteAll(string tableName, string fieldName, ref ClassGenExceptionCollection errors)
		{
			string sql = string.Empty;
			SqlCommand cmd = null;
			DataTable dt = new DataTable();
			int recsAffected = 0;
			SqlConnection oConn = null;
			SqlTransaction transaction = null;
			List<string> tables = new List<string>();
			int nestingLevel = 50;		// Set how deep the delete will go

			try
			{
				#region Get Values From Table - SQL
				sql = "" +
					//"-- DELETE Child Records based on foreign keys examples " + Environment.NewLine + 
					"SET NOCOUNT ON  " + Environment.NewLine +
					//"DECLARE @pGUID UNIQUEIDENTIFIER,  " + Environment.NewLine + 
					//"	@psTableName VARCHAR(200), " + Environment.NewLine + 
					//"	@psFieldName VARCHAR(200), " + Environment.NewLine + 
					//"	@piNestingLevel INT " + Environment.NewLine + 
					//"SELECT @pGUID = '24F4096B-DE6B-4146-808B-033B6D96D826',  " + Environment.NewLine + 
					//"	@psTableName = 'tKeyslotChangeDetail', " + Environment.NewLine + 
					//"	@psFieldName = 'sChangeDetailGUID', " + Environment.NewLine + 
					//"	@piNestingLevel = 25 " + Environment.NewLine + 
					" " + Environment.NewLine +

					"DECLARE @iLevel INT   SELECT @iLevel = 0 " + Environment.NewLine +
					"DECLARE @iRowCount INT		SELECT @iRowCount = 1 " + Environment.NewLine +
					"DECLARE @recID INT		SELECT @recID = 0 " + Environment.NewLine +
					" " + Environment.NewLine +

					//"-- Create a temp table to hold the foreign keys " + Environment.NewLine + 
					"IF object_id('tempdb..#tmpForeignKeys') IS NOT NULL DROP TABLE #tmpForeignKeys " + Environment.NewLine +
					"CREATE TABLE #tmpForeignKeys (TargetTable VARCHAR(200), " + Environment.NewLine +
					"	TargetColumn VARCHAR(200), " + Environment.NewLine +
					"	SourceTable VARCHAR(200), " + Environment.NewLine +
					"	SourceColumn VARCHAR(200), " + Environment.NewLine +
					"	iLevel INT DEFAULT(0), " + Environment.NewLine +
					"	bHasChildren BIT DEFAULT(0), " + Environment.NewLine +
					"	recID INT IDENTITY(1,1), " + Environment.NewLine +
					"	parentRecID INT) " + Environment.NewLine +
					" " + Environment.NewLine +

					//"-- Build the foreign keys" + Environment.NewLine + 
					"INSERT INTO #tmpForeignKeys (TargetTable, TargetColumn, SourceTable, SourceColumn, iLevel) " + Environment.NewLine +
					"SELECT @psTableName, @psFieldName, @psTableName, @psFieldName, 0 " + Environment.NewLine +
					"" + Environment.NewLine +

					"WHILE ((SELECT MAX(iLevel) FROM #tmpForeignKeys) < @piNestingLevel - 1 AND @recID < @piNestingLevel * 10) " + Environment.NewLine +
					"BEGIN " + Environment.NewLine +
					"	SELECT @recID = @recID + 1 " + Environment.NewLine +
					"" + Environment.NewLine +

					"	INSERT INTO #tmpForeignKeys (TargetTable, TargetColumn, SourceTable, SourceColumn, iLevel, parentRecID)" + Environment.NewLine +
					"	SELECT target_table.name AS target_table, target_column.name AS target_column,  " + Environment.NewLine +
					"		source_table.name AS source_table, source_column.name AS source_column, T.iLevel + 1, @recID" + Environment.NewLine +
					"	FROM sysforeignkeys fk " + Environment.NewLine +
					"	INNER JOIN sysobjects target_table ON fk.fkeyid = target_table.id  " + Environment.NewLine +
					"	INNER JOIN syscolumns target_column ON fk.fkeyid = target_column.id AND fk.fkey = target_column.colid  " + Environment.NewLine +
					"	INNER JOIN sysobjects source_table ON fk.rkeyid = source_table.id " + Environment.NewLine +
					"	INNER JOIN syscolumns source_column ON fk.rkeyid = source_column.id AND fk.rkey = source_column.colid " + Environment.NewLine +
					"	INNER JOIN #tmpForeignKeys T ON T.TargetTable = source_table.name" + Environment.NewLine +
					"	WHERE T.recID = @recID" + Environment.NewLine +
					"	ORDER BY target_table.name, target_column.name " + Environment.NewLine +
					"" + Environment.NewLine +

					"	SELECT @iRowCount = @@ROWCOUNT " + Environment.NewLine +
					"" + Environment.NewLine +

					"	IF @iRowCount > 0 UPDATE #tmpForeignKeys SET bHasChildren = 1 WHERE recID = @recID " + Environment.NewLine +
					"END " + Environment.NewLine +
					"" + Environment.NewLine +

					"SELECT * FROM #tmpForeignKeys ORDER BY recID ";
				#endregion Get Values From Table - SQL

				cmd = new SqlCommand(sql);
				cmd.Parameters.Add("@psTableName", SqlDbType.VarChar, 200).Value = tableName;
				cmd.Parameters.Add("@psFieldName", SqlDbType.VarChar, 200).Value = fieldName;
				cmd.Parameters.Add("@piNestingLevel", SqlDbType.Int).Value = nestingLevel;

				// Go out and get the records
				if (oConn != null && oConn.State == ConnectionState.Closed) { oConn = null; }
				bool connectionEstablished = (oConn != null);
				if (oConn == null) { oConn = DAL.OpenConnection(); }

				// Get the records
				dt = DAL.SQLExecDataTable(cmd, oConn);

				bool tableReferencesItself = false;
				string fieldToNull = string.Empty;
				foreach (DataRow row in dt.Rows)
				{
					if (row["TargetTable"] != DBNull.Value &&
						!tables.Contains(row["TargetTable"].ToString()))
					{
						tables.Add(row["TargetTable"].ToString());
					}
					if (!tableReferencesItself &&
						row["TargetTable"].ToString() == row["SourceTable"].ToString() &&
						row["TargetColumn"].ToString() != row["SourceColumn"].ToString())
					{
						tableReferencesItself = true;
						fieldToNull = row["TargetColumn"].ToString();
					}
				}

				
				// Start a new transaction
				transaction = oConn.BeginTransaction();

				// Find out if the table references itself
				// If it does, reset the target field before the delete
				if (tableReferencesItself)
				{
					cmd = new SqlCommand("UPDATE " + tables[0] + " SET " + fieldToNull + " = NULL");
					cmd.Transaction = transaction;
					recsAffected += DAL.SQLExecNonQuery(cmd, oConn);
				}
		
				// Roll through the SQL statements and fire them against the DB
				for (int i = tables.Count - 1; i >= 0; i--)
				{
					cmd = new SqlCommand("DELETE FROM " + tables[i]);
					cmd.Transaction = transaction;
					recsAffected += DAL.SQLExecNonQuery(cmd, oConn);
				}

				// Attempt to close the connection
				if (transaction != null)
				{
					try
					{
						transaction.Commit();		// Commit the transaction
						transaction.Dispose();		// Dispose of the transaction
						transaction = null;
					}
					catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
					catch (Exception ex) { errors.Add(new ClassGenException(ex)); }
				}
				if (!connectionEstablished) { DAL.CloseConnection(oConn); oConn = null; }		// Close the connection if we created it here
			}
			catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
			catch (Exception ex) { errors.Add(new ClassGenException(ex)); }
			finally
			{
				if (transaction != null)
				{
					try
					{
						transaction.Rollback();
					}
					catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
					catch (Exception ex) { errors.Add(new ClassGenException(ex)); }
				}
			}

			return recsAffected;			// Return the count of affected records
		}

		/// <summary>
		/// Immediately delete the element from the database 
		/// This method will automatically delete any children that exist for the object
		/// If you wish to have children remain, you must enumerate through the children
		/// and set their parent identifier to null in the database prior to running this 
		/// method
		/// </summary>
		/// <param name="tableName">The name of the table to delete the record from</param>
		/// <param name="fieldName">The name of the field to key off for the delete - should be the primary key</param>
		/// <param name="recGUID">The record identifier you wish to search for and delete</param>
		/// <param name="deleteTopLevelRecord">True if you wish the system to delete the top level record in the chain</param>
		/// <returns></returns>
		public static ClassGenExceptionCollection DeleteImmediate(string tableName, 
			string fieldName, 
			object recGUID, 
			bool deleteTopLevelRecord)
		{
			string sql = string.Empty;
			SqlCommand cmd = null;
			DataTable dt = new DataTable();
			int recsAffected = 0;
			ClassGenExceptionCollection errors = new ClassGenExceptionCollection();
			SqlConnection oConn = null;
			SqlTransaction transaction = null;
			List<string> sqlStatements = new List<string>();
			int nestingLevel = 50;		// Set how deep the delete will go

			// Find out if the record needs to be deleted
			if (recGUID is string && String.IsNullOrEmpty((string)recGUID)) { return errors; }
			else if (recGUID == null) { return errors; }

			try
			{
				#region Get Values From Table - SQL
				sql = "" +
					"SET NOCOUNT ON " + Environment.NewLine + Environment.NewLine + 
 
					"DECLARE @iLevel INT   SELECT @iLevel = 0  " + Environment.NewLine +
					"DECLARE @iRowCount INT		SELECT @iRowCount = 1  " + Environment.NewLine +
					"DECLARE @recID INT		SELECT @recID = 0  " + Environment.NewLine +
					"DECLARE @useIntForPrimary BIT, @useIntForLink BIT, @useIntForParent BIT " + Environment.NewLine +
					"SELECT @useIntForPrimary = CASE WHEN c.xType = 36 THEN 0 ELSE 1 END  " + Environment.NewLine +
					"FROM sysobjects o INNER JOIN syscolumns c ON o.id = c.id  " + Environment.NewLine +
					"WHERE o.name = @psTableName AND c.name = @psFieldName  " + Environment.NewLine + Environment.NewLine + 
 
					"IF object_id('tempdb..#tmpForeignKeys') IS NOT NULL DROP TABLE #tmpForeignKeys  " + Environment.NewLine +
					"CREATE TABLE #tmpForeignKeys (TargetTable VARCHAR(200),  " + Environment.NewLine +
					"	TargetColumn VARCHAR(200),  " + Environment.NewLine +
					"	SourceTable VARCHAR(200),  " + Environment.NewLine +
					"	SourceColumn VARCHAR(200),  " + Environment.NewLine +
					"	iLevel INT DEFAULT(0),  " + Environment.NewLine +
					"	bHasChildren BIT DEFAULT(0),  " + Environment.NewLine +
					"	recID INT IDENTITY(1,1),  " + Environment.NewLine +
					"	parentRecID INT)  " + Environment.NewLine + Environment.NewLine + 
 
					"INSERT INTO #tmpForeignKeys (TargetTable, TargetColumn, SourceTable, SourceColumn, iLevel)  " + Environment.NewLine +
					"SELECT @psTableName, @psFieldName, @psTableName, @psFieldName, 0  " + Environment.NewLine + Environment.NewLine + 

					"WHILE ((SELECT MAX(iLevel) FROM #tmpForeignKeys) < @piNestingLevel - 1 AND @recID < @piNestingLevel * 10)  " + Environment.NewLine +
					"BEGIN  " + Environment.NewLine +
					"	SELECT @recID = @recID + 1  " + Environment.NewLine + Environment.NewLine + 

					"	INSERT INTO #tmpForeignKeys (TargetTable, TargetColumn, SourceTable, SourceColumn, iLevel, parentRecID) " + Environment.NewLine +
					"	SELECT target_table.name AS target_table, target_column.name AS target_column,   " + Environment.NewLine + 
					"		source_table.name AS source_table, source_column.name AS source_column, T.iLevel + 1, @recID " + Environment.NewLine + 
					"	FROM sysforeignkeys fk  " + Environment.NewLine + 
					"	INNER JOIN sysobjects target_table ON fk.fkeyid = target_table.id   " + Environment.NewLine + 
					"	INNER JOIN syscolumns target_column ON fk.fkeyid = target_column.id AND fk.fkey = target_column.colid   " + Environment.NewLine + 
					"	INNER JOIN sysobjects source_table ON fk.rkeyid = source_table.id  " + Environment.NewLine + 
					"	INNER JOIN syscolumns source_column ON fk.rkeyid = source_column.id AND fk.rkey = source_column.colid  " + Environment.NewLine + 
					"	INNER JOIN #tmpForeignKeys T ON T.TargetTable = source_table.name " + Environment.NewLine + 
					"	WHERE T.recID = @recID " + Environment.NewLine +
					"	ORDER BY target_table.name, target_column.name  " + Environment.NewLine + Environment.NewLine + 

					"	SELECT @iRowCount = @@ROWCOUNT  " + Environment.NewLine + Environment.NewLine + 

					"	IF @iRowCount > 0 UPDATE #tmpForeignKeys SET bHasChildren = 1 WHERE recID = @recID  " + Environment.NewLine +
					"END  " + Environment.NewLine + Environment.NewLine + 

					"IF object_id('tempdb..#tmpDeleteValues') IS NOT NULL DROP TABLE #tmpDeleteValues  " + Environment.NewLine + 
					"CREATE TABLE #tmpDeleteValues ( " + Environment.NewLine + 
					"	sPrimaryKeyGUID UNIQUEIDENTIFIER, " + Environment.NewLine + 
					"	sLinkGUID UNIQUEIDENTIFIER, " + Environment.NewLine + 
					"	sPrimaryKeyID INT, " + Environment.NewLine + 
					"	sLinkID INT, " + Environment.NewLine + 
					"	sPrimaryKeyFieldName VARCHAR(200), " + Environment.NewLine + 
					"	iFKRecID INT DEFAULT(0), " + Environment.NewLine + 
					"	ChildTableName VARCHAR(200), " + Environment.NewLine + 
					"	ChildFieldName VARCHAR(200), " + Environment.NewLine + 
					"	ParentTableName VARCHAR(200), " + Environment.NewLine + 
					"	ParentFieldName VARCHAR(200), " + Environment.NewLine +
					"	recID INT IDENTITY(1,1))  " + Environment.NewLine + Environment.NewLine + 

					"SELECT @iLevel = ISNULL((SELECT MIN(recID) FROM #tmpForeignKeys), 9999999)  " + Environment.NewLine + 
					"IF (@useIntForPrimary = 0)  " + Environment.NewLine + 
					"	INSERT INTO #tmpDeleteValues (sPrimaryKeyGUID, sLinkGUID, sPrimaryKeyFieldName, ChildTableName,  " + Environment.NewLine + 
					"		ChildFieldName, ParentTableName, ParentFieldName, iFKRecID)  " + Environment.NewLine + 
					"	SELECT @pGUID, @pGUID, @psFieldName, @psTableName, @psFieldName, NULL, NULL, 1  " + Environment.NewLine + 
					"	FROM #tmpForeignKeys  " + Environment.NewLine + 
					"	WHERE recID = 1  " + Environment.NewLine + 
					"ELSE  " + Environment.NewLine + 
					"	INSERT INTO #tmpDeleteValues (sPrimaryKeyID, sLinkID, sPrimaryKeyFieldName, ChildTableName,  " + Environment.NewLine + 
					"		ChildFieldName, ParentTableName, ParentFieldName, iFKRecID)  " + Environment.NewLine + 
					"	SELECT @pID, @pID, @psFieldName, @psTableName, @psFieldName, NULL, NULL, 1  " + Environment.NewLine + 
					"	FROM #tmpForeignKeys  " + Environment.NewLine +
					"	WHERE recID = 1  " + Environment.NewLine + Environment.NewLine + 

					"SELECT @iLevel = 2 " + Environment.NewLine + Environment.NewLine + 

					"DECLARE @sql VARCHAR(7000), @primaryField VARCHAR(100), @linkField VARCHAR(100), @parentKeyField VARCHAR(100) " + Environment.NewLine + 
					"SELECT @sql = '' " + Environment.NewLine + 
					"WHILE (@iLevel <= ISNULL((SELECT MAX(recID) FROM #tmpForeignKeys), 0))  " + Environment.NewLine + 
					"BEGIN  " + Environment.NewLine +
					"	SELECT @iLevel = @iLevel + 1  " + Environment.NewLine + Environment.NewLine + 

					"	SELECT @useIntForPrimary = CASE WHEN c.xType = 36 THEN 0 ELSE 1 END  " + Environment.NewLine + 
					"	FROM sysobjects o INNER JOIN syscolumns c ON o.id = c.id  " + Environment.NewLine + 
					"	INNER JOIN #tmpForeignKeys T ON o.name = T.TargetTable AND c.name =  " + Environment.NewLine + 
					"	(SELECT KU.COLUMN_NAME  " + Environment.NewLine + 
					"				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC  " + Environment.NewLine + 
					"				INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KU  " + Environment.NewLine + 
					"				ON TC.CONSTRAINT_TYPE = 'PRIMARY KEY' AND " + Environment.NewLine + 
					"				TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME WHERE KU.TABLE_NAME = TargetTable) " + Environment.NewLine +
					"	WHERE T.recID = @iLevel - 1  " + Environment.NewLine + Environment.NewLine + 
	
					"	SELECT @useIntForLink = CASE WHEN c.xType = 36 THEN 0 ELSE 1 END  " + Environment.NewLine + 
					"	FROM sysobjects o INNER JOIN syscolumns c ON o.id = c.id  " + Environment.NewLine + 
					"	INNER JOIN #tmpForeignKeys T ON o.name = T.SourceTable AND c.name =  " + Environment.NewLine + 
					"	(SELECT KU.COLUMN_NAME  " + Environment.NewLine + 
					"				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC  " + Environment.NewLine + 
					"				INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KU  " + Environment.NewLine + 
					"				ON TC.CONSTRAINT_TYPE = 'PRIMARY KEY' AND " + Environment.NewLine + 
					"				TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME WHERE KU.TABLE_NAME = SourceTable) " + Environment.NewLine +
					"	WHERE T.recID = @iLevel - 1  " + Environment.NewLine + Environment.NewLine + 
		
					"	SELECT @primaryField = CASE WHEN @useIntForPrimary <> 0 THEN 'sPrimaryKeyID' ELSE 'sPrimaryKeyGUID' END " + Environment.NewLine + 
					"		,@linkField = CASE WHEN @useIntForLink <> 0 THEN 'sLinkID' ELSE 'sLinkGUID' END " + Environment.NewLine +
					"		,@parentKeyField = CASE WHEN @useIntForLink <> 0 THEN 'sPrimaryKeyID' ELSE 'sPrimaryKeyGUID' END " + Environment.NewLine + Environment.NewLine + 

					"	SELECT @sql = 'INSERT INTO #tmpDeleteValues (' + @primaryField + ', ' + @linkField + ', sPrimaryKeyFieldName, ChildTableName, ' +  " + Environment.NewLine + 
					"		'ChildFieldName, ParentTableName, ParentFieldName, iFKRecID) ' +  " + Environment.NewLine + 
					"		'SELECT T0.' + (SELECT KU.COLUMN_NAME  " + Environment.NewLine + 
					"			FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC  " + Environment.NewLine + 
					"			INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KU  " + Environment.NewLine + 
					"			ON TC.CONSTRAINT_TYPE = 'PRIMARY KEY' AND " + Environment.NewLine + 
					"			TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME WHERE KU.TABLE_NAME = TargetTable) + ', ' +  " + Environment.NewLine + 
					"			'T0.' + TargetColumn + ',' +  " + Environment.NewLine + 
					"			' ''' + (SELECT KU.COLUMN_NAME  " + Environment.NewLine + 
					"			FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC " + Environment.NewLine + 
					"			INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KU " + Environment.NewLine + 
					"			ON TC.CONSTRAINT_TYPE = 'PRIMARY KEY' AND " + Environment.NewLine + 
					"			TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME WHERE KU.TABLE_NAME = TargetTable) + ''',' + " + Environment.NewLine + 
					"		'''' + TargetTable + ''', ''' +  " + Environment.NewLine +
					"		TargetColumn + ''', ''' + SourceTable + ''', ''' + SourceColumn + ''', ' + CAST(recID AS VARCHAR(50)) + ' ' + " + Environment.NewLine + 
					"		'FROM ' + TargetTable + ' T0 INNER JOIN #tmpDeleteValues T1 ON T0.' + TargetColumn + ' = T1.' + @parentKeyField + ' ' + " + Environment.NewLine + 
					"		'WHERE T1.iFKRecID IN (SELECT parentRecID FROM #tmpForeignKeys WHERE recID = ' + CAST(recID AS VARCHAR(50)) + ') ' " + Environment.NewLine + 
					"	FROM #tmpForeignKeys " + Environment.NewLine + 
					"	WHERE recID = @iLevel - 1  " + Environment.NewLine +
					"	PRINT @sql " + Environment.NewLine + Environment.NewLine + 

					"	EXEC(@sql)  " + Environment.NewLine + Environment.NewLine + 

					"END  " + Environment.NewLine + Environment.NewLine + 

					"DROP TABLE #tmpForeignKeys  " + Environment.NewLine + Environment.NewLine + 

					"SELECT iFKRecID, sPrimaryKeyGUID, sPrimaryKeyID, sPrimaryKeyFieldName, ChildTableName, recID FROM #tmpDeleteValues ORDER BY recID DESC   " + Environment.NewLine + Environment.NewLine + 

					"DROP TABLE #tmpDeleteValues " + Environment.NewLine + Environment.NewLine + 

					"";

				#region Old Code
				//sql = "" +
				//    //"-- DELETE Child Records based on foreign keys examples " + Environment.NewLine + 
				//    "SET NOCOUNT ON  " + Environment.NewLine +
				//    " " + Environment.NewLine +

				//    "DECLARE @iLevel INT   SELECT @iLevel = 0 " + Environment.NewLine +
				//    "DECLARE @iRowCount INT		SELECT @iRowCount = 1 " + Environment.NewLine +
				//    "DECLARE @recID INT		SELECT @recID = 0 " + Environment.NewLine +
				//    "DECLARE @useIntField BIT " + Environment.NewLine +
				//    "SELECT @useIntField = CASE WHEN c.xType = 36 THEN 0 ELSE 1 END " + Environment.NewLine +
				//    "FROM sysobjects o INNER JOIN syscolumns c ON o.id = c.id " + Environment.NewLine +
				//    "WHERE o.name = @psTableName AND c.name = @psFieldName " + Environment.NewLine +
				//    " " + Environment.NewLine +

				//    //"-- Create a temp table to hold the foreign keys " + Environment.NewLine + 
				//    "IF object_id('tempdb..#tmpForeignKeys') IS NOT NULL DROP TABLE #tmpForeignKeys " + Environment.NewLine +
				//    "CREATE TABLE #tmpForeignKeys (TargetTable VARCHAR(200), " + Environment.NewLine +
				//    "	TargetColumn VARCHAR(200), " + Environment.NewLine +
				//    "	SourceTable VARCHAR(200), " + Environment.NewLine +
				//    "	SourceColumn VARCHAR(200), " + Environment.NewLine +
				//    "	iLevel INT DEFAULT(0), " + Environment.NewLine +
				//    "	bHasChildren BIT DEFAULT(0), " + Environment.NewLine +
				//    "	recID INT IDENTITY(1,1), " + Environment.NewLine +
				//    "	parentRecID INT) " + Environment.NewLine +
				//    " " + Environment.NewLine +

				//    //"-- Build the foreign keys" + Environment.NewLine + 
				//    "INSERT INTO #tmpForeignKeys (TargetTable, TargetColumn, SourceTable, SourceColumn, iLevel) " + Environment.NewLine +
				//    "SELECT @psTableName, @psFieldName, @psTableName, @psFieldName, 0 " + Environment.NewLine +
				//    "" + Environment.NewLine +

				//    "WHILE ((SELECT MAX(iLevel) FROM #tmpForeignKeys) < @piNestingLevel - 1 AND @recID < @piNestingLevel * 10) " + Environment.NewLine +
				//    "BEGIN " + Environment.NewLine +
				//    "	SELECT @recID = @recID + 1 " + Environment.NewLine +
				//    "" + Environment.NewLine +

				//    "	INSERT INTO #tmpForeignKeys (TargetTable, TargetColumn, SourceTable, SourceColumn, iLevel, parentRecID)" + Environment.NewLine +
				//    "	SELECT target_table.name AS target_table, target_column.name AS target_column,  " + Environment.NewLine +
				//    "		source_table.name AS source_table, source_column.name AS source_column, T.iLevel + 1, @recID" + Environment.NewLine +
				//    "	FROM sysforeignkeys fk " + Environment.NewLine +
				//    "	INNER JOIN sysobjects target_table ON fk.fkeyid = target_table.id  " + Environment.NewLine +
				//    "	INNER JOIN syscolumns target_column ON fk.fkeyid = target_column.id AND fk.fkey = target_column.colid  " + Environment.NewLine +
				//    "	INNER JOIN sysobjects source_table ON fk.rkeyid = source_table.id " + Environment.NewLine +
				//    "	INNER JOIN syscolumns source_column ON fk.rkeyid = source_column.id AND fk.rkey = source_column.colid " + Environment.NewLine +
				//    "	INNER JOIN #tmpForeignKeys T ON T.TargetTable = source_table.name" + Environment.NewLine +
				//    "	WHERE T.recID = @recID" + Environment.NewLine +
				//    "	ORDER BY target_table.name, target_column.name " + Environment.NewLine +
				//    "" + Environment.NewLine +

				//    "	SELECT @iRowCount = @@ROWCOUNT " + Environment.NewLine +
				//    "" + Environment.NewLine +

				//    "	IF @iRowCount > 0 UPDATE #tmpForeignKeys SET bHasChildren = 1 WHERE recID = @recID " + Environment.NewLine +
				//    "END " + Environment.NewLine +
				//    "" + Environment.NewLine +

				//    //"-- Go through the foreign keys and load up a table with all the values you have to delete" + Environment.NewLine + 
				//    "IF object_id('tempdb..#tmpDeleteValues') IS NOT NULL DROP TABLE #tmpDeleteValues " + Environment.NewLine +
				//    "CREATE TABLE #tmpDeleteValues (" + Environment.NewLine +
				//    "	sPrimaryKeyGUID UNIQUEIDENTIFIER," + Environment.NewLine +
				//    "	sLinkGUID UNIQUEIDENTIFIER," + Environment.NewLine +
				//    "	sPrimaryKeyID INT," + Environment.NewLine +
				//    "	sLinkID INT," + Environment.NewLine +
				//    "	sPrimaryKeyFieldName VARCHAR(200)," + Environment.NewLine +
				//    "	iFKRecID INT DEFAULT(0)," + Environment.NewLine +
				//    "	ChildTableName VARCHAR(200)," + Environment.NewLine +
				//    "	ChildFieldName VARCHAR(200)," + Environment.NewLine +
				//    //"	--sParentRecGUID UNIQUEIDENTIFIER, " + Environment.NewLine +
				//    "	ParentTableName VARCHAR(200)," + Environment.NewLine +
				//    "	ParentFieldName VARCHAR(200)," + Environment.NewLine +
				//    "	recID INT IDENTITY(1,1)) " + Environment.NewLine +
				//    "" + Environment.NewLine +

				//    "SELECT @iLevel = ISNULL((SELECT MIN(recID) FROM #tmpForeignKeys), 9999999) " + Environment.NewLine +
				//    "IF (@useIntField = 0) " + Environment.NewLine +
				//    "	INSERT INTO #tmpDeleteValues (sPrimaryKeyGUID, sLinkGUID, sPrimaryKeyFieldName, ChildTableName, " + Environment.NewLine +
				//    "		ChildFieldName, ParentTableName, ParentFieldName, iFKRecID) " + Environment.NewLine +
				//    "	SELECT @pGUID, @pGUID, @psFieldName, @psTableName, @psFieldName, NULL, NULL, 1 " + Environment.NewLine +
				//    "	FROM #tmpForeignKeys " + Environment.NewLine +
				//    "	WHERE recID = 1 " + Environment.NewLine +
				//    "ELSE " + Environment.NewLine +
				//    "	INSERT INTO #tmpDeleteValues (sPrimaryKeyID, sLinkID, sPrimaryKeyFieldName, ChildTableName, " + Environment.NewLine +
				//    "		ChildFieldName, ParentTableName, ParentFieldName, iFKRecID) " + Environment.NewLine +
				//    "	SELECT @pID, @pID, @psFieldName, @psTableName, @psFieldName, NULL, NULL, 1 " + Environment.NewLine +
				//    "	FROM #tmpForeignKeys " + Environment.NewLine +
				//    "	WHERE recID = 1 " + Environment.NewLine +
				//    "" + Environment.NewLine +

				//    "SELECT @iLevel = 2 " + Environment.NewLine +
				//    "" + Environment.NewLine +

				//    "DECLARE @sql VARCHAR(7000) " + Environment.NewLine +
				//    "SELECT @sql = ''" + Environment.NewLine +
				//    "WHILE (@iLevel <= ISNULL((SELECT MAX(recID) FROM #tmpForeignKeys), 0)) " + Environment.NewLine +
				//    "BEGIN " + Environment.NewLine +
				//    "	SELECT @iLevel = @iLevel + 1 " + Environment.NewLine +
				//    "" + Environment.NewLine +

				//    "	IF (@useIntField = 0) " + Environment.NewLine +
				//    "		SELECT @sql = 'INSERT INTO #tmpDeleteValues (sPrimaryKeyGUID, sLinkGUID, sPrimaryKeyFieldName, ChildTableName, ' + " + Environment.NewLine +
				//    "			'ChildFieldName, ParentTableName, ParentFieldName, iFKRecID) ' + " + Environment.NewLine +
				//    "			'SELECT T0.' + (SELECT KU.COLUMN_NAME " + Environment.NewLine +
				//    "				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC " + Environment.NewLine +
				//    "				INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KU " + Environment.NewLine +
				//    "				ON TC.CONSTRAINT_TYPE = 'PRIMARY KEY' AND" + Environment.NewLine +
				//    "				TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME WHERE KU.TABLE_NAME = TargetTable) + ', ' + " + Environment.NewLine +
				//    "				'T0.' + TargetColumn + ',' + " + Environment.NewLine +
				//    "				' ''' + (SELECT KU.COLUMN_NAME " + Environment.NewLine +
				//    "				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC" + Environment.NewLine +
				//    "				INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KU" + Environment.NewLine +
				//    "				ON TC.CONSTRAINT_TYPE = 'PRIMARY KEY' AND" + Environment.NewLine +
				//    "				TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME WHERE KU.TABLE_NAME = TargetTable) + ''',' +" + Environment.NewLine +
				//    "			'''' + TargetTable + ''', ''' + " + Environment.NewLine +
				//    "			TargetColumn + ''', ''' + SourceTable + ''', ''' + SourceColumn + ''', ' + CAST(recID AS VARCHAR(50)) + ' ' +" + Environment.NewLine +
				//    "			'FROM ' + TargetTable + ' T0 INNER JOIN #tmpDeleteValues T1 ON T0.' + TargetColumn + ' = T1.sPrimaryKeyGUID ' +" + Environment.NewLine +
				//    "			'WHERE T1.iFKRecID IN (SELECT parentRecID FROM #tmpForeignKeys WHERE recID = ' + CAST(recID AS VARCHAR(50)) + ') '" + Environment.NewLine +
				//    "		FROM #tmpForeignKeys" + Environment.NewLine +
				//    "		WHERE recID = @iLevel - 1 " + Environment.NewLine +
				//    "	ELSE " + Environment.NewLine +
				//    "		SELECT @sql = 'INSERT INTO #tmpDeleteValues (sPrimaryKeyID, sLinkID, sPrimaryKeyFieldName, ChildTableName, ' + " + Environment.NewLine +
				//    "			'ChildFieldName, ParentTableName, ParentFieldName, iFKRecID) ' + " + Environment.NewLine +
				//    "			'SELECT T0.' + (SELECT KU.COLUMN_NAME " + Environment.NewLine +
				//    "				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC " + Environment.NewLine +
				//    "				INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KU " + Environment.NewLine +
				//    "				ON TC.CONSTRAINT_TYPE = 'PRIMARY KEY' AND" + Environment.NewLine +
				//    "				TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME WHERE KU.TABLE_NAME = TargetTable) + ', ' + " + Environment.NewLine +
				//    "				'T0.' + TargetColumn + ',' + " + Environment.NewLine +
				//    "				' ''' + (SELECT KU.COLUMN_NAME " + Environment.NewLine +
				//    "				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC" + Environment.NewLine +
				//    "				INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KU" + Environment.NewLine +
				//    "				ON TC.CONSTRAINT_TYPE = 'PRIMARY KEY' AND" + Environment.NewLine +
				//    "				TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME WHERE KU.TABLE_NAME = TargetTable) + ''',' +" + Environment.NewLine +
				//    "			'''' + TargetTable + ''', ''' + " + Environment.NewLine +
				//    "			TargetColumn + ''', ''' + SourceTable + ''', ''' + SourceColumn + ''', ' + CAST(recID AS VARCHAR(50)) + ' ' +" + Environment.NewLine +
				//    "			'FROM ' + TargetTable + ' T0 INNER JOIN #tmpDeleteValues T1 ON T0.' + TargetColumn + ' = T1.sPrimaryKeyID ' +" + Environment.NewLine +
				//    "			'WHERE T1.iFKRecID IN (SELECT parentRecID FROM #tmpForeignKeys WHERE recID = ' + CAST(recID AS VARCHAR(50)) + ') '" + Environment.NewLine +
				//    "		FROM #tmpForeignKeys" + Environment.NewLine +
				//    "		WHERE recID = @iLevel - 1 " + Environment.NewLine +
				//    "" + Environment.NewLine +

				//    "	EXEC(@sql) " + Environment.NewLine +
				//    "" + Environment.NewLine +

				//    "END " + Environment.NewLine +
				//    "" + Environment.NewLine +

				//    //"-- Drop the foreign keys" + Environment.NewLine + 
				//    "DROP TABLE #tmpForeignKeys " + Environment.NewLine +
				//    "" + Environment.NewLine +

				//    //"-- Return the values" + Environment.NewLine +
				//    "SELECT iFKRecID, sPrimaryKeyGUID, sPrimaryKeyID, sPrimaryKeyFieldName, ChildTableName, recID " + 
				//    "FROM #tmpDeleteValues ORDER BY recID DESC " + Environment.NewLine +
				//    "" + Environment.NewLine +

				//    //"-- Drop the tmpTable" + Environment.NewLine +
				//    "DROP TABLE #tmpDeleteValues " + Environment.NewLine +
				//    "";
				#endregion Old Code
				#endregion Get Values From Table - SQL

				// Find out which records need to be changed out
				cmd = new SqlCommand(sql);
                long val = 0;
                bool useInt = long.TryParse((string)recGUID, out val);
                if (useInt)
                {
					cmd.Parameters.Add("@pGUID", SqlDbType.UniqueIdentifier).Value = DBNull.Value;
					cmd.Parameters.Add("@pID", SqlDbType.Int).Value = val;
					//useInt = true;
				}
				//if (int.TryParse(recGUID, out val))
				//{
				//    cmd.Parameters.Add("@pGUID", SqlDbType.UniqueIdentifier).Value = DBNull.Value;
				//    cmd.Parameters.Add("@pID", SqlDbType.Int).Value = val;
				//    useInt = true;
				//}
				else
				{
					cmd.Parameters.Add("@pGUID", SqlDbType.UniqueIdentifier).Value = new System.Guid((string)recGUID);
					cmd.Parameters.Add("@pID", SqlDbType.Int).Value = DBNull.Value;
				}
				cmd.Parameters.Add("@psTableName", SqlDbType.VarChar, 200).Value = tableName;
				cmd.Parameters.Add("@psFieldName", SqlDbType.VarChar, 200).Value = fieldName;
				cmd.Parameters.Add("@piNestingLevel", SqlDbType.Int).Value = nestingLevel;

				// Go out and get the records
				if (oConn != null && oConn.State == ConnectionState.Closed) { oConn = null; }
				bool connectionEstablished = (oConn != null);
				if (oConn == null) { oConn = DAL.OpenConnection(); }

				// Get the records
				dt = DAL.SQLExecDataTable(cmd, oConn);

				// Now that you have the records, go through them and figure out how to build the deletes
				long lastIndex = 9999999;
				string lastTableName = string.Empty, lastFieldName = string.Empty;
				int processingIndex = 0;
				StringBuilder sb = new StringBuilder();
				foreach (DataRow row in dt.Rows)
				{
					if (!deleteTopLevelRecord &&
						(row["ParentTableName"] == DBNull.Value ||
						String.IsNullOrEmpty(row["ParentTableName"].ToString()))) { break; }

					if (lastIndex != (int)row["iFKRecID"] || processingIndex >= 150)
					{
						if (lastIndex != 9999999)
						{
							// We have a new table to deal with here, start a new statement
							sqlStatements.Add("DELETE FROM " + lastTableName + " " +
								"WHERE " + lastFieldName + " " +
								"IN (" + sb.ToString() + ")");
						}
						sb = new StringBuilder();
						lastIndex = long.Parse(row["iFKRecID"].ToString());
						lastTableName = row["ChildTableName"].ToString();
						lastFieldName = row["sPrimaryKeyFieldName"].ToString();
						processingIndex = 0;
					}

					// Append the string
					//if (useInt)
					//{
					//    sb.Append((sb.ToString().Trim().Length > 0 ? "," : "") +
					//        row["sPrimaryKeyID"].ToString() +
					//        "");
					//}
					//else
					//{
					//    sb.Append((sb.ToString().Trim().Length > 0 ? "," : "") +
					//        "'" + row["sPrimaryKeyGUID"].ToString() + "'" +
					//        "");
					//}
					if (row["sPrimaryKeyGUID"] != DBNull.Value ||
						row["sPrimaryKeyID"] != DBNull.Value)
					{
						sb.Append((sb.ToString().Trim().Length > 0 ? "," : "") +
							(row["sPrimaryKeyGUID"] != DBNull.Value ? "'" + row["sPrimaryKeyGUID"].ToString() + "'" : row["sPrimaryKeyID"].ToString()) +
							"");
					}
				}
				// Write the last string
				sqlStatements.Add("DELETE FROM " + lastTableName + " " +
					"WHERE " + lastFieldName + " " +
					"IN (" + sb.ToString() + ")");

				// Start a new transaction
				transaction = oConn.BeginTransaction();

				// Roll through the SQL statements and fire them against the DB
				foreach (string s in sqlStatements)
				{
					cmd = new SqlCommand(s);
					cmd.Transaction = transaction;
					recsAffected += DAL.SQLExecNonQuery(cmd, oConn);
				}

				// Attempt to close the connection
				if (transaction != null)
				{
					try
					{
						transaction.Commit();		// Commit the transaction
						transaction.Dispose();		// Dispose of the transaction
						transaction = null;			// Reset the transaction to null
					}
					catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
					catch (Exception ex) { errors.Add(new ClassGenException(ex)); }
					finally
					{
						if (transaction != null)
						{
							try
							{
								transaction.Rollback();		// Attempt a rollback
							}
							catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
							catch (Exception ex) { errors.Add(new ClassGenException(ex)); }
						}
					}
					if (!connectionEstablished) { DAL.CloseConnection(oConn); oConn = null; }		// Close the connection if we created it here
				}
			}
			catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
			catch (Exception ex) { errors.Add(new ClassGenException(ex)); }

			return errors;			// Return the exception collection
		}
		#endregion Static Methods
    }

#region ClassGenBindingList 
	public interface IClassGenBindingList
	{
		// Events
		//event MasterPopulateEventHandler MasterPopulate;
		//event DetailPopulateEventHandler DetailPopulate;
		//event MasterUpdateEventHandler MasterUpdate;
		//event DetailUpdateEventHandler DetailUpdate;
		//event MasterDeleteEventHandler MasterDelete;
		//event DetailDeleteEventHandler DetailDelete;

		// Properties
		GetCollectionConfiguration GetCollectionConfig { get; set; }
		bool IsDisposable { get; set; }
		Type GenericBaseType { get; }
		bool IsLoaded { get; set; }
		RefreshFrequency RefreshRate { get; set; }
		bool GetThreadRunning { get; }
		ClassGenRules Rules { get; set; }
		int CheckedCount { get; }

		// Methods
		List<string> PropertyValueExistsMoreThanOnceInCollection(string propertyName);
		List<string> PropertyValueExistsMoreThanOnceInCollection(string propertyName, bool sortDescending);
		void Sort(string orderBy);
		void UndoChanges();
		void CheckAll();
		void CheckNone();
		void ClearChecked();
	}

	public interface IClassGenBindingList<T, K> where T : LEAD_BaseObject
	{
		// Methods
		void Sort(DynamicComparer<T> comparer);
		ClassGenBindingList<T, K> GetRecordsBasedOnStatus(RecordStatus status);
	}

	public interface IImportable : IList
    {
        string Name { get; }
        //Type ItemType { get; }
		ClassGenExceptionCollection ImportValidateData();
		ClassGenExceptionCollection ImportPutAddOnly();
		ClassGenExceptionCollection ImportPutOverwrite();
		ClassGenExceptionCollection ImportPutDeleteFirst();
    }

	public class ImportExportFieldAttribute : Attribute
    {
		private int _ordinalPos = -1;
		private bool _hidden = false;

		public ImportExportFieldAttribute(int ordinalPos, bool hidden)
        {
			_ordinalPos = ordinalPos;
			_hidden = hidden;
        }

		public ImportExportFieldAttribute()
		{
		}

		public int OrdinalPos
		{
			get { return _ordinalPos; }
		}

		public bool Hidden
		{
			get { return _hidden; }
		}
    }

	[Serializable]
	public class ClassGenBindingList<T, K> : BindingList<T>, IClassGenBindingList<T, K>, IClassGenBindingList,
		IBindingListView, ITypedList, IRaiseItemChangedEvents, IClassGenILCode
		where T : LEAD_BaseObject
	{
		private bool _sorted = false;
		protected internal bool _isDisposable = true;
		private bool _filtered = false;
		private string _filterString = null;

		[NonSerialized, System.Xml.Serialization.XmlIgnore]
		private ListSortDirection _sortDirection = ListSortDirection.Ascending;

		[NonSerialized, System.Xml.Serialization.XmlIgnore]
		private PropertyDescriptor _sortProperty = null;
		
		[NonSerialized, System.Xml.Serialization.XmlIgnore]
		private ListSortDescriptionCollection _sortDescriptions = new ListSortDescriptionCollection();
		
		private List<T> _originalCollection = new List<T>();

		#region Delegate Handlers
		private delegate DataTable EmptyDTFunc<T1>();
		#endregion Delegate Handlers

		#region Static IL Holders
		[NonSerialized, System.Xml.Serialization.XmlIgnore]
		public static Dictionary<Type, Delegate> EmptyDataTableIL = new Dictionary<Type, Delegate>();
		#endregion Static IL Holders

		#region Protected Variables
		protected internal bool _isLoaded = false;		// For use with parent and child collections
		protected internal DateTime? _dateTimeObjectPopulated = null;		// The Date/Time the object was populated
		protected internal RefreshFrequency _refreshRate = RefreshFrequency.ByDeveloperOnly;			// Set the refresh rate
		protected internal ClassGenRules _rules = null;
		[NonSerialized, System.Xml.Serialization.XmlIgnore]
		protected internal SqlConnection _asyncConnection = null;		// A connection that can be used for updates

		[NonSerialized, System.Xml.Serialization.XmlIgnore]
		protected internal BackgroundWorker _backgroundWorker = new BackgroundWorker();

		private GetCollectionConfiguration _getCollectionConfig = new GetCollectionConfiguration();		// The configuration for the get method

		protected internal bool _disposed = false;

		//protected Dictionary<string, T> _keyedStringCollection = new Dictionary<string, T>();		// Generate a Keyed Collection for the objects
		//protected Dictionary<int, T> _keyedIntCollection = new Dictionary<int, T>();			// Generate a Keyed Collection for the objects

		protected string KeyFieldName = string.Empty;
		protected List<string> ForeignKeyFields = new List<string>();
		protected IDictionary<K, T> KeyedCollection;
		protected IDictionary<string, Dictionary<object, List<T>>> FKKeyedCollection;		// The field name is the string here, then object is the GUID/int RecID of the record, then list is the objects

		#endregion Protected Variables

		#region Collection Add/Insert/Remove Overrides
		// Add Method
		public new void Add(T item)
		{
			this.Insert(this.Count, item);
		}

		// AddRange Method
		public void AddRange(IEnumerable<T> collection)
		{
			foreach (T item in collection)
			{
				this.Add(item);
			}
		}

		// Clear method
		public new void Clear()
		{
			base.Clear();					// Call the base method
			KeyedCollection.Clear();		// Clear the Keyed collection
			FKKeyedCollection.Clear();		// Clear the Foreign Key Collection
		}

		// Alternate Contains Method
		public bool Contains(K key)
		{
			return KeyedCollection.ContainsKey(key);
		}

		// Insert Method
		public new void Insert(int index, T item)
		{
			base.Insert(index, item);		// Call the base method
			if (!KeyedCollection.ContainsKey((K)item.GetType().GetProperty(KeyFieldName).GetValue(item, null)))
			{
				KeyedCollection.Add((K)item.GetType().GetProperty(KeyFieldName).GetValue(item, null), item);
			}
			else
			{
				// Log the error
				throw new LEADBaseException("The key: " + ((K)item.GetType().GetProperty(KeyFieldName).GetValue(item, null)).ToString() +
					" already exists in the " + item.GetType().ToString() + " collection.  Please try again.");
			}

			// Go through and get the key fields
			if (ForeignKeyFields.Count > 0)
			{
				foreach (string keyFieldName in ForeignKeyFields)
				{
					if (item.GetType().GetProperty(keyFieldName).GetValue(item, null) != null)
					{
						this.MaintainFKCollectionAdd((Dictionary<string, Dictionary<object, List<T>>>)FKKeyedCollection,
							keyFieldName,
							item.GetType().GetProperty(keyFieldName).GetValue(item, null),
							item);
					}
				}
			}
			
			//// See if the parent item(s) are set
			//if (item.BackgroundItem != null) { item.BackgroundItem.RecordStatus = RecordStatus.Modified; }
		}

		// InsertRange Method
		public void InsertRange(int index, IEnumerable<T> collection)
		{
			int count = 0;
			foreach (T item in collection)
			{
				this.Insert(index + count, item);
				count++;
			}
		}

		// Remove Method
		public new bool Remove(T item)
		{
			//// See if the parent item(s) are set
			//if (item.BackgroundItem != null) { item.BackgroundItem.RecordStatus = RecordStatus.Modified; }

			// Go through and get the key fields
			if (ForeignKeyFields.Count > 0)
			{
				foreach (string keyFieldName in ForeignKeyFields)
				{
					if (item.GetType().GetProperty(keyFieldName).GetValue(item, null) != null)
					{
						this.MaintainFKCollectionRemove((Dictionary<string, Dictionary<object, List<T>>>)FKKeyedCollection,
							keyFieldName,
							(K)item.GetType().GetProperty(keyFieldName).GetValue(item, null),
							(K)item.GetType().GetProperty(this.KeyFieldName).GetValue(item, null));
					}
				}
			}

			KeyedCollection.Remove((K)item.GetType().GetProperty(KeyFieldName).GetValue(item, null));
			return base.Remove(item);		// Call the base method
		}

		// RemoveAll
		public int RemoveAll(Predicate<T> match)
		{
			int count = 0;
			foreach (T item in this)
			{
				if (match.Invoke(item))
				{
					this.Remove(item);
					count++;
				}
			}
			return count;
		}

		// RemoveAt method
		public new void RemoveAt(int index)
		{
			this.Remove(this[index]);
		}

		// Remove Range Method
		public void RemoveRange(int index, int count)
		{
			for (int i = index + count - 1; i >= 0; i--)
			{
				this.Remove(this[i]);
			}
		}

		/// <summary>
		/// Add the item to the collection
		/// </summary>
		private void MaintainFKCollectionAdd(Dictionary<string, Dictionary<object, List<T>>> collection,
			string keyFieldName,
			object collectionKey,
			T item)
		{
			if (collectionKey is string && String.IsNullOrEmpty(collectionKey.ToString())) { return; }
			List<T> list = new List<T>();
			if (collection.ContainsKey(keyFieldName))
			{
				if (collection[keyFieldName].ContainsKey(collectionKey))
				{
					// Update the element
					list = collection[keyFieldName][collectionKey];
					collection[keyFieldName].Remove(collectionKey);
				}
			}
			list.Add(item);		// Add the item to the list
			if (collection.ContainsKey(keyFieldName))
			{
				// Let's find the one that exists and update it
				collection[keyFieldName].Add(collectionKey, list);
			}
			else
			{
				// Just add the new one
				Dictionary<object, List<T>> newRec = new Dictionary<object,List<T>>();
				newRec.Add(collectionKey, list);
				collection.Add(keyFieldName, newRec);
			}
		}

		/// <summary>
		/// Remove the item from the collection
		/// </summary>
		private void MaintainFKCollectionRemove(Dictionary<string, Dictionary<object, List<T>>> collection,
			string keyFieldName,
			object collectionKey,
			K itemKey)
		{
			if (collectionKey is string && String.IsNullOrEmpty(collectionKey.ToString())) { return; }
			if (collection.ContainsKey(keyFieldName))
			{
				if (collection[keyFieldName].ContainsKey(collectionKey))
				{
					// Update the element
					List<T> list = collection[keyFieldName][collectionKey];
					for (int i = list.Count - 1; i >= 0; i--)
					{
						K val1 = (K)list[i].GetType().GetProperty(KeyFieldName).GetValue(list[i], null);
						K val2 = (K)itemKey;
						if (val1.Equals(val2))
						{
							list.RemoveAt(i);
						}
					}
					collection[keyFieldName].Remove(collectionKey);
					collection[keyFieldName].Add(collectionKey, list);
				}
			}
		}
		#endregion Collection Add/Insert/Remove Overrides

		public ClassGenBindingList()
			: base()
		{
			KeyedCollection = new Dictionary<K, T>();
			FKKeyedCollection = new Dictionary<string, Dictionary<object, List<T>>>();
		}

		public ClassGenBindingList(List<T> list)
			: base(list)
		{
			KeyedCollection = new Dictionary<K, T>();
			FKKeyedCollection = new Dictionary<string, Dictionary<object, List<T>>>();
		}

		protected override bool SupportsSearchingCore
		{
			get { return true; }
		}

		public GetCollectionConfiguration GetCollectionConfig
		{
			get { return _getCollectionConfig; }
			set { _getCollectionConfig = value; }
		}

		public bool IsDisposable
		{
			get { return _isDisposable; }
			set { _isDisposable = value; }
		}

		public bool IsCurrent
		{
			get 
			{
				if (!(typeof(LEAD_BaseObject).IsAssignableFrom(typeof(T))))
				{
					return true;
				}
				foreach (T item in this)
				{
					if ((item as LEAD_BaseObject).RecordStatus != RecordStatus.Current)
					{
						return false;
					}
				}
				return true;
			}
		}

		protected override int FindCore(PropertyDescriptor property, object key)
		{
			// Simple iteration:
			for (int i = 0; i < Count; i++)
			{
				T item = this[i];
				if (property.GetValue(item).Equals(key))
				{
					return i;
				}
			}
			return -1; // Not found
		}

		protected override bool SupportsSortingCore
		{
			get { return true; }
		}

		protected override bool IsSortedCore
		{
			get { return _sorted; }
		}

		protected override ListSortDirection SortDirectionCore
		{
			get { return _sortDirection; }
		}

		protected override PropertyDescriptor SortPropertyCore
		{
			get { return _sortProperty; }
		}

		protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
		{
			_sortDirection = direction;
			_sortProperty = property;
			SortProperty[] sortProperties = new SortProperty[] {
				new SortProperty(property.Name, direction == ListSortDirection.Descending),
			};
			//DynamicComparer<T> comparer = new DynamicComparer<T>(property, direction);
			DynamicComparer<T> comparer = new DynamicComparer<T>(sortProperties);
			ApplySortInternal(comparer);
		}

		public bool Exists(Predicate<T> match)
		{
			List<T> listRef = this.Items as List<T>;
			if (listRef == null) { return false; }

			return listRef.Exists(match);
		}

		protected List<T> GetAsList()
		{
			List<T> listRef = this.Items as List<T>;
			if (listRef == null) { return null; }

			return listRef;
		}

		private void ApplySortInternal(DynamicComparer<T> comparer)
		{
			if (_originalCollection.Count == 0)
			{
				_originalCollection.AddRange(this);
			}
			List<T> listRef = this.Items as List<T>;
			if (listRef == null) { return; }

			listRef.Sort(comparer);
			_sorted = true;
			OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}

		public void Sort(DynamicComparer<T> comparer)
		{
			ApplySortInternal(comparer);
		}

		protected override void RemoveSortCore()
		{
			if (!_sorted) { return; }
			Clear();
			foreach (T item in _originalCollection)
			{
				Add(item);
			}
			_originalCollection.Clear();
			_sortProperty = null;
			_sortDescriptions = null;
			_sorted = false;
		}


		#region IBindingListView Members

		void IBindingListView.ApplySort(ListSortDescriptionCollection sorts)
		{
			_sortProperty = null;
			_sortDescriptions = sorts;
			//SortComparer<T> comparer = new SortComparer<T>(sorts);
			SortProperty[] sortProperties = new SortProperty[sorts.Count];
			for (int i = 0; i < sorts.Count; i++)
			{
				sortProperties[i] = new SortProperty(sorts[i].PropertyDescriptor.Name, sorts[i].SortDirection == ListSortDirection.Descending);
			}
			DynamicComparer<T> comparer = new DynamicComparer<T>(sortProperties);
			ApplySortInternal(comparer);
		}

		string IBindingListView.Filter
		{
			get
			{
				return _filterString;
			}
			set
			{
				_filterString = value;
				_filtered = true;
				UpdateFilter();
			}
		}

		void IBindingListView.RemoveFilter()
		{
			if (!_filtered) { return; }
			_filterString = null;
			_filtered = false;
			_sorted = false;
			_sortDescriptions = null;
			_sortProperty = null;
			Clear();
			foreach (T item in _originalCollection)
			{
				Add(item);
			}
			_originalCollection.Clear();
		}

		ListSortDescriptionCollection IBindingListView.SortDescriptions
		{
			get
			{
				return _sortDescriptions;
			}
		}

		bool IBindingListView.SupportsAdvancedSorting
		{
			get
			{
				return true;
			}
		}

		bool IBindingListView.SupportsFiltering
		{
			get
			{
				return true;
			}
		}

		#endregion
		protected virtual void UpdateFilter()
		{

			int equalsPos = _filterString.IndexOf('=');
			// Get property name
			string propName = _filterString.Substring(0, equalsPos).Trim();
			// Get Filter criteria
			string criteria = _filterString.Substring(equalsPos + 1, _filterString.Length - equalsPos - 1).Trim();
			criteria = criteria.Substring(1, criteria.Length - 2); // string leading and trailing quotes
			// Get a property descriptor for the filter property
			PropertyDescriptor propDesc = TypeDescriptor.GetProperties(typeof(T))[propName];
			if (_originalCollection.Count == 0)
			{
				_originalCollection.AddRange(this);
			}
			List<T> currentCollection = new List<T>(this);
			Clear();
			foreach (T item in currentCollection)
			{
				object value = propDesc.GetValue(item);
				if (value.ToString() == criteria)
				{
					Add(item);
				}
			}

		}

		#region IL Code
		/// <summary>
		/// Generates an Empty Data Table from the passed in object type
		/// </summary>
		/// <typeparam name="T">The type parameter to use to generate the object</typeparam>
		/// <returns>The empty data table structure</returns>
		public DataTable EmptyDataTableWithIL<T1>()
		{
			try
			{
				Delegate myExec = null;
				try
				{
					if (!EmptyDataTableIL.TryGetValue(typeof(T), out myExec))
					{
						// Create ILGenerator
						DynamicMethod dymMethod = new DynamicMethod("EmptyDataTable", typeof(System.Data.DataTable), new Type[] { typeof(T) }, true);
						ConstructorInfo cInfo = typeof(System.Data.DataTable).GetConstructor(new Type[1] { typeof(string) });
						
						ILGenerator generator = dymMethod.GetILGenerator();

						LocalBuilder lbf = generator.DeclareLocal(typeof(System.Data.DataTable));

						generator.Emit(OpCodes.Nop);
						generator.Emit(OpCodes.Ldstr, typeof(T).ToString().Substring(typeof(T).ToString().LastIndexOf(".") + 1));
						generator.Emit(OpCodes.Newobj, cInfo);
						generator.Emit(OpCodes.Stloc_0);

						foreach (PropertyInfo propInfo in typeof(T).GetProperties(
							System.Reflection.BindingFlags.Instance |
							System.Reflection.BindingFlags.NonPublic |
							System.Reflection.BindingFlags.Public))
						{
							
							generator.Emit(OpCodes.Ldloc_0);
							generator.Emit(OpCodes.Callvirt, typeof(System.Data.DataTable).GetMethod("get_Columns"));

							generator.Emit(OpCodes.Ldstr, propInfo.Name);

							if (propInfo.PropertyType.IsGenericType &&
								propInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
							{
								generator.Emit(OpCodes.Ldtoken, System.Type.GetType(propInfo.PropertyType.GetGenericArguments()[0].FullName));
							}
							else
							{
								generator.Emit(OpCodes.Ldtoken, propInfo.PropertyType);
							}

							generator.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle", new Type[1] { typeof(RuntimeTypeHandle) }));
							//class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)

							generator.Emit(OpCodes.Callvirt, typeof(System.Data.DataColumnCollection).GetMethod("Add", new Type[2] { typeof(string), typeof(System.Type) }));
							//instance class [System.Data]System.Data.DataColumn [System.Data]System.Data.DataColumnCollection::Add(string, class [mscorlib]System.Type)

							generator.Emit(OpCodes.Pop);
						}

						// Load new constructed obj on eval stack -> 1 item on stack
						generator.Emit(OpCodes.Ldloc_0);
						generator.Emit(OpCodes.Ret);

						myExec = dymMethod.CreateDelegate(typeof(EmptyDTFunc<T1>));
						EmptyDataTableIL.Add(typeof(T1), myExec);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
				return ((EmptyDTFunc<T1>)myExec)();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return null;
		}
		#endregion IL Code

		#region IBindingList overrides

		bool IBindingList.AllowNew
		{
			get
			{
				return CheckReadOnly();
			}
		}

		bool IBindingList.AllowRemove
		{
			get
			{
				return CheckReadOnly();
			}
		}

		private bool CheckReadOnly()
		{
			if (_sorted || _filtered)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		#endregion

		protected override void InsertItem(int index, T item)
		{
			//foreach (PropertyDescriptor propDesc in TypeDescriptor.GetProperties(item))
			//{
			//    if (propDesc.SupportsChangeEvents)
			//    {
			//        propDesc.AddValueChanged(item, OnItemChanged);
			//    }
			//}
			base.InsertItem(index, item);
		}

		protected override void RemoveItem(int index)
		{
			T item = Items[index];
			PropertyDescriptorCollection propDescs = TypeDescriptor.GetProperties(item);
			foreach (PropertyDescriptor propDesc in propDescs)
			{
				if (propDesc.SupportsChangeEvents)
				{
					propDesc.RemoveValueChanged(item, OnItemChanged);
				}
			}
			base.RemoveItem(index);
		}

		void OnItemChanged(object sender, EventArgs args)
		{
			int index = Items.IndexOf((T)sender);
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
		}

		#region IRaiseItemChangedEvents Members

		bool IRaiseItemChangedEvents.RaisesItemChangedEvents
		{
			get { return true; }
		}

		#endregion

		#region PropertyValueExistsMoreThanOnceInCollection Methods
		/// <summary>
		/// Returns a collection of duplicate items in the collection.
		/// The list comes back presorted in ascending order.
		/// </summary>
		public List<string> PropertyValueExistsMoreThanOnceInCollection(string propertyName)
		{
			return PropertyValueExistsMoreThanOnceInCollection(propertyName, false);
		}

		/// <summary>
		/// Returns a collection of duplicate items in the collection.
		/// </summary>
		public List<string> PropertyValueExistsMoreThanOnceInCollection(string propertyName, bool sortDescending)
		{
			string val = string.Empty;
			List<string> exists = new List<string>();
			List<string> duplicates = new List<string>();
			if (!(typeof(LEAD_BaseObject).IsAssignableFrom(typeof(T))))
			{
				return duplicates;
			}
			foreach (T item in this)
			{
				if ((item as LEAD_BaseObject).RecordStatus != RecordStatus.Deleted)
				{
					val = (string)item.GetType().GetProperty(propertyName).GetValue(item, null);
					if (!String.IsNullOrEmpty(val))
					{
						if (exists.Contains(val))
						{
							if (!duplicates.Contains(val))
							{
								duplicates.Add(val);
							}
						}
						else
						{
							exists.Add(val);
						}
					}
				}
			}
			if (duplicates.Count > 0)		// Sort the list
			{
				if (!sortDescending)
				{
					duplicates.Sort(delegate(string s1, string s2) { return s1.CompareTo(s2); });
				}
				else
				{
					duplicates.Sort(delegate(string s1, string s2) { return s2.CompareTo(s1); });
				}
			}
			return duplicates;
		}
		#endregion PropertyValueExistsMoreThanOnceInCollection Methods

		#region Random Methods
		/// <summary>
		/// Sorts the collection in memory.
		/// </summary>
		public void Sort(string orderBy)
		{
			DynamicComparer<T> comparer = new DynamicComparer<T>(orderBy);
			this.Sort(comparer);
		}

		/// <summary>
		/// Undo all the changes on the collection
		/// </summary>
		public void UndoChanges()
		{
			foreach (T item in this)
			{
				item.UndoChanges();		// Undo the changes on the object
			}
		}

		/// <summary>
		/// Get the record collection based on state
		/// </summary>
		protected ClassGenBindingList<T, K> ActiveRecords
		{
			get 
			{
				Type type = this.GetType();
				ClassGenBindingList<T, K> rtv = (ClassGenBindingList<T, K>)Activator.CreateInstance(type);
				foreach (T item in this)
				{
					if (item.RecordStatus != RecordStatus.Deleted) { rtv.Add(item); }
				}
				return rtv;		// Return the collection
			}
		}

		/// <summary>
		/// Get the record collection based on state
		/// </summary>
		public ClassGenBindingList<T, K> GetRecordsBasedOnStatus(RecordStatus status)
		{
			Type type = this.GetType();
			ClassGenBindingList<T, K> rtv = (ClassGenBindingList<T, K>)Activator.CreateInstance(type);

			switch (status)
			{
				case RecordStatus.Current:
					throw new Exception("Invalid Parameter...To return the Current Recordset, call the ActiveRecords Method.");
				default:
					foreach (T item in this)
					{
						if (item.RecordStatus == status) { rtv.Add(item); }
					}
					break;
			}

			return rtv;		// Return the collection
		}

		
		/// <summary>
		/// Get a dictionary of Key/Values that can be passed back to populate a combo box
		/// </summary>
		/// <typeparam name="TKey">The type of the key to return</typeparam>
		/// <typeparam name="TValue">The type of the value to return</typeparam>
		public Dictionary<TKey, TValue> GetKeyValueDictionary<TKey, TValue>(string keyProperty, string valueProperty)
		{
			Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
			T obj = (T)Activator.CreateInstance(this.GenericBaseType);
			PropertyInfo keyProp = obj.GetType().GetProperty(keyProperty);
			PropertyInfo valueProp = obj.GetType().GetProperty(valueProperty);

			foreach (T item in this)
			{
				if (!dict.ContainsKey((TKey)keyProp.GetValue(item, null)))
				{
					dict.Add((TKey)keyProp.GetValue(item, null), (TValue)valueProp.GetValue(item, null));
				}
			}

			return dict;		// Return the dictionary
		}
		#endregion Random Methods

		#region Clone/Copy Methods
		/// <summary>
		/// Create an exact copy of all the elements in the collection
		/// </summary>
		/// <returns>The cloned collection</returns>
		public virtual ClassGenBindingList<T, K> Clone()
		{
			ClassGenBindingList<T, K> coll = new ClassGenBindingList<T, K>();

			coll.KeyFieldName = this.KeyFieldName;		// Set the key field name
			// Set the foreign key fields
			foreach (string fkField in this.ForeignKeyFields)
			{
				coll.ForeignKeyFields.Add(fkField);
			}
			coll.Rules = new ClassGenRules(this);		// Set the base collection of rules

			foreach (T item in this)
			{
				//coll.Add((T)item.Clone());		// Clone each object individually
				coll.Add(item.CloneObjectWithIL<T>(item));	// Clone each object individually
			}
			return coll;
		}

		/// <summary>
		/// Copy the objects in the collection, creating a new id for each one of them in the process
		/// </summary>
		/// <returns></returns>
		public virtual ClassGenBindingList<T, K> CopyWithNewIDs()
		{
			ClassGenBindingList<T, K> coll = new ClassGenBindingList<T, K>();

			coll.KeyFieldName = this.KeyFieldName;		// Set the key field name
			// Set the foreign key fields
			foreach (string fkField in this.ForeignKeyFields)
			{
				coll.ForeignKeyFields.Add(fkField);
			}
			coll.Rules = new ClassGenRules(this);		// Set the base collection of rules

			foreach (T item in this)
			{
				if (item is IClassGenCopyWithNewID)
				{
					coll.Add(((IClassGenCopyWithNewID)item).CopyWithNewID() as T);		// Copy each object individually
				}
			}
			return coll;
		}
		#endregion Clone/Copy Methods

		#region Properties
		public Type GenericBaseType
		{
			get
			{
				return typeof(T);
			}
		}

		/// <summary>
		/// Tells whether the object has data already loaded in it.
		/// </summary>
		public bool IsLoaded
		{
			get { return _isLoaded; }
			set { _isLoaded = value; }
		}

		/// <summary>
		/// Sets the Refresh rate on the object and/or collection
		/// </summary>
		public RefreshFrequency RefreshRate
		{
			get { return _refreshRate; }
			set { _refreshRate = value; }
		}

		/// <summary>
		/// Tells if the background worker is busy
		/// </summary>
		public bool GetThreadRunning
		{
			get { return (_backgroundWorker.IsBusy); }
		}

		/// <summary>
		/// The Rules collection for the class
		/// </summary>
		public ClassGenRules Rules
		{
			get { return _rules; }
			set { _rules = value; }
		}
		#endregion Properties

		#region Event Handlers for Population of the Collection
		public delegate void MasterPopulateEventHandler(object sender, PopulateUpdateDelete_EventArgs e);
		public event MasterPopulateEventHandler MasterPopulate;
		protected void OnMasterPopulate(string name, string description, string errors)
		{
			if (MasterPopulate != null)
			{
				PopulateUpdateDelete_EventArgs e = new PopulateUpdateDelete_EventArgs(name, description, errors, 0, 0);
				MasterPopulate(this, e);
			}
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
		#endregion Event Handlers for Population of the Collection

		#region Event Handlers for Updating of the Collection
		public delegate void MasterUpdateEventHandler(object sender, PopulateUpdateDelete_EventArgs e);
		public event MasterUpdateEventHandler MasterUpdate;
		protected void OnMasterUpdate(string name, string description, string errors)
		{
			if (MasterUpdate != null)
			{
				PopulateUpdateDelete_EventArgs e = new PopulateUpdateDelete_EventArgs(name, description, errors, 0, 0);
				MasterUpdate(this, e);
			}
		}

		public delegate void DetailUpdateEventHandler(object sender, PopulateUpdateDelete_EventArgs e);
		public event DetailUpdateEventHandler DetailUpdate;
		protected void OnDetailUpdate(string name, string description, string errors, int totalCount, int currentIndex)
		{
			if (DetailUpdate != null)
			{
				PopulateUpdateDelete_EventArgs e = new PopulateUpdateDelete_EventArgs(name, description, errors, totalCount, currentIndex);
				DetailUpdate(this, e);
			}
		}
		#endregion Event Handlers for Updating of the Collection

		#region Event Handlers for Deletion of the Collection
		public delegate void MasterDeleteEventHandler(object sender, PopulateUpdateDelete_EventArgs e);
		public event MasterDeleteEventHandler MasterDelete;
		protected void OnMasterDelete(string name, string description, string errors)
		{
			if (MasterDelete != null)
			{
				PopulateUpdateDelete_EventArgs e = new PopulateUpdateDelete_EventArgs(name, description, errors, 0, 0);
				MasterDelete(this, e);
			}
		}

		public delegate void DetailDeleteEventHandler(object sender, PopulateUpdateDelete_EventArgs e);
		public event DetailDeleteEventHandler DetailDelete;
		protected void OnDetailDelete(string name, string description, string errors, int totalCount, int currentIndex)
		{
			if (DetailDelete != null)
			{
				PopulateUpdateDelete_EventArgs e = new PopulateUpdateDelete_EventArgs(name, description, errors, totalCount, currentIndex);
				DetailDelete(this, e);
			}
		}
		#endregion Event Handlers for Deletion of the Collection
		
		#region Grid Checked Methods
		/// <summary>
		/// Get the number of checked items in the collection
		/// </summary>
		/// <returns>The number of values that are checked</returns>
		public int CheckedCount
		{
			get
			{
				int count = 0;
				foreach (T item in this)
				{
					if (item.CheckedInGrid) { count++; }
				}
				return count;
			}
		}

		/// <summary>
		/// Check all the values in the collection
		/// </summary>
		public void CheckAll()
		{
			foreach (T item in this)
			{
				item.CheckedInGrid = true;
			}
		}

		/// <summary>
		/// Uncheck all the items in the collection
		/// </summary>
		public void CheckNone()
		{
			foreach (T item in this)
			{
				item.CheckedInGrid = false;
			}
		}

		/// <summary>
		/// Uncheck all the items in the collection
		/// </summary>
		public void ClearChecked()
		{
			CheckNone();
		}

		/// <summary>
		/// Get all the checked items in the collection
		/// </summary>
		/// <returns>The list of items that are checked in the collection</returns>
		public List<T> GetChecked()
		{
			List<T> list = new List<T>();
			foreach (T item in this)
			{
				if (item.CheckedInGrid) { list.Add(item); }
			}
			return list;		// Return the list
		}

		/// <summary>
		/// Get all the unchecked items in the collection
		/// </summary>
		/// <returns>The list of items that are unchecked in the collection</returns>
		public List<T> GetUnchecked()
		{
			List<T> list = new List<T>();
			foreach (T item in this)
			{
				if (!item.CheckedInGrid) { list.Add(item); }
			}
			return list;		// Return the list
		}
		#endregion Grid Checked Methods

		#region ITypedList Members
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) 
		{
			return TypeDescriptor.GetProperties(typeof(T));
		}
		
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors) 
		{
			return typeof(T).ToString();
		}
		#endregion 
	}
	#endregion ClassGenBindingList 
}


