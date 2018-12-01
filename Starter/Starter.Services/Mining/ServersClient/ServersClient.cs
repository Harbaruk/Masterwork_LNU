using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Starter.API.Policies;
using Starter.Services.Blocks.Models;
using Starter.Services.Token;
using Starter.Services.Transactions.Models;

namespace Starter.Services.Mining.ServersClient
{
    public class ServersClient : IServersClient
    {
        private readonly TrustfullServersOptions _serversOptions;
        private readonly ITokenService _tokenService;
        private readonly HttpClient _httpClient;
        private string _serverHash = "8f00234b-dabb-4765-b616-841d5b92a9a0";

        public ServersClient(IOptions<TrustfullServersOptions> serversOptions, ITokenService tokenService)
        {
            _serversOptions = serversOptions.Value;
            _tokenService = tokenService;
            _httpClient = new HttpClient();
        }

        public CreateBlockModel GetUnverifiedBlock()
        {
            var token = GetToken();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);
            foreach (var server in _serversOptions.IPAdressess)
            {
                var block = JsonConvert.DeserializeObject<CreateBlockModel>(_httpClient.GetAsync($"{server}/blocks/unverified").Result.Content.ReadAsStringAsync().Result);
                if (block != null)
                {
                    return block;
                }
            }
            return null;
        }

        private string GetToken()
        {
            return _tokenService.TrustfullServerToken(new Token.Models.TrustfullServerCredentialModel { Hash = _serverHash, Password = "serverpassword" })?.AccessToken;
        }

        public int GetUnverifiedTransactionCount()
        {
            var token = GetToken();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);
            var count = 0;
            foreach (var server in _serversOptions.IPAdressess)
            {
                var result = _httpClient.GetAsync($"{server}/transactions/unverified_count").Result;
                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return 0;
                }
                count += int.Parse(result.Content.ReadAsStringAsync().Result);
            }
            return count;
        }

        public void VerifyBlock(string block)
        {
            var token = GetToken();

            foreach (var server in _serversOptions.IPAdressess)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);
                _httpClient.PostAsync($"{server}/blocks/verify", new StringContent(JsonConvert.SerializeObject(block)));
            }
        }

        public IEnumerable<TransactionDetailedModel> GetUnverifiedTransactions()
        {
            Console.WriteLine("Get unverified tx");
            var token = GetToken();
            var result = new List<TransactionDetailedModel>();
            foreach (var server in _serversOptions.IPAdressess)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);
                result.AddRange(JsonConvert.DeserializeObject<IEnumerable<TransactionDetailedModel>>(_httpClient.GetAsync($"{server}/transactions/unverified").Result.Content.ReadAsStringAsync().Result));
            }
            return result;
        }

        public string GetLastBlock()
        {
            var token = GetToken();
            foreach (var server in _serversOptions.IPAdressess)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);
                var block = JsonConvert.DeserializeObject<BlockModel>(_httpClient.GetAsync($"{server}/block/last_verified").Result.Content.ReadAsStringAsync().Result);
                if (block != null)
                {
                    return block.Hash;
                }
            }
            return null;
        }
    }
}