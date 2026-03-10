using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Assignment
{
    public partial class admin_edit_doctor : System.Web.UI.Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 1) Populate dropdown from DB
                LoadSpecializations();

                // 2) Then load the doctor data
                string doctorID = Request.QueryString["DoctorID"];
                if (!string.IsNullOrEmpty(doctorID))
                {
                    LoadDoctorData(doctorID);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(
                        this,
                        GetType(),
                        "noDocId",
                        @"Swal.fire({
        icon: 'warning',
        title: 'Missing ID',
        text: 'Doctor ID not provided.'
    }).then(() => {
        window.location = 'admin-manage-doctor.aspx';
    });",
                        true
                    );

                }
            }
        }

        private void LoadDoctorData(string doctorID)
        {
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(
                "SELECT * FROM Doctors WHERE DoctorID = @DoctorID", con))
            {
                cmd.Parameters.AddWithValue("@DoctorID", doctorID);
                con.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        txtDoctorID.Text = reader["DoctorID"].ToString();
                        txtFullName.Text = reader["FullName"].ToString();
                        txtICNumber.Text = reader["ICNumber"].ToString();
                        txtDOB.Text = Convert.ToDateTime(reader["DOB"]).ToString("yyyy-MM-dd");
                        ddlGender.SelectedValue = reader["Gender"].ToString();
                        txtEmail.Text = reader["Email"].ToString();
                        txtPhoneNumber.Text = reader["PhoneNumber"].ToString();
                        txtAddress.Text = reader["Address"].ToString();
                        ddlSpecialization.SelectedValue = reader["Specialization"].ToString();
                        ddlStatus.SelectedValue = reader["Status"].ToString();
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(
                            this,
                            GetType(),
                            "docNotFound",
                            @"Swal.fire({
        icon: 'error',
        title: 'Not Found',
        text: 'Doctor not found.'
    }).then(() => {
        window.location = 'admin-manage-doctor.aspx';
    });",
                            true
                        );

                    }
                }
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phoneNumber = txtPhoneNumber.Text.Trim();
            string address = txtAddress.Text.Trim();
            string specialization = ddlSpecialization.SelectedValue;
            string status = ddlStatus.SelectedValue;

            // Validate required fields
            if (string.IsNullOrEmpty(fullName))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "nameRequired",
                    "Swal.fire({ icon: 'warning', title: 'Full Name Required', text: 'Please enter the full name.' });", true);
                return;
            }

            if (string.IsNullOrEmpty(email))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "emailRequired",
                    "Swal.fire({ icon: 'warning', title: 'Email Required', text: 'Please enter the email address.' });", true);
                return;
            }

            if (string.IsNullOrEmpty(phoneNumber))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "phoneRequired",
                    "Swal.fire({ icon: 'warning', title: 'Phone Number Required', text: 'Please enter the phone number.' });", true);
                return;
            }

            if (string.IsNullOrEmpty(address))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "addressRequired",
                    "Swal.fire({ icon: 'warning', title: 'Address Required', text: 'Please enter the address.' });", true);
                return;
            }

            // Validate phone number (between 10 to 15 characters and numeric)
            if (phoneNumber.Length < 10 || phoneNumber.Length > 15 || !System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^\d+$"))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "invalidPhone",
                    "Swal.fire({ icon: 'warning', title: 'Invalid Phone Number', text: 'Phone number must be between 10 and 15 digits and contain only numbers.' });", true);
                return; // Stop the save process if phone number is invalid
            }

            // Ensure specialization is selected
            if (string.IsNullOrEmpty(specialization))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "specializationRequired",
                    "Swal.fire({ icon: 'warning', title: 'Specialization Required', text: 'Please select a specialization.' });", true);
                return;
            }

            // Prevent inactivation if there are upcoming appointments
            if (status == "Inactive")
            {
                const string checkQuery = @"
            SELECT COUNT(*) 
              FROM Appointments 
             WHERE DoctorID = @DoctorID 
               AND CAST(AppointmentDate AS DATE) >= CAST(GETDATE() AS DATE)
               AND Status != 'Cancelled'";

                using (var con = new SqlConnection(connectionString))
                using (var checkCmd = new SqlCommand(checkQuery, con))
                {
                    checkCmd.Parameters.AddWithValue("@DoctorID", txtDoctorID.Text);
                    con.Open();
                    int count = (int)checkCmd.ExecuteScalar();
                    if (count > 0)
                    {
                        ScriptManager.RegisterStartupScript(
                            this,
                            GetType(),
                            "cannotDeactivate",
                            $@"Swal.fire({{
                        icon: 'error',
                        title: 'Cannot Deactivate',
                        text: 'Doctor has {count} upcoming appointment(s).'
                    }});",
                            true
                        );
                        return;
                    }
                }
            }

            // Update record if all validations pass
            const string updateQuery = @"
        UPDATE Doctors SET 
            FullName       = @FullName,
            ICNumber       = @ICNumber,
            DOB            = @DOB,
            Gender         = @Gender,
            Email          = @Email,
            PhoneNumber    = @PhoneNumber,
            Address        = @Address,
            Specialization = @Specialization,
            Status         = @Status
          WHERE DoctorID     = @DoctorID";

            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(updateQuery, con))
            {
                cmd.Parameters.AddWithValue("@DoctorID", txtDoctorID.Text);
                cmd.Parameters.AddWithValue("@FullName", fullName);
                cmd.Parameters.AddWithValue("@ICNumber", txtICNumber.Text);
                cmd.Parameters.AddWithValue(
                    "@DOB",
                    string.IsNullOrWhiteSpace(txtDOB.Text)
                        ? (object)DBNull.Value
                        : txtDOB.Text);
                cmd.Parameters.AddWithValue("@Gender", ddlGender.SelectedValue);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                cmd.Parameters.AddWithValue("@Address", address);
                cmd.Parameters.AddWithValue("@Specialization", specialization);
                cmd.Parameters.AddWithValue("@Status", status);

                con.Open();
                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    // Success alert + redirect
                    ScriptManager.RegisterStartupScript(
                        this,
                        GetType(),
                        "docUpdateSuccess",
                        @"Swal.fire({
                    icon: 'success',
                    title: 'Updated!',
                    text: 'Doctor updated successfully!'
                }).then(() => {
                    window.location = 'admin-manage-doctor.aspx';
                });",
                        true
                    );
                }
                else
                {
                    // Failure alert
                    ScriptManager.RegisterStartupScript(
                        this,
                        GetType(),
                        "docUpdateFail",
                        @"Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Update failed.'
                });",
                        true
                    );
                }

            }
        }


        private void LoadSpecializations()
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT DISTINCT Specialization FROM Doctors";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                ddlSpecialization.Items.Clear();
                ddlSpecialization.Items.Add(new ListItem("Select Specialization", ""));

                while (reader.Read())
                {
                    ddlSpecialization.Items.Add(new ListItem(reader["Specialization"].ToString(), reader["Specialization"].ToString()));
                }
                reader.Close();
            }
        }


        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("admin-manage-doctor.aspx");
        }
    }
}