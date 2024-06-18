<%@ Page Title="Table Template" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TableTemplate.aspx.cs" Inherits="Themis.TableTemplate" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row" style="padding: 50px">
    <div class="col-md-12">
        <div class="panel panel-primary" runat="server" id="pnlResults">
            <div class="panel-heading">
                <h3 style="font-size: 20px;"><i class="fas fa-table"></i>&nbsp;&nbsp;&nbsp;&nbsp;Template Table</h3>
            </div>
            <div class="panel-body" style="padding: 0px">
                <asp:Repeater runat="server" ID="rpCustomFormTickets" OnItemCommand="rpCustomFormTickets_ItemCommand">
                    <HeaderTemplate>
                        <table id="FormTable" class="table table-bordered table-striped table-hover text-center" style="padding: 0px; margin: 0px">
                            <thead>
                                <tr>
                                    <th style="width: 10%; text-align: center"><strong>Date</strong></th>
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
                                <asp:HiddenField runat="server" ID="hdnID" Value='<%# DataBinder.Eval(Container.DataItem, "FormID")%>' />
                                <asp:Label ID="date" Text='<%# DataBinder.Eval(Container.DataItem, "EffectiveDate", "{0:MM/dd/yyyy}") %>' runat="server"/>
                            </td>
                           <td style="vertical-align: middle;">
                                <asp:Label ID="formType" Text='<%# DataBinder.Eval(Container.DataItem, "FormTypeDesc") %>' runat="server"/>
                            </td>
                            <td style="vertical-align: middle;">
                                <asp:Label ID="contact" Text='<%# DataBinder.Eval(Container.DataItem, "ContactName") %>' runat="server"/>
                            </td>
                             <td style="vertical-align: middle;">
                                <asp:Label ID="employee" Text='<%# DataBinder.Eval(Container.DataItem, "EmployeeName") %>' runat="server"/>
                            </td>
                            <td style="vertical-align: middle;">
                                <asp:Label ID="notes" Text='<%# DataBinder.Eval(Container.DataItem, "Comments") %>' runat="server"/>
                            </td>
                            <td style="vertical-align: middle;">
                                <asp:Button runat="server" ID="delete" CommandName="delete" CausesValidation="false" CssClass="btn btn-danger btn-sm" Text="Delete" Width="90%" autopostback="false" />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
                <asp:Panel ID="pnlPagingP" CssClass="panel" Style="margin-top: 0px; margin-bottom: 0px" runat="server" Visible="false">
                    <table class="table" runat="server">
                        <tr>
                            <td class="text-left">
                                <asp:Button ID="lnkFirstSearchP" CssClass="btn btn-primary" runat="server" CausesValidation="false" OnClick="lnkFirstSearchP_Click" Style="width: 150px" Text="<< First Page"></asp:Button>
                            </td>
                            <td class="text-center">
                                <asp:Button ID="lnkPreviousSearchP" CssClass="btn btn-primary" runat="server" CausesValidation="false" OnClick="lnkPreviousSearchP_Click" Style="width: 150px" Text="< Prev Page"></asp:Button>
                            </td>
                            <td class="text-center">
                                <div style="margin-top: 5px">
                                    <asp:Label Style="font-weight: bold; font-size: 18px" ID="lblCurrentPageBottomSearchP" runat="server"></asp:Label>
                                </div>
                            </td>
                            <td class="text-center">
                                <asp:Button ID="lnkNextSearchP" CssClass="btn btn-primary" runat="server" CausesValidation="false" OnClick="lnkNextSearchP_Click" Style="width: 150px" Text="Next Page >"></asp:Button>
                            </td>
                            <td class="text-right">
                                <asp:Button ID="lnkLastSearchP" CssClass="btn btn-primary" runat="server" CausesValidation="false" OnClick="lnkLastSearchP_Click" Style="width: 150px" Text="Last Page >>"></asp:Button>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </div>
        </div>
    </div>
</div>
<div class="row">
    <div class="col-md-12">
        <asp:Label runat="server" ID="lblModalTarget1" Text=" "></asp:Label>
        <asp:ModalPopupExtender runat="server" ID="deleteModal" TargetControlID="lblModalTarget1" ClientIDMode="AutoID"
            PopupControlID="deleteModalPnl" RepositionMode="RepositionOnWindowResize" CancelControlID="bCloseDeleteMdl"
            Drag="False" DropShadow="True" BackgroundCssClass="modalBackgroundLight">
        </asp:ModalPopupExtender>
        <asp:Panel runat="server" ID="deleteModalPnl" CssClass="panel panel-primary" Style="width: 800px">
            <div class="panel-heading">
                <div class="row">
                    <div class="col-md-10">
                        <h3>&nbsp;&nbsp;<i class="fa fa-exclamation-triangle"></i>&nbsp;&nbsp;&nbsp;&nbsp;Are you sure you want to delete this record?</h3>
                    </div>
                    <div class="col-md-2">
                        <button runat="server" id="bCloseDeleteMdl"
                            type="button" causesvalidation="false" class="close" aria-label="Close">
                            <span aria-hidden="true">&times;</span></button>
                    </div>
                </div>
            </div>
            <div class="panel-body">
                <div class="row">
                    <div class="col-md-6">
                        <button type="button" class="btn btn-danger" style="width: 100%;" runat="server" id="mdlDeleteSubmit" causesvalidation="false" onserverclick="mdlDeleteSubmit_ServerClick">Confirm Delete</button>
                    </div>
                    <div class="col-md-6">
                        <button type="button" class="btn btn-default" style="width: 100%;" runat="server" id="mdlCancelBtn" causesvalidation="false" onserverclick="mdlCancelBtn_ServerClick">Cancel</button>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>
</div>
</asp:Content>
