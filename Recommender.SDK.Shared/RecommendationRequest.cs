using System;
using System.Threading.Tasks;

namespace Recommender.SDK.Shared
{
    public class RecommendationRequest
    {
        public string VisitId { get; set; }
        public string Codename { get; set; }
        public string ContentTypeName { get; set; }
        public int Limit { get; set; }
        public string BoosterQuery { get; set; }
        public string FilterQuery { get; set; }
        public string SourceApp { get; set; }
        public bool SeparateTracking { get; set; }
        public CallerInfo VisitorData { get; set; }
        public HttpMethodEnum Method { get; set; }

        public Func<RecommendationRequest, Task<RecommendedContentItem[]>> Executor;
    }

    public enum HttpMethodEnum : byte
    {
        Get = 0,
        Post = 1
    }
}
