using System.Security.Cryptography;
using OnlineSchoolBusniss;
namespace OnlineSchool_Back_end.Middlewares;

public class muxTraficValidatorMiddleware
{

    private readonly RequestDelegate _next;
    private readonly string _webhookKey;

    public muxTraficValidatorMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _webhookKey = configuration["MUXVideoService:WebHookKey"]!;
    }

    public async Task InvokeAsync(HttpContext context)
    {

        Console.WriteLine("--- Incoming Request Headers ---");
    
        foreach (var header in context.Request.Headers)
        {
            // header.Key is the name (e.g., "User-Agent")
            // header.Value is a StringValues object (can contain multiple strings)
            Console.WriteLine($"{header.Key}: {header.Value}");
        }
        
        Console.WriteLine(context.Request.Path);

        //check if the request is headed for the mux webhook  
        if (string.Compare(context.Request.Path, "/api/MuxWebhook") != 0)
        {
            await _next(context);
            return;

        }



        
        if (context.Request.Headers.TryGetValue("Mux-Signature", out var signatureHeader))
        {
            var body =await  new StreamReader(context.Request.Body).ReadToEndAsync();
            //Mux-Signature: t=1565220904,v1=20c75c1180c701ee8a796e81507cfd5c932fc17cf63a4a55566fd38da3a2d3d2`
            //we need to split it to get the timestaps and the signature
            var muxSignature_elements = signatureHeader.ToString().Split(',');

            var timestamp = muxSignature_elements[0].Split("=")[1];
            var signature = muxSignature_elements[1].Split("=")[1];


            //for the http request to be valide 
            // the encryption(timestamp+.+body, webhookKey)==signature must be true

            string payload = $"{timestamp}.{body}";
            Console.WriteLine(payload);

            Console.WriteLine(signature);
            Console.WriteLine(MyEncriptionFunctions.hmachashing(payload, _webhookKey, new HMACSHA256()));

            if (string.Compare(signature, MyEncriptionFunctions.hmachashing(payload, _webhookKey, new HMACSHA256())) == 0)
            {

                context.Items["RawBody"] = body;//to avoid having to reread the stream (to do that we need to enable buffering)
                Console.WriteLine(payload);
                await _next(context);
            }
        }





        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return;
    }

}

