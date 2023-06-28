using Flurl;
using Flurl.Http;
using OData.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace OData
{
    public class TableStore
    {
        private static Url _url;
        private readonly string _tableName;

        public TableStore(string storageAcct, string sas, string tableName)
        {
            // For local storage :-
            // Create a 'Manage Access Policy' with a name of Application - no start/end date and all access.
            //| Storage Account                                  | tableName     |                                                | SAS                                                      |
            //|--------------------------------------------------|---------------|------------------------------------------------|----------------------------------------------------------|
            //|https://fmlotterystrvsene.table.core.windows.net  |/ThunderBall   |?si=Application&sv=2019-12-12&tn=ThunderBall    | &sig=8S2APGgD48VzRWXmeA0AaoreWCdIDbl6vionhBdCYiQ%3D      |
            //|http://127.0.0.1:10002/devstoreaccount1           |/ThunderBall   |?si=Application&sv=2015-04-05&tn=thunderball    | &sig=eed0kJ5uDqBWPNkqW4g%2F8YaL1%2BiRuGuqF6GZM2QLa38%3D  |

            _url = new Url(storageAcct);

            _url.SetQueryParam("si", "Application")
                .SetQueryParam("sv", "2015-04-05") //"2019-12-12")
                .SetQueryParam("tn", tableName)
                .SetQueryParam("sig", sas, true); // true stops it changing the string

            _tableName = tableName;
        }

        public Models.OData Select(string filter = null)
        {
            Models.OData res;

            // If the last path segment contains the tablename, then clear it
            var ps = _url.PathSegments;
            if (ps[ps.Count-1].Contains(_tableName))
            {
                _url.RemovePathSegment();
            }

            var url  = _url
                        .AppendPathSegment(_tableName)
                        .SetQueryParam("$Filter", filter, true)
                        .WithHeader("Accept", "application/json");

            res = url.GetJsonAsync<Models.OData>().Result;
            return res;
        }

        public IFlurlResponse Insert(TableEntity entity)
        {
            IFlurlResponse res;

            // If the last path segment contains the tablename, then clear it
            var ps = _url.PathSegments;
            if (ps[ps.Count - 1].Contains(_tableName))
            {
                _url.RemovePathSegment();
            }

            var url = _url
                        .AppendPathSegment(_tableName)
                        .WithHeaders(new { Accept = "*/*", Content_Type = "application/json" });

            res = url.PostJsonAsync(entity).Result;

            return res;
        }

        public IFlurlResponse Update(TableEntity entity)
        {
            var tablename = _tableName;

            // if Table contains multiple partitions.
            if (!string.IsNullOrEmpty(entity.PartitionKey))
            {
                tablename = entity.PartitionKey;
            }

            // If the last path segment contains the tablename, then clear it
            var ps = _url.PathSegments;
            if (ps[ps.Count - 1].Contains(_tableName))
            {
                _url.RemovePathSegment();
            }

            var url = _url
                        .AppendPathSegment(_tableName + "(PartitionKey= '" + tablename + "', RowKey='" + entity.RowKey + "')")
                        .WithHeaders(new { Accept = "*/*", Content_Type = "application/json", If_Match = "*" });

            var res = url.PutJsonAsync(entity).Result;

            return res;
        }

        public IFlurlResponse Upsert(TableEntity entity)
        {
            var tablename = _tableName;

            // if Table contains multiple partitions.
            if (!string.IsNullOrEmpty(entity.PartitionKey))
            {
                tablename = entity.PartitionKey;
            }

            // If the last path segment contains the tablename, then clear it
            var ps = _url.PathSegments;
            if (ps[ps.Count - 1].Contains(_tableName))
            {
                _url.RemovePathSegment();
            }

            Console.WriteLine( JsonConvert.SerializeObject(entity));

            var url = _url
                        .AppendPathSegment(_tableName + "(PartitionKey='" + tablename + "',RowKey='" + entity.RowKey + "')")
                        .WithHeaders(new { Accept = "*/*", Content_Type = "application/json" });
            var res = url.PutJsonAsync(entity).Result;

            return res;
        }

        public IFlurlResponse Delete(string rowKey, string partitonKey = null)
        {
            var tablename = _tableName;

            // if Table contains multiple partitions.
            if( !string.IsNullOrEmpty(partitonKey))
            {
                tablename = partitonKey;
            }

            // If the last path segment contains the tablename, then clear it
            var ps = _url.PathSegments;
            if (ps[ps.Count - 1].Contains(_tableName))
            {
                _url.RemovePathSegment();
            }

            var url = _url
                        .AppendPathSegment(_tableName + "(PartitionKey= '" + tablename + "', RowKey='" + rowKey + "')")
                        .WithHeaders(new { Accept = "*/*", Content_Type = "application/json", If_Match = "*" });
            var res = url.DeleteAsync().Result;

            return res;
        }

    }
}
