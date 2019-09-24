using System;
using System.Linq;
using System.Web;

namespace Kentico.Kontent.Recommender.Recommender.MVC
{
    public static class HttpRequestBaseExtensions
    {
        public static TrackingCookieModel GetCurrentTrackingCookie(this HttpRequestBase request, HttpResponseBase response, string projectId)
        {
            var cookieKey = request.Cookies.AllKeys.FirstOrDefault(x => x.StartsWith(TrackingCookieModel.Name));
            return !string.IsNullOrEmpty(cookieKey)
                ? Helpers.ParseTrackingCookie(request.Cookies[cookieKey].Value)
                : request.GetNewTrackingCookie(response, projectId);
        }

        public static TrackingCookieModel GetNewTrackingCookie(this HttpRequestBase request, HttpResponseBase response, string projectId)
        {
            var cookie = Helpers.CreateCookie(request.Headers["User-Agent"], request.Headers["Host"]);
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
                Ip = Helpers.ParseIp(ipDirty),
                Referer = request.Headers["Referer"],
                VisitId = sessionBased ? sid : uid
            };
        }
    }
}
