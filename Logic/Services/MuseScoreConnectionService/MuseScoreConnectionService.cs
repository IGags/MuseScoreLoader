using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Logic.Constants;
using Logic.Enums;
using Logic.Models;
using Logic.Services.MuseScoreConnectionService.Interfaces;
using Newtonsoft.Json.Linq;

namespace Logic.Services.MuseScoreConnectionService
{
    ///<inheritdoc cref="IMuseScoreConnectionService"/>
    internal class MuseScoreConnectionService : IMuseScoreConnectionService
    {
        ///<inheritdoc />
        public async Task<Stream> LoadRawFileAsync(MuseScoreConnectionModel model)
        {
            var client = new HttpClient();
            var uriBuilder = new UriBuilder(MuseScoreConnectionConstants.MuseScoreHostUri);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query = AddRequestQuery(query, model.EntityId, model.ContentQuery, model.Index, model.V2Flag);
            uriBuilder.Query = query.ToString();

            client.DefaultRequestHeaders.Add("authorization", model.AuthId);
            var responseMessage = await client.GetAsync(uriBuilder.ToString());
            if (!responseMessage.IsSuccessStatusCode)
            {
                throw new FileNotFoundException();
            }

            var bodyBytes = await responseMessage.Content.ReadAsByteArrayAsync();
            var bodyString = Encoding.UTF8.GetString(bodyBytes);

            var bodyJson = JObject.Parse(bodyString);
            var ultimateGuitarUri = bodyJson["info"]["url"].ToObject<string>();

            client.DefaultRequestHeaders.Clear();
            try
            {
                var response = await client.GetStreamAsync(ultimateGuitarUri);
                return response;
            }
            catch
            {
                throw new FileNotFoundException();
            }
        }

        public async Task<byte[]> LoadFileAsByteArrayAsync(MuseScoreConnectionModel model)
        {
            await using var stream = await LoadRawFileAsync(model);
            var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            stream.Close();
            await stream.DisposeAsync();
            return ms.ToArray();
        }

        private NameValueCollection AddRequestQuery(NameValueCollection query, string entityId, string contentType, string index, string v2Flag)
        {
            query.Add("id", entityId);
            query.Add("index", index);
            query.Add("type", contentType);
            query.Add("v2", v2Flag);
            return query;
        }
    }
}
