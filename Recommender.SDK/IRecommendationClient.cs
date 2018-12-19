using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace KenticoCloud.Recommender.SDK
{
    public interface IRecommendationClient
    {
        Task<RecommendedContentItem[]> GetRecommendationsAsync(string visitId, string codename, int limit, string contentType, string filterQuery = "", string boosterQuery= "", string sourceApp = "", bool separateTracking = false);
        Task<RecommendedContentItem[]> GetRecommendationsAsync(string codename, HttpRequest request, HttpResponse response, int limit, string contentType, string filterQuery = "", string boosterQuery = "", string sourceApp = "", bool sessionBased = false, bool separateTracking = false);

        RecommendationRequest CreateRequest(string visitId, string codename, int limit, string contentType);
        RecommendationRequest CreateRequest(HttpRequest request, HttpResponse response, string codename, int limit, string contentType, bool sessionBased = false);

        Task TrackVisitAsync(string visitId, string codename, CallerInfo callerInfo = null);
        Task TrackVisitAsync(string codename, HttpRequest request, HttpResponse response, string sourceApp = "", bool sessionBased = false);
        Task TrackConversionAsync(string visitId, string codename, CallerInfo callerInfo = null);
        Task TrackConversionAsync(string codename, HttpRequest request, HttpResponse response, string sourceApp = "", bool sessionBased = false);
        Task TrackPortionViewAsync(string visitId, string codename, int portionPercentage, CallerInfo callerInfo = null);
        Task TrackPortionViewAsync(string codename, int portionPercentage, HttpRequest request, HttpResponse response, string sourceApp = "", bool sessionBased = false);
    }
}