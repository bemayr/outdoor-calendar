using EventlistToICal;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Npoi.Mapper;

var mapper = new Mapper("events.xlsx");

var result = mapper.Take<Event>("2022")
    .Select(row => row.Value)
    .Where(e => e.From != new DateTime())
    .Select(e => {
        e.Kind = e.Kind switch
        {
            "O" => "Orienteering",
            "AR" => "Adventure Race",
            "RG" => "Rogaining",
            "MM" => "Mountain Marathon",
            "S" => "Survival",
            "TR" => "Trail Run",
            _ => e.Kind
        };
        e.Type = e.Type switch
        {
            "S" => "Single",
            "T" => "Team",
            _ => e.Type
        };

        return e;
    })
    .Select(e => new CalendarEvent
    {
        Summary = e.Title,
        Description = $"{e.Kind} ({e.Type})\r\n{e.Link}",
        Location = e.Country,
        Start = new CalDateTime(e.From),
        End = new CalDateTime(e.To),
        IsAllDay = true,
    });

var calendar = new Calendar();
calendar.AddProperty("X-WR-CALNAME", "Outdoor Sport Events");
calendar.Events.AddRange(result);

var icsString = new CalendarSerializer().SerializeToString(calendar);

await File.WriteAllTextAsync("events.ics", icsString);

Console.WriteLine("Finished... 🏁");