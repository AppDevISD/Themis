<%@ Page Title="Ordinance Request" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OrdinanceRequest.aspx.cs" Inherits="Themis.OrdinanceRequest" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <section class="container" style="background: url('../../assets/images/backgrounds/scales-bg.png') no-repeat center center !important; height: 225px; box-shadow: 5px 5px 5px 0px #0000007b; position: relative; z-index: 10;">
		<div class="overlay dark-7">
			<div class="display-table">
				<div class="display-table-cell align-middle">
					<div class="container text-center">
						<h1 class="nomargin size-50 weight-300" style="color: white;"><i class="fas fa-gavel"></i>&nbsp;Ordinance Request</h1>
					</div>
				</div>
			</div>
		</div>
	</section>
	<div class="container">
       <div class="col-md-12">
           <div class="col">
               <br />
               <p class="text-justify">
                   Et tellus suspendisse suscipit orci sit amet sem venenatis nec lobortis sem suscipit nullam nec imperdiet velit mauris eu nisi a felis imperdiet porta at ac nulla vivamus faucibus felis nec dolor pretium eget pellentesque dolor suscipit maecenas vitae enim arcu, at tincidunt nunc pellentesque eleifend vulputate lacus, vel semper sem ornare sit amet proin sem sapien, auctor vel faucibus id, aliquet vitae ipsum etiam auctor ultricies ante, at dapibus urna viverra sed nullam mi arcu, tempor vitae interdum a.
               </p>

               <p class="text-justify" style="color: gray;"><i class="fa-solid fa-asterisk"></i> = Required Field</p>
               <div class="row border-top pt-4 mt-1">
                   <div class="col-12 col-sm-6 col-md-5">
                        <br />
                        <div class="form-label-group mb-3">
                            <label for="dept_dd">Office Requesting <span class="required-field">*</span></label>
                            <asp:DropDownList ID="dept_dd" runat="server" AutoPostBack="true" OnSelectedIndexChanged="dept_dd_SelectedIndexChanged" CssClass="form-control">
                                <asp:ListItem selected="true" hidden="false" Value="" Text="Select Department..."></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <br />
                    </div>
                   <div class="col-12 col-sm-6 col-md-5">
                        <br />
                        <div id="div_div" class="form-label-group mb-3 disabled-control" runat="server">
                            <label for="div_dd" >Division</label>
                            <asp:DropDownList ID="div_dd" runat="server" AutoPostBack="true" OnSelectedIndexChanged="div_dd_SelectedIndexChanged" CssClass="form-control" Enabled="false">
                                <asp:ListItem selected="true">Select Division...</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <br />
                    </div>
                   <div class="col-6 col-sm-6 col-md-2">
                        <br />
                        <div class="form-label-group mb-3">
                            <label for="first_reading_date">Date of 1st Reading <span class="required-field">*</span></label>
                            <input runat="server" id="first_reading_date" type="date" class="form-control" autocomplete="off" required>
                        </div>
                        <br />
                    </div>
                   <div class="col-6 col-sm-4 col-md-4">
                       <br />
                       <div class="form-label-group mb-3">
                           <label for="contact_name">Contact Name <span class="required-field">*</span></label>
                           <input runat="server" id="contact_name" type="text" class="form-control" placeholder="John Doe" autocomplete="off" required>
                       </div>
                       <br />
                   </div>
                   <div class="col-6 col-sm-4 col-md-2">
                       <br />
                       <div class="form-label-group mb-3">
                           <label for="tel1">Phone Number <span class="required-field">*</span></label>
                           <input runat="server" id="tel1" type="tel" class="form-control" placeholder="(555) 123-4567" minlength="14" maxlength="14" autocomplete="off" required>
                       </div>
                       <br />
                   </div>
                   <div class="col-12 col-sm-1 col-md-1">
                        <br />
                        <div class="form-label-group mb-3">
                            <label for="ext1">Ext</label>
                            <input runat="server" id="ext1" type="text" class="form-control" placeholder="x1234" autocomplete="off">
                        </div>
                        <br />
                    </div>
               </div>
               <div class="row">
                   <div class="col-12 col-sm-6 col-md-12">
                       <div class="form-label-group mb-3">
                           <label for="emergency_passage">Emergency Passage</label>
                           <asp:TextBox ID="emergency_passage" runat="server" TextMode="MultiLine" Rows="6" CssClass="form-control" />
                       </div>
                       <br />
                   </div>
               </div>
               <div class="row">
                   <div class="col-12">
                       <div class="alert alert-success noborder text-center weight-400 nomargin noradius" id="divSuccess" visible="false" runat="server" style="font-weight: 900 !important;">
                           Request Submitted!
                       </div>
                       <div class="border-top pt-4 mt-1">
                           <br />
                           <asp:Button ID="TemplateFormSubmit" runat="server" type="submit" class="btn btn-primary btn-adjust" Text="Submit"></asp:Button> <%--ONCLICK--%>
                       </div>
                   </div>
               </div>
               <br />
               <br />
           </div>
       </div>
   </div>
</asp:Content>
