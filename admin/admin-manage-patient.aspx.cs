using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Assignment
{
    public partial class admin_manage_patient : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindPatientsGrid();
            }
        }

        protected string GetProfileImagePath(object profilePicObj)
        {
            try
            {
                // 1. Handle null/DBNull cases
                if (profilePicObj == null || Convert.IsDBNull(profilePicObj))
                    return GetDefaultImagePath();

                // 2. Convert to string and check for empty
                string dbPath = profilePicObj.ToString().Trim();
                if (string.IsNullOrWhiteSpace(dbPath))
                    return GetDefaultImagePath();

                // 3. Verify path format security
                if (dbPath.Contains("..") || dbPath.Contains(":"))
                    return GetDefaultImagePath();

                // 4. Check physical file existence
                string physicalPath = Server.MapPath(dbPath);
                if (!File.Exists(physicalPath))
                {
                    // Optional: Log missing files
                    System.Diagnostics.Trace.WriteLine($"Missing profile image: {dbPath}");
                    return GetDefaultImagePath();
                }

                return ResolveUrl(dbPath);
            }
            catch (Exception ex)
            {
                // Log error and return default
                System.Diagnostics.Trace.WriteLine($"Profile image error: {ex.Message}");
                return GetDefaultImagePath();
            }
        }

        private string GetDefaultImagePath()
        {
            return ResolveUrl("~/profile_pics/default.jpeg");
        }

        private void BindPatientsGrid()
        {
            try
            {
                gvPatients.DataBind();
            }
            catch (Exception ex)
            {
                // Handle grid binding errors
                Response.Write($"<script>console.error('Grid binding error: {ex.Message}');</script>");
            }
        }

        protected void gvPatients_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string patientId = e.CommandArgument.ToString();

            if (e.CommandName == "Edit")
            {
                Response.Redirect("admin-edit-patient.aspx?id=" + patientId);
            }

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();

            if (!string.IsNullOrEmpty(keyword))
            {
                SqlDataSource1.SelectCommand = "SELECT * FROM Patients WHERE FullName LIKE @Keyword";
                SqlDataSource1.SelectParameters.Clear();
                SqlDataSource1.SelectParameters.Add("Keyword", "%" + keyword + "%");
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            SqlDataSource1.SelectCommand = "SELECT * FROM Patients";
            SqlDataSource1.SelectParameters.Clear();
            gvPatients.DataBind(); // Optional to force refresh
        }


    }
}