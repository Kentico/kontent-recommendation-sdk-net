﻿using System;
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
        private const string SearchEndpointRoutePrefix = "/api/v2/seach";

        /// <summary>
        /// Create a new instance of the recommendation client
        /// </summary>
        /// <param name="recommendationApiKey">Your recommendation Api Key (token)</param>
        /// <param name="timeoutSeconds">Time after which the request will be canceled if not completed. You should implement a fallback if this situation ever occurs.</param>
        public RecommendationClient(string recommendationApiKey, int timeoutSeconds) : this("https://recommend.kontent.ai", recommendationApiKey, timeoutSeconds)
        {
        }

        /// <summary>
        /// Create a new instance of the recommendation client
        /// </summary>
        /// <param name="endpointUrl">Address of Kontent Recommendation API</param>
        /// <param name="recommendationApiKey">Your recommendation Api Key (token)</param>
        /// <param name="timeoutSeconds"></param>
        public RecommendationClient(string endpointUrl, string recommendationApiKey, int timeoutSeconds)
        {
            Client = new HttpClient { BaseAddress = new Uri(endpointUrl), Timeout = TimeSpan.FromSeconds(timeoutSeconds)};
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {recommendationApiKey}");
        }

        /// <summary>
        /// Create a new instance of the recommendation client - for testing purposes
        /// </summary>
        /// <param name="client">Underlying http client (mock, ...)</param>
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
        public Task<RecommendedContentItem[]> SearchAsync(SearchRequest request)
        {
            return PostAsync<RecommendedContentItem[]>($"{SearchEndpointRoutePrefix}/items", JsonConvert.SerializeObject(request));
        }

        /// <inheritdoc />
        public Task CreateVisitorAsync(string visitId, VisitorDetails visitor)
        {
            if (string.IsNullOrEmpty(visitId))
                throw new ArgumentException("Visit Id has to be set.", nameof(visitId));

            if (visitor == null)
                throw new ArgumentException("Visitor details has to be set.", nameof(visitor));

            var request = new CreateVisitorRequest
            {
                VisitId = visitId,
                Visitor = visitor
            };

            return PostAsync<object>($"{TrackingEndpointRoutePrefix}/visitor", JsonConvert.SerializeObject(request));
        }

        /// <inheritdoc />
        public Task TrackVisitAsync(string visitId, string currentItemCodename)
        {
            if (string.IsNullOrEmpty(visitId))
                throw new ArgumentException("Visit Id has to be set.", nameof(visitId));

            if (string.IsNullOrEmpty(currentItemCodename))
                throw new ArgumentException("Codename of the currently viewed item has to be set.", nameof(currentItemCodename));

            var data = new VisitData { VisitId = visitId, CurrentItemCodename = currentItemCodename };

            return PostAsync<object>($"{TrackingEndpointRoutePrefix}/visit", JsonConvert.SerializeObject(data));
        }

        /// <inheritdoc />
        public Task TrackConversionAsync(string visitId, string currentItemCodename)
        {
            if (string.IsNullOrEmpty(visitId))
                throw new ArgumentException("Visit Id has to be set.", nameof(visitId));

            if (string.IsNullOrEmpty(currentItemCodename))
                throw new ArgumentException("Codename of the currently viewed item has to be set.", nameof(currentItemCodename));

            var data = new VisitData { VisitId = visitId, CurrentItemCodename = currentItemCodename};

            return PostAsync<object>($"{TrackingEndpointRoutePrefix}/conversion",JsonConvert.SerializeObject(data));
        }

        /// <inheritdoc />
        public Task TrackPortionViewAsync(string visitId, string currentItemCodename, int portionPercentage)
        {
            if (string.IsNullOrEmpty(visitId))
                throw new ArgumentException("Visit Id has to be set.", nameof(visitId));

            if (string.IsNullOrEmpty(currentItemCodename))
                throw new ArgumentException("Codename of the currently viewed item has to be set.", nameof(currentItemCodename));

            var data = new PortionViewData
                {VisitId = visitId, CurrentItemCodename = currentItemCodename, PortionPercentage = portionPercentage};

            return PostAsync<object>($"{TrackingEndpointRoutePrefix}/portion", JsonConvert.SerializeObject(data));
        }
    }
}

