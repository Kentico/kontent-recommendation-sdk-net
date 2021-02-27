## Kentico Kontent Recommendation .NET SDK
[![Build & Test](https://github.com/Kentico/kontent-recommendations-net/actions/workflows/integrate.yml/badge.svg)](https://github.com/Kentico/kontent-recommendations-net/actions/workflows/integrate.yml)
[![Stack Overflow](https://img.shields.io/badge/Stack%20Overflow-ASK%20NOW-FE7A16.svg?logo=stackoverflow&logoColor=white)](https://stackoverflow.com/tags/kentico-kontent)

| Package  | Downloads |
|:-------------:| :-------------:|
 [![NuGet](https://img.shields.io/nuget/v/Kentico.Kontent.Recommendations.svg)](https://www.nuget.org/packages/Kentico.Kontent.Recommendations) | [![NuGet](https://img.shields.io/nuget/dt/Kentico.Kontent.Recommendations.svg)](https://www.nuget.org/packages/Kentico.Kontent.Recommendations) |

# Summary
Forget manually browsing, updating and testing your related content - create personalized content recommendations using our integrated AI-based recommendation engine fast and simple.

# Getting started
Installation via Package Manager Console in Visual Studio:

```powershell
PM> Install-Package Kentico.Kontent.Recommendations 
```

Installation via .NET CLI:

```console
> dotnet add package Kentico.Kontent.Recommendations 
```

## Prerequisities
* Kentico Kontent Account & Project
* Project registered in the KC Content Recommendation Engine

# Getting Recommendations
To recommend content from your Kentico Kontent projects, you'll be using an implementation of the `IRecommendationClient` interface. This is the main interface of the SDK.

#### Use dependency injection (ideal for ASP.NET Core web apps)

## Basic scenario for getting recommendations
```csharp
using Kentico.Kontent.Recommendations;
using Kentico.Kontent.Recommendations.Models;

//Creates an instance of the recommendation client
var recommendationClient = new RecommendationClient(accessToken: "recommendation_token", timeoutSeconds: 5);

//Creates a new recommendation request
var recommendationRequest = new RecommendationRequest {
        VisitId = "clientId",
        CurrentItemCodename = "current_codename",
        ResponseLimit = 2,
        RequestedTypeCodename = "article"
};

//The method returns a requested number of recommended content items (codenames)
RecommendedContentItem[] recommendedArticles = await recommendationClient.GetRecommendationsAsync(recommendationRequest);
```

## Working with visitor data
```csharp
using Kentico.Kontent.Recommendations.Models;

//Manually inserted visitor details
var visitor = new VisitorDetails {
    //Source of the visit
    Referer = "google", 
    //Your own custom data (you can create a custom profile using this property)
    Custom = new Dictionary<string, string> 
    {
        //e.g. Persona property
        {"Persona", "Manager"} 
    },
   //Geo-location information about the visitor
    Location = new LocationDetails 
    {
        City = "Brno",
        Country = "Czechia",
        Timezone = "CET"
    }
}

var recommendationRequest = new RecommendationRequest {
        VisitId = "clientId",
        CurrentItemCodename = "current_codename",
        ResponseLimit = 2,
        RequestedTypeCodename = "article",
       // Adding the visitor data into the recommendation request
        Visitor = visitor 
};

// ALTERNATIVELY you can use the CreateVisitorAsync method to take control of visitor creation
await recommendationClient.CreateVisitorAsync("clientId", visitor);
```

# Searching content
In addition to recommending content based on other content, the system offers an option to search for items directly. The results are based on the full-text matching of a search query and can be filtered by content type. Search method returns the same type of result as the recommendation method does, i.e. array of objects with a codename property containing codename of the found item. 

```csharp
using Kentico.Kontent.Recommendations;
using Kentico.Kontent.Recommendations.Models;

//Creates an instance of the recommendation client
var recommendationClient = new RecommendationClient(accessToken: "recommendation_token", timeoutSeconds: 5);

//Creates a new recommendation request
var searchRequest = new SearchRequest {
        VisitId = "clientId",
        Query = "this is a search query",
        ResultCount = 10,
        RequestedTypeCodename = "article"
};

//The method returns a requested number of recommended content items (codenames)
RecommendedContentItem[] foundArticles = await recommendationClient.SearchAsync(searchRequest);
```

# CookieHelper 
This package provides a helper class, that let's you setup a very simple yet effective way to distinguish your visitors and recover additional details about them. It works with the .net core _Request_ and _Response_ objects and creates uses **cookies to track visitors**. 

Alternatively, the helper let's you reuse **Google Analytics** clientId to identify the visitor inside of the recommendation system.

| Version        | Package  | Downloads |
| ------------- |:-------------:| :-------------:|
| netcore2.0+   |      [![NuGet](https://img.shields.io/nuget/v/Kentico.Kontent.Recommendations.CookieHelper.svg)](https://www.nuget.org/packages/Kentico.Kontent.Recommendations.CookieHelper) | [![NuGet](https://img.shields.io/nuget/dt/Kentico.Kontent.Recommendations.CookieHelper.svg)](https://www.nuget.org/packages/Kentico.Kontent.Recommendations.CookieHelper) |


Installation via Package Manager Console in Visual Studio:

```powershell
PM> Install-Package Kentico.Kontent.Recommendations.CookieHelper 
```

Installation via .NET CLI:

```console
> dotnet add package Kentico.Kontent.Recommendations.CookieHelper 
```

> :memo: Keep in mind, that you may be legally bound to disclose this information to your visitors and let them disable tracking cookies when using this package.


> :warning: Check if your received *Request* object contains cookies when processing it on your backend. Cross domain calls will block cookies by default, as well as multiple popular javascript http clients won't send them by without proper settings in place ([axios example](https://stackoverflow.com/questions/43002444/make-axios-send-cookies-in-its-requests-automatically)) if you are using ajax to retreive recommendations asynchronously. 
**Check your settings if you are using different domains for your frontend and backend and if you are using ajax to retreive recommendations asynchronously!**


```csharp
using Kentico.Kontent.Recommendations.CookieHelper;

//Get the tracking cookie either from google analytics or from our custom tracking cookie or create a newone
var cookie = RecommendationCookieHelper.GetGoogleTrackingCookie(Request) ?? RecommendationCookieHelper.GetRecommendationTrackingCookie(Request) ??           RecommendationCookieHelper.SetNewRecommendationTrackingCookie(Request, Response, TimeSpan.FromDays(60), "mydomain.com");


//Generated visitId we can use in the recommendation request
var visitId = trackingCookie.VisitId; 

//The helper also let's you extract initialized VisitorDetails object
//It fills out the referrer and IP address
var visitor = RecommendationCookieHelper.GetVisitorDetails(Request); 
```

# Tracking visitor-content interactions
Simply requesting a recommendation will track a default interaction between the visitor and content in a form of a simple visit, however, you can also track more granular visitor–content interactions by yourself via the SDK. 

```csharp
using Kentico.Kontent.Recommendations;

var recommendationClient = new RecommendationClient(accessToken: "recommendation_token", timeoutSeconds: 5);

//Track content item visit -> visitor with visitId just visited content with itemCodename codename
await recommendationClient.TrackVisitAsync("visitId", "itemCodename");

//Track conversion -> visitor  just performed a conversion
await recommendationClient.TrackConversionAsync("visitId","itemCodename");

//Track portion view -> visitor just read 10% of content item
await recommendationClient.TrackPortionViewAsync("visitId", "itemCodename", 10);
```

# Advanced recommendations
You are able to use [Filtering](https://docs.recombee.com/reql_filtering_and_boosting.html#reql-filtering) and [Boosting](https://docs.recombee.com/reql_filtering_and_boosting.html#reql-boosting) in order to alter the outcome of your recommendations. The syntax of filters and boosters is written in [ReQL](https://docs.recombee.com/reql.html). The set of properties you can filter on is displayed inside of your Kontent App within the Recommendations module.

```csharp
using Kentico.Kontent.Recommendations;
using Kentico.Kontent.Recommendations.Models;

var recommendationSettings = new RecommendationSettings
{
   // recommend only items for specific persona
   Filter = $"\"persona=developer\" in 'properties'",
   
   // prefer articles from last 30 days
   Booster = $"if 'lastupdated' >= now() - {TimeSpan.FromDays(30).Milliseconds} then 2 else 1"
}


var recommendationClient = new RecommendationClient(accessToken: "recommendation_token", timeoutSeconds: 5);

//Creates a new recommendation request
var recommendationRequest = new RecommendationRequest {
        VisitId = "clientId",
        CurrentItemCodename = "current_codename",
        ResponseLimit = 2,
        RequestedTypeCodename = "article",
        RecommendationSettings = recommendationSettings;
};

```



# Further information
* [Relevant documentation](https://docs.kontent.ai/tutorials/develop-apps/build-strong-foundation/personalize-content-with-ai)
* [Recommendation API Reference](https://docs.kontent.ai/reference/recommendation-api)

# Get involved
Check out the [contributing](CONTRIBUTING.md) page to see the best places to file issues, start discussions, and begin contributing.

