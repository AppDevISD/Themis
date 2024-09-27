using DataLibrary;
using ISD.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DataLibrary
{
    public class Factory
    {
        public static Factory _Factory = null;
        private static SqlConnection _cn = null;
        public string errorMsg = "";

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
        public List<Division> LoadDivisionsByDept(int deptCode)
        {
            List<Division> lDivision = new List<Division>();
            _cn = new SqlConnection(Properties.Settings.Default["EmployeeDirectoryDB"].ToString());
            SqlCommand cmd = new SqlCommand("spLoadDivisionsByDepartment", _cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@pDeptID", deptCode);

            using (_cn)
            {
                _cn.Open();
                SqlDataReader rs;
                rs = cmd.ExecuteReader();

                while (rs.Read())
                {
                    Division e = new Division();
                    e.div_code = rs["divCode"].ToString();
                    e.div_name = rs["divDescription"].ToString();
                    e.div_name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(e.div_name.ToLower());
                    lDivision.Add(e);
                }

            }
            return lDivision;
        }
        public List<Department> LoadDepartments()
        {
            List<Department> lDepartments = new List<Department>();

            _cn = new SqlConnection(Properties.Settings.Default["ISDAdmin"].ToString());
            SqlCommand cmd = new SqlCommand("GetlkDepartment", _cn);
            cmd.CommandType = CommandType.StoredProcedure;
            using (_cn)
            {
                _cn.Open();
                SqlDataReader rs;
                rs = cmd.ExecuteReader();

                while (rs.Read())
                {
                    Department e = new Department();
                    e.dept_code = rs["DepartmentID"].ToString();
                    e.dept_name = rs["DepartmentDesc"].ToString();
                    lDepartments.Add(e);
                }

            }

            return lDepartments;
        }


        // ORDINANCE TRACKING
        // INSERTS
        public int InsertAccounting(Accounting accounting)
        {
            int ret = 0;
            _cn = new SqlConnection(Properties.Settings.Default["ThemisDatabase"].ToString());
            SqlCommand cmd = new SqlCommand("sp_InsertAccounting", _cn);
            cmd.Parameters.AddWithValue("@FundCode", accounting.FundCode);
            cmd.Parameters.AddWithValue("@DepartmentCode", accounting.DepartmentCode);
            cmd.Parameters.AddWithValue("@UnitCode", accounting.UnitCode);
            cmd.Parameters.AddWithValue("@ActivityCode", accounting.ActivityCode);
            cmd.Parameters.AddWithValue("@ObjectCode", accounting.ObjectCode);
            cmd.Parameters.AddWithValue("@Amount", accounting.Amount);
            SqlParameter outputParam = new SqlParameter("@nID", SqlDbType.Int);
            outputParam.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(outputParam);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                using (_cn)
                {
                    _cn.Open();
                    ret = cmd.ExecuteNonQuery();
                    ret = Convert.ToInt32(outputParam.Value);
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ret = -1;
            }
            return ret;
        }
        public int InsertAccountingType(AccountingType accountingType)
        {
            int ret = 0;
            _cn = new SqlConnection(Properties.Settings.Default["ThemisDatabase"].ToString());
            SqlCommand cmd = new SqlCommand("sp_InsertlkAccountingType", _cn);
            cmd.Parameters.AddWithValue("@AccountingTypeDescription", accountingType.AccountingTypeDescription);
            SqlParameter outputParam = new SqlParameter("@nID", SqlDbType.Int);
            outputParam.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(outputParam);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                using (_cn)
                {
                    _cn.Open();
                    ret = cmd.ExecuteNonQuery();
                    ret = Convert.ToInt32(outputParam.Value);
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ret = -1;
            }
            return ret;
        }
        public int InsertBidder(Bidder bidder)
        {
            int ret = 0;
            _cn = new SqlConnection(Properties.Settings.Default["ThemisDatabase"].ToString());
            SqlCommand cmd = new SqlCommand("sp_InsertBidder", _cn);
            cmd.Parameters.AddWithValue("@VendorID", bidder.VendorID);
            cmd.Parameters.AddWithValue("@BidderTypeID", bidder.BidderTypeID);
            cmd.Parameters.AddWithValue("@BidderName", bidder.BidderName);
            cmd.Parameters.AddWithValue("@InsertDate", bidder.InsertDate);
            SqlParameter outputParam = new SqlParameter("@nID", SqlDbType.Int);
            outputParam.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(outputParam);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                using (_cn)
                {
                    _cn.Open();
                    ret = cmd.ExecuteNonQuery();
                    ret = Convert.ToInt32(outputParam.Value);
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ret = -1;
            }
            return ret;
        }
        public int InsertBidderType(BidderType bidderType)
        {
            int ret = 0;
            _cn = new SqlConnection(Properties.Settings.Default["ThemisDatabase"].ToString());
            SqlCommand cmd = new SqlCommand("sp_InsertlkBidderType", _cn);
            cmd.Parameters.AddWithValue("@BidderTypeDescription", bidderType.BidderTypeDescription);
            SqlParameter outputParam = new SqlParameter("@nID", SqlDbType.Int);
            outputParam.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(outputParam);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                using (_cn)
                {
                    _cn.Open();
                    ret = cmd.ExecuteNonQuery();
                    ret = Convert.ToInt32(outputParam.Value);
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ret = -1;
            }
            return ret;
        }
        public int InsertContract(Contract contract)
        {
            int ret = 0;
            _cn = new SqlConnection(Properties.Settings.Default["ThemisDatabase"].ToString());
            SqlCommand cmd = new SqlCommand("sp_InsertContract", _cn);
            cmd.Parameters.AddWithValue("@ContractName", contract.ContractName);
            cmd.Parameters.AddWithValue("@Terms", contract.ContractTerms);
            cmd.Parameters.AddWithValue("@ChangeOfScope", contract.ChangeOfScope);
            cmd.Parameters.AddWithValue("@Amount", contract.ContractAmount);
            cmd.Parameters.AddWithValue("@ChangeOrder", contract.ChangeOrder);
            cmd.Parameters.AddWithValue("@AdditionalAmount", contract.AdditionalAmount);
            cmd.Parameters.AddWithValue("@StaffAnalysis", contract.StaffAnalysis);
            cmd.Parameters.AddWithValue("@OrdinanceRequested", contract.OrdinanceRequested);
            SqlParameter outputParam = new SqlParameter("@nID", SqlDbType.Int);
            outputParam.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(outputParam);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                using (_cn)
                {
                    _cn.Open();
                    ret = cmd.ExecuteNonQuery();
                    ret = Convert.ToInt32(outputParam.Value);
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ret = -1;
            }
            return ret;
        }
        public int InsertContractBidder(ContractBidder contractBidder)
        {
            int ret = 0;
            _cn = new SqlConnection(Properties.Settings.Default["ThemisDatabase"].ToString());
            SqlCommand cmd = new SqlCommand("sp_InsertContractBidder", _cn);
            cmd.Parameters.AddWithValue("@ContractID", contractBidder.ContractID);
            cmd.Parameters.AddWithValue("@BidderID", contractBidder.BidderID);
            SqlParameter outputParam = new SqlParameter("@nID", SqlDbType.Int);
            outputParam.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(outputParam);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                using (_cn)
                {
                    _cn.Open();
                    ret = cmd.ExecuteNonQuery();
                    ret = Convert.ToInt32(outputParam.Value);
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ret = -1;
            }
            return ret;
        }
        public int InsertContractMaterial(ContractMaterial contractMaterial)
        {
            int ret = 0;
            _cn = new SqlConnection(Properties.Settings.Default["ThemisDatabase"].ToString());
            SqlCommand cmd = new SqlCommand("sp_InsertContractMaterial", _cn);
            cmd.Parameters.AddWithValue("@ContractID", contractMaterial.ContractID);
            cmd.Parameters.AddWithValue("@MaterialID", contractMaterial.MaterialID);
            SqlParameter outputParam = new SqlParameter("@nID", SqlDbType.Int);
            outputParam.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(outputParam);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                using (_cn)
                {
                    _cn.Open();
                    ret = cmd.ExecuteNonQuery();
                    ret = Convert.ToInt32(outputParam.Value);
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ret = -1;
            }
            return ret;
        }
        public int InsertContractOrdinance(ContractOrdinance contractOrdinance)
        {
            int ret = 0;
            _cn = new SqlConnection(Properties.Settings.Default["ThemisDatabase"].ToString());
            SqlCommand cmd = new SqlCommand("sp_InsertContractOrdinance", _cn);
            cmd.Parameters.AddWithValue("@ContractID", contractOrdinance.ContractID);
            cmd.Parameters.AddWithValue("@OrdinanceID", contractOrdinance.OrdinanceID);
            SqlParameter outputParam = new SqlParameter("@nID", SqlDbType.Int);
            outputParam.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(outputParam);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                using (_cn)
                {
                    _cn.Open();
                    ret = cmd.ExecuteNonQuery();
                    ret = Convert.ToInt32(outputParam.Value);
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ret = -1;
            }
            return ret;
        }
        public int InsertMaterial(Material material)
        {
            int ret = 0;
            _cn = new SqlConnection(Properties.Settings.Default["ThemisDatabase"].ToString());
            SqlCommand cmd = new SqlCommand("sp_InsertMaterial", _cn);
            cmd.Parameters.AddWithValue("@MaterialDescription", material.MaterialDescription);
            cmd.Parameters.AddWithValue("@SpecificationDocID", material.SpecDocID);
            SqlParameter outputParam = new SqlParameter("@nID", SqlDbType.Int);
            outputParam.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(outputParam);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                using (_cn)
                {
                    _cn.Open();
                    ret = cmd.ExecuteNonQuery();
                    ret = Convert.ToInt32(outputParam.Value);
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ret = -1;
            }
            return ret;
        }
        public int InsertMaterialSpecificationDocument(MaterialSpecificationDocument materialSpecificationDocument)
        {
            int ret = 0;
            _cn = new SqlConnection(Properties.Settings.Default["ThemisDatabase"].ToString());
            SqlCommand cmd = new SqlCommand("sp_InsertMaterialSpecificationDocument", _cn);
            cmd.Parameters.AddWithValue("@DocDescription", materialSpecificationDocument.DocDescription);
            cmd.Parameters.AddWithValue("@DocImage", materialSpecificationDocument.DocImage);
            SqlParameter outputParam = new SqlParameter("@nID", SqlDbType.Int);
            outputParam.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(outputParam);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                using (_cn)
                {
                    _cn.Open();
                    ret = cmd.ExecuteNonQuery();
                    ret = Convert.ToInt32(outputParam.Value);
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ret = -1;
            }
            return ret;
        }
        public int InsertMaterialType(MaterialType materialType)
        {
            int ret = 0;
            _cn = new SqlConnection(Properties.Settings.Default["ThemisDatabase"].ToString());
            SqlCommand cmd = new SqlCommand("sp_InsertlkMaterialType", _cn);
            cmd.Parameters.AddWithValue("@MaterialTypeDescription", materialType.MaterialTypeDescription);
            SqlParameter outputParam = new SqlParameter("@nID", SqlDbType.Int);
            outputParam.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(outputParam);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                using (_cn)
                {
                    _cn.Open();
                    ret = cmd.ExecuteNonQuery();
                    ret = Convert.ToInt32(outputParam.Value);
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ret = -1;
            }
            return ret;
        }
        public int InsertOrdinance(Ordinance ordinance)
        {
            int ret = 0;
            _cn = new SqlConnection(Properties.Settings.Default["ThemisDatabase"].ToString());
            SqlCommand cmd = new SqlCommand("sp_InsertOrdinance", _cn);
            cmd.Parameters.AddWithValue("@FormNumber", ordinance.FormNumber);
            SqlParameter outputParam = new SqlParameter("@nID", SqlDbType.Int);
            outputParam.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(outputParam);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                using (_cn)
                {
                    _cn.Open();
                    ret = cmd.ExecuteNonQuery();
                    ret = Convert.ToInt32(outputParam.Value);
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ret = -1;
            }
            return ret;
        }
        public int InsertPurchase(Purchase purchase)
        {
            int ret = 0;
            _cn = new SqlConnection(Properties.Settings.Default["ThemisDatabase"].ToString());
            SqlCommand cmd = new SqlCommand("sp_InsertPurchase", _cn);
            cmd.Parameters.AddWithValue("@PurchaseName", purchase.PurchaseName);
            cmd.Parameters.AddWithValue("@ContractID", purchase.ContractID);
            cmd.Parameters.AddWithValue("@PurchaseAgentRequired", purchase.PurchaseAgentRequired);
            cmd.Parameters.AddWithValue("@PurchaseAgentApproval", purchase.PurchaseAgentApproval);
            cmd.Parameters.AddWithValue("@AgentApprovalID", purchase.AgentApprovalID);
            cmd.Parameters.AddWithValue("@PurchaseTypeID", purchase.PurchaseTypeID);
            SqlParameter outputParam = new SqlParameter("@nID", SqlDbType.Int);
            outputParam.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(outputParam);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                using (_cn)
                {
                    _cn.Open();
                    ret = cmd.ExecuteNonQuery();
                    ret = Convert.ToInt32(outputParam.Value);
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ret = -1;
            }
            return ret;
        }
        public int InsertPurchaseAccounting(PurchaseAccounting purchaseAccounting)
        {
            int ret = 0;
            _cn = new SqlConnection(Properties.Settings.Default["ThemisDatabase"].ToString());
            SqlCommand cmd = new SqlCommand("sp_InsertPurchaseAccounting", _cn);
            cmd.Parameters.AddWithValue("@PurchaseID", purchaseAccounting.PurchaseID);
            cmd.Parameters.AddWithValue("@AccountingID", purchaseAccounting.AccountingID);
            SqlParameter outputParam = new SqlParameter("@nID", SqlDbType.Int);
            outputParam.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(outputParam);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                using (_cn)
                {
                    _cn.Open();
                    ret = cmd.ExecuteNonQuery();
                    ret = Convert.ToInt32(outputParam.Value);
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ret = -1;
            }
            return ret;
        }
        public int InsertPurchaseAgentApproval(PurchaseAgentApproval purchaseAgentApproval)
        {
            int ret = 0;
            _cn = new SqlConnection(Properties.Settings.Default["ThemisDatabase"].ToString());
            SqlCommand cmd = new SqlCommand("sp_InsertPurchaseAgentApproval", _cn);
            cmd.Parameters.AddWithValue("@ApprovalName", purchaseAgentApproval.ApprovalName);
            cmd.Parameters.AddWithValue("@ApprovalImage", purchaseAgentApproval.ApprovalImage);
            SqlParameter outputParam = new SqlParameter("@nID", SqlDbType.Int);
            outputParam.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(outputParam);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                using (_cn)
                {
                    _cn.Open();
                    ret = cmd.ExecuteNonQuery();
                    ret = Convert.ToInt32(outputParam.Value);
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ret = -1;
            }
            return ret;
        }
        public int InsertPurchaseType(PurchaseType purchaseType)
        {
            int ret = 0;
            _cn = new SqlConnection(Properties.Settings.Default["ThemisDatabase"].ToString());
            SqlCommand cmd = new SqlCommand("sp_InsertlkPurchaseType", _cn);
            cmd.Parameters.AddWithValue("@PurchaseDescription", purchaseType.PurchaseTypeDescription);
            SqlParameter outputParam = new SqlParameter("@nID", SqlDbType.Int);
            outputParam.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(outputParam);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                using (_cn)
                {
                    _cn.Open();
                    ret = cmd.ExecuteNonQuery();
                    ret = Convert.ToInt32(outputParam.Value);
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ret = -1;
            }
            return ret;
        }
        public int InsertRequest(Request request)
        {
            int ret = 0;
            _cn = new SqlConnection(Properties.Settings.Default["ThemisDatabase"].ToString());
            SqlCommand cmd = new SqlCommand("sp_InsertRequest", _cn);
            cmd.Parameters.AddWithValue("@OfficeName", request.OfficeName);
            cmd.Parameters.AddWithValue("@UserID", request.UserID);
            cmd.Parameters.AddWithValue("@DateFirstReading", request.DateFirstReading);
            cmd.Parameters.AddWithValue("@EmergencyPassage", request.EmergencyPassage);
            cmd.Parameters.AddWithValue("@SuggestedTitle", request.SuggestedTitle);
            SqlParameter outputParam = new SqlParameter("@nID", SqlDbType.Int);
            outputParam.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(outputParam);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                using (_cn)
                {
                    _cn.Open();
                    ret = cmd.ExecuteNonQuery();
                    ret = Convert.ToInt32(outputParam.Value);
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ret = -1;
            }
            return ret;
        }
        public int InsertRequestContract(RequestContract requestContract)
        {
            int ret = 0;
            _cn = new SqlConnection(Properties.Settings.Default["ThemisDatabase"].ToString());
            SqlCommand cmd = new SqlCommand("sp_InsertRequestContract", _cn);
            cmd.Parameters.AddWithValue("@RequestID", requestContract.RequestID);
            cmd.Parameters.AddWithValue("@ContractID", requestContract.ContractID);
            SqlParameter outputParam = new SqlParameter("@nID", SqlDbType.Int);
            outputParam.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(outputParam);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                using (_cn)
                {
                    _cn.Open();
                    ret = cmd.ExecuteNonQuery();
                    ret = Convert.ToInt32(outputParam.Value);
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ret = -1;
            }
            return ret;
        }
        public int InsertSpecificationDocument(SpecificationDocument specificationDocument)
        {
            int ret = 0;
            _cn = new SqlConnection(Properties.Settings.Default["ThemisDatabase"].ToString());
            SqlCommand cmd = new SqlCommand("sp_InsertSpecificationDocument", _cn);
            cmd.Parameters.AddWithValue("@DocDescription", specificationDocument.DocDescription);
            cmd.Parameters.AddWithValue("@DocImage", specificationDocument.DocImage);
            SqlParameter outputParam = new SqlParameter("@nID", SqlDbType.Int);
            outputParam.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(outputParam);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                using (_cn)
                {
                    _cn.Open();
                    ret = cmd.ExecuteNonQuery();
                    ret = Convert.ToInt32(outputParam.Value);
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ret = -1;
            }
            return ret;
        }
        public int InsertVendor(Vendor vendor)
        {
            int ret = 0;
            _cn = new SqlConnection(Properties.Settings.Default["ThemisDatabase"].ToString());
            SqlCommand cmd = new SqlCommand("sp_InsertlkVendor", _cn);
            cmd.Parameters.AddWithValue("@VendorName", vendor.VendorName);
            cmd.Parameters.AddWithValue("@VendorDescription", vendor.VendorDescription);
            cmd.Parameters.AddWithValue("@VendorNumber", vendor.VendorNumber);
            SqlParameter outputParam = new SqlParameter("@nID", SqlDbType.Int);
            outputParam.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(outputParam);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                using (_cn)
                {
                    _cn.Open();
                    ret = cmd.ExecuteNonQuery();
                    ret = Convert.ToInt32(outputParam.Value);
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ret = -1;
            }
            return ret;
        }
    }
}