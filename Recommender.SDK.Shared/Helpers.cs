using System;
using System.Security.Cryptography;
using System.Text;
using JWT;
using JWT.Serializers;

namespace KenticoCloud.Recommender
{
    public static class Helpers
    {
        private static readonly Random Rnd = new Random();

        public static TrackingCookieModel ParseTrackingCookie(string cookieVal)
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

        public static string Sha1HashStringForUtf8String(string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);

            var sha1 = SHA1.Create();
            var hashBytes = sha1.ComputeHash(bytes);

            return HexStringFromBytes(hashBytes);
        }

        public static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }

        public static string ParseIp(string ipDirty)
        {
            var index = ipDirty.IndexOf(":", StringComparison.Ordinal);
            if (index > 0)
                return ipDirty.Substring(0, index);
            
            return ipDirty;
        }

        public static TrackingCookieModel CreateCookie(string userAgent, string host)
        {
            var uidGenerationString = $"{userAgent}{DateTimeOffset.UtcNow.Ticks}{Rnd.Next()}{host}uid";
            var uid = Sha1HashStringForUtf8String(uidGenerationString).Substring(0, 16);

            var sidGenerationString = $"{userAgent}{DateTimeOffset.UtcNow.Ticks}{Rnd.Next()}{host}sid";
            var sid = Sha1HashStringForUtf8String(sidGenerationString).Substring(0, 16);

            return new TrackingCookieModel
            {
                Uid = uid,
                Sid = sid,
                SessionExpiration = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds
            };
        }

        public static string GetProjectIdFromToken(string token)
        {
            var serializer = new JsonNetSerializer();
            var decoder = new JwtDecoder(serializer, new JwtValidator(serializer, new UtcDateTimeProvider()), new JwtBase64UrlEncoder());
            var decodedToken = decoder.DecodeToObject(token);
            return decodedToken["pid"].ToString();
        }

        public static string GetQueryFromRequest(RecommendationRequest request, string prefix)
        {
            var queryString =
                new StringBuilder($"{prefix}/Items?currentItemId={request.Codename}&visitId={request.VisitId}&limit={request.Limit}");

            if (!string.IsNullOrWhiteSpace(request.ContentTypeName) && request.ContentTypeName != "*")
                queryString.Append($"&contentTypeName={request.ContentTypeName}");

            if(!string.IsNullOrWhiteSpace(request.FilterQuery))
                queryString.Append($"&filterQuery={request.FilterQuery}");

            if(!string.IsNullOrWhiteSpace(request.BoosterQuery))
                queryString.Append($"&boosterQuery={request.BoosterQuery}");

            if(!string.IsNullOrWhiteSpace(request.SourceApp))
                queryString.Append($"&sourceApp={request.SourceApp}");

            if (request.SeparateTracking)
                queryString.Append($"&separateTracking=true");

            return queryString.ToString();
        }

    }
}
