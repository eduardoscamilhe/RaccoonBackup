using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Topshelf;

namespace Raccoon.Backup
{
    public class BackupService : ServiceControl
    {
        private Timer _timer;

        public bool Start(HostControl hostControl)
        {

            _timer = new Timer(callback: async c => await ExecuteAsync(),

            //State: Objeto que carrega informações que podem ser utilizadas pelo método de callback (ExecuteAsync)

              state: null,

              //dueTime: Delay para inicializar o ExecuteAsync

              dueTime: TimeSpan.Zero,

              //Period: Frequência que o ExecuteAsync deverá ser executado

              period: TimeSpan.FromSeconds(60));



            return true;

        }
        public bool Stop(HostControl hostControl)
        {
            _timer.Dispose();
            return true;
        }


        private async Task ExecuteAsync()
        {
            AppSettings settings = new AppSettings();
            using (StreamReader r = new StreamReader("./appsettings.json"))
            {
                string json = r.ReadToEnd();
                settings = JsonConvert.DeserializeObject<AppSettings>(json);
            }
            var timePeriod = settings.MinutePeriod * 60;

            try
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);

                if (settings.ExactHour != 0 && DateTime.Now.Hour == settings.ExactHour)
                {
                    Copy(settings.OriginFolder, settings.DestinationFolder);
                }
                else if (settings.MinutePeriod != 0)
                {
                    Copy(settings.OriginFolder, settings.DestinationFolder);
                }

            }
            finally
            {
                _timer.Change(TimeSpan.FromSeconds(timePeriod), TimeSpan.FromSeconds(timePeriod));
            }
            await Task.CompletedTask;
        }

        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            var listSource = sourceDirectory.Split(';');
            foreach (var item in listSource)
            {
                var diSource = new DirectoryInfo(item);
                var diTarget = new DirectoryInfo(targetDirectory);

                CopyAll(diSource, diTarget);
            }
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target, bool isSubDir = false)
        {
            var dirName = !isSubDir ? Path.GetDirectoryName(source.FullName).Split("\\").LastOrDefault() : string.Empty;
            var fulPath = Path.Combine(target.FullName, dirName);
            Directory.CreateDirectory(fulPath);
            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, dirName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(Path.Combine(dirName, diSourceSubDir.Name));
                CopyAll(diSourceSubDir, nextTargetSubDir, true);
            }
        }
    }
}
