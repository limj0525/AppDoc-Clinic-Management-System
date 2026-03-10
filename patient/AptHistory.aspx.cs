using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Configuration;

namespace patientsystem
{
    public partial class AptHistory : System.Web.UI.Page
    {
        private readonly string connStr =
            ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 1) Refresh any Scheduled → Pending transitions for this patient
                string patientId = Session["PatientID"].ToString();

                // 2) Now set up the page
                lblCurrentDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                LoadDoctors();
                UpdatePastAppointmentsStatus();

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


        /// <summary>
        /// Moves any Scheduled appointments for this patient whose start date/time
        /// has passed into Pending status.
        /// </summary>


        // Load Doctors into the filter dropdown
        private void LoadDoctors()
        {
            using (var conn = new SqlConnection(connStr))
            {
                const string q = "SELECT DoctorID, FullName FROM Doctors";
                using (var cmd = new SqlCommand(q, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        ddlDoctorFilter.Items.Clear();
                        ddlDoctorFilter.Items.Add(new ListItem("All Doctors", "All"));
                        while (reader.Read())
                        {
                            ddlDoctorFilter.Items.Add(
                                new ListItem(
                                    reader["FullName"].ToString(),
                                    reader["DoctorID"].ToString()
                                )
                            );
                        }
                    }
                }
            }
        }

        // Initial bind or after reset/search
        private void BindAppointments()
        {
            // Re-use same logic as in FilterAppointments, but no extra filters
            string query = @"
                SELECT 
                  a.AppointmentID,
                  a.PatientID,
                  a.DoctorID,
                  d.FullName AS DoctorName,
                  a.AppointmentDate,
                  a.StartTime,
                  a.Status,
                  a.TreatmentType
                FROM Appointments a
                JOIN Doctors d ON a.DoctorID = d.DoctorID
                WHERE a.PatientID = @PatientID
                ORDER BY a.AppointmentDate DESC, a.StartTime DESC";

            SqlDataSource1.SelectCommand = query;
            SqlDataSource1.SelectParameters.Clear();
            SqlDataSource1.SelectParameters.Add("PatientID", Session["PatientID"].ToString());
            gvAppointments.DataBind();
        }

        // Filter the appointments based on the selected criteria
        protected void FilterAppointments(object sender, EventArgs e)
        {
            string doctor = ddlDoctorFilter.SelectedValue;
            string selectedDate = txtDate.Text;
            string status = ddlStatusFilter.SelectedValue;

            string query = @"
                SELECT 
                  a.AppointmentID,
                  a.PatientID,
                  a.DoctorID,
                  d.FullName AS DoctorName,
                  a.AppointmentDate,
                  a.StartTime,
                  a.Status,
                  a.TreatmentType
                FROM Appointments a
                JOIN Doctors d ON a.DoctorID = d.DoctorID
                WHERE a.PatientID = @PatientID";

            if (doctor != "All")
                query += " AND a.DoctorID = @DoctorID";
            if (!string.IsNullOrEmpty(selectedDate))
                query += " AND CAST(a.AppointmentDate AS DATE) = @SelectedDate";
            if (status != "All")
                query += " AND a.Status = @Status";

            query += " ORDER BY a.AppointmentDate DESC, a.StartTime DESC";

            SqlDataSource1.SelectCommand = query;
            SqlDataSource1.SelectParameters.Clear();
            SqlDataSource1.SelectParameters.Add("PatientID", Session["PatientID"].ToString());
            if (doctor != "All")
                SqlDataSource1.SelectParameters.Add("DoctorID", doctor);
            if (!string.IsNullOrEmpty(selectedDate))
                SqlDataSource1.SelectParameters.Add("SelectedDate", selectedDate);
            if (status != "All")
                SqlDataSource1.SelectParameters.Add("Status", status);

            gvAppointments.DataBind();
        }

        // Reset the filters and show all appointments
        protected void ClearFilters(object sender, EventArgs e)
        {
            ddlDoctorFilter.SelectedIndex = 0;
            ddlStatusFilter.SelectedIndex = 0;
            txtDate.Text = "";

            BindAppointments();
        }

        // Handle Row Command events (e.g., Edit or Cancel Appointment)
        protected void gvAppointments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditAppointment")
            {
                string appointmentId = e.CommandArgument.ToString();
                Response.Redirect($"admin-edit-appointment.aspx?id={appointmentId}");
            }
            else if (e.CommandName == "CancelAppointment")
            {
                string appointmentId = e.CommandArgument.ToString();
                const string q = "UPDATE Appointments SET Status = 'Cancelled' WHERE AppointmentID = @AppointmentID";

                using (var conn = new SqlConnection(connStr))
                using (var cmd = new SqlCommand(q, conn))
                {
                    cmd.Parameters.AddWithValue("@AppointmentID", appointmentId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                gvAppointments.DataBind();
            }
        }
    }
}
