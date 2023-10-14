using DemoClasses.Enums;
using Microsoft.IdentityModel.Tokens;
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
        private List<Group> groups = new List<Group>();

        /*
         Simple homme => Qualifs, 1er tour, 2 tour, 3 tour, 4 tour, Quart de finale, Demi finale, Finale
         Simple femme => Qualifs, 1er tour, 2 tour, 3 tour, 4 tour, Quart de finale, Demi finale, Finale

         Simple homme / simple femme se déroulent les mm jours

         Double homme => 1er tour, 2 tour, 3 tour, Quart de finale, Demi finale, Finale
         Double femme => 1er tour, 2 tour, 3 tour, Quart de finale, Demi finale, Finale

         Double homme / Double femme / Double mixte se déroulent les mm jours

         Double mixte => 1er tour, 2 tour, Quart de finale, Demi finale, Finale
         
         */
        private List<Match> matches = new List<Match>();
    }
}
