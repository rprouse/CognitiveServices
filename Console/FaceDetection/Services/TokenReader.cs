using System;
using System.IO;

namespace FaceDetection.Services
{
    public static class TokenReader
    {
        const string TOKEN = "TOKEN";

        public static string GetApiToken()
        {
            string token = null;
            if (File.Exists(TOKEN))
            {
                using (var reader = new StreamReader(TOKEN))
                {
                    token = reader.ReadToEnd();
                }
            }
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new Exception("The file TOKEN must exist with the Azure subscription");
            }
            return token;
        }
    }
}
