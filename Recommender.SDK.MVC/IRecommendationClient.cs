using System.Threading.Tasks;
using System.Web;

namespace KenticoCloud.Recommender.MVC
{
    public interface IRecommendationClient : IRecommendationClientBase
    {
        Task<RecommendedContentItem[]> GetRecommendationsAsync(string codename, HttpRequestBase request, HttpResponseBase response, int limit, string contentType, string filterQuery = "", string boosterQuery = "", string sourceApp = "", bool sessionBased = false, bool separateTracking = false);

        RecommendationRequest CreateRequest(HttpRequestBase request, HttpResponseBase response, string codename, int limit, string contentType, bool sessionBased = false);

        Task TrackVisitAsync(string codename, HttpRequestBase request, HttpResponseBase response, string sourceApp = "", bool sessionBased = false);
        Task TrackConversionAsync(string codename, HttpRequestBase request, HttpResponseBase response, string sourceApp = "", bool sessionBased = false);
        Task TrackPortionViewAsync(string codename, int portionPercentage, HttpRequestBase request, HttpResponseBase response, string sourceApp = "", bool sessionBased = false);

    }
}