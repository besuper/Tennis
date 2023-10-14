// See https://aka.ms/new-console-template for more information
using DemoClasses;
using DemoClasses.Enum;
using DemoClasses.Objects;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System;

namespace DemoClasses
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Player> list = new List<Player>();

            DatabaseManager dm = new DatabaseManager();

            SqlDataReader request = dm.Get("SELECT * FROM Joueur");
            while (request.Read())
            {
                list.Add((new Player(request.GetInt32(0), request.GetString(1), request.GetString(2), request.GetString(3))));
            }

            CompetitionType ct = CompetitionType.MixedDouble;


            List<Group> groups = makeGroups(list, ct);
            foreach (Group temp in groups)
            {
                Console.WriteLine(temp.GetGroupName());
                /*foreach (Player player in temp.Players)
                {
                    Console.WriteLine(player.Gender);
                }*/
            }

            List<Match> matches = createMatches(groups, ct);

            Console.WriteLine(matches.Count);
            /*foreach (Match match in matches)
            {
                Console.WriteLine("Match :D");
                Console.WriteLine(match.Groups[0] + " et " + match.Groups[1]);
            }*/

            //Jouer les matchs (groupe aléatoire),
            //Récupérer les gagants,
            //Refaire jouer les matchs
            //Jusqu'au moment ou il n'y a qu'un winner ou un match??


        }

        //On devrait mettre en paramètre "Competition", qui possède les groupes ainsi que le type de compétition
        static List<Match> createMatches(List<Group> groups, CompetitionType competitionType)
        {
            List<Match> matches = new List<Match>();
            Random random = new Random();

            int indexSelected;

            List<Group> tempGroupBattle;
            while (!groups.IsNullOrEmpty())
            {
                tempGroupBattle = new List<Group>();
                for (int i = 0; i < 2; i++)
                {
                    indexSelected = random.Next(groups.Count);
                    tempGroupBattle.Add(groups[indexSelected]);
                    groups.Remove(groups[indexSelected]);

                }
                matches.Add(new Match(5, tempGroupBattle[0], tempGroupBattle[1]));
            }


            return matches;
        }

        //On devrait mettre en paramètre "Competition", qui possède les groupes ainsi que le type de compétition
        static List<Group> makeGroups(List<Player> list, CompetitionType competitionType)
        {
            List<Group> groups = new List<Group>();
            Random random = new Random();
            int howManyPlayerPerGroup = 0;
            int indexSelected;

            switch (competitionType)
            {
                case CompetitionType.MenSingle:

                    howManyPlayerPerGroup = 1;
                    list = list.GetRange(0, 128);

                    break;
                case CompetitionType.WomenSingle:

                    howManyPlayerPerGroup = 1;
                    list = list.GetRange(127, 128);

                    break;
                case CompetitionType.MenDouble:

                    list = list.GetRange(0, 128);
                    howManyPlayerPerGroup = 2;

                    break;
                case CompetitionType.WomenDouble:

                    list = list.GetRange(127, 128);
                    howManyPlayerPerGroup = 2;

                    break;
                case CompetitionType.MixedDouble:
                    howManyPlayerPerGroup = 2;

                    break;
            }


            Group temp;
            if (competitionType == CompetitionType.MixedDouble)
            {
                List<Player> menList = list.GetRange(0, 128);
                List<Player> womenList = list.GetRange(127, 128);

                while (groups.Count < 32)
                {
                    temp = new Group();

                    //Select a man
                    indexSelected = random.Next(menList.Count);
                    temp.AddPlayer(menList[indexSelected]);
                    menList.Remove(list[indexSelected]);


                    //Select a woman
                    indexSelected = random.Next(womenList.Count);
                    temp.AddPlayer(womenList[indexSelected]);
                    womenList.Remove(list[indexSelected]);

                    groups.Add(temp);

                }
            }
            else
            {
                while (!list.IsNullOrEmpty() || ((competitionType == CompetitionType.MenDouble || competitionType == CompetitionType.WomenDouble) && groups.Count < 64))
                {
                    temp = new Group();
                    for (int i = 0; i < howManyPlayerPerGroup; i++)
                    {
                        indexSelected = random.Next(list.Count);
                        temp.AddPlayer(list[indexSelected]);
                        list.Remove(list[indexSelected]);
                    }
                    groups.Add(temp);

                }
            }

            return groups;
        }
    }
}