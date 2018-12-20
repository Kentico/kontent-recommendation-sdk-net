using System;
using System.Linq;
using System.Web;
using KenticoCloud.Recommender.SDK.Shared;

namespace KenticoCloud.Recommender.SDK.MVC
{
    public static class HttpRequestBaseExtensions
    {
        public static TrackingCookieModel GetCurrentTrackingCookie(this HttpRequestBase request, HttpResponseBase response, string projectId)
        {
            var cookieKey = request.Cookies.AllKeys.FirstOrDefault(x => x.StartsWith("k_e_id"));
            return !string.IsNullOrEmpty(cookieKey)
                ? CookieHelpers.ParseTrackingCookie(request.Cookies[cookieKey].Value)
                : request.GetNewTrackingCookie(response, projectId);
        }

        public static TrackingCookieModel GetNewTrackingCookie(this HttpRequestBase request, HttpResponseBase response, string projectId)
        {
            var cookie = CookieHelpers.CreateCookie(request.Headers["User-Agent"], request.Headers["Host"]);
            response.Cookies.Add(new HttpCookie(TrackingCookieModel.Name, cookie.ToString())
            {
                Expires = DateTimeOffset.Now.AddYears(1).DateTime,
                Domain = $".{request.Headers["Host"]}"
            });
            return cookie;
        }

        public static CallerInfo GetCallerInfo(this HttpRequestBase request, string uid, string sid, bool sessionBased)
        {
            var ipDirty = request.Headers.AllKeys.FirstOrDefault(key => key == "x-forwarded-for") ??
                          request.UserHostAddress;
            
            return new CallerInfo
            {
                Ip = CookieHelpers.ParseIp(ipDirty),
                Referer = request.Headers["Referer"],
                VisitId = sessionBased ? sid : uid
            };
        }
    }
}
