using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Assignment
{
    public partial class admin_manage_doctor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Code for page load if needed
        }

        protected void gvDoctors_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string doctorId = e.CommandArgument.ToString();

            if (e.CommandName == "Edit")
            {
                Response.Redirect("admin-edit-doctor.aspx?DoctorID=" + doctorId);
            }

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            string statusFilter = ddlStatusFilter.SelectedValue;  // Get the selected status filter

            // Build the SQL query dynamically based on the filter
            string query = "SELECT * FROM Doctors WHERE FullName LIKE @Keyword";

            // If a status filter is selected, add the condition to the query
            if (!string.IsNullOrEmpty(statusFilter))
            {
                query += " AND Status = @Status";
            }

            // Update the SqlDataSource command
            SqlDataSource1.SelectCommand = query;
            SqlDataSource1.SelectParameters.Clear();
            SqlDataSource1.SelectParameters.Add("Keyword", "%" + keyword + "%");

            // If a status filter is selected, add the status filter parameter
            if (!string.IsNullOrEmpty(statusFilter))
            {
                SqlDataSource1.SelectParameters.Add("Status", statusFilter);
            }
        }

        protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Trigger search when the status is changed
            btnSearch_Click(sender, e);
        }


        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            SqlDataSource1.SelectCommand = "SELECT * FROM Doctors";
            SqlDataSource1.SelectParameters.Clear();
            gvDoctors.DataBind(); // Optional to force refresh
        }


    }
}