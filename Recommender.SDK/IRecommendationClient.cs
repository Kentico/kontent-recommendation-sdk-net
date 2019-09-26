using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Kentico.Kontent.Recommender
{
    public interface IRecommendationClient : IRecommendationClientBase
    {
        Task<RecommendedContentItem[]> GetRecommendationsAsync(string codename, HttpRequest request, HttpResponse response, int limit, string contentType, string filterQuery = "", string boosterQuery = "", string sourceApp = "", bool sessionBased = false, bool separateTracking = false);
        RecommendationRequest CreateRequest(HttpRequest request, HttpResponse response, string codename, int limit, string contentType, bool sessionBased = false);

        Task TrackVisitAsync(string codename, HttpRequest request, HttpResponse response, string sourceApp = "", bool sessionBased = false);
        Task TrackConversionAsync(string codename, HttpRequest request, HttpResponse response, string sourceApp = "", bool sessionBased = false);
        Task TrackPortionViewAsync(string codename, int portionPercentage, HttpRequest request, HttpResponse response, string sourceApp = "", bool sessionBased = false);
    }
}