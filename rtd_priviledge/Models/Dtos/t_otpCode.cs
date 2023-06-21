namespace rtd_priviledge.Models.Dtos
{
    public class t_otpCode
    {
        public long otpId { get; set; }
        public string otpRef { get; set; }
        public string otpTel { get; set; }
        public string otpCode { get; set; }
        public DateTime otpExpire { get; set; }
        public DateTime otpCreateDate { get; set; }
    }
}
