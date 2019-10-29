using HalfWerk.CommonModels.DsBestelService.Models;

namespace HalfWerk.DsBestelService.Helpers
{
    public static class BestelStatusHelper
    {
        public static bool UpdateIsAllowed(this BestelStatus current, BestelStatus newStatus)
        {
            switch (current)
            {
                case BestelStatus.Geplaatst when newStatus == BestelStatus.Goedgekeurd:
                case BestelStatus.Geplaatst when newStatus == BestelStatus.Betaald:
                case BestelStatus.Geplaatst when newStatus == BestelStatus.Afgekeurd:
                case BestelStatus.Geplaatst when newStatus == BestelStatus.WachtenOpAanbetaling:

                case BestelStatus.Goedgekeurd when newStatus == BestelStatus.WordtIngepakt:
                case BestelStatus.Goedgekeurd when newStatus == BestelStatus.Betaald:
                case BestelStatus.Goedgekeurd when newStatus == BestelStatus.Afgekeurd:

                case BestelStatus.Verzonden when newStatus == BestelStatus.Afgerond:
                case BestelStatus.Betaald when newStatus == BestelStatus.Afgerond:

                case BestelStatus.WachtenOpAanbetaling when newStatus == BestelStatus.Betaald:
                case BestelStatus.WachtenOpAanbetaling when newStatus == BestelStatus.Goedgekeurd:
                case BestelStatus.WachtenOpAanbetaling when newStatus == BestelStatus.Afgekeurd:

                case BestelStatus.WordtIngepakt when newStatus == BestelStatus.Verzonden:
                case BestelStatus.Betaald when newStatus == BestelStatus.Verzonden:
                case BestelStatus.Afgekeurd when newStatus == BestelStatus.Goedgekeurd:
                    return true;
            }

            return false;
        }
    }
}
