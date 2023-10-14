// See https://aka.ms/new-console-template for more information
using DemoClasses;
using DemoClasses.Enums;
using DemoClasses.Objects;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;

namespace DemoClasses
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // TODO: Deplacer les listes joueurs, courts et arbitres dans edition et les fetch la ?
            List<Player> list = new List<Player>();

            DatabaseManager dm = new DatabaseManager();

            SqlDataReader request = dm.Get("SELECT * FROM Joueur");
            while (request.Read())
            {
                list.Add((new Player(request.GetInt32(0), request.GetString(1), request.GetString(2), request.GetString(3))));
            }

            request.Close();

            // Faire de courts des objects static?
            // pareil pour les arbitres
            List<Court> courts = new List<Court>();

            SqlDataReader request2 = dm.Get("SELECT * FROM Court");
            while (request2.Read())
            {
                courts.Add((new Court(request2.GetInt32(0), request2.GetString(1))));
            }

            request2.Close();

            DateTime startDate = DateTime.Now;
            CompetitionType ct = CompetitionType.MenSingle;


            List<Group> groups = makeGroups2(list.ToList(), ct);
            List<Group> womensGroups = makeGroups2(list.ToList(), CompetitionType.WomenDouble);
            /*foreach (Group temp in groups)
            {
                Console.WriteLine(temp.GetGroupName());
                foreach (Player player in temp.Players)
                {
                    Console.WriteLine(player.Gender);
                }
            }*/

            Console.WriteLine(groups.Count);

            List<Match> matches = createMatches(groups, ct);
            List<Match> matches2 = createMatches(womensGroups, CompetitionType.WomenDouble);

            Console.WriteLine(matches.Count);
            Console.WriteLine(matches2.Count);

            datesForMatches(matches, courts, startDate);
            datesForMatches(matches2, courts, startDate);
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

        static void datesForMatches(List<Match> matches, List<Court> courts, DateTime start)
        {
            Court temp = null;
            Match currentMatch;
            int matchPerDays = 16;
            int todayMatches = 0;

            for (int i = 0; i < matches.Count; i++)
            {
                temp = null;
                currentMatch = matches[i];

                foreach (Court c in courts)
                {
                    if (c.IsAvailable(start))
                    {
                        temp = c;
                        break;
                    }
                }

                // TODO: Vérifier aussi avec les arbitres

                if (temp == null || todayMatches >= matchPerDays)
                {
                    if (todayMatches >= matchPerDays)
                    {
                        start = start.AddDays(1);

                        todayMatches = 0;
                    }

                    // Faire un minimum et max niveau heure => si atteint passer au jour suivant
                    start = start.AddMinutes(60);
                    i--;
                }
                else
                {
                    currentMatch.Date = start;
                    currentMatch.Court = temp;
                    temp.AddMatch(currentMatch);
                    todayMatches++;
                }
            }

            foreach (Match _match in matches)
            {
                Console.WriteLine("Match start at " + _match.Date + " on court " + _match.Court);
            }
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

        static List<Group> makeGroups2(List<Player> players, CompetitionType competitionType)
        {
            List<Group> groups = new List<Group>();
            int countGroups = 64;
            int countPlayerPerGroup = 2;

            switch (competitionType)
            {
                case CompetitionType.MenSingle | CompetitionType.WomenSingle:
                    countGroups = 128;
                    countPlayerPerGroup = 1;
                    break;

                case CompetitionType.MenDouble | CompetitionType.WomenDouble:
                    countGroups = 64;
                    countPlayerPerGroup = 2;
                    break;
                case CompetitionType.MixedDouble:
                    countGroups = 32;
                    break;
            }

            switch (competitionType)
            {
                case CompetitionType.MenSingle | CompetitionType.MenDouble:
                    players = players.GetRange(0, 128);
                    break;
                case CompetitionType.WomenSingle | CompetitionType.WomenDouble:
                    players = players.GetRange(127, 128);
                    break;
            }

            if (players.Count < (countGroups * countPlayerPerGroup))
            {
                throw new Exception("Not enough players");
            }

            Group temp;
            Random rand = new Random();
            Player currentPlayer;
            int currentIndex;

            if (competitionType == CompetitionType.MixedDouble)
            {
                List<Player> mens = players.GetRange(0, 128);
                List<Player> womens = players.GetRange(127, 128);

                Console.WriteLine(countGroups);

                for (int i = 0; i < countGroups; i++)
                {
                    temp = new Group();

                    //Select a man
                    currentIndex = rand.Next(mens.Count);
                    currentPlayer = mens[currentIndex];

                    temp.AddPlayer(currentPlayer);
                    mens.Remove(currentPlayer);

                    //Select a woman
                    currentIndex = rand.Next(womens.Count);
                    currentPlayer = womens[currentIndex];

                    temp.AddPlayer(currentPlayer);
                    womens.Remove(currentPlayer);

                    groups.Add(temp);
                }

                return groups;
            }

            for (int i = 0; i < countGroups; i++)
            {
                temp = new Group();

                for (int j = 0; j < countPlayerPerGroup; j++)
                {
                    currentIndex = rand.Next(players.Count);
                    currentPlayer = players[currentIndex];

                    temp.AddPlayer(currentPlayer);
                    players.Remove(currentPlayer);
                }

                groups.Add(temp);
            }

            return groups;
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