using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimchasContributorsProject.Data;

namespace SimchasContributorsProject.Models
{
    public class ShowHistoryViewModel
    {
        public Contributor Contributor { get; set; }
        public List<Transection> Transections { get; set; }
    }
}
