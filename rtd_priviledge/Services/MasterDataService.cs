using rtd_priviledge.Models.Dtos;
using rtd_priviledge.Repositories;
using System.Data.SqlClient;

namespace rtd_priviledge.Services
{
    public class MasterDataService
    {
        public MasterDataService()
        {
            
        }

        public List<m_addrprovince> getProv(SqlConnection dbCon)
        {
            try
            {
                MasterDataRepository repo = new MasterDataRepository();
                List<m_addrprovince> lstData = repo.getProvince(dbCon);
                return lstData;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        public m_addrprovince getProvByProvCode(SqlConnection dbCon, int provCode)
        {
            try
            {
                MasterDataRepository repo = new MasterDataRepository();
                m_addrprovince data = repo.getProvinceByProvCode(dbCon, provCode);

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        public List<m_addrdistrict> getDistByProvCode(SqlConnection dbCon, long provId)
        {
            try
            {
                MasterDataRepository repo = new MasterDataRepository();
                List<m_addrdistrict> lstData = repo.getDistByProvID(dbCon, provId);

                return lstData;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        public List<m_addrsubdistrict> getSubDistByDistCode(SqlConnection dbCon, long distId)
        {
            try
            {
                MasterDataRepository repo = new MasterDataRepository();
                List<m_addrsubdistrict> lstData = repo.getSubDistByDistId(dbCon, distId);

                return lstData;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        public List<m_addrsubdistrict> getZipCodeBySubDistId(SqlConnection dbCon, long subDistId)
        {
            try
            {
                MasterDataRepository repo = new MasterDataRepository();
                List<m_addrsubdistrict> lstData = repo.getZipCodeBySubDistId(dbCon, subDistId);

                return lstData;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        public List<m_shoptype> getShopType(SqlConnection dbCon)
        {
            try
            {
                MasterDataRepository repo = new MasterDataRepository();
                List<m_shoptype> lstData = repo.getShopType(dbCon);

                return lstData;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        public m_shoptype getShopTypeById(SqlConnection dbCon, string shopTypeCode)
        {
            try
            {
                MasterDataRepository repo = new MasterDataRepository();
                m_shoptype data = repo.getShopTypeById(dbCon, shopTypeCode);

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        public m_shopcode getShopByShopCode(SqlConnection dbCon, string shopCode)
        {
            try
            {
                MasterDataRepository repo = new MasterDataRepository();
                m_shopcode data = repo.getShopByShopCode(dbCon, shopCode);
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        public void DisableShopCodeByShopCode(SqlConnection dbCon,SqlTransaction transaction, string shopCode)
        {
            try
            {
                MasterDataRepository repo = new MasterDataRepository();
                repo.DisableShopCodeByShopCode(dbCon, transaction, shopCode);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
    }
}
