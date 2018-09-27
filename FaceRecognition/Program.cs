using System;
using System.IO;
using System.Threading.Tasks;
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

            if (File.Exists(imageFilePath))
            {
                // Execute the REST API call.
                try
                {
                    await FaceDetection.MakeAnalysisRequest(imageFilePath);
                }
                catch (Exception e)
                {
                    Console.WriteLine();
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Invalid file path.");
            }

            Console.WriteLine();
            Console.WriteLine("=== Press ENTER to Exit ===");
            Console.ReadLine();
        }
    }
}
