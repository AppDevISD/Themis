<%@ Page Title="Sandbox" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Sandbox.aspx.cs" Inherits="WebUI.Sandbox" ClientIDMode="Static" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
	<section>
		<div class="card">
			<div class="card-header bg-body">
				<h3><i class="fas fa-key"></i>&nbsp;&nbsp;Sandbox</h3>
			</div>
			<asp:UpdatePanel runat="server" ID="pnlSandbox" UpdateMode="Always">
				<ContentTemplate>
					<div class="card-body bg-body-tertiary">
						<div class="container-fluid">
							<div id="firstContainer" class="bg-body-secondary p-2 rounded" style="max-width: 750px; min-height: 250px;">
								<asp:Repeater runat="server" ID="rpEmails">
									<ItemTemplate>
										<div class="badge rounded-pill text-bg-secondary fs-6 m-1">
											<%# Container.DataItem %>
											<asp:LinkButton runat="server" ID="removeBtn" CssClass="text-danger" style="margin-left: 10px;"><span class="fa-solid fa-xmark"></span></asp:LinkButton>
										</div>
									</ItemTemplate>
								</asp:Repeater>
								
									
								<button class="btn badge rounded-pill text-bg-success"><span class="fa-solid fa-plus"></span>&nbsp;Add</button>
							</div>
						</div>
					</div>
				</ContentTemplate>
			</asp:UpdatePanel>
		</div>
	</section>
</asp:Content>
