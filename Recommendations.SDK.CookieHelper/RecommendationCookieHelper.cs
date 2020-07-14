using System;
using System.Linq;
using Kentico.Kontent.Recommendations.Models;
using Microsoft.AspNetCore.Http;

namespace Kentico.Kontent.Recommendations.CookieHelper
{
    public static class RecommendationCookieHelper
    {
        /// <summary>
        /// Returns current recommender tracking cookie containing the visitId
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static TrackingCookieModel GetRecommendationTrackingCookie(HttpRequest request)
        {
            var cookieKey = request.Cookies.Keys.FirstOrDefault(x => x.StartsWith(TrackingCookieModel.Name));
            return !string.IsNullOrEmpty(cookieKey)
                ? Helpers.ParseTrackingCookie(request.Cookies[cookieKey])
                : null;
        }

        /// <summary>
        /// Creates a new tracking cookie with a unique visitId
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="expiration">cookie expiration (if empty, 1year will be set)</param>
        /// <param name="hostname">hostname of the website the cookie will be hosted on (otherwise will be filled out from the Host header) It's suggested for custom domains where controllers might call the backend address and not the dns name making the cookie non functional.</param>
        /// <returns></returns>
        public static TrackingCookieModel SetNewRecommendationTrackingCookie(HttpRequest request, HttpResponse response, TimeSpan? expiration = null, string hostname = null)
        {
            if (expiration == null) expiration = TimeSpan.FromDays(365);
            if (string.IsNullOrEmpty(hostname)) hostname = request.Headers["Host"].ToString()?.Split(':')[0];

            var cookie = Helpers.CreateCookie(request.Headers["User-Agent"], hostname);
            response.Cookies.Append(TrackingCookieModel.Name, cookie.ToString(), new CookieOptions { Expires = DateTimeOffset.Now.Add(expiration.Value), Domain = $"{hostname}"});
            return cookie;
        }

        /// <summary>
        /// Extracts and cleans IP and Referer from the request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static VisitorDetails GetVisitorDetails(HttpRequest request)
        {
            var dirtyIp = request.Headers["x-forwarded-for"].FirstOrDefault() ??
                          request.HttpContext.Connection.RemoteIpAddress.ToString();
            
            return new VisitorDetails
            {
                Ip = Helpers.ParseIp(dirtyIp),
                Referer = request.Headers["Referer"]
            };
        }
    }
}
