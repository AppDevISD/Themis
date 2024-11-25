using DataLibrary;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace DataLibrary
{
    public class Factory
    {
        public int retVal;
        public string errorMsg = "";
        public static Factory _Factory = null;
        private static SqlConnection _cn = null;

        public static Factory Instance
        {
            get
            {
                if (_Factory == null)
                {
                    _Factory = new Factory();
                }
                return _Factory;
            }
        }
        private Factory()
        {
            _cn = new SqlConnection();
            _cn.Close();
        }

        // TEMPLATE //
        public int InsertTemplateForm(Template template)
        {
            SqlConnection cn = new SqlConnection(Properties.Settings.Default["SandboxDB"].ToString());
            SqlCommand cmd = new SqlCommand("sp_InsertTemplateForm", cn);
            cmd.Parameters.AddWithValue("@FormTypeID", template.FormTypeID);
            cmd.Parameters.AddWithValue("@ContactName", template.ContactName);
            cmd.Parameters.AddWithValue("@EmployeeName", template.EmployeeName);
            cmd.Parameters.AddWithValue("@Comments", template.Comments);
            cmd.Parameters.AddWithValue("@EffectiveDate", template.EffectiveDate);
            cmd.Parameters.AddWithValue("@ExpirationDate", template.ExpirationDate);
            cmd.Parameters.AddWithValue("@LastUpdateBy", template.LastUpdateBy);
            cmd.Parameters.AddWithValue("@LastUpdateDate", template.LastUpdateDate);
            SqlParameter outputParam = new SqlParameter("@nID", SqlDbType.Int);
            outputParam.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(outputParam);
            cmd.CommandType = CommandType.StoredProcedure;

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
        public int DeleteTemplateForm(int templateID)
        {
            SqlConnection cn = new SqlConnection(Properties.Settings.Default["SandboxDB"].ToString());
            SqlCommand cmd = new SqlCommand("sp_DeleteTemplateForm", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("FormID", templateID);
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
        public List<Template> GetAllTemplateForms()
        {
            List<Template> list = new List<Template>();
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
                    Template tf = new Template();
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
        public Template GetTemplateForm(int templateID)
        {
            Template tf = new Template();
            SqlConnection cn = new SqlConnection(Properties.Settings.Default["SandboxDB"].ToString());
            SqlCommand cmd = new SqlCommand("sp_GetTemplateForms", cn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("FormID", templateID);
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
