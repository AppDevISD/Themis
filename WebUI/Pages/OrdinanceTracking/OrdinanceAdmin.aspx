<%@ Page Title="Ordinance Admin" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OrdinanceAdmin.aspx.cs" Inherits="WebUI.OrdinanceAdmin" ClientIDMode="Static" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
	<%-- PAGE CONTENT --%>
	<section>
		<asp:UpdatePanel runat="server" ID="pnlAdmin" UpdateMode="Always" class="overlap-panels">
			<Triggers></Triggers>

			<ContentTemplate>
				<div runat="server" id="adminCard" class="card">
					<div class="card-header bg-body">
						<h3><i class="fa-kit fa-solid-user-crown-gear"></i>&nbsp;Ordinance Admin</h3>
					</div>
					<div class="card-body bg-body-tertiary">
						<ul id="adminTabs" class="nav nav-tabs border-0" role="tablist" data-collapse-tabs="#adminMoreDD">
								<li class="nav-item">
									<button id="defaultEmailsBtn" class="nav-link active" data-toggle="tab" data-target="#defaultEmails-tab-pane" type="button" role="tab">Default Emails</button>
								</li>
								<li id="adminMoreDD" class="nav-item dropdown">
									<button class="nav-link dropdown-toggle" data-toggle="dropdown" type="button" role="button">More</button>
									<div class="dropdown-menu">
									</div>
								</li>
						</ul>
						<div id="adminTabsContent" class="tab-content" style="min-height: 85%">
							<div id="defaultEmails-tab-pane" class="tab-pane fade active show" role="tabpanel">
								<div class="row h-100">
									<div class="col-md-6">
										<div id="statusEmailsDiv">
											<ul id="statusEmailsTabs" class="nav nav-tabs border-0" role="tablist" data-collapse-tabs="#statusEmailsMoreDD">
												<li class="nav-item">
												  <button id="pendingBtn" class="nav-link active" data-toggle="tab" data-target="#pending-tab-pane" type="button" role="tab">Pending</button>
												</li>
												<li class="nav-item">
												  <button id="underReviewBtn" class="nav-link" data-toggle="tab" data-target="#underReview-tab-pane" type="button" role="tab">Under Review</button>
												</li>
												<li class="nav-item">
												  <button id="beingHeldBtn" class="nav-link" data-toggle="tab" data-target="#beingHeld-tab-pane" type="button" role="tab">Being Held</button>
												</li>
												<li class="nav-item">
												  <button id="draftedBtn" class="nav-link" data-toggle="tab" data-target="#drafted-tab-pane" type="button" role="tab">Drafted</button>
												</li>
												<li class="nav-item">
												  <button id="approvedBtn" class="nav-link" data-toggle="tab" data-target="#approved-tab-pane" type="button" role="tab">Approved</button>
												</li>
												<li class="nav-item">
												  <button id="rejectedBtn" class="nav-link" data-toggle="tab" data-target="#rejected-tab-pane" type="button" role="tab">Rejected</button>
												</li>
												<li class="nav-item">
												  <button id="deletedBtn" class="nav-link" data-toggle="tab" data-target="#approved-tab-pane" type="button" role="tab">Deleted</button>
												</li>
												<li id="statusEmailsMoreDD" class="nav-item dropdown">
													<button class="nav-link dropdown-toggle" data-toggle="dropdown" type="button" role="button">More</button>
													<div class="dropdown-menu">
													</div>
												</li>
											</ul>
											<div id="statusEmailsTabsContent" class="tab-content p-0 border-0">
												<div id="pending-tab-pane" class="tab-pane fade active show" role="tabpanel">
													<div class="card b-0 mh-0">
														<div class="card-body bg-body-secondary">
													
														</div>
														<div class="card-footer bg-body-tertiary">
															<div class="row">
																<div class="col-md-12">
																	<div class="input-group">
																		<span class="input-group-text fas fa-address-book"></span>
																		<asp:TextBox runat="server" ID="TextBox1" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com" data-enter-btn="FundsCheckByAddRequestEmailAddress"></asp:TextBox>
																		<asp:Button runat="server" ID="Button1" UseSubmitBehavior="false" CssClass="btn btn-success fas-btn" disabled="disabled" Text='&#xf055;' data-disable-btn="aspIconBtn" TabIndex="-1"/>
																		<%--OnClick="AddRequestEmailAddress_Click"--%>
																	</div>
																</div>
															</div>
														</div>
													</div>
												</div>
											</div>
										</div>
									</div>
									<div class="col-md-6 border-start">
										<div id="sigEmailsDiv">
											<ul id="sigEmailsTabs" class="nav nav-tabs border-0" role="tablist" data-collapse-tabs="#sigEmailsMoreDD">
												<li class="nav-item">
													<button id="fundsCheckByBtn" class="nav-link active" data-toggle="tab" data-target="#fundsCheckBy-tab-pane" type="button" role="tab">Funds Check By</button>
												</li>
												<li class="nav-item">
													<button id="directorSupervisorBtn" class="nav-link" data-toggle="tab" data-target="#directorSupervisor-tab-pane" type="button" role="tab">Director/Supervisor</button>
												</li>
												<li class="nav-item">
													<button id="cPABtn" class="nav-link" data-toggle="tab" data-target="#cPA-tab-pane" type="button" role="tab">City Purchasing Agent</button>
												</li>
												<li class="nav-item">
													<button id="obmDirectorBtn" class="nav-link" data-toggle="tab" data-target="#obmDirector-tab-pane" type="button" role="tab">OBM Director</button>
												</li>
												<li class="nav-item">
													<button id="mayorBtn" class="nav-link" data-toggle="tab" data-target="#mayor-tab-pane" type="button" role="tab">Mayor</button>
												</li>
												<li id="sigEmailsMoreDD" class="nav-item dropdown">
													<button class="nav-link dropdown-toggle" data-toggle="dropdown" type="button" role="button">More</button>
													<div class="dropdown-menu">
													</div>
												</li>
											</ul>
											<div id="sigEmailsTabsContent" class="tab-content p-0 border-0">
												<div id="fundsCheckBy-tab-pane" class="tab-pane fade active show" role="tabpanel">
													<div class="card b-0 mh-0">
														<div class="card-body bg-body-secondary">
													
														</div>
														<div class="card-footer bg-body-tertiary">
															<div class="row">
																<div class="col-md-12">
																	<div class="input-group">
																		<span class="input-group-text fas fa-address-book"></span>
																		<asp:TextBox runat="server" ID="fundsCheckBySignatureEmailAddress" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com" data-enter-btn="FundsCheckByAddRequestEmailAddress"></asp:TextBox>
																		<asp:Button runat="server" ID="FundsCheckByAddRequestEmailAddress" UseSubmitBehavior="false" CssClass="btn btn-success fas-btn" disabled="disabled" Text='&#xf055;' data-disable-btn="aspIconBtn" TabIndex="-1"/>
																		<%--OnClick="AddRequestEmailAddress_Click"--%>
																	</div>
																</div>
															</div>
														</div>
													</div>
												</div>
												<div id="directorSupervisor-tab-pane" class="tab-pane fade" role="tabpanel">
													<div class="card b-0 mh-0">
														<div class="card-body bg-body-secondary">
													
														</div>
														<div class="card-footer bg-body-tertiary">
															<div class="row">
																<div class="col-md-12">
																	<div class="input-group">
																		<span class="input-group-text fas fa-address-book"></span>
																		<asp:TextBox runat="server" ID="directorSupervisorSignatureEmailAddress" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com" data-enter-btn="DirectorSupervisorAddRequestEmailAddress"></asp:TextBox>
																		<asp:Button runat="server" ID="DirectorSupervisorAddRequestEmailAddress" UseSubmitBehavior="false" CssClass="btn btn-success fas-btn" disabled="disabled" Text='&#xf055;' data-disable-btn="aspIconBtn" TabIndex="-1"/>
																		<%--OnClick="AddRequestEmailAddress_Click"--%>
																	</div>
																</div>
															</div>
														</div>
													</div>
												</div>
												<div id="cPA-tab-pane" class="tab-pane fade" role="tabpanel">
													<div class="card b-0 mh-0">
														<div class="card-body bg-body-secondary">
													
														</div>
														<div class="card-footer bg-body-tertiary">
															<div class="row">
																<div class="col-md-12">
																	<div class="input-group">
																		<span class="input-group-text fas fa-address-book"></span>
																		<asp:TextBox runat="server" ID="cPASignatureEmailAddress" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com" data-enter-btn="CPAAddRequestEmailAddress"></asp:TextBox>
																		<asp:Button runat="server" ID="CPAAddRequestEmailAddress" UseSubmitBehavior="false" CssClass="btn btn-success fas-btn" disabled="disabled" Text='&#xf055;' data-disable-btn="aspIconBtn" TabIndex="-1"/>
																		<%--OnClick="AddRequestEmailAddress_Click"--%>
																	</div>
																</div>
															</div>
														</div>
													</div>
												</div>
												<div id="obmDirector-tab-pane" class="tab-pane fade" role="tabpanel">
													<div class="card b-0 mh-0">
														<div class="card-body bg-body-secondary">
													
														</div>
														<div class="card-footer bg-body-tertiary">
															<div class="row">
																<div class="col-md-12">
																	<div class="input-group">
																		<span class="input-group-text fas fa-address-book"></span>
																		<asp:TextBox runat="server" ID="obmDirectorSignatureEmailAddress" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com" data-enter-btn="OBMDirectorAddRequestEmailAddress"></asp:TextBox>
																		<asp:Button runat="server" ID="OBMDirectorAddRequestEmailAddress" UseSubmitBehavior="false" CssClass="btn btn-success fas-btn" disabled="disabled" Text='&#xf055;' data-disable-btn="aspIconBtn" TabIndex="-1"/>
																		<%--OnClick="AddRequestEmailAddress_Click"--%>
																	</div>
																</div>
															</div>
														</div>
													</div>
												</div>
												<div id="mayor-tab-pane" class="tab-pane fade" role="tabpanel">
													<div class="card b-0 mh-0">
														<div class="card-body bg-body-secondary">
													
														</div>
														<div class="card-footer bg-body-tertiary">
															<div class="row">
																<div class="col-md-12">
																	<div class="input-group">
																		<span class="input-group-text fas fa-address-book"></span>
																		<asp:TextBox runat="server" ID="mayorSignatureEmailAddress" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com" data-enter-btn="MayorAddRequestEmailAddress"></asp:TextBox>
																		<asp:Button runat="server" ID="MayorAddRequestEmailAddress" UseSubmitBehavior="false" CssClass="btn btn-success fas-btn" disabled="disabled" Text='&#xf055;' data-disable-btn="aspIconBtn" TabIndex="-1"/>
																		<%--OnClick="AddRequestEmailAddress_Click"--%>
																	</div>
																</div>
															</div>
														</div>
													</div>
												</div>
											</div>
										</div>
									</div>
								</div>
							</div>
						</div>
					</div>
				</div>
			</ContentTemplate>
		</asp:UpdatePanel>
	</section>

	<script>
		InitialLoad();

		function InitialLoad() {
			CollapseTabs();
		}

		Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
			CollapseTabs();
		});
	</script>
</asp:Content>
