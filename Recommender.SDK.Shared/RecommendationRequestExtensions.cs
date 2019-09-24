using System.Threading.Tasks;

namespace Kentico.Kontent.Recommender.Recommender
{
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