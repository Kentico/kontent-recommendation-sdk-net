namespace Kentico.Kontent.Recommendations.Models
{
    /// <summary>
    /// Extra recommendation options
    /// </summary>
    public class RecommendationSettings
    {
        /// <summary>
        /// ReQL filter query
        /// Let's you filter results based on different properties
        /// https://docs.recombee.com/reql.html
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// ReQL booster query
        /// Let's you prioritize content that is going to be recommended
        /// https://docs.recombee.com/reql.html
        /// </summary>
        public string Booster { get; set; }

        /// <summary>
        /// Let's you specify recommendation scenario
        /// </summary>
        public string Scenario { get; set; }
    }
}