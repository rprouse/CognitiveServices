using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FaceRecognition.Extensions;

namespace FaceRecognition.Services
{
    public static class FaceDetection
    {
        static readonly string apiToken = TokenReader.GetApiToken();

        const string uriBase = "https://eastus.api.cognitive.microsoft.com/face/v1.0/detect";

        /// <summary>
        /// Gets the analysis of the specified image by using the Face REST API.
        /// </summary>
        /// <param name="imageFilePath">The image file.</param>
        public static async Task MakeAnalysisRequest(string imageFilePath)
        {
            var client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiToken);

            // Request parameters. A third optional parameter is "details".
            string requestParameters = "returnFaceId=true&returnFaceLandmarks=false" +
                "&returnFaceAttributes=age,gender,headPose,smile,facialHair,glasses," +
                "emotion,hair,makeup,occlusion,accessories,blur,exposure,noise";

            // Assemble the URI for the REST API Call.
            string uri = uriBase + "?" + requestParameters;

            HttpResponseMessage response;

            // Request body. Posts a locally stored JPEG image.
            byte[] byteData = imageFilePath.GetImageAsByteArray();

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                // Execute the REST API call.
                response = await client.PostAsync(uri, content);

                // Get the JSON response.
                string json = await response.Content.ReadAsStringAsync();

                // Display the JSON response.
                Console.WriteLine("\nResponse:\n");
                Console.WriteLine(json.PrettyPrint());
                Console.WriteLine("\nPress Enter to exit...");
            }
        }
    }
}
