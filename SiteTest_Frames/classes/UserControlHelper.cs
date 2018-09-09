using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;

namespace SiteTest_Frames.classes
{
	public class UserControlHelper
	{
		/// <summary>
		/// Loads the UserControl at the given path to the given page
		/// </summary>
		/// <param name="page">The Page context into which the UserControl will be loaded</param>
		/// <param name="userControlPath">The path to the UserControl</param>
		/// <param name="constructorParameters">The UserControl constructor parameter list</param>
		/// <returns>A UserControl object</returns>
		public static UserControl LoadControl(Page page, string userControlPath, params object[] constructorParameters)
		{
			UserControl ctl = null;
			try
			{
				List<Type> constParamTypes = new List<Type>();
				foreach (object constParam in constructorParameters)
				{
					constParamTypes.Add(constParam.GetType());
				}

				ctl = page.LoadControl(userControlPath) as UserControl;

				// Find the relevant constructor
				ConstructorInfo constructor = ctl.GetType().BaseType.GetConstructor(constParamTypes.ToArray());

				// And then call the relevant constructor
				if (constructor == null)
				{
					throw new MemberAccessException("The requested constructor was not found on : " + ctl.GetType().BaseType.ToString());
				}
				else
				{
					constructor.Invoke(ctl, constructorParameters);
				}
			}
			catch (Exception ex)
			{
				//PersistentError.LogError(ex);
			}

			// Finally return the fully initialized UC
			return ctl;
		}
	}
}