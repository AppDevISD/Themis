<%@ Page Title="Access Denied" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccessDenied.aspx.cs" Inherits="WebUI.AccessDenied" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
	<section>
		<div runat="server" id="errorSection" class="card" style="min-height: 10vh;">
			<div class="card-header bg-body">
				<h1><i class="fas fa-shield-keyhole text-danger"></i>&nbsp;Access Denied</h1>
			</div>
			<div class="card-body bg-body-tertiary text-center">
				<div class="row">
					<div class="col-md-12 text-start">
						<p runat="server" id="errorMessage">You don't have permission to access this page/application. If you believe this is a mistake, please contact your Supervisor or ISD Help Desk.</p>
					</div>
				</div>
			</div>
		</div>
	</section>
</asp:Content>
