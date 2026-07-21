using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnlineSchoolBusniss
{
    public sealed class clsMUXSigningService
    {
        private readonly clsMUXSigningServiceModel _muxSigningServiceModel;
        private readonly RSA _rsa;

        public clsMUXSigningService(IOptions<clsMUXSigningServiceModel> muxSigningServiceModel) {

            _muxSigningServiceModel = muxSigningServiceModel.Value;

            _rsa = RSA.Create();

            byte[] keyBytes = Convert.FromBase64String(_muxSigningServiceModel.SecretKey);
            string pemText = Encoding.UTF8.GetString(keyBytes);

            Console.WriteLine(pemText);

            _rsa.ImportFromPem(pemText);
           

        }


        public string CreateSignedURL(string playbackid)
        {



            var securityKey = new RsaSecurityKey(_rsa);
            var cridentiels = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);



            var header = new JwtHeader(cridentiels)
            {
                    { "kid", _muxSigningServiceModel.KeyID } // header key id
            };



            var payload = new JwtPayload
            {
                     { "aud", "v" },
                     { "sub", playbackid },
                     { "exp", DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeSeconds() }
            };



            //var jwtPayload = new JwtPayload(claims);

            var token = new JwtSecurityToken(header, payload);

            var handler = new JwtSecurityTokenHandler();

            string jwt = handler.WriteToken(token);


            string signedUrl = $"https://stream.mux.com/{playbackid}.m3u8?token={jwt}";


            return signedUrl;


        }












    }
}
