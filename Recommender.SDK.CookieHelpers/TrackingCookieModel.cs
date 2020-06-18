namespace Kentico.Kontent.Recommender.SDK.CookieHelper
{
    public class TrackingCookieModel
    {
        public const string Name = "k_e_id";
        public string VisitId { get; set; }
        public int Expiration { get; set; }

        public override string ToString()
        {
            return $"{VisitId}.{Expiration}";
        }
    }
}