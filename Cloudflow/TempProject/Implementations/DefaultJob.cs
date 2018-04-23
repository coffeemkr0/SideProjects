﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using TempProject.Interfaces;
using TempProject.Tests.Job;
using TempProject.Tests.Steps;
using TempProject.Tests.Triggers;

namespace TempProject.Implementations
{
    [Export(typeof(IExtension))]
    [ExportMetadata("ExtensionId", ExtensionId)]
    public class DefaultJob : IJob, ITriggerMonitor, IStepMonitor, IExtension
    {
        public const string ExtensionId = "{8BFDC5EA-9890-463E-822F-5A0704846660}";

        private readonly DefaultJobConfiguration _configuration;
        private IJobMonitor _jobMonitor;

        [ImportingConstructor]
        public DefaultJob([Import("Configuration")]IExtension configuration)
        {
            _configuration = (DefaultJobConfiguration)configuration;
        }

        public void Stop()
        {
            foreach (var trigger in _configuration.Triggers) trigger.Stop();

            _jobMonitor.OnJobStopped(this);
        }

        public void Dispose()
        {
            foreach (var trigger in _configuration.Triggers) trigger.Dispose();
        }

        public void Start(IJobMonitor jobMonitor)
        {
            _jobMonitor = jobMonitor;

            foreach (var trigger in _configuration.Triggers) trigger.Start(this);

            _jobMonitor.OnJobStarted(this);
        }

        public void OnStepStarted(IStep step)
        {
            _jobMonitor.OnJobActivity(this, "Step started");
        }

        public void OnStepActivity(IStep step, string activity)
        {
            _jobMonitor.OnJobActivity(this, $"Step activity - {activity}");
        }

        public void OnStepCompleted(IStep step)
        {
            _jobMonitor.OnJobActivity(this, "Step completed");
        }

        public void OnTriggerFired(ITrigger trigger)
        {
            _jobMonitor.OnJobActivity(this, "Trigger fired");

            foreach (var step in _configuration.Steps)
                try
                {
                    step.Execute(this);
                }
                catch (Exception e)
                {
                    _jobMonitor.OnException(this, e);
                }
        }

        public void OnTriggerStarted(ITrigger trigger)
        {
            _jobMonitor.OnJobActivity(this, "Trigger started");
        }

        public void OnTriggerStopped(ITrigger trigger)
        {
            _jobMonitor.OnJobActivity(this, "Trigger stopped");
        }
    }
}