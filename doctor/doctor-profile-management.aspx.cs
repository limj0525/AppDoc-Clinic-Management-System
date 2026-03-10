using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;

namespace Web_Assignment
{
    public partial class doctor_profile_management : System.Web.UI.Page
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            lblCurrentDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

            if (!IsPostBack)
            {
                if (Session["DoctorID"] != null)
                {
                    LoadDoctorData();
                }
                else
                {
                    Response.Redirect("~/login.aspx");
                }
            }
        }

        private void LoadDoctorData()
        {
            string doctorID = Session["DoctorID"].ToString();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT FullName, Email, PhoneNumber, Specialization FROM Doctors WHERE DoctorID = @DoctorID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DoctorID", doctorID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    lblFullName.Text = reader["FullName"].ToString();
                    lblEmail.Text = reader["Email"].ToString();
                    lblPhoneNumber.Text = reader["PhoneNumber"].ToString();
                    lblSpecialization.Text = reader["Specialization"].ToString();
                }
            }
        }

        protected void btnSaveProfile_Click(object sender, EventArgs e)
        {
            string doctorID = Session["DoctorID"].ToString();

            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phoneNumber = txtPhoneNumber.Text.Trim();
            string specialization = txtSpecialization.Text.Trim();
            string password = txtPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            // Validate phone number (between 10 to 15 characters and numeric)
            if (phoneNumber.Length < 10 || phoneNumber.Length > 15 || !System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^\d+$"))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "invalidPhone",
                    "Swal.fire({ icon: 'warning', title: 'Invalid Phone Number', text: 'Phone number must be between 10 and 15 digits and contain only numbers.' });", true);
                return; // Stop the save process if phone number is invalid
            }

            // Validate password match
            if (!string.IsNullOrEmpty(password) && password != confirmPassword)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "passwordMismatch",
                    "Swal.fire({ icon: 'warning', title: 'Password Mismatch', text: 'Passwords do not match!' });", true);
                return; // Stop the save process if passwords don't match
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Step 1: Load existing values
                string loadQuery = "SELECT FullName, Email, PhoneNumber, Specialization FROM Doctors WHERE DoctorID = @DoctorID";
                SqlCommand loadCmd = new SqlCommand(loadQuery, conn);
                loadCmd.Parameters.AddWithValue("@DoctorID", doctorID);

                SqlDataReader reader = loadCmd.ExecuteReader();
                string existingFullName = "", existingEmail = "", existingPhoneNumber = "", existingSpecialization = "";

                if (reader.Read())
                {
                    existingFullName = reader["FullName"].ToString();
                    existingEmail = reader["Email"].ToString();
                    existingPhoneNumber = reader["PhoneNumber"].ToString();
                    existingSpecialization = reader["Specialization"].ToString();
                }
                reader.Close();

                // Step 2: Prepare values (use new value if provided, else keep old one)
                string updatedFullName = string.IsNullOrEmpty(fullName) ? existingFullName : fullName;
                string updatedEmail = string.IsNullOrEmpty(email) ? existingEmail : email;
                string updatedPhoneNumber = string.IsNullOrEmpty(phoneNumber) ? existingPhoneNumber : phoneNumber;
                string updatedSpecialization = string.IsNullOrEmpty(specialization) ? existingSpecialization : specialization;

                // Step 3: Update everything
                string updateQuery = "UPDATE Doctors SET FullName = @FullName, Email = @Email, PhoneNumber = @PhoneNumber, Specialization = @Specialization";

                if (!string.IsNullOrEmpty(password))
                {
                    updateQuery += ", Password = @Password";
                }

                updateQuery += " WHERE DoctorID = @DoctorID";

                SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                updateCmd.Parameters.AddWithValue("@FullName", updatedFullName);
                updateCmd.Parameters.AddWithValue("@Email", updatedEmail);
                updateCmd.Parameters.AddWithValue("@PhoneNumber", updatedPhoneNumber);
                updateCmd.Parameters.AddWithValue("@Specialization", updatedSpecialization);

                if (!string.IsNullOrEmpty(password))
                {
                    string hashedPassword = HashPassword(password);
                    updateCmd.Parameters.AddWithValue("@Password", hashedPassword);
                }

                updateCmd.Parameters.AddWithValue("@DoctorID", doctorID);

                int rowsAffected = updateCmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    ScriptManager.RegisterStartupScript(
                        this,
                        GetType(),
                        "profileSuccess",
                        "Swal.fire({ icon: 'success', title: 'Updated!', text: 'Profile updated successfully!' });",
                        true
                    );
                }
                else
                {
                    ScriptManager.RegisterStartupScript(
                        this,
                        GetType(),
                        "profileNoChange",
                        "Swal.fire({ icon: 'info', title: 'No Changes', text: 'No changes were made.' });",
                        true
                    );
                }

            }

            LoadDoctorData(); // Refresh displayed information
        }




        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
