<%@ Page Title="Ordinance Admin" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OrdinanceAdmin.aspx.cs" Inherits="WebUI.OrdinanceAdmin" ClientIDMode="Static" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
	<%-- PAGE CONTENT --%>
	<section>
		<asp:UpdatePanel runat="server" ID="pnlAdmin" UpdateMode="Always" class="overlap-panels">
			<Triggers>
				<asp:AsyncPostBackTrigger ControlID="filterDepartment" EventName="SelectedIndexChanged" />
				<asp:AsyncPostBackTrigger ControlID="filterDivision" EventName="SelectedIndexChanged" />
			</Triggers>

			<ContentTemplate>
				<div runat="server" id="adminCard" class="card">
					<div class="card-header bg-body-secondary">
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
						<div id="adminTabsContent" class="tab-content tab-card" style="min-height: 85%">
							<div id="defaultEmails-tab-pane" class="tab-pane fade active show" role="tabpanel">
								<div class="card">
									<div class="card-header bg-body-secondary p-3">
										<h5><strong>Default Emails</strong></h5>
									</div>
									<div class="card-body bg-body-tertiary">
										<div class="row h-100">
											<div class="col-md-5">
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
															<button id="deletedBtn" class="nav-link" data-toggle="tab" data-target="#deleted-tab-pane" type="button" role="tab">Deleted</button>
														</li>
														<li id="statusEmailsMoreDD" class="nav-item dropdown">
															<button class="nav-link dropdown-toggle" data-toggle="dropdown" type="button" role="button">More</button>
															<div class="dropdown-menu">
															</div>
														</li>
													</ul>
													<div id="statusEmailsTabsContent" class="tab-content tab-card">
														<div id="pending-tab-pane" class="tab-pane fade active show" role="tabpanel">
															<div class="card">
																<div class="card-header bg-body p-3">
																	<strong>Pending</strong>
																</div>
																<div class="card-body bg-body">
													
																</div>
																<div class="card-footer bg-body-tertiary">
																	<div class="row">
																		<div class="col-md-12">
																			<div class="input-group">
																				<span class="input-group-text fas fa-address-book"></span>
																				<asp:TextBox runat="server" ID="pendingEmailAddress" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com" data-enter-btn="PendingAddEmailAddress"></asp:TextBox>
																				<asp:Button runat="server" ID="PendingAddEmailAddress" UseSubmitBehavior="false" CssClass="btn btn-success fas-btn" disabled="disabled" Text='&#xf055;' data-disable-btn="aspIconBtn" TabIndex="-1"/>
																				<%--OnClick="AddRequestEmailAddress_Click"--%>
																			</div>
																		</div>
																	</div>
																</div>
															</div>
														</div>
														<div id="underReview-tab-pane" class="tab-pane fade" role="tabpanel">
															<div class="card">
																<div class="card-header bg-body p-3">
																	<strong>Under Review</strong>
																</div>
																<div class="card-body bg-body">
													
																</div>
																<div class="card-footer bg-body-tertiary">
																	<div class="row">
																		<div class="col-md-12">
																			<div class="input-group">
																				<span class="input-group-text fas fa-address-book"></span>
																				<asp:TextBox runat="server" ID="underReviewEmailAddress" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com" data-enter-btn="UnderReviewAddEmailAddress"></asp:TextBox>
																				<asp:Button runat="server" ID="UnderReviewAddEmailAddress" UseSubmitBehavior="false" CssClass="btn btn-success fas-btn" disabled="disabled" Text='&#xf055;' data-disable-btn="aspIconBtn" TabIndex="-1"/>
																				<%--OnClick="AddRequestEmailAddress_Click"--%>
																			</div>
																		</div>
																	</div>
																</div>
															</div>
														</div>
														<div id="beingHeld-tab-pane" class="tab-pane fade" role="tabpanel">
															<div class="card">
																<div class="card-header bg-body p-3">
																	<strong>Being Held</strong>
																</div>
																<div class="card-body bg-body">
													
																</div>
																<div class="card-footer bg-body-tertiary">
																	<div class="row">
																		<div class="col-md-12">
																			<div class="input-group">
																				<span class="input-group-text fas fa-address-book"></span>
																				<asp:TextBox runat="server" ID="beingHeldEmailAddress" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com" data-enter-btn="BeingHeldAddEmailAddress"></asp:TextBox>
																				<asp:Button runat="server" ID="BeingHeldAddEmailAddress" UseSubmitBehavior="false" CssClass="btn btn-success fas-btn" disabled="disabled" Text='&#xf055;' data-disable-btn="aspIconBtn" TabIndex="-1"/>
																				<%--OnClick="AddRequestEmailAddress_Click"--%>
																			</div>
																		</div>
																	</div>
																</div>
															</div>
														</div>
														<div id="drafted-tab-pane" class="tab-pane fade" role="tabpanel">
															<div class="card">
																<div class="card-header bg-body p-3">
																	<strong>Drafted</strong>
																</div>
																<div class="card-body bg-body">
													
																</div>
																<div class="card-footer bg-body-tertiary">
																	<div class="row">
																		<div class="col-md-12">
																			<div class="input-group">
																				<span class="input-group-text fas fa-address-book"></span>
																				<asp:TextBox runat="server" ID="draftedEmailAddress" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com" data-enter-btn="DraftedAddEmailAddress"></asp:TextBox>
																				<asp:Button runat="server" ID="DraftedAddEmailAddress" UseSubmitBehavior="false" CssClass="btn btn-success fas-btn" disabled="disabled" Text='&#xf055;' data-disable-btn="aspIconBtn" TabIndex="-1"/>
																				<%--OnClick="AddRequestEmailAddress_Click"--%>
																			</div>
																		</div>
																	</div>
																</div>
															</div>
														</div>
														<div id="approved-tab-pane" class="tab-pane fade" role="tabpanel">
															<div class="card">
																<div class="card-header bg-body p-3">
																	<strong>Approved</strong>
																</div>
																<div class="card-body bg-body">
													
																</div>
																<div class="card-footer bg-body-tertiary">
																	<div class="row">
																		<div class="col-md-12">
																			<div class="input-group">
																				<span class="input-group-text fas fa-address-book"></span>
																				<asp:TextBox runat="server" ID="approvedEmailAddress" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com" data-enter-btn="ApprovedAddEmailAddress"></asp:TextBox>
																				<asp:Button runat="server" ID="ApprovedAddEmailAddress" UseSubmitBehavior="false" CssClass="btn btn-success fas-btn" disabled="disabled" Text='&#xf055;' data-disable-btn="aspIconBtn" TabIndex="-1"/>
																				<%--OnClick="AddRequestEmailAddress_Click"--%>
																			</div>
																		</div>
																	</div>
																</div>
															</div>
														</div>
														<div id="rejected-tab-pane" class="tab-pane fade" role="tabpanel">
															<div class="card">
																<div class="card-header bg-body p-3">
																	<strong>Rejected</strong>
																</div>
																<div class="card-body bg-body">
													
																</div>
																<div class="card-footer bg-body-tertiary">
																	<div class="row">
																		<div class="col-md-12">
																			<div class="input-group">
																				<span class="input-group-text fas fa-address-book"></span>
																				<asp:TextBox runat="server" ID="rejectedEmailAddress" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com" data-enter-btn="RejectedAddEmailAddress"></asp:TextBox>
																				<asp:Button runat="server" ID="RejectedAddEmailAddress" UseSubmitBehavior="false" CssClass="btn btn-success fas-btn" disabled="disabled" Text='&#xf055;' data-disable-btn="aspIconBtn" TabIndex="-1"/>
																				<%--OnClick="AddRequestEmailAddress_Click"--%>
																			</div>
																		</div>
																	</div>
																</div>
															</div>
														</div>
														<div id="deleted-tab-pane" class="tab-pane fade" role="tabpanel">
															<div class="card">
																<div class="card-header bg-body p-3">
																	<strong>Deleted</strong>
																</div>
																<div class="card-body bg-body">
													
																</div>
																<div class="card-footer bg-body-tertiary">
																	<div class="row">
																		<div class="col-md-12">
																			<div class="input-group">
																				<span class="input-group-text fas fa-address-book"></span>
																				<asp:TextBox runat="server" ID="deletedEmailAddress" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com" data-enter-btn="DeletedAddEmailAddress"></asp:TextBox>
																				<asp:Button runat="server" ID="DeletedAddEmailAddress" UseSubmitBehavior="false" CssClass="btn btn-success fas-btn" disabled="disabled" Text='&#xf055;' data-disable-btn="aspIconBtn" TabIndex="-1"/>
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
											<div class="col-md-7 border-start">
												<div id="sigEmailsDiv">
													<ul id="sigEmailsTabs" class="nav nav-tabs border-0" role="tablist" data-collapse-tabs="#sigEmailsMoreDD">
														<li class="nav-item">
															<button runat="server" id="fundsCheckByBtn" class="nav-link active" data-toggle="tab" data-target="#fundsCheckByTabPane" type="button" role="tab">Funds Check By</button>
														</li>
														<li class="nav-item">
															<button runat="server" id="directorSupervisorBtn" class="nav-link" data-toggle="tab" data-target="#directorSupervisorTabPane" type="button" role="tab">Director/Supervisor</button>
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
													<div id="sigEmailsTabsContent" class="tab-content tab-card">
														<div runat="server" id="fundsCheckByTabPane" class="tab-pane fade active show" role="tabpanel">
															<div class="card">
																<div class="card-header bg-body p-3"><strong>Funds Check By</strong></div>
																<div class="card-body bg-body">
													
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
														<div runat="server" id="directorSupervisorTabPane" class="tab-pane fade" role="tabpanel">
															<div class="card">
																<div class="card-header bg-body p-3">
																	<div class="row align-items-center">
																		<div class="col-md-3">
																			<strong>Director/Supervisor</strong>
																		</div>
																		<div class="col-md-9">
																			<div class="input-group">
																				<asp:DropDownList ID="filterDepartment" runat="server" AutoPostBack="true" CssClass="form-select" OnSelectedIndexChanged="Filter_SelectedIndexChanged" data-command="department" onchange="showLoadingModal();" data-filter="true"></asp:DropDownList>
																				<asp:DropDownList ID="filterDivision" runat="server" AutoPostBack="true" CssClass="form-select" OnSelectedIndexChanged="Filter_SelectedIndexChanged" data-command="division" onchange="showLoadingModal();" data-filter="true"></asp:DropDownList>
																			</div>
																		</div>
																	</div>
																</div>
																<div class="card-body bg-body">
													
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
															<div class="card">
																<div class="card-header bg-body p-3"><strong>City Purchasing Agent</strong></div>
																<div class="card-body bg-body">
													
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
															<div class="card">
																<div class="card-header bg-body p-3"><strong>OBM Director</strong></div>
																<div class="card-body bg-body">
													
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
															<div class="card">
																<div class="card-header bg-body p-3"><strong>Mayor</strong></div>
																<div class="card-body bg-body">
													
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
					</div>
				</div>
			</ContentTemplate>
		</asp:UpdatePanel>


		<%--<asp:Button runat="server" ID="CreateDefaultTypes" Text="Create Defaults" OnClick="CreateDefaultTypes_Click" CssClass="btn btn-info mt-5" />--%>
	</section>

	<script>
		InitialLoad();

		function InitialLoad() {
			//CollapseTabs(['adminTabs', 'statusEmailsTabs', 'sigEmailsTabs']);
			CollapseTabs();
		}

		Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
			//CollapseTabs(['adminTabs', 'statusEmailsTabs', 'sigEmailsTabs']);
			CollapseTabs();
		});
	</script>
</asp:Content>
