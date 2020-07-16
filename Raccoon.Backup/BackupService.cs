using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
                switch (settings.RunType)
                {
                    case RunType.PerMinute:
                        Copy(settings.OriginFolder, settings.DestinationFolder);
                        break;

                    default:
                    case RunType.ExactHour:
                        if (DateTime.Now.Hour == settings.ExactHour)
                            Copy(settings.OriginFolder, settings.DestinationFolder);
                        break;

                }
            }
            finally
            {
                _timer.Change(TimeSpan.FromSeconds(timePeriod), TimeSpan.FromSeconds(timePeriod));
            }
            await Task.CompletedTask;
        }
        private void Log() 
        {
            
            if (!File.Exists("./log.txt")) 
            {
                File.Create("./log.txt");
            }
         
            var log =  File.OpenRead("./log.txt");
          
            string content = string.Empty;
          
            using (StreamReader r = new StreamReader("./log.txt"))
            {
                content = r.ReadToEnd();
            }

            using (StreamWriter sw = new StreamWriter("./log.txt"))
            {
                sw.Write(content + "\n" + $"Data atualizada : {DateTime.Now.ToString("dd/MM/yyyy")} ");
            }
        }

        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            var diSource = new DirectoryInfo(sourceDirectory);
            var diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
    }
}
