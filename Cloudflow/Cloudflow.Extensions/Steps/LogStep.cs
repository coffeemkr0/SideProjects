﻿using Cloudflow.Core.Extensions;
using Cloudflow.Core.Extensions.ExtensionAttributes;
using Cloudflow.Core.Runtime;
using System;
using System.ComponentModel.Composition;

namespace Cloudflow.Extensions.Steps
{
    [ExportConfigurableExtension("43D6FD16-0344-4204-AEE9-A09B3998C017", typeof(LogStep), "191A3C1A-FD25-4790-8141-DFC132DA4970",
        "LogStepName", "LogStepDescription")]
    public class LogStep : Step
    {
        #region Constructors
        [ImportingConstructor]
        public LogStep([Import("ExtensionConfiguration")]ExtensionConfiguration stepConfiguration) : base(stepConfiguration)
        {

        }
        #endregion

        #region  Private Methods

        #endregion

        #region Public Methods
        public override void Execute()
        {
            try
            {
                this.StepLogger.Info(((LogStepConfiguration)this.StepConfiguration).LogMessage);
            }
            catch (Exception ex)
            {
                this.StepLogger.Error(ex);
                OnStepOutput(OutputEventLevels.Error, ex.ToString());
            }
        }
        #endregion
    }
}
