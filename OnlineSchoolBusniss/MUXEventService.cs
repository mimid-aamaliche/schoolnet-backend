using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OnlineSchoolBusniss
{
    public sealed class MUXEventService
    {
        private readonly ILogger<MUXEventService> _logger;

        public MUXEventService(ILogger<MUXEventService> logger)
        {
            _logger = logger;
        }

        //******************************************************************************************************************************************
        //*****************************************               MUX webhook Events              **************************************************
        //******************************************************************************************************************************************
        public async Task<bool> HandleMuxEvents(string HttpBody)
        {
            var jsonPayload = JsonSerializer.Deserialize<JsonElement>(HttpBody);

            string currentEvent = jsonPayload.GetProperty("type").GetString() ?? "";

            _logger.LogInformation("Processing MUX webhook event | EventType={EventType}", currentEvent);

            if (string.Compare(currentEvent, "video.upload.asset_created") == 0)
            {
                try
                {
                    return await HandleVideo_upload_asset_created_Event(jsonPayload.GetProperty("data"));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error handling video.upload.asset_created event");
                    return false;
                }
            }
            else if (string.Compare(currentEvent, "video.asset.ready") == 0)
            {
                try
                {
                    return await HandleVideo_asset_ready_Event(jsonPayload.GetProperty("data"));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error handling video.asset.ready event");
                    return false;
                }
            }

            _logger.LogInformation("MUX event unhandled/ignored | EventType={EventType}", currentEvent);
            return false;
        }


        /// <summary>
        /// Fired by MUX when a direct upload finishes and MUX has created the asset internally.
        /// At this point the asset is still "preparing" — not yet streamable.
        /// </summary>
        private async ValueTask<bool> HandleVideo_upload_asset_created_Event(JsonElement data)
        {
            string uploadID    = data.GetProperty("id").GetString() ?? "";
            string assetID     = data.GetProperty("asset_id").GetString() ?? "";
            string assetStatus = data.GetProperty("status").GetString() ?? "";

            _logger.LogInformation("HandleVideo_upload_asset_created | UploadID={UploadID} AssetID={AssetID} Status={Status}",
                uploadID, assetID, assetStatus);

            if (string.IsNullOrEmpty(uploadID))
            {
                _logger.LogWarning("video.upload.asset_created event has missing or empty UploadID");
                return false;
            }

            clsCourseContent content = clsCourseContent.FindByUploadID(uploadID);
            if (content == null)
            {
                _logger.LogWarning("No content record found in DB for UploadID={UploadID}", uploadID);
                return false;
            }

            content.AssetID     = assetID;
            content.AssetStatus = assetStatus;

            bool saved = content.Save();
            if (saved)
            {
                _logger.LogInformation("Updated content record with AssetID | ContentID={ContentID} AssetID={AssetID}",
                    content.CourseContentID, assetID);
            }
            else
            {
                _logger.LogError("Failed to save content record with AssetID | ContentID={ContentID} UploadID={UploadID}",
                    content.CourseContentID, uploadID);
            }

            return saved;
        }


        /// <summary>
        /// Fired by MUX when the asset finishes processing and is ready for streaming.
        /// </summary>
        private async ValueTask<bool> HandleVideo_asset_ready_Event(JsonElement data)
        {
            if (!data.TryGetProperty("upload_id", out JsonElement uploadIdElement))
            {
                _logger.LogWarning("video.asset.ready event has no upload_id — cannot correlate to content record");
                return false;
            }

            string uploadID    = uploadIdElement.GetString() ?? "";
            string assetID     = data.GetProperty("id").GetString() ?? "";
            string assetStatus = data.GetProperty("status").GetString() ?? "";

            _logger.LogInformation("HandleVideo_asset_ready | UploadID={UploadID} AssetID={AssetID} Status={Status}",
                uploadID, assetID, assetStatus);

            if (string.IsNullOrEmpty(uploadID))
            {
                _logger.LogWarning("video.asset.ready event has empty UploadID");
                return false;
            }

            clsCourseContent content = clsCourseContent.FindByUploadID(uploadID);
            if (content == null)
            {
                _logger.LogWarning("No content record found in DB for UploadID={UploadID}", uploadID);
                return false;
            }

            content.AssetID     = assetID;
            content.AssetStatus = assetStatus;

            if (data.TryGetProperty("playback_ids", out JsonElement playbackIds)
                && playbackIds.ValueKind == JsonValueKind.Array
                && playbackIds.GetArrayLength() > 0)
            {
                content.PlayBackID = playbackIds[0].GetProperty("id").GetString() ?? "";
                _logger.LogInformation("Extracted PlaybackID for ready asset | ContentID={ContentID} PlaybackID={PlaybackID}",
                    content.CourseContentID, content.PlayBackID);
            }

            bool saved = content.Save();
            if (saved)
            {
                _logger.LogInformation("Content marked READY in DB | ContentID={ContentID} PlaybackID={PlaybackID}",
                    content.CourseContentID, content.PlayBackID);
            }
            else
            {
                _logger.LogError("Failed to save READY status to DB | ContentID={ContentID} UploadID={UploadID}",
                    content.CourseContentID, uploadID);
            }

            return saved;
        }
    }
}
