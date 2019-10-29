using System.Collections.Generic;

namespace HalfWerk.CommonModels.DsBestelService.Models
{
    public class BestellingCM
    {
        public long Klantnummer { get; set; }
        public ContactInfoCM ContactInfo { get; set; }
        public AfleveradresCM Afleveradres { get; set; }
        public List<BestelRegelCM> BestelRegels { get; set; }
    }
}