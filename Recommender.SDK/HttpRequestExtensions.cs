using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace KenticoCloud.Recommender.SDK
{
    public static class HttpRequestExtensions
    {
        private static readonly Random Rnd = new Random();

        public static TrackingCookieModel GetCurrentTrackingCookie(this HttpRequest request, HttpResponse response, string projectId)
        {
            var cookieKey = request.Cookies.Keys.FirstOrDefault(x => x.StartsWith("k_e_id"));
            return !string.IsNullOrEmpty(cookieKey)
                ? ParseTrackingCookie(request.Cookies[cookieKey])
                : request.GetNewTrackingCookie(response, projectId);
        }

        private static TrackingCookieModel ParseTrackingCookie(string cookieVal)
        {
            var values = cookieVal.Split('.');
            if(values.Length != 3)
                return new TrackingCookieModel
                {
                    Uid = values[0]
                };

            return new TrackingCookieModel
            {
                Uid = values[0],
                Sid = values[2],
                SessionExpiration = int.Parse(values[1])
            };
        }

        public static TrackingCookieModel GetNewTrackingCookie(this HttpRequest request, HttpResponse response, string projectId)
        {
            var uidGenerationString = $"{request.Headers["User-Agent"]}{DateTimeOffset.UtcNow.Ticks}{Rnd.Next()}{request.Headers["Host"]}uid";
            var uid = Sha1HashStringForUtf8String(uidGenerationString).Substring(0, 16);

            var sidGenerationString = $"{request.Headers["User-Agent"]}{DateTimeOffset.UtcNow.Ticks}{Rnd.Next()}{request.Headers["Host"]}sid";
            var sid = Sha1HashStringForUtf8String(sidGenerationString).Substring(0, 16);

            var cookie = new TrackingCookieModel
            {
                Uid = uid,
                Sid = sid,
                SessionExpiration = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds
            };
            
            response.Cookies.Append(TrackingCookieModel.Name, cookie.ToString(), new CookieOptions { Expires = DateTimeOffset.Now.AddYears(1), Domain = $".{request.Headers["Host"]}"});
            return cookie;
        }

        public static CallerInfo GetCallerInfo(this HttpRequest request, string uid, string sid, bool sessionBased)
        {
            return new CallerInfo
            {
                Ip = ParseIp(request),
                Referer = request.Headers["Referer"],
                VisitId = sessionBased ? sid : uid
            };
        }

        private static string ParseIp(HttpRequest request)
        {
            var ipDirty = request.Headers["x-forwarded-for"].FirstOrDefault() ?? request.HttpContext.Connection.RemoteIpAddress.ToString();
            var index = ipDirty.IndexOf(":", StringComparison.Ordinal);
            if (index > 0)
                return ipDirty.Substring(0, index);
            
            return ipDirty;
        }

        private static string Sha1HashStringForUtf8String(string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);

            var sha1 = SHA1.Create();
            var hashBytes = sha1.ComputeHash(bytes);

            return HexStringFromBytes(hashBytes);
        }

        private static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }
    }
}
