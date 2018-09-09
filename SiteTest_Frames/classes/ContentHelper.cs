using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace SiteTest_Frames.classes
{
	public class ContentHelper
	{
		private static StringBuilder _sb = new StringBuilder();
		private static TextWriter _textWriter;
		private static HtmlTextWriter _htmlTextWriter;

		public static string RenderControlToHTML(System.Web.UI.UserControl uc)
		{
			string rtv = string.Empty;
			_sb = new StringBuilder();
			TextWriter _textWriter = new StringWriter(_sb);
			HtmlTextWriter _htmlTextWriter = new HtmlTextWriter(_textWriter);
			uc.RenderControl(_htmlTextWriter);
			rtv = _htmlTextWriter.ToString();
			return rtv;
		}
	}
}