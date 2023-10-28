
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Schedule {

    private ScheduleType type;

    private int actualRound;
    private Tournament tournament;
    private List<Match> matches;
    public Schedule()
    {
    }

    public int NbWinningSets() {
        //TODO : Vérifier la véracité des nombres;
        int nb = 0;
        switch (type)
        {
            case ScheduleType.LadiesDouble:
                nb = 3;
                break;
            case ScheduleType.LadiesSingle:
                nb = 3;
                break;
            case ScheduleType.GentlemenSingle:
                nb = 5;
                break;
            case ScheduleType.GentlemenDouble:
                nb = 3;
                break;
            case ScheduleType.MixedDouble:
                nb = 3;
                break;
                
        }

        return nb;
    }

    public void PlayNextRound() {
        // TODO implement here
    }

    public void GetWinner() {
        // TODO implement here
    }

}