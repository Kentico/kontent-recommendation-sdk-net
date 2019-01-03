using System;
using Xunit;
using FluentAssertions;
using KenticoCloud.Recommender.SDK.Shared;

namespace Recommender.SDK.Tests
{
    public class CookieHelpersTests
    {
        [Fact]
        public void ParseTrackingCookie_CorrectCookie_ReturnsParsedCookie()
        {
            var cookieBody = "uid.1.sid";
            var expectedModel = new TrackingCookieModel
            {
                Uid = "uid",
                Sid = "sid",
                SessionExpiration = 1
            };

            var cookieModel = CookieHelpers.ParseTrackingCookie(cookieBody);

            cookieModel.Should().BeEquivalentTo(expectedModel);
        }

        [Theory]
        [InlineData("google.com", "google.com")]
        [InlineData("test.com:12345", "test.com")]
        [InlineData("www.test.com:12345", "www.test.com")]
        public void ParseIp_ParsesIpCorrectly(string source, string expected)
        {
            var parsed = CookieHelpers.ParseIp(source);
            parsed.Should().Be(expected);
        }

        [Fact]
        public void CreateCookie_CreatesCookieCorrectly()
        {
            var cookie = CookieHelpers.CreateCookie("userAgent", "host");
            var exp = (int) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            cookie.Should().NotBeNull();
            cookie.Uid.Should().NotBeNullOrWhiteSpace();
            cookie.Sid.Should().NotBeNullOrWhiteSpace();
            cookie.Uid.Length.Should().Be(16);
            cookie.Sid.Length.Should().Be(16);
            cookie.SessionExpiration.Should().BeLessOrEqualTo(exp);
        }
    }
}
