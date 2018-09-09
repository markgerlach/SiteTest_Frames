<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ASPXPanel_Test_01.aspx.cs" Inherits="SiteTest_Frames.Frames" %>

<%@ Register assembly="DevExpress.Web.v17.1, Version=17.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<!DOCTYPE html>

<%--<script runat="server">
    protected void Page_Load(object sender, EventArgs e) {
        (Master as DeviceViewer).Url = "~/Panel/FixedPositionPage.aspx";
    }
</script>--%>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Test Frames Page</title>
	<script>
	function getObjectProps(obj) {
		// This is how you call this:
		// getObjectProps(document.getElementById('TopPanel').style);s
		var sVal = '';
		for (var key in obj) {
			//alert(sVal.length);
			if (sVal.length > 0) { sVal = sVal + '\n'; }
			//if (sVal.trim().length > 0) { sVal = sVal + '\r\n'); }
			sVal = sVal + key;
		}
		alert(sVal);
		}

		// Set the center content
		function setCenterContent(url) {
			var element = document.getElementById('mainContent');
			//alert(url);
			//element.url = url;
			element.innerHTML = '<object type="type/html" data="http://www.ebay.com">';
		}

		// Set the background color
		function setBackColor(clr) {
			//alert(clr);
			//document.getElementById('TopPanel').style.backgroundColor = clr;
			//document.body.style.backgroundColor = clr;
			var element = document.getElementById('TopPanel');
			//alert(element.style);
			var zInd = 'z-index:' + element.style.zIndex + ';';
			//alert(zInd);
			element.setAttribute('style', 'background-color: ' + clr + ' !important; ' + zInd);
			//alert(element.style);
			//alert('Color Set: ' + document.getElementById('TopPanel').style.backgroundColor);
		}
	</script>
	<link rel="stylesheet" type="text/css" href="css/FixedPanel.css" />
</head>
<body>
    <form id="form1" runat="server">
        <dx:ASPxPanel id="TopPanel" runat="server" fixedposition="WindowTop" fixedpositionoverlap="true" CssClass="topPanel">
            <PanelCollection>
                <dx:PanelContent runat="server" SupportsDisabledAttribute="True">
                    <div class="panelContent">
						<dx:ASPxColorEdit ID="ASPxColorEdit1" runat="server" OnColorChanged="ASPxColorEdit1_ColorChanged">
							<ClientSideEvents ColorChanged="function(s,e) { setBackColor(s.GetColor()); }" />
						</dx:ASPxColorEdit>
						&nbsp;<a class="btn btn-default" 
						href="javascript:setCenterContent('http://www.ebay.com');">Learn more</a></div>
                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxPanel>
        <dx:ASPxCallbackPanel id="BottomPanel" runat="server" FixedPosition="WindowBottom" CssClass="bottomPanel">
            <PanelCollection>
                <dx:PanelContent runat="server" SupportsDisabledAttribute="True">
                    <div class="panelContent">This is ASPxPanel control with FixedPosition = WindowBottom</div>
                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxCallbackPanel>
		<dx:ASPxPanel id="LeftPanel" runat="server" fixedposition="WindowLeft" FixedPositionOverlap="true" CssClass="leftPanel">
            <PanelCollection>
                <dx:PanelContent runat="server" SupportsDisabledAttribute="True">
                    <div class="panelContent">This is ASPxPanel control with FixedPosition = WindowLeft</div>
                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxPanel>
        <div class="bodyContent" id="mainContent">
            <h3>UI Controls for Your Next Great Web App</h3>
            <p>
                With DevExpress web controls, you can build a bridge to the future on the platform you know and love. The 175+ AJAX controls and MVC extensions that make up the DevExpress ASP.NET Subscription have been engineered so you can deliver exceptional, touch-enabled, interactive experiences for the web, regardless of the target browser or computing device. DevExpress web UI components support all major browsers including Internet Explorer, FireFox, Chrome, Safari and Opera, and are continuously tested to ensure the best possible compatibility across platforms, operating systems and devices.
            </p>
            <p>
                And to ensure you can build your best and meet the needs of your enterprise each and every time, the DevExpress ASP.NET Subscription ships with 20 built-in application themes that can be used out-of-the box or customized via our ASP.NET Theme Builder.
            </p>
            <h3>UI Controls for Your Next Great Hybrid App</h3>
            <p>
                With DevExpress UI controls, you'll be able to deliver elegant line-of-business applications that emulate the touch-first Windows 8 Pro UX and maintain backward compatibility with previous versions of the Windows operating system. Whether you need to create a tile-based, modern UI application for Windows or need to quickly convert an existing application for use on Microsoft Surface, the DevExpress WinForms & WPF Subscriptions will help you deliver immersive business solutions, without abandoning your existing code investments. And because we continue to extend our .NET product line with enterprise-ready capabilities designed to help you build next-generation user experiences on the platform of your choice, you can rest assured that your apps will never be left behind, regardless of changes made to Windows or the introduction of new device form-factors.
            </p>
            <h3>UI Controls for Your Next Great Mobile App</h3>
            <p>
                Create highly-responsive mobile apps that meet the needs of your ever-changing enterprise and the BYOD world. Use the power of HTML, CSS3 and JavaScript to deliver line-of-business solutions that look, feel and behave just like native apps, without learning multiple languages or frameworks.
            </p>
            <p>
                DevExpress Universal ships with DevExtreme—the framework you'll need to deliver beautiful, easy-to-use HTML5/ JavaScript applications for smartphones, tablets and desktop browsers. With nine high-impact themes replicating today's most popular mobile OS interfaces, DevExtreme apps automatically apply the appropriate theme to your solution so your "write once, use everywhere" apps look great, regardless of the end-user's device or platform choice.
            </p>
            <h3>UI Controls for Your Next Great Dashboard App</h3>
            <p>
                With DevExpress Dashboard for .NET, creating insightful and information-rich decision support systems for executives and business users across platforms and devices is a simple matter of selecting the appropriate UI element (Chart, Pivot Table, Data Card, Gauge, Map or Grid) and dropping data fields onto corresponding arguments, values and series. And because DevExpress Dashboard elements automatically provide the best data visualization option for you, results are immediate, accurate and always relevant.
            </p>
            <p>
                With full Visual Studio integration, DevExpress Dashboard allows you to deliver business solutions that target Windows, the Web and Mobile devices with breathtaking ease. It's built so you can manage everything inside your favorite IDE: from data-binding and dashboard design to filtering, drill down and event handling. 
            </p>
        </div>
    </form>
</body>
</html>
