using System;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using KenticoCloud.Recommender.SDK.Shared;

namespace Recommender.SDK.Tests
{
    public class HelpersTests
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

            var cookieModel = Helpers.ParseTrackingCookie(cookieBody);

            cookieModel.Should().BeEquivalentTo(expectedModel);
        }

        [Theory]
        [InlineData("google.com", "google.com")]
        [InlineData("test.com:12345", "test.com")]
        [InlineData("www.test.com:12345", "www.test.com")]
        public void ParseIp_ParsesIpCorrectly(string source, string expected)
        {
            var parsed = Helpers.ParseIp(source);
            parsed.Should().Be(expected);
        }

        [Fact]
        public void CreateCookie_CreatesCookieCorrectly()
        {
            var cookie = Helpers.CreateCookie("userAgent", "host");
            var exp = (int) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            cookie.Should().NotBeNull();
            cookie.Uid.Should().NotBeNullOrWhiteSpace();
            cookie.Sid.Should().NotBeNullOrWhiteSpace();
            cookie.Uid.Length.Should().Be(16);
            cookie.Sid.Length.Should().Be(16);
            cookie.SessionExpiration.Should().BeLessOrEqualTo(exp);
        }

        [Theory]
        [MemberData(nameof(RequestData))]   
        public void GetQueryFromRequest_CreatesCorrectQueryString(RecommendationRequest request, string expectedQueryString)
        {
            var queryString = Helpers.GetQueryFromRequest(request, "prefix");
            queryString.Should().Be(expectedQueryString);
        }

        public static IEnumerable<object[]> RequestData => new List<object[]>
        {
            new object[] {new RecommendationRequest
            {
                Codename = "codename",
                ContentTypeName = "contentTypeName",
                BoosterQuery = "boosterQuery",
                FilterQuery = "filterQuery",
                Limit = 1,
                SourceApp = "sourceApp",
                VisitId = "visitId",
                SeparateTracking = true
            }, "prefix/Items?currentItemId=codename&visitId=visitId&limit=1&contentTypeName=contentTypeName&filterQuery=filterQuery&boosterQuery=boosterQuery&sourceApp=sourceApp&separateTracking=true"},
            new object[] {new RecommendationRequest
            {
                Codename = "codename",
                ContentTypeName = "*",
                Limit = 3,
                VisitId = "visitId"
            }, "prefix/Items?currentItemId=codename&visitId=visitId&limit=3"},
            new object[] {new RecommendationRequest
            {
                Codename = "codename",
                ContentTypeName = "contentTypeName",
                Limit = 2,
                VisitId="visitId",
                SeparateTracking = true,
                SourceApp = "sourceApp"
            }, "prefix/Items?currentItemId=codename&visitId=visitId&limit=2&contentTypeName=contentTypeName&sourceApp=sourceApp&separateTracking=true"}
        };
    }
}
