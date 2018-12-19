using System;
using System.Threading.Tasks;

namespace KenticoCloud.Recommender.SDK
{
    public class RecommendationRequest
    {
        internal string VisitId { get; set; }
        internal string Codename { get; set; }
        internal string ContentTypeName { get; set; }
        internal int Limit { get; set; }
        internal string BoosterQuery { get; set; }
        internal string FilterQuery { get; set; }
        internal string SourceApp { get; set; }
        internal bool SeparateTracking { get; set; }
        internal CallerInfo VisitorData { get; set; }
        internal HttpMethodEnum Method { get; set; }

        internal Func<RecommendationRequest, Task<RecommendedContentItem[]>> Executor;
    }

    internal enum HttpMethodEnum : byte
    {
        Get = 0,
        Post = 1
    }
}
