<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PatientProfile.aspx.cs" Inherits="Web_Assignment.patient.PatientProfile" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Profile Management</title>
    <link rel="stylesheet" href="../css/global.css">
    <link rel="stylesheet" href="../css/patient/patient-profile.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

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
        <a href="PatientMain.aspx">Dashboard <i class="fas fa-home"></i></a>
        <a href="BookAppointment.aspx">Book Appointment <i class="fas fa-calendar-plus"></i></a>
        <a href="CancelAppointment.aspx">Cancel Appointment <i class="fas fa-calendar-times"></i></a>
        <a href="AptHistory.aspx">Appointment History <i class="fas fa-history"></i></a>
        <a href="PatientProfile.aspx" class="active">Profile Management <i class="fas fa-user-cog"></i></a>
        <a href="../index.aspx"><i class="fas fa-sign-out-alt"></i> Logout</a>
                                    <!-- Translate Widget at the Bottom -->
            <div class="translate-container">
                <div id="google_translate_element"></div>
            </div>
    </div>



    <!-- Main Content -->
    <div class="content clearfix">
        <form id="form1" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server" />
            <div class="dashboard-header">
                <h1>Profile Management <i class="fas fa-user-cog"></i></h1>
                <div class="date-label">
                    <i class="fa-solid fa-calendar"></i>
                    Today's Date: 
                    <asp:Label ID="lblCurrentDate" runat="server"></asp:Label>
                </div>
            </div>

            <!-- Profile Card -->
            <div class="profile-section">
               <!-- Profile Picture Section -->
<div class="profile-picture card">
    <h2>Profile Picture <i class="fas fa-camera"></i></h2>
    <div class="picture-container">
        <asp:Image ID="imgProfile" runat="server" CssClass="profile-thumbnail" ImageUrl="~/Uploads/default-profile.jpg"  style="
    width: 200px;
    height: 200px;
    object-fit: cover;
"/>
        <div class="upload-controls">
            <asp:FileUpload ID="fileUpload" runat="server" CssClass="file-upload" />
            <asp:Button ID="btnUpload" runat="server" Text="Update Picture" CssClass="btn-primary action-btn upload-btn" OnClick="btnUpload_Click" />
        </div>
    </div>
</div>
            <!-- Change Password Section -->
            <div class="password-info card">
             <h2>Change Password <i class="fas fa-key"></i></h2>
            <div class="form-group">
        <label for="txtCurrentPassword"><i class="fas fa-lock"></i> Current Password</label>
        <asp:TextBox ID="txtCurrentPassword" runat="server" TextMode="Password" CssClass="form-control"></asp:TextBox>
    </div>
    <div class="form-group">
        <label for="txtNewPassword"><i class="fas fa-lock"></i> New Password</label>
        <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password" CssClass="form-control"></asp:TextBox>
    </div>
    <div class="form-group">
        <label for="txtConfirmPassword"><i class="fas fa-lock"></i> Confirm New Password</label>
        <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" CssClass="form-control"></asp:TextBox>
    </div>

    <!-- Update Password Button -->
    <div class="form-group">
        <asp:Button ID="btnUpdatePassword" runat="server" Text="Update Password" CssClass="action-btn save-btn" OnClick="btnUpdatePassword_Click" />
    </div>
</div>

                            <div class="password-info card">
             <h2>Security Question and Answer <i class="fas fa-question-circle input-icon"></i></h2>
            <div class="form-group">
        <label for="txtCurrentQuestion"><i class="fas fa-lock"></i> Security Question</label>
              <asp:DropDownList 
        ID="ddlSecurityQuestion" 
        runat="server" 
        CssClass="form-control">
        <asp:ListItem Text="-- Select a Security Question --" Value="" />
        <asp:ListItem Text="What is your favorite color?" 
                       Value="What is your favorite color?" />
        <asp:ListItem Text="What is your favourite food?" 
                       Value="What is your favourite food?" />
        <asp:ListItem Text="What is your nickname?" 
                       Value="What is your nickname?" />
        <asp:ListItem Text="What is your dream job?" 
                       Value="What is your dream job?" />
        <asp:ListItem Text="What city were you born in?" 
                       Value="What city were you born in?" />
      </asp:DropDownList>
    </div>
    <div class="form-group">
        <label for="txtAnswer"><i class="fas fa-lock"></i> Security Answer</label>
      <asp:TextBox 
        ID="txtSecurityAnswer" 
        runat="server" 
        CssClass="form-control" 
        placeholder="Your Answer" />
    </div>
</div>




                <!-- Personal Information Section -->
                <div class="personal-info card">
                    <h2>Personal Information <i class="fas fa-id-card"></i></h2>
                    <div class="form-group">
                        <label for="txtFullName"><i class="fas fa-user"></i> Full Name</label>
                        <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-row">
                        <div class="form-group">
                            <label for="txtDOB"><i class="fas fa-birthday-cake"></i> Date of Birth</label>
                            <asp:TextBox ID="txtDOB" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="ddlGender"><i class="fas fa-venus-mars"></i> Gender</label>
                            <asp:DropDownList ID="ddlGender" runat="server" CssClass="form-control">
                                <asp:ListItem Text="Male" Value="Male"></asp:ListItem>
                                <asp:ListItem Text="Female" Value="Female"></asp:ListItem>
                                <asp:ListItem Text="Other" Value="Other"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>

                


                <!-- Contact Information Section -->
                <div class="contact-info card">
                    <h2>Contact Information <i class="fas fa-address-book"></i></h2>
                    <div class="form-group">
                        <label for="txtEmail"><i class="fas fa-envelope"></i> Email Address</label>
                        <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label for="txtPhone"><i class="fas fa-phone"></i> Phone Number</label>
                        <asp:TextBox ID="txtPhone" runat="server" TextMode="Phone" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label for="txtAddress"><i class="fas fa-map-marker-alt"></i> Address</label>
                        <asp:TextBox ID="txtAddress" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>

                <!-- Medical Information Section -->
                <div class="medical-info card">
                    <h2>Medical Information <i class="fas fa-heartbeat"></i></h2>
                    <div class="form-group">
                        <label for="ddlBloodType"><i class="fas fa-tint"></i> Blood Type</label>
                        <asp:DropDownList ID="ddlBloodType" runat="server" CssClass="form-control">
                            <asp:ListItem Text="Select Blood Type" Value=""></asp:ListItem>
                            <asp:ListItem Text="A+" Value="A+"></asp:ListItem>
                            <asp:ListItem Text="A-" Value="A-"></asp:ListItem>
                            <asp:ListItem Text="B+" Value="B+"></asp:ListItem>
                            <asp:ListItem Text="B-" Value="B-"></asp:ListItem>
                            <asp:ListItem Text="AB+" Value="AB+"></asp:ListItem>
                            <asp:ListItem Text="AB-" Value="AB-"></asp:ListItem>
                            <asp:ListItem Text="O+" Value="O+"></asp:ListItem>
                            <asp:ListItem Text="O-" Value="O-"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="form-group">
                        <label for="txtAllergies"><i class="fas fa-allergies"></i> Allergies</label>
                        <asp:TextBox ID="txtAllergies" runat="server" TextMode="MultiLine" Rows="2" CssClass="form-control" placeholder="List any allergies you have"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label for="txtConditions"><i class="fas fa-file-medical"></i> Medical Conditions</label>
                        <asp:TextBox ID="txtConditions" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" placeholder="List any chronic conditions or past surgeries"></asp:TextBox>
                    </div>
                </div>

                <!-- Emergency Contact Section -->
                <div class="emergency-contact card">
                    <h2>Emergency Contact <i class="fas fa-exclamation-triangle"></i></h2>
                    <div class="form-group">
                        <label for="txtEmergencyName"><i class="fas fa-user"></i> Contact Name</label>
                        <asp:TextBox ID="txtEmergencyName" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label for="txtEmergencyRelation"><i class="fas fa-users"></i> Relationship</label>
                        <asp:TextBox ID="txtEmergencyRelation" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label for="txtEmergencyPhone"><i class="fas fa-phone"></i> Phone Number</label>
                        <asp:TextBox ID="txtEmergencyPhone" runat="server" TextMode="Phone" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>

                <!-- Action Buttons -->
                <div class="action-buttons">
                    <asp:Button ID="btnSave" runat="server" Text="Save Changes" CssClass="btn-primary action-btn save-btn" OnClick="btnSave_Click"/>
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn-primary action-btn cancel-btn" OnClick="btnCancel_Click"/>
                </div>
            </div>
        </form>
    </div>
</body>
</html>
