<%@ Page Title="Ordinance Admin" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OrdinanceTrackingAdmin.aspx.cs" Inherits="WebUI.OrdinanceTrackingAdmin" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<section>
		<div class="card">
			<div class="card-header bg-body">
				<h3><i class="fas fa-key"></i>&nbsp;&nbsp;Ordinance Admin</h3>
			</div>
			<asp:UpdatePanel runat="server" ID="pnlOrdinanceTable" UpdateMode="Always">
				<ContentTemplate>
					<div class="card-body bg-body-tertiary">

					</div>
				</ContentTemplate>
			</asp:UpdatePanel>
		</div>
	</section>
</asp:Content>