using System;
using System.Threading;
using System.Threading.Tasks;

namespace ARSignTranslator
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "AR Sign Translator (C# - VS Code)";

            Console.WriteLine("=======================================");
            Console.WriteLine("   AR Sign Translator - Phase 1 Demo   ");
            Console.WriteLine("   (Mock Tracking + Mock AI Classifier)");
            Console.WriteLine("=======================================");
            Console.WriteLine();

            var app = new App();

            using var cts = new CancellationTokenSource();

            // Stop app with CTRL+C
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                Console.WriteLine("\n[INFO] Stopping...");
                cts.Cancel();
            };

            try
            {
                await app.RunAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("[INFO] App stopped successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Unexpected crash:");
                Console.WriteLine(ex);
            }
        }
    }
}
