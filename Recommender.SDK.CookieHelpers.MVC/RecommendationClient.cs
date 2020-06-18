using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace Kentico.Kontent.Recommender.MVC
{
    public class RecommendationClient : RecommendationClientBase, IRecommendationClient
    {
        public RecommendationClient(string accessToken, int timeoutSeconds) : this(
            "https://recommendations.kontent.ai", accessToken, timeoutSeconds)
        {
        }

        public RecommendationClient(string endpointUrl, string accessToken, int timeoutSeconds) : base(endpointUrl, accessToken, timeoutSeconds)
        {
        }

        public RecommendationClient(HttpClient client, string accessToken) : base(client, accessToken)
        {
        }

        private CallerInfo GetCallerInfoFromRequest(HttpRequestBase request, HttpResponseBase response,
            bool sessionBased)
        {
            var projectId = Helpers.GetProjectIdFromToken(Token);
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentException("Invalid authorization token.");

            var cookie = request.GetCurrentTrackingCookie(response, projectId);
            if (string.IsNullOrEmpty(cookie.Uid))
                throw new ArgumentException("Uid has to be set.", nameof(cookie.Uid));

            return request.GetCallerInfo(cookie.Uid, cookie.Sid, sessionBased);
        }

        public Task<RecommendedContentItem[]> GetRecommendationsAsync(string codename, HttpRequestBase request,
            HttpResponseBase response, int limit,
            string contentType, string filterQuery = "", string boosterQuery = "", string sourceApp = "",
            bool sessionBased = false, bool separateTracking = false)
        {
            if (string.IsNullOrEmpty(codename))
                throw new ArgumentException("Codename has to be set.", nameof(codename));
            
            if(string.IsNullOrEmpty(contentType))
                throw new ArgumentException("Content Type has to be set.", nameof(contentType));

            var callerInfo = GetCallerInfoFromRequest(request, response, sessionBased);
            callerInfo.SourceApp = sourceApp;

            var queryString =
                new StringBuilder(
                    $"{RecommendationApiRoutePrefix}/Items?visitId={callerInfo.VisitId}&currentItemId={codename}&limit={limit}");

            if (!string.IsNullOrWhiteSpace(contentType))
                queryString.Append($"&contentTypeName={contentType}");

            if (!string.IsNullOrWhiteSpace(filterQuery))
                queryString.Append($"&filterQuery={filterQuery}");

            if (!string.IsNullOrWhiteSpace(boosterQuery))
                queryString.Append($"&boosterQuery={boosterQuery}");

            if (separateTracking)
                queryString.Append("&separateTracking=true");

            return PostAsync<RecommendedContentItem[]>(queryString.ToString(), JsonConvert.SerializeObject(callerInfo));
        }

        public Task TrackConversionAsync(
            string codename,
            HttpRequestBase request,
            HttpResponseBase response,
            string sourceApp = "",
            bool sessionBased = false)
        {
            if (string.IsNullOrEmpty(codename))
                throw new ArgumentException("Codename has to be set.", nameof(codename));

            var callerInfo = GetCallerInfoFromRequest(request, response, sessionBased);
            callerInfo.SourceApp = sourceApp;

            return PostAsync<object>(
                $"{TrackingApiRoutePrefix}/Conversion?visitId={callerInfo.VisitId}&contentItemId={codename}",
                JsonConvert.SerializeObject(callerInfo));
        }

        public Task TrackPortionViewAsync(
            string codename,
            int portionPercentage,
            HttpRequestBase request,
            HttpResponseBase response,
            string sourceApp = "",
            bool sessionBased = false)
        {
            if (string.IsNullOrEmpty(codename))
                throw new ArgumentException("Codename has to be set.", nameof(codename));

            var callerInfo = GetCallerInfoFromRequest(request, response, sessionBased);
            callerInfo.SourceApp = sourceApp;

            return PostAsync<object>(
                $"{TrackingApiRoutePrefix}/PortionView?visitId={callerInfo.VisitId}&contentItemId={codename}&portionPercentage={portionPercentage}",
                JsonConvert.SerializeObject(callerInfo));
        }

        public Task TrackVisitAsync(
            string codename,
            HttpRequestBase request,
            HttpResponseBase response,
            string sourceApp = "",
            bool sessionBased = false)
        {
            if (string.IsNullOrEmpty(codename))
                throw new ArgumentException("Codename has to be set.", nameof(codename));

            var callerInfo = GetCallerInfoFromRequest(request, response, sessionBased);
            callerInfo.SourceApp = sourceApp;

            return PostAsync<object>(
                $"{TrackingApiRoutePrefix}/Visit?visitId={callerInfo.VisitId}&contentItemId={codename}",
                JsonConvert.SerializeObject(callerInfo));
        }

        public RecommendationRequest CreateRequest(HttpRequestBase request, HttpResponseBase response, string codename,
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
}