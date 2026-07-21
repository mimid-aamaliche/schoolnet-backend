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
                //maybe notify the student of the course that this video belongs to
                //and notify the teacher of this course


                //first we should access the body and get the playback id 
                //and store it to db along side the video status

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


        private async ValueTask<bool> HandleVideo_upload_asset_created_Event(JsonElement data)
        {
            //the asset is created
            //1) get the asset id and playback id and store them in db 



            //the id of our video in mux database , can be used to retrieve info , create playbacks ....
            string AssetID = data.GetProperty("asset_id").GetString();

            //status can be : "preparing" "ready" "errored"
            string AssetStatus = data.GetProperty("status").GetString();

            //here we will find the data we added when we just created the direct upload 
            var metaData = data.GetProperty("asset_settings").GetProperty("meta");




            //now we will save the asset id and its status in db



            clsCourseContent Content = clsCourseContent.Find(metaData.GetProperty("ContentID").GetInt32());
            if (Content != null)
            {
                //here we save the info in the content object 

                Content.AssetID = AssetID;
                Content.AssetStatus = AssetStatus;


                //and save it to db
                if (Content.Save())
                {

                    return true;

                }



            }

            return false;






        }
      
        
        private async ValueTask<bool> HandleVideo_asset_ready_Event(JsonElement data)
        {
            //the asset is ready
            //1) get the asset id and playback id and store them in db 



            //the id of our video in mux database , can be used to retrieve info , create playbacks ....
            string AssetID = data.GetProperty("id").GetString() ?? "";

            //status can be : "preparing" "ready" "errored"
            string AssetStatus = data.GetProperty("status").GetString() ?? "";

            //here we will find the data we added when we just created the direct upload (contentid ...)

            if (!data.TryGetProperty("metadata", out JsonElement metadata))
            {
                return false;
            }



            //playback ids with there policy
            var playback_ids = data.GetProperty("playback_ids");




            //now we will save the first playback ids in database



            clsCourseContent Content = clsCourseContent.Find(metadata.GetProperty("ContentID").GetInt32());
            if (Content != null)
            {
                //here we save the info in the content object 




                Content.AssetID = AssetID;
                Content.AssetStatus = AssetStatus;


                if (playback_ids.ValueKind == JsonValueKind.Array && playback_ids.GetArrayLength() > 0)
                {

                    Content.PlayBackID = playback_ids[0].GetProperty("id").GetString() ?? "";


                }






                //and save it to db
                if (Content.Save())
                {

                    return true;

                }



            }

            return false;






        }






    }
}
