using Microsoft.AspNetCore.Mvc.Rendering;
using rtd_priviledge.Models.Dtos;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace rtd_priviledge.Models
{
    public class CustRegisterViewModel : t_custregister
    {
        public bool privacyPolicy { get; set; }
        public bool confirmAddAddress { get; set; }
        public string? deliveryZipcode { get; set; }
        public string? shopCode { get; set; }
        public string? regPhoneNumber { get; set; }
        public SelectList? selectProvinces { get; set; }        
        public SelectList? selectDistricts { get; set; }
        public SelectList? selectSubDistricts { get; set; }
    }
}
