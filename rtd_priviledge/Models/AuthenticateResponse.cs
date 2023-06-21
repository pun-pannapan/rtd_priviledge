namespace rtd_priviledge.Models
{
    public class CheckOTP_ResponseResult
    {
        public bool isValid { get; set; }
        public string resultDesc { get; set; }
        public bool isTelExistedWhenInvalid { get; set; }
    }
    public class OTPResponseModel
    {
        public string otpRef { get; set; }
    }
}
