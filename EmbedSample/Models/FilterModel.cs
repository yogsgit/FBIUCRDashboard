using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FBIUCRDemo.Models
{
    public class FilterModel
    {
        public int? Year { get; set; }
        public int? MonthFrom { get; set; }
        public int? MonthTo { get; set; }
        public int? HourFrom { get; set; }
        public int? HourTo { get; set; }
        public string OffenseName { get; set; }
        public string OffenseLocation { get; set; }
        public List<string> OffenseWeapon { get; set; }
        public List<string> OffensePropertyLoss { get; set; }
        public List<string> OffensePropertyDesc { get; set; }
        public List<string> OffenseDrug { get; set; }
        public List<string> VictimType { get; set; }
        public List<string> VictimGender { get; set; }
        public List<string> VictimRace { get; set; }
        public List<string> VictimEthnicity { get; set; }
        public List<string> VictimInjury { get; set; }
        public string VictimAgeComp { get; set; }
        public int? VictimAge { get; set; }
        public List<string> OffenderGender { get; set; }
        public List<string> OffenderRace { get; set; }
        public List<string> OffenderEthnicity { get; set; }
        public List<string> OffenderRelation { get; set; }
        public string OffenderAgeComp { get; set; }
        public int? OffenderAge { get; set; }
        public List<string> ArresteeGender { get; set; }
        public List<string> ArresteeRace { get; set; }
        public List<string> ArresteeEthnicity { get; set; }
        public List<string> ArresteeOffense { get; set; }
        public List<string> ArresteeType { get; set; }
        public List<string> ArresteeWeapon { get; set; }
        public string ArresteeAgeComp { get; set; }
        public int? ArresteeAge { get; set; }
    }
}