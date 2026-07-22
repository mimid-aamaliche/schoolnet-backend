using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnlineSchoolBusniss
{
    public sealed class MUXEventService
    {


        //******************************************************************************************************************************************
        //*****************************************               MUX webhook Events              **************************************************
        //******************************************************************************************************************************************
        public async Task<bool> HandleMuxEvents(string HttpBody)
        {

            var jsonPayload = JsonSerializer.Deserialize<JsonElement>(HttpBody);

            string CurrentEvent = jsonPayload.GetProperty("type").GetString();


            if (string.Compare(CurrentEvent, "video.upload.asset_created") == 0)
            {

                try
                {
                    return await HandleVideo_upload_asset_created_Event(jsonPayload.GetProperty("data"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }

            }
            else if (string.Compare(CurrentEvent, "video.asset.ready") == 0)
            {
                //the video is ready to be streamed to student 

                try
                {
                    return await HandleVideo_asset_ready_Event(jsonPayload.GetProperty("data"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }

            }

            return false;

        }


        /// <summary>
        /// Fired by MUX when a direct upload finishes and MUX has created the asset internally.
        /// At this point the asset is still "preparing" — not yet streamable.
        ///
        /// Payload shape (data object):
        ///   data.id          → the MUX Upload ID  (same one we stored in CourseContent.UploadID)
        ///   data.asset_id    → the real MUX Asset ID
        ///   data.status      → "asset_created" (asset is preparing)
        ///
        /// We look up the content record by UploadID and store the real AssetID + status.
        /// </summary>
        private async ValueTask<bool> HandleVideo_upload_asset_created_Event(JsonElement data)
        {
            // The upload ID is data.id in the upload event payload
            string uploadID = data.GetProperty("id").GetString() ?? "";

            // The real MUX asset ID (will also appear in subsequent asset events)
            string assetID = data.GetProperty("asset_id").GetString() ?? "";

            // Status at this stage is typically "asset_created" (asset is preparing in MUX)
            string assetStatus = data.GetProperty("status").GetString() ?? "";

            if (string.IsNullOrEmpty(uploadID)) return false;

            // Find our content record by the UploadID we stored when generating the upload URL
            clsCourseContent Content = clsCourseContent.FindByUploadID(uploadID);
            if (Content == null)
            {
                Console.WriteLine($"MUXEventService: no content record found for UploadID={uploadID}");
                return false;
            }

            // Store the real asset ID and current status
            Content.AssetID = assetID;
            Content.AssetStatus = assetStatus;

            return Content.Save();
        }


        /// <summary>
        /// Fired by MUX when the asset finishes processing and is ready for streaming.
        ///
        /// Payload shape (data object):
        ///   data.upload_id      → the MUX Upload ID (same one we stored in CourseContent.UploadID)
        ///   data.id             → the real MUX Asset ID
        ///   data.status         → "ready"
        ///   data.playback_ids   → array of playback objects with id + policy
        ///
        /// We look up by UploadID, then store AssetID, status, and the first PlaybackID.
        /// </summary>
        private async ValueTask<bool> HandleVideo_asset_ready_Event(JsonElement data)
        {
            // The upload_id field is present on video.asset.ready events
            if (!data.TryGetProperty("upload_id", out JsonElement uploadIdElement))
            {
                Console.WriteLine("MUXEventService: video.asset.ready event has no upload_id — cannot correlate to content record.");
                return false;
            }

            string uploadID   = uploadIdElement.GetString() ?? "";
            string assetID    = data.GetProperty("id").GetString() ?? "";
            string assetStatus = data.GetProperty("status").GetString() ?? "";

            if (string.IsNullOrEmpty(uploadID)) return false;

            // Find our content record by UploadID
            clsCourseContent Content = clsCourseContent.FindByUploadID(uploadID);
            if (Content == null)
            {
                Console.WriteLine($"MUXEventService: no content record found for UploadID={uploadID}");
                return false;
            }

            Content.AssetID    = assetID;
            Content.AssetStatus = assetStatus;

            // Store the first available playback ID
            if (data.TryGetProperty("playback_ids", out JsonElement playbackIds)
                && playbackIds.ValueKind == JsonValueKind.Array
                && playbackIds.GetArrayLength() > 0)
            {
                Content.PlayBackID = playbackIds[0].GetProperty("id").GetString() ?? "";
            }

            return Content.Save();
        }


    }
}
