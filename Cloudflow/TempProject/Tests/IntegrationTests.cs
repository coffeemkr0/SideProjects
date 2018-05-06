﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TempProject.Implementations;
using TempProject.Interfaces;
using TempProject.Tests.Agents;
using TempProject.Tests.Steps;
using TempProject.Tests.Triggers;

namespace TempProject.Tests
{
    [TestClass]
    public class IntegrationTests
    {
        private Agent _agent;
        private AgentMonitor _agentMonitor;

        [TestInitialize]
        public void InitializeTest()
        {
            var jobDefinition = new JobDefinition
            {
                Name = "Integration Test Job"
            };

            jobDefinition.TriggerDefinitions.Add(new TriggerDefinition
            {
                AssemblyPath = this.GetType().Assembly.CodeBase,
                ExtensionId = Guid.Parse(ImmediateTrigger.ExtensionId),
                Name = "Immediate Trigger"
            });

            jobDefinition.StepDefinitions.Add(new StepDefinition
            {
                AssemblyPath = this.GetType().Assembly.CodeBase,
                ExtensionId = Guid.Parse(TestStep.ExtensionId),
                Name = "Test Step"
            });

            jobDefinition.StepDefinitions.Add(new StepDefinition
            {
                AssemblyPath = this.GetType().Assembly.CodeBase,
                ExtensionId = Guid.Parse(ConfigurableTestStep.ExtensionId),
                ConfigurationExtensionId = Guid.Parse(ConfigurableStepConfiguration.ExtensionId),
                Configuration = "{\"Message\":\"Integration Test\"}",
                Name = "Configurable Test Step"
            });

            var extensionService = new ExtensionService();
            var jobConfigurationFactory = new JobConfigurationFactory(jobDefinition, extensionService);
            var jobConfiguration = jobConfigurationFactory.CreateJobConfiguration();

            var job = new Implementations.Job(jobConfiguration);
            var jobs = new List<IJob>
            {
                job
            };

            _agent = new Agent(jobs);

            _agentMonitor = new AgentMonitor();
        }

        [TestMethod]
        public void AgentStartsRunsAndStops()
        {
            _agent.Start(_agentMonitor);
            _agent.Stop();

            Assert.IsTrue(_agentMonitor.OnAgentStartedCalled);
            Assert.IsTrue(_agentMonitor.OnAgentActivityCalled);
            Assert.IsTrue(_agentMonitor.OnAgentStopCalled);
        }
    }
}
