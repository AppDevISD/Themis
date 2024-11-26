<%@ Page Title="Ordinances" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Ordinances.aspx.cs" Inherits="WebUI.Ordinances" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<section>
		<div runat="server" id="errorAlert" class="alert alert-danger alert-dismissible" role="alert">
			<asp:Label runat="server" ID="errorMsg"></asp:Label>
			<button runat="server" id="CloseAlert" type="button" class="btn btn-close" onserverclick="CloseAlert_ServerClick" aria-label="Close"></button>
		</div>
		<div class="card">
			<div class="card-header bg-body">
				<h3><i class="fas fa-book-section"></i>&nbsp;Ordinances</h3>
			</div>
			<div class="card-body bg-body-tertiary">
				<asp:Repeater runat="server" ID="rpTemplateForm">
					<HeaderTemplate>
						<table id="FormTable" class="table table-bordered table-striped table-hover text-center" style="padding: 0px; margin: 0px">
							<thead>
								<tr>
									<th style="width: 10%; text-align: center"><strong>Submitted</strong></th>
									<th style="width: 10%; text-align: center"><strong>Form</strong></th>
									<th style="width: 15%; text-align: center"><strong>Contact</strong></th>
									<th style="width: 15%; text-align: center"><strong>Employee</strong></th>
									<th style="width: 50%; text-align: center"><strong>Notes</strong></th>
									<th style="width: 1%; text-align: center"><strong>Modify</strong></th>
								</tr>
							</thead>
					</HeaderTemplate>
					<ItemTemplate>
						<tr>
							<td style="vertical-align: middle;">
								<asp:Label ID="date" Text='<%# DataBinder.Eval(Container.DataItem, "EffectiveDate", "{0:MM/dd/yyyy}") %>' runat="server" />
							</td>
							<td style="vertical-align: middle;">
								<asp:Label ID="formType" Text='<%# DataBinder.Eval(Container.DataItem, "FormTypeDesc") %>' runat="server" />
							</td>
							<td style="vertical-align: middle;">
								<asp:Label ID="contact" Text='<%# DataBinder.Eval(Container.DataItem, "ContactName") %>' runat="server" />
							</td>
							<td style="vertical-align: middle;">
								<asp:Label ID="employee" Text='<%# DataBinder.Eval(Container.DataItem, "EmployeeName") %>' runat="server" />
							</td>
							<td style="vertical-align: middle;">
								<asp:Label ID="notes" Text='<%# DataBinder.Eval(Container.DataItem, "Comments") %>' runat="server" />
							</td>
							<td style="vertical-align: middle;">
								<a runat="server" id="delete" class="btn btn-danger btn-sm" data-toggle="modal" data-target="#deleteModal" autopostback="false" onclick='<%#$"DeleteForm(\"{DataBinder.Eval(Container.DataItem, "FormID")}\")"%>'>Delete</a>
							</td>
						</tr>
					</ItemTemplate>
					<FooterTemplate>
						</table>
					</FooterTemplate>
				</asp:Repeater>
			</div>
			<div class="card-footer p-0">
				<asp:Panel ID="pnlPagingP" CssClass="panel m-0" runat="server" Visible="false">
					<table class="table m-0" runat="server">
						<tr>
							<td class="text-left">
								<button id="lnkFirstSearchP" class="btn btn-primary" runat="server" onserverclick="lnkFirstSearchP_Click" style="width: 150px;" causesvalidation="false"><i class="fas fa-angles-left"></i>&nbsp;First</button>
							</td>
							<td class="text-center">
								<button id="lnkPreviousSearchP" class="btn btn-primary" runat="server" onserverclick="lnkPreviousSearchP_Click" style="width: 150px;" causesvalidation="false"><i class="fas fa-angle-left"></i>&nbsp;Previous</button>
							</td>
							<td class="text-center">
								<div style="margin-top: 5px">
									<asp:Label Style="font-weight: bold; font-size: 18px" ID="lblCurrentPageBottomSearchP" runat="server"></asp:Label>
								</div>
							</td>
							<td class="text-center">
								<button id="lnkNextSearchP" class="btn btn-primary" runat="server" onserverclick="lnkNextSearchP_Click" style="width: 150px;" causesvalidation="false"><i class="fas fa-angle-right"></i>&nbsp;Next</button>
							</td>
							<td class="text-end">
								<button id="lnkLastSearchP" class="btn btn-primary" runat="server" onserverclick="lnkLastSearchP_Click" style="width: 150px;" causesvalidation="false"><i class="fas fa-angles-right"></i>&nbsp;Last</button>
							</td>
						</tr>
					</table>
				</asp:Panel>
			</div>
		</div>
	</section>
	<!-- DELETE Modal -->
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
					<asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-danger" Visible="true" OnClick="mdlDeleteSubmit_ServerClick" />
				</div>
			</div>
		</div>
	</div>
	<asp:HiddenField runat="server" ID="deleteID" Value="0"/>
	<script>
		var hdnID = document.getElementById('<%= deleteID.ClientID %>')
		function DeleteForm(formID) {
			hdnID.setAttribute('Value', formID);
		}
	</script>
</asp:Content>