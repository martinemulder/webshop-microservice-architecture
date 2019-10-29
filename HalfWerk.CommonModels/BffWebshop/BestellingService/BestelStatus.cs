using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HalfWerk.CommonModels.BffWebshop.BestellingService
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BestelStatus
    {
        Geplaatst,
        Goedgekeurd,
        WordtIngepakt,
        Verzonden,
        WachtenOpAanbetaling,
        Betaald,
        Afgekeurd,
        Afgerond
    }
}