using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimchasContributorsProject.Data;

namespace SimchasContributorsProject.Models
{
    public class ContributorsViewModel
    {
        public List<Contributor> Contributors { get; set; }
        public string SuccessMessage { get; set; }
    }
}
