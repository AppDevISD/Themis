<%@ Page Title="Access Denied" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccessDenied.aspx.cs" Inherits="WebUI.AccessDenied" ClientIDMode="Static" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
	<section class="position-relative">
		<div runat="server" id="errorSection" class="card access-denied-bg" style="min-height: 80vh;">
			<div class="card-header bg-body">
				<h3><span class="placeholder col-md-2"></span></h3>
			</div>
			<div class="card-body bg-body-tertiary">
				<%-- TABLE --%>
				<div id="deniedBlur">
					<table class="table table-bordered table-striped table-hover text-center" style="padding: 0px; margin: 0px">
						<thead>
							<tr>
								<th style="width: 4%; text-align: center"><span class="placeholder col-md-8"></span></th>
								<th style="width: 6%; text-align: center"><span class="placeholder col-md-8"></span></th>
								<th style="width: 34%; text-align: center"><span class="placeholder col-md-4"></span></th>
								<th style="width: 25%; text-align: center"><span class="placeholder col-md-3"></span></th>
								<th style="width: 15%; text-align: center"><span class="placeholder col-md-4"></span></th>
								<th style="width: 10%; text-align: center"><span class="placeholder col-md-4"></span></th>
								<th style="width: 6%; text-align: center"><span class="placeholder col-md-8"></span></th>
							</tr>
						</thead>
						<tbody>
							<tr>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
							</tr>
							<tr>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
							</tr>
							<tr>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
							</tr>
							<tr>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
							</tr>
							<tr>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
							</tr>
							<tr>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
							</tr>
							<tr>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
							</tr>
							<tr>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
							</tr>
							<tr>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
							</tr>
							<tr>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
								<td class="align-middle p-4">
								</td>
							</tr>
						</tbody>
					</table>
				</div>
			</div>
		</div>

		<div class="popup-bg bg-black m-auto">
			<div class="position-relative w-100 h-100 text-center">
				<div class="position-absolute w-100 start-50 top-25 translate-middle">
					<h1 class="fas fa-shield-keyhole text-danger mb-3" style="font-size: 10rem;"></h1>
					<h1 class="mb-5 text-danger"><strong>ACCESS DENIED</strong></h1>
					<p runat="server" id="errorMessage" class="text-light">You don't have permission to access this page/application. If you believe this is a mistake, please contact your Supervisor or ISD Help Desk.</p>
				</div>
			</div>
		</div>
	</section>
</asp:Content>
