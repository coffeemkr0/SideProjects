﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cloudflow.Core.Data.Agent;
using Cloudflow.Core.Data.Agent.Models;
using Cloudflow.Core.Data.Shared.Models;
using Cloudflow.Core.Extensions;
using Cloudflow.Core.Extensions.Controllers;
using log4net;
using Microsoft.AspNet.SignalR;

namespace Cloudflow.Core.Agents
{
    public class AgentHub : Hub, IAgentMonitor
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Agent _agent;
        private static readonly object AgentControlSynch = new object();
        private static readonly object PublishJobSynch = new object();

        #region Private Methods

        public void AgentStatusChanged(AgentStatus status)
        {
            Clients.All.updateStatus(status);
        }

        public void RunStatusChanged(Run run)
        {
            Clients.All.runStatusChanged(run);
        }

        public void StepOutput(Job job, Step step, OutputEventLevels level, string message)
        {
            switch (level)
            {
                case OutputEventLevels.Debug:
                    Logger.Debug($"[Step Output] {message}");
                    break;
                case OutputEventLevels.Info:
                    Logger.Info($"[Step Output] {message}");
                    break;
                case OutputEventLevels.Warning:
                    Logger.Warn($"[Step Output] {message}");
                    break;
                case OutputEventLevels.Error:
                    Logger.Error($"[Step Output] {message}");
                    break;
                case OutputEventLevels.Fatal:
                    Logger.Fatal($"[Step Output] {message}");
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region Public Methods

        public AgentStatus GetAgentStatus()
        {
            try
            {
                lock (AgentControlSynch)
                {
                    if (_agent == null)
                        return new AgentStatus
                        {
                            Status = AgentStatus.AgentStatuses.NotRunning
                        };
                    return _agent.AgentStatus;
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            return null;
        }

        public void PublishJob(JobDefinition jobDefinition)
        {
            lock (PublishJobSynch)
            {
                using (var agentDbContext = new AgentDbContext())
                {
                    var existingJobDefinition =
                        agentDbContext.JobDefinitions.FirstOrDefault(i =>
                            i.JobDefinitionId == jobDefinition.JobDefinitionId);
                    if (existingJobDefinition == null)
                    {
                        agentDbContext.JobDefinitions.Add(jobDefinition);
                        agentDbContext.SaveChanges();
                    }
                    else
                    {
                        agentDbContext.JobDefinitions.Remove(existingJobDefinition);
                        agentDbContext.JobDefinitions.Add(jobDefinition);
                        agentDbContext.SaveChanges();
                    }
                }

                if (_agent != null)
                {
                    StopAgent();
                    StartAgent();
                }
            }
        }

        public void StartAgent()
        {
            try
            {
                lock (AgentControlSynch)
                {
                    if (_agent == null)
                    {
                        Logger.Info("Starting agent");

                        _agent = new Agent(this);
                        _agent.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }
        }

        public void StopAgent()
        {
            try
            {
                lock (AgentControlSynch)
                {
                    if (_agent != null)
                    {
                        _agent.Stop();
                        _agent = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }
        }

        public List<Run> GetCompletedRuns(int startIndex, int pageSize)
        {
            using (var agentDbContext = new AgentDbContext())
            {
                return agentDbContext.Runs.Where(i => i.Status == Run.RunStatuses.Completed ||
                                                      i.Status == Run.RunStatuses.Completed ||
                                                      i.Status == Run.RunStatuses.Failed ||
                                                      i.Status == Run.RunStatuses.Canceled)
                    .OrderByDescending(i => i.DateEnded)
                    .Skip(startIndex).Take(pageSize).ToList();
            }
        }

        public List<Run> GetQueuedRuns()
        {
            lock (AgentControlSynch)
            {
                if (_agent == null)
                    return new List<Run>();
                return _agent.GetQueuedRuns();
            }
        }

        #endregion
    }
}