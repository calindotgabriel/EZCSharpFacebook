# EZCSharpFacebook
Light &amp; easy to use wrapper over the .NET Facebook SDK.

Before you start using this library, I strongly suggest you read the documentation from [FacebookSDk.Net](http://http://facebooksdk.net/).

### Download via NuGet
```
  PM> Install-Package EZCSharpFacebook
```
### Sample usage

```c#
var facebookManager = new FacebookManager("YOUR_ACCESS_TOKEN");
var userInfo = facebookManager.GetUserInfo(); // get info for current user 
var likedPages = facebookManager.GetAllLikedPages("me"); // get liked pages for a NODE id
var posts = facebookManager.GetAllPosts("me"); // get all posts for a NODE id
var friends = facebookManager.GetUserFriends("me"); // get all friends for a NODE id 
var longLivedAccessToken = facebookManager.GetLongLivedAccessToken("CLIENT_ID", "CLIENT_SECRET"); // gets a long lived access token, usable 60 days
```

> This version is still very beta, if you have time to improve the codebase, feel free to contribute.
