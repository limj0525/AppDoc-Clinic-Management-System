using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Security.Cryptography;
using System.Text;

namespace Assignment
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session.Clear();
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string usernameOrEmail = txtUsername.Text.Trim(); // Still using txtUsername textbox
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(usernameOrEmail) || string.IsNullOrEmpty(password))
            {
                ShowErrorMessage("Please enter both username/email and password");
                return;
            }

            ValidationResult result = null;

            if (usernameOrEmail.Equals("admin1", StringComparison.OrdinalIgnoreCase))
            {
                result = ValidateUser("Admin", "AdminID", usernameOrEmail, password);
                if (result.IsValid)
                {
                    Session["UserRole"] = "Admin";
                    Session["Username"] = usernameOrEmail;
                    Response.Redirect("~/admin/admin-dashboard.aspx", true);
                    return;
                }
            }
            else if (usernameOrEmail.StartsWith("D", StringComparison.OrdinalIgnoreCase))
            {
                result = ValidateUser("Doctors", "DoctorID", usernameOrEmail, password);
                if (result.IsValid)
                {
                    Session["UserRole"] = "Doctor";
                    Session["Username"] = usernameOrEmail;
                    Session["DoctorID"] = usernameOrEmail;
                    Response.Redirect("~/doctor/doctor-dashboard.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
            }
            else
            {
                result = ValidateUser("Patients", "Email", usernameOrEmail, password);
                if (result.IsValid)
                {
                    Session["UserRole"] = "Patient";
                    Session["Email"] = usernameOrEmail;

                    // Get actual PatientID using Email
                    string patientID = GetPatientIDByEmail(usernameOrEmail);
                    if (!string.IsNullOrEmpty(patientID))
                    {
                        Session["PatientID"] = patientID;
                    }

                    Response.Redirect("~/patient/patientMain.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
            }

            if (result != null && !string.IsNullOrEmpty(result.ErrorMessage))
            {
                ShowErrorMessage(result.ErrorMessage);
            }
            else
            {
                ShowErrorMessage("Invalid username or password!");
            }
        }

        private ValidationResult ValidateUser(string tableName, string idColumn, string userId, string password)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\DocoSystem.mdf;Integrated Security=True";
            string query = tableName.Equals("Doctors", StringComparison.OrdinalIgnoreCase)
                ? $"SELECT Password, Status FROM {tableName} WHERE {idColumn} = @UserID"
                : $"SELECT Password FROM {tableName} WHERE {idColumn} = @UserID";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    conn.Open();

                    if (tableName.Equals("Doctors", StringComparison.OrdinalIgnoreCase))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedPassword = reader["Password"].ToString();
                                string status = reader["Status"].ToString();

                                if (status.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
                                {
                                    return new ValidationResult { IsValid = false, ErrorMessage = "You're currently deactivated. Contact admin for more info." };
                                }

                                string enteredHashedPassword = HashPassword(password);
                                return new ValidationResult { IsValid = storedPassword == enteredHashedPassword };
                            }
                        }
                    }
                    else
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            string storedPassword = result.ToString();

                            if (tableName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                            {
                                return new ValidationResult { IsValid = storedPassword == password };
                            }
                            else
                            {
                                string enteredHashedPassword = HashPassword(password);
                                return new ValidationResult { IsValid = storedPassword == enteredHashedPassword };
                            }
                        }
                    }

                    return new ValidationResult { IsValid = false };
                }
            }
            catch (Exception ex)
            {
                return new ValidationResult { IsValid = false, ErrorMessage = $"Database error: {ex.Message}" };
            }
        }

        private string GetPatientIDByEmail(string email)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\DocoSystem.mdf;Integrated Security=True";
            string query = "SELECT PatientID FROM Patients WHERE Email = @Email";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                conn.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? result.ToString() : null;
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        private void ShowErrorMessage(string message)
        {
            lblMessage.Text = message;
            lblMessage.ForeColor = System.Drawing.Color.Red;
            lblMessage.Visible = true;

            string script = $@"<script>
                document.querySelector('.login-container').classList.add('animate__animated', 'animate__shakeX');
                setTimeout(function() {{
                    document.querySelector('.login-container').classList.remove('animate__animated', 'animate__shakeX');
                }}, 1000);
            </script>";
            ClientScript.RegisterStartupScript(this.GetType(), "ShakeAnimation", script);
        }

        private class ValidationResult
        {
            public bool IsValid { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}
