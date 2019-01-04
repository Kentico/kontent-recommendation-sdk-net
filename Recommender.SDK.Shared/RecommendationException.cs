using System;
using System.Net;

namespace KenticoCloud.Recommender
{
    public class RecommendationException : Exception
    {
        public RecommendationException(HttpStatusCode statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }

        /// <summary>
        ///     HTTP status code.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        ///     Detailed message from the API.
        /// </summary>
        public override string Message { get; }
    }
}