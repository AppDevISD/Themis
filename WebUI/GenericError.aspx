<%@ Page Title="Error" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GenericError.aspx.cs" Inherits="WebUI.GenericError" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
	<section>
		<div runat="server" id="errorSection" class="card" style="min-height: 80vh;">
			<div class="card-header bg-body">
				<h1><i class="fas fa-triangle-exclamation text-danger"></i>&nbsp;Error</h1>
			</div>
			<div class="card-body bg-body-tertiary text-center">
				<div class="row mb-4">
					<div class="col-md-12">
						<h2 runat="server" id="errorLabel">Generic Error</h2>
					</div>
				</div>
				<div class="row mb-3">
					<div class="col-md-12">
						<p runat="server" id="errorMessageLine">Line Error</p>
					</div>
				</div>
				<div class="row">
					<div class="col-md-6 mx-auto text-start">
						<p runat="server" id="errorMessage">Error Message</p>
					</div>
				</div>
			</div>
		</div>
	</section>
</asp:Content>
