namespace Kentico.Kontent.Recommendations.Models
{
    internal class VisitData
    {
        /// <summary>
        /// Id of the visitor (user-provided)
        /// </summary>
        public string VisitId { get; set; }

        /// <summary>
        /// Codename of currently viewed content item
        /// </summary>
        public string CurrentItemCodename { get; set; }
    }

    internal class PortionViewData : VisitData
    {
        /// <summary>
        /// Percentage of read content (only the highest value reported counts in case of multiple consecutive reports)
        /// </summary>
        public int PortionPercentage { get; set; }
    }
}
