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

        private static ComputerVisionClient CreateClient()
        {
            var client = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(apiToken),
                new DelegatingHandler[] { });

            client.Endpoint = uriBase;

            return client;
        }

        public static async Task<ImageAnalysis> MakeAnalysisRequest(MediaFile file)
        {
            using (var stream = file.GetStream())
            {
                return await CreateClient().AnalyzeImageInStreamAsync(stream, features);
            }
        }

        public static async Task<OcrResult> MakeOcrRequest(MediaFile file)
        {
            using (var stream = file.GetStream())
            {
                return await CreateClient().RecognizePrintedTextInStreamAsync(true, stream, OcrLanguages.En);
            }
        }

        public static async Task<RecognitionResult> MakeTextRequest(MediaFile file)
        {
            using (var stream = file.GetStream())
            {
                var client = CreateClient();
                RecognizeTextInStreamHeaders headers = await client.RecognizeTextInStreamAsync(stream, TextRecognitionMode.Handwritten);
                if (headers?.OperationLocation == null) return null;

                // Extract the operation id from the url
                string operationId = headers.OperationLocation.Substring(headers.OperationLocation.Length - 36);
                TextOperationResult result = await client.GetTextOperationResultAsync(operationId);

                // Wait for the operation to complete
                int i = 0;
                int maxRetries = 10;
                while ((result.Status == TextOperationStatusCodes.Running ||
                        result.Status == TextOperationStatusCodes.NotStarted) && i++ < maxRetries)
                {
                    await Task.Delay(1000);

                    result = await client.GetTextOperationResultAsync(operationId);
                }
                return result.RecognitionResult;
            }
        }
    }
}
