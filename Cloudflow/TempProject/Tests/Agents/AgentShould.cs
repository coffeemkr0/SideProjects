﻿using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TempProject.Agents;
using TempProject.Jobs;
using TempProject.Steps;
using TempProject.Tests.Steps;
using TempProject.Tests.Triggers;
using TempProject.Triggers;

namespace TempProject.Tests.Agents
{
    [TestClass]
    public class AgentShould
    {
        private Agent _agent;
        private AgentMonitor _agentMonitor;

        private IJob GetTestJob()
        {
            var triggers = new List<ITrigger>
            {
                new ImmediateTrigger()
            };

            var steps = new List<IStep>
            {
                new TestStep()
            };

            var jobConfiguration = new JobConfiguration
            {
                Triggers = triggers,
                Steps = steps
            };

            return new Jobs.Job(jobConfiguration);
        }

        private IJob GetExceptionJob()
        {
            var triggers = new List<ITrigger>
            {
                new ImmediateTrigger()
            };

            var steps = new List<IStep>
            {
                new ExceptionTestStep()
            };

            var jobConfiguration = new JobConfiguration
            {
                Triggers = triggers,
                Steps = steps
            };

            return new Jobs.Job(jobConfiguration);
        }

        [TestInitialize]
        public void InitializeTest()
        {
            _agentMonitor = new AgentMonitor();
        }

        [TestMethod]
        public void Start()
        {
            _agent = new Agent(new List<IJob> {GetTestJob()});
            _agent.Start(_agentMonitor);
            Assert.IsTrue(_agentMonitor.OnAgentStartedCalled);
        }

        [TestMethod]
        public void Stop()
        {
            _agent = new Agent(new List<IJob> {GetTestJob()});
            _agent.Start(_agentMonitor);
            _agent.Stop();
            Assert.IsTrue(_agentMonitor.OnAgentStopCalled);
        }

        [TestMethod]
        public void PostActivityWhenJobStarts()
        {
            _agent = new Agent(new List<IJob> {GetTestJob()});
            _agent.Start(_agentMonitor);
            Assert.IsTrue(_agentMonitor.OnAgentActivityCalled);
        }

        [TestMethod]
        public void PostActivityWhenJobHasAnException()
        {
            _agent = new Agent(new List<IJob> {GetExceptionJob()});
            _agent.Start(_agentMonitor);
            Assert.IsTrue(_agentMonitor.OnAgentActivityCalled);
        }
    }
}