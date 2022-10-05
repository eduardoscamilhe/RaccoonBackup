namespace Raccoon.Backup
{
    public class AppSettings
    {
        public string OriginFolder { get; set; }
        public string DestinationFolder { get; set; }
        public int MinutePeriod { get; set; }
        public int ExactHour { get; set; }
    }
}
