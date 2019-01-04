namespace KenticoCloud.Recommender
{
    public class CallerInfo
    {
        public string VisitId { get; set; }
        public string Ip { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Referer { get; set; }        
        public string SourceApp { get; set; }
        public bool Mobile { get; set; }
    }
}
