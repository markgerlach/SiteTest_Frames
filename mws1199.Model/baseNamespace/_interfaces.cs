using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace LEADBase
{
	#region Interface: IUniqueGUID
	public interface IUniqueGUID
	{
		Guid UniqueGUID { get; }
	}
	#endregion Interface: IUniqueGUID

	#region Interface: IClassGenILCode
	public interface IClassGenILCode
	{
		DataTable EmptyDataTableWithIL<T>();
	}
	#endregion Interface: IClassGenILCode

	#region Interface: IClassGenCloneable
	public interface IClassGenCloneable
	{
		T CloneObjectWithIL<T>(T myObject);
		DataRow ToDataRowWithIL<T>(T myObject, DataTable dt);
	}
	#endregion Interface: IClassGenCloneable

	#region Interface: IClassGenCopyWithNewID
	public interface IClassGenCopyWithNewID
	{
		object CopyWithNewID();
	}
	#endregion Interface: IClassGenCopyWithNewID

	#region Interface: IClassGenLogXML
	public interface IClassGenLogXML
	{
		string GetChangedElementsAsXML();
		string GetAsXML();
		string GetAsXML(bool includeBaseProperties);
	}
	#endregion Interface: IClassGenLogXML

	#region Interface: IClassGenLogXMLCollection
	public interface IClassGenLogXMLCollection
	{
		void GetXMLChanges(ref List<string> newElements,
			ref List<string> readElements,
			ref List<string> changedElements,
			ref List<string> deletedElements);
		string GetAsXML();
		string GetAsXML(bool includeBaseProperties);
	}
	#endregion Interface: IClassGenLogXMLCollection

	#region Interface: IClassGenRecordStatus
	public interface IClassGenRecordStatus
	{
		RecordStatus RecordStatus { get; set; }
	}
	#endregion Interface: IClassGenRecordStatus

	#region Interface: IClassGenClassGenerated
	public interface IClassGenClassGenerated
	{
		// Properties
		bool IsLoaded { get; set; }
		RefreshFrequency RefreshRate { get; set; }
		bool GetThreadRunning { get; }
		ClassGenRules Rules { get; set; }

		ClassGenExceptionCollection BrokenRules { get; }

		// Methods
		List<string> PropertyValueExistsMoreThanOnceInCollection(string propertyName);
		List<string> PropertyValueExistsMoreThanOnceInCollection(string propertyName, bool sortDescending);

		ClassGenExceptionCollection GetFromDBWithChildren();
		ClassGenExceptionCollection GetFromDB(string whereClause);
		ClassGenExceptionCollection GetFromDB(string whereClause, bool getChildren);
		ClassGenExceptionCollection GetFromDB();

		ClassGenExceptionCollection GetFromDB(string whereClause, ref long dbCount);
		ClassGenExceptionCollection GetFromDB(string whereClause, bool getChildren, ref long dbCount);
		ClassGenExceptionCollection GetFromDB(ref long dbCount);

		//long GetCountFromDB();
		//long GetCountFromDB(string whereClause);

		//int CheckedCount();
		int CheckedCount { get; }
		void CheckAll();
		void CheckNone();
		void ClearChecked();

		ClassGenExceptionCollection RefreshCollection(int threshHoldInMinutes);
		ClassGenExceptionCollection RefreshCollection();
		ClassGenExceptionCollection RefreshForce();

		DataTable ToDataTable();
		DataTable ToDataTable(bool includeDeleted);
		DataTable ToEmptyDataTable();

		void GetFromDBThreaded(int pageSize, string whereClause);
		void GetFromDBThreaded(string whereClause);
		void GetFromDBThreaded(int pageSize);
		void GetFromDBThreaded();
		void GetFromDBThreadedStop();
	}
	#endregion Interface: IClassGenClassGenerated

	#region Interface: IClassGenClassUpdatable
	public interface IClassGenClassUpdatable
	{
		// Methods
		ClassGenExceptionCollection AddUpdateAll();

		void UndoChanges();
	}
	#endregion Interface: IClassGenClassUpdatable
}

