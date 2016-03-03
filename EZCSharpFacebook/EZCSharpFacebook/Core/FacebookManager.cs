// Created by Călin Gabriel
// at 20:45 on 27/02/2016.
//  

using System;
using System.Collections.Generic;
using System.Linq;
using EZCSharpFacebook.Models;
using Facebook;

namespace EZCSharpFacebook.Core
{
    /**
     * Core class of EZCSharpFacebook.
     * You can access nodes, edges and fields as static constants.
     */
    public class FacebookManager
    {
        private FacebookClient _fbClient;
        private string _accesToken;


        /** 
         * Nodes & Edges
         */
        public const string ME_NODE = "me";
        private const string FRIEND_NODE = "252098181664574";
        public const string FRIENDS_EDGE = "friends";
        public const string LIKES_EDGE = "likes";

        /**
         * Pagination
         */
        public const string PAGING = "paging";
        public const string PAGING_TOKEN = "_paging_token";
        public const string NEXT = "next";
        public const string UNTIL = "until";

        /**
         * Fields
         */
        public const string DATA = "data";
        public const string ID = "id";
        public const string NAME = "name";
        public const string FIRST_NAME = "first_name";
        public const string LAST_NAME = "last_name";
        public const string GENDER = "gender";
        public const string CREATED_TIME = "created_time";
        public const string ABOUT = "about";
        public const string AGE_RANGE = "age_range";
        public const string BIO = "bio";
        public const string BIRTHDAY = "birthday";
        public const string DEVICES = "devices";
        public const string EDUCATION = "education";
        public const string EMAIL = "email";
        public const string FAVORITE_ATHLETES = "favorite_athletes";
        public const string AGE_RANGE_MIN = "min";
        public const string AGE_RANGE_MAX = "max";
        public const string DEVICES_OS = "os";
        public const string DEVICES_HARDWARE = "hardware";
        public const string SCHOOL = "school";
        public const string CONCENTRATION = "concentration";
        public const string LIKES = "likes";
        public const string CURSORS = "cursors";
        public const string AFTER = "after";
        public const string POSTS_EDGE = "posts";
        public const string MESSAGE = "message";
        public const string DESCRIPTION = "description";
        public const string LINK = "link";
        public const string FROM = "from";
        public const string STATUS_TYPE = "status_type";
        public const string ACCESS_TOKEN = "access_token";
        private const string SHARES = "shares";
        private const string COUNT = "count";
        private const string NODE_OAUTH = "oauth";
        private const string EDGE_ACCESS_TOKEN = "access_token";
        private const string CLIENT_ID = "client_id";
        private const string CLIENT_SECRET = "client_secret";
        private const string FB_EXCHANGE_TOKEN = "fb_exchange_token";
        private const string GRANT_TYPE = "grant_type";



        public FacebookManager(string accessToken)
        {
            this._accesToken = accessToken;
            this._fbClient = new FacebookClient(accessToken);
        }

        /**
         * Gets user's friends ( who granted our app access )
         */
        public List<User> GetUserFriends(string userNode)
        {
            var builder = new FBGraphQueryBuilder(userNode, FRIENDS_EDGE);
            builder
                .RegisterParam(FBGraphQueryBuilder.FIELDS);
            builder
                .Field(ID)
                .Field(NAME);
            var query = builder.Build();
            var result = _fbClient.Get(query) as IDictionary<string, object>;
            var extractor = new FieldExtractor(result);
            return extractor.ExtractArray(DATA).Select(field => new User()
            {
                FacebookID = field.Extract(ID),
                Name = field.Extract(NAME)
            }).ToList();
        }

        /**
         * Gets information about user's profile.
         */
        public User GetUserInfo()
        {
            var builder = new FBGraphQueryBuilder(ME_NODE);
            builder.RegisterParam(FBGraphQueryBuilder.FIELDS);
            var query = builder
                            .Field(ID)
                            .Field(FIRST_NAME)
                            .Field(LAST_NAME)
                            .Field(NAME)
                //                            .Field(ABOUT)
                //                            .Field(AGE_RANGE)
                //                            .Field(BIO)
                //                            .Field(BIRTHDAY)
                //                            .Field(LIKES)
                //                            .Field(DEVICES)
                //                            .Field(EDUCATION)
                //                            .Field(EMAIL)
                //                            .Field(FAVORITE_ATHLETES)
                //                            .Field(GENDER)
                            .Build();

            IDictionary<string, object> result = _fbClient.Get(query) as IDictionary<string, object>;
            var extractor = new FieldExtractor(result);

            var userInfo = new User();
            userInfo.FacebookID = extractor.Extract(ID);
            userInfo.FirstName = extractor.Extract(FIRST_NAME);
            userInfo.LastName = extractor.Extract(LAST_NAME);
            userInfo.Name = extractor.Extract(NAME);
            userInfo.About = extractor.Extract(ABOUT);
            userInfo.AgeRangeMin = extractor.Advance(AGE_RANGE).Extract(AGE_RANGE_MIN);
            userInfo.AgeRangeMax = extractor.Advance(AGE_RANGE).Extract(AGE_RANGE_MAX);
            userInfo.Bio = extractor.Extract(BIO);
            userInfo.Birthday = extractor.Extract(BIRTHDAY);
            userInfo.Gender = extractor.Extract(GENDER);

            return userInfo;
        }


        public List<Page> GetAllLikedPages(string nodeID)
        {
            var container = new List<Page>();

            var builder = new FBGraphQueryBuilder(nodeID, LIKES_EDGE);
            builder.RegisterParam(FBGraphQueryBuilder.FIELDS)
                   .RegisterParam(FBGraphQueryBuilder.LIMIT)
                   .RegisterParam(FBGraphQueryBuilder.AFTER);
            var queryBuilder = builder
                .Field(ID)
                .Field(NAME)
                .Field(ABOUT)
                .Field(DESCRIPTION)
                .Field(LINK)
                .Field(CREATED_TIME)
                .Limit(100);
            var result = _fbClient.Get(queryBuilder.Build()) as IDictionary<string, object>;

            var extractor = new FieldExtractor(result);
            while (extractor.HasMoreResults())
            {
                var likes = extractor.ExtractArray(DATA);
                container.AddRange(likes.Select(field => new Page()
                {
                    FacebookID = field.Extract(ID),
                    Name = field.Extract(NAME),
                    About = field.Extract(ABOUT),
                    Description = field.Extract(DESCRIPTION),
                    Link = field.Extract(LINK),
                    CreatedTime = field.Extract(CREATED_TIME)
                }));
                queryBuilder.After(extractor.GetAfter());
                result = _fbClient.Get(queryBuilder.Build()) as IDictionary<string, object>;
                extractor.Update(result);
            }

            return container;
        }


        public List<Post> GetAllPosts(string nodeID)
        {
            var container = new List<Post>();

            var builder = new FBGraphQueryBuilder(nodeID, POSTS_EDGE);
            builder.RegisterParam(FBGraphQueryBuilder.FIELDS)
                   .RegisterParam(FBGraphQueryBuilder.LIMIT)
                   .RegisterParam(FBGraphQueryBuilder.PAGING_TOKEN)
                   .RegisterParam(FBGraphQueryBuilder.UNTIL);
            var queryBuilder = builder
                .Field(ID)
                .Field(NAME)
                .Field(MESSAGE)
                .Field(DESCRIPTION)
                .Field(LINK)
                .Field(FROM)
                .Field(SHARES)
                .Limit(5000);
            var result = _fbClient.Get(queryBuilder.Build()) as IDictionary<string, object>;

            var extractor = new FieldExtractor(result);
            while (extractor.HasMoreResults())
            {
                var posts = extractor.ExtractArray(DATA);
                container.AddRange(posts.Select(field => new Post()
                {
                    FacebookID = field.Extract(ID),
                    Name = field.Extract(NAME),
                    Message = field.Extract(MESSAGE),
                    Description = field.Extract(DESCRIPTION),
                    Link = field.Extract(LINK),
                    FromFacebokUserID = field.Advance(FROM).Extract(ID),
                    FromFacebookUserName = field.Advance(FROM).Extract(NAME),
                    StatusType = field.Extract(STATUS_TYPE),
                    Shares = String.IsNullOrEmpty(field.Advance(SHARES).Extract(COUNT)) ? 0 : Int32.Parse(field.Advance(SHARES).Extract(COUNT))
                }));
                queryBuilder.PagingToken(extractor.GetPagingToken());
                queryBuilder.Until(extractor.GetUntil());
                var query = queryBuilder.Build();
                result = _fbClient.Get(query) as IDictionary<string, object>;
                extractor.Update(result);
                if (posts.Count == 1) break; 
            }

            return container;
        }

        public string GetLongLivedAccessToken(string clientID, string clientSecret)
        {
            var builder = new FBGraphQueryBuilder(NODE_OAUTH, EDGE_ACCESS_TOKEN);
            builder.RegisterParam(CLIENT_ID)
                   .RegisterParam(CLIENT_SECRET)
                   .RegisterParam(FB_EXCHANGE_TOKEN)
                   .RegisterParam(GRANT_TYPE);
            builder.SetParam(CLIENT_ID, clientID)
                   .SetParam(CLIENT_SECRET, clientSecret)
                   .SetParam(FB_EXCHANGE_TOKEN, _accesToken)
                   .SetParam(GRANT_TYPE, FB_EXCHANGE_TOKEN);

            var query = builder.Build();
            var result = _fbClient.Get(query) as IDictionary<string, object>;

            var extractor = new FieldExtractor(result);
            var longLivedAccessToken = extractor.Extract(FacebookManager.ACCESS_TOKEN);

            return longLivedAccessToken;
        }
    }
}