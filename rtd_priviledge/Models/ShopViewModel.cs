using rtd_priviledge.Models.Dtos;

namespace rtd_priviledge.Models
{
    public class ShopViewModel : m_shopcode
    {
        public Guid? regGuid { get; set; }
        public string provinceName { get; set; }

        public string shopTypeName { get; set; }

        public string shopTypeShortCode { get; set; }

        public string? regPhoneNumber { get; set; }

        public bool deliveryConfirmAddress { get; set; }

        public OTPResponseModel? otpResponseModel { get; set; }

        public string? otpCode { get; set; }
    }
}
