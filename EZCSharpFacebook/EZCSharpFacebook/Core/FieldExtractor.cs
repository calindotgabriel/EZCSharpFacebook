// Created by Călin Gabriel
// at 20:49 on 27/02/2016.
//  

using System;
using System.Collections.Generic;
using System.Linq;
using EZCSharpFacebook.Exception;
using Facebook;

namespace EZCSharpFacebook.Core
{
    public class FieldExtractor
    {
        private IDictionary<string, object> _baseDict;

        public FieldExtractor(IDictionary<string, object> baseDict)
        {
            _baseDict = baseDict;
        }

        /**
         * Extracts the given field.
         * String.Empty is returned if no such field is in the dictionary.
         */
        public string Extract(string key)
        {
            try
            {
                var value = _baseDict[key];
                if (value is int || value is string || value is long)
                    return value.ToString();
                throw new CantConvertThisTypeException(
                    String.Format("Don't know how to extract a {0}, associated to key {1}, is the field a string?", value.GetType(), key));
            }
            catch (KeyNotFoundException)
            {
                return String.Empty;
            }
        }

        /**
         * Advances in the result tree by one level.
         */
        public FieldExtractor Advance(string key)
        {
            try
            {
                var dict = (IDictionary<string, object>)_baseDict[key];
                return new FieldExtractor(dict);
            }
            catch (KeyNotFoundException)
            {
                return Empty(); 
            }
        }

        /**
         * Returns an empty result tree.
         */
        private static FieldExtractor Empty()
        {
            return new FieldExtractor(new Dictionary<string, object>());
        }

        /**
         * Checks to see if the extractor can advance in the result tree.
         */
        public bool CanAdvance(string key)
        {
            return _baseDict.ContainsKey(key);
        }

        /**
         * Extracts a list of json objects from the result tree.
         */
        public List<FieldExtractor> ExtractArray(string key)
        {
            var array = _baseDict[key];
            return (from JsonObject obj in (JsonArray)array select obj as IDictionary<string, object> into dict select new FieldExtractor(dict)).ToList();
        }

        /**
         * Checks to see if the result has more data by 
         * examining the "next" attribute from the result tree.
         */
        public bool HasMoreResults()
        {
            try
            {
                return Advance(FacebookManager.PAGING).CanAdvance(FacebookManager.NEXT);
            }
            catch (CannotAdvanceException)
            {
                return false;
            }
        }

        /**
         * Gets the next page token.
         */
        public string GetAfter()
        {
            return Advance(FacebookManager.PAGING).Advance(FacebookManager.CURSORS).Extract(FacebookManager.AFTER);
        }



        /**
         * Updates the root dictionary.
         */
        public void Update(IDictionary<string, object> baseDict)
        {
            _baseDict = baseDict;
        }

        /**
         * Gets the paging token parameter, necesarry to cycle through query results.
         */
        public string GetPagingToken()
        {
            return GetPagingToken(FacebookManager.NEXT);
        }

        /**
         * EXPOSED FOR TESTING PURPOSES ONLY
         * Gets the paging token, necesarry to cycle through query results.
         */
        public string GetPagingToken(string nextEndpoint)
        {
            return Advance(FacebookManager.PAGING).Extract(nextEndpoint).ExtractFacebookParameter(FacebookManager.PAGING_TOKEN);
        }


        /**
         * Gets the until parameter, necesarry to cycle through query results.
         */
        public string GetUntil(string nextEndpoint)
        {
            return Advance(FacebookManager.PAGING).Extract(nextEndpoint).ExtractFacebookParameter(FacebookManager.UNTIL);
        }

        public string GetUntil()
        {
            return GetUntil(FacebookManager.NEXT);
        }

        
    }
}