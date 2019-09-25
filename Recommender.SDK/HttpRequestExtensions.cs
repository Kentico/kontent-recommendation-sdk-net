using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace KenticoKontent.Recommender
{
    public static class HttpRequestExtensions
    {
        public static TrackingCookieModel GetCurrentTrackingCookie(this HttpRequest request, HttpResponse response, string projectId)
        {
            var cookieKey = request.Cookies.Keys.FirstOrDefault(x => x.StartsWith(TrackingCookieModel.Name));
            return !string.IsNullOrEmpty(cookieKey)
                ? Helpers.ParseTrackingCookie(request.Cookies[cookieKey])
                : request.GetNewTrackingCookie(response, projectId);
        }

        public static TrackingCookieModel GetNewTrackingCookie(this HttpRequest request, HttpResponse response, string projectId)
        {
            var cookie = Helpers.CreateCookie(request.Headers["User-Agent"], request.Headers["Host"]);
            response.Cookies.Append(TrackingCookieModel.Name, cookie.ToString(), new CookieOptions { Expires = DateTimeOffset.Now.AddYears(1), Domain = $".{request.Headers["Host"]}"});
            return cookie;
        }

        public static CallerInfo GetCallerInfo(this HttpRequest request, string uid, string sid, bool sessionBased)
        {
            var dirtyIp = request.Headers["x-forwarded-for"].FirstOrDefault() ??
                          request.HttpContext.Connection.RemoteIpAddress.ToString();
            
            return new CallerInfo
            {
                Ip = Helpers.ParseIp(dirtyIp),
                Referer = request.Headers["Referer"],
                VisitId = sessionBased ? sid : uid
            };
        }
    }
}
