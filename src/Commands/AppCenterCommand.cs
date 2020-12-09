using System;
using System.Threading.Tasks;
using CommandLine;

namespace AppCenterClient.Commands
{
    public abstract class AppCenterCommand
    {
        [Option('u', "url", Required = false, Default = "https://api.appcenter.ms", HelpText = "AppCenter base url")]
        public string AppCenterBaseUrl { get; set; } = "https://api.appcenter.ms";

        [Option('t', "token", Required = true, HelpText = "AppCenter authentication token.")]
        public string Token { get; set; } = string.Empty;

        public Task Run()
        {
            return RunInternal();
        }

        protected TService GetService<TService>() where TService : AppCenterService => (TService) Activator.CreateInstance(typeof(TService), AppCenterBaseUrl, Token)!;

        protected AppCenterUploadApplicationService GetUploadApplicationService(string uploadDomain) => new AppCenterUploadApplicationService(uploadDomain, Token);

        protected abstract Task RunInternal();
    }
}