using DataLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary
{
    public class TemplateForm
    {
        public int retVal;
        public string errorMsg = "";

        public int FormID { get; set; }
        public int FormTypeID { get; set; }
        public int FormTypeDescID { get; set; }
        public string FormTypeDesc { get; set; }
        public string ContactName { get; set; }
        public string EmployeeName { get; set; }
        public string Comments { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }

        public int Insert()
        {
            SqlConnection cn = new SqlConnection(Properties.Settings.Default["SandboxDB"].ToString());
            SqlCommand cmd = new SqlCommand("sp_InsertTemplateForm", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FormTypeID", FormTypeID);
            cmd.Parameters.AddWithValue("@ContactName", ContactName);
            cmd.Parameters.AddWithValue("@EmployeeName", EmployeeName);
            cmd.Parameters.AddWithValue("@Comments", Comments);
            cmd.Parameters.AddWithValue("@EffectiveDate", EffectiveDate);
            cmd.Parameters.AddWithValue("@ExpirationDate", ExpirationDate);
            cmd.Parameters.AddWithValue("@LastUpdateBy", LastUpdateBy);
            cmd.Parameters.AddWithValue("@LastUpdateDate", LastUpdateDate);

            using (cn)
            {
                try
                {
                    cn.Open();
                    retVal = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (Exception e)
                {
                    string message = e.Message;
                    retVal = 0;
                }
                finally
                {
                    cn.Close();
                }
            }

            return retVal;
        }

        public int Delete()
        {
            SqlConnection cn = new SqlConnection(Properties.Settings.Default["SandboxDB"].ToString());
            SqlCommand cmd = new SqlCommand("sp_DeleteTemplateForm", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("FormID", FormID);
            using (cn)
            {
                try
                {
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    retVal = 1;
                }
                catch (Exception e)
                {
                    string message = e.Message;
                    retVal = 0;
                }
                finally
                {
                    cn.Close();
                }
            }
            return retVal;
        }

        public List<TemplateForm> GetAll()
        {
            List<TemplateForm> list = new List<TemplateForm>();
            SqlConnection cn = new SqlConnection(Properties.Settings.Default["SandboxDB"].ToString());
            SqlCommand cmd = new SqlCommand("sp_GetAllTemplateForms", cn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            using (cn)
            {
                cn.Open();
                SqlDataReader rs;
                rs = cmd.ExecuteReader();
                while (rs.Read())
                {
                    TemplateForm tf = new TemplateForm();
                    tf.FormID = Convert.ToInt32(rs["FormID"]);
                    tf.FormTypeID = Convert.ToInt32(rs["FormTypeID"]);
                    tf.FormTypeDesc = rs["FormTypeDesc"].ToString();
                    tf.ContactName = rs["ContactName"].ToString();
                    tf.EmployeeName = rs["EmployeeName"].ToString();
                    tf.Comments = rs["Comments"].ToString();
                    tf.EffectiveDate = Convert.ToDateTime(rs["EffectiveDate"]);
                    tf.ExpirationDate = Convert.ToDateTime(rs["ExpirationDate"]);
                    tf.LastUpdateBy = rs["LastUpdateBy"].ToString();
                    tf.LastUpdateDate = Convert.ToDateTime(rs["LastUpdateDate"]);
                    list.Add(tf);
                }
            }
            return list;
        }

        public TemplateForm Get()
        {
            TemplateForm tf = new TemplateForm();
            SqlConnection cn = new SqlConnection(Properties.Settings.Default["SandboxDB"].ToString());
            SqlCommand cmd = new SqlCommand("sp_GetTemplateForms", cn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("FormID", FormID);
            using (cn)
            {
                cn.Open();
                SqlDataReader rs;
                rs = cmd.ExecuteReader();
                while (rs.Read())
                {
                    tf.FormID = Convert.ToInt32(rs["FormID"]);
                    tf.FormTypeID = Convert.ToInt32(rs["FormTypeID"]);
                    tf.FormTypeDesc = rs["FormTypeDesc"].ToString();
                    tf.ContactName = rs["ContactName"].ToString();
                    tf.EmployeeName = rs["EmployeeName"].ToString();
                    tf.Comments = rs["Comments"].ToString();
                    tf.EffectiveDate = Convert.ToDateTime(rs["EffectiveDate"]);
                    tf.ExpirationDate = Convert.ToDateTime(rs["ExpirationDate"]);
                    tf.LastUpdateBy = rs["LastUpdateBy"].ToString();
                    tf.LastUpdateDate = Convert.ToDateTime(rs["LastUpdateDate"]);
                }
            }
            return tf;
        }
    }
}
