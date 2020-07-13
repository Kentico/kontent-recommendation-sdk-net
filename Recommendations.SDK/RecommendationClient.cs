using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Kentico.Kontent.Recommendations.Models;
using Newtonsoft.Json;

namespace Kentico.Kontent.Recommendations
{
    /// <inheritdoc />
    public class RecommendationClient : IRecommendationClient
    {
        private readonly HttpClient Client;
        private const string RecommendationEndpointRoutePrefix = "/api/v2/recommend";
        private const string TrackingEndpointRoutePrefix = "/api/v2/track";

        public RecommendationClient(string accessToken, int timeoutSeconds) : this("http://recommend.kontent.ai", accessToken, timeoutSeconds)
        {
        }

        public RecommendationClient(string endpointUrl, string accessToken, int timeoutSeconds)
        {
            Client = new HttpClient { BaseAddress = new Uri(endpointUrl), Timeout = TimeSpan.FromSeconds(timeoutSeconds)};
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        }

        public RecommendationClient(HttpClient client)
        {
            Client = client;
        }

        private async Task<T> PostAsync<T>(string url, string content)
        {
            using (var response = await Client.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/json")))
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<T>(responseBody);

                throw new Exception(responseBody);
            }
        }

        /// <inheritdoc />
        public Task<RecommendedContentItem[]> GetRecommendationsAsync(RecommendationRequest request) { 
            return PostAsync<RecommendedContentItem[]>($"{RecommendationEndpointRoutePrefix}/items", JsonConvert.SerializeObject(request));
        }

        /// <inheritdoc />
        public Task CreateVisitorAsync(string visitId, VisitorDetails visitor)
        {
            if (string.IsNullOrEmpty(visitId))
                throw new ArgumentException("Visit Id has to be set.", nameof(visitId));

            if (visitor == null)
                throw new ArgumentException("Visitor details has to be set.", nameof(visitor));

            return PostAsync<object>($"{TrackingEndpointRoutePrefix}/visitor?visitId={visitId}", JsonConvert.SerializeObject(visitor));
        }

        /// <inheritdoc />
        public Task TrackVisitAsync(string visitId, string currentItemCodename)
        {
            if (string.IsNullOrEmpty(visitId))
                throw new ArgumentException("Visit Id has to be set.", nameof(visitId));

            if (string.IsNullOrEmpty(currentItemCodename))
                throw new ArgumentException("Codename of the currently viewed item has to be set.", nameof(currentItemCodename));

            return PostAsync<object>($"{TrackingEndpointRoutePrefix}/visit?visitId={visitId}&contentItemId={currentItemCodename}", "");
        }

        /// <inheritdoc />
        public Task TrackConversionAsync(string visitId, string currentItemCodename)
        {
            if (string.IsNullOrEmpty(visitId))
                throw new ArgumentException("Visit Id has to be set.", nameof(visitId));

            if (string.IsNullOrEmpty(currentItemCodename))
                throw new ArgumentException("Codename of the currently viewed item has to be set.", nameof(currentItemCodename));

            return PostAsync<object>($"{TrackingEndpointRoutePrefix}/Conversion?visitId={visitId}&contentItemId={currentItemCodename}","");
        }

        /// <inheritdoc />
        public Task TrackPortionViewAsync(string visitId, string currentItemCodename, int portionPercentage)
        {
            if (string.IsNullOrEmpty(visitId))
                throw new ArgumentException("Visit Id has to be set.", nameof(visitId));

            if (string.IsNullOrEmpty(currentItemCodename))
                throw new ArgumentException("Codename of the currently viewed item has to be set.", nameof(currentItemCodename));

            return PostAsync<object>($"{TrackingEndpointRoutePrefix}/PortionView?visitId={visitId}&contentItemId={currentItemCodename}&portionPercentage={portionPercentage}","");
        }
    }
}

