using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace sprintconfigcli
{
    public class Iterations
    {
        readonly SprintGeneratorOptions Options;

        public Iterations(SprintGeneratorOptions options)
        {
            Options = options;
        }

        public async Task<bool> RegisterIterations()
        {
            bool result = true;

            VssConnection connection = new VssConnection(new Uri(Options.Url),
                new VssBasicCredential(string.Empty, Options.PAT));

            //  get VSTS's project collection
            ProjectHttpClient projectClient = connection.GetClient<ProjectHttpClient>();
            IEnumerable<TeamProjectReference> projects = projectClient.GetProjects().Result;
            var targetProject =
                projects.FirstOrDefault(p => string.CompareOrdinal(p.Name, Options.Project) == 0);
            if(targetProject == null)
            {
                Console.WriteLine($"Project {Options.Project} not found in Server {Options.Url}");
                return false;
            }
          
            // get team of project setting
            TeamHttpClient teamClient = connection.GetClient<TeamHttpClient>();
            var teams = await teamClient.GetTeamsAsync(targetProject.Id.ToString());

            // found specific team
            var team = teams.FirstOrDefault(p => string.CompareOrdinal(p.Name, Options.Team) == 0);
            if(team == null)
            {
                Console.WriteLine($"Team {Options.Team} not found in Project {Options.Project}");
                return false;
            }
            WorkHttpClient workHttpClient = connection.GetClient<WorkHttpClient>();

            // get team's iteration 
            var teamContext = new TeamContext(targetProject.Id, team.Id);
            var teamSetting = await workHttpClient.GetTeamIterationsAsync(teamContext);
            if(teamSetting == null)
            {
                Console.WriteLine("Can\'t get team Iteration");
                return false;
            }
            try
            {
                int SprintCounter = Options.SprintStartIndex;
                do
                {
                    var teamIteration = new TeamSettingsIteration
                    {
                        // these date values looks like ignore.
                        Attributes = new TeamIterationAttributes
                        {
                            StartDate = DateTime.Today,
                            FinishDate = DateTime.Today
                        },
                        Name = string.Format($"Sprint {SprintCounter}"),
                        Path = string.Format($"{Options.Project}\\{Options.SprintPrefix} {SprintCounter}"),
                    };

                    // add team iteration from Project iteration.
                    var execResult = await workHttpClient.PostTeamIterationAsync(teamIteration, teamContext);
                    if (execResult != null)
                    {
                        SprintCounter++;
                        Console.WriteLine($"Sprint {execResult.Path} add team iteration");
                    }
                } while (true);
            }
            catch (Exception ex)
            {
                // Exception is ignore, because PostTeamIteration Async method can't return error value.
            }
            return result;
        }
    }
}
