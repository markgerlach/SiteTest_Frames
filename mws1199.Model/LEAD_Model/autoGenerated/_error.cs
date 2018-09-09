using System;
using System.Reflection;
using System.Text;
using System.IO;
using System.Data;

namespace LEADBase
{
	public class LEADBaseException : Exception
	{
		public LEADBaseException()
		{
		}

		public LEADBaseException(string message)
			: base(message)
		{
		}
	}
}
