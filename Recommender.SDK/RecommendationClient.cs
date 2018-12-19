using System;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace KenticoCloud.Recommender.SDK
{
    public partial class RecommendationClient : IRecommendationClient
    {
        private readonly HttpClient _httpClient;
        private const string RecommendationApiRoutePrefix = "api/Recommend";
        private readonly string _token;

        public RecommendationClient(string accessToken, int timeoutSeconds) : this("https://kc-recommender-api-beta.kenticocloud.com", accessToken, timeoutSeconds)
        {
        }

        public RecommendationClient(string endpointUrl, string accessToken, int timeoutSeconds)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(endpointUrl), Timeout = TimeSpan.FromSeconds(timeoutSeconds)};
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            _token = accessToken;
        }

        public RecommendationClient(HttpClient client, string accessToken)
        {
            _httpClient = client;
            _token = accessToken;
        }

        private CallerInfo GetCallerInfoFromRequest(HttpRequest request, HttpResponse response, bool sessionBased)
        {
            var projectId = TokenHelpers.GetProjectIdFromToken(_token);
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentException("Invalid authorization token.");

            var cookie = request.GetCurrentTrackingCookie(response, projectId);
            if (string.IsNullOrEmpty(cookie.Uid))
                throw new ArgumentException("Uid has to be set.", nameof(cookie.Uid));

            return request.GetCallerInfo(cookie.Uid, cookie.Sid, sessionBased);
        }

        private async Task<T> GetAsync<T>(string url)
        {
            using (var response = await _httpClient.GetAsync(url))
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<T>(responseBody);

                throw new RecommendationException(response.StatusCode, responseBody);
            }
        }

        private async Task<T> PostAsync<T>(string url, string content)
        {
            using (var response = await _httpClient.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/json")))
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<T>(responseBody);

                throw new RecommendationException(response.StatusCode, responseBody);
            }
        }

        private Task<RecommendedContentItem[]> GetRecommendationsFromRequest(RecommendationRequest request)
        {
            var queryString =
                new StringBuilder($"{RecommendationApiRoutePrefix}/Items?currentItemId={request.Codename}&visitId={request.VisitId}&limit={request.Limit}");

            if (!string.IsNullOrWhiteSpace(request.ContentTypeName) && request.ContentTypeName != "*")
                queryString.Append($"&contentTypeName={request.ContentTypeName}");

            if(!string.IsNullOrWhiteSpace(request.FilterQuery))
                queryString.Append($"&filterQuery={request.FilterQuery}");

            if(!string.IsNullOrWhiteSpace(request.BoosterQuery))
                queryString.Append($"&boosterQuery={request.BoosterQuery}");

            if(!string.IsNullOrWhiteSpace(request.SourceApp))
                queryString.Append($"&sourceApp={request.SourceApp}");

            if (request.SeparateTracking)
                queryString.Append($"&separateTracking=true");

            switch (request.Method)
            {
                case HttpMethodEnum.Get:
                    return GetAsync<RecommendedContentItem[]>(queryString.ToString());
                case HttpMethodEnum.Post:
                    return PostAsync<RecommendedContentItem[]>(queryString.ToString(), JsonConvert.SerializeObject(request.VisitorData));
                default:
                    throw new Exception("Unspecified Htttp Method for Request");
            }
        }

        public Task<RecommendedContentItem[]> GetRecommendationsAsync(
            string visitId, 
            string codename, 
            int limit, 
            string contentType,
            string filterQuery = "", 
            string boosterQuery = "", 
            string sourceApp = "",
            bool separateTracking = false)
        {
            if (string.IsNullOrEmpty(visitId))
                throw new ArgumentException("Visit Id has to be set.", nameof(visitId));

            if(string.IsNullOrEmpty(codename))
                throw new ArgumentException("Codename has to be set.", nameof(codename));

            if(string.IsNullOrEmpty(contentType))
                throw new ArgumentException("Content Type has to be set.", nameof(contentType));

            var projectId = TokenHelpers.GetProjectIdFromToken(_token);
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentException("Invalid authorization token.");

            var request = new RecommendationRequest
            {
                Method = HttpMethodEnum.Get,
                VisitId = visitId,
                Codename = codename,
                Limit = limit,
                ContentTypeName = contentType,
                FilterQuery = filterQuery,
                BoosterQuery = boosterQuery,
                SourceApp = sourceApp,
                SeparateTracking = separateTracking
            };

            return GetRecommendationsFromRequest(request);
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
    }
}

