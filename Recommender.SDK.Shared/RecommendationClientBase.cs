using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KenticoCloud.Recommender.SDK.Shared
{
    public abstract partial class RecommendationClientBase : IRecommendationClientBase
    {
        protected readonly HttpClient Client;
        protected const string RecommendationApiRoutePrefix = "api/Recommend";
        protected readonly string Token;

        protected RecommendationClientBase(string accessToken, int timeoutSeconds) : this("https://kc-recommender-api-beta.kenticocloud.com", accessToken, timeoutSeconds)
        {
        }

        protected RecommendationClientBase(string endpointUrl, string accessToken, int timeoutSeconds)
        {
            Client = new HttpClient { BaseAddress = new Uri(endpointUrl), Timeout = TimeSpan.FromSeconds(timeoutSeconds)};
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            Token = accessToken;
        }

        protected RecommendationClientBase(HttpClient client, string accessToken)
        {
            Client = client;
            Token = accessToken;
        }

        protected async Task<T> GetAsync<T>(string url)
        {
            using (var response = await Client.GetAsync(url))
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<T>(responseBody);

                throw new RecommendationException(response.StatusCode, responseBody);
            }
        }

        protected async Task<T> PostAsync<T>(string url, string content)
        {
            using (var response = await Client.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/json")))
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<T>(responseBody);

                throw new RecommendationException(response.StatusCode, responseBody);
            }
        }

        public Task<RecommendedContentItem[]> GetRecommendationsFromRequest(RecommendationRequest request)
        {
            switch (request.Method)
            {
                case HttpMethodEnum.Get:
                    return GetAsync<RecommendedContentItem[]>(Helpers.GetQueryFromRequest(request, RecommendationApiRoutePrefix));
                case HttpMethodEnum.Post:
                    return PostAsync<RecommendedContentItem[]>(Helpers.GetQueryFromRequest(request, RecommendationApiRoutePrefix), JsonConvert.SerializeObject(request.VisitorData));
                default:
                    throw new Exception("Unspecified Http Method for Request");
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

            var projectId = Helpers.GetProjectIdFromToken(Token);
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
    }
}

