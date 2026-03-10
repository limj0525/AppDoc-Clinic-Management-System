using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;

namespace Web_Assignment.patient
{
    public partial class PatientProfile : System.Web.UI.Page
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            lblCurrentDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

            if (!IsPostBack)
            {
                if (Session["PatientID"] != null)
                {
                    LoadPatientData();
                }
                else
                {
                    Response.Redirect("~/login.aspx");
                }
            }
        }

        private void LoadPatientData()
        {
            string patientID = Session["PatientID"].ToString();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Patients WHERE PatientID = @PatientID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientID", patientID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    txtFullName.Text = reader["FullName"].ToString();
                    txtDOB.Text = Convert.ToDateTime(reader["DOB"]).ToString("yyyy-MM-dd");
                    ddlGender.SelectedValue = reader["Gender"].ToString();
                    txtEmail.Text = reader["Email"].ToString();
                    txtPhone.Text = reader["Phone"].ToString();
                    txtAddress.Text = reader["Address"].ToString();
                    ddlBloodType.SelectedValue = reader["BloodType"].ToString();
                    txtAllergies.Text = reader["Allergy"].ToString();
                    txtConditions.Text = reader["MedicalCon"].ToString();
                    txtEmergencyName.Text = reader["EmerName"].ToString();
                    txtEmergencyRelation.Text = reader["EmerRship"].ToString();
                    txtEmergencyPhone.Text = reader["EmerPhone"].ToString();
                    ddlSecurityQuestion.SelectedValue = reader["SecurityQuestion"]?.ToString() ?? "";
                    txtSecurityAnswer.Text = reader["SecurityAnswer"]?.ToString() ?? "";

                    string imagePath = reader["Profile_Pic"].ToString();
                    if (!string.IsNullOrEmpty(imagePath) && File.Exists(Server.MapPath(imagePath)))
                    {
                        imgProfile.ImageUrl = imagePath;
                    }
                    else
                    {
                        imgProfile.ImageUrl = "~/Uploads/default-profile.jpg"; // Default fallback
                    }

                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string patientID = Session["PatientID"].ToString();

            // Check if a security question has been selected before allowing an answer
            if (string.IsNullOrEmpty(ddlSecurityQuestion.SelectedValue))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "securityQuestionRequired",
                    "Swal.fire({ icon: 'warning', title: 'Security Question Required', text: 'Please select a security question before answering it.' });", true);
                return; // Stop the save process if no question is selected
            }

            // Validate name, email, phone, and address are not empty
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "nameRequired",
                    "Swal.fire({ icon: 'warning', title: 'Name Required', text: 'Please enter your full name.' });", true);
                return; // Stop the save process if name is empty
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "emailRequired",
                    "Swal.fire({ icon: 'warning', title: 'Email Required', text: 'Please enter your email address.' });", true);
                return; // Stop the save process if email is empty
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "phoneRequired",
                    "Swal.fire({ icon: 'warning', title: 'Phone Number Required', text: 'Please enter your phone number.' });", true);
                return; // Stop the save process if phone number is empty
            }

            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "addressRequired",
                    "Swal.fire({ icon: 'warning', title: 'Address Required', text: 'Please enter your address.' });", true);
                return; // Stop the save process if address is empty
            }

            // Validate phone number (between 10 to 15 characters and numeric)
            string phone = txtPhone.Text.Trim();
            if (phone.Length < 10 || phone.Length > 15 || !System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\d+$"))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "invalidPhone",
                    "Swal.fire({ icon: 'warning', title: 'Invalid Phone Number', text: 'Phone number must be between 10 and 15 digits and contain only numbers.' });", true);
                return; // Stop the save process if phone number is invalid
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            UPDATE Patients SET
                FullName          = @FullName,
                DOB               = @DOB,
                Gender            = @Gender,
                Email             = @Email,
                Phone             = @Phone,
                Address           = @Address,
                BloodType         = @BloodType,
                Allergy           = @Allergies,
                MedicalCon        = @MedicalConditions,
                EmerName          = @EmergencyName,
                EmerRship         = @EmergencyRelation,
                EmerPhone         = @EmergencyPhone,

                -- new security fields
                SecurityQuestion  = @SecurityQuestion,
                SecurityAnswer    = @SecurityAnswer

            WHERE PatientID = @PatientID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FullName", txtFullName.Text.Trim());
                cmd.Parameters.AddWithValue("@DOB", txtDOB.Text);
                cmd.Parameters.AddWithValue("@Gender", ddlGender.SelectedValue);
                cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@Phone", phone);
                cmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());
                cmd.Parameters.AddWithValue("@BloodType", ddlBloodType.SelectedValue);
                cmd.Parameters.AddWithValue("@Allergies", txtAllergies.Text.Trim());
                cmd.Parameters.AddWithValue("@MedicalConditions", txtConditions.Text.Trim());
                cmd.Parameters.AddWithValue("@EmergencyName", txtEmergencyName.Text.Trim());
                cmd.Parameters.AddWithValue("@EmergencyRelation", txtEmergencyRelation.Text.Trim());
                cmd.Parameters.AddWithValue("@EmergencyPhone", txtEmergencyPhone.Text.Trim());

                // **Add these two:**
                cmd.Parameters.AddWithValue("@SecurityQuestion", ddlSecurityQuestion.SelectedValue);
                cmd.Parameters.AddWithValue("@SecurityAnswer", txtSecurityAnswer.Text.Trim());

                cmd.Parameters.AddWithValue("@PatientID", patientID);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
            }

            LoadPatientData(); // refresh the data
            ScriptManager.RegisterStartupScript(
                this, GetType(), "profileUpdated",
                "Swal.fire({ icon: 'success', title: 'Updated!', text: 'Your profile has been updated.' });",
                true
            );
        }




        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (fileUpload.HasFile)
            {
                string filename = Path.GetFileName(fileUpload.FileName);
                string folderPath = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filePath = "~/Uploads/" + filename;
                fileUpload.SaveAs(Path.Combine(folderPath, filename));
                imgProfile.ImageUrl = filePath;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Patients SET Profile_Pic = @Path WHERE PatientID = @PatientID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Path", filePath);
                    cmd.Parameters.AddWithValue("@PatientID", Session["PatientID"].ToString());

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                ScriptManager.RegisterStartupScript(
    this,
    GetType(),
    "picUpdated",
    "Swal.fire({ icon: 'success', title: 'Updated!', text: 'Profile picture updated successfully.' });",
    true
);

            }
        }
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }


        protected void btnUpdatePassword_Click(object sender, EventArgs e)
        {
            string current = txtCurrentPassword.Text.Trim();
            string newPass = txtNewPassword.Text.Trim();
            string confirm = txtConfirmPassword.Text.Trim();
            string patientID = Session["PatientID"].ToString();

            if (newPass != confirm)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "pwdMismatch",
                    "Swal.fire({ icon: 'warning', title: 'Mismatch', text: 'Passwords do not match!' });", true);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string checkQuery = "SELECT Password FROM Patients WHERE PatientID = @PatientID";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@PatientID", patientID);

                conn.Open();
                string storedHashedPassword = checkCmd.ExecuteScalar()?.ToString();

                if (storedHashedPassword == null)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "userNotFound",
                        "Swal.fire({ icon: 'error', title: 'Not Found', text: 'User not found!' });", true);
                    return;
                }

                // Hash the entered current password
                string enteredCurrentHashed = HashPassword(current);

                if (storedHashedPassword != enteredCurrentHashed)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "wrongCurrent",
                        "Swal.fire({ icon: 'error', title: 'Incorrect', text: 'Current password is incorrect!' });", true);
                    return;
                }

                // Hash the new password before saving
                string newHashedPassword = HashPassword(newPass);

                string updateQuery = "UPDATE Patients SET Password = @NewPassword WHERE PatientID = @PatientID";
                SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                updateCmd.Parameters.AddWithValue("@NewPassword", newHashedPassword);
                updateCmd.Parameters.AddWithValue("@PatientID", patientID);
                updateCmd.ExecuteNonQuery();
            }

            ScriptManager.RegisterStartupScript(this, GetType(), "pwdChanged",
                "Swal.fire({ icon: 'success', title: 'Changed!', text: 'Password changed successfully!' });", true);
        }


        protected void btnCancel_Click(object sender, EventArgs e)
        {
            LoadPatientData();
        }
    }
}
