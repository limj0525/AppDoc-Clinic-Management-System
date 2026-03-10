using System;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;

namespace Assignment
{
    public partial class admin_manage_appointments : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["updated"] == "true")
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "updateSuccess",
                        "Swal.fire({ icon: 'success', title: 'Updated!', text: 'Appointment updated successfully.' });", true);
                }

                LoadDoctorsDropdown();
                ApplyFilterQuery("All", "", "All");
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


        private void LoadDoctorsDropdown()
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT DoctorID, FullName, Specialization FROM Doctors WHERE Status = 'Active'";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                ddlDoctor.Items.Clear();
                ddlDoctor.Items.Add(new ListItem("All", "All"));

                while (reader.Read())
                {
                    string displayText = $"{reader["FullName"]} - {reader["Specialization"]}";
                    ddlDoctor.Items.Add(new ListItem(displayText, reader["DoctorID"].ToString()));
                }
                reader.Close();
            }
        }

        protected void FilterAppointments(object sender, EventArgs e)
        {
            string doctor = ddlDoctor.SelectedValue;
            string date = txtDate.Text;
            string status = ddlStatus.SelectedValue;

            ViewState["Filter_Doctor"] = doctor;
            ViewState["Filter_Date"] = date;
            ViewState["Filter_Status"] = status;

            ApplyFilterQuery(doctor, date, status);
        }

        private void ApplyFilterQuery(string doctor, string date, string status)
        {
            string query = @"
                SELECT 
                    a.AppointmentID, a.PatientID, a.DoctorID, 
                    p.FullName AS PatientName, d.FullName AS DoctorName, 
                    a.AppointmentDate, a.StartTime, a.Status, a.TreatmentType
                FROM Appointments a
                JOIN Patients p ON a.PatientID = p.PatientID
                JOIN Doctors d ON a.DoctorID = d.DoctorID
                WHERE 1=1";

            SqlDataSource1.SelectParameters.Clear();

            if (doctor != "All")
            {
                query += " AND a.DoctorID = @DoctorID";
                SqlDataSource1.SelectParameters.Add("DoctorID", doctor);
            }

            if (!string.IsNullOrEmpty(date))
            {
                query += " AND CAST(a.AppointmentDate AS DATE) = @Date";
                SqlDataSource1.SelectParameters.Add("Date", date);
            }

            if (status != "All")
            {
                query += " AND a.Status = @Status";
                SqlDataSource1.SelectParameters.Add("Status", status);
            }

            SqlDataSource1.SelectCommand = query;
            gvAppointments.DataBind();
        }

        protected void ClearFilters(object sender, EventArgs e)
        {
            ddlDoctor.SelectedIndex = 0;
            ddlStatus.SelectedIndex = 0;
            txtDate.Text = "";

            ViewState["Filter_Doctor"] = "All";
            ViewState["Filter_Date"] = "";
            ViewState["Filter_Status"] = "All";

            ApplyFilterQuery("All", "", "All");
        }

        protected void gvAppointments_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvAppointments.PageIndex = e.NewPageIndex;

            string doctor = ViewState["Filter_Doctor"] as string ?? "All";
            string date = ViewState["Filter_Date"] as string ?? "";
            string status = ViewState["Filter_Status"] as string ?? "All";

            ApplyFilterQuery(doctor, date, status);
        }

        protected void gvAppointments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditAppointment")
            {
                string appointmentId = e.CommandArgument.ToString();
                Response.Redirect("admin-edit-appointment.aspx?AppointmentID=" + appointmentId);
            }
            else if (e.CommandName == "CancelAppointment")
            {
                string appointmentId = e.CommandArgument.ToString();

                string connStr = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string query = "UPDATE Appointments SET Status = 'Cancelled' WHERE AppointmentID = @AppointmentID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@AppointmentID", appointmentId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                // Refresh data
                string doctor = ViewState["Filter_Doctor"] as string ?? "All";
                string date = ViewState["Filter_Date"] as string ?? "";
                string status = ViewState["Filter_Status"] as string ?? "All";

                ApplyFilterQuery(doctor, date, status);

                ScriptManager.RegisterStartupScript(this, GetType(), "cancelSuccess",
                    "Swal.fire({ icon: 'success', title: 'Cancelled!', text: 'Appointment successfully cancelled.' });", true);
            }
        }
    }
}
