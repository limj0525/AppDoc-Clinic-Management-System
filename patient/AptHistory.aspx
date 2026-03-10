<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AptHistory.aspx.cs" Inherits="patientsystem.AptHistory" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Appointment History</title>
    <link rel="stylesheet" href="../css/global.css">
    <link rel="stylesheet" href="../css/patient/patient-history.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css">
                        <script type="text/javascript">
                        function googleTranslateElementInit() {
                            new google.translate.TranslateElement(
                                { pageLanguage: 'en' },
                                'google_translate_element'
                            );
                        }
                        </script>

<script type="text/javascript" src="//translate.google.com/translate_a/element.js?cb=googleTranslateElementInit"></script>
    <style>
        /* Card Styling */
.history-card {
    margin: 20px auto;
    padding: 20px;
    border-radius: 8px;
    background-color: white;
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
    max-width: 1200px;
    box-sizing: border-box;
}

/* Card Header Styling */
.card-header {
    padding-bottom: 20px;
    border-bottom: 1px solid #eee;
    margin-bottom: 20px;
}

.card-header h2 {
    font-size: 24px;
    font-weight: bold;
}

.results-count {
    font-size: 14px;
    color: #666;
}

/* Card Body Styling */
.card-body {
    padding: 20px;  /* Add padding to ensure the content doesn't touch the edges */
    background-color: #fff;
    border-radius: 8px;
    box-sizing: border-box;
}

/* Empty Data Styling */
.empty-state {
    text-align: center;
    font-size: 16px;
    color: #666;
}

.empty-state i {
    font-size: 50px;
    margin-bottom: 10px;
}

.empty-state h3 {
    margin: 0;
    font-size: 18px;
}

.empty-state p {
    font-size: 14px;
    color: #999;
}

/* GridView Styling */
.GridView {
    width: 100%;
    border-collapse: collapse;
    padding: 10px;
    margin-top: 20px;
    box-sizing: border-box;
}

.GridView th, .GridView td {
    padding: 12px;  /* Padding inside the table cells for spacing */
    text-align: left;
    border-bottom: 1px solid #ddd;
}

.GridView th {
    background-color: #f0f0f0;
}

.GridView td {
    background-color: #fff;
}

    </style>
</head>
<body>
    <div class="sidebar">
        <h2>Patient</h2>
        <a href="PatientMain.aspx"><i class="fas fa-home"></i> Dashboard</a>
        <a href="BookAppointment.aspx"><i class="fas fa-calendar-plus"></i> Book Appointment</a>
        <a href="CancelAppointment.aspx"><i class="fas fa-calendar-times"></i> Cancel Appointment</a>
        <a href="AptHistory.aspx" class="active"><i class="fas fa-history"></i> Appointment History</a>
        <a href="PatientProfile.aspx">Profile Management <i class="fas fa-user-cog"></i></a>
        <a href="../index.aspx"><i class="fas fa-sign-out-alt"></i> Logout</a>
        
                                <!-- Translate Widget at the Bottom -->
            <div class="translate-container">
                <div id="google_translate_element"></div>
            </div>
    </div>


    <div class="content">
        <form id="form1" runat="server">
            <div class="dashboard-header">
                <h1>Appointment History <i class="fas fa-history"></i></h1>
                <div class="date-label">
                    <i class="fa-solid fa-calendar"></i>
                    Today's Date: <asp:Label ID="lblCurrentDate" runat="server"></asp:Label>
                </div>
            </div>

            <div class="card filter-card">
                <h2>Filter Appointments <i class="fas fa-filter"></i> </h2>
                <div class="form-row">
                    <div class="form-group">
                        <label for="ddlStatusFilter">Status <i class="fas fa-info-circle"></i> </label>
                        <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="form-control">
                            <asp:ListItem Text="All Statuses" Value="All" Selected="True" />
                            <asp:ListItem Text="Scheduled" Value="Scheduled" />
                            <asp:ListItem Text="Completed" Value="Completed" />
                            <asp:ListItem Text="Cancelled" Value="Cancelled" />
                            <asp:ListItem Text="Pending" Value="Pending" />
                            <asp:ListItem Text="Missing" Value="Missing" />
                        </asp:DropDownList>
                    </div>
                    
                    <div class="form-group">
                        <label for="ddlDoctorFilter">Doctor <i class="fas fa-user-md"></i> </label>
                        <asp:DropDownList ID="ddlDoctorFilter" runat="server" CssClass="form-control">
                            <asp:ListItem Text="All Doctors" Value="All" Selected="True" />
                        </asp:DropDownList>
                    </div>
                    
                    <div class="form-group">
                        <label for="txtDate">Date <i class="fas fa-calendar-day"></i> </label>
                        <asp:TextBox ID="txtDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                    </div>


                    <div class="form-group">
                        <asp:Button ID="btnFilter" runat="server" Text="Apply Filters" CssClass="btn-primary" OnClick="FilterAppointments" />
                    </div>

                    <div class="form-group">
                        <asp:Button ID="btnResetFilters" runat="server" Text="Reset Filters" CssClass="btn-primary" OnClick="ClearFilters" />
                    </div>
                </div>
            </div>

<div class="card history-card">
    <!-- Card Header Section -->
    <div class="card-header">
        <h2>Your Appointments <i class="fas fa-list"></i></h2>
        <div class="results-count">
        </div>
    </div>

    <!-- Card Body Section -->
    <div class="card-body">
        <!-- GridView Section inside the card -->
       <asp:GridView ID="gvAppointments" runat="server" AutoGenerateColumns="False" CssClass="appointment-table" 
    DataKeyNames="AppointmentID" DataSourceID="SqlDataSource1" OnRowCommand="gvAppointments_RowCommand" AllowPaging="True">
    
    <Columns>
        <asp:BoundField DataField="DoctorName" HeaderText="Doctor" SortExpression="DoctorName" />
                <asp:BoundField DataField="AppointmentDate" HeaderText="Appointment Date" SortExpression="AppointmentDate" 
            DataFormatString="{0:dd-MMM-yyyy}" />
        <asp:BoundField DataField="StartTime" HeaderText="Start Time" SortExpression="StartTime" />
        <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
        <asp:BoundField DataField="TreatmentType" HeaderText="Treatment Type" SortExpression="TreatmentType" />
    </Columns>

    <EmptyDataTemplate>
        <div class="no-data-message">
            <i class="fas fa-info-circle"></i> No appointments found. Please adjust your filter options and try again.
        </div>
    </EmptyDataTemplate>
</asp:GridView>
    </div>
</div>

                                    <asp:SqlDataSource 
    ID="SqlDataSource1" 
    runat="server" 
    ConnectionString="<%$ ConnectionStrings:MyDbConnection %>" 
    SelectCommand="SELECT a.AppointmentID, a.PatientID, a.DoctorID, d.FullName AS DoctorName, a.AppointmentDate, a.StartTime, a.Status, a.TreatmentType 
                   FROM Appointments a 
                   JOIN Doctors d ON a.DoctorID = d.DoctorID 
                   WHERE a.PatientID = @PatientID">
    <SelectParameters>
        <asp:SessionParameter Name="PatientID" SessionField="PatientID" />
    </SelectParameters>
</asp:SqlDataSource>



                    </div>
                </div>
                
            </div>
        </form>
    </div>
</body>
</html>
