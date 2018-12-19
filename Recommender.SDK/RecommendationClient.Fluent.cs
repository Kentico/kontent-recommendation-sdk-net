using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace KenticoCloud.Recommender.SDK
{
    public partial class RecommendationClient
    {
        public RecommendationRequest CreateRequest(string visitId, string codename, int limit, string contentTypeName)
        {
            var request = new RecommendationRequest
            {
                VisitId = visitId,
                Codename = codename,
                Limit = limit,
                ContentTypeName = contentTypeName,
                Method = HttpMethodEnum.Get,
                Executor = GetRecommendationsFromRequest
            };

            return request;
        }

        public RecommendationRequest CreateRequest(HttpRequest request, HttpResponse response, string codename,
            int limit, string contentType, bool sessionBased = false)
        {
            var visitorData = GetCallerInfoFromRequest(request, response, sessionBased);
            var req = new RecommendationRequest
            {
                VisitId = visitorData.VisitId,
                Codename = codename,
                Limit = limit,
                ContentTypeName = contentType,
                VisitorData = visitorData,
                Method = HttpMethodEnum.Post,
                Executor = GetRecommendationsFromRequest
            };

            return req;
        }
    }

    public static class RecommendationRequestExtensions
    {
        public static RecommendationRequest WithBoosterQuery(this RecommendationRequest request, string boosterQuery)
        {
            request.BoosterQuery = boosterQuery;
            return request;
        }

        public static RecommendationRequest WithFilterQuery(this RecommendationRequest request, string filterQuery)
        {
            request.FilterQuery = filterQuery;
            return request;
        }

        public static RecommendationRequest WithSeparateTracking(this RecommendationRequest request)
        {
            request.SeparateTracking = true;
            return request;
        }

        public static RecommendationRequest IgnoreContentType(this RecommendationRequest request)
        {
            request.ContentTypeName = "*";
            return request;
        }

        public static Task<RecommendedContentItem[]> Execute(this RecommendationRequest request)
        {
            return request.Executor(request);
        }
    }
}
