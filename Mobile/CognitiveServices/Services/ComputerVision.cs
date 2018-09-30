using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Plugin.Media.Abstractions;

namespace CognitiveServices.Services
{
    public static class ComputerVision
    {
        // Add a class Secret with a property ComputerVisionApiToken with the API Token from Azure
        static readonly string apiToken = Secret.ComputerVisionApiToken;

        const string uriBase = "https://eastus.api.cognitive.microsoft.com";

        static readonly List<VisualFeatureTypes> features =
            new List<VisualFeatureTypes>()
        {
            VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
            VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
            VisualFeatureTypes.Tags
        };

        public static async Task<ImageAnalysis> MakeAnalysisRequest(MediaFile file)
        {
            var client = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(apiToken),
                new DelegatingHandler[] { });

            client.Endpoint = uriBase;

            using (var stream = file.GetStream())
            {
                return await client.AnalyzeImageInStreamAsync(stream, features);
            }
        }
    }
}
