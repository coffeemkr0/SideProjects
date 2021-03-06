﻿using Cloudflow.Core.Data.Shared.Models;
using System;

namespace Cloudflow.Web.ViewModels.Jobs
{
    public class TriggerViewModel
    {
        #region Properties
        public Guid TriggerDefinitionId { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        public int Index { get; set; }

        public TriggerConfigurationViewModel ConfigurationViewModel { get; set; }

        public ConditionCollectionViewModel Conditions { get; set; }
        #endregion

        #region Constructors
        public TriggerViewModel()
        {
            Conditions = new ConditionCollectionViewModel();
        }
        #endregion

        #region Public Methods
        public static TriggerViewModel FromTriggerDefinition(TriggerDefinition triggerDefinition, int index)
        {
            var model = new TriggerViewModel
            {
                TriggerDefinitionId = triggerDefinition.TriggerDefinitionId,
                Index = index
            };

            model.ConfigurationViewModel = TriggerConfigurationViewModel.FromTriggerDefinition(triggerDefinition);
            model.Conditions = ConditionCollectionViewModel.FromTriggerDefinition(triggerDefinition, index);

            return model;
        }
        #endregion
    }
}