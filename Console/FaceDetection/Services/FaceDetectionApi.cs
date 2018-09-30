using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FaceDetection.Extensions;

namespace FaceDetection.Services
{
    public static class FaceDetectionApi
    {
        static readonly string apiToken = TokenReader.GetApiToken();

        const string uriBase = "https://eastus.api.cognitive.microsoft.com/face/v1.0/detect";

        const string requestParameters = "returnFaceId=true&returnFaceLandmarks=false" +
            "&returnFaceAttributes=age,gender,headPose,smile,facialHair,glasses," +
            "emotion,hair,makeup,occlusion,accessories,blur,exposure,noise";

        const string uri = uriBase + "?" + requestParameters;

        /// <summary>
        /// Gets the analysis of the specified image by using the Face REST API.
        /// </summary>
        /// <param name="imageFilePath">The image file.</param>
        public static async Task<string> MakeAnalysisRequest(string imageFilePath)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiToken);

            // Request body.
            byte[] byteData = imageFilePath.GetImageAsByteArray();

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                var response = await client.PostAsync(uri, content);

                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
