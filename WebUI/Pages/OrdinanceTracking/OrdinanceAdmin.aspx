﻿<%@ Page Title="Ordinance Admin" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OrdinanceAdmin.aspx.cs" Inherits="WebUI.OrdinanceAdmin" ClientIDMode="Static" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
	<%-- PAGE CONTENT --%>
	<section>
		<asp:UpdatePanel runat="server" ID="pnlAdmin" UpdateMode="Always" class="overlap-panels">
			<Triggers>
				<asp:AsyncPostBackTrigger ControlID="filterDepartment" EventName="SelectedIndexChanged" />
				<asp:AsyncPostBackTrigger ControlID="filterDivision" EventName="SelectedIndexChanged" />

				<asp:AsyncPostBackTrigger ControlID="PendingAddEmailAddress" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="UnderReviewAddEmailAddress" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="BeingHeldAddEmailAddress" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="DraftedAddEmailAddress" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="ApprovedAddEmailAddress" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="RejectedAddEmailAddress" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="DeletedAddEmailAddress" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="FundsCheckByAddDefaultEmailAddress" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="DirectorSupervisorAddDefaultEmailAddress" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="CPAAddDefaultEmailAddress" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="OBMDirectorAddDefaultEmailAddress" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="MayorAddDefaultEmailAddress" EventName="Click" />
				
			</Triggers>

			<ContentTemplate>
				<div runat="server" id="adminCard" class="card">
					<div class="card-header bg-body-secondary">
						<h3><i class="fa-kit fa-solid-user-crown-gear"></i>&nbsp;Ordinance Admin</h3>
					</div>
					<div class="card-body bg-body-tertiary">
						<ul id="adminTabs" class="nav nav-tabs border-0" role="tablist" data-collapse-tabs="#adminMoreDD">
							<li class="nav-item">
								<button runat="server" id="defaultEmailsBtn" class="nav-link active" data-toggle="tab" data-target="#defaultEmailsTabPane" type="button" role="tab">Default Emails</button>
							</li>
							<li id="adminMoreDD" class="nav-item dropdown">
								<button runat="server" id="adminMoreDDBtn" class="nav-link dropdown-toggle" data-toggle="dropdown" type="button" role="button">More</button>
								<div class="dropdown-menu">
								</div>
							</li>
						</ul>
						<div id="adminTabsContent" class="tab-content tab-card" style="min-height: 85%">
							<div runat="server" id="defaultEmailsTabPane" class="tab-pane fade active show" role="tabpanel">
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
															<button runat="server" id="pendingBtn" class="nav-link active" data-toggle="tab" data-target="#pendingTabPane" type="button" role="tab">Pending</button>
														</li>
														<li class="nav-item">
															<button runat="server" id="underReviewBtn" class="nav-link" data-toggle="tab" data-target="#underReviewTabPane" type="button" role="tab">Under Review</button>
														</li>
														<li class="nav-item">
															<button runat="server" id="beingHeldBtn" class="nav-link" data-toggle="tab" data-target="#beingHeldTabPane" type="button" role="tab">Being Held</button>
														</li>
														<li class="nav-item">
															<button runat="server" id="draftedBtn" class="nav-link" data-toggle="tab" data-target="#draftedTabPane" type="button" role="tab">Drafted</button>
														</li>
														<li class="nav-item">
															<button runat="server" id="approvedBtn" class="nav-link" data-toggle="tab" data-target="#approvedTabPane" type="button" role="tab">Approved</button>
														</li>
														<li class="nav-item">
															<button runat="server" id="rejectedBtn" class="nav-link" data-toggle="tab" data-target="#rejectedTabPane" type="button" role="tab">Rejected</button>
														</li>
														<li class="nav-item">
															<button runat="server" id="deletedBtn" class="nav-link" data-toggle="tab" data-target="#deletedTabPane" type="button" role="tab">Deleted</button>
														</li>
														<li id="statusEmailsMoreDD" class="nav-item dropdown">
															<button runat="server" id="statusEmailsMoreDDBtn" class="nav-link dropdown-toggle" data-toggle="dropdown" type="button" role="button">More</button>
															<div class="dropdown-menu">
															</div>
														</li>
													</ul>
													<div id="statusEmailsTabsContent" class="tab-content tab-card">
														<div runat="server" id="pendingTabPane" class="tab-pane fade active show" role="tabpanel">
															<div class="card">
																<div class="card-header bg-body p-3">
																	<strong>Pending</strong>
																</div>
																<div class="card-body bg-body">
																	<div runat="server" id="lblNoItemsPending" class="row text-center" style="margin-top: 12.5%;">
																		<div class="col-md-12">
																			<h5 class="text-danger">There are no default emails set for the current category</h5>
																		</div>
																	</div>
																	<asp:Repeater runat="server" ID="rpPendingDefaultList" OnItemCommand="rpDefaultList_ItemCommand" OnItemCreated="rpDefaultList_ItemCreated">
																		<ItemTemplate>
																			<div class="badge rounded-pill text-bg-secondary m-1" style="font-size: .95rem">
																				<%# Container.DataItem %>
																				<asp:LinkButton runat="server" ID="removeBtn" CssClass="text-danger" style="margin-left: 10px;" CommandName="Pending" CommandArgument='<%# Container.DataItem %>' TabIndex="-1"><span class="fa-solid fa-xmark" data-disable-btn="aspIconBtn"></span></asp:LinkButton>
																			</div>
																		</ItemTemplate>
																	</asp:Repeater>
																</div>
																<div class="card-footer bg-body-tertiary">
																	<div class="row">
																		<div class="col-md-12">
																			<div class="input-group">
																				<span class="input-group-text fas fa-address-book"></span>
																				<asp:TextBox runat="server" ID="pendingEmailAddress" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com" data-enter-btn="PendingAddEmailAddress"></asp:TextBox>
																				<asp:Button runat="server" ID="PendingAddEmailAddress" UseSubmitBehavior="false" CssClass="btn btn-success fas-btn" disabled="disabled" Text='&#xf055;' data-disable-btn="aspIconBtn" TabIndex="-1" OnClick="AddDefaultEmailAddress_Click" data-email-text="pendingEmailAddress" CommandName="Pending" CommandArgument="1"/>
																			</div>
																		</div>
																	</div>
																</div>
															</div>
														</div>
														<div runat="server" id="underReviewTabPane" class="tab-pane fade" role="tabpanel">
															<div class="card">
																<div class="card-header bg-body p-3">
																	<strong>Under Review</strong>
																</div>
																<div class="card-body bg-body">
																	<div runat="server" id="lblNoItemsUnderReview" class="row text-center" style="margin-top: 12.5%;">
																		<div class="col-md-12">
																			<h5 class="text-danger">There are no default emails set for the current category</h5>
																		</div>
																	</div>
																	<asp:Repeater runat="server" ID="rpUnderReviewDefaultList" OnItemCommand="rpDefaultList_ItemCommand" OnItemCreated="rpDefaultList_ItemCreated">
																		<ItemTemplate>
																			<div class="badge rounded-pill text-bg-secondary m-1" style="font-size: .95rem">
																				<%# Container.DataItem %>
																				<asp:LinkButton runat="server" ID="removeBtn" CssClass="text-danger" style="margin-left: 10px;" CommandName="UnderReview" CommandArgument='<%# Container.DataItem %>' TabIndex="-1"><span class="fa-solid fa-xmark" data-disable-btn="aspIconBtn"></span></asp:LinkButton>
																			</div>
																		</ItemTemplate>
																	</asp:Repeater>
																</div>
																<div class="card-footer bg-body-tertiary">
																	<div class="row">
																		<div class="col-md-12">
																			<div class="input-group">
																				<span class="input-group-text fas fa-address-book"></span>
																				<asp:TextBox runat="server" ID="underReviewEmailAddress" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com" data-enter-btn="UnderReviewAddEmailAddress"></asp:TextBox>
																				<asp:Button runat="server" ID="UnderReviewAddEmailAddress" UseSubmitBehavior="false" CssClass="btn btn-success fas-btn" disabled="disabled" Text='&#xf055;' data-disable-btn="aspIconBtn" TabIndex="-1" OnClick="AddDefaultEmailAddress_Click" data-email-text="underReviewEmailAddress" CommandName="Under Review" CommandArgument="2"/>
																			</div>
																		</div>
																	</div>
																</div>
															</div>
														</div>
														<div runat="server" id="beingHeldTabPane" class="tab-pane fade" role="tabpanel">
															<div class="card">
																<div class="card-header bg-body p-3">
																	<strong>Being Held</strong>
																</div>
																<div class="card-body bg-body">
																	<div runat="server" id="lblNoItemsBeingHeld" class="row text-center" style="margin-top: 12.5%;">
																		<div class="col-md-12">
																			<h5 class="text-danger">There are no default emails set for the current category</h5>
																		</div>
																	</div>
																	<asp:Repeater runat="server" ID="rpBeingHeldDefaultList" OnItemCommand="rpDefaultList_ItemCommand" OnItemCreated="rpDefaultList_ItemCreated">
																		<ItemTemplate>
																			<div class="badge rounded-pill text-bg-secondary m-1" style="font-size: .95rem">
																				<%# Container.DataItem %>
																				<asp:LinkButton runat="server" ID="removeBtn" CssClass="text-danger" style="margin-left: 10px;" CommandName="BeingHeld" CommandArgument='<%# Container.DataItem %>' TabIndex="-1"><span class="fa-solid fa-xmark" data-disable-btn="aspIconBtn"></span></asp:LinkButton>
																			</div>
																		</ItemTemplate>
																	</asp:Repeater>
																</div>
																<div class="card-footer bg-body-tertiary">
																	<div class="row">
																		<div class="col-md-12">
																			<div class="input-group">
																				<span class="input-group-text fas fa-address-book"></span>
																				<asp:TextBox runat="server" ID="beingHeldEmailAddress" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com" data-enter-btn="BeingHeldAddEmailAddress"></asp:TextBox>
																				<asp:Button runat="server" ID="BeingHeldAddEmailAddress" UseSubmitBehavior="false" CssClass="btn btn-success fas-btn" disabled="disabled" Text='&#xf055;' data-disable-btn="aspIconBtn" TabIndex="-1" OnClick="AddDefaultEmailAddress_Click" data-email-text="beingHeldEmailAddress" CommandName="Being Held" CommandArgument="3"/>
																			</div>
																		</div>
																	</div>
																</div>
															</div>
														</div>
														<div runat="server" id="draftedTabPane" class="tab-pane fade" role="tabpanel">
															<div class="card">
																<div class="card-header bg-body p-3">
																	<strong>Drafted</strong>
																</div>
																<div class="card-body bg-body">
																	<div runat="server" id="lblNoItemsDrafted" class="row text-center" style="margin-top: 12.5%;">
																		<div class="col-md-12">
																			<h5 class="text-danger">There are no default emails set for the current category</h5>
																		</div>
																	</div>
																	<asp:Repeater runat="server" ID="rpDraftedDefaultList" OnItemCommand="rpDefaultList_ItemCommand" OnItemCreated="rpDefaultList_ItemCreated">
																		<ItemTemplate>
																			<div class="badge rounded-pill text-bg-secondary m-1" style="font-size: .95rem">
																				<%# Container.DataItem %>
																				<asp:LinkButton runat="server" ID="removeBtn" CssClass="text-danger" style="margin-left: 10px;" CommandName="Drafted" CommandArgument='<%# Container.DataItem %>' TabIndex="-1"><span class="fa-solid fa-xmark" data-disable-btn="aspIconBtn"></span></asp:LinkButton>
																			</div>
																		</ItemTemplate>
																	</asp:Repeater>
																</div>
																<div class="card-footer bg-body-tertiary">
																	<div class="row">
																		<div class="col-md-12">
																			<div class="input-group">
																				<span class="input-group-text fas fa-address-book"></span>
																				<asp:TextBox runat="server" ID="draftedEmailAddress" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com" data-enter-btn="DraftedAddEmailAddress"></asp:TextBox>
																				<asp:Button runat="server" ID="DraftedAddEmailAddress" UseSubmitBehavior="false" CssClass="btn btn-success fas-btn" disabled="disabled" Text='&#xf055;' data-disable-btn="aspIconBtn" TabIndex="-1" OnClick="AddDefaultEmailAddress_Click" data-email-text="draftedEmailAddress" CommandName="Drafted" CommandArgument="4"/>
																			</div>
																		</div>
																	</div>
																</div>
															</div>
														</div>
														<div runat="server" id="approvedTabPane" class="tab-pane fade" role="tabpanel">
															<div class="card">
																<div class="card-header bg-body p-3">
																	<strong>Approved</strong>
																</div>
																<div class="card-body bg-body">
																	<div runat="server" id="lblNoItemsApproved" class="row text-center" style="margin-top: 12.5%;">
																		<div class="col-md-12">
																			<h5 class="text-danger">There are no default emails set for the current category</h5>
																		</div>
																	</div>
																	<asp:Repeater runat="server" ID="rpApprovedDefaultList" OnItemCommand="rpDefaultList_ItemCommand" OnItemCreated="rpDefaultList_ItemCreated">
																		<ItemTemplate>
																			<div class="badge rounded-pill text-bg-secondary m-1" style="font-size: .95rem">
																				<%# Container.DataItem %>
																				<asp:LinkButton runat="server" ID="removeBtn" CssClass="text-danger" style="margin-left: 10px;" CommandName="Approved" CommandArgument='<%# Container.DataItem %>' TabIndex="-1"><span class="fa-solid fa-xmark" data-disable-btn="aspIconBtn"></span></asp:LinkButton>
																			</div>
																		</ItemTemplate>
																	</asp:Repeater>
																</div>
																<div class="card-footer bg-body-tertiary">
																	<div class="row">
																		<div class="col-md-12">
																			<div class="input-group">
																				<span class="input-group-text fas fa-address-book"></span>
																				<asp:TextBox runat="server" ID="approvedEmailAddress" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com" data-enter-btn="ApprovedAddEmailAddress"></asp:TextBox>
																				<asp:Button runat="server" ID="ApprovedAddEmailAddress" UseSubmitBehavior="false" CssClass="btn btn-success fas-btn" disabled="disabled" Text='&#xf055;' data-disable-btn="aspIconBtn" TabIndex="-1" OnClick="AddDefaultEmailAddress_Click" data-email-text="approvedEmailAddress" CommandName="Approved" CommandArgument="5"/>
																			</div>
																		</div>
																	</div>
																</div>
															</div>
														</div>
														<div runat="server" id="rejectedTabPane" class="tab-pane fade" role="tabpanel">
															<div class="card">
																<div class="card-header bg-body p-3">
																	<strong>Rejected</strong>
																</div>
																<div class="card-body bg-body">
																	<div runat="server" id="lblNoItemsRejected" class="row text-center" style="margin-top: 12.5%;">
																		<div class="col-md-12">
																			<h5 class="text-danger">There are no default emails set for the current category</h5>
																		</div>
																	</div>
																	<asp:Repeater runat="server" ID="rpRejectedDefaultList" OnItemCommand="rpDefaultList_ItemCommand" OnItemCreated="rpDefaultList_ItemCreated">
																		<ItemTemplate>
																			<div class="badge rounded-pill text-bg-secondary m-1" style="font-size: .95rem">
																				<%# Container.DataItem %>
																				<asp:LinkButton runat="server" ID="removeBtn" CssClass="text-danger" style="margin-left: 10px;" CommandName="Rejected" CommandArgument='<%# Container.DataItem %>' TabIndex="-1"><span class="fa-solid fa-xmark" data-disable-btn="aspIconBtn"></span></asp:LinkButton>
																			</div>
																		</ItemTemplate>
																	</asp:Repeater>
																</div>
																<div class="card-footer bg-body-tertiary">
																	<div class="row">
																		<div class="col-md-12">
																			<div class="input-group">
																				<span class="input-group-text fas fa-address-book"></span>
																				<asp:TextBox runat="server" ID="rejectedEmailAddress" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com" data-enter-btn="RejectedAddEmailAddress"></asp:TextBox>
																				<asp:Button runat="server" ID="RejectedAddEmailAddress" UseSubmitBehavior="false" CssClass="btn btn-success fas-btn" disabled="disabled" Text='&#xf055;' data-disable-btn="aspIconBtn" TabIndex="-1" OnClick="AddDefaultEmailAddress_Click" data-email-text="rejectedEmailAddress" CommandName="Rejected" CommandArgument="6"/>
																			</div>
																		</div>
																	</div>
																</div>
															</div>
														</div>
														<div runat="server" id="deletedTabPane" class="tab-pane fade" role="tabpanel">
															<div class="card">
																<div class="card-header bg-body p-3">
																	<strong>Deleted</strong>
																</div>
																<div class="card-body bg-body">
																	<div runat="server" id="lblNoItemsDeleted" class="row text-center" style="margin-top: 12.5%;">
																		<div class="col-md-12">
																			<h5 class="text-danger">There are no default emails set for the current category</h5>
																		</div>
																	</div>
																	<asp:Repeater runat="server" ID="rpDeletedDefaultList" OnItemCommand="rpDefaultList_ItemCommand" OnItemCreated="rpDefaultList_ItemCreated">
																		<ItemTemplate>
																			<div class="badge rounded-pill text-bg-secondary m-1" style="font-size: .95rem">
																				<%# Container.DataItem %>
																				<asp:LinkButton runat="server" ID="removeBtn" CssClass="text-danger" style="margin-left: 10px;" CommandName="Deleted" CommandArgument='<%# Container.DataItem %>' TabIndex="-1"><span class="fa-solid fa-xmark" data-disable-btn="aspIconBtn"></span></asp:LinkButton>
																			</div>
																		</ItemTemplate>
																	</asp:Repeater>
																</div>
																<div class="card-footer bg-body-tertiary">
																	<div class="row">
																		<div class="col-md-12">
																			<div class="input-group">
																				<span class="input-group-text fas fa-address-book"></span>
																				<asp:TextBox runat="server" ID="deletedEmailAddress" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com" data-enter-btn="DeletedAddEmailAddress"></asp:TextBox>
																				<asp:Button runat="server" ID="DeletedAddEmailAddress" UseSubmitBehavior="false" CssClass="btn btn-success fas-btn" disabled="disabled" Text='&#xf055;' data-disable-btn="aspIconBtn" TabIndex="-1" OnClick="AddDefaultEmailAddress_Click" data-email-text="deletedEmailAddress" CommandName="Deleted" CommandArgument="7"/>
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
															<button runat="server" id="cPABtn" class="nav-link" data-toggle="tab" data-target="#cPATabPane" type="button" role="tab">City Purchasing Agent</button>
														</li>
														<li class="nav-item">
															<button runat="server" id="obmDirectorBtn" class="nav-link" data-toggle="tab" data-target="#obmDirectorTabPane" type="button" role="tab">OBM Director</button>
														</li>
														<li class="nav-item">
															<button runat="server" id="budgetBtn" class="nav-link" data-toggle="tab" data-target="#budgetTabPane" type="button" role="tab">Budget</button>
														</li>
														<li class="nav-item">
															<button runat="server" id="mayorBtn" class="nav-link" data-toggle="tab" data-target="#mayorTabPane" type="button" role="tab">Mayor</button>
														</li>
														<li class="nav-item">
															<button runat="server" id="ccDirectorBtn" class="nav-link" data-toggle="tab" data-target="#ccDirectorTabPane" type="button" role="tab">Corporation Counsel Director</button>
														</li>
														<li id="sigEmailsMoreDD" class="nav-item dropdown">
															<button runat="server" id="sigEmailsMoreDDBtn" class="nav-link dropdown-toggle" data-toggle="dropdown" type="button" role="button">More</button>
															<div class="dropdown-menu">
															</div>
														</li>
													</ul>
													<div id="sigEmailsTabsContent" class="tab-content tab-card">
														<div runat="server" id="fundsCheckByTabPane" class="tab-pane fade active show" role="tabpanel">
															<div class="card">
																<div class="card-header bg-body p-3"><strong>Funds Check By</strong></div>
																<div class="card-body bg-body">
																	<div runat="server" id="lblNoItemsFundsCheckBy" class="row text-center" style="margin-top: 12.5%;">
																		<div class="col-md-12">
																			<h5 class="text-danger">There are no default emails set for the current category</h5>
																		</div>
																	</div>
																	<asp:Repeater runat="server" ID="rpFundsCheckByDefaultList" OnItemCommand="rpDefaultList_ItemCommand" OnItemCreated="rpDefaultList_ItemCreated">
																		<ItemTemplate>
																			<div class="badge rounded-pill text-bg-secondary m-1" style="font-size: .95rem">
																				<%# Container.DataItem %>
																				<asp:LinkButton runat="server" ID="removeBtn" CssClass="text-danger" style="margin-left: 10px;" CommandName="FundsCheckBy" CommandArgument='<%# Container.DataItem %>' TabIndex="-1"><span class="fa-solid fa-xmark" data-disable-btn="aspIconBtn"></span></asp:LinkButton>
																			</div>
																		</ItemTemplate>
																	</asp:Repeater>
																</div>
																<div class="card-footer bg-body-tertiary">
																	<div class="row">
																		<div class="col-md-12">
																			<div class="input-group">
																				<span class="input-group-text fas fa-address-book"></span>
																				<asp:TextBox runat="server" ID="fundsCheckByDefaultEmailAddress" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com" data-enter-btn="FundsCheckByAddDefaultEmailAddress"></asp:TextBox>
																				<asp:Button runat="server" ID="FundsCheckByAddDefaultEmailAddress" UseSubmitBehavior="false" CssClass="btn btn-success fas-btn" disabled="disabled" Text='&#xf055;' data-disable-btn="aspIconBtn" TabIndex="-1" OnClick="AddDefaultEmailAddress_Click" data-email-text="fundsCheckByDefaultEmailAddress" CommandName="Funds Check By" CommandArgument="8"/>
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
																				<asp:DropDownList ID="filterDepartment" runat="server" AutoPostBack="true" CssClass="form-select" OnSelectedIndexChanged="Filter_SelectedIndexChanged" data-command="department" data-type="DirectorSupervisor" onchange="showLoadingModal();" data-filter="true"></asp:DropDownList>
																				<asp:DropDownList ID="filterDivision" runat="server" AutoPostBack="true" CssClass="form-select" OnSelectedIndexChanged="Filter_SelectedIndexChanged" data-command="division" data-type="DirectorSupervisor" onchange="showLoadingModal();" data-filter="true"></asp:DropDownList>
																			</div>
																		</div>
																	</div>
																</div>
																<div class="card-body bg-body">
																	<div runat="server" id="lblNoItemsDirectorSupervisor" class="row text-center" style="margin-top: 12.5%;">
																		<div class="col-md-12">
																			<h5 runat="server" id="lblNoItemsTxtDirectorSupervisor" class="text-gray">Please Select a Department and/or Division</h5>
																		</div>
																	</div>
																	<asp:Repeater runat="server" ID="rpDirectorSupervisorDefaultList" OnItemCommand="rpDefaultList_ItemCommand" OnItemCreated="rpDefaultList_ItemCreated">
																		<ItemTemplate>
																			<div class="badge rounded-pill text-bg-secondary m-1" style="font-size: .95rem">
																				<%# Container.DataItem %>
																				<asp:LinkButton runat="server" ID="removeBtn" CssClass="text-danger" style="margin-left: 10px;" CommandName="DirectorSupervisor" CommandArgument='<%# Container.DataItem %>' TabIndex="-1"><span class="fa-solid fa-xmark" data-disable-btn="aspIconBtn"></span></asp:LinkButton>
																			</div>
																		</ItemTemplate>
																	</asp:Repeater>
																</div>
																<div class="card-footer bg-body-tertiary">
																	<div class="row">
																		<div class="col-md-12">
																			<div class="input-group">
																				<span class="input-group-text fas fa-address-book"></span>
																				<asp:TextBox runat="server" ID="directorSupervisorDefaultEmailAddress" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com" data-enter-btn="DirectorSupervisorAddDefaultEmailAddress"></asp:TextBox>
																				<asp:Button runat="server" ID="DirectorSupervisorAddDefaultEmailAddress" UseSubmitBehavior="false" CssClass="btn btn-success fas-btn" disabled="disabled" Text='&#xf055;' data-disable-btn="aspIconBtn" TabIndex="-1" OnClick="AddDefaultEmailAddress_Click" data-email-text="directorSupervisorDefaultEmailAddress" CommandName="Director/Supervisor"/>
																			</div>
																		</div>
																	</div>
																</div>
															</div>
														</div>
														<div runat="server" id="cPATabPane" class="tab-pane fade" role="tabpanel">
															<div class="card">
																<div class="card-header bg-body p-3"><strong>City Purchasing Agent</strong></div>
																<div class="card-body bg-body">
																	<div runat="server" id="lblNoItemsCityPurchasingAgent" class="row text-center" style="margin-top: 12.5%;">
																		<div class="col-md-12">
																			<h5 class="text-danger">There are no default emails set for the current category</h5>
																		</div>
																	</div>
																	<asp:Repeater runat="server" ID="rpCPADefaultList" OnItemCommand="rpDefaultList_ItemCommand" OnItemCreated="rpDefaultList_ItemCreated">
																		<ItemTemplate>
																			<div class="badge rounded-pill text-bg-secondary m-1" style="font-size: .95rem">
																				<%# Container.DataItem %>
																				<asp:LinkButton runat="server" ID="removeBtn" CssClass="text-danger" style="margin-left: 10px;" CommandName="CPA" CommandArgument='<%# Container.DataItem %>' TabIndex="-1"><span class="fa-solid fa-xmark" data-disable-btn="aspIconBtn"></span></asp:LinkButton>
																			</div>
																		</ItemTemplate>
																	</asp:Repeater>
																</div>
																<div class="card-footer bg-body-tertiary">
																	<div class="row">
																		<div class="col-md-12">
																			<div class="input-group">
																				<span class="input-group-text fas fa-address-book"></span>
																				<asp:TextBox runat="server" ID="cPADefaultEmailAddress" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com" data-enter-btn="CPAAddDefaultEmailAddress"></asp:TextBox>
																				<asp:Button runat="server" ID="CPAAddDefaultEmailAddress" UseSubmitBehavior="false" CssClass="btn btn-success fas-btn" disabled="disabled" Text='&#xf055;' data-disable-btn="aspIconBtn" TabIndex="-1" OnClick="AddDefaultEmailAddress_Click" data-email-text="cPADefaultEmailAddress" CommandName="City Purchasing Agent" CommandArgument="9"/>
																			</div>
																		</div>
																	</div>
																</div>
															</div>
														</div>
														<div runat="server" id="obmDirectorTabPane" class="tab-pane fade" role="tabpanel">
															<div class="card">
																<div class="card-header bg-body p-3"><strong>OBM Director</strong></div>
																<div class="card-body bg-body">
																	<div runat="server" id="lblNoItemsOBMDirector" class="row text-center" style="margin-top: 12.5%;">
																		<div class="col-md-12">
																			<h5 class="text-danger">There are no default emails set for the current category</h5>
																		</div>
																	</div>
																	<asp:Repeater runat="server" ID="rpOBMDirectorDefaultList" OnItemCommand="rpDefaultList_ItemCommand" OnItemCreated="rpDefaultList_ItemCreated">
																		<ItemTemplate>
																			<div class="badge rounded-pill text-bg-secondary m-1" style="font-size: .95rem">
																				<%# Container.DataItem %>
																				<asp:LinkButton runat="server" ID="removeBtn" CssClass="text-danger" style="margin-left: 10px;" CommandName="OBMDirector" CommandArgument='<%# Container.DataItem %>' TabIndex="-1"><span class="fa-solid fa-xmark" data-disable-btn="aspIconBtn"></span></asp:LinkButton>
																			</div>
																		</ItemTemplate>
																	</asp:Repeater>
																</div>
																<div class="card-footer bg-body-tertiary">
																	<div class="row">
																		<div class="col-md-12">
																			<div class="input-group">
																				<span class="input-group-text fas fa-address-book"></span>
																				<asp:TextBox runat="server" ID="obmDirectorDefaultEmailAddress" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com" data-enter-btn="OBMDirectorAddDefaultEmailAddress"></asp:TextBox>
																				<asp:Button runat="server" ID="OBMDirectorAddDefaultEmailAddress" UseSubmitBehavior="false" CssClass="btn btn-success fas-btn" disabled="disabled" Text='&#xf055;' data-disable-btn="aspIconBtn" TabIndex="-1" OnClick="AddDefaultEmailAddress_Click" data-email-text="obmDirectorDefaultEmailAddress" CommandName="OBM Director" CommandArgument="10"/>
																			</div>
																		</div>
																	</div>
																</div>
															</div>
														</div>
														<div runat="server" id="budgetTabPane" class="tab-pane fade" role="tabpanel">
															<div class="card">
																<div class="card-header bg-body p-3">
																	<div class="row align-items-center">
																		<div class="col-md-3">
																			<strong>Budget</strong>
																		</div>
																		<div class="col-md-9">
																			<asp:DropDownList ID="filterBusiness" runat="server" AutoPostBack="true" CssClass="form-select" OnSelectedIndexChanged="Filter_SelectedIndexChanged" data-command="budget" data-type="Budget" onchange="showLoadingModal();" data-filter="true"></asp:DropDownList>
																		</div>
																	</div>
																</div>
																<div class="card-body bg-body">
																	<div runat="server" id="lblNoItemsBudget" class="row text-center" style="margin-top: 12.5%;">
																		<div class="col-md-12">
																			<h5 runat="server" id="lblNoItemsTxtBudget" class="text-gray">Please Select a Business</h5>
																		</div>
																	</div>
																	<asp:Repeater runat="server" ID="rpBudgetDefaultList" OnItemCommand="rpDefaultList_ItemCommand" OnItemCreated="rpDefaultList_ItemCreated">
																		<ItemTemplate>
																			<div class="badge rounded-pill text-bg-secondary m-1" style="font-size: .95rem">
																				<%# Container.DataItem %>
																				<asp:LinkButton runat="server" ID="removeBtn" CssClass="text-danger" style="margin-left: 10px;" CommandName="Budget" CommandArgument='<%# Container.DataItem %>' TabIndex="-1"><span class="fa-solid fa-xmark" data-disable-btn="aspIconBtn"></span></asp:LinkButton>
																			</div>
																		</ItemTemplate>
																	</asp:Repeater>
																</div>
																<div class="card-footer bg-body-tertiary">
																	<div class="row">
																		<div class="col-md-12">
																			<div class="input-group">
																				<span class="input-group-text fas fa-address-book"></span>
																				<asp:TextBox runat="server" ID="budgetDefaultEmailAddress" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com" data-enter-btn="BudgetAddDefaultEmailAddress"></asp:TextBox>
																				<asp:Button runat="server" ID="BudgetAddDefaultEmailAddress" UseSubmitBehavior="false" CssClass="btn btn-success fas-btn" disabled="disabled" Text='&#xf055;' data-disable-btn="aspIconBtn" TabIndex="-1" OnClick="AddDefaultEmailAddress_Click" data-email-text="budgetDefaultEmailAddress" CommandName="Budget"/>
																			</div>
																		</div>
																	</div>
																</div>
															</div>
														</div>
														<div runat="server" id="mayorTabPane" class="tab-pane fade" role="tabpanel">
															<div class="card">
																<div class="card-header bg-body p-3"><strong>Mayor</strong></div>
																<div class="card-body bg-body">
																	<div runat="server" id="lblNoItemsMayor" class="row text-center" style="margin-top: 12.5%;">
																		<div class="col-md-12">
																			<h5 class="text-danger">There are no default emails set for the current category</h5>
																		</div>
																	</div>
																	<asp:Repeater runat="server" ID="rpMayorDefaultList" OnItemCommand="rpDefaultList_ItemCommand" OnItemCreated="rpDefaultList_ItemCreated">
																		<ItemTemplate>
																			<div class="badge rounded-pill text-bg-secondary m-1" style="font-size: .95rem">
																				<%# Container.DataItem %>
																				<asp:LinkButton runat="server" ID="removeBtn" CssClass="text-danger" style="margin-left: 10px;" CommandName="Mayor" CommandArgument='<%# Container.DataItem %>' TabIndex="-1"><span class="fa-solid fa-xmark" data-disable-btn="aspIconBtn"></span></asp:LinkButton>
																			</div>
																		</ItemTemplate>
																	</asp:Repeater>
																</div>
																<div class="card-footer bg-body-tertiary">
																	<div class="row">
																		<div class="col-md-12">
																			<div class="input-group">
																				<span class="input-group-text fas fa-address-book"></span>
																				<asp:TextBox runat="server" ID="mayorDefaultEmailAddress" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com" data-enter-btn="MayorAddDefaultEmailAddress"></asp:TextBox>
																				<asp:Button runat="server" ID="MayorAddDefaultEmailAddress" UseSubmitBehavior="false" CssClass="btn btn-success fas-btn" disabled="disabled" Text='&#xf055;' data-disable-btn="aspIconBtn" TabIndex="-1" OnClick="AddDefaultEmailAddress_Click" data-email-text="mayorDefaultEmailAddress" CommandName="Mayor" CommandArgument="13"/>
																			</div>
																		</div>
																	</div>
																</div>
															</div>
														</div>
														<div runat="server" id="ccDirectorTabPane" class="tab-pane fade" role="tabpanel">
															<div class="card">
																<div class="card-header bg-body p-3"><strong>Corporation Counsel Director</strong></div>
																<div class="card-body bg-body">
																	<div runat="server" id="lblNoItemsCCDirector" class="row text-center" style="margin-top: 12.5%;">
																		<div class="col-md-12">
																			<h5 class="text-danger">There are no default emails set for the current category</h5>
																		</div>
																	</div>
																	<asp:Repeater runat="server" ID="rpCCDirectorDefaultList" OnItemCommand="rpDefaultList_ItemCommand" OnItemCreated="rpDefaultList_ItemCreated">
																		<ItemTemplate>
																			<div class="badge rounded-pill text-bg-secondary m-1" style="font-size: .95rem">
																				<%# Container.DataItem %>
																				<asp:LinkButton runat="server" ID="removeBtn" CssClass="text-danger" style="margin-left: 10px;" CommandName="CCDirector" CommandArgument='<%# Container.DataItem %>' TabIndex="-1"><span class="fa-solid fa-xmark" data-disable-btn="aspIconBtn"></span></asp:LinkButton>
																			</div>
																		</ItemTemplate>
																	</asp:Repeater>
																</div>
																<div class="card-footer bg-body-tertiary">
																	<div class="row">
																		<div class="col-md-12">
																			<div class="input-group">
																				<span class="input-group-text fas fa-address-book"></span>
																				<asp:TextBox runat="server" ID="ccDirectorDefaultEmailAddress" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com" data-enter-btn="CCDirectorAddDefaultEmailAddress"></asp:TextBox>
																				<asp:Button runat="server" ID="CCDirectorAddDefaultEmailAddress" UseSubmitBehavior="false" CssClass="btn btn-success fas-btn" disabled="disabled" Text='&#xf055;' data-disable-btn="aspIconBtn" TabIndex="-1" OnClick="AddDefaultEmailAddress_Click" data-email-text="ccDirectorDefaultEmailAddress" CommandName="CCDirector" CommandArgument="14"/>
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
				<asp:HiddenField runat="server" ID="hdnActiveTabs" Value="defaultEmailsBtn,pendingBtn,fundsCheckByBtn" />
			</ContentTemplate>
		</asp:UpdatePanel>
	</section>

	<script>
		InitialLoad();

		function InitialLoad() {
			CollapseTabs();
			addSignatureEmails([
				{ addressID: '<%= pendingEmailAddress.ClientID %>', btnID: '<%= PendingAddEmailAddress.ClientID %>' },
				{ addressID: '<%= underReviewEmailAddress.ClientID %>', btnID: '<%= UnderReviewAddEmailAddress.ClientID %>' },
				{ addressID: '<%= beingHeldEmailAddress.ClientID %>', btnID: '<%= BeingHeldAddEmailAddress.ClientID %>' },
				{ addressID: '<%= draftedEmailAddress.ClientID %>', btnID: '<%= DraftedAddEmailAddress.ClientID %>' },
				{ addressID: '<%= approvedEmailAddress.ClientID %>', btnID: '<%= ApprovedAddEmailAddress.ClientID %>' },
				{ addressID: '<%= rejectedEmailAddress.ClientID %>', btnID: '<%= RejectedAddEmailAddress.ClientID %>' },
				{ addressID: '<%= deletedEmailAddress.ClientID %>', btnID: '<%= DeletedAddEmailAddress.ClientID %>' },
				{ addressID: '<%= fundsCheckByDefaultEmailAddress.ClientID %>', btnID: '<%= FundsCheckByAddDefaultEmailAddress.ClientID %>' },
				{ addressID: '<%= directorSupervisorDefaultEmailAddress.ClientID %>', btnID: '<%= DirectorSupervisorAddDefaultEmailAddress.ClientID %>' },
				{ addressID: '<%= cPADefaultEmailAddress.ClientID %>', btnID: '<%= CPAAddDefaultEmailAddress.ClientID %>' },
				{ addressID: '<%= obmDirectorDefaultEmailAddress.ClientID %>', btnID: '<%= OBMDirectorAddDefaultEmailAddress.ClientID %>' },
				{ addressID: '<%= budgetDefaultEmailAddress.ClientID %>', btnID: '<%= BudgetAddDefaultEmailAddress.ClientID %>' },
				{ addressID: '<%= mayorDefaultEmailAddress.ClientID %>', btnID: '<%= MayorAddDefaultEmailAddress.ClientID %>' },
				{ addressID: '<%= ccDirectorDefaultEmailAddress.ClientID %>', btnID: '<%= CCDirectorAddDefaultEmailAddress.ClientID %>' }
			]);
			FilterFirstItem();
			saveTabState();
		}

		Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
			CollapseTabs();
			addSignatureEmails([
				{ addressID: '<%= pendingEmailAddress.ClientID %>', btnID: '<%= PendingAddEmailAddress.ClientID %>' },
				{ addressID: '<%= underReviewEmailAddress.ClientID %>', btnID: '<%= UnderReviewAddEmailAddress.ClientID %>' },
				{ addressID: '<%= beingHeldEmailAddress.ClientID %>', btnID: '<%= BeingHeldAddEmailAddress.ClientID %>' },
				{ addressID: '<%= draftedEmailAddress.ClientID %>', btnID: '<%= DraftedAddEmailAddress.ClientID %>' },
				{ addressID: '<%= approvedEmailAddress.ClientID %>', btnID: '<%= ApprovedAddEmailAddress.ClientID %>' },
				{ addressID: '<%= rejectedEmailAddress.ClientID %>', btnID: '<%= RejectedAddEmailAddress.ClientID %>' },
				{ addressID: '<%= deletedEmailAddress.ClientID %>', btnID: '<%= DeletedAddEmailAddress.ClientID %>' },
				{ addressID: '<%= fundsCheckByDefaultEmailAddress.ClientID %>', btnID: '<%= FundsCheckByAddDefaultEmailAddress.ClientID %>' },
				{ addressID: '<%= directorSupervisorDefaultEmailAddress.ClientID %>', btnID: '<%= DirectorSupervisorAddDefaultEmailAddress.ClientID %>' },
				{ addressID: '<%= cPADefaultEmailAddress.ClientID %>', btnID: '<%= CPAAddDefaultEmailAddress.ClientID %>' },
				{ addressID: '<%= obmDirectorDefaultEmailAddress.ClientID %>', btnID: '<%= OBMDirectorAddDefaultEmailAddress.ClientID %>' },
				{ addressID: '<%= budgetDefaultEmailAddress.ClientID %>', btnID: '<%= BudgetAddDefaultEmailAddress.ClientID %>' },
				{ addressID: '<%= mayorDefaultEmailAddress.ClientID %>', btnID: '<%= MayorAddDefaultEmailAddress.ClientID %>' },
				{ addressID: '<%= ccDirectorDefaultEmailAddress.ClientID %>', btnID: '<%= CCDirectorAddDefaultEmailAddress.ClientID %>' }
			]);
			FilterFirstItem();
			saveTabState();
		});
	</script>
</asp:Content>