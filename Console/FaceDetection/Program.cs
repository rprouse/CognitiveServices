using System;
using System.IO;
using System.Threading.Tasks;
using FaceDetection.Extensions;
using FaceDetection.Services;

namespace FaceDetection
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            if(args.Length != 1)
            {
                Console.WriteLine(@"Usage: FaceDetection C:\path\to\image.jpg");
                return;
            }

            string imageFilePath = args[0];

            Console.WriteLine($"Performing face detection on {imageFilePath}");
            Console.WriteLine();

            if (!File.Exists(imageFilePath))
            {
                Console.WriteLine("File does not exist");
                return;
            }

            try
            {
                string json = await FaceDetectionApi.MakeAnalysisRequest(imageFilePath);

                Console.WriteLine(json.PrettyPrint());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine();
            Console.WriteLine("=== Press ENTER to Exit ===");
            Console.ReadLine();
        }
    }
}
