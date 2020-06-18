using System.Collections.Generic;

namespace Kentico.Kontent.Recommender
{
    /// <summary>
    /// Details about current visitor
    /// </summary>
    public class VisitorDetails
    {
        /// <summary>
        /// Source of the visit
        /// </summary>
        public string Referer { get; set; }
        
        /// <summary>
        /// OPTIONAL: used for autonomous enrichment of user data with location data 
        /// (only works if the location enrichment is setup in the recommendation project settings)
        /// IMPORTANT: IP is not being saved, it is only used once for calling a location API
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Optionally, you can provide the user location details yourself
        /// </summary>
        public LocationDetails Location { get; set; }

        /// <summary>
        /// You can enrich user profile by any key-value property you wish
        /// </summary>
        public Dictionary<string, string> Custom { get; set; }
    }
}