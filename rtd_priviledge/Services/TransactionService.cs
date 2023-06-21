using rtd_priviledge.Models;
using rtd_priviledge.Models.Dtos;
using rtd_priviledge.Repositories;
using System.Data.SqlClient;

namespace rtd_priviledge.Services
{
    public class TransactionService
    {
        public void CustRegisterInsert(SqlConnection dbCon, SqlTransaction transaction, t_custregister custregister)
        {
            try
            {
                TransactionRepository repo = new TransactionRepository();
                repo.CustRegisterInsert(dbCon, transaction, custregister);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        public void CustRegisterAddAddress(SqlConnection dbCon, SqlTransaction transaction, t_custregister custregister)
        {
            try
            {
                TransactionRepository repo = new TransactionRepository();
                repo.CustAddAddress(dbCon, transaction, custregister);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        public t_custregister getTransactionRegisterByShopCodeAndTel(SqlConnection dbCon, string shopCode, string phoneNumber)
        {
            try
            {
                TransactionRepository repo = new TransactionRepository();
                return repo.getTransactionRegisterByShopCodeAndTel(dbCon, shopCode, phoneNumber);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
                
        public CustRegisterShopCode getTransactionRegisterAndShopByTel(SqlConnection dbCon, string phoneNumber)
        {
            try
            {
                TransactionRepository repo = new TransactionRepository();
                return repo.getTransactionRegisterAndShopByTel(dbCon, phoneNumber);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        public CheckOTP_ResponseResult checkValidOTP(SqlConnection dbCon, string tel, string otp)
        {
            try
            {
                validateCheckValidOTP(tel.Trim(), otp.Trim());
                AuthenRepository authenRepo = new AuthenRepository();
                t_otpCode result = authenRepo.checkValidOTP(dbCon, tel.Trim(), otp.Trim());

                CheckOTP_ResponseResult resp = new CheckOTP_ResponseResult();
                if (result == null)
                {
                    resp.isValid = false;
                    resp.resultDesc = "ขออภัย รหัสของท่านไม่ถูกต้อง กรุณาทำรายการอีกครั้ง";
                }
                else if (result.otpExpire < DateTime.Now)
                {
                    resp.isValid = false;
                    resp.resultDesc = "ขออภัย รหัสของท่านหมดอายุแล้ว กรุณาทำรายการอีกครั้ง";
                }
                else
                {
                    resp.isTelExistedWhenInvalid = true;
                    resp.isValid = true;
                    resp.resultDesc = "";
                }

                return resp;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        private void validateCheckValidOTP(string tel, string otp)
        {
            if (otp == null || otp.Trim().Equals(""))
            {
                throw new Exception("OTP is empty.");
            }
            if (tel == null || tel.Trim().Equals("") || tel.Trim().Equals("null"))
            {
                throw new Exception("Telephone number is required.");
            }
        }
    }
}
