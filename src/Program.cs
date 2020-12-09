using System;
using System.Threading.Tasks;
using AppCenterClient.Commands;

namespace AppCenterClient
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var commandProcessor = new CommandProcessor();
            await commandProcessor.Process(args);
        }
    }
}