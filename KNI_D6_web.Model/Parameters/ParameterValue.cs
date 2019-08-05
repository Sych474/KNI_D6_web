﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KNI_D6_web.Model.Parameters
{
    public class ParameterValue
    {
        public int ParameterId { get; set; }

        public Parameter Parameter { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }

        public int? IntValue { get; set; }

        public string StringValue { get; set; }

        public double? DoubleValue { get; set; }

        public DateTime DateValue { get; set; }
    }
}
