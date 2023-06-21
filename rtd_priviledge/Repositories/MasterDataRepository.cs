using Dapper;
using rtd_priviledge.Models.Dtos;
using System.Data.SqlClient;
using System.Transactions;

namespace rtd_priviledge.Repositories
{
    public class MasterDataRepository
    {
        public List<m_addrprovince> getProvince(SqlConnection dbCon)
        {
            const string sql = "select * from [dbo].[m_addrprovince]  order by provId";

            return dbCon.Query<m_addrprovince>(sql).ToList();
        }
        public m_addrprovince getProvinceByProvCode(SqlConnection dbCon, int provCode)
        {
            const string sql = @" select * from [dbo].[m_addrprovince]
                                   where [provCode] = @provCode ";

            return dbCon.QuerySingleOrDefault<m_addrprovince>(sql, new
            {
                provCode = provCode
            });
        }
        public List<m_addrdistrict> getDistByProvID(SqlConnection dbCon, long province_id)
        {
            const string sql = "select * from [dbo].[m_addrdistrict] where province_id = @province_id order by distId";

            return dbCon.Query<m_addrdistrict>(sql, new { province_id = province_id }).ToList();
        }
        public List<m_addrsubdistrict> getSubDistByDistId(SqlConnection dbCon, long subDistDistId)
        {
            //const string sql = "select subDistId,subDistZipCode,subDistNameTh + ' [' + subDistZipCode+ ']' as subDistNameTh,subDistNameEn,subDistDistId  from [dbo].[m_addrsubdistrict] where subDistDistId = @subDistDistId order by subDistId";
            const string sql = "select subDistId,subDistZipCode,subDistNameTh as subDistNameTh,subDistNameEn,subDistDistId  from [dbo].[m_addrsubdistrict] where subDistDistId = @subDistDistId order by subDistId";
            return dbCon.Query<m_addrsubdistrict>(sql, new { subDistDistId = subDistDistId }).ToList();
        }

        public List<m_addrsubdistrict> getZipCodeBySubDistId(SqlConnection dbCon, long subDistId)
        {            
            const string sql = "select subDistId,subDistZipCode,subDistNameTh as subDistNameTh,subDistNameEn,subDistDistId  from [dbo].[m_addrsubdistrict] where subDistId = @subDistId";
            return dbCon.Query<m_addrsubdistrict>(sql, new { subDistId = subDistId }).ToList();
        }

        public List<m_shoptype> getShopType(SqlConnection dbCon)
        {
            const string sql = "select * from [dbo].[m_shopType]";

            return dbCon.Query<m_shoptype>(sql).ToList();
        }

        public m_shoptype getShopTypeById(SqlConnection dbCon, string shopTypeCode)
        {
            const string sql = "select * from [dbo].[m_shopType] where [shopTypeCode] = @shopTypeCode ";

            return dbCon.QuerySingleOrDefault<m_shoptype>(sql, new
            {
                shopTypeCode = shopTypeCode
            });
        }

        public m_shopcode getShopByShopCode(SqlConnection dbCon, string shopCode)
        {
            const string sql = @" select * from [dbo].[m_shopCode] 
                                   where [shopCode] = @shopCode";

            return dbCon.QuerySingleOrDefault<m_shopcode>(sql, new
            {
                shopCode = shopCode
            });
        }

        public void DisableShopCodeByShopCode(SqlConnection dbCon, SqlTransaction transaction, string shopCode)
        {
            dbCon.Execute(@"UPDATE [dbo].[m_shopCode]
                               SET [isRegister] = 1
                             WHERE [shopCode] = @shopCode", new { shopCode = shopCode }, transaction);
        }
    }
}
