using EventlistToICal;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Npoi.Mapper;
using System.Security.Cryptography;
using System.Text;

var year = 2023;
var mapper = new Mapper($"input/events.{year}.xlsx");
var sha = SHA256.Create();

var result = mapper.Take<Event>(sheetName: year.ToString())
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

        // set end date to next day
        e.To = e.To.AddDays(1);

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
        Uid = ComputeSHA256Hash($"{year}/{e.Title}/{e.Link}"),
        Categories = new [] { e.Kind, e.Type }
    });

var calendar = new Calendar();
calendar.AddProperty("X-WR-CALNAME", "Outdoor Sport Events");
calendar.Events.AddRange(result);

var icsString = new CalendarSerializer().SerializeToString(calendar);

Directory.CreateDirectory("output");
await File.WriteAllTextAsync("output/events.ics", icsString);

Console.WriteLine("Finished... 🏁");


static string ComputeSHA256Hash(string text)
{
    using var sha256 = SHA256.Create();
    return BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(text)))
                       .Replace("-", "");
}