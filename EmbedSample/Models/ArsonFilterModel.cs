using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FBIUCRDemo.Models
{
    public class ArsonFilterModel
    {
        public int? Year { get; set; }
        public int? MonthFrom { get; set; }
        public int? MonthTo { get; set; }
        public List<string> OffenseName { get; set; }
    }
}