using System;
using System.Collections.Generic;
using System.Linq;

namespace EZCSharpFacebook.Core
{

    public class FBGraphQueryBuilder
    {
        /** The id of the facebook node.
         * A node can be : Photo, User, Comments and others
         */
        private readonly string _nodeID;
        /**
         * The name of the edge.
         * Edges are the connections between nodes.
         * Edge examples: Page's photos, Photo's comments 
         */
        private readonly string _edgeName;


        private readonly IDictionary<string, List<string>> _params = new Dictionary<string, List<string>>();

        public const string FIELDS = "fields";
        public const string AFTER = "after";
        public const string LIMIT = "limit";
        public const string PAGING_TOKEN = "_paging_token";
        public const string UNTIL = "until";

        public FBGraphQueryBuilder(string nodeID, string edgeName)
        {
            _nodeID = nodeID;
            _edgeName = edgeName;
        }

        public FBGraphQueryBuilder(string nodeId)
            : this(nodeId, String.Empty)
        {
        }

        /**
         * Builds the query, appending the specified fields at the end.
         */

        private bool ParamHasValueSet(KeyValuePair<string, List<string>> pair)
        {
            return pair.Value.Count != 0;
        }

        public string Build()
        {
            var first = Concat(_params.First());
            var other = String.Join("&", _params.Skip(1).Where(ParamHasValueSet).Select(Concat).ToList());
            other = AppendSeparatorIfNecessary(other);
            return String.Format("{0}?{1}{2}", String.Concat(GetNode(), GetEdge()), first, other);
        }

        /**
         * Appends the parameter specifier if more params are added.
         */
        private string AppendSeparatorIfNecessary(string other)
        {
            if (_params.Count >= 2)
            {
                other = String.Concat("&", other);
            }
            
            return other;
        }

        private string GetNode()
        {
            return _nodeID;
        }

        private string Concat(KeyValuePair<string, List<string>> keyValuePair)
        {
            return String.Concat(keyValuePair.Key, "=", String.Join(",", keyValuePair.Value));
        }


        /**
         * Utility function that returns the 'edge part' of the query,
         *          returns the empty string if no edge is specified.
         */
        private string GetEdge()
        {
            return !String.IsNullOrEmpty(_edgeName) ? "/" + _edgeName : String.Empty;
        }


        public FBGraphQueryBuilder RegisterParam(string paramName)
        {
            _params.Add(paramName, new List<string>());
            return this;
        }

        public FBGraphQueryBuilder SetParam(string paramName, string paramValue)
        {
            _params[paramName].Add(paramValue);
            return this;
        }


        /**
         * Appends a given fields to the list.
         */
        public FBGraphQueryBuilder Field(string fieldValue)
        {
            return TryAddingParamValue(FIELDS, fieldValue);
        }


        public FBGraphQueryBuilder Limit(int limitValue)
        {
            return TryAddingParamValue(LIMIT, limitValue.ToString());
        }

        public FBGraphQueryBuilder After(string afterValue)
        {
            return TryAddingParamValue(AFTER, afterValue, clear: true);
        }

        private FBGraphQueryBuilder TryAddingParamValue(string paramName, string paramValue, bool clear = false)
        {
            if (clear)
            {
                _params[paramName].Clear();
            }
            try
            {
                _params[paramName].Add(paramValue);
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException(String.Format("Param '{0}' not registered.", paramName));
            }
            return this;
        }

        public FBGraphQueryBuilder PagingToken(string pagingToken)
        {
            TryAddingParamValue(PAGING_TOKEN, pagingToken, clear: true);
            return this;
        }

        public FBGraphQueryBuilder Until(string untilValue)
        {
            return TryAddingParamValue(UNTIL, untilValue, clear: true);
        }
    }
}