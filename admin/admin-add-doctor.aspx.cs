using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography; // for hashing
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Assignment
{
    public partial class admin_add_doctor : System.Web.UI.Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 1) Populate specialization dropdown
                LoadSpecializations();

                // 2) Generate a new doctor ID
                txtDoctorID.Text = GenerateDoctorID();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Basic HTML5 "required" will prevent empty, but extra server check:
            if (string.IsNullOrWhiteSpace(txtFullName.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPhoneNumber.Text) ||
                string.IsNullOrEmpty(ddlSpecialization.SelectedValue))
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
            string phoneNumber = txtPhoneNumber.Text.Trim();
            if (phoneNumber.Length < 10 || phoneNumber.Length > 15 || !System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^\d+$"))
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

            string doctorID = txtDoctorID.Text;
            string hashedPassword = HashPassword(txtPassword.Text.Trim());

            using (var con = new SqlConnection(connectionString))
            {
                con.Open();

                const string query = @"
            INSERT INTO Doctors 
                (DoctorID, FullName, ICNumber, DOB, Gender, Email, PhoneNumber, Address, Password, Specialization, Status)
            VALUES 
                (@DoctorID, @FullName, @ICNumber, @DOB, @Gender, @Email, @PhoneNumber, @Address, @Password, @Specialization, @Status)";

                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@DoctorID", doctorID);
                    cmd.Parameters.AddWithValue("@FullName", txtFullName.Text.Trim());
                    cmd.Parameters.AddWithValue("@ICNumber", txtICNumber.Text.Trim());
                    cmd.Parameters.AddWithValue("@DOB", string.IsNullOrWhiteSpace(txtDOB.Text) ? (object)DBNull.Value : txtDOB.Text);
                    cmd.Parameters.AddWithValue("@Gender", ddlGender.SelectedValue);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                    cmd.Parameters.AddWithValue("@Address", address);
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);
                    cmd.Parameters.AddWithValue("@Specialization", ddlSpecialization.SelectedValue);
                    cmd.Parameters.AddWithValue("@Status", "Active");

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        ScriptManager.RegisterStartupScript(
                            this,
                            GetType(),
                            "doctorAddSuccess",
                            @"Swal.fire({
                        icon: 'success',
                        title: 'Added!',
                        text: 'Doctor added successfully!'
                    }).then(() => {
                        window.location = 'admin-manage-doctor.aspx';
                    });",
                            true
                        );
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(
                            this,
                            GetType(),
                            "doctorAddFail",
                            @"Swal.fire({
                        icon: 'error',
                        title: 'Oops...',
                        text: 'Failed to add doctor.'
                    });",
                            true
                        );
                    }
                }
            }
        }


        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("admin-manage-doctor.aspx");
        }

        private string GenerateDoctorID()
        {
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT MAX(DoctorID) FROM Doctors", con))
            {
                con.Open();
                var result = cmd.ExecuteScalar() as string;
                if (string.IsNullOrEmpty(result))
                {
                    // no doctors yet
                    return "D001";
                }
                int lastNum;
                if (int.TryParse(result.Substring(1), out lastNum))
                {
                    return "D" + (lastNum + 1).ToString("D3");
                }
                else
                {
                    return "D001";
                }
            }
        }


        private void LoadSpecializations()
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT DISTINCT Specialization FROM Doctors ORDER BY Specialization", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    ddlSpecialization.Items.Clear();
                    ddlSpecialization.Items.Add(new ListItem("-- Select Specialization --", ""));

                    bool hasData = false;
                    while (reader.Read())
                    {
                        hasData = true;
                        string spec = reader.GetString(0);
                        ddlSpecialization.Items.Add(new ListItem(spec, spec));
                    }

                    // If no specializations found, add default ones
                    if (!hasData)
                    {
                        string[] defaultSpecializations = { "General", "Cardiology", "Dermatology", "Psychiatry" };
                        foreach (var spec in defaultSpecializations)
                        {
                            ddlSpecialization.Items.Add(new ListItem(spec, spec));
                        }
                    }
                }
            }
        }


        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}