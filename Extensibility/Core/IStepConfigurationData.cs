﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public interface IStepConfigurationData
    {
        string Name { get; }

        Type Type { get; }
    }
}
