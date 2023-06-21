using rtd_priviledge.Constants;
using rtd_priviledge.Repositories;
using RestSharp;
using RestSharp.Authenticators;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using rtd_priviledge.Models;
using rtd_priviledge.Models.Dtos;

namespace rtd_priviledge.Service
{
    public class SMSService
    {
       
        private readonly ILogger _logger;
        public SMSService(ILogger logger)
        {
            _logger = logger;
        }

        private string OTPCharacters()
        {
            string OTPLength = "4";

            string NewCharacters = "";

            //This one tells you which characters are allowed in this new password
            string allowedChars = "";
            //Here Specify your OTP Characters
            allowedChars = "1,2,3,4,5,6,7,8,9,0";

            char[] sep = { ',' };
            string[] arr = allowedChars.Split(sep);

            string IDString = "";
            string temp = "";

            //utilize the "random" class
            Random rand = new Random();

            for (int i = 0; i < Convert.ToInt32(OTPLength); i++)
            {
                temp = arr[rand.Next(0, arr.Length)];
                IDString += temp;
                NewCharacters = IDString;
            }

            return NewCharacters;
        }
      
        private string OTPGenerator(string uniqueIdentity, string uniqueCustomerIdentity)
        {
            int length = 4;
            string oneTimePassword = "";
            DateTime dateTime = DateTime.Now;
            string _strParsedReqNo = dateTime.Day.ToString();
            _strParsedReqNo = _strParsedReqNo + dateTime.Month.ToString();
            _strParsedReqNo = _strParsedReqNo + dateTime.Year.ToString();
            _strParsedReqNo = _strParsedReqNo + dateTime.Hour.ToString();
            _strParsedReqNo = _strParsedReqNo + dateTime.Minute.ToString();
            _strParsedReqNo = _strParsedReqNo + dateTime.Second.ToString();
            _strParsedReqNo = _strParsedReqNo + dateTime.Millisecond.ToString();
            _strParsedReqNo = _strParsedReqNo + uniqueCustomerIdentity;


            _strParsedReqNo = uniqueIdentity + uniqueCustomerIdentity;
            using (MD5 md5 = MD5.Create())
            {
                //Get hash code of entered request id in byte format.
                byte[] _reqByte = md5.ComputeHash(Encoding.UTF8.GetBytes(_strParsedReqNo));
                //convert byte array to integer.
                int _parsedReqNo = BitConverter.ToInt32(_reqByte, 0);
                string _strParsedReqId = Math.Abs(_parsedReqNo).ToString();
                //Check if length of hash code is less than 9.
                //If so, then prepend multiple zeros upto the length becomes atleast 9 characters.
                if (_strParsedReqId.Length < 9)
                {
                    StringBuilder sb = new StringBuilder(_strParsedReqId);
                    for (int k = 0; k < (9 - _strParsedReqId.Length); k++)
                    {
                        sb.Insert(0, '0');
                    }
                    _strParsedReqId = sb.ToString();
                }
                oneTimePassword = _strParsedReqId;
            }

            if (oneTimePassword.Length >= length)
            {
                oneTimePassword = oneTimePassword.Substring(0, length);
            }
            return oneTimePassword;
        }

        private SendSMSResult sendsmsOtp(string tomobile, string OTP, string otpRef)
        {
            var client = new RestClient(ENUM_SMS.apiUrl);
            client.Authenticator = new HttpBasicAuthenticator(ENUM_SMS.apiUser, ENUM_SMS.apiPassword);
            var request = new RestRequest(Method.POST);

            request.AddHeader("content-type", "application/json");
            request.AddParameter("msisdn", tomobile);
            request.AddParameter("message", "รหัส OTP คือ " + OTP + " เพื่อลงทะเบียนยืนยันตัวตนกับร้านติดดาว POS (ref:" + otpRef + ")");
            request.AddParameter("sender", ENUM_SMS.smsSenderName);
            request.AddParameter("force", "Corporate");


            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            IRestResponse response = client.Execute(request);

            SendSMSResult result = new SendSMSResult();
            result.isPass = (response.StatusCode.ToString().Trim().ToUpper().Equals("OK") || response.StatusCode.ToString().Trim().ToUpper().Equals("CREATED") ? true : false);
            result.status_code = response.StatusCode.ToString().Trim().ToUpper();
            result.status_desc = response.StatusDescription.ToString().Trim();
            return result;
        }

        public OTPResponseModel requestOTPByTel(SqlConnection dbCon, string tel)
        {
            _logger.LogInformation("requestOTPByTel tel: " + tel.Trim());
            AuthenRepository authenRepo = new AuthenRepository();
            string OtpCharacters = OTPCharacters();

            //Createing More Secure OTP Password by Using MD5 algorithm
            Random rng = new Random();
            string OTPPassword = OTPGenerator(OtpCharacters, rng.Next(10).ToString());
            _logger.LogInformation("requestOTPByTel OTPPassword: " + OTPPassword);
            //if you want to send otp to password to mobile number just uncomment below line and provide your bulksms url
            string telNoForSendSMS = tel.Trim();
            string otpRef = Guid.NewGuid().ToString().Substring(0, 10);
            SendSMSResult smsResult = sendsmsOtp(telNoForSendSMS, OTPPassword, otpRef);
            _logger.LogInformation("requestOTPByTel response ispass: " + smsResult.isPass + " for tel: " + tel.Trim());

            OTPResponseModel resp = new OTPResponseModel();
            resp.otpRef = otpRef;
            if (smsResult.isPass)
            {
                t_otpCode dtoOTP = new t_otpCode();
                dtoOTP.otpRef = otpRef;
                dtoOTP.otpTel = tel.Trim();
                dtoOTP.otpCode = OTPPassword;
                dtoOTP.otpExpire = DateTime.Now.AddMinutes(3).AddSeconds(10);
                dtoOTP.otpCreateDate = DateTime.Now;
                authenRepo.insertOTP(dbCon, dtoOTP);
            }
            else
            {
                _logger.LogInformation("requestOTPByTel fail: " + smsResult.status_desc);
                throw new Exception("Send otp fail [" + smsResult.status_desc + "]");
            }

            return resp;
        }
    }
}
