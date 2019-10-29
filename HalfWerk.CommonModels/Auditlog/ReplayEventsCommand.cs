namespace HalfWerk.CommonModels.Auditlog
{
    public class ReplayEventsCommand
    {
        public string ExchangeName { get; set; }
        public long? FromTimestamp { get; set; }
        public long? ToTimestamp { get; set; }
        public string EventType { get; set; }
        public string Topic { get; set; }
    }
}
