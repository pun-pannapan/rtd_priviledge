using System.Data.SqlClient;
using Dapper;
using rtd_priviledge.Models.Dtos;

namespace rtd_priviledge.Repositories
{
    public class AuthenRepository
    {
        public void insertOTP(SqlConnection dbCon, t_otpCode dtoOTP)
        {
            using (var transaction = dbCon.BeginTransaction())
            {
                try
                {
                    dbCon.Execute(" DELETE FROM [dbo].[T_OTPCode] WHERE [otpTel] = @otpTel"
                      , new
                      {
                          otpTel = dtoOTP.otpTel
                      }, transaction);


                    dbCon.Execute("INSERT INTO [dbo].[t_otpCode] " +
                       " ([otpRef]" +
                       ",[otpTel]" +
                       ",[otpCode]" +
                       ",[otpExpire]" +
                       ",[otpCreateDate]) " +
                       "VALUES " +
                       "(@otpRef" +
                       ", @otpTel" +
                       ", @otpCode" +
                       ", @otpExpire" +
                       ", @otpCreateDate)"
                    , new
                    {
                        otpRef = dtoOTP.otpRef,
                        otpTel = dtoOTP.otpTel,
                        otpCode = dtoOTP.otpCode,
                        otpExpire = dtoOTP.otpExpire,
                        otpCreateDate = dtoOTP.otpCreateDate
                    }, transaction);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }
        public t_otpCode checkValidOTP(SqlConnection dbCon, string otpTel, string otpCode)
        {
            const string sql = @"SELECT * FROM [t_otpCode] 
                    WHERE [otpCode] = @otpCode 
                    and [otpTel] = @otpTel ";

            return dbCon.QuerySingleOrDefault<t_otpCode>(sql, new
            {
                otpCode = otpCode,
                otpTel = otpTel
            });
        }
    }
}
