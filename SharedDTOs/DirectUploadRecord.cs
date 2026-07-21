namespace SharedDTOs;

public record DirectUploadRecord
{
    public string upload_id;
    public string upload_url;

    public DirectUploadRecord(string upload_id,string upload_url)
    {
        this.upload_id = upload_id;
        this.upload_url = upload_url;
    }
    
}

