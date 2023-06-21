namespace rtd_priviledge.Models.Dtos
{
    public class t_custregister
    {
        public Guid regGuid { get; set; }
        public string shopCode { get; set; }
        public DateTime regDate { get; set; }
        public bool mktConsentAccept { get; set; }
        public string regPhoneNumber { get; set; }
        public DateTime AddAddressDate { get; set; }       

        //delivery address & phone number
        public string? delivery_name { get; set; }
        public string? delivery_lastname { get; set; }
        public string? delivery_phonenumber { get; set; }
        public string? delivery_address1 { get; set; }
        public int? delivery_prov_id { get; set; }
        public int? delivery_dist_id { get; set; }
        public int? delivery_subdist_id { get; set; }
        public bool delivery_confirm_address { get; set; }
    }
}
