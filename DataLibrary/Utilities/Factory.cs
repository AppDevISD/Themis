using DataLibrary;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using System.Web.UI;
using System.Linq;
using System.Runtime.InteropServices;

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


        // GETS //
        public List<T> GetAll<T>(string sp)
        {
            PropertyInfo[] classType = typeof(T).GetProperties();
            List<T> list = new List<T>();
            SqlConnection cn = new SqlConnection(Properties.Settings.Default["ThemisDB"].ToString());
            SqlCommand cmd = new SqlCommand(sp, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            using (cn)
            {
                cn.Open();
                SqlDataReader rs;
                rs = cmd.ExecuteReader();
                while (rs.Read())
                {

                    T item = (T)Activator.CreateInstance(typeof(T));
                    foreach (var property in classType)
                    {
                        Type propertyType = property.PropertyType;
                        object value;
                        try
                        {
                            value = Convert.ChangeType(rs[property.Name], propertyType);
                        }
                        catch (Exception)
                        {

                            value = null;
                        }
                         
                        property.SetValue(item, value);                      
                    }
                    list.Add(item);
                }
            }
            return list;
        }
        public List<T> GetAllLookup<T>(int id, string sp)
        {
            PropertyInfo[] classType = typeof(T).GetProperties();
            List<T> list = new List<T>();
            SqlConnection cn = new SqlConnection(Properties.Settings.Default["ThemisDB"].ToString());
            SqlCommand cmd = new SqlCommand(sp, cn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            using (cn)
            {
                cn.Open();
                SqlDataReader rs;
                rs = cmd.ExecuteReader();
                while (rs.Read())
                {

                    T item = (T)Activator.CreateInstance(typeof(T));
                    foreach (var property in classType)
                    {
                        Type propertyType = property.PropertyType;
                        object value = Convert.ChangeType(rs[property.Name], propertyType);
                        property.SetValue(item, value);
                    }
                    list.Add(item);
                }
            }
            return list;
        }
        public T GetByID<T>(int id, string sp)
        {
            T item = (T)Activator.CreateInstance(typeof(T));
            PropertyInfo[] classType = typeof(T).GetProperties();
            SqlConnection cn = new SqlConnection(Properties.Settings.Default["ThemisDB"].ToString());
            SqlCommand cmd = new SqlCommand(sp, cn);
            cmd.CommandType = CommandType.StoredProcedure;

            using (cn)
            {
                cn.Open();
                SqlDataReader rs;
                rs = cmd.ExecuteReader();
                if (rs.Read())
                {
                    foreach (var property in classType)
                    {
                        Type propertyType = property.PropertyType;
                        object value = Convert.ChangeType(rs[property.Name], propertyType);
                        property.SetValue(item, value);
                    }
                    cmd.CommandType = CommandType.StoredProcedure;
                }
            }
            return item;
        }



        // INSERTS //
        public int Insert<T>(T item, string sp)
        {
            SqlConnection cn = new SqlConnection(Properties.Settings.Default["ThemisDB"].ToString());
            
            PropertyInfo[] classType = typeof(T).GetProperties();
            SqlCommand cmd = new SqlCommand(sp, cn);
            foreach (var property in classType.Skip(1))
            {
                cmd.Parameters.AddWithValue($"@p{property.Name}", property.GetValue(item));
            }
            SqlParameter outputParam = new SqlParameter("@nID", SqlDbType.Int);
            outputParam.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(outputParam);
            cmd.CommandType = CommandType.StoredProcedure;

            using (cn)
            {
                try
                {
                    cn.Open();
                    retVal = cmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(outputParam.Value);
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



        // DELETES //
        public int Delete<T>(int id, string tableName)
        {
            SqlConnection cn = new SqlConnection(Properties.Settings.Default["ThemisDB"].ToString());

            PropertyInfo[] classType = typeof(T).GetProperties();
            SqlCommand cmd = new SqlCommand($"sp_Delete{tableName}", cn);
            cmd.Parameters.AddWithValue($"@p{classType.First().Name}", id);
            cmd.CommandType = CommandType.StoredProcedure;

            int ret;
            try
            {
                using (cn)
                {
                    cn.Open();
                    ret = cmd.ExecuteNonQuery();
                    Debug.WriteLine(ret);
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ret = -1;
            }
            return ret;
        }
        public int Expire<T>(T item, string tableName) {
            SqlConnection cn = new SqlConnection(Properties.Settings.Default["ThemisDB"].ToString());

            PropertyInfo[] classType = typeof(T).GetProperties();
            SqlCommand cmd = new SqlCommand($"sp_Update{tableName}", cn);
            foreach (var property in classType)
            {
                cmd.Parameters.AddWithValue($"@p{property.Name}", property.GetValue(item));
            }
            cmd.Parameters["@pExpirationDate"].Value = DateTime.Now;
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                using (cn)
                {
                    cn.Open();
                    retVal = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                retVal = -1;
            }
            return retVal;
        }
    }
}
