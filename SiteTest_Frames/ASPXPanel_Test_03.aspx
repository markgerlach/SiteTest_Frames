<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ASPXPanel_Test_03.aspx.cs" Inherits="SiteTest_Frames.ASPXPanel_Test_03" %>

<%@ Register assembly="DevExpress.Web.v17.1, Version=17.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<!DOCTYPE html>

<%--<script runat="server">
    protected void Page_Load(object sender, EventArgs e) {
        (Master as DeviceViewer).Url = "~/Panel/FixedPositionPage.aspx";
    }
</script>--%>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Frames Page with Margins</title>
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
			//ASPxCallbackPanel1.PerformCallback(url);
			//alert(url);
			mainContent.PerformCallback(url);
			
			//window.location.url = '';
			//alert(url);
			//element.url = url;
			//element.innerHTML = '<object type="type/html" data="http://www.ebay.com">';
		}

		function setCenterContentEdit(url) {
			//alert('hi1');
			var elems = document.forms[0].elements;
			//var elems = form.elements;
			//alert('hi2');
			//alert(elems.length);
			var formElements = '';
			for (var i = 0; i < elems.length; i++) {
				//alert(elems[i].name);
				//alert(elems[i].name.toLowerCase());
				//alert(elems[i].name.toLowerCase().indexOf('uccontactedit_'));
				//debugger;
				if (elems[i].name.toLowerCase().indexOf('uccontactedit_') == -1) { continue; }
				if (formElements.length > 0) { formElements = formElements + '||'; }
				var elemName = elems[i].name;
				if (elemName.indexOf('$') > -1) { elemName = elemName.substring(elemName.lastIndexOf('$') + 1); }	
				formElements = formElements + elemName + '|' + elems[i].value;
				//alert(formElements);
			}
			//if (formElements.length > 0) { alert(formElements); }
			//var element = document.ge.getElementById('mainContent');
			//ASPxCallbackPanel1.PerformCallback(url);
			//alert(url);
			var element = document.getElementById('mainContent');
			if (formElements.length > 0) { url = url + '||||' + formElements; }
			mainContent.PerformCallback(url);
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
                    <div class="panelContent">
						<dx:ASPxNavBar ID="leftNavBar" runat="server" EnableAnimation="true" EnableTheming="true">
							<Groups>
								<dx:NavBarGroup Text="Function List" ItemBulletStyle="Circle">
									<Items>
										<dx:NavBarItem Text="Contact List" NavigateUrl="javascript:setCenterContent('/Personnel/Contact/List');"></dx:NavBarItem>
										<dx:NavBarItem Text="Contact Edit - Abernathy" NavigateUrl="javascript:setCenterContentEdit('/Personnel/Contact/Edit/-9223372036854775767');"></dx:NavBarItem>
										<dx:NavBarItem Text="Contact Edit - Person 2" NavigateUrl="javascript:setCenterContentEdit('/Personnel/Contact/Edit/-9223372036854775766');"></dx:NavBarItem>
									</Items>
								</dx:NavBarGroup>
								<dx:NavBarGroup Text="Secondary Functions" ItemBulletStyle="Circle">
									<Items>
										<dx:NavBarItem Text="Equipment List"></dx:NavBarItem>
										<dx:NavBarItem Text="Edit Equipment"></dx:NavBarItem>
									</Items>
								</dx:NavBarGroup>
							</Groups>
							<ItemStyle ForeColor="Blue" />
							<Paddings Padding="0" />
						</dx:ASPxNavBar>
                    </div>
                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxPanel>
		<dx:ASPxCallbackPanel id="mainContent" ClientInstanceName="mainContent" runat="server" 
			Height="100%" 
			OnCallback="mainContent_Callback"
			CssClass="centerPanel">
            <PanelCollection>
                <dx:PanelContent runat="server" SupportsDisabledAttribute="True">
                    <div class="panelContent">This is ASPxPanel control with stupid = WindowBottom</div>
                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxCallbackPanel>
		<%--<div id="mainContent1">
            This is a test
        </div>--%>
    </form>
</body>
</html>