using System.Threading.Tasks;
using Kentico.Kontent.Recommendations.Models;

namespace Kentico.Kontent.Recommendations
{
    /// <summary>
    /// SDK for using the Recommendation API in Kentico Kontent
    /// </summary>
    public interface IRecommendationClient
    {
        /// <summary>
        /// Returns recommendations provided by the recommendation engine based on sent request
        /// </summary>
        /// <param name="recommendationRequest"></param>
        /// <returns></returns>
        Task<RecommendedContentItem[]> GetRecommendationsAsync(RecommendationRequest recommendationRequest);

        /// <summary>
        /// Creates a new visitor in the recommendation engine
        /// IMPORTANT: Does not need separate tracking enabled to work
        /// </summary>
        /// <param name="visitId"></param>
        /// <param name="visitor"></param>
        /// <returns></returns>
        Task CreateVisitorAsync(string visitId, VisitorDetails visitor);

        /// <summary>
        /// Logs an event of a visitor visiting an item with a given codename
        /// </summary>
        /// <param name="visitId"></param>
        /// <param name="currentItemCodename"></param>
        /// <returns></returns>
        Task TrackVisitAsync(string visitId, string currentItemCodename);

        /// <summary>
        /// Logs an event of visitor converting on an item with a given codename
        /// </summary>
        /// <param name="visitId"></param>
        /// <param name="currentItemCodename"></param>
        /// <returns></returns>
        Task TrackConversionAsync(string visitId, string currentItemCodename);

        /// <summary>
        /// Logs an event of visitor viewing a portion of item with a given codename
        /// </summary>
        /// <param name="visitId"></param>
        /// <param name="currentItemCodename"></param>
        /// <param name="portionPercentage"></param>
        /// <returns></returns>
        Task TrackPortionViewAsync(string visitId, string currentItemCodename, int portionPercentage);
    }
}