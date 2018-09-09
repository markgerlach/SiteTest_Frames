using System;
using System.Reflection;
using System.Text;
using System.IO;
using System.Data;

namespace LEADBase
{
	//public enum TabKey : int
	//{
	//	Empty = 0,

	//	Dashboard,

	//	PersonnelAAG,
	//	DemographicInformation,
	//}

	public enum ImageType : int
	{	
		Unknown = 0,
		JPG,
		PNG,
		GIF,
		BMP,
	}

	public enum ImageSize : int
	{
		Size_16x16 = 0,
		Size_24x24,
		Size_32x32,
		Size_48x48,
	}

	public enum UserExtendedPropertyKeyEnum : int
	{
		Empty = 0,
		ControlledDocumentAdmin,
		ButtonNav_Employees_EmployeeVacAccrualListing,
		DigitalSignage_DefaultPowerPointDirectory,
		DigitalSignage_DefaultPDFDirectory,
		DigitalSignage_DefaultGeneralDirectory,

		ForgeLoadOpt_OverlayPreviousPoints,
		ForgeLoadOpt_DontShowPrevious,
		ForgeLoadOpt_MarkZeroZero,
		ForgeLoadOpt_ShowXY,
		ForgeLoadOpt_ShowDieRegister,

		//LicensedUsers = 0,
		//CanChangeEmployeeOnTimeRequest,
		//CanEditMandatoryOTRecords,
		//Workforce_ChangeRequestInGrid,
		//Workforce_IndividualPayrollListingView,
	}

	public enum HRApplicantSearchDateType : int
	{
		Empty = 0,
		TempHire,
		FullTimeHire,
		TerminationDate,
		ApplicationDate,
		RejectionDate,
		SeparationDate,
	}

	public enum ForgeCondRateSearchDateType : int
	{
		Empty = 0,
		DateReceived,
		DateEstimatedCompletion,
		DateCompleted,
	}
	
	public enum SecObjectType : int
	{
		FormName = 0,
		Menu,
		NavBar,
		Screen,
	}

	public enum SiemensSecObjectType : int
	{
		Screen = 0,
		//Menu,
		//NavBar,
		//Screen,
	}

	public enum InfoSysIPCategoryEditScreenType : int
	{
		Empty = 0,
		ByIPAddress,
		ByCategory,
	}

	public enum NotesRTFKey : int
	{
		Empty = 0,
		PONote,
		PONoteExt,
		ForgeMetricNote,
		ForgeCondRateNote,
		HRApplicantNote,
	}

	public enum RateLookupKey : int
	{
		Empty = 0,
		SawSetupBar,
		SawSetupCut,
	}

	public enum PrintOrientation : int
	{
		Portrait = 0,
		Landscape
	}

	public enum LeftNavTab
	{
		OutlookNavigation,
		TreeNavigation,
		MyViews,
		QuickViews,
	}

	public enum ssScreenSize : int
	{
		FormFillsClientArea = 0,
		CenterAndSize,
		CascadeFromLastForm,
	}

	public enum ClientFormPosition : int
	{
		TopLeft = 0,
		TopCenter,
		TopRight,
		MiddleLeft,
		MiddleCenter,
		MiddleRight,
		BottomLeft,
		BottomCenter,
		BottomRight,
	}

	public enum ssFilterDataType : int
	{
		Empty = 0,
		SupportDropDown,
		CustomSQL,
		Dictionary,
		List,
	}

	public enum ssFilterBasicType : int
	{
		CheckedList = 0,
		StandardList,
	}

	public enum HRApplicantStatsSearchField : int
	{
		Empty = 0,
		Ethnicity,
		Gender,
		
		StatusCode,
		JobCode,
		RecruiterSupervisor,
		ReferralSource,
		JobCategory,
	}

	public enum ForgeMetricsChartingSearchField : int
	{
		Empty = 0,
		Operation,
		Unit,
		Shift,
		JobNum,
	}

	public enum SawBladeMetricsChartingSearchField : int
	{
		Empty = 0,
		BladeNum,
		ToothType,
		Manufacturer,
		AlloyType,
		MaterialSize,
	}

	public enum SawRunChartSearchField : int
	{
		Empty = 0,
		Material,
		Alloy,
		JobNum,
		HeatNum,
		HeatCode,
		BilletNum,
		Saw,
	}

	public enum ForgeCondRateSearchField : int
	{
		Empty = 0,
		JobNum,

		Operation,
		SizeStage,
		Machine,
		MaterialType,
	}

	public enum EncryptedField : int
	{
		Password = 0,
		SSN,
		FirstName,
		LastName,
	}

	public enum EncryptAction : int
	{
		Encrypt = 0,
		Decrypt,
	}

	public enum QVType : int
	{
		// Standard Views - used by the ExecuteView Code
		Empty = 0,
	}

	public enum ActionType : int
	{
		Empty = 0,
		Login,
		Logout,
		ApplicationClose,
		Error,
		Information,

		// Run System utilities
		RunSystemUtils,

		// Standard CRUD entries
		Create,
		Read,
		Update,
		Delete,

		// Digital Signage Actions
		DigitalSignage_StartPackage,
		DigitalSignage_RefreshContent,

		// Other actions
		OpenScreen,
		EmailSent,
	}

	public enum SearchExControlType : int
	{
		Empty = 0,
		CheckBox,
		ComboBoxEdit,
		LookupEdit,
		GridLookupEdit,
		DateRange,
		TextBox,
		ListBox,
		UserControl,
		ucLookupEdit,
		DateTime,
		RadioGroup,
	}

	public enum SearchExTypes : int
	{
		Empty = 0,
		Basic,
		Advanced,
		Favorites,
		None,
	}

	[Flags]
	public enum SearchActionType : int
	{
		None = 0,
		Print = 1,
		Edit = 2,
		Archive = 4,
		Delete = 8,
		//BackgroundAddToList = 16,
		//BackgroundRemoveFromList = 32,
		//CopyReportTemplate = 64,
	}

	public enum SearchMatchType : int
	{
		None = 0,
		StartsWith,
		EndsWith,
		Contains,
		ExactMatch,
	}

	public enum GridDisplayFormat : int
	{
		None = 0,
		SSN,
		PhoneNumber,
		Currency,
		CurrencyNoRounding,
		ConciseDateTime,
		ConciseDate,
		DateTime,
		FullDateTime,
		Date,
		Time,
		Decimal1Place,
		Decimal2Places,
		Decimal3Places,
		Decimal4Places,
		Decimal1PlaceWithPlaceholder,
		Decimal2PlacesWithPlaceholder,
		Decimal3PlacesWithPlaceholder,
		Decimal4PlacesWithPlaceholder,
		Integer,
		Percentage,
		Percentage0Decimal,
		Percentage1Decimal,
		Percentage2Decimals,
		Percentage3Decimals,
		Percentage4Decimals,
		Multiline,
		RTFMultiline,
	}

	public enum AppEntryComputerPropertyName : int
	{
		Empty = 0,
		MotionStudyMachineShopMachineName,
		MotionStudyForgeMachineName,
	}

	public enum SystemSettingDropDown : int
	{
		Empty = 0,

		//Gender,

		//HelpDeskPriority,
		//HelpDeskUrgency,

		//HR_EEOCode,
		//HR_JobCode,
		//HR_JobCodeNoDesc,
		//HR_Ethnicity,
		//HR_ReferralSource,

		//HRAbsenceReason,

		//ScreenOption,

		//DEFORM_SimulationType,		

		//ForgeMetricReason,
		//ForgeCondRateReason,
		//ForgeMetricDieSet,
		//ForgeCondRateMachine,
	}
}

