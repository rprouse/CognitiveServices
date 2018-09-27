using System;
using System.IO;
using System.Threading.Tasks;
using FaceRecognition.Extensions;
using FaceRecognition.Services;

namespace FaceRecognition
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            if(args.Length != 1)
            {
                Console.WriteLine(@"Usage: FaceRecognition C:\path\to\image.jpg");
                return;
            }

            string imageFilePath = args[0];

            Console.WriteLine($"Performing face detection on {imageFilePath}");
            Console.WriteLine();

            if (File.Exists(imageFilePath))
            {
                // Execute the REST API call.
                try
                {
                    string json = await FaceDetection.MakeAnalysisRequest(imageFilePath);

                    Console.WriteLine(json.PrettyPrint());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                Console.WriteLine("File does not exist");
            }

            Console.WriteLine();
            Console.WriteLine("=== Press ENTER to Exit ===");
            Console.ReadLine();
        }
    }
}
