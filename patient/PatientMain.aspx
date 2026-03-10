<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PatientMain.aspx.cs" Inherits="patientsystem.PatientMain" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Patient Dashboard</title>
    <link rel="stylesheet" href="../css/global.css">
    <link rel="stylesheet" href="../css/patient/patient-dashboard.css">
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

</head>
<body>
    <!-- Sidebar Navigation -->
    <div class="sidebar">
        <h2>Patient</h2>
        <a href="PatientMain.aspx" class="active">Dashboard <i class="fas fa-home"></i></a>
        <a href="BookAppointment.aspx">Book Appointment <i class="fas fa-calendar-plus"></i></a>
        <a href="CancelAppointment.aspx">Cancel Appointment <i class="fas fa-calendar-times"></i></a>
        <a href="AptHistory.aspx">Appointment History <i class="fas fa-history"></i></a>
        <a href="PatientProfile.aspx">Profile Management <i class="fas fa-user-cog"></i></a>
        <a href="../index.aspx"><i class="fas fa-sign-out-alt"></i> Logout</a>
        
                            <!-- Translate Widget at the Bottom -->
            <div class="translate-container">
                <div id="google_translate_element"></div>
            </div>
    </div>


    <!-- Main Content -->
    <div class="content clearfix">
        <form id="form1" runat="server">
            <div class="dashboard-header">
                <h1>Welcome, <asp:Label ID="patientName" runat="server" Text="Patient"></asp:Label></h1>
                <div class="date-label">
                    <i class="fa-solid fa-calendar"></i>
                    Today's Date: <asp:Label ID="lblCurrentDate" runat="server"></asp:Label>
                </div>
            </div>

<!-- Appointment Reminders Section -->
<div class="appointment-reminder card" id="appointmentCard" runat="server" visible="true">
    <h2>Next Appointment <i class="fas fa-bell"></i></h2>
    <p><strong>Doctor:</strong> <asp:Label ID="nextDoctorName" runat="server" Text="Not scheduled"></asp:Label></p>
    <p><strong>Date:</strong> <asp:Label ID="nextAptDate" runat="server" Text="Not scheduled"></asp:Label></p>
    <p><strong>Time:</strong> <asp:Label ID="nextAptTime" runat="server" Text="Not scheduled"></asp:Label></p>
    <div class="countdown fade-in" id="countdown" runat="server">
        <i class="fas fa-hourglass-half"></i> No upcoming appointments
    </div>
</div>


            <!-- Patient Details Section -->
            <div class="patient-details card">
                <asp:Image ID="Image1" runat="server" CssClass="patient-image" ImageUrl="../image/default-profile.png" AlternateText="Profile Picture" />
                <h2>Patient Details <i class="fas fa-user"></i></h2>
                <p><strong><i class="fas fa-phone"></i> Phone:</strong> <asp:Label ID="phoneNumber" runat="server" Text="Not provided"></asp:Label></p>
                <p><strong><i class="fas fa-birthday-cake"></i> Date Of Birth:</strong> <asp:Label ID="DOB" runat="server" Text="Not provided"></asp:Label></p>
                <p><strong><i class="fas fa-venus-mars"></i> Gender:</strong> <asp:Label ID="gender" runat="server" Text="Not provided"></asp:Label></p>
                <p><strong><i class="fas fa-envelope"></i> Email:</strong> <asp:Label ID="email" runat="server" Text="Not provided"></asp:Label></p>
            </div>

<!-- Upcoming Appointments Section -->
<div class="appointments card">
    <h2>Upcoming Appointments <i class="fas fa-calendar-alt"></i></h2>

    <asp:Repeater ID="rptAppointments" runat="server" DataSourceID="SqlDataSource1">
        <ItemTemplate>
            <div class="appointment-card">
                <p><strong><i class="fas fa-user-md"></i> Doctor:</strong> <%# Eval("DoctorName") %></p>
                <p><strong><i class="fas fa-calendar-day"></i> Date:</strong> <%# Eval("AppointmentDate", "{0:dd-MM-yyyy}") %></p>
                <p><strong><i class="fas fa-clock"></i> Time:</strong> <%# Eval("StartTime", "{0:hh\\:mm}") %></p>
            </div>
        </ItemTemplate>
        <FooterTemplate>
            <asp:Label ID="lblEmpty" runat="server" Visible='<%# rptAppointments.Items.Count == 0 %>' Text="No upcoming appointments"></asp:Label>
        </FooterTemplate>
    </asp:Repeater>

    <asp:SqlDataSource 
        ID="SqlDataSource1" 
        runat="server" 
        ConnectionString="<%$ ConnectionStrings:MyDbConnection %>" 
        SelectCommand="
            SELECT TOP 5 
                a.AppointmentID, 
                a.PatientID, 
                a.DoctorID, 
                d.FullName AS DoctorName, 
                a.AppointmentDate, 
                a.StartTime, 
                a.Status, 
                a.TreatmentType
            FROM Appointments a
            JOIN Doctors d ON a.DoctorID = d.DoctorID
            WHERE a.PatientID = @PatientID 
              AND a.Status = 'Scheduled' 
              AND a.AppointmentDate >= CONVERT(date, GETDATE())
            ORDER BY a.AppointmentDate ASC, a.StartTime ASC">
        <SelectParameters>
            <asp:SessionParameter Name="PatientID" SessionField="PatientID" />
        </SelectParameters>
    </asp:SqlDataSource>
</div>


            <!-- Healthy Tips Section -->
            <div class="healthy-tips card">
                <h2>Healthy Tips <i class="fas fa-heartbeat"></i></h2>
                <ul>
                    <li><i class="fas fa-tint"></i> Stay hydrated by drinking at least 8 glasses of water daily.</li>
                    <li><i class="fas fa-apple-alt"></i> Eat a balanced diet rich in fruits, vegetables, and whole grains.</li>
                    <li><i class="fas fa-running"></i> Exercise for at least 30 minutes a day to keep your body active.</li>
                    <li><i class="fas fa-bed"></i> Get 7-8 hours of sleep every night for optimal health.</li>
                    <li><i class="fas fa-spa"></i> Practice mindfulness or meditation to reduce stress.</li>
                </ul>
            </div>
        </form>
    </div>

    <!-- Countdown Timer Script -->
    <script>
        // This will be updated by server-side code if there's an appointment
        const appointmentElement = document.getElementById("countdown");
        const hasAppointment = appointmentElement.getAttribute("data-has-appointment") === "true";

        if (hasAppointment) {
            // Set the date and time of the next appointment from server-side values
            const appointmentDateStr = document.getElementById("nextAptDate").innerText + " " +
                document.getElementById("nextAptTime").innerText;
            const appointmentDate = new Date(appointmentDateStr).getTime();

            // Update the countdown every second
            const countdown = setInterval(() => {
                const now = new Date().getTime();
                const timeRemaining = appointmentDate - now;

                // Calculate days, hours, minutes, and seconds
                const days = Math.floor(timeRemaining / (1000 * 60 * 60 * 24));
                const hours = Math.floor((timeRemaining % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
                const minutes = Math.floor((timeRemaining % (1000 * 60 * 60)) / (1000 * 60));
                const seconds = Math.floor((timeRemaining % (1000 * 60)) / 1000);

                // Display the countdown
                document.getElementById("countdown").innerHTML =
                    `<i class="fas fa-hourglass-half"></i> Time Remaining: ${days}d ${hours}h ${minutes}m ${seconds}s`;

                // If the countdown is over, display a message
                if (timeRemaining < 0) {
                    clearInterval(countdown);
                    document.getElementById("countdown").innerHTML = "<i class='fas fa-bell'></i> Appointment time has arrived!";
                }
            }, 1000);
        }
    </script>
</body>
</html>