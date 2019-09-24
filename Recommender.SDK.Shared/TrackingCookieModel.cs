namespace Kentico.Kontent.Recommender.Recommender
{
    public class TrackingCookieModel
    {
        public const string Name = "k_e_id";
        public string Uid { get; set; }
        public int SessionExpiration { get; set; }
        public string Sid { get; set; }

        public override string ToString()
        {
            return $"{Uid}.{SessionExpiration}.{Sid}";
        }
    }
}