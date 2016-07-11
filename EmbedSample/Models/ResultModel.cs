using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FBIUCRDemo.Models
{
    public class ResultModel
    {
        public int TotalRecords { get; set; }
        public int UniqueIncidents { get; set; }
        public string CsvLink { get; set; }
        public string SampleTable { get; set; }
        public string JsonLink { get; set; }
    }
}