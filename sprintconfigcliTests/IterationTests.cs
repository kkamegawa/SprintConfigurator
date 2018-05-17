using Microsoft.VisualStudio.TestTools.UnitTesting;
using sprintconfigcli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace sprintconfigcli.Tests
{
    [TestClass()]
    public class AuthenticationTests
    {
        SprintGeneratorOptions SprintGeneratorOptions;

        [TestInitialize]
        public void TestInit()
        {
            SprintGeneratorOptions = new SprintGeneratorOptions();
            var fileName = @".\secretdata.json";
            if (File.Exists(fileName))
            {
                var jsontext = File.ReadAllText(fileName);
                var jsonSecret = JsonConvert.DeserializeObject<SecretData>(jsontext);
                SprintGeneratorOptions.Url = jsonSecret.url;
                SprintGeneratorOptions.PAT = jsonSecret.PAT;
                SprintGeneratorOptions.Project = jsonSecret.Project;
                SprintGeneratorOptions.Team = jsonSecret.Team;
            }
            else
            {
                SprintGeneratorOptions.Url = Environment.GetEnvironmentVariable("SPCONFIG_TARGETURL");
                SprintGeneratorOptions.PAT = Environment.GetEnvironmentVariable("SPCONFIG_PAT");
                SprintGeneratorOptions.Project = Environment.GetEnvironmentVariable("SPCONFIG_PROJECT");
                SprintGeneratorOptions.Team = Environment.GetEnvironmentVariable("SPCONFIG_TEAM");
            }
            SprintGeneratorOptions.SprintStartIndex = 7;
        }

        [TestMethod]
        public void NothingUrlTest()
        {
            SprintGeneratorOptions.Url = "https://nothing.exsample.com";
            SprintConfiguratorCLI.CommandLineOption = SprintGeneratorOptions;
            
            var iteration = new Iterations(SprintGeneratorOptions);

            var result = iteration.RegisterIterations();
            Assert.IsFalse(result.Result, $"Failed to Authentication for {SprintGeneratorOptions.Url}");
        }

        [TestMethod]
        public void NothingProjectTest()
        {
            SprintGeneratorOptions.Project = "NotExistProject";
            SprintConfiguratorCLI.CommandLineOption = SprintGeneratorOptions;

            var iteration = new Iterations(SprintGeneratorOptions);

            var result = iteration.RegisterIterations();
            Assert.IsFalse(result.Result);
        }

        [TestMethod]
        public void NothingTeamTest()
        {
            SprintGeneratorOptions.Team = "NotExistTeam";
            SprintConfiguratorCLI.CommandLineOption = SprintGeneratorOptions;

            var iteration = new Iterations(SprintGeneratorOptions);

            var result = iteration.RegisterIterations();
            Assert.IsFalse(result.Result);
        }

        [TestMethod()]
        public void IterationRegisterTest()
        {
            SprintConfiguratorCLI.CommandLineOption = SprintGeneratorOptions;
            var iteration = new Iterations(SprintGeneratorOptions);

            var result = iteration.RegisterIterations();
            Assert.IsTrue(result.Result, $"Failed to Authentication for {SprintGeneratorOptions.Url}");
            
        }
    }
}