using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowManager
{
    public class WorkflowDefinition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Version { get; set; }
        public InstanceState State { get; set; }
        public Activity Activity { get; set; }
    }
}
