<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SiteTest_Frames._Default" %>

<%@ Register assembly="DevExpress.Web.v17.1, Version=17.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>ASP.NET</h1>
        <p class="lead">ASP.NET is a free web framework for building great Web sites and Web applications using HTML, CSS, and JavaScript.</p>
        <p><a href="http://www.asp.net" class="btn btn-primary btn-lg">Learn more &raquo;</a></p>
    </div>

    <div class="row">
        <div class="col-md-4">
            <h2>Getting started</h2>
            <p>
                ASP.NET Web Forms lets you build dynamic websites using a familiar drag-and-drop, event-driven model.
            A design surface and hundreds of controls and components let you rapidly build sophisticated, powerful UI-driven sites with data access.
            </p>
            <p>
                <a class="btn btn-default" href="https://go.microsoft.com/fwlink/?LinkId=301948">Learn more &raquo;<dx:ASPxRoundPanel ID="ASPxRoundPanel1" runat="server" 
					ShowCollapseButton="true" Width="200px">
				</dx:ASPxRoundPanel>
				</a>
            &nbsp;<dx:ASPxLoadingPanel ID="ASPxLoadingPanel1" runat="server">
				</dx:ASPxLoadingPanel>
            </p>
        	<dx:ASPxCallbackPanel ID="ASPxCallbackPanel1" runat="server" Width="200px">
				<PanelCollection>
<dx:PanelContent runat="server">
	<dx:ASPxPanel ID="ASPxPanel1" runat="server" Width="200px">
		<PanelCollection>
			<dx:PanelContent runat="server">
			</dx:PanelContent>
		</PanelCollection>
	</dx:ASPxPanel>
					</dx:PanelContent>
</PanelCollection>
			</dx:ASPxCallbackPanel>
        </div>
        <div class="col-md-4">
            <h2>Get more libraries</h2>
            <p>
                NuGet is a free Visual Studio extension that makes it easy to add, remove, and update libraries and tools in Visual Studio projects.
            </p>
            <p>
                <a class="btn btn-default" href="https://go.microsoft.com/fwlink/?LinkId=301949">Learn more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Web Hosting</h2>
            <p>
                You can easily find a web hosting company that offers the right mix of features and price for your applications.
            </p>
            <p>
                <a class="btn btn-default" href="https://go.microsoft.com/fwlink/?LinkId=301950">Learn more &raquo;</a>
            </p>
        </div>
    </div>

</asp:Content>
