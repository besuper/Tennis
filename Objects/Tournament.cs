
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tennis;

public class Tournament {

    public Tournament() {
    }

    private string name;

    private List<Court> CourtList;
    private List<Referee> RefereeList;
    private List<Schedule> ScheduleList;

    public Tournament(string name)
    {
        this.name = name;
        this.CourtList = new List<Court>();
        this.RefereeList = new List<Referee>();
        this.ScheduleList = new List<Schedule>();
    }

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

    public void Play() {
        ScheduleType ScheduleType;
        List<Opponent> winner = new List<Opponent>();

        Array types = Enum.GetValues(typeof(ScheduleType));

        foreach (ScheduleType type in types)
        {
            ScheduleList.Add(new Schedule(this, type, GetPlayers()));

            //Ask DAO to get a list of opponents compatible with the type


            ScheduleList[ScheduleList.Count - 1].PlayNextRound();
            winner.Add(ScheduleList[ScheduleList.Count - 1].GetWinner());
        }

        Console.WriteLine("Voici les winners");

        foreach (var item in winner)
        {
            Console.WriteLine(item);
        }

    }

}