﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloudflow.Core.Extensions.ExtensionAttributes
{
    /// <summary>
    /// Specifies the name of a resource entry to load and use as the text on the label of a property in
    /// the user interface for an extension's configuration
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class LabelTextResourceAttribute : Attribute
    {
        #region Properties
        /// <summary>
        /// Gets the name of the resource entry to use.
        /// </summary>
        public string ResourceName { get; }
        #endregion

        #region Constructors
        public LabelTextResourceAttribute(string resourceName)
        {
            this.ResourceName = resourceName;
        }
        #endregion
    }
}
