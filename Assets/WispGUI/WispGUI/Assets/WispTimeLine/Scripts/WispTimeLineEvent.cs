using System;

public class WispTimeLineEvent
{
    public enum WispTimeLineEventMarkType { Moment, Period }

    //private uint id;
    private string name;
    private string description;
    private DateTime startingDate;
    private DateTime endingDate;
    private WispTimeLineEventMarkType type;
    private double startingDateInTicks;
    private double endingDateInTicks;

    //public uint Id { get => id; }
    public string Name { get => name; }
    public string Description { get => description; }
    public DateTime StartingDate { get => startingDate; }
    public DateTime EndingDate { get => endingDate; }
    public WispTimeLineEventMarkType Type { get => type; }
    public double StartingDateInTicks { get => startingDateInTicks; }
    public double EndingDateInTicks { get => endingDateInTicks; }

    public static long DateToTicks (DateTime ParamDate)
    {
        return ParamDate.Ticks;
    }

    public WispTimeLineEvent(/*uint ParamID,*/ string ParamName, string ParamDescription, DateTime ParamDate)
    {
        //id = ParamID;
        name = ParamName;
        description = ParamDescription;
        startingDate = ParamDate;

        startingDateInTicks = WispTimeLineEvent.DateToTicks(StartingDate);

        type = WispTimeLineEventMarkType.Moment;
    }

    public WispTimeLineEvent(/*uint ParamID,*/ string ParamName, string ParamDescription, DateTime ParamStartDate, DateTime ParamEndDate)
    {
        //id = ParamID;
        name = ParamName;
        description = ParamDescription;
        startingDate = ParamStartDate;
        endingDate = ParamEndDate;

        startingDateInTicks = WispTimeLineEvent.DateToTicks(StartingDate);
        endingDateInTicks = WispTimeLineEvent.DateToTicks(EndingDate);

        type = WispTimeLineEventMarkType.Period;
    }
}
