using System.Threading.Tasks;

namespace Kentico.Kontent.Recommender.Recommender
{
    public interface IRecommendationClientBase
    {
        Task<RecommendedContentItem[]> GetRecommendationsAsync(string visitId, string codename, int limit, string contentType, string filterQuery = "", string boosterQuery= "", string sourceApp = "", bool separateTracking = false);
        
        RecommendationRequest CreateRequest(string visitId, string codename, int limit, string contentType);
        
        Task TrackVisitAsync(string visitId, string codename, CallerInfo callerInfo = null);
        Task TrackConversionAsync(string visitId, string codename, CallerInfo callerInfo = null);
        Task TrackPortionViewAsync(string visitId, string codename, int portionPercentage, CallerInfo callerInfo = null);
    }
}