using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Options;
using System.Text;
using System.Threading.Tasks;

namespace sprintconfigcli
{
    public enum ErrorCode
    {
        Normal = 0,
        CommandLineFail = 1,
        AuthenticateFail = 2,
        ProjectNotFound = 3,
    }
    public struct SprintGeneratorOptions
    {
        public string Url { get; set; }
        public string PAT { get; set; }
        public string Project { get; set; }
        public string Team { get; set; }
        public string SprintPrefix { get; set; }
        public int SprintStartIndex { get; set; }
    }
    public class SprintConfiguratorCLI
    {
        static OptionSet Options;
        public static SprintGeneratorOptions CommandLineOption;
        static bool ShowHelp = false;

        static void OptionInitializer()
        {
            Options = new OptionSet()
                .Add("url=", "VSTS/TFS URL(https://youraccount.visualstudio.com/)", v => CommandLineOption.Url = v)
                .Add("PAT=", "VSTS Personal Access Token", v => CommandLineOption.PAT = v)
                .Add("Project=|p=", "VSTS/TFS Project Name", v => CommandLineOption.Project = v)
                .Add("Team=|t=", "VSTS/TFS Team Name(Optional)", v => CommandLineOption.Team = v)
                .Add("Index|i=", "VSTS/TFS Start Project Iteration Index(ex:You want to start Sprint 3, you must input i=3 )", (int v) => CommandLineOption.SprintStartIndex = v)
                .Add("SprintPrefix=|f=", "Sprint Name Prefix(ex:SprintXXX)", v => CommandLineOption.SprintPrefix = v)
                .Add("Help=|h=", "show help message", v => ShowHelp = v != null);
        }

        public static bool ParseCommandLine(string[] args)
        {
            OptionInitializer();

            if(args.Length == 0)
            {
                return false;
            }

            try
            {
                Options.Parse(args);
            }

            catch (OptionException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        static async Task<int> Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            //parse command line
            var result = ParseCommandLine(args);
            if (!result)
            {
                Console.WriteLine("Command Line argument error!");
                return (int)ErrorCode.CommandLineFail;
            }

            // authorize to host
            var iteration = new Iterations(CommandLineOption);
            result = await iteration.RegisterIterations();
            if (!result)
            {
                Console.WriteLine("Iteration Sync from Project Failed");
                return (int)ErrorCode.AuthenticateFail;
            }
            return (int)ErrorCode.Normal;
        }
        //  catch unhandled Exception
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("Something happed Error. Please check ");
        }
    }
}
