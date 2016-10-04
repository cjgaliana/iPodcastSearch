# iPodcastSearch
iTunes Podcast Search library


## Usage
Searching iTunes 
```csharp
var results = await this.client.SearchPodcastsAsync("Code on the rocks");
```
From FeedUrl
```csharp
var podcast = await this.client.GetPodcastFromFeedUrlAsyc(feedUrl);
```
