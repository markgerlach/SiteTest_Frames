using System;
using System.Reflection;
using System.Text;
using System.IO;
using System.Data;

namespace LEADBase
{
	[Serializable]
	public enum SystemEventType : int
	{
		LoginSuccess = 0,
		LoginFailure,
		LoginSystemError,

		SessionError,

		GridFormattingError,
		PrefCleanup,
	}

	[Serializable]
	public enum RefreshFrequency : int
    {
        ByDeveloperOnly = 2000000000,
        Every01Minute = 1,
        Every02Minutes = 2,
        Every03Minutes = 3,
        Every04Minutes = 4,
        Every05Minutes = 5,
        Every06Minutes = 6,
        Every07Minutes = 7,
        Every08Minutes = 8,
        Every09Minutes = 9,
        Every10Minutes = 10,
        Every15Minutes = 15,
        Every20Minutes = 20,
        Every30Minutes = 30,
        Every45Minutes = 45,
        Every60Minutes = 60,
    }

	[Serializable]
	public enum BrokenRuleType : int
	{
		Empty = 0,
		SimpleCustomRule,
		PropertyRequiredCustomRule,
		MinValueCustomRule,
		MaxValueCustomRule,
		MinMaxValueCustomRule,
		MinLengthCustomRule,
		MaxLengthCustomRule,
		DuplicateInCollectionCustomRule,
		Value1MustBeLessThanValue2CustomRule,
		RegExCustomRule,
	}

	[Serializable]
	[Flags]
	public enum RecordStatus : int
    {
        Current = 1,
        Modified = 2,
        Deleted = 4,
        New = 8,
    }

	[Serializable]
	public enum ClassGenExceptionIconType : int
	{
		None = 0,
		Default,
		Critical,
		Warning,
		Information,
		User1,
		User2,
		User3,
		System,
	}

	[Serializable]
	public enum ClassGenExceptionType : int
	{
		Empty = 0,
		Exception,
		SQLException,
		TextOnly,
		BrokenRule,
	}

	
}

