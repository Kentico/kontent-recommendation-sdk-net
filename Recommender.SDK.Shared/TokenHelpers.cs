using JWT;
using JWT.Serializers;

namespace Recommender.SDK.Shared
{
    public class TokenHelpers
    {
        public static string GetProjectIdFromToken(string token)
        {
            var serializer = new JsonNetSerializer();
            var decoder = new JwtDecoder(serializer, new JwtValidator(serializer, new UtcDateTimeProvider()), new JwtBase64UrlEncoder());
            var decodedToken = decoder.DecodeToObject(token);
            return decodedToken["pid"].ToString();
        }
    }
}
