<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ListEmployees.aspx.cs" Inherits="SiteTest_Frames.Personnel.ListEmployees" %>

<%@ Register Assembly="DevExpress.Web.v17.1, Version=17.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <div>
		<dx:ASPxGridView ID="gridViewEmployees" runat="server"></dx:ASPxGridView>
	</div>
</body>
</html>
