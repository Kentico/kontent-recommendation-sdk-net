using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Kentico.Kontent.Recommender
{
    public class RecommendationClient : RecommendationClientBase, IRecommendationClient
    {
        public RecommendationClient(string accessToken, int timeoutSeconds) : this("https://recommendations.kontent.ai", accessToken, timeoutSeconds)
        {
        }

        public RecommendationClient(string endpointUrl, string accessToken, int timeoutSeconds) : base(endpointUrl, accessToken, timeoutSeconds)
        {
        }

        public RecommendationClient(HttpClient client, string accessToken) : base(client, accessToken)
        {
        }

        private CallerInfo GetCallerInfoFromRequest(HttpRequest request, HttpResponse response, bool sessionBased)
        {
            var projectId = Helpers.GetProjectIdFromToken(Token);
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentException("Invalid authorization token.");

            var cookie = request.GetCurrentTrackingCookie(response, projectId);
            if (string.IsNullOrEmpty(cookie.Uid))
                throw new ArgumentException("Uid has to be set.", nameof(cookie.Uid));

            return request.GetCallerInfo(cookie.Uid, cookie.Sid, sessionBased);
        }

        public Task<RecommendedContentItem[]> GetRecommendationsAsync(
            string codename, 
            HttpRequest request, 
            HttpResponse response, 
            int limit,
            string contentType, 
            string filterQuery = "", 
            string boosterQuery = "", 
            string sourceApp = "",
            bool sessionBased = false,
            bool separateTracking = false)
        {
            if (string.IsNullOrEmpty(codename))
                throw new ArgumentException("Codename has to be set.", nameof(codename));

            if(string.IsNullOrEmpty(contentType))
                throw new ArgumentException("Content Type has to be set.", nameof(contentType));

            var callerInfo = GetCallerInfoFromRequest(request, response, sessionBased);
            
            var req = new RecommendationRequest
            {
                VisitId = callerInfo.VisitId,
                Method = HttpMethodEnum.Post,
                Codename = codename,
                Limit = limit,
                ContentTypeName = contentType,
                FilterQuery = filterQuery,
                BoosterQuery = boosterQuery,
                SourceApp = sourceApp,
                SeparateTracking = separateTracking,
                VisitorData = callerInfo
            };

            return GetRecommendationsFromRequest(req);
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

         public Task TrackConversionAsync(
            string codename, 
            HttpRequest request, 
            HttpResponse response, 
            string sourceApp = "", 
            bool sessionBased = false)
        {
            if (string.IsNullOrEmpty(codename))
                throw new ArgumentException("Codename has to be set.", nameof(codename));

            var callerInfo = GetCallerInfoFromRequest(request, response, sessionBased);
            callerInfo.SourceApp = sourceApp;

            return PostAsync<object>($"{TrackingApiRoutePrefix}/Conversion?visitId={callerInfo.VisitId}&contentItemId={codename}",
                JsonConvert.SerializeObject(callerInfo));
        }

        public Task TrackPortionViewAsync(
            string codename,
            int portionPercentage,
            HttpRequest request, 
            HttpResponse response,
            string sourceApp = "", 
            bool sessionBased = false)
        {
            if (string.IsNullOrEmpty(codename))
                throw new ArgumentException("Codename has to be set.", nameof(codename));

            var callerInfo = GetCallerInfoFromRequest(request, response, sessionBased);
            callerInfo.SourceApp = sourceApp;

            return PostAsync<object>($"{TrackingApiRoutePrefix}/PortionView?visitId={callerInfo.VisitId}&contentItemId={codename}&portionPercentage={portionPercentage}",
                JsonConvert.SerializeObject(callerInfo));
        }

        public Task TrackVisitAsync(
            string codename, 
            HttpRequest request, 
            HttpResponse response, 
            string sourceApp = "", 
            bool sessionBased = false)
        {
            if (string.IsNullOrEmpty(codename))
                throw new ArgumentException("Codename has to be set.", nameof(codename));

            var callerInfo = GetCallerInfoFromRequest(request, response, sessionBased);
            callerInfo.SourceApp = sourceApp;

            return PostAsync<object>($"{TrackingApiRoutePrefix}/Visit?visitId={callerInfo.VisitId}&contentItemId={codename}",
                JsonConvert.SerializeObject(callerInfo));
        }
    }
}

