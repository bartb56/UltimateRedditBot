using System.Threading.Tasks;

namespace UltimateRedditBot
{
    class Program
    {
        static Task Main(string[] args)
           => Startup.RunAsync(args);
    }
}
