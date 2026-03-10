using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web_Assignment
{
    public partial class doctor_view_appointment : System.Web.UI.Page
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;
        string currentPatientId;
        protected void Page_Load(object sender, EventArgs e)
        {
            InjectChartData();
            if (!IsPostBack)
            {
                lblCurrentDate.Text = DateTime.Now.ToString("dd-MMM-yyyy");
                if (Session["DoctorID"] == null)
                {
                    Response.Redirect("../index.aspx");
                    return;
                }

                // build a 24-item list: "00:00", "01:00", …, "23:00"
                for (int h = 0; h < 24; h++)
                {
                    string hh = h.ToString("D2") + ":00";
                    ddlStartHour.Items.Add(new ListItem(hh, hh));
                    ddlEndHour.Items.Add(new ListItem(hh, hh));
                }

                // optionally insert a prompt at top
                ddlStartHour.Items.Insert(0, new ListItem("-- Start Hour --", ""));
                ddlEndHour.Items.Insert(0, new ListItem("-- End Hour --", ""));

                UpdatePastAppointmentsStatus();
                LoadAvailability();
                LoadAppointments();
                BindAvailability();
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


        private void BindAvailability()
        {
            var dt = new DataTable();
            using (var con = new SqlConnection(connectionString))
            using (var da = new SqlDataAdapter(
                @"SELECT 
                    ScheduleID,
                    [date]         AS Date,
                    available_start_time AS StartTime,
                    available_end_time   AS EndTime
                  FROM Schedule
                 WHERE DoctorID = @DoctorID
                 ORDER BY [date]",
                con))
            {
                da.SelectCommand.Parameters.AddWithValue("@DoctorID", Session["DoctorID"]);
                da.Fill(dt);
            }

            gvAvailability.DataSource = dt;
            gvAvailability.DataBind();
        }

        protected void gvAppointments_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            // Tell the GridView which page to show
            gvAppointments.PageIndex = e.NewPageIndex;

            // Re-bind your data
            BindAppointments();
            // (or whatever method you use to load & DataBind() gvAppointments)
        }

        private void BindAppointments()
        {
            const string sql = @"
        SELECT 
            a.AppointmentID,
            a.PatientID,
            p.FullName   AS PatientName,
            a.AppointmentDate,
            a.StartTime,
            a.Status,
            a.TreatmentType
          FROM Appointments a
          JOIN Patients     p
            ON a.PatientID = p.PatientID
         WHERE a.DoctorID = @doc
      ORDER BY a.AppointmentDate, a.StartTime";

            var dt = new DataTable();
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, con))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@doc", Session["DoctorID"].ToString());
                da.Fill(dt);
            }

            gvAppointments.DataSource = dt;
            gvAppointments.DataBind();
        }



        private void LoadAvailability()
        {
            string doctorId = Session["DoctorID"].ToString();
            string query = "SELECT ScheduleID, date AS [Date], available_start_time AS StartTime, available_end_time AS EndTime FROM Schedule WHERE DoctorID = @DoctorID ORDER BY date";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvAvailability.DataSource = dt;
                gvAvailability.DataBind();
            }
        }

        protected void btnSaveAvailability_Click(object sender, EventArgs e)
        {
            // 1) Parse inputs
            if (string.IsNullOrWhiteSpace(txtAvailabilityDate.Text)
             || string.IsNullOrWhiteSpace(ddlStartHour.SelectedValue)
             || string.IsNullOrWhiteSpace(ddlEndHour.SelectedValue))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alertMissing",
                    "Swal.fire({ icon: 'error', title: 'Missing Info', text: 'Please select date, start and end hours.' });", true);
                return;
            }

            if (!DateTime.TryParse(txtAvailabilityDate.Text, out DateTime date))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alertDate",
                    "Swal.fire({ icon: 'error', title: 'Invalid Date', text: 'Invalid date format.' });", true);
                return;
            }

            var start = TimeSpan.Parse(ddlStartHour.SelectedValue);
            var end = TimeSpan.Parse(ddlEndHour.SelectedValue);

            // 2) Validate order and business hours
            var businessStart = new TimeSpan(9, 0, 0);
            var businessEnd = new TimeSpan(21, 0, 0);

            if (start >= end)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alertOrder",
                    "Swal.fire({ icon: 'error', title: 'Invalid Range', text: 'Start time must be earlier than end time.' });", true);
                return;
            }
            if (start < businessStart || end > businessEnd)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alertBusiness",
                    "Swal.fire({ icon: 'error', title: 'Outside Business Hours', text: 'Availability must be between 09:00 and 21:00.' });", true);
                return;
            }

            // 3) Check for overlap
            string doctorId = Session["DoctorID"]?.ToString() ?? "";

            const string overlapSql = @"
        SELECT COUNT(*) 
          FROM Schedule
         WHERE DoctorID            = @doc
           AND [date]              = @date
           AND @start < available_end_time
           AND @end   > available_start_time";

            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(overlapSql, con))
            {
                cmd.Parameters.AddWithValue("@doc", doctorId);
                cmd.Parameters.AddWithValue("@date", date.Date);
                cmd.Parameters.AddWithValue("@start", start);
                cmd.Parameters.AddWithValue("@end", end);

                con.Open();
                int conflictCount = (int)cmd.ExecuteScalar();
                if (conflictCount > 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alertOverlap",
                       "Swal.fire({ icon: 'error', title: 'Overlap', text: 'This slot overlaps an existing availability.' });", true);
                    return;
                }
            }
            // 0) Generate a new ScheduleID of the form "S001", "S002", etc.
            string newId;
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT MAX(ScheduleID) FROM Schedule", con))
            {
                con.Open();
                var result = cmd.ExecuteScalar() as string;
                if (!string.IsNullOrEmpty(result) && result.Length > 1)
                {
                    // parse numeric part and increment
                    int num = int.Parse(result.Substring(1));
                    newId = "S" + (num + 1).ToString("D3");
                }
                else
                {
                    // first row ever
                    newId = "S001";
                }
            }

            // 4) Insert new availability, now including ScheduleID
            const string insertSql = @"
        INSERT INTO Schedule
            (ScheduleID, DoctorID, [date], available_start_time, available_end_time)
        VALUES
            (@id, @doc, @date, @start, @end)";

            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(insertSql, con))
            {
                cmd.Parameters.AddWithValue("@id", newId);
                cmd.Parameters.AddWithValue("@doc", Session["DoctorID"].ToString());
                cmd.Parameters.AddWithValue("@date", date.Date);
                cmd.Parameters.AddWithValue("@start", start);
                cmd.Parameters.AddWithValue("@end", end);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            // 5) Refresh
            BindAvailability();
            ScriptManager.RegisterStartupScript(this, GetType(), "alertSaved",
                "Swal.fire({ icon: 'success', title: 'Saved', text: 'Availability saved.' });", true);
        }



        private void LoadAppointments()
        {
            string doctorId = Session["DoctorID"]?.ToString();
            string search = txtSearch.Text.Trim();
            string status = ddlStatus.SelectedValue;

            string query = @"
        SELECT a.AppointmentID, a.PatientID, p.FullName AS PatientName, a.AppointmentDate,
               a.StartTime, a.Status, a.TreatmentType
        FROM Appointments a
        INNER JOIN Patients p ON a.PatientID = p.PatientID
        WHERE a.DoctorID = @DoctorID";

            if (!string.IsNullOrEmpty(search))
                query += " AND p.FullName LIKE @Search";

            if (status != "All")
                query += " AND a.Status = @Status";

            query += " ORDER BY a.AppointmentDate DESC";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                if (!string.IsNullOrEmpty(search))
                    cmd.Parameters.AddWithValue("@Search", "%" + search + "%");
                if (status != "All")
                    cmd.Parameters.AddWithValue("@Status", status);

                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                gvAppointments.DataSource = dt;
                gvAppointments.DataBind();
            }
        }


        protected void btnFilter_Click(object sender, EventArgs e)
        {
            LoadAppointments(); // or your filtering logic
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            ddlStatus.SelectedIndex = 0;
            LoadAppointments(); // reload with default (all) filters
        }


        protected void gvAvailability_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string scheduleId = gvAvailability.DataKeys[e.RowIndex].Value.ToString();
            string doctorId = Session["DoctorID"].ToString();

            // 1) Fetch the availability window
            DateTime date;
            TimeSpan start, end;
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(
                "SELECT [date], available_start_time, available_end_time " +
                "FROM Schedule WHERE ScheduleID = @ScheduleID", con))
            {
                cmd.Parameters.AddWithValue("@ScheduleID", scheduleId);
                con.Open();
                using (var rdr = cmd.ExecuteReader())
                {
                    if (!rdr.Read())
                    {
                        // nothing to delete
                        e.Cancel = true;
                        return;
                    }
                    date = (DateTime)rdr["date"];
                    start = (TimeSpan)rdr["available_start_time"];
                    end = (TimeSpan)rdr["available_end_time"];
                }
            }

            // 2) Check for any appointments in this slot
            const string checkSql = @"
        SELECT COUNT(*) 
          FROM Appointments
         WHERE DoctorID        = @doc
           AND AppointmentDate = @date
           AND Status != 'Cancelled'
           AND StartTime >= @start
           AND StartTime <  @end";
            int conflictCount;
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(checkSql, con))
            {
                cmd.Parameters.AddWithValue("@doc", doctorId);
                cmd.Parameters.AddWithValue("@date", date.Date);
                cmd.Parameters.AddWithValue("@start", start);
                cmd.Parameters.AddWithValue("@end", end);

                con.Open();
                conflictCount = (int)cmd.ExecuteScalar();
            }

            if (conflictCount > 0)
            {
                // 3) Prevent deletion if any appointments exist
                ScriptManager.RegisterStartupScript(this, GetType(), "alertDelete",
                     $"Swal.fire({{ icon: 'error', title: 'Cannot Delete', text: 'There are {conflictCount} appointment(s) in this slot.' }});", true);

                e.Cancel = true;
                return;
            }

            // 4) No conflicts → delete and rebind
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(
                "DELETE FROM Schedule WHERE ScheduleID = @ScheduleID", con))
            {
                cmd.Parameters.AddWithValue("@ScheduleID", scheduleId);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            BindAvailability();
        }



        protected void gvAvailability_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvAvailability.PageIndex = e.NewPageIndex;
            BindAvailability();
        }
        private void InjectChartData()
        {
            string doctorId = Session["DoctorID"].ToString();
            Dictionary<string, int> counts = new Dictionary<string, int>
            {
                { "Scheduled", 0 }, { "Pending", 0 }, { "Completed", 0 }, { "Missed", 0 }, { "Cancelled", 0 }
            };

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                foreach (var status in counts.Keys.ToList())
                {
                    SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Appointments WHERE DoctorID = @DoctorID AND Status = @Status", con);
                    cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                    cmd.Parameters.AddWithValue("@Status", status);
                    counts[status] = (int)cmd.ExecuteScalar();
                }
            }

            var labels = string.Join(",", counts.Keys.Select(s => $"'{s}'"));
            var data = string.Join(",", counts.Values);

            string script = $@"<script>
                var ctx = document.getElementById('appointmentChart').getContext('2d');
                new Chart(ctx, {{
                    type: 'pie',
                    data: {{
                        labels: [{labels}],
                        datasets: [{{
                            data: [{data}],
                            backgroundColor: ['#36a2eb', '#ffcd56', '#4bc0c0', '#ff6384', '#999999']
                        }}]
                    }}
                }});
            </script>";

            ScriptManager.RegisterStartupScript(this, GetType(), "chartScript", script, false);
        }

        protected void gvAppointments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // only handle our new commands here
            if (e.CommandName == "MarkComplete" || e.CommandName == "MarkMissed")
            {
                // get the appointment ID
                var apptId = e.CommandArgument.ToString();
                // decide new status
                var newStatus = e.CommandName == "MarkComplete"
                                ? "Completed"
                                : "Missed";

                UpdateAppointmentStatus(apptId, newStatus);
                BindAppointments();
            }

            if (e.CommandName == "AddNotes")
            {
                currentPatientId = e.CommandArgument.ToString();
                ViewState["CurrentPatient"] = currentPatientId;
                BindNotesModal(currentPatientId);
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowNotesModal", "$('#pnlNotesModal').modal('show');", true);
            }

            if (e.CommandName == "PatientDetail")
            {
                string patientId = e.CommandArgument.ToString();

                // 1) Load patient data into the modal DetailsView
                BindPatientModal(patientId);

                // 2) Show the Bootstrap modal
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "ShowPatientModal",
                    "$('#pnlPatientModal').modal('show');",
                    true
                );
            }
        }
        private void BindNotesModal(string patientId)
        {
            var dt = new DataTable();
            using (var da = new SqlDataAdapter(
                     "SELECT NoteID, DateTime, Assessment, Prescription, Treatment " +
                     "FROM Notes WHERE PatientID=@pid ORDER BY DateTime DESC",
                     new SqlConnection(connectionString)))
            {
                da.SelectCommand.Parameters.AddWithValue("@pid", patientId);
                da.Fill(dt);
            }
            gvNotesModal.DataSource = dt;
            gvNotesModal.DataBind();
        }
        protected void gvNotesModal_RowEditing(object sender, GridViewEditEventArgs e)
        {
            // set the row into edit mode
            gvNotesModal.EditIndex = e.NewEditIndex;
            BindNotesModal(ViewState["CurrentPatient"].ToString());

            // re-open the modal
            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "ShowNotesModal",
                "$('#pnlNotesModal').modal('show');",
                true);
        }

        protected void gvNotesModal_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            // cancel edit mode
            gvNotesModal.EditIndex = -1;
            BindNotesModal(ViewState["CurrentPatient"].ToString());

            // and re-open
            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "ShowNotesModal",
                "$('#pnlNotesModal').modal('show');",
                true);
        }

        protected void gvNotesModal_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            // your existing update logic…
            string noteId = gvNotesModal.DataKeys[e.RowIndex].Value.ToString();
            var row = gvNotesModal.Rows[e.RowIndex];
            string assess = ((TextBox)row.FindControl("txtAssessmentEdit")).Text;
            string prescr = ((TextBox)row.FindControl("txtPrescriptionEdit")).Text;
            string treat = ((TextBox)row.FindControl("txtTreatmentEdit")).Text;

            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(
                "UPDATE Notes SET Assessment=@a, Prescription=@p, Treatment=@t WHERE NoteID=@nid",
                con))
            {
                cmd.Parameters.AddWithValue("@a", assess);
                cmd.Parameters.AddWithValue("@p", prescr);
                cmd.Parameters.AddWithValue("@t", treat);
                cmd.Parameters.AddWithValue("@nid", noteId);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            // leave edit mode and re-bind
            gvNotesModal.EditIndex = -1;
            BindNotesModal(ViewState["CurrentPatient"].ToString());

            // and re-open
            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "ShowNotesModal",
                "$('#pnlNotesModal').modal('show');",
                true);
        }

        // **Saving new note**  
        protected void btnSaveNewNote_Click(object sender, EventArgs e)
        {
            string pid = ViewState["CurrentPatient"].ToString();
            // generate NoteID (e.g. N001)
            string newId;
            using (var cmd = new SqlCommand("SELECT MAX(NoteID) FROM Notes", new SqlConnection(connectionString)))
            {
                cmd.Connection.Open();
                var max = cmd.ExecuteScalar() as string;
                if (!string.IsNullOrEmpty(max) && max.Length > 1)
                {
                    int num = int.Parse(max.Substring(1)) + 1;
                    newId = "N" + num.ToString("D3");
                }
                else newId = "N001";
            }

            using (var cmd = new SqlCommand(
                "INSERT INTO Notes(NoteID, PatientID, DateTime, Assessment, Prescription, Treatment) " +
                "VALUES(@nid,@pid,GETDATE(),@a,@p,@t)",
                new SqlConnection(connectionString)))
            {
                cmd.Parameters.AddWithValue("@nid", newId);
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.Parameters.AddWithValue("@a", txtNewAssessment.Text);
                cmd.Parameters.AddWithValue("@p", txtNewPrescription.Text);
                cmd.Parameters.AddWithValue("@t", txtNewTreatment.Text);
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }

            // clear inputs
            txtNewAssessment.Text = txtNewPrescription.Text = txtNewTreatment.Text = "";
            BindNotesModal(pid);
        }

        private void BindPatientModal(string patientId)
        {
            var dt = new DataTable();
            using (var con = new SqlConnection(connectionString))
            using (var da = new SqlDataAdapter(
                @"SELECT 
              PatientID,
              FullName,
              ICNumber,
              DOB,
              Gender,
              Email,
              Phone,
              Address,
              BloodType,
              Allergy,
              MedicalCon,
              EmerName,
              EmerRship,
              EmerPhone
          FROM Patients
         WHERE PatientID = @pid", con))
            {
                da.SelectCommand.Parameters.AddWithValue("@pid", patientId);
                da.Fill(dt);
            }

            dvPatientModal.DataSource = dt;
            dvPatientModal.DataBind();
        }

        private void UpdateAppointmentStatus(string appointmentId, string status)
        {
            const string sql = @"
                UPDATE Appointments
                   SET Status = @status
                 WHERE AppointmentID = @id";

            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@id", appointmentId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
