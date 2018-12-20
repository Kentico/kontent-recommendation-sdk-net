using System;
using System.Security.Cryptography;
using System.Text;

namespace KenticoCloud.Recommender.SDK.Shared
{
    public static class CookieHelpers
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

    }
}
