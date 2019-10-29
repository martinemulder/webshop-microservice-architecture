using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace HalfWerk.CommonModels.BffWebshop.BestellingService
{
    public class UpdateBestelStatus
    {
        [Required(ErrorMessage = "BestelStatus is verplicht")]
        [JsonConverter(typeof(StringEnumConverter))]
        public BestelStatus Status { get; set; }
    }
}
