<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="doctor-dashboard.aspx.cs" Inherits="Web_Assignment.doctor_dashboard" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Doctor Dashboard</title>
    <link rel="stylesheet" href="../css/global.css" />
    <link rel="stylesheet" href="../css/doctor/doctor-dashboard.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css" />
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script type="text/javascript">
        function googleTranslateElementInit() {
            new google.translate.TranslateElement({ pageLanguage: 'en' }, 'google_translate_element');
        }
    </script>
    <script type="text/javascript" src="//translate.google.com/translate_a/element.js?cb=googleTranslateElementInit"></script>

    <style>
        .section {
    background-color: #ffffff;
    padding: 20px;
    margin-bottom: 25px;
    border-radius: 10px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
}

.section h2 {
    margin-bottom: 15px;
    color: #89A8B2;
    border-bottom: 1px solid #eee;
    padding-bottom: 5px;
    font-size: 1.3rem;
}

.appointment-grid {
    width: 100%;
    border-collapse: collapse;
    margin-top: 10px;
}

.appointment-grid th, .appointment-grid td {
    padding: 10px;
    border-bottom: 1px solid #e0e0e0;
    text-align: left;
}

.appointment-grid th {
    background-color: #f7f7f7;
    color: #333;
}
.pending-card {
    background-color: #f9f9f9;
    border-left: 4px solid #3498db;
    padding: 15px;
    margin-bottom: 15px;
    border-radius: 8px;
    position: relative;
}

.action-button-group {
    display: flex;
    justify-content: flex-end;
    gap: 10px;
    margin-top: 12px;
}

.btn-small {
    min-width: 110px;
    padding: 6px 12px;
    font-size: 0.85rem;
    border-radius: 5px;
    border: none;
    cursor: pointer;
    transition: 0.2s ease-in-out;
}

.success-btn {
    background-color: #2ecc71;
    color: white;
}

.success-btn:hover {
    background-color: #27ae60;
}

.danger-btn {
    background-color: #e74c3c;
    color: white;
}

.danger-btn:hover {
    background-color: #c0392b;
}



    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="sidebar">
            <h2>Doctor Panel</h2>
            <a href="doctor-dashboard.aspx" class="active">Dashboard <i class="fa-solid fa-house"></i></a>
            <a href="doctor-view-appointment.aspx">View Appointment <i class="fa-solid fa-calendar-check"></i></a>
            <a href="doctor-profile-management.aspx">Profile Management <i class="fa-solid fa-user"></i></a>
            <a href="../index.aspx">Logout <i class="fas fa-sign-out-alt"></i></a>
            <div class="translate-container"><div id="google_translate_element"></div></div>
        </div>

        <div class="dashboard-container">
            <div class="dashboard-header">
                <h1>Dashboard</h1>
                <div class="date-label">
                    <i class="fa-solid fa-calendar"></i>
                    Today's Date: <asp:Label ID="lblCurrentDate" runat="server"></asp:Label>
                </div>
            </div>

            <div class="summary-cards">
                <div class="card"><h3>Total Appointments</h3><p><asp:Label ID="lblTotalAppointments" runat="server"></asp:Label></p></div>
                <div class="card"><h3>Pending Approvals</h3><p><asp:Label ID="lblPendingApprovals" runat="server"></asp:Label></p></div>
                <div class="card"><h3>Total Patients</h3><p><asp:Label ID="lblTotalPatients" runat="server"></asp:Label></p></div>
            </div>

<div class="section">
    <h2><i class="fas fa-hourglass-half"></i> Pending Approvals</h2>

    <asp:Repeater ID="rptPendingAppointments" runat="server" OnItemCommand="rptPendingAppointments_ItemCommand">
        <ItemTemplate>
            <div class="pending-card">
                <p><strong>Patient:</strong> <%# Eval("PatientName") %></p>
                <p><strong>Date:</strong> <%# Eval("AppointmentDate", "{0:dd-MMM-yyyy}") %></p>
                <p><strong>Appointment Time:</strong> <%# Eval("StartTime", "{0:hh\\:mm}") %></p>

                <div class="action-button-group">
                    <asp:Button ID="btnMarkCompleted" runat="server" Text="✔ Complete"
                        CommandName="Completed" CommandArgument='<%# Eval("AppointmentID") %>' CssClass="btn-small success-btn" />
                    <asp:Button ID="btnMarkMissed" runat="server" Text="✘ Missed"
                        CommandName="Missed" CommandArgument='<%# Eval("AppointmentID") %>' CssClass="btn-small danger-btn" />
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>

    <asp:Label ID="lblNoPending" runat="server" CssClass="no-data-message" Text="You have no pending approvals at the moment." Visible="false" />
</div>



            <div class="section">
                <h2><i class="fa-solid fa-calendar-days"></i> Today's Appointments</h2>
<asp:GridView ID="gvTodayAppointments" runat="server" CssClass="appointment-grid"
    AutoGenerateColumns="False">
    <Columns>
        <asp:BoundField DataField="PatientName" HeaderText="Patient" />
        <asp:BoundField DataField="AppointmentDate" HeaderText="Date" DataFormatString="{0:dd-MMM-yyyy}" />
        <asp:BoundField DataField="StartTime" HeaderText="Start Time" />
        <asp:BoundField DataField="Status" HeaderText="Status" />
        <asp:BoundField DataField="TreatmentType" HeaderText="Treatment" />
    </Columns>
    <EmptyDataTemplate>
        <div class="no-data-message">
            <i class="fas fa-info-circle"></i> You have no appointments for today.
        </div>
    </EmptyDataTemplate>
</asp:GridView>


            </div>

            <div class="section">
                <h2><i class="fa-solid fa-clock"></i> My Next 7 Days Availability</h2>
<asp:Repeater ID="rptSchedule" runat="server">
    <ItemTemplate>
        <p><strong><%# Eval("Date") %></strong> — <%# Eval("TimeSlots") %></p>
    </ItemTemplate>
</asp:Repeater>

            </div>

            <div class="section">
                <h2><i class="fa-solid fa-calendar-check"></i> My Upcoming Appointments</h2>
<asp:GridView ID="gvUpcomingAppointments" runat="server" CssClass="appointment-grid"
    AutoGenerateColumns="False" DataKeyNames="AppointmentID">
    <Columns>
        <asp:BoundField DataField="PatientName" HeaderText="Patient" />
        <asp:BoundField DataField="AppointmentDate" HeaderText="Date" DataFormatString="{0:dd-MMM-yyyy}" />
        <asp:BoundField DataField="StartTime" HeaderText="Start Time" />
        <asp:BoundField DataField="Status" HeaderText="Status" />
        <asp:BoundField DataField="TreatmentType" HeaderText="Treatment" />
    </Columns>
    <EmptyDataTemplate>
        <div class="no-data-message">
            <i class="fas fa-info-circle"></i> No upcoming appointments found.
        </div>
    </EmptyDataTemplate>
</asp:GridView>

            </div>
        </div>
    </form>
</body>
</html>