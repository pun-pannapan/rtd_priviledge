using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using rtd_priviledge.Models;
using rtd_priviledge.Service;
using rtd_priviledge.Services;
using rtd_priviledge.Utilities;
using System.Data.SqlClient;

namespace rtd_priviledge.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private string _connectionString;
        private SqlConnection _dbCon;

        public AuthController(ILogger<HomeController> logger, IOptions<ConnectionString> connectionString)
        {
            _logger = logger;
            _connectionString = connectionString.Value.DatabaseConnection;
            _dbCon = new SqlConnection(_connectionString);
        }

        [HttpPost("requestOTPByTel")]
        public IActionResult requestOTPByTel(TelNoModel telNoModel)
        {
            string methodName = "requestOTPByTel";
            try
            {
                _logger.LogInformation($"{methodName} req[{ConvertUtil.obj2string(telNoModel)}]");
                ConnectionHandle.openConnection(_dbCon);

                SMSService otp = new SMSService(_logger);
                OTPResponseModel resp = otp.requestOTPByTel(_dbCon, telNoModel.telNo);

                return Ok(resp);
            }
            catch (Exception ex)
            {
                ConnectionHandle.closeConnection(_dbCon);
                _logger.LogError($"{methodName} error: " + ConvertUtil.obj2string(ex));
                return BadRequest(ex.Message);
            }
            finally
            {
                ConnectionHandle.closeConnection(_dbCon);
            }
        }

        [HttpGet("checkValidOTP")]
        public IActionResult checkValidOTP(string telNo, string otpCode)
        {
            string methodName = "checkValidOTP";
            try
            {
                _logger.LogInformation($"{methodName} [custTel: {telNo}, otpCode: {otpCode}]");

                ConnectionHandle.openConnection(_dbCon);
                var serv = new TransactionService();

                CheckOTP_ResponseResult resp = serv.checkValidOTP(_dbCon, telNo, otpCode);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                ConnectionHandle.closeConnection(_dbCon);
                _logger.LogError($"{methodName} error: " + ConvertUtil.obj2string(ex));
                return BadRequest(ex.Message);
            }
            finally
            {
                ConnectionHandle.closeConnection(_dbCon);
            }
        }
    }
}
