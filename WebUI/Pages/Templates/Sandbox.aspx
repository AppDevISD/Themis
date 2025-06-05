<%@ Page Title="Sandbox" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Sandbox.aspx.cs" Inherits="WebUI.Sandbox" ClientIDMode="Static" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
	
</asp:Content>
<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
	<section>
		<div class="card">
			<div class="card-header bg-body">
				<h3><i class="fas fa-key"></i>&nbsp;&nbsp;Sandbox</h3>
			</div>
			<div class="card-body bg-body-tertiary">
				<ul id="adminTabs" class="nav nav-tabs border-0" role="tablist" data-collapse-tabs="#adminMoreDD">
					<li class="nav-item">
						<button id="defaultEmailsBtn" class="nav-link active" data-toggle="tab" data-target="#defaultEmails-tab-pane" type="button" role="tab">Default Emails</button>
					</li>
				</ul>
				<div id="adminTabsContent" class="tab-content tab-card" style="min-height: 50%">
					<div id="defaultEmails-tab-pane" class="tab-pane fade active show" role="tabpanel">
						<div class="card">

							<%--<div class="card-header bg-body">

							</div>--%>

							<div class="card-body bg-body-tertiary">

							</div>

							<%--<div class="card-footer bg-body" style="min-height: 50px;">

							</div>--%>
						</div>
					</div>
				</div>
			</div>
		</div>
	</section>
</asp:Content>
