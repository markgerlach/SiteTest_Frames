using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace LEADBase
{
	#region ClassGenRules
	[Serializable]
	public class ClassGenRules : List<CustomRule>
	{
		private object _baseObject = null;
		private IList _baseObjectCollection = null;
	
		public event PropertyChangedEventHandler PropertyChanged;		

		// Default Constructor
		public ClassGenRules(object baseObject)
		{
			// Set the base object for the rules collection
			_baseObject = baseObject;

			// Add a default set of rules
			this.AddRange(this.CreateRules());
		}

		// Default Constructor
		public ClassGenRules(IList baseObjectCollection)
		{
			// Set the base object collection for the rules collection
			_baseObjectCollection = baseObjectCollection;

			// Add a default set of rules
			this.AddRange(this.CreateRules());
		}

		// Default private constructor to allow serialization
		public ClassGenRules()
		{
		}

        /// <summary>
		/// Gets a value indicating whether or not this domain object is valid. 
		/// </summary>
		public virtual bool IsValid
		{
			get { return this.GetBrokenRules().Count == 0; }
		}

		/// <summary>
		/// Validates all rules on this domain object, returning a list of the broken rules.
		/// </summary>
		/// <returns>A read-only collection of rules that have been broken.</returns>
		private System.Collections.ObjectModel.ReadOnlyCollection<CustomRule> GetBrokenRules()
		{
			return GetBrokenRules(string.Empty);
		}

		/// <summary>
		/// Validates all rules on this domain object for a given property, returning a list of the broken rules.
		/// </summary>
		/// <param name="property">The name of the property to check for. If null or empty, all rules will be checked.</param>
		/// <returns>A read-only collection of rules that have been broken.</returns>
		private System.Collections.ObjectModel.ReadOnlyCollection<CustomRule> GetBrokenRules(string property)
		{
			property = CleanString(property);

			List<CustomRule> broken = new List<CustomRule>();

			foreach (CustomRule r in this)
			{
				// Ensure we only validate a rule 
				if (String.IsNullOrEmpty(property) ||
					r.PropertyName == property)
				{
					bool isRuleBroken = (_baseObject != null ? !r.ValidateRule(_baseObject) : !r.ValidateRule(_baseObjectCollection));
					if (isRuleBroken)
					{
						broken.Add(r);
					}
				}
			}

			return broken.AsReadOnly();
		}

		/// <summary>
		/// Returns a broken rules collection
		/// </summary>
		public ClassGenExceptionCollection BrokenRules
		{
			get
			{
				System.Collections.ObjectModel.ReadOnlyCollection<CustomRule> brokenRules = GetBrokenRules();
				ClassGenExceptionCollection errors = new ClassGenExceptionCollection();
				foreach (CustomRule rule in brokenRules)
				{
					ClassGenException err = new ClassGenException();
					err.ClassGenExceptionType = ClassGenExceptionType.BrokenRule;
					err.BrokenRuleType = rule.BrokenRuleType;
					err.Description = rule.Description;
					err.PropertyName = rule.PropertyName;
					err.ClassGenExceptionIconType = (ClassGenExceptionIconType)Enum.Parse(typeof(ClassGenExceptionIconType), rule.ClassGenExceptionIconTypeDX, true);
					errors.Add(err);
				}
				return errors;
			}
		}

		/// <summary>
		/// Return the validation errors as an MG Error Collection
		/// </summary>
		/// <returns></returns>
		public ClassGenExceptionCollection ValidationErrors()
		{
			return this.BrokenRules;
		}

		/// <summary>
		/// Validate the data on the form
		/// </summary>
		/// <returns>True if succcessfully validated, otherwise, false</returns>
		public bool ValidateData()
		{
			return (this.BrokenRules.Count > 0);
		}

		/// <summary>
		/// Override this method to create your own rules to validate this business object. These rules must all be met before 
		/// the business object is considered valid enough to save to the data store.
		/// </summary>
		/// <returns>A collection of rules to add for this business object.</returns>
		protected virtual List<CustomRule> CreateRules()
		{
			return new List<CustomRule>();
		}

		/// <summary>
		/// A helper method that raises the PropertyChanged event for a property.
		/// </summary>
		/// <param name="propertyNames">The names of the properties that changed.</param>
		protected virtual void NotifyChanged(params string[] propertyNames)
		{
			foreach (string name in propertyNames)
			{
				OnPropertyChanged(new PropertyChangedEventArgs(name));
			}
			OnPropertyChanged(new PropertyChangedEventArgs("IsValid"));
		}

		/// <summary>
		/// Cleans a string by ensuring it isn't null and trimming it.
		/// </summary>
		/// <param name="s">The string to clean.</param>
		protected string CleanString(string s)
		{
			return (s ?? string.Empty).Trim();
		}

		/// <summary>
		/// Raises the PropertyChanged event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, e);
			}
		}

		/// <summary>
		/// Use this method to set any additional rules
		/// </summary>
		protected virtual void SetAdditionalRules()
		{
		}
	}
	#endregion ClassGenRules

	#region CustomRule
	/// <summary>
    /// An abstract class that contains information about a rule as well as a method to validate it.
    /// </summary>
    /// <remarks>
    /// This class is primarily designed to be used on a domain object to validate a business rule. In most cases, you will want to use the 
    /// concrete class SimpleRule, which just needs you to supply a delegate used for validation. For custom, complex business rules, you can 
    /// extend this class and provide your own method to validate the rule.
    /// </remarks>
	[Serializable]
	[System.Xml.Serialization.XmlInclude(typeof(SimpleCustomRule))]
	[System.Xml.Serialization.XmlInclude(typeof(PropertyRequiredCustomRule))]
	[System.Xml.Serialization.XmlInclude(typeof(MinLengthCustomRule))]
	[System.Xml.Serialization.XmlInclude(typeof(MaxLengthCustomRule))]
	[System.Xml.Serialization.XmlInclude(typeof(DuplicateInCollectionRule))]
	[System.Xml.Serialization.XmlInclude(typeof(RegexCustomRule))]
    public abstract class CustomRule
    {
        private string _description;
        private string _propertyName;
        private ClassGenExceptionIconType _errorIconType;
		private RecordStatus _onlyRunRuleForRecordStatus = 0;
		private BrokenRuleType _ruleType = BrokenRuleType.Empty;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="propertyName">The name of the property the rule is based on. This may be blank if the rule is not for any specific property.</param>
        /// <param name="brokenDescription">A description of the rule that will be shown if the rule is broken.</param>
        public CustomRule(string propertyName, string brokenDescription)
        {
            this.Description = brokenDescription;
            this.PropertyName = propertyName;
            this.ClassGenExceptionIconType = ClassGenExceptionIconType.Default;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="propertyName">The name of the property the rule is based on. This may be blank if the rule is not for any specific property.</param>
        /// <param name="brokenDescription">A description of the rule that will be shown if the rule is broken.</param>
        /// <param name="errorIconType">The Error icon type to show with the error</param>
		public CustomRule(string propertyName, string brokenDescription, ClassGenExceptionIconType ClassGenExceptionIconType)
        {
            this.Description = brokenDescription;
            this.PropertyName = propertyName;
            this.ClassGenExceptionIconType = ClassGenExceptionIconType;
        }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="propertyName">The name of the property the rule is based on. This may be blank if the rule is not for any specific property.</param>
		/// <param name="brokenDescription">A description of the rule that will be shown if the rule is broken.</param>
		/// <param name="onlyRunRuleForRecordStatus">Only run for the record statusses passed</param>
		public CustomRule(string propertyName, string brokenDescription, RecordStatus onlyRunRuleForRecordStatus)
		{
			this.Description = brokenDescription;
			this.PropertyName = propertyName;
			this.OnlyRunRuleForRecordStatus = onlyRunRuleForRecordStatus;
			this.ClassGenExceptionIconType = ClassGenExceptionIconType.Default;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="propertyName">The name of the property the rule is based on. This may be blank if the rule is not for any specific property.</param>
		/// <param name="brokenDescription">A description of the rule that will be shown if the rule is broken.</param>
		/// <param name="ClassGenExceptionIconType">The Error icon type to show with the error</param>
		/// <param name="onlyRunRuleForRecordStatus">Only run for the record statusses passed</param>
		public CustomRule(string propertyName, string brokenDescription, ClassGenExceptionIconType ClassGenExceptionIconType, RecordStatus onlyRunRuleForRecordStatus)
		{
			this.Description = brokenDescription;
			this.PropertyName = propertyName;
			this.OnlyRunRuleForRecordStatus = onlyRunRuleForRecordStatus;
			this.ClassGenExceptionIconType = ClassGenExceptionIconType;
		}

        // Default private constructor to allow serialization
		public CustomRule()
		{
		}

		/// <summary>
		/// The type of rule
		/// </summary>
		public virtual BrokenRuleType BrokenRuleType
		{
			get { return _ruleType; }
			set { _ruleType = value; }
		}

        /// <summary>
        /// Gets descriptive text about this broken rule.
        /// </summary>
        public virtual string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Gets the name of the property the rule belongs to.
        /// </summary>
        public virtual string PropertyName
        {
            get { return (_propertyName ?? string.Empty).Trim(); }
            set { _propertyName = value; }
        }

        /// <summary>
        /// Gets the Error Icon Type
        /// </summary>
        public virtual ClassGenExceptionIconType ClassGenExceptionIconType
        {
            get { return _errorIconType; }
            set { _errorIconType = value; }
        }

		/// <summary>
		/// Only run the validation if the recordstatus matches one of the passed types
		/// </summary>
		public virtual RecordStatus OnlyRunRuleForRecordStatus
		{
			get { return _onlyRunRuleForRecordStatus; }
			set { _onlyRunRuleForRecordStatus = value; }
		}

        /// <summary>
        /// Gets the Error Icon Type as a DX Constant
        /// </summary>
        public virtual string ClassGenExceptionIconTypeDX
        {
            get
            {
                string rtv = string.Empty;
                switch (_errorIconType)
                {
                    case ClassGenExceptionIconType.Warning:
                        rtv = "Warning";
                        break;
                    case ClassGenExceptionIconType.Information:
                        rtv = "Information";
                        break;
                    case ClassGenExceptionIconType.User1:
                        rtv = "User1";
                        break;
                    case ClassGenExceptionIconType.User2:
                        rtv = "User2";
                        break;
                    case ClassGenExceptionIconType.User3:
                        rtv = "User3";
                        break;
					case ClassGenExceptionIconType.Critical:
                    case ClassGenExceptionIconType.Default:
					default:
                        rtv = "Critical";
                        break;
                }
                return rtv;
            }
        }

        /// <summary>
        /// Validates that the rule has been followed.
        /// </summary>
        public abstract bool ValidateRule(object baseObject);

		/// <summary>
		/// Validates that the rule has been followed.
		/// </summary>
		public abstract bool ValidateRule(IList baseObjectCollection);

		/// <summary>
		/// Check to see if we have a record status match on the object to perform the validation
		/// </summary>
		/// <param name="baseObject">The object to eval</param>
		/// <param name="setRecordStatus">The record status set on the rule</param>
		/// <returns>True if there is a match and the rule should run, otherwise, false</returns>
		protected internal virtual bool RecordStatusMatch(object baseObject, RecordStatus setRecordStatus)
		{
			bool rtv = false;
			if (setRecordStatus > 0 &&
				baseObject is IClassGenRecordStatus)
			{
				// Check the record status
				if ((((int)setRecordStatus & (int)RecordStatus.Current) == (int)RecordStatus.Current) &&
					(((int)((IClassGenRecordStatus)baseObject).RecordStatus & (int)RecordStatus.Current) == (int)RecordStatus.Current))
				{
					rtv = true;
				}
				if ((((int)setRecordStatus & (int)RecordStatus.Modified) == (int)RecordStatus.Modified) &&
					(((int)((IClassGenRecordStatus)baseObject).RecordStatus & (int)RecordStatus.Modified) == (int)RecordStatus.Modified))
				{
					rtv = true;
				}
				if ((((int)setRecordStatus & (int)RecordStatus.New) == (int)RecordStatus.New) &&
					(((int)((IClassGenRecordStatus)baseObject).RecordStatus & (int)RecordStatus.New) == (int)RecordStatus.New))
				{
					rtv = true;
				}
				if ((((int)setRecordStatus & (int)RecordStatus.Deleted) == (int)RecordStatus.Deleted) &&
					(((int)((IClassGenRecordStatus)baseObject).RecordStatus & (int)RecordStatus.Deleted) == (int)RecordStatus.Deleted))
				{
					rtv = true;
				}
			}
			return rtv;
		}

        /// <summary>
        /// Gets a string representation of this rule.
        /// </summary>
        /// <returns>A string containing the description of the rule.</returns>
        public override string ToString()
        {
            return this.Description;
        }

        /// <summary>
        /// Serves as a hash function for a particular type. System.Object.GetHashCode()
        /// is suitable for use in hashing algorithms and data structures like a hash
        /// table.
        /// </summary>
        /// <returns>A hash code for the current rule.</returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
	}
	#endregion CustomRule

	#region SimpleCustomRule
	/// <summary>
    /// A simple type of domain object rule that uses a delegate for validation. 
    /// </summary>
    /// <returns>True if the rule has been followed, or false if it has been broken.</returns>
    /// <remarks>
    /// Usage:
    /// <code>
    ///     this.Rules.Add(new SimpleRule("Name", "The customer name must be at least 5 letters long.", delegate { return this.Name &gt; 5; } ));
    /// </code>
    /// </remarks>
    public delegate bool SimpleCustomRuleDelegate();

    /// <summary>
    /// A class to define a simple rule, using a delegate for validation.
    /// </summary>
	[Serializable]
	public class SimpleCustomRule : CustomRule
    {
        private SimpleCustomRuleDelegate _ruleDelegate;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="propertyName">The name of the property this rule validates for. This may be blank.</param>
        /// <param name="brokenDescription">A description message to show if the rule has been broken.</param>
        /// <param name="ruleDelegate">A delegate that takes no parameters and returns a boolean value, used to validate the rule.</param>
        public SimpleCustomRule(string propertyName, string brokenDescription, SimpleCustomRuleDelegate ruleDelegate)
            :
            base(propertyName, brokenDescription)
        {
			this.BrokenRuleType = BrokenRuleType.SimpleCustomRule;
            this.RuleDelegate = ruleDelegate;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="propertyName">The name of the property this rule validates for. This may be blank.</param>
        /// <param name="brokenDescription">A description message to show if the rule has been broken.</param>
        /// <param name="errorIconType">The Error Icon Type that you want to use as part of the error</param>
        /// <param name="ruleDelegate">A delegate that takes no parameters and returns a boolean value, used to validate the rule.</param>
		public SimpleCustomRule(string propertyName, string brokenDescription, ClassGenExceptionIconType ClassGenExceptionIconType, SimpleCustomRuleDelegate ruleDelegate)
            :
            base(propertyName, brokenDescription, ClassGenExceptionIconType)
        {
			this.BrokenRuleType = BrokenRuleType.SimpleCustomRule;
            this.RuleDelegate = ruleDelegate;
        }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="propertyName">The name of the property this rule validates for. This may be blank.</param>
		/// <param name="brokenDescription">A description message to show if the rule has been broken.</param>
		/// <param name="ruleDelegate">A delegate that takes no parameters and returns a boolean value, used to validate the rule.</param>
		/// <param name="onlyRunRuleForRecordStatus">Only run for the record statusses passed</param>
		public SimpleCustomRule(string propertyName, string brokenDescription, SimpleCustomRuleDelegate ruleDelegate, RecordStatus onlyRunRuleForRecordStatus)
			:
			base(propertyName, brokenDescription, onlyRunRuleForRecordStatus)
		{
			this.BrokenRuleType = BrokenRuleType.SimpleCustomRule;
			this.RuleDelegate = ruleDelegate;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="propertyName">The name of the property this rule validates for. This may be blank.</param>
		/// <param name="brokenDescription">A description message to show if the rule has been broken.</param>
		/// <param name="errorIconType">The Error Icon Type that you want to use as part of the error</param>
		/// <param name="ruleDelegate">A delegate that takes no parameters and returns a boolean value, used to validate the rule.</param>
		/// <param name="onlyRunRuleForRecordStatus">Only run for the record statusses passed</param>
		public SimpleCustomRule(string propertyName, string brokenDescription, ClassGenExceptionIconType ClassGenExceptionIconType, SimpleCustomRuleDelegate ruleDelegate, RecordStatus onlyRunRuleForRecordStatus)
			:
			base(propertyName, brokenDescription, ClassGenExceptionIconType, onlyRunRuleForRecordStatus)
		{
			this.BrokenRuleType = BrokenRuleType.SimpleCustomRule;
			this.RuleDelegate = ruleDelegate;
		}

        // Default private constructor to allow serialization
		public SimpleCustomRule()
		{
			this.BrokenRuleType = BrokenRuleType.SimpleCustomRule;
		}

        /// <summary>
        /// Gets or sets the delegate used to validate this rule.
        /// </summary>
		protected virtual SimpleCustomRuleDelegate RuleDelegate
        {
            get { return _ruleDelegate; }
            set { _ruleDelegate = value; }
        }

        /// <summary>
        /// Validates that the rule has not been broken.
        /// </summary>
        /// <param name="domainObject">The domain object being validated.</param>
        /// <returns>True if the rule has not been broken, or false if it has.</returns>
        public override bool ValidateRule(object baseObject)
        {
            //return RuleDelegate();
			bool rtv = RuleDelegate();
			if (OnlyRunRuleForRecordStatus != 0 &&
				!RecordStatusMatch(baseObject, OnlyRunRuleForRecordStatus))
			{
				rtv = true;
			}
			return rtv;
        }

		public override bool ValidateRule(IList baseObjectCollection)
		{
			return true;
		}
	}
	#endregion SimpleCustomRule

	#region PropertyRequiredCustomRule
	/// <summary>
    /// A rule that states that a value is required for a property
    /// </summary>
    /// <returns>True if the rule has been followed, or false if it has been broken.</returns>
    /// <remarks>
    /// Usage:
    /// <code>
    ///     this.Rules.Add(new PropertyRule("Name", "The customer name must be at least 5 letters long."));
    /// </code>
    /// </remarks>
    //public delegate bool PropertyRequiredRuleDelegate();

    /// <summary>
    /// A class to define a simple rule, using a delegate for validation.
    /// </summary>
	[Serializable]
	public class PropertyRequiredCustomRule : CustomRule
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="propertyName">The name of the property this rule validates for. This may be blank.</param>
        /// <param name="brokenDescription">A description message to show if the rule has been broken.</param>
        public PropertyRequiredCustomRule(string propertyName, string brokenDescription)
            :
            base(propertyName, brokenDescription)
        {
			this.BrokenRuleType = BrokenRuleType.PropertyRequiredCustomRule;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="propertyName">The name of the property this rule validates for. This may be blank.</param>
        /// <param name="brokenDescription">A description message to show if the rule has been broken.</param>
        /// <param name="errorIconType">The Error Icon Type that you want to use as part of the error</param>
		public PropertyRequiredCustomRule(string propertyName, string brokenDescription, ClassGenExceptionIconType ClassGenExceptionIconType)
            :
            base(propertyName, brokenDescription, ClassGenExceptionIconType)
        {
			this.BrokenRuleType = BrokenRuleType.PropertyRequiredCustomRule;
        }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="propertyName">The name of the property this rule validates for. This may be blank.</param>
		/// <param name="brokenDescription">A description message to show if the rule has been broken.</param>
		/// <param name="onlyRunRuleForRecordStatus">Only run for the record statusses passed</param>
		public PropertyRequiredCustomRule(string propertyName, string brokenDescription, RecordStatus onlyRunRuleForRecordStatus)
			:
			base(propertyName, brokenDescription, onlyRunRuleForRecordStatus)
		{
			this.BrokenRuleType = BrokenRuleType.PropertyRequiredCustomRule;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="propertyName">The name of the property this rule validates for. This may be blank.</param>
		/// <param name="brokenDescription">A description message to show if the rule has been broken.</param>
		/// <param name="errorIconType">The Error Icon Type that you want to use as part of the error</param>
		/// <param name="onlyRunRuleForRecordStatus">Only run for the record statusses passed</param>
		public PropertyRequiredCustomRule(string propertyName, string brokenDescription, ClassGenExceptionIconType ClassGenExceptionIconType, RecordStatus onlyRunRuleForRecordStatus)
			:
			base(propertyName, brokenDescription, ClassGenExceptionIconType, onlyRunRuleForRecordStatus)
		{
			this.BrokenRuleType = BrokenRuleType.PropertyRequiredCustomRule;
		}

        // Default private constructor to allow serialization
		public PropertyRequiredCustomRule()
		{
			this.BrokenRuleType = BrokenRuleType.PropertyRequiredCustomRule;
		}

        /// <summary>
        /// Validates that the rule has not been broken.
        /// </summary>
        /// <param name="domainObject">The domain object being validated.</param>
        /// <returns>True if the rule has not been broken, or false if it has.</returns>
        public override bool ValidateRule(object baseObject)
        {
			bool rtv = true;
            PropertyInfo pi = baseObject.GetType().GetProperty(this.PropertyName);
			if (pi != null)
			{
				rtv = (pi.GetValue(baseObject, null) != null &&
					!String.IsNullOrEmpty(pi.GetValue(baseObject, null).ToString()));

				if (OnlyRunRuleForRecordStatus != 0 &&
					!RecordStatusMatch(baseObject, OnlyRunRuleForRecordStatus))
				{
					rtv = true;
				}
			}
			else
			{
				this.Description = "The " + this.PropertyName +
					" property does not exist in object type: " + baseObject.GetType().ToString() + "" +
					Environment.NewLine + this.Description;
				rtv = false;
			}
			return rtv;
        }

		public override bool ValidateRule(IList baseObjectCollection)
		{
			return true;
		}
	}
	#endregion PropertyRequiredCustomRule

	#region MinValueCustomRule
	[Serializable]
	public class MinValueCustomRule<T> : CustomRule
	{
		private T _minValue = default(T);

		public MinValueCustomRule(string propertyName, T minValue, string brokenDescription)
			: base(propertyName, brokenDescription)
		{
			_minValue = minValue;
			this.BrokenRuleType = BrokenRuleType.MinValueCustomRule;
		}

		public MinValueCustomRule(string propertyName, T minValue, string brokenDescription, ClassGenExceptionIconType ClassGenExceptionIconType)
			: base(propertyName, brokenDescription, ClassGenExceptionIconType)
		{
			_minValue = minValue;
			this.BrokenRuleType = BrokenRuleType.MinValueCustomRule;
		}

		public MinValueCustomRule(string propertyName, T minValue, string brokenDescription, RecordStatus onlyRunRuleForRecordStatus)
			: base(propertyName, brokenDescription, onlyRunRuleForRecordStatus)
		{
			_minValue = minValue;
			this.BrokenRuleType = BrokenRuleType.MinValueCustomRule;
		}

		public MinValueCustomRule(string propertyName, T minValue, string brokenDescription, ClassGenExceptionIconType ClassGenExceptionIconType, RecordStatus onlyRunRuleForRecordStatus)
			: base(propertyName, brokenDescription, ClassGenExceptionIconType, onlyRunRuleForRecordStatus)
		{
			_minValue = minValue;
			this.BrokenRuleType = BrokenRuleType.MinValueCustomRule;
		}

		// Default private constructor to allow serialization
		public MinValueCustomRule()
		{
			this.BrokenRuleType = BrokenRuleType.MinValueCustomRule;
		}

		public override bool ValidateRule(object baseObject)
		{
			bool rtv = true;
			PropertyInfo pi = baseObject.GetType().GetProperty(this.PropertyName);
			if (pi != null)
			{
				object value = pi.GetValue(baseObject, null);
				rtv = (pi.GetValue(baseObject, null) == null ||
					(_minValue is IComparable ? ((IComparable)_minValue).CompareTo(value) <= 0 : true));  

				if (OnlyRunRuleForRecordStatus != 0 &&
					!RecordStatusMatch(baseObject, OnlyRunRuleForRecordStatus))
				{
					rtv = true;
				}
			}
			else
			{
				this.Description = "The " + this.PropertyName +
					" property does not exist in object type: " + baseObject.GetType().ToString() + "" +
					Environment.NewLine + this.Description;
				rtv = false;
			}
			return rtv;
		}

		public override bool ValidateRule(IList baseObjectCollection)
		{
			return true;
		}
	}
	#endregion MinValueCustomRule

	#region MaxValueCustomRule
	[Serializable]
	public class MaxValueCustomRule<T> : CustomRule
	{
		private T _maxValue = default(T);

		public MaxValueCustomRule(string propertyName, T maxValue, string brokenDescription)
			: base(propertyName, brokenDescription)
		{
			_maxValue = maxValue;
			this.BrokenRuleType = BrokenRuleType.MaxValueCustomRule;
		}

		public MaxValueCustomRule(string propertyName, T maxValue, string brokenDescription, ClassGenExceptionIconType ClassGenExceptionIconType)
			: base(propertyName, brokenDescription, ClassGenExceptionIconType)
		{
			_maxValue = maxValue;
			this.BrokenRuleType = BrokenRuleType.MaxValueCustomRule;
		}

		public MaxValueCustomRule(string propertyName, T maxValue, string brokenDescription, RecordStatus onlyRunRuleForRecordStatus)
			: base(propertyName, brokenDescription, onlyRunRuleForRecordStatus)
		{
			_maxValue = maxValue;
			this.BrokenRuleType = BrokenRuleType.MaxValueCustomRule;
		}

		public MaxValueCustomRule(string propertyName, T maxValue, string brokenDescription, ClassGenExceptionIconType ClassGenExceptionIconType, RecordStatus onlyRunRuleForRecordStatus)
			: base(propertyName, brokenDescription, ClassGenExceptionIconType, onlyRunRuleForRecordStatus)
		{
			_maxValue = maxValue;
			this.BrokenRuleType = BrokenRuleType.MaxValueCustomRule;
		}

		// Default private constructor to allow serialization
		public MaxValueCustomRule()
		{
			this.BrokenRuleType = BrokenRuleType.MaxValueCustomRule;
		}

		public override bool ValidateRule(object baseObject)
		{
			bool rtv = true;
			PropertyInfo pi = baseObject.GetType().GetProperty(this.PropertyName);
			if (pi != null)
			{
				object value = pi.GetValue(baseObject, null);
				rtv = (pi.GetValue(baseObject, null) == null ||
					(_maxValue is IComparable ? ((IComparable)_maxValue).CompareTo(value) >= 0 : true));

				if (OnlyRunRuleForRecordStatus != 0 &&
					!RecordStatusMatch(baseObject, OnlyRunRuleForRecordStatus))
				{
					rtv = true;
				}
			}
			else
			{
				this.Description = "The " + this.PropertyName +
					" property does not exist in object type: " + baseObject.GetType().ToString() + "" +
					Environment.NewLine + this.Description;
				rtv = false;
			}
			return rtv;
		}

		public override bool ValidateRule(IList baseObjectCollection)
		{
			return true;
		}
	}
	#endregion MaxValueCustomRule

	#region MinMaxValueCustomRule
	[Serializable]
	public class MinMaxValueCustomRule<T> : CustomRule
	{
		private T _minValue = default(T);
		private T _maxValue = default(T);

		public MinMaxValueCustomRule(string propertyName, T minValue, T maxValue, string brokenDescription)
			: base(propertyName, brokenDescription)
		{
			_minValue = minValue;
			_maxValue = maxValue;
			this.BrokenRuleType = BrokenRuleType.MinMaxValueCustomRule;
		}

		public MinMaxValueCustomRule(string propertyName, T minValue, T maxValue, string brokenDescription, ClassGenExceptionIconType ClassGenExceptionIconType)
			: base(propertyName, brokenDescription, ClassGenExceptionIconType)
		{
			_minValue = minValue;
			_maxValue = maxValue;
			this.BrokenRuleType = BrokenRuleType.MinMaxValueCustomRule;
		}

		public MinMaxValueCustomRule(string propertyName, T minValue, T maxValue, string brokenDescription, RecordStatus onlyRunRuleForRecordStatus)
			: base(propertyName, brokenDescription, onlyRunRuleForRecordStatus)
		{
			_minValue = minValue;
			_maxValue = maxValue;
			this.BrokenRuleType = BrokenRuleType.MinMaxValueCustomRule;
		}

		public MinMaxValueCustomRule(string propertyName, T minValue, T maxValue, string brokenDescription, ClassGenExceptionIconType ClassGenExceptionIconType, RecordStatus onlyRunRuleForRecordStatus)
			: base(propertyName, brokenDescription, ClassGenExceptionIconType, onlyRunRuleForRecordStatus)
		{
			_minValue = minValue;
			_maxValue = maxValue;
			this.BrokenRuleType = BrokenRuleType.MinMaxValueCustomRule;
		}

		// Default private constructor to allow serialization
		public MinMaxValueCustomRule()
		{
			this.BrokenRuleType = BrokenRuleType.MinMaxValueCustomRule;
		}

		public override bool ValidateRule(object baseObject)
		{
			bool rtv = true;
			PropertyInfo pi = baseObject.GetType().GetProperty(this.PropertyName);
			if (pi != null)
			{
				object value = pi.GetValue(baseObject, null);
				rtv = (pi.GetValue(baseObject, null) == null ||
					(_maxValue is IComparable ?
					((IComparable)_maxValue).CompareTo(value) >= 0 && ((IComparable)_minValue).CompareTo(value) <= 0 : 
					true));

				if (OnlyRunRuleForRecordStatus != 0 &&
					!RecordStatusMatch(baseObject, OnlyRunRuleForRecordStatus))
				{
					rtv = true;
				}
			}
			else
			{
				this.Description = "The " + this.PropertyName +
					" property does not exist in object type: " + baseObject.GetType().ToString() + "" +
					Environment.NewLine + this.Description;
				rtv = false;
			}
			return rtv;
		}

		public override bool ValidateRule(IList baseObjectCollection)
		{
			return true;
		}
	}
	#endregion MinMaxValueCustomRule

	#region MinLengthCustomRule
	[Serializable]
	public class MinLengthCustomRule : CustomRule
	{
		private int _minLength = 0;

		public MinLengthCustomRule(string propertyName, int minLength, string brokenDescription) : base(propertyName, brokenDescription)
		{
			_minLength = minLength;
			this.BrokenRuleType = BrokenRuleType.MinLengthCustomRule;
		}

		public MinLengthCustomRule(string propertyName, int minLength, string brokenDescription, ClassGenExceptionIconType ClassGenExceptionIconType)
			: base(propertyName, brokenDescription, ClassGenExceptionIconType)
		{
			_minLength = minLength;
			this.BrokenRuleType = BrokenRuleType.MinLengthCustomRule;
		}

		public MinLengthCustomRule(string propertyName, int minLength, string brokenDescription, RecordStatus onlyRunRuleForRecordStatus)
			: base(propertyName, brokenDescription, onlyRunRuleForRecordStatus)
		{
			_minLength = minLength;
			this.BrokenRuleType = BrokenRuleType.MinLengthCustomRule;
		}

		public MinLengthCustomRule(string propertyName, int minLength, string brokenDescription, ClassGenExceptionIconType ClassGenExceptionIconType, RecordStatus onlyRunRuleForRecordStatus)
			: base(propertyName, brokenDescription, ClassGenExceptionIconType, onlyRunRuleForRecordStatus)
		{
			_minLength = minLength;
			this.BrokenRuleType = BrokenRuleType.MinLengthCustomRule;
		}

		// Default private constructor to allow serialization
		public MinLengthCustomRule()
		{
			this.BrokenRuleType = BrokenRuleType.MinLengthCustomRule;
		}

        public override bool ValidateRule(object baseObject)
		{
			bool rtv = true;
			PropertyInfo pi = baseObject.GetType().GetProperty(this.PropertyName);
			if (pi != null)
			{
				rtv = (pi.GetValue(baseObject, null) == null ||
					pi.GetValue(baseObject, null).ToString().Trim().Length >= _minLength);
				if (OnlyRunRuleForRecordStatus != 0 &&
					!RecordStatusMatch(baseObject, OnlyRunRuleForRecordStatus))
				{
					rtv = true;
				}
			}
			else
			{
				this.Description = "The " + this.PropertyName +
					" property does not exist in object type: " + baseObject.GetType().ToString() + "" +
					Environment.NewLine + this.Description;
				rtv = false;
			}
			return rtv;
		}

		public override bool ValidateRule(IList baseObjectCollection)
		{
			return true;
		}
	}
	#endregion MinLengthCustomRule

	#region MaxLengthCustomRule
	[Serializable]
	public class MaxLengthCustomRule : CustomRule
	{
		private int _maxLength = 0;

		public MaxLengthCustomRule(string propertyName, int maxLength, string brokenDescription)
			: base(propertyName, brokenDescription)
		{
			_maxLength = maxLength;
			this.BrokenRuleType = BrokenRuleType.MaxLengthCustomRule;
		}

		public MaxLengthCustomRule(string propertyName, int maxLength, string brokenDescription, ClassGenExceptionIconType ClassGenExceptionIconType)
			: base(propertyName, brokenDescription, ClassGenExceptionIconType)
		{
			_maxLength = maxLength;
			this.BrokenRuleType = BrokenRuleType.MaxLengthCustomRule;
		}

		public MaxLengthCustomRule(string propertyName, int maxLength, string brokenDescription, RecordStatus onlyRunRuleForRecordStatus)
			: base(propertyName, brokenDescription, onlyRunRuleForRecordStatus)
		{
			_maxLength = maxLength;
			this.BrokenRuleType = BrokenRuleType.MaxLengthCustomRule;
		}

		public MaxLengthCustomRule(string propertyName, int maxLength, string brokenDescription, ClassGenExceptionIconType ClassGenExceptionIconType, RecordStatus onlyRunRuleForRecordStatus)
			: base(propertyName, brokenDescription, ClassGenExceptionIconType, onlyRunRuleForRecordStatus)
		{
			_maxLength = maxLength;
			this.BrokenRuleType = BrokenRuleType.MaxLengthCustomRule;
		}

		// Default private constructor to allow serialization
		public MaxLengthCustomRule()
		{
			this.BrokenRuleType = BrokenRuleType.MaxLengthCustomRule;
		}

        public override bool ValidateRule(object baseObject)
		{
			bool rtv = true;
            PropertyInfo pi = baseObject.GetType().GetProperty(this.PropertyName);
			if (pi != null)
			{
				rtv = (pi.GetValue(baseObject, null) == null ||
					pi.GetValue(baseObject, null).ToString().Trim().Length <= _maxLength);

				if (OnlyRunRuleForRecordStatus != 0 &&
					!RecordStatusMatch(baseObject, OnlyRunRuleForRecordStatus))
				{
					rtv = true;
				}
			}
			else
			{
				this.Description = "The " + this.PropertyName +
					" property does not exist in object type: " + baseObject.GetType().ToString() + "" +
					Environment.NewLine + this.Description;
				rtv = false;
			}
			return rtv;
		}

		public override bool ValidateRule(IList baseObjectCollection)
		{
			return true;
		}
	}
	#endregion MaxLengthCustomRule

	#region DuplicateInCollectionRule
	[Serializable]
	public class DuplicateInCollectionRule : CustomRule
	{
		public DuplicateInCollectionRule(string propertyName, string brokenDescription)
			: base(propertyName, brokenDescription)
		{
			this.BrokenRuleType = BrokenRuleType.DuplicateInCollectionCustomRule;
		}

		public DuplicateInCollectionRule(string propertyName, string brokenDescription, ClassGenExceptionIconType ClassGenExceptionIconType)
			: base(propertyName, brokenDescription, ClassGenExceptionIconType)
		{
			this.BrokenRuleType = BrokenRuleType.DuplicateInCollectionCustomRule;
		}

		public DuplicateInCollectionRule(string propertyName, string brokenDescription, RecordStatus onlyRunRuleForRecordStatus)
			: base(propertyName, brokenDescription, onlyRunRuleForRecordStatus)
		{
			this.BrokenRuleType = BrokenRuleType.DuplicateInCollectionCustomRule;
		}

		public DuplicateInCollectionRule(string propertyName, string brokenDescription, ClassGenExceptionIconType ClassGenExceptionIconType, RecordStatus onlyRunRuleForRecordStatus)
			: base(propertyName, brokenDescription, ClassGenExceptionIconType, onlyRunRuleForRecordStatus)
		{
			this.BrokenRuleType = BrokenRuleType.DuplicateInCollectionCustomRule;
		}

		// Default private constructor to allow serialization
		public DuplicateInCollectionRule()
		{
			this.BrokenRuleType = BrokenRuleType.DuplicateInCollectionCustomRule;
		}

        public override bool ValidateRule(object baseObject)
		{
			return true;
		}

		public override bool ValidateRule(IList baseObjectCollection)
		{
			bool rtv = true;
			int currentVal = 0;

			SortedList<string, int> keyCount = new SortedList<string, int>();
			foreach (object o in baseObjectCollection)
			{
				PropertyInfo pi = o.GetType().GetProperty(base.PropertyName);
				if (pi != null &&
					pi.GetValue(o, null) != null)
				{
					string val = pi.GetValue(o, null).ToString().Trim();
					if (keyCount.ContainsKey(val))
					{
						// Update the count on the key
						currentVal = keyCount.Values[keyCount.IndexOfKey(val)] + 1;
						keyCount.Remove(val);
						keyCount.Add(val, currentVal);
					}
					else
					{
						keyCount.Add(val, 1);
					}
				}
			}

			List<string> existingKeys = new List<string>();
			StringBuilder sb = new StringBuilder();
			foreach (KeyValuePair<string, int> kvp in keyCount)
			{
				if (kvp.Value > 1)
				{
					// You've got a duplicate here
					existingKeys.Add(kvp.Key);
					if (!String.IsNullOrEmpty(sb.ToString())) { sb.Append(Environment.NewLine); }
					sb.Append("\t" + kvp.Key);
				}
			}
			if (existingKeys.Count > 0)
			{
				this.Description = "The following values are duplicates in the \"" + this.PropertyName + "\" field:\r\n" +
					sb.ToString();
				rtv = false;
			}

			return rtv;
		}
	}
	#endregion DuplicateInCollectionRule

	#region Value1MustBeLessThanValue2CustomRule
	/// <summary>
	/// A rule that states that value 1 must be less than the second specified value
	/// </summary>
	/// <returns>True if the rule has been followed, or false if it has been broken.</returns>
	/// <remarks>
	/// Usage:
	/// <code>
	///     this.Rules.Add(new PropertyRule("Name", "The customer name must be at least 5 letters long."));
	/// </code>
	/// </remarks>
	//public delegate bool PropertyRequiredRuleDelegate();

	/// <summary>
	/// A class to define a simple rule, using a delegate for validation.
	/// </summary>
	[Serializable]
	public class Value1MustBeLessThanValue2CustomRule : CustomRule
	{
		private string _propertyName2 = string.Empty;

		/// <summary>
        /// Gets the name of the second value to compare.
        /// </summary>
        public virtual string PropertyName2
        {
            get { return (_propertyName2 ?? string.Empty).Trim(); }
            set { _propertyName2 = value; }
        }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="propertyName">The name of the property this rule validates for. This may be blank.</param>
		/// <param name="brokenDescription">A description message to show if the rule has been broken.</param>
		public Value1MustBeLessThanValue2CustomRule(string propertyName, string propertyName2, string brokenDescription)
		{
			this.PropertyName = propertyName;
			this.PropertyName2 = propertyName2;
			this.Description = brokenDescription;
			this.BrokenRuleType = BrokenRuleType.Value1MustBeLessThanValue2CustomRule;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="propertyName">The name of the property this rule validates for. This may be blank.</param>
		/// <param name="brokenDescription">A description message to show if the rule has been broken.</param>
		/// <param name="errorIconType">The Error Icon Type that you want to use as part of the error</param>
		public Value1MustBeLessThanValue2CustomRule(string propertyName, string propertyName2, string brokenDescription, ClassGenExceptionIconType ClassGenExceptionIconType)
		{
			this.PropertyName = propertyName;
			this.PropertyName2 = propertyName2;
			this.Description = brokenDescription;
			this.ClassGenExceptionIconType = ClassGenExceptionIconType;
			this.BrokenRuleType = BrokenRuleType.Value1MustBeLessThanValue2CustomRule;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="propertyName">The name of the property this rule validates for. This may be blank.</param>
		/// <param name="brokenDescription">A description message to show if the rule has been broken.</param>
		/// <param name="onlyRunRuleForRecordStatus">Only run for the record statusses passed</param>
		public Value1MustBeLessThanValue2CustomRule(string propertyName, string propertyName2, string brokenDescription, RecordStatus onlyRunRuleForRecordStatus)
		{
			this.PropertyName = propertyName;
			this.PropertyName2 = propertyName2;
			this.Description = brokenDescription;
			this.OnlyRunRuleForRecordStatus = onlyRunRuleForRecordStatus;
			this.BrokenRuleType = BrokenRuleType.Value1MustBeLessThanValue2CustomRule;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="propertyName">The name of the property this rule validates for. This may be blank.</param>
		/// <param name="brokenDescription">A description message to show if the rule has been broken.</param>
		/// <param name="errorIconType">The Error Icon Type that you want to use as part of the error</param>
		/// <param name="onlyRunRuleForRecordStatus">Only run for the record statusses passed</param>
		public Value1MustBeLessThanValue2CustomRule(string propertyName, string propertyName2, string brokenDescription, ClassGenExceptionIconType ClassGenExceptionIconType, RecordStatus onlyRunRuleForRecordStatus)
		{
			this.PropertyName = propertyName;
			this.PropertyName2 = propertyName2;
			this.Description = brokenDescription;
			this.ClassGenExceptionIconType = ClassGenExceptionIconType;
			this.OnlyRunRuleForRecordStatus = onlyRunRuleForRecordStatus;
			this.BrokenRuleType = BrokenRuleType.Value1MustBeLessThanValue2CustomRule;
		}

		// Default private constructor to allow serialization
		public Value1MustBeLessThanValue2CustomRule()
		{
			this.BrokenRuleType = BrokenRuleType.Value1MustBeLessThanValue2CustomRule;
		}

		/// <summary>
		/// Validates that the rule has not been broken.
		/// </summary>
		/// <param name="domainObject">The domain object being validated.</param>
		/// <returns>True if the rule has not been broken, or false if it has.</returns>
		public override bool ValidateRule(object baseObject)
		{
			bool rtv = true;
			PropertyInfo pi = baseObject.GetType().GetProperty(this.PropertyName);
			PropertyInfo pi2 = baseObject.GetType().GetProperty(this.PropertyName2);
			if (pi2 == null)
			{
				this.Description = "The " + this.PropertyName2 +
					" property does not exist in object type: " + baseObject.GetType().ToString() + "" +
					Environment.NewLine + this.Description;
				rtv = false;
			}
			else if (pi == null)
			{
				this.Description = "The " + this.PropertyName +
					" property does not exist in object type: " + baseObject.GetType().ToString() + "" +
					Environment.NewLine + this.Description;
				rtv = false;
			}
			else if (pi.PropertyType != pi2.PropertyType)
			{
				this.Description = "The two types don't match one another: " + baseObject.GetType().ToString() + "" +
					Environment.NewLine + this.Description;
				rtv = false;
			}
			else if (pi != null)
			{
				if (pi.GetValue(baseObject, null) == null ||
					pi2.GetValue(baseObject, null) == null)
				{
					rtv = true;
				}
				else if (pi.GetValue(baseObject, null) == pi2.GetValue(baseObject, null))
				{
					// They're both empty - we can't compare them
					rtv = false;
				}
				else if (pi.PropertyType.IsAssignableFrom(typeof(System.DateTime)))
				{
					if ((DateTime)pi.GetValue(baseObject, null) >= (DateTime)pi2.GetValue(baseObject, null))
					{
						rtv = false;
					}
				}
				else if (pi.PropertyType.IsAssignableFrom(typeof(System.Int16)) ||
					 pi.PropertyType.IsAssignableFrom(typeof(System.Int32)) ||
					 pi.PropertyType.IsAssignableFrom(typeof(System.Int64)))
				{
					if ((Int64)pi.GetValue(baseObject, null) >= (Int64)pi2.GetValue(baseObject, null))
					{
						rtv = false;
					}
				}

				if (OnlyRunRuleForRecordStatus != 0 &&
					!RecordStatusMatch(baseObject, OnlyRunRuleForRecordStatus))
				{
					rtv = true;
				}
			}
			return rtv;
		}

		public override bool ValidateRule(IList baseObjectCollection)
		{
			return true;
		}
	}
	#endregion Value1MustBeLessThanValue2CustomRule

	#region RegExCustomRule
	[Serializable]
	public class RegexCustomRule : CustomRule
    {
        private string _regex;

        /// <summary>
        /// Constructor.
        /// </summary>
		public RegexCustomRule(string propertyName, string description, string regex)
            : base(propertyName, description)
        {
            _regex = regex;
			this.BrokenRuleType = BrokenRuleType.RegExCustomRule;
        }

		/// <summary>
        /// Constructor.
        /// </summary>
		public RegexCustomRule(string propertyName, string description, string regex, RecordStatus onlyRunRuleForRecordStatus)
            : base(propertyName, description, onlyRunRuleForRecordStatus)
        {
            _regex = regex;
			this.BrokenRuleType = BrokenRuleType.RegExCustomRule;
        }

        // Default private constructor to allow serialization
		public RegexCustomRule()
		{
			this.BrokenRuleType = BrokenRuleType.RegExCustomRule;
		}

        public override bool ValidateRule(object baseObject)
        {
			bool rtv = true;
			PropertyInfo pi = baseObject.GetType().GetProperty(this.PropertyName);
			if (pi != null)
			{
				rtv = Regex.Match(pi.GetValue(baseObject, null).ToString(), _regex).Success;
				if (OnlyRunRuleForRecordStatus != 0 &&
					!RecordStatusMatch(baseObject, OnlyRunRuleForRecordStatus))
				{
					rtv = true;
				}
			}
			else
			{
				this.Description = "The " + this.PropertyName +
					" property does not exist in object type: " + baseObject.GetType().ToString() + "" +
					Environment.NewLine + this.Description;
				rtv = false;
			}
			return rtv;
        }

		public override bool ValidateRule(IList baseObjectCollection)
		{
			return true;
		}
	}
	#endregion RegExCustomRule
}


