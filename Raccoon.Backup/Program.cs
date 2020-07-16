using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Threading.Tasks;
using Topshelf;

namespace Raccoon.Backup
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                 .UseContentRoot(Directory.GetCurrentDirectory())
                 .ConfigureServices((hostBuilderContext, services) =>
                 {
                     services.AddSingleton(typeof(BackupService));

                 });



            HostFactory.Run(x =>
            {
                x.Service<BackupService>(sc =>
                {
                    sc.ConstructUsing(s =>
                    host.Build().Services.GetRequiredService<BackupService>());
                    sc.WhenStarted((s, c) => s.Start(c));
                    sc.WhenStopped((s, c) => s.Stop(c));

                });

                x.RunAsLocalSystem()
                  .DependsOnEventLog()
                 .StartAutomatically()
                 .EnableServiceRecovery(rc => rc.RestartService(1));

                x.SetDescription("Servico de backup do MHW");
                x.SetDisplayName("Raccoon Backup");
                x.SetServiceName("Raccoon Backup");
                x.StartAutomatically();


            });
            await Task.CompletedTask;
        }

    }

}