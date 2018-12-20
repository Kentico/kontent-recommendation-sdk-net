using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Recommender.SDK.Shared
{
    public abstract partial class RecommendationClientBase
    {
        protected const string TrackingApiRoutePrefix = "api/Track";

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
    }
}
