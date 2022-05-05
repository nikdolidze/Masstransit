﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Contracts
{
    public interface CheckOrder
    {
        Guid OrderId { get; }

    }
    public interface OrderNotFound
    {
        Guid OrderId { get; }
    }
}
