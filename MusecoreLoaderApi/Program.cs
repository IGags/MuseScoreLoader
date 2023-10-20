using System.Runtime;
using Logic.Services;

namespace Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            var builder = Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(x => x.UseStartup<Startup>());
            var host = builder.Build();
            await host.RunAsync();
        }
    }
}