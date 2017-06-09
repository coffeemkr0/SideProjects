﻿using Cloudflow.Core.Data.Shared.Models;
using Cloudflow.Core.Extensions;
using Cloudflow.Core.Extensions.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cloudflow.Web.ViewModels.Jobs
{
    public class EditViewModel
    {
        #region Properties
        public ExtensionConfigurationViewModel JobConfigurationViewModel { get; set; }

        public List<ExtensionConfigurationViewModel> TriggerConfigurationViewModels { get; set; }

        public List<ExtensionConfigurationViewModel> StepConfigurationViewModels { get; set; }
        #endregion

        #region Constructors
        public EditViewModel()
        {
            this.JobConfigurationViewModel = new ExtensionConfigurationViewModel();
            this.TriggerConfigurationViewModels = new List<ExtensionConfigurationViewModel>();
            this.StepConfigurationViewModels = new List<ExtensionConfigurationViewModel>();
        }
        #endregion

        #region Public Methods
        public static EditViewModel FromJobDefinition(JobDefinition jobDefinition)
        {
            var editViewModel = new EditViewModel();

            editViewModel.JobConfigurationViewModel.Id = jobDefinition.JobDefinitionId;
            editViewModel.JobConfigurationViewModel.ExtensionId = jobDefinition.JobConfigurationExtensionId;
            editViewModel.JobConfigurationViewModel.ExtensionAssemblyPath = jobDefinition.JobConfigurationExtensionAssemblyPath;

            var extensionConfigurationController = new ExtensionConfigurationController(jobDefinition.JobConfigurationExtensionId,
               jobDefinition.JobConfigurationExtensionAssemblyPath);
            editViewModel.JobConfigurationViewModel.Configuration = extensionConfigurationController.Load(jobDefinition.Configuration);

            foreach (var triggerDefinition in jobDefinition.TriggerDefinitions)
            {
                var triggerConfigurationViewModel = new ExtensionConfigurationViewModel();
                triggerConfigurationViewModel.Id = triggerDefinition.TriggerDefinitionId;
                triggerConfigurationViewModel.ExtensionId = triggerDefinition.TriggerConfigurationExtensionId;
                triggerConfigurationViewModel.ExtensionAssemblyPath = triggerDefinition.TriggerConfigurationExtensionAssemblyPath;

                extensionConfigurationController = new ExtensionConfigurationController(triggerDefinition.TriggerConfigurationExtensionId,
                    triggerDefinition.TriggerConfigurationExtensionAssemblyPath);
                triggerConfigurationViewModel.Configuration = extensionConfigurationController.Load(triggerDefinition.Configuration);

                editViewModel.TriggerConfigurationViewModels.Add(triggerConfigurationViewModel);
            }

            foreach (var stepDefinition in jobDefinition.StepDefinitions)
            {
                var stepConfigurationViewModel = new ExtensionConfigurationViewModel();
                stepConfigurationViewModel.Id = stepDefinition.StepDefinitionId;
                stepConfigurationViewModel.ExtensionId = stepDefinition.StepConfigurationExtensionId;
                stepConfigurationViewModel.ExtensionAssemblyPath = stepDefinition.StepConfigurationExtensionAssemblyPath;

                extensionConfigurationController = new ExtensionConfigurationController(stepDefinition.StepConfigurationExtensionId,
                    stepDefinition.StepConfigurationExtensionAssemblyPath);
                stepConfigurationViewModel.Configuration = extensionConfigurationController.Load(stepDefinition.Configuration);

                editViewModel.StepConfigurationViewModels.Add(stepConfigurationViewModel);
            }

            return editViewModel;
        }
        #endregion
    }
}