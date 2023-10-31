
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public class Schedule {

    private ScheduleType type;

    private int actualRound = 0;
    private Tournament tournament;
    private List<Match> matches;

    private Opponent scheduleWinner;

    List<Player> players;  

    public Schedule(Tournament tournament, ScheduleType type, List<Player> players)
    {
        this.tournament = tournament;
        this.type = type;
        this.players = players;

    }

    public int NbWinningSets() {
        int nb = 0;
        switch (type)
        {
            case ScheduleType.LadiesDouble:
            case ScheduleType.LadiesSingle:
            case ScheduleType.MixedDouble:
                nb = 3;
                break;
            case ScheduleType.GentlemenSingle:
            case ScheduleType.GentlemenDouble:
                nb = 5;
                break;
        }

        return nb;
    }

    public void PlayNextRound()
    {
        List<Opponent> winners = MakeGroups(players, type);

        do
        {
            actualRound++;

            Console.WriteLine("/!\\ /!\\ /!\\ /!\\ /!\\ /!\\ /!\\ /!\\ /!\\ /!\\ Playing round " + actualRound + " /!\\ /!\\ /!\\ /!\\ /!\\ /!\\ /!\\ /!\\ /!\\ /!\\");

            matches = CreateMatches(winners);
            winners = new List<Opponent>();

            foreach (Match match in matches)
            {
                //La on recherche un refereeS
                match.Referee = tournament.GetAvailableReferee(match);
                match.Play();
                Console.WriteLine("Le winner est " + match.GetWinner());
                winners.Add(match.GetWinner());

            }

        } while(winners.Count != 1);

        scheduleWinner = winners[0];
    }

    public List<Match> CreateMatches(List<Opponent> opponents)
    {
        List<Match> matches = new List<Match>();
        Random random = new Random();

        int indexSelected;

        List<Opponent> tempGroupBattle;
        Match tempMatch = null;

        while (opponents.Count != 0)
        {
            tempGroupBattle = new List<Opponent>();
            for (int i = 0; i < 2; i++)
            {
                indexSelected = random.Next(opponents.Count);
                tempGroupBattle.Add(opponents[indexSelected]);
                opponents.Remove(opponents[indexSelected]);

            }

            tempMatch = new Match(this, tempGroupBattle);
            // TODO: If referee is null ?
            //tempMatch.Referee = tournament.GetAvailableReferee(tempMatch);
            tempMatch.Date = tournament.CurrentDate;

            tournament.AddNewMatch();

            matches.Add(tempMatch);
        }


        return matches;
    }

    static List<Opponent> MakeGroups(List<Player> players, ScheduleType competitionType)
    {
        List<Opponent> opponents = new List<Opponent>();
        int countGroups = 64;
        int countPlayerPerGroup = 2;

        switch (competitionType)
        {
            case ScheduleType.GentlemenSingle:
            case ScheduleType.LadiesSingle:
                countGroups = 128;
                countPlayerPerGroup = 1;
                break;

            case ScheduleType.GentlemenDouble:
            case ScheduleType.LadiesDouble:
                countGroups = 64;
                countPlayerPerGroup = 2;
                break;
            case ScheduleType.MixedDouble:
                countGroups = 32;
                break;
        }

        switch (competitionType)
        {
            case ScheduleType.GentlemenSingle:
            case ScheduleType.GentlemenDouble:
                players = players.GetRange(0, 128);
                break;
            case ScheduleType.LadiesSingle:
            case ScheduleType.LadiesDouble:
                players = players.GetRange(127, 128);
                break;
        }

        if (players.Count < (countGroups * countPlayerPerGroup))
        {
            throw new Exception("Not enough players");
        }

        Opponent temp;
        Random rand = new Random();
        Player currentPlayer;
        int currentIndex;

        if (competitionType == ScheduleType.MixedDouble)
        {
            List<Player> mens = players.GetRange(0, 128);
            List<Player> womens = players.GetRange(127, 128);

            Console.WriteLine(countGroups);

            for (int i = 0; i < countGroups; i++)
            {
                temp = new Opponent();

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

                opponents.Add(temp);
            }

            return opponents;
        }

        for (int i = 0; i < countGroups; i++)
        {
            temp = new Opponent();

            for (int j = 0; j < countPlayerPerGroup; j++)
            {
                currentIndex = rand.Next(players.Count);
                currentPlayer = players[currentIndex];

                temp.AddPlayer(currentPlayer);
                players.Remove(currentPlayer);
            }

            opponents.Add(temp);
        }

        return opponents;
    }


    public Opponent GetWinner() {
        return scheduleWinner;
    }

}