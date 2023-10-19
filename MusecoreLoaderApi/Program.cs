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
            var isFileExist = File.Exists("key.txt");
            if (isFileExist)
            {
                KeyStorageService.Key = await File.ReadAllTextAsync("key.txt");
            }
            else
            {
                Console.WriteLine("Введите ключ api");
                KeyStorageService.Key = Console.ReadLine();
                await File.WriteAllTextAsync("key.txt", KeyStorageService.Key);
            }
            var host = builder.Build();
            await host.RunAsync();
        }
    }
}