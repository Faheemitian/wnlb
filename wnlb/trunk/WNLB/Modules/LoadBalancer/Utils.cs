﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WNLB.Modules.LoadBalancer
{
    public static class Utils
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }

    }
}