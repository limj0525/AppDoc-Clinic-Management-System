<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="admin-edit-appointment.aspx.cs" Inherits="Assignment.admin_edit_appointment" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Edit Appointment</title>
    <link rel="stylesheet" href="~/css/global.css" />
    <link rel="stylesheet" href="../css/admin/admin-edit-appointment.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

    <style>
    .timeslot-item {
        display: inline-block;
        padding: 10px 20px;
        border: 1px solid #ccc;
        margin: 5px;
        border-radius: 5px;
        cursor: pointer;
        transition: background-color 0.3s;
    }

    .timeslot-item.selected {
        background-color: #5cb85c; /* green */
        color: white;
        border-color: #4cae4c;
    }
</style>

    <script>
        window.onload = function () {
            var txtTime = document.getElementById('<%= txtTime.ClientID %>');
            var selectedTime = txtTime ? txtTime.value.trim() : "";

            var allSlots = document.querySelectorAll('.timeslot-item');

            allSlots.forEach(function (div) {
                var rb = div.querySelector('input[type="radio"]');
                var label = rb ? rb.nextSibling.textContent.trim() : "";

                if (label === selectedTime) {
                    rb.checked = true;
                    div.classList.add('selected');
                }
            });
        };
    </script>


    <script>
        function selectTimeSlot(clickedDiv) {
            // Remove 'selected' class from all time slots
            var allSlots = document.querySelectorAll('.timeslot-item');
            allSlots.forEach(function (div) {
                div.classList.remove('selected');
            });

            // Add 'selected' class to clicked slot
            clickedDiv.classList.add('selected');

            // Find the RadioButton inside and check it
            var radio = clickedDiv.querySelector('input[type="radio"]');
            if (radio) {
                radio.checked = true;
            }
        }
    </script>

<script>
    function selectTimeSlot(clickedDiv) {
        // Remove 'selected' class from all slots
        var allSlots = document.querySelectorAll('.timeslot-item');
        allSlots.forEach(function (div) {
            div.classList.remove('selected');
            var rb = div.querySelector('input[type="radio"]');
            if (rb) rb.checked = false; // uncheck all radios
        });

        // Add selected class to clicked and check it
        clickedDiv.classList.add('selected');
        var selectedRadio = clickedDiv.querySelector('input[type="radio"]');
        if (selectedRadio) {
            selectedRadio.checked = true;

            // Also update hidden textbox if needed
            var txtTime = document.getElementById('<%= txtTime.ClientID %>');
            if (txtTime) {
                txtTime.value = selectedRadio.nextSibling.textContent.trim();
            }
        }
    }
</script>

<script type="text/javascript">
  function googleTranslateElementInit() {
    new google.translate.TranslateElement(
      {pageLanguage: 'en'},
      'google_translate_element'
    );
  }
        </script>
<script type="text/javascript" src="//translate.google.com/translate_a/element.js?cb=googleTranslateElementInit"></script>
</head>
<body>
        <div class="sidebar">
        <h2>Admin Panel</h2>
        <a href="admin-dashboard.aspx"><i class="fas fa-tachometer-alt"></i> Dashboard</a>
        <a href="admin-manage-doctor.aspx"><i class="fas fa-user-md"></i> Manage Doctors</a>
        <a href="admin-manage-patient.aspx"><i class="fas fa-hospital-user"></i> Manage Patients</a>
        <a href="admin-manage-appointments.aspx"class="active"><i class="fas fa-calendar-check"></i> Manage Appointments</a>
        <a href="admin-reports.aspx"><i class="fas fa-chart-bar"></i> Reports</a>
        <a href="../index.aspx"><i class="fas fa-sign-out-alt"></i> Logout</a>
        <!-- Translate Widget at the Bottom -->
        <div class="translate-container">
            <div id="google_translate_element"></div>
        </div>
    </div>



    <div class="content">
        <form id="form1" runat="server">
            <div class="header">
                <h2><i class="fas fa-user-edit"></i> Update Appointment Details</h2>
            </div>

            <div class="form-group">
                <label for="txtAppointmentID">Appointment ID <i class="fas fa-hashtag"></i></label>
                <asp:TextBox ID="txtAppointmentID" runat="server" CssClass="form-control" ReadOnly="true" />
            </div>

            <div class="form-group">
                <label for="txtPatientID">Patient ID <i class="fas fa-id-card"></i></label>
                <asp:TextBox ID="txtPatientID" runat="server" CssClass="form-control" ReadOnly="true" />
            </div>

            <div class="form-group">
                <label for="ddlTreatment">Treatment Type <i class="fas fa-stethoscope"></i></label>
                <asp:DropDownList ID="ddlTreatment" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlTreatment_SelectedIndexChanged" />
            </div>

<div class="form-group">
    <label for="ddlDoctors">Assigned Doctor <i class="fas fa-user-md"></i></label>
    <asp:DropDownList ID="ddlDoctors" runat="server" CssClass="form-control" 
                      AutoPostBack="true" OnSelectedIndexChanged="ddlDoctors_SelectedIndexChanged" />
</div>

  <div class="form-row">
    <div class="form-group half-width">
        <label for="txtDate">Appointment Date <i class="fas fa-calendar-day"></i></label>
        <asp:TextBox ID="txtDate" runat="server" TextMode="Date" CssClass="form-control" 
                     AutoPostBack="true" OnTextChanged="txtDate_TextChanged" />
    </div>
    <div class="form-group half-width">
        <label for="txtTime">Appointment Time <i class="fas fa-clock"></i></label>
        <!-- Selected Time Hidden -->
   <!-- Selected Time Visible -->
<asp:TextBox ID="txtTime" runat="server" CssClass="form-control" ReadOnly="true"/>

    </div>
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
        <div class="timeslot-item" onclick="selectTimeSlot(this)">
            <asp:RadioButton ID="rbTimeSlot" runat="server" 
                GroupName="TimeSlotGroup"
                Text='<%# Eval("TimeSlot") %>' 
                CssClass="timeslot-radio" />
        </div>
    </ItemTemplate>
</asp:Repeater>



    </div>
</div>





            <div class="form-group">
                <asp:Button ID="btnUpdate" runat="server" Text="Update Appointment" CssClass="submit-btn" OnClick="btnUpdate_Click" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="cancel-btn" OnClick="btnCancel_Click" CausesValidation="false" />
            </div>

            <asp:Panel ID="pnlMessage" runat="server" CssClass="alert" Visible="false">
                <asp:Label ID="lblMessage" runat="server" />
            </asp:Panel>
        </form>
    </div>
</body>
</html>
