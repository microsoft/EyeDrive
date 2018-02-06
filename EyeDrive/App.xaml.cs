using Microsoft.Shell;
using Squirrel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;

namespace EyeDrive
{
    public partial class App : Application, ISingleInstanceApp
    {
        public static string AppName =
            Assembly.GetEntryAssembly().GetName().Name + " " +
            Assembly.GetEntryAssembly().GetName().Version + " " +
            ((AssemblyInformationalVersionAttribute)Attribute.GetCustomAttribute(Assembly.GetEntryAssembly(), typeof(AssemblyInformationalVersionAttribute))).InformationalVersion;

        internal static readonly long AppStartTime = DateTime.UtcNow.Ticks;

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            return true;
        }

        [STAThread]
        public static void Main()
        {
            HandleSquirrelEvents();

            if (SingleInstance<App>.InitializeAsFirstInstance("{35BBF621-4276-48A8-AEEB-71011A44C476}"))
            {
                var application = new App();

                application.InitializeComponent();
                application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
            else
            {
                MessageBox.Show($"{Assembly.GetEntryAssembly().GetName().Name} is already running.");
            }
        }

        public App()
        {
            DispatcherUnhandledException += (o, e) =>
            {
                var ex = e.Exception;

                // Grabbing the InnerException if it's there to get closer to the source.
                if (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }

                MessageBox.Show(ex.ToString());
            };

            Exit += OnExit;
        }

        private static bool IsPrivateBuild()
        {
            var appVersionInfo = ((AssemblyInformationalVersionAttribute)Attribute.GetCustomAttribute(Assembly.GetEntryAssembly(), typeof(AssemblyInformationalVersionAttribute))).InformationalVersion;
            return (appVersionInfo == "private");
        }

        private static void HandleSquirrelEvents()
        {
            if (!IsPrivateBuild())
            {
                using (var mgr = new UpdateManager("https://foo.com/bar"))
                {
                    // Note, in most of these scenarios, the app exits after this method
                    // completes!
                    SquirrelAwareApp.HandleEvents(
                      onInitialInstall: v => CreateShortcutForThisExe(mgr),
                      onAppUpdate: v => CreateShortcutForThisExe(mgr),
                      onAppUninstall: v => RemoveShortcutForThisExe(mgr));
                }
            }
        }

        static void CreateShortcutForThisExe(UpdateManager mgr)
        {
            mgr.CreateShortcutsForExecutable(Path.GetFileName(
                Assembly.GetEntryAssembly().Location),
                ShortcutLocation.Desktop | ShortcutLocation.StartMenu,
                Environment.CommandLine.Contains("squirrel-install") == false,
                null, null);
        }

        static void RemoveShortcutForThisExe(UpdateManager mgr)
        {
            mgr.RemoveShortcutsForExecutable(
                Path.GetFileName(Assembly.GetEntryAssembly().Location),
                ShortcutLocation.Desktop | ShortcutLocation.StartMenu);
        }

        private void OnExit(object o, object args)
        {
            //  Duration = DateTime.UtcNow.Ticks - AppStartTime,
        }
    }
}