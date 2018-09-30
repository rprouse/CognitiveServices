using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace CognitiveServices.Services
{
    public class FaceDetection
    {
        // Add a class Secret with a property ApiToken with the API Token from Azure
        static readonly string apiToken = Secret.ApiToken;

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

        Page _parent;

        public FaceDetection(Page parent)
        {
            _parent = parent;
        }

        public async Task<IList<DetectedFace>> MakeAnalysisRequest(MediaFile file)
        {
            var client = new FaceClient(
                new ApiKeyServiceClientCredentials(apiToken),
                new System.Net.Http.DelegatingHandler[] { });
            client.Endpoint = uriBase;

            try
            {
                using (var stream = file.GetStream())
                {
                    return await client.Face.DetectWithStreamAsync(stream, true, false, faceAttributes);
                }
            }
            catch (APIErrorException e)
            {
                await _parent.DisplayAlert("Analysis Error", e.Message, "OK");
            }
            return new List<DetectedFace>();
        }
    }
}
