using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Net.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class HelpCategoryAttribute : Attribute
    {
        public HelpCategoryAttribute(string category)
        {
            Category = category;
        }
        public string Category { get; set; }
    }
}
