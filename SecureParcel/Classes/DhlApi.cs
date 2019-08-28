using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SecureParcel.Classes
{
    public class DhlApi
    {
        const string ClientId = "e9ed82a8-4237-4185-8e36-47264aa9e718"; //"rcoNcZtkDL7xiJF4PF3DDF2Sl2wyB6LQ";
        const string ClientSecret = "b1ed1bfa-689b-4d5b-bbf2-39dde64ccb64"; //"GalKk1rYr8YhLnIn";
        const string AccountNumber = "";
        static DateTime TokenExpiredAt = DateTime.MinValue;
        static string Token = "";

        public static async Task<string> GetToken()
        {
            if (TokenExpiredAt < DateTime.Now)
            {
                var baseAddress = new Uri("https://private-anon-5673d6647d-dhlfulfillmentapidoc.apiary-proxy.com/");

                using (var httpClient = new HttpClient { BaseAddress = baseAddress })
                {
                    string keys = string.Format("{0}:{1}", ClientId, ClientSecret);
                    string authorization = string.Format("Basic {0}", Base64Encode(keys));

                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("authorization", authorization);

                    using (var response = await httpClient.GetAsync("efulfillment/v1/auth/accesstoken"))
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            string data = await response.Content.ReadAsStringAsync();
                            var json = JsonConvert.DeserializeObject<TokenJson.RootObject>(data);

                            Token = json.access_token;
                            TokenExpiredAt = DateTime.Now.AddSeconds(18000);
                        }
                    }
                }
            }

            return Token;
        }

        public static async Task<string> AddItem()
        {
            var item = new DHLJson();
            item.description = "";

            //var createItem = new DHLJson.property.Item();
            //item.AccountNumber = "e9ed82a8-4237-4185-8e36";
            //item.ItemID = "39dde64ccb64";
            //item.ItemDetails = new DHLJson.property.Item.ItemDetail();
            //item.ItemDetails.IsInsert = false;
            //item.ItemDetails.IsReturnable = false;
            //item.ItemDetails.IsSubstitutionAllowed = false;
            //item.ItemDetails.IsSubstitutionAutomatic = false;
            //item.ItemDetails.ItemStatus = "PUBLISHED";
            //item.ItemDetails.ShortDescription = "iPhone 77";
            //item.ItemDetails.UnitCost = new DHLJson.property.Item.UnitCost();
            //item.ItemDetails.UnitCost.Amount = 100;
            //item.ItemDetails.UnitCost.Currency = "USD";
            //item.ItemDetails.UnitOfMeasure = "point";

            //var baseAddress = new Uri("https://private-anon-5673d6647d-dhlfulfillmentapidoc.apiary-proxy.com/");

            //using (var httpClient = new HttpClient { BaseAddress = baseAddress })
            //{
            //    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");

            //    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("authorization", await GetToken());

            //    using (var content = new StringContent(JsonConvert.SerializeObject(item), System.Text.Encoding.Default, "application/json"))
            //    {
            //        using (var response = await httpClient.PostAsync("efulfillment/v1/item", content))
            //        {
            //            string responseData = await response.Content.ReadAsStringAsync();
            //        }
            //    }
            //}

            return "";
        }

        static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public class TokenJson
        {
            public class RootObject
            {
                public string access_token { get; set; }
                public string token_type { get; set; }
                public int expires_in { get; set; }
                public string scope { get; set; }
            }            
        }


        public class DHLJson
        {
            public string description { get; set; }
            public string type { get; set; }
            public property properties { get; set; }

            public class property
            {
                public Item CreateItem { get; set; }

                public class Item
                {
                    public string AccountNumber { get; set; }
                    public string ItemID { get; set; }
                    public ItemDetail ItemDetails { get; set; }

                    public class ItemDetail
                    {
                        public string ShortDescription { get; set; }
                        public string UnitOfMeasure { get; set; }
                        public bool IsReturnable { get; set; }
                        public bool IsSubstitutionAllowed { get; set; }
                        public bool IsSubstitutionAutomatic { get; set; }
                        public string ItemStatus { get; set; }
                        public bool IsInsert { get; set; }
                        public UnitCost UnitCost { get; set; }
                    }

                    public class UnitCost
                    {
                        public decimal Amount { get; set; }
                        public string Currency { get; set; }
                    }
                }
            }
        }

        
    }
}