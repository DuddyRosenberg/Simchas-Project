using System;
using System.Collections.Generic;

namespace SimchasContributorsProject.Data
{
    public class Simcha
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public List<decimal> Contributions { get; set; } = new List<decimal>();
        public List<Contributor> Contributors { get;set; }
    }
}
