namespace rtd_priviledge.Models.Dtos
{
    public class m_shopcode
    {
        public Guid shopCodeGuid { get; set; }
        public string shopCode { get; set; }
        public string shopName { get; set; }
        public bool isRegister { get; set; }
        public int distId { get; set; }
        public int provId { get; set; }
        public int subDistId { get; set; }
        public string address { get; set; }
        public string shopTypeCode { get; set; }
        public string shopPhoneNumber { get; set; }
    }
}
