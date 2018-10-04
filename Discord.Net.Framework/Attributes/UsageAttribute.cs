﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Net.Framework.Attributes
{
    public class UsageAttribute : Attribute
    {
        public UsageAttribute(string text)
        {
            Text = text;
        }
        public string Text { get; set; }
    }
}
