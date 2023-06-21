using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using rtd_priviledge.Models;
using rtd_priviledge.Models.Dtos;
using rtd_priviledge.Models;
using rtd_priviledge.Services;
using rtd_priviledge.Utilities;
using System.Data.SqlClient;
using System.Diagnostics;
using rtd_priviledge.Service;

namespace rtd_priviledge.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private string _connectionString;
        private SqlConnection _dbCon;
        private string _showMessage;
        private string _zipCode;

        public HomeController(ILogger<HomeController> logger, IOptions<ConnectionString> connectionString)
        {
            _logger = logger;
            _connectionString = connectionString.Value.DatabaseConnection;
            _dbCon = new SqlConnection(_connectionString);
            _showMessage = "ไม่พบรหัสร้านของคุณในระบบ";
        }

        //[HttpGet]
        //public IActionResult Login()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public IActionResult Login(CustRegisterViewModel CusLogin)
        //{
        //    if (!ModelState.IsValid)
        //        return View();
                        
        //    try
        //    {
        //        _logger.LogInformation("Post Login");
        //        var shopCodeString = CusLogin.shopCode.Trim();
        //        var phoneNumberString = CusLogin.regPhoneNumber.Trim();
        //        ConnectionHandle.openConnection(_dbCon);
        //        TransactionService transactionServ = new TransactionService();
        //        var custRegister = transactionServ.getTransactionRegisterByShopCodeAndTel(_dbCon, shopCodeString, phoneNumberString);

        //        if (custRegister != null)
        //        {
        //            MasterDataService masterDataServ = new MasterDataService();
        //            m_shopcode shopCode = masterDataServ.getShopByShopCode(_dbCon, shopCodeString);

        //            var shopData = getShopDataByShopCode(shopCode);
        //            return RedirectToAction("Priviledge", shopData);
        //        }

        //        return RedirectToAction("NoShopCode");
        //    }
        //    catch (Exception ex)
        //    {
        //        ConnectionHandle.closeConnection(_dbCon);
        //        _logger.LogError("Post Register error: " + ConvertUtil.obj2string(ex));
        //        return RedirectToAction("NoShopCode");
        //    }
        //    finally
        //    {
        //        ConnectionHandle.closeConnection(_dbCon);
        //    }
        //}

        [HttpGet]
        public IActionResult Register() 
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(CustRegisterViewModel CustRegister)
        {
            if (!ModelState.IsValid)
                return View();

            try
            {                
                _logger.LogInformation("Post Register");

                //login first if cannot login do register
                var shopCodeString = CustRegister.shopCode.Trim();
                var phoneNumberString = CustRegister.regPhoneNumber.Trim();
                ConnectionHandle.openConnection(_dbCon);
                MasterDataService masterDataServ = new MasterDataService();
                TransactionService transactionServ = new TransactionService();
                var custRegister = transactionServ.getTransactionRegisterByShopCodeAndTel(_dbCon, shopCodeString, phoneNumberString);
                m_shopcode shopCode = new m_shopcode();
                shopCode = masterDataServ.getShopByShopCode(_dbCon, shopCodeString);
                if (custRegister != null)
                {                    
                    var shopData = getShopDataByShopCode(shopCode, custRegister);
                    //bypass otp for test
                    //return RedirectToAction("Priviledge", shopData);
                    return RedirectToAction("ValidateOTP", shopData);
                }

                //do register by insert t_custRegister
                //update DisableShopCodeByShopCode
                var showMessageViewModel = new ShowMessageViewModel();
                if (shopCode != null && shopCode.isRegister == false)
                {
                    var chkIsRegisterAnotherShop = transactionServ.getTransactionRegisterAndShopByTel(_dbCon, phoneNumberString);

                    if (chkIsRegisterAnotherShop != null)
                    {
                        showMessageViewModel.showMessage = string.Format("เบอร์นี้ได้ลงทะเบียนแล้วที่ร้าน \"{0}\"", chkIsRegisterAnotherShop.shopName);
                        return RedirectToAction("NoShopCode", showMessageViewModel);
                    }

                    var customerRegister = getCustomerRegister(CustRegister);
                    using (var transaction = _dbCon.BeginTransaction())
                    {                        
                        transactionServ.CustRegisterInsert(_dbCon, transaction, customerRegister);
                        masterDataServ.DisableShopCodeByShopCode(_dbCon, transaction, shopCodeString);
                        transaction.Commit();
                    }
                    var shopData = getShopDataByShopCode(shopCode, customerRegister);
                    //return RedirectToAction("Priviledge", shopData);
                    return RedirectToAction("ValidateOTP", shopData);
                }

                if (shopCode != null && shopCode.isRegister == true)
                {
                    showMessageViewModel.showMessage = "รหัสร้านค้านี้ลงทะเบียนแล้ว <br />กรุณาลองใหม่อีกครั้ง";
                    return RedirectToAction("NoShopCode", showMessageViewModel);
                }

                showMessageViewModel.showMessage = _showMessage;
                return RedirectToAction("NoShopCode", showMessageViewModel);
            }
            catch (Exception ex)
            {
                ConnectionHandle.closeConnection(_dbCon);
                _logger.LogError("Post Register error: " + ConvertUtil.obj2string(ex));
                return RedirectToAction("ShowError");
            }
            finally
            {
                ConnectionHandle.closeConnection(_dbCon);
            }
        }

        public IActionResult Priviledge(ShopViewModel shopData) 
        {
            if (shopData.shopCodeGuid == Guid.Empty)
                return RedirectToAction("Register");

            return View(shopData);
        }

        public IActionResult ValidateOTP(ShopViewModel shopData)
        {
            try
            {
                ConnectionHandle.openConnection(_dbCon);
                var smsService = new SMSService(_logger);
                shopData.otpResponseModel = smsService.requestOTPByTel(_dbCon, shopData.regPhoneNumber);
            }
            catch (Exception ex)
            {
                ConnectionHandle.closeConnection(_dbCon);
                _logger.LogError("ValidateOTP error: " + ConvertUtil.obj2string(ex));
                return RedirectToAction("ShowError");
            }
            finally
            {
                ConnectionHandle.closeConnection(_dbCon);
            }
            return View(shopData);
        }

        public IActionResult NoShopCode(ShowMessageViewModel showMessageViewModel)
        {
            return View(showMessageViewModel);
        }

        public IActionResult AlreadyRegister(ShowMessageViewModel showMessageViewModel)
        {
            return View(showMessageViewModel);
        }

        [HttpGet]
        public IActionResult AddAddress(string shopCode, string regPhoneNumber)
        {
            var custRegisterViewModel = new CustRegisterViewModel();
            ConnectionHandle.openConnection(_dbCon);
            custRegisterViewModel.shopCode = shopCode;
            custRegisterViewModel.regPhoneNumber = regPhoneNumber;
            custRegisterViewModel.selectProvinces = getSelectListProvince();
            custRegisterViewModel.selectDistricts = getSelectListDistrict(0);
            custRegisterViewModel.selectSubDistricts = getSelectListSubDistrict(0);
            custRegisterViewModel.deliveryZipcode = _zipCode;
            return View(custRegisterViewModel);
        }

        [HttpPost]
        public IActionResult AddAddress(CustRegisterViewModel custRegisterViewModel)
        {
            var transactionService = new TransactionService();
            var shopCodeString = custRegisterViewModel.shopCode?.Trim();
            var phoneNumberString = custRegisterViewModel.regPhoneNumber?.Trim();
            try
            {
                var custRegister = transactionService.getTransactionRegisterByShopCodeAndTel(_dbCon, shopCodeString, phoneNumberString);
                ConnectionHandle.openConnection(_dbCon);
                using (var transaction = _dbCon.BeginTransaction())
                {
                    var custRegisterAddress = getCustRegisterAddress(custRegisterViewModel, custRegister.regGuid);
                    transactionService.CustRegisterAddAddress(_dbCon, transaction, custRegisterAddress);
                    transaction.Commit();
                }
                MasterDataService masterDataServ = new MasterDataService();
                m_shopcode shopCode = new m_shopcode();
                shopCode = masterDataServ.getShopByShopCode(_dbCon, shopCodeString);

                var shopData = getShopDataByShopCode(shopCode, custRegisterViewModel);
                return RedirectToAction("Priviledge", shopData);
            }
            catch (Exception ex)
            {
                ConnectionHandle.closeConnection(_dbCon);
                _logger.LogError("AddAddress error: " + ConvertUtil.obj2string(ex));
                return RedirectToAction("ShowError");
            }
            finally
            {
                ConnectionHandle.closeConnection(_dbCon);
            }
        }

        private t_custregister getCustRegisterAddress(CustRegisterViewModel custRegisterViewModel, Guid regGuid)
        {
            return new t_custregister()
            {
                AddAddressDate = DateTime.Now,
                delivery_name = custRegisterViewModel.delivery_name,
                delivery_lastname = custRegisterViewModel.delivery_lastname,
                delivery_phonenumber = custRegisterViewModel.delivery_phonenumber,
                delivery_address1 = custRegisterViewModel.delivery_address1,
                delivery_prov_id = custRegisterViewModel.delivery_prov_id,
                delivery_dist_id = custRegisterViewModel.delivery_dist_id,
                delivery_subdist_id = custRegisterViewModel.delivery_subdist_id,
                delivery_confirm_address = custRegisterViewModel.delivery_confirm_address,
                regGuid = regGuid
            };
        }

        [HttpPost]
        public JsonResult setDropDrownList(string type, int value)
        {
            var custRegisterViewModel = new CustRegisterViewModel();
            switch (type)
            {
                case "delivery_prov_id":
                    custRegisterViewModel.selectDistricts = getSelectListDistrict(value);
                    break;
                case "delivery_dist_id":
                    custRegisterViewModel.selectSubDistricts = getSelectListSubDistrict(value);
                    break;
                case "delivery_subdist_id":
                    custRegisterViewModel.deliveryZipcode = getZipCodeBySubdistrictId(value);
                    break;
            }
            return Json(custRegisterViewModel);
        }

        public IActionResult ShowError() 
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private ShopViewModel getShopDataByShopCode(m_shopcode shopCode, t_custregister custRegister)
        {
            var shopData = new ShopViewModel()
            {
                regGuid = custRegister.regGuid,
                shopCodeGuid = shopCode.shopCodeGuid,
                shopName = shopCode.shopName,
                shopCode = shopCode.shopCode,
                provinceName = "",
                shopTypeName = "",
                regPhoneNumber = custRegister.regPhoneNumber,
                deliveryConfirmAddress = false
            };

            MasterDataService masterDataServ = new MasterDataService();
            var shopType = masterDataServ.getShopTypeById(_dbCon, shopCode.shopTypeCode.Trim());
            shopData.shopTypeName = shopType != null ? shopType.shopTypeDisplayName : "";
            shopData.shopTypeShortCode = shopType != null ? shopType.shopTypeName + ".png" : "RTD.png";
            var province = masterDataServ.getProvByProvCode(_dbCon, shopCode.provId);
            shopData.provinceName = province.provNameTh;
            shopData.deliveryConfirmAddress = custRegister.delivery_confirm_address;
            return shopData;
        }

        private t_custregister getCustomerRegister(CustRegisterViewModel custregister)
        {
            return new t_custregister()
            {
                shopCode = custregister.shopCode,
                regDate = DateTime.Now,
                mktConsentAccept = custregister.mktConsentAccept,
                regPhoneNumber = custregister.regPhoneNumber,
                delivery_confirm_address = custregister.delivery_confirm_address
            };
        }

        private SelectList getSelectListProvince()
        {
            var masterDataService = new MasterDataService();
            var selectListProvince = new SelectList(masterDataService.getProv(_dbCon), "provId", "provNameTh");

            return selectListProvince;
        }

        private SelectList getSelectListDistrict(long provId)
        {
            var masterDataService = new MasterDataService();
            var selectListDistrict = new SelectList(masterDataService.getDistByProvCode(_dbCon, provId), "distId", "name_th");

            return selectListDistrict;
        }

        private SelectList getSelectListSubDistrict(long distId)
        {
            var masterDataService = new MasterDataService();
            var subDistrictList = masterDataService.getSubDistByDistCode(_dbCon, distId);
            //_zipCode = subDistrictList.FirstOrDefault()?.subDistZipCode ?? "";
            var selectListDistrict = new SelectList(subDistrictList, "subDistId", "subDistNameTh");

            return selectListDistrict;
        }

        private string getZipCodeBySubdistrictId(long subDistId)
        {
            var masterDataService = new MasterDataService();
            var subDistrictList = masterDataService.getZipCodeBySubDistId(_dbCon, subDistId);
            var zipCode = subDistrictList.FirstOrDefault()?.subDistZipCode ?? "";
            return zipCode;
        }
    }
}