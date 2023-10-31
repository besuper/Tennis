
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tennis;

public class Tournament
{

    private string name;

    private List<Court> courtList;
    private List<Referee> refereeList;
    private List<Schedule> scheduleList;

    private DateTime currentDate = DateTime.Now;

    public Tournament(string name)
    {
        this.name = name;
        this.courtList = new List<Court>();
        this.refereeList = new List<Referee>();
        this.scheduleList = new List<Schedule>();

        this.refereeList.Add(new Referee("Carlos", "Bernardes", "Bresil"));
        this.refereeList.Add(new Referee("Jaume", "Campistol", "Espagne"));
        this.refereeList.Add(new Referee("Pierre", "Bacchi", "France"));
        this.refereeList.Add(new Referee("Ricardo", "Ortiz", "Argentine"));
        this.refereeList.Add(new Referee("John", "Blom", "Australie"));
    }

    public DateTime CurrentDate { get { return currentDate; } }

    private List<Player> GetPlayers()
    {
        //Put this function in DAO
        List<Player> list = new List<Player>();

        DatabaseManager dm = new DatabaseManager();

        SqlDataReader request = dm.Get("SELECT * FROM Joueur");
        while (request.Read())
        {
            list.Add(new Player(request.GetString(1), request.GetString(2), "", 0, request.GetString(3)));
        }

        request.Close();

        return list;
    }

    public void Play()
    {
        //ScheduleType ScheduleType;
        List<Opponent> winner = new List<Opponent>();

        Array types = Enum.GetValues(typeof(ScheduleType));

        foreach (ScheduleType type in types)
        {
            scheduleList.Add(new Schedule(this, type, GetPlayers()));

            //Ask DAO to get a list of opponents compatible with the type

            scheduleList[scheduleList.Count - 1].PlayNextRound();
            winner.Add(scheduleList[scheduleList.Count - 1].GetWinner());
        }

        Console.WriteLine("Voici les winners");

        foreach (var item in winner)
        {
            Console.WriteLine(item);
        }

    }

    public Referee GetAvailableReferee(Match match)
    {
        Referee referee = null;

        foreach (Referee item in refereeList)
        {
            if(item.Match != null)
            {
                if(item.Match.Date == match.Date)
                {
                    continue;
                }
            }

            if (item.Available(match))
            {
                referee = item;
                break;
            }
        }

        return referee;
    }

    private int currentMatchHours = 0;

    public void SkipNewDay()
    {
        DateTime tempDate = currentDate;
        tempDate = tempDate.AddDays(1);

        currentDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 10, 00, 00);
    }

    public void AddNewMatch()
    {
        if (currentDate.Hour >= 16)
        {
            SkipNewDay();
            return;
        }

        if(currentMatchHours >= 1)
        {
            currentDate = currentDate.AddHours(2);

            currentMatchHours = 0;
        }else
        {
            currentMatchHours++;
        }
    }

}