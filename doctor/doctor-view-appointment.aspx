<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="doctor-view-appointment.aspx.cs" Inherits="Web_Assignment.doctor_view_appointment" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Doctor Appointment Management</title>
    <link rel="stylesheet" href="../css/global.css" />
    <link rel="stylesheet" href="../css/doctor/doctor-appointment.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css" />
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <link
  rel="stylesheet"
  href="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" />

<script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
<script
  src="https://cdn.jsdelivr.net/npm/bootstrap@4.5.2/dist/js/bootstrap.bundle.min.js">
</script>

                                <script type="text/javascript">
                                    function googleTranslateElementInit() {
                                        new google.translate.TranslateElement(
                                            { pageLanguage: 'en' },
                                            'google_translate_element'
                                        );
                                    }
                                </script>

<script type="text/javascript" src="//translate.google.com/translate_a/element.js?cb=googleTranslateElementInit"></script>

</head>

<style>
    .sidebar h2 {
        font-family: 'Arial', sans-serif;
        font-size: 24px;display: block;
    font-size: 1.5em;
    margin-block-start: 0.83em;
    margin-block-end: 0.83em;
    margin-inline-start: 0px;
    margin-inline-end: 0px;
    font-weight: bold;
    unicode-bidi: isolate;
    }
    .dashboard-header  h1 {
        font-weight:bold;
    }
</style>
<body>
    <form id="form1" runat="server">
         <asp:ScriptManager ID="ScriptManager1" runat="server" />
         <!-- Sidebar -->
 <div class="sidebar">
     <h2>Doctor Panel</h2>
     <a href="doctor-dashboard.aspx">Dashboard <i class="fa-solid fa-house"></i></a>
     <a href="doctor-view-appointment.aspx"class="active">View Appointment <i class="fa-solid fa-calendar-check"></i></a>
     <a href="doctor-profile-management.aspx" >Profile Management <i class="fa-solid fa-user"></i></a>
     <a href="../index.aspx">Logout <i class="fas fa-sign-out-alt"></i></a>
                                 <!-- Translate Widget at the Bottom -->
     <div class="translate-container">
         <div id="google_translate_element"></div>
     </div>
 </div>

        <div class="dashboard-container">
            <!-- Header -->
            <div class="dashboard-header">
                <h1>Appointment Management</h1>
                <div class="date-label">
                    <i class="fa-solid fa-calendar"></i>
                    Today's Date: <asp:Label ID="lblCurrentDate" runat="server" />
                </div>
            </div>

            <!-- Manage Availability -->
            <div class="availability-section">
                <h2>Manage Availability</h2>
                <div class="form-group">
                    <label for="txtAvailabilityDate">Date:</label>
                    <asp:TextBox ID="txtAvailabilityDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label for="ddlStartHour">Start Hour:</label>
  <asp:DropDownList ID="ddlStartHour" runat="server" CssClass="form-control" />
                </div>
                <div class="form-group">
                     <label for="ddlEndHour">End Hour:</label>
  <asp:DropDownList ID="ddlEndHour" runat="server" CssClass="form-control" />
                </div>
                <asp:Button ID="btnSaveAvailability" runat="server" Text="Save" CssClass="btn-primary" OnClick="btnSaveAvailability_Click" />
            </div>

            <!-- Current Availability -->
            <div class="availability-section">
                <h3>Current Availability</h3>
<asp:GridView 
    ID="gvAvailability" 
    runat="server" 
    AutoGenerateColumns="False"
    DataKeyNames="ScheduleID"
    OnRowDeleting="gvAvailability_RowDeleting"
    AllowPaging="True"
    PageSize="10"
    OnPageIndexChanging="gvAvailability_PageIndexChanging">
    
    <Columns>
        <asp:BoundField DataField="Date" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}" />
        <asp:BoundField DataField="StartTime"   HeaderText="Start Time" />
        <asp:BoundField DataField="EndTime"     HeaderText="End Time" />
        <asp:CommandField ShowDeleteButton="True" ButtonType="Link" />
    </Columns>
</asp:GridView>

            </div>

            <!-- Appointment Summary Pie Chart -->
            <div class="chart-section">
                <h2>Appointment Summary</h2>
                <canvas id="appointmentChart"></canvas>
            </div>

            <!-- Filters -->
            <div class="filters">
                <asp:TextBox ID="txtSearch" runat="server" placeholder="Search by patient name..." CssClass="search-input" />
                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="filter-dropdown">
                    <asp:ListItem Text="All" Value="All" />
                    <asp:ListItem Text="Scheduled" Value="Scheduled" />
                    <asp:ListItem Text="Pending" Value="Pending" />
                    <asp:ListItem Text="Completed" Value="Completed" />
                    <asp:ListItem Text="Missed" Value="Missed" />
                    <asp:ListItem Text="Cancelled" Value="Cancelled" />
                </asp:DropDownList>
                <asp:Button ID="btnFilter" runat="server" Text="Apply Filters" CssClass="filter-btn" OnClick="btnFilter_Click" />
                <asp:Button ID="btnReset" runat="server" Text="Clear Filters" CssClass="reset-btn" OnClick="btnReset_Click" />
            </div>

            <!-- Appointment Grid -->
<asp:GridView 
    ID="gvAppointments" 
    runat="server"
    AutoGenerateColumns="False" 
    CssClass="appointment-grid" 
    AllowPaging="True" 
    OnPageIndexChanging="gvAppointments_PageIndexChanging"
    OnRowCommand="gvAppointments_RowCommand"
    DataKeyNames="AppointmentID">
  <Columns>
    <asp:BoundField DataField="PatientName" HeaderText="Patient" />
    <asp:BoundField DataField="AppointmentDate" HeaderText="Date" DataFormatString="{0:dd-MMM-yyyy}" />
    <asp:BoundField DataField="StartTime" HeaderText="Start Time" />
    <asp:BoundField DataField="Status" HeaderText="Status" />
    <asp:BoundField DataField="TreatmentType" HeaderText="Treatment" />

   <asp:TemplateField HeaderText="Actions">
  <ItemTemplate>
    <div class="btn-group" role="group" aria-label="Actions">
      
      <!-- Add Notes -->
      <asp:LinkButton 
         ID="lnkAddNotes" 
         runat="server"
         CssClass="btn btn-sm btn-info"
         CommandName="AddNotes" 
         CommandArgument='<%# Eval("PatientID") %>'
         ToolTip="Add Notes">
        <i class="fas fa-notes-medical"></i>
      </asp:LinkButton>

      <!-- Patient Details -->
      <asp:LinkButton 
         ID="lnkViewDetails" 
         runat="server"
         CssClass="btn btn-sm btn-primary"
         CommandName="PatientDetail" 
         CommandArgument='<%# Eval("PatientID") %>'
         ToolTip="View Patient">
        <i class="fas fa-user"></i>
      </asp:LinkButton>

      <!-- Mark Complete (only when Pending) -->
      <asp:LinkButton 
         ID="lnkMarkComplete" 
         runat="server"
         CssClass="btn btn-sm btn-success"
         CommandName="MarkComplete" 
         CommandArgument='<%# Eval("AppointmentID") %>'
         Visible='<%# Eval("Status").ToString() == "Pending" %>'
         ToolTip="Mark Complete">
        <i class="fas fa-check"></i>
      </asp:LinkButton>

      <!-- Mark Missed (only when Pending) -->
      <asp:LinkButton 
         ID="lnkMarkMissed" 
         runat="server"
         CssClass="btn btn-sm btn-danger"
         CommandName="MarkMissed" 
         CommandArgument='<%# Eval("AppointmentID") %>'
         Visible='<%# Eval("Status").ToString() == "Pending" %>'
         ToolTip="Mark Missed">
        <i class="fas fa-times"></i>
      </asp:LinkButton>

    </div>
  </ItemTemplate>
</asp:TemplateField>

  </Columns>
                        <EmptyDataTemplate>
                        <div class="no-data-message">
                            <i class="fas fa-info-circle"></i> No appointments found.
                        </div>
                    </EmptyDataTemplate>
</asp:GridView>


        </div>
       <asp:Panel 
    ID="pnlNotesModal" 
    runat="server"
    CssClass="modal fade" 
    ClientIDMode="Static"
    tabindex="-1" 
    role="dialog" 
    aria-labelledby="notesModalLabel" 
    aria-hidden="true">
  <div class="modal-dialog modal-lg" role="document">
    <div class="modal-content">
      <div class="modal-header">
        <h5 id="notesModalLabel" class="modal-title">Patient Notes</h5>
        <button type="button" class="close" data-dismiss="modal">
          <span>&times;</span>
        </button>
      </div>
      <div class="modal-body">

        <!-- Existing notes, inline edit -->
        <asp:GridView
            ID="gvNotesModal"
            runat="server"
            CssClass="table table-sm table-striped"
            AutoGenerateColumns="False"
            DataKeyNames="NoteID"
            OnRowEditing="gvNotesModal_RowEditing"
            OnRowUpdating="gvNotesModal_RowUpdating"
            OnRowCancelingEdit="gvNotesModal_RowCancelingEdit">
          <Columns>
            <asp:BoundField DataField="DateTime" HeaderText="Date/Time" DataFormatString="{0:dd-MMM-yyyy HH:mm}" ReadOnly="True" />
            <asp:TemplateField HeaderText="Assessment">
              <ItemTemplate><%# Eval("Assessment") %></ItemTemplate>
              <EditItemTemplate>
                <asp:TextBox ID="txtAssessmentEdit" runat="server" Text='<%# Bind("Assessment") %>' CssClass="form-control form-control-sm" />
              </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Prescription">
              <ItemTemplate><%# Eval("Prescription") %></ItemTemplate>
              <EditItemTemplate>
                <asp:TextBox ID="txtPrescriptionEdit" runat="server" Text='<%# Bind("Prescription") %>' CssClass="form-control form-control-sm" />
              </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Treatment">
              <ItemTemplate><%# Eval("Treatment") %></ItemTemplate>
              <EditItemTemplate>
                <asp:TextBox ID="txtTreatmentEdit" runat="server" Text='<%# Bind("Treatment") %>' CssClass="form-control form-control-sm" />
              </EditItemTemplate>
            </asp:TemplateField>
            <asp:CommandField ShowEditButton="True" ButtonType="Link" />
          </Columns>
        </asp:GridView>

        <hr />

        <!-- New note form -->
        <asp:Panel CssClass="form-row">
          <div class="form-group col-md-4">
            <label>Assessment</label>
            <asp:TextBox ID="txtNewAssessment" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2"  Style="width:700px;"/>
          </div>
          <div class="form-group col-md-4">
            <label>Prescription</label>
            <asp:TextBox ID="txtNewPrescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2"  Style="width:700px;"/>
          </div>
          <div class="form-group col-md-4">
            <label>Treatment</label>
            <asp:TextBox ID="txtNewTreatment" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" Style="width:700px;" />
          </div>
          <div class="form-group col-12 text-right">
            <asp:Button ID="btnSaveNewNote" runat="server" Text="Save Note" CssClass="btn btn-sm btn-primary" OnClick="btnSaveNewNote_Click" />
          </div>
        </asp:Panel>
        
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
      </div>
    </div>
  </div>
</asp:Panel>


        <asp:Panel
    ID="pnlPatientModal"
    runat="server"
    CssClass="modal fade"
    ClientIDMode="Static"
    tabindex="-1"
    role="dialog"
    aria-labelledby="patientModalLabel"
    aria-hidden="true">
  <div class="modal-dialog" role="document">
    <div class="modal-content">
      <div class="modal-header">
        <h5 id="patientModalLabel" class="modal-title">Patient Details</h5>
        <button type="button"
                class="close"
                data-dismiss="modal"
                aria-label="Close">
          <span aria-hidden="true">&times;</span>
        </button>
      </div>
      <div class="modal-body">
        <asp:DetailsView
            ID="dvPatientModal"
            runat="server"
            AutoGenerateRows="False"
            CssClass="table table-bordered">
          <Fields>
            <asp:BoundField DataField="PatientID"  HeaderText="Patient ID" />
            <asp:BoundField DataField="FullName"   HeaderText="Full Name" />
            <asp:BoundField DataField="ICNumber"   HeaderText="IC Number" />
            <asp:BoundField DataField="DOB"        HeaderText="DOB" DataFormatString="{0:yyyy-MM-dd}" />
            <asp:BoundField DataField="Gender"     HeaderText="Gender" />
            <asp:BoundField DataField="Email"      HeaderText="Email" />
            <asp:BoundField DataField="Phone"      HeaderText="Phone" />
            <asp:BoundField DataField="Address"    HeaderText="Address" />
            <asp:BoundField DataField="BloodType"  HeaderText="Blood Type" />
            <asp:BoundField DataField="Allergy"    HeaderText="Allergies" />
            <asp:BoundField DataField="MedicalCon" HeaderText="Medical Conditions" />
            <asp:BoundField DataField="EmerName"   HeaderText="Emergency Contact Name" />
            <asp:BoundField DataField="EmerRship"  HeaderText="Relationship" />
            <asp:BoundField DataField="EmerPhone"  HeaderText="Emergency Phone" />
          </Fields>
        </asp:DetailsView>
      </div>
      <div class="modal-footer">
        <button type="button"
                class="btn btn-secondary"
                data-dismiss="modal">
          Close
        </button>
      </div>
    </div>
  </div>
</asp:Panel>

    </form>

    <script>
        // Injected chart rendering from backend
    </script>
</body>
</html>