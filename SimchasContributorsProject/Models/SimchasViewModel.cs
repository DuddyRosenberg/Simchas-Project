using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimchasContributorsProject.Data;

namespace SimchasContributorsProject.Models
{
    public class SimchasViewModel
    {
        public List<Simcha> Simchas { get; set; }
        public int TotalContributors { get; set; }
    }
}
