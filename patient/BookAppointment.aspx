<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BookAppointment.aspx.cs" Inherits="patientsystem.BookAppointment" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Book Appointment</title>
    <link rel="stylesheet" href="../css/global.css">
    <link rel="stylesheet" href="../css/patient/patient-book-appointment.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css">
    <script type="text/javascript">
        function googleTranslateElementInit() {
            new google.translate.TranslateElement({ pageLanguage: 'en' }, 'google_translate_element');
        }
    </script>
    <script type="text/javascript" src="//translate.google.com/translate_a/element.js?cb=googleTranslateElementInit"></script>

    <script type="text/javascript">
        function confirmBooking() {
            return confirm("Are you sure you want to book this appointment?");
        }
    </script>


    <style>
        .info-label {
    display: block;
    font-weight: bold;
    color: #31708f;
    background-color: #d9edf7;
    padding: 8px;
    border-radius: 5px;
    margin-bottom: 15px;
}
        /* Overall Styles for Timeslot Selection */
.timeslot-container {
    display: flex;
    flex-wrap: wrap;
    gap: 15px;
    margin-top: 20px;
}

.timeslot-item {
    display: flex;
    align-items: center;
    padding: 10px 20px;
    border-radius: 5px;
    border: 1px solid #ddd;
    transition: background-color 0.3s ease, border 0.3s ease;
    cursor: pointer;
    font-size: 16px;
    background-color: #f7f7f7;
}

.timeslot-item:hover {
    background-color: #d3eaf7;
    border-color: #a0c6f2;
}

.timeslot-item input[type="radio"] {
    margin-right: 10px;
    accent-color: #31708f; /* Blue accent for radio button */
}

.timeslot-item input[type="radio"]:checked {
    background-color: #31708f;
    border-color: #31708f;
}

.timeslot-item span {
    color: #31708f;
}

.timeslot-item input[type="radio"]:focus-visible {
    outline: 3px solid #5cb85c;
    outline-offset: 3px;
}

/* Label styles for the time slot */
.timeslot-label {
    display: inline-block;
    padding: 8px;
    font-weight: 600;
    color: #31708f;
    border-radius: 5px;
    margin-bottom: 15px;
}

/* For time slot section when no slots are available */
.no-slots-message {
    color: #e74c3c;
    font-weight: bold;
    text-align: center;
    font-size: 18px;
    margin-top: 30px;
}

    </style>
</head>
<body>
    <!-- Sidebar -->
    <div class="sidebar">
        <h2>Patient</h2>
        <a href="PatientMain.aspx"><i class="fas fa-home"></i> Dashboard</a>
        <a href="BookAppointment.aspx" class="active"><i class="fas fa-calendar-plus"></i> Book Appointment</a>
        <a href="CancelAppointment.aspx"><i class="fas fa-calendar-times"></i> Cancel Appointment</a>
        <a href="AptHistory.aspx"><i class="fas fa-history"></i> Appointment History</a>
        <a href="PatientProfile.aspx"><i class="fas fa-user-cog"></i> Profile Management</a>
        <a href="../index.aspx"><i class="fas fa-sign-out-alt"></i> Logout</a>

        <div class="translate-container">
            <div id="google_translate_element"></div>
        </div>
    </div>

    <!-- Content -->
    <div class="content">
        <form id="form1" runat="server">
            <div class="dashboard-header">
                <h1>Book Appointment <i class="fas fa-calendar-plus"></i></h1>
                <div class="date-label">
                    <i class="fa-solid fa-calendar"></i> Today's Date: <asp:Label ID="lblCurrentDate" runat="server"></asp:Label>
                </div>
            </div>

            <div class="card booking-form">
                <h2>Find Available Appointment <i class="fas fa-search"></i></h2>

                <!-- Treatment Type -->
                <div class="form-group">
                    <label for="ddlTreatment">Treatment Type <i class="fas fa-stethoscope"></i></label>
                    <asp:DropDownList ID="ddlTreatment" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlTreatment_SelectedIndexChanged"></asp:DropDownList>
                </div>

                <!-- Doctor -->
                <div class="form-group">
                    <label for="ddlDoctors">Select Doctor <i class="fas fa-user-md"></i></label>
                    <asp:DropDownList ID="ddlDoctors" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlDoctors_SelectedIndexChanged"></asp:DropDownList>
                </div>

                <!-- Date -->
                <div class="form-group">
                    <label for="txtDate">Appointment Date <i class="fas fa-calendar-day"></i></label>
                    <asp:TextBox ID="txtDate" runat="server" TextMode="Date" CssClass="form-control" AutoPostBack="true" OnTextChanged="txtDate_TextChanged"></asp:TextBox>
                </div>

                <div class="form-group">
                 <asp:Label ID="lblNotice" runat="server" Text="Note: All appointments are scheduled for 1 hour." CssClass="info-label"></asp:Label>
                </div>


            <!-- Available Time Slots -->
            <div class="form-group">
                <label class="timeslot-label">Available Time Slots <i class="fas fa-clock"></i></label>
                <div id="timeslotContainer" class="timeslot-container">
                    <asp:Repeater ID="rptTimeSlots" runat="server">
                        <ItemTemplate>
                            <div class="timeslot-item">
                                <input type="radio" name="timeSlot" 
                                   id="timeSlot_<%# Eval("TimeSlot") %>" 
                                   value="<%# Eval("TimeSlot") %>" 
                                   onclick="document.getElementById('<%= txtTime.ClientID %>').value = this.value;" />
                                <label for="timeSlot_<%# Eval("TimeSlot") %>"><%# Eval("TimeSlot") %></label>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>



                <!-- Selected Time Hidden -->
                <asp:TextBox ID="txtTime" runat="server" Style="display:none;" />

                <!-- Confirm Button -->
                <div class="form-group">
<asp:Button ID="btnConfirm" runat="server" Text="Confirm Booking" CssClass="btn-primary"
    OnClientClick="return confirmBooking();" OnClick="btnConfirm_Click" />

                </div>

                <!-- Status Message -->
                <asp:Panel ID="pnlMessage" runat="server" CssClass="alert" Visible="false">
                    <asp:Label ID="lblMessage" runat="server"></asp:Label>
                </asp:Panel>
            </div>
        </form>

    </div>
                <script type="text/javascript">
                    function getSelectedTimeSlot() {
                        var timeSlot = document.querySelector('input[name="timeSlot"]:checked');
                        if (timeSlot) {
                            document.getElementById('<%= txtTime.ClientID %>').value = timeSlot.value;
                // Submit the form
                document.getElementById('<%= form1.ClientID %>').submit();
                        } else {
                            alert("Please select a time slot.");
                        }
                    }
                </script>

<script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
<script type="text/javascript">
    function confirmBooking() {
        event.preventDefault();

        Swal.fire({
            title: 'Confirm Your Booking?',
            text: 'You won’t be able to change this appointment later. Proceed?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, confirm it!',
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.isConfirmed) {
                __doPostBack('<%= btnConfirm.UniqueID %>', '');
            }
        });

        return false;
    }
</script>


</body>
</html>
