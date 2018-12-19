using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace KenticoCloud.Recommender.SDK
{
    public partial class RecommendationClient
    {
        private const string TrackingApiRoutePrefix = "api/Track";

        public Task TrackConversionAsync(
            string visitId, 
            string codename, 
            CallerInfo callerInfo = null)
        {
            if (string.IsNullOrEmpty(visitId))
                throw new ArgumentException("Visit Id has to be set.", nameof(visitId));

            if(string.IsNullOrEmpty(codename))
                throw new ArgumentException("Codename has to be set.", nameof(codename));

            return PostAsync<object>($"{TrackingApiRoutePrefix}/Conversion?visitId={visitId}&contentItemId={codename}",
                JsonConvert.SerializeObject(callerInfo));
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
            string visitId, 
            string codename,
            int portionPercentage,
            CallerInfo callerInfo = null)
        {
            if (string.IsNullOrEmpty(visitId))
                throw new ArgumentException("Visit Id has to be set.", nameof(visitId));

            if(string.IsNullOrEmpty(codename))
                throw new ArgumentException("Codename has to be set.", nameof(codename));

            return PostAsync<object>($"{TrackingApiRoutePrefix}/PortionView?visitId={visitId}&contentItemId={codename}&portionPercentage={portionPercentage}",
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
            string visitId, 
            string codename, 
            CallerInfo callerInfo = null)
        {
            if (string.IsNullOrEmpty(visitId))
                throw new ArgumentException("Visit Id has to be set.", nameof(visitId));

            if(string.IsNullOrEmpty(codename))
                throw new ArgumentException("Codename has to be set.", nameof(codename));

            return PostAsync<object>($"{TrackingApiRoutePrefix}/Visit?visitId={visitId}&contentItemId={codename}",
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
