using System.Runtime;
using QuestPDF;
using QuestPDF.Infrastructure;

namespace Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            Settings.License = LicenseType.Community;
            var builder = Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(x => x.UseStartup<Startup>());
            var host = builder.Build();
            await host.RunAsync();
        }
    }
}