<%@ Page Title="Form Template" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FormTemplate.aspx.cs" Inherits="Themis.FormTemplate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <section class="container" style="background: url('../../assets/images/backgrounds/default.jpg') no-repeat center center !important; height: 225px; box-shadow: 5px 5px 5px 0px #0000007b; position: relative; z-index: 10;">
		<div class="overlay dark-4">
			<div class="display-table">
				<div class="display-table-cell align-middle">
					<div class="container text-center">
						<h1 class="nomargin size-50 weight-300" style="color: white;"><i class="fas fa-file-lines"></i>&nbsp;Form Template</h1>
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
               <div class="row border-top pt-4 mt-1">
                   <div class="col-12 col-sm-6 col-md-6">
                       <br />
                       <div class="form-label-group mb-3">
                           <label for="contact_name">Contact Name</label>
                           <input runat="server" id="contact_name" type="text" class="form-control">
                       </div>
                       <br />
                   </div>
                   <div class="col-12 col-sm-6 col-md-6">
                       <br />
                       <div class="form-label-group mb-3">
                           <label for="boring_employee">Employee Name</label>
                           <input runat="server" id="employee_name" type="text" class="form-control">
                       </div>
                       <br />
                   </div>
               </div>
               <div class="row">
                   <div class="col-12 col-sm-6 col-md-12">
                       <div class="form-label-group mb-3">
                           <label for="reason_why">List specific reasons why</label>
                           <asp:TextBox ID="reason_why" runat="server" TextMode="MultiLine" Rows="6" CssClass="form-control" />
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
                           <asp:Button ID="TemplateFormSubmit" runat="server" type="submit" class="btn btn-primary btn-adjust" Text="Submit" OnClick="TemplateFormSubmit_Click"></asp:Button>
                       </div>
                   </div>
               </div>
               <br />
               <br />
           </div>
       </div>
   </div>
</asp:Content>
