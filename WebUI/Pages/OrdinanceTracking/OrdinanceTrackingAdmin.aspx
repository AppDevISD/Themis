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
						<div class="container-fluid">
							<div class="row justify-content-between">
								<label for="firstContainer" class="pill-container-label col-md-2 px-0">First</label>
								<label for="secondContainer" class="pill-container-label col-md-2 px-0">Second</label>
								<label for="thirdContainer" class="pill-container-label col-md-2 px-0">Third</label>
								<label for="fourthContainer" class="pill-container-label col-md-2 px-0">Fourth</label>
								<label for="fifthContainer" class="pill-container-label col-md-2 px-0">Fifth</label>
							</div>
							<div class="row justify-content-between">
								<div id="firstContainer" class="col-md-2 bg-body-secondary p-2 rounded" style="min-height: 250px;">
									<div class="btn badge rounded-pill text-bg-secondary">Test</div>
									<button class="btn badge rounded-pill text-bg-success"><span class="fa-solid fa-plus"></span>&nbsp;Add</button>
								</div>

								<div id="secondContainer" class="col-md-2 bg-body-secondary p-2 rounded" style="min-height: 250px;">
									<button class="btn badge rounded-pill text-bg-success"><span class="fa-solid fa-plus"></span>&nbsp;Add</button>
								</div>

								<div id="thirdContainer" class="col-md-2 bg-body-secondary p-2 rounded" style="min-height: 250px;">
									<button class="btn badge rounded-pill text-bg-success"><span class="fa-solid fa-plus"></span>&nbsp;Add</button>
								</div>

								<div id="fourthContainer" class="col-md-2 bg-body-secondary p-2 rounded" style="min-height: 250px;">
									<button class="btn badge rounded-pill text-bg-success"><span class="fa-solid fa-plus"></span>&nbsp;Add</button>
								</div>

								<div id="fifthContainer" class="col-md-2 bg-body-secondary p-2 rounded" style="min-height: 250px;">
									<button class="btn badge rounded-pill text-bg-success"><span class="fa-solid fa-plus"></span>&nbsp;Add</button>
								</div>
							</div>
						</div>
					</div>
				</ContentTemplate>
			</asp:UpdatePanel>
		</div>
	</section>
	<!-- ADD/EDIT MODAL -->
	<div class="modal fade" id="deleteModal" tabindex="-1" role="dialog" aria-labelledby="deleteModalLabel">
		<div class="modal-dialog" role="document">
			<div class="modal-content">
				<div class="modal-header">
					<h4 class="modal-title" id="deleteModalLabel">Delete</h4>
				</div>
				<div class="modal-body">
					<asp:Label runat="server" ID="deleteLabel" Style="font-size: 18px; font-weight: bold" CssClass="text-danger" Text="Are you sure you want to delete this item? (This cannot be undone)" />
				</div>
				<div class="modal-footer">
					<button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
					<asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-danger" Visible="true" OnClick="mdlDeleteSubmit_ServerClick" OnClientClick="ShowSubmitToast();" />
				</div>
			</div>
		</div>
	</div>
</asp:Content>