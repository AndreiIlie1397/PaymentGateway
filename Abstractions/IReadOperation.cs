﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions
{
    public interface IReadOperation<TInput, TResult>
    {
        public TResult PerformOperation(TInput query);
    }
}
