using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SharedDTOs;

namespace OnlineSchoolBusniss
{
    public class clsMUXVideoService
    {
        private readonly HttpClient _httpclient;
        private readonly ILogger<clsMUXVideoService>? _logger;

        public clsMUXVideoService(HttpClient httpClient, ILogger<clsMUXVideoService>? logger = null)
        {
            _httpclient = httpClient;
            _logger     = logger;
        }

        public async Task<DirectUploadRecord?> CreateAsset()
        {
            _logger?.LogInformation("Creating MUX Direct Upload Asset via video/v1/uploads...");

            var body = new
            {
                cors_origin = "*",
                new_asset_settings = new
                {
                    playback_policies = new List<string>
                    {
                        "signed"
                    }
                }
            };

            string json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpclient.PostAsync("video/v1/uploads", content);

            string responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                using var document = JsonDocument.Parse(responseString);
                var data = document.RootElement.GetProperty("data");

                string? uploadUrl = data.GetProperty("url").GetString();
                string? uploadId  = data.GetProperty("id").GetString();

                _logger?.LogInformation("MUX Direct Upload created successfully | UploadID={UploadID}", uploadId);
                return new DirectUploadRecord(uploadId, uploadUrl);
            }

            _logger?.LogError("MUX CreateAsset failed | StatusCode={StatusCode} Error={Response}",
                response.StatusCode, responseString);

            return null;
        }

        public async Task<string?> GetAssetPlayBackID(string AssetID)
        {
            _logger?.LogInformation("Fetching MUX PlaybackID for AssetID={AssetID}...", AssetID);

            HttpResponseMessage response = await _httpclient.GetAsync($"video/v1/assets/{AssetID}");
            string responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                using var document = JsonDocument.Parse(responseString);
                var rootElement = document.RootElement;

                JsonElement playbackIDS = rootElement.GetProperty("data").GetProperty("playback_ids");

                if (playbackIDS.ValueKind == JsonValueKind.Array && playbackIDS.GetArrayLength() > 0)
                {
                    string? playbackId = playbackIDS[0].GetProperty("id").GetString();
                    _logger?.LogInformation("Found PlaybackID={PlaybackID} for AssetID={AssetID}", playbackId, AssetID);
                    return playbackId;
                }

                _logger?.LogInformation("No playback IDs found on asset yet | AssetID={AssetID}", AssetID);
                return null;
            }

            _logger?.LogError("GetAssetPlayBackID failed | AssetID={AssetID} StatusCode={StatusCode} Error={Response}",
                AssetID, response.StatusCode, responseString);

            return null;
        }

        public async Task<string?> CreateAssetPlayBackID(string AssetID)
        {
            _logger?.LogInformation("CreateAssetPlayBackID requested for AssetID={AssetID}", AssetID);
            return "the new playback id ";
        }
    }
}
