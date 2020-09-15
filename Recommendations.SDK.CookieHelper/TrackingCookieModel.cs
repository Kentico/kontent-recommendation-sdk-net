namespace Kentico.Kontent.Recommendations.CookieHelper
{
    public class TrackingCookieModel
    {
        public const string DefaultName = "k_e_id";
        
        public string Name { get; set; }= DefaultName;
        public string VisitId { get; set; }
        public int Expiration { get; set; }

        public override string ToString()
        {
            return $"{VisitId}.{Expiration}";
        }
    }
}