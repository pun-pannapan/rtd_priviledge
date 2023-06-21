using Dapper;
using rtd_priviledge.Models;
using rtd_priviledge.Models.Dtos;
using System.Data.SqlClient;

namespace rtd_priviledge.Repositories
{
    public class TransactionRepository
    {
        public List<m_addrprovince> getCustRegister(SqlConnection dbCon)
        {
            const string sql = "select * from [dbo].[t_custRegister]";

            return dbCon.Query<m_addrprovince>(sql).ToList();
        }
        public t_custregister findCustRegisterByGuid(SqlConnection dbCon, Guid regGuid)
        {
            const string sql = @"select * 
                                 from [dbo].[t_custRegister]
                                 where regGuid = @regGuid";
            return dbCon.QuerySingleOrDefault<t_custregister>(sql, new
            {
                regGuid = regGuid
            });
        }
        public void CustRegisterInsert(SqlConnection dbCon, SqlTransaction transaction, t_custregister dataDB)
        {
            dbCon.Execute(@"INSERT INTO [dbo].[t_custRegister]
                                   ([regGuid]
                                   ,[shopCode]
                                   ,[regDate]
                                   ,[mktConsentAccept]
                                   ,[regPhoneNumber])
                             VALUES
                                   (NEWID()
                                   , @shopCode
                                   , @regDate
                                   , @mktConsentAccept
                                   , @regPhoneNumber
                                    )",
                dataDB, transaction);
        }
        public void CustRegisterUpdate(SqlConnection dbCon, SqlTransaction transaction, t_custregister dataDB)
        {
            dbCon.Execute(@"UPDATE [dbo].[t_custRegister]
                               SET[shopCode] = @shopCode
                                  ,[regDate] = @regDate
                                  ,[mktConsentAccept] = @mktConsentAccept
                                  ,[regPhoneNumber] = @regPhoneNumber
                             WHERE regGuid = @regGuid",
                 dataDB, transaction);
        }

        public void CustAddAddress(SqlConnection dbCon, SqlTransaction transaction, t_custregister dataDB)
        {
            dbCon.Execute(@"UPDATE [dbo].[t_custRegister]
                               SET [delivery_name] = @delivery_name
                                    ,[delivery_lastname] = @delivery_lastname
                                    ,[delivery_phonenumber] = @delivery_phonenumber
                                    ,[delivery_address1] = @delivery_address1
                                    ,[delivery_prov_id] = @delivery_prov_id
                                    ,[delivery_dist_id] = @delivery_dist_id
                                    ,[delivery_subdist_id] = @delivery_subdist_id
                                    ,[delivery_confirm_address] = @delivery_confirm_address
                                    ,[AddAddressDate] = @AddAddressDate
                             WHERE regGuid = @regGuid",
                 dataDB, transaction);
        }

        public t_custregister getTransactionRegisterByShopCodeAndTel(SqlConnection dbCon, string shopCode, string regPhoneNumber)
        {
            const string sql = @" select * from [dbo].[t_custRegister] 
                                   where [shopCode] = @shopCode 
                                    and [regPhoneNumber] = @regPhoneNumber";

            return dbCon.QuerySingleOrDefault<t_custregister>(sql, new
            {
                shopCode = shopCode,
                regPhoneNumber = regPhoneNumber
            });
        }

        public CustRegisterShopCode getTransactionRegisterAndShopByTel(SqlConnection dbCon, string regPhoneNumber)
        {
            const string sql = @"select tcr.*,tsc.shopName 
                                  from [t_custRegister] tcr
                                  left join [m_shopCode] tsc on tcr.shopCode = tsc.shopCode
                                  where [regPhoneNumber] = @regPhoneNumber";

            return dbCon.QuerySingleOrDefault<CustRegisterShopCode>(sql, new
            {
                regPhoneNumber = regPhoneNumber
            });
        }
    }
}
