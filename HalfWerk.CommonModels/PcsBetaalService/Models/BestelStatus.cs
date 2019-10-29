using System;
using System.Collections.Generic;
using System.Text;

namespace HalfWerk.CommonModels.PcsBetaalService.Models
{
    public enum BestelStatus
    {
        Geplaatst,
        Goedgekeurd,
        WordtIngepakt,
        Verzonden,
        Betaald,
        Afgekeurd,
        WachtenOpAanbetaling,
        Afgerond
    }
}
