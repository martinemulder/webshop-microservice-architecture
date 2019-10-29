namespace HalfWerk.CommonModels
{
    public static class NameConstants
    {
        // Catalogus
        public const string CatalogusServiceCategorieAanCatalogusToegevoegdEvent = "Kantilever.CatalogusService.ArtikelAanCatalogusToegevoegd";

        // MagazijnService
        public const string MagazijnServiceCommandQueue = "Kantilever.MagazijnService";
        public const string MagazijnServiceVoorraadChangedEvent = "Kantilever.MagazijnService.*";
        public const string MagazijnServiceVoorraadVerlaagdEvent = "Kantilever.MagazijnService.VoorraadVerlaagdEvent";
        public const string MagazijnServiceVoorraadVerhoogdEvent = "Kantilever.MagazijnService.VoorraadVerhoogdEvent";
        public const string MagazijnServiceHaalVoorraadUitMagazijnCommand = "Kantilever.MagazijnService.HaalVoorraadUitMagazijnCommand";
        public const string MagazijnServicePlaatsVoorraadInMagazijnCommand = "Kantilever.MagazijnService.PlaatsVoorraadInMagazijnCommand";

        // BffWebshop
        public const string BffWebshopEventReplayExchange = "BffWebshop.EventReplayExchange";
        public const string BffWebshopEventQueue = "Kantilever.BffWebshop.EventQueue";
        public const string BffWebshopEventReplayQueue = "Kantilever.BffWebshop.EventReplayQueue";
        public const string BffWebshopBestellingQueue = "Kantilever.BffWebshop.BestellingQueue";

        // BestelService
        public const string BestelServiceEventReplayExchange = "DsBestelService.EventReplayExchange";
        public const string BestelServiceEventReplayQueue = "DsBestelService.EventReplayQueue";
        public const string BestelServicePlaatsBestellingCommandQueue = "Kantilever.DsBestelService.PlaatsBestelling";
        public const string BestelServiceBestellingGeplaatstEvent = "Kantilever.DsBestelService.BestellingGeplaatst";
        public const string BestelServiceEventQueue = "Kantilever.DsBestelService.EventQueue";
        public const string BestelServiceUpdateBestelStatusCommandQueue = "Kantilever.DsBestelService.BestelStatusUpgedate";
        public const string BestelServiceBestelStatusUpgedateEvent = "Kantilever.DsBestelService.BestelStatusUpgedate";

        // BetaalService
        public const string BetaalServiceBetalingVerwerkenCommandQueue = "Kantilever.PcsBetaalService.VerwerkBetaling";
        public const string BetaalServiceEventQueue = "Kantilever.PcsBetaalService.EventQueue";
        public const string BetaalServiceBetalingGeaccrediteerdEvent = "Kantilever.PcsBetaalService.BestellingGeaccrediteerd";

        // Auditlog
        public const string AuditlogReplayCommandType = "Minor.WSA.AuditLog.Commands.ReplayEventsCommand";
        public const string AuditlogQueue = "AuditlogReplayService";

        // KlantBeheer
        public const string KlantBeheerEventQueue = "Kantilever.DsKlantBeheer.EventQueue";
        public const string KlantBeheerVoegKlantToeCommand = "Kantilever.DsKlantBeheer.VoegKlantToe";
        public const string KlantBeheerKlantToegevoegdEvent = "Kantilver.DsKlantBeheer.KlantToegevoegd";

        // AuthenticationService
        public const string AuthenticationServiceLoginCommand = "Kantilever.AuthenticationService.LoginCommand";
        public const string AuthenticationServiceRegisterCommand = "Kantilever.AuthenticationService.RegisterCommand";
        public const string AuthenticationServiceValidateCommand = "Kantilever.AuthenticationService.ValidateCommand";
        public const string AuthenticationServiceAddRoleCommand = "Kantilever.AuthenticationService.AddRole";

    }
}