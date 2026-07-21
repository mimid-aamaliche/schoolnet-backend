using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Azure;
using Microsoft.IdentityModel.Tokens;
using SharedDTOs;



namespace OnlineSchoolBusniss
{
    public class clsMUXVideoService
    {
        private readonly HttpClient _httpclient;

        public clsMUXVideoService(HttpClient httpClient)
        {

            _httpclient = httpClient;
        }

        public async Task<DirectUploadRecord?> CreateAsset()
        {

            // make the http body
            var body = new
            {
                cors_origin = "*",
                new_asset_settings = new
                {
                    playback_policies = new List<string>
                                {
                                    "signed"
                                },
                    resumable = true

                }


            };

            //convert body to json format
            string json = System.Text.Json.JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");




            //send request
            HttpResponseMessage responce = await _httpclient.PostAsync("video/v1/uploads", content);



            if (responce.IsSuccessStatusCode)
            {
                //read the responce data that we want !!!!!
                using (var document = JsonDocument.Parse(await responce.Content.ReadAsStringAsync()))
                {
                    var rootElement = document.RootElement;

                    //here we should extract the upload id and store it as asset id
                    //and when the asset created event arrives , we match our record with mux record using that uploadid 


                    //here we should return the direct upload record wich will contain the upload_id and the url 

                    var data=rootElement.GetProperty("data");
                    string? upload_url= data.GetProperty("url").GetString();
                    string? upload_id= data.GetProperty("id").GetString();

                    return new DirectUploadRecord(upload_id,upload_url);

                }




            }






            return null;
        }

        
        public async Task<string?> GetAssetPlayBackID(string AssetID)
        {




            //send request
            HttpResponseMessage responce = await _httpclient.GetAsync($"video/v1/assets/{AssetID}");


            if (responce.IsSuccessStatusCode)
            {
                //read the responce data that we want !!!!!


                using (var document = JsonDocument.Parse(await responce.Content.ReadAsStringAsync()))
                {
                    var rootElement = document.RootElement;



                    JsonElement playbackIDS = rootElement.GetProperty("data").GetProperty("playback_ids");

                    if (playbackIDS.ValueKind == JsonValueKind.Array && playbackIDS.GetArrayLength() > 0)
                    {
                        //here we return the first element
                        return playbackIDS[0].GetProperty("id").GetString();


                    }
                    else
                    {
                        //here we return null , meaning the user needs to creat a playback befor watching the content
                        return null;


                    }



                }



            }
            else
            {
                return null;
            }








        }


        public async Task<string?> CreateAssetPlayBackID(string AssetID)
        {



            return "the new playback id ";







        }







    }
}
