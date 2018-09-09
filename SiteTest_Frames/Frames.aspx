01<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Frames.aspx.cs" Inherits="SiteTest_Frames.Frames1" %>

<%@ Register Assembly="DevExpress.Web.v17.1, Version=17.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

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
        <div>
			<iframe id="hdrFrame" src="/Contact.aspx" style="height:100px;width:100%;border:none;"></iframe>
        </div>
		<div>
			<dx:ASPxSplitter ID="splitMain" runat="server" Orientation="Horizontal" FullscreenMode="true">
				<Panes>
					<dx:SplitterPane Size="15%">
						<ContentCollection>
							<dx:SplitterContentControl ID="SplitterContentControl1" runat="server">
								<div>
								</div>
							</dx:SplitterContentControl>
						</ContentCollection>
					</dx:SplitterPane>
					<dx:SplitterPane>
						<ContentCollection>
							<dx:SplitterContentControl ID="SplitterContentControl2" runat="server">
								<dx:ASPxSplitter ID="splitCenter" runat="server" Orientation="Vertical" AllowResize="false" FullscreenMode="true">
									<Panes>
										<dx:SplitterPane Name="TopPane">
											<ContentCollection>
												<dx:SplitterContentControl>
													<div>Top Pane</div>
												</dx:SplitterContentControl>
											</ContentCollection>
										</dx:SplitterPane>
										<dx:SplitterPane MaxSize="200" MinSize="200">
											<ContentCollection>
												<dx:SplitterContentControl>
													<div>Bottom Right Pane</div>
												</dx:SplitterContentControl>
											</ContentCollection>
											<Separator>
												<SeparatorStyle BackColor="Blue" />
											</Separator>
										</dx:SplitterPane>
									</Panes>
									<Styles>
										<Pane Border-BorderStyle="None">	
											<Paddings Padding="0" />
										</Pane>
									</Styles>
								</dx:ASPxSplitter>
							</dx:SplitterContentControl>
						</ContentCollection>
						<Separator>
							<SeparatorStyle BackColor="Silver" />
						</Separator>
					</dx:SplitterPane>
				</Panes>
				<Styles>
					<Pane Border-BorderStyle="None">	
						<Paddings Padding="0" />
					</Pane>
				</Styles>
			</dx:ASPxSplitter>
		</div>
    </form>
</body>
</html>
