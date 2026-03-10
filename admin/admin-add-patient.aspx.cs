using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography; // add this
using System.Text;
using System.Web.UI;

namespace Assignment
{
    public partial class admin_add_patient : System.Web.UI.Page
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtPatientID.Text = GeneratePatientID();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Basic HTML5 "required" will prevent empty, but extra server check:
            if (string.IsNullOrWhiteSpace(txtFullName.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPhone.Text) ||
                string.IsNullOrEmpty(ddlGender.SelectedValue) ||
                string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "missingFields",
                    "Swal.fire({ icon: 'warning', title: 'Missing Info', text: 'Please fill in all required fields.' });",
                    true
                );
                return;
            }

            // Validate Phone Number (between 10 to 15 digits and numeric)
            string phone = txtPhone.Text.Trim();
            if (phone.Length < 10 || phone.Length > 15 || !System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\d+$"))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "invalidPhone",
                    "Swal.fire({ icon: 'warning', title: 'Invalid Phone Number', text: 'Phone number must be between 10 and 15 digits and contain only numbers.' });", true);
                return;
            }

            // Validate Address (not empty)
            string address = txtAddress.Text.Trim();
            if (string.IsNullOrWhiteSpace(address))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "addressRequired",
                    "Swal.fire({ icon: 'warning', title: 'Address Required', text: 'Please enter the address.' });", true);
                return;
            }

            // Handle Profile Picture Upload
            string profilePicPath;
            string profilePicFileName = txtFullName.Text.Trim().ToLower().Replace(" ", "") + Path.GetExtension(fileProfilePic.FileName).ToLower();

            if (fileProfilePic.HasFile)
            {
                try
                {
                    string extension = Path.GetExtension(fileProfilePic.FileName).ToLower();
                    if (!new[] { ".jpg", ".jpeg", ".png", ".gif" }.Contains(extension))
                    {
                        ScriptManager.RegisterStartupScript(
                            this, GetType(), "invalidExt",
                            "Swal.fire({ icon: 'warning', title: 'Invalid File', text: 'Only JPG, PNG, GIF are allowed.' });",
                            true
                        );
                        return;
                    }

                    string uploadDir = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(uploadDir))
                        Directory.CreateDirectory(uploadDir);

                    string filePath = Path.Combine(uploadDir, profilePicFileName);
                    fileProfilePic.SaveAs(filePath);
                    profilePicPath = "~/Uploads/" + profilePicFileName;
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(
                        this, GetType(), "saveError",
                        $"Swal.fire({{ icon: 'error', title: 'Upload Failed', text: 'Error saving profile picture: {ex.Message}' }});",
                        true
                    );
                    return;
                }
            }
            else
            {
                profilePicPath = "~/Uploads/default.jpeg";
            }

            // Hash the Password
            string hashedPassword = HashPassword(txtPassword.Text.Trim());

            // Save to Database
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Patients 
                         (PatientID, FullName, ICNumber, DOB, Gender, Email, Phone, Address, Password, Profile_Pic) 
                         VALUES 
                         (@PatientID, @FullName, @ICNumber, @DOB, @Gender, @Email, @Phone, @Address, @Password, @Profile_Pic)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@PatientID", txtPatientID.Text);
                    cmd.Parameters.AddWithValue("@FullName", txtFullName.Text.Trim());
                    cmd.Parameters.AddWithValue("@ICNumber", txtICNumber.Text.Trim());
                    cmd.Parameters.AddWithValue("@DOB", DateTime.Parse(txtDOB.Text));
                    cmd.Parameters.AddWithValue("@Gender", ddlGender.SelectedValue);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@Phone", phone);
                    cmd.Parameters.AddWithValue("@Address", address);
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);
                    cmd.Parameters.AddWithValue("@Profile_Pic", profilePicPath);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        ScriptManager.RegisterStartupScript(
                            this,
                            GetType(),
                            "patientAdded",
                            @"Swal.fire({
                        icon: 'success',
                        title: 'Added!',
                        text: 'Patient added successfully!'
                    }).then(() => {
                        window.location = 'admin-manage-patient.aspx';
                    });",
                            true
                        );
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(
                            this,
                            GetType(),
                            "patientError",
                            $"Swal.fire({{ icon: 'error', title: 'Oops...', text: 'Error saving patient: {ex.Message}' }});",
                            true
                        );
                    }
                }
            }
        }


        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("admin-manage-patient.aspx");
        }

        private string GeneratePatientID()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT ISNULL(MAX(CAST(SUBSTRING(PatientID, 2, LEN(PatientID)-1) AS INT)), 0) + 1 FROM Patients";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    int nextId = (int)cmd.ExecuteScalar();
                    return "P" + nextId.ToString("D3");
                }
            }
        }

        // Add the HashPassword method here
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
