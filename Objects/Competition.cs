using DemoClasses.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoClasses.Objects
{
    internal class Competition
    {
        private int id;
        private CompetitionType competitionType;
        private Tournament tournament;
        private List<Group> groups;
        private List<Match> matches;
    }
}
