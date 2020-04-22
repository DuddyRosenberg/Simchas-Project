using System;
using System.Collections.Generic;
using System.Text;

namespace SimchasContributorsProject.Data
{
    public class Contributor
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CellNumber { get; set; }
        public decimal Balance { get; set; }
        public bool AlwaysInclude { get; set; }
        public decimal Contribution { get; set; }
    }
}
