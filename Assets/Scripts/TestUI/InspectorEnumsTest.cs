using System;
using System.Collections.Generic;
using UnityEngine;


public enum Position
{
    First,
    Second,
    Third
}

[Flags]
public enum DaysOfWeek
{
    None = 0,
    Sunday = 1 << 0,
    Monday = 1 << 1,
    Tuesday = 1 << 2,
    Wednesday = 1 << 3,
    Thursday = 1 << 4,
    Friday = 1 << 5,
    Saturday = 1 << 6,

    Weekdays = Monday | Tuesday | Wednesday | Thursday | Friday,
    Weekend = Saturday | Sunday,
}
public class InspectorEnumsTest : MonoBehaviour
{

    [EnumButtons]
    public Position number;

    [EnumButtons]
    public DaysOfWeek days;

    [EnumButtons]
    public List<Position> numbersList;
}
