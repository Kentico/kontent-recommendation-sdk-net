namespace KenticoCloud.Recommender
{
    public abstract partial class RecommendationClientBase
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
    }
}
