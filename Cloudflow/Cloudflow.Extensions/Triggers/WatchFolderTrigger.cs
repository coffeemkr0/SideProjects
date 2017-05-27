﻿using Cloudflow.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cloudflow.Core.Configuration;
using System.ComponentModel.Composition;
using System.IO;

namespace Cloudflow.Extensions.Triggers
{
    [ExportExtension("DAD72E34-F229-43B6-8B4E-AEDA64BCCF4E", typeof(WatchFolderTrigger))]
    public class WatchFolderTrigger : Trigger
    {
        FileSystemWatcher _fileSystemWatcher;

        [ImportingConstructor]
        public WatchFolderTrigger([Import("ExtensionConfiguration")]ExtensionConfiguration triggerConfiguration) : base(triggerConfiguration)
        {
            _fileSystemWatcher = new FileSystemWatcher();

            var configuration = (WatchFolderTriggerConfiguration)triggerConfiguration;
            _fileSystemWatcher.Path = configuration.WatchFolderPath;
            _fileSystemWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            _fileSystemWatcher.Filter = string.Join(";", configuration.FileNameMasks.ToArray());

            _fileSystemWatcher.Created += _fileSystemWatcher_Created;
        }

        private void _fileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            var triggerData = new Dictionary<string, object>();
            triggerData.Add("FilePath", e.FullPath);
            OnTriggerFired(triggerData);
        }

        public override void Start()
        {
            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        public override void Stop()
        {
            _fileSystemWatcher.EnableRaisingEvents = false;
        }
    }
}