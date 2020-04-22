using System;
using System.Collections.Generic;
using System.Text;

namespace SimchasContributorsProject.Data
{
  public  class Contribution
    {
        public decimal Amount { get; set; }
        public bool Contribute { get; set; }
        public int ContributorID { get; set; }
        public int SimchaID { get; set; }
    }
}
