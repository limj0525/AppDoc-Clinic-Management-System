using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace patientsystem
{
    public partial class CancelAppointment : System.Web.UI.Page
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblCurrentDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                // Populate the appointment dropdown (fetching appointments from the database)
                UpdatePastAppointmentsStatus();
                PopulateAppointments();
            }
        }

        private void UpdatePastAppointmentsStatus()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString))
            {
                string query = @"
            UPDATE Appointments
            SET Status = 'Pending'
            WHERE Status = 'Scheduled'
              AND DATEADD(MINUTE, DATEDIFF(MINUTE, 0, StartTime), CAST(AppointmentDate AS DATETIME)) < GETDATE()";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Method to populate the appointments dropdown from the database
        private void PopulateAppointments()
        {
            string patientID = Session["PatientID"].ToString();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT AppointmentID, DoctorID, AppointmentDate, StartTime FROM Appointments WHERE PatientID = @PatientID AND Status = 'Scheduled' ORDER BY AppointmentDate, StartTime";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientID", patientID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                ddlAppointments.Items.Clear();
                ddlAppointments.Items.Add(new ListItem("-- Select an Appointment --", ""));

                while (reader.Read())
                {
                    string appointmentText = $"Dr. {GetDoctorName(reader["DoctorID"].ToString())} - {Convert.ToDateTime(reader["AppointmentDate"]).ToString("dd-MM-yyyy")} {reader["StartTime"]}";
                    ddlAppointments.Items.Add(new ListItem(appointmentText, reader["AppointmentID"].ToString()));
                }
            }
        }

        // Helper method to get doctor name based on DoctorID (Replace with your actual data access)
        private string GetDoctorName(string doctorID)
        {
            string doctorName = string.Empty;

            // Fetch the doctor's name based on doctorID (implement database logic for this)
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT FullName FROM Doctors WHERE DoctorID = @DoctorID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DoctorID", doctorID);

                conn.Open();
                doctorName = cmd.ExecuteScalar()?.ToString();
            }

            return doctorName ?? "Doctor not found";
        }


        // Event handler for when an appointment is selected
        protected void ddlAppointments_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlAppointments.SelectedIndex <= 0) // No selection
            {
                pnlAppointmentDetails.Visible = false;
                return;
            }

            string appointmentId = ddlAppointments.SelectedValue;
            // Get appointment details from the database and show them in the panel
            LoadAppointmentDetails(appointmentId);
        }

        // Method to load appointment details
        private void LoadAppointmentDetails(string appointmentId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT AppointmentDate, StartTime, DoctorID, AppointmentID FROM Appointments WHERE AppointmentID = @AppointmentID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AppointmentID", appointmentId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    lblBookDate.Text = Convert.ToDateTime(reader["AppointmentDate"]).ToString("dd-MM-yyyy");
                    lblAppId.Text = reader["AppointmentID"].ToString();
                    lblDoctor.Text = GetDoctorName(reader["DoctorID"].ToString());
                    lblAppDate.Text = reader["StartTime"].ToString();
                    // Retrieve the StartTime as a TimeSpan object


                    pnlAppointmentDetails.Visible = true;
                }
            }
        }

        // Method to cancel the selected appointment
        protected void btnCancelBook_Click(object sender, EventArgs e)
        {
            string appointmentId = ddlAppointments.SelectedValue;
            if (!string.IsNullOrEmpty(appointmentId))
            {
                // Update the appointment status to "Cancelled"
                CancelAppointmentInDatabase(appointmentId);

                // Show success confirmation
                confirmationMessage.Visible = true;
                errorMessage.Visible = false;

                // Clear the form
                ddlAppointments.SelectedIndex = 0;
                lblBookDate.Text = "";
                lblAppId.Text = "";
                lblDoctor.Text = "";
                lblAppDate.Text = "";

                // Show confirmation message using JavaScript
                string script = "showConfirmationMessage();";
                ClientScript.RegisterStartupScript(this.GetType(), "ShowConfirmation", script, true);
            }
            else
            {
                // Show error message if no appointment is selected
                errorMessage.Visible = true;
                confirmationMessage.Visible = false;

                // Show error message using JavaScript
                string script = "showErrorMessage();";
                ClientScript.RegisterStartupScript(this.GetType(), "ShowError", script, true);
            }
        }

        // Method to update the appointment status to "Cancelled" in the database
        private void CancelAppointmentInDatabase(string appointmentId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE Appointments SET Status = 'Cancelled' WHERE AppointmentID = @AppointmentID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AppointmentID", appointmentId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
