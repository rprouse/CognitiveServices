using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Plugin.Media.Abstractions;

namespace CognitiveServices.Services
{
    public static class FaceDetection
    {
        // Add a class Secret with a property FaceApiToken with the API Token from Azure
        static readonly string apiToken = Secret.FaceApiToken;

        const string uriBase = "https://eastus.api.cognitive.microsoft.com";

        static readonly FaceAttributeType[] faceAttributes =
        {
            FaceAttributeType.Accessories,
            FaceAttributeType.Age,
            FaceAttributeType.Emotion,
            FaceAttributeType.FacialHair,
            FaceAttributeType.Gender,
            FaceAttributeType.Glasses,
            FaceAttributeType.Hair,
            FaceAttributeType.Makeup,
            FaceAttributeType.Smile
        };

        public static async Task<IList<DetectedFace>> MakeAnalysisRequest(MediaFile file)
        {
            var client = new FaceClient(
                new ApiKeyServiceClientCredentials(apiToken),
                new DelegatingHandler[] { });

            client.Endpoint = uriBase;

            using (var stream = file.GetStream())
            {
                return await client.Face.DetectWithStreamAsync(stream, true, false, faceAttributes);
            }
        }
    }
}
