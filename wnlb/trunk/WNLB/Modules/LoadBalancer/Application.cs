﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WNLB.Modules.LoadBalancer
{
    public interface Application
    {
        String AppName { get; }
        String AppPath { get; }
        RequestRouter RequestRouter { get; }
    }
}
