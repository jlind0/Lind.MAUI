namespace Lind.MAUI.Example
{
    public class WeatherForecast
    {
        public DateOnly Date { get; set; }

        public string DateString => Date.ToShortDateString();

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }
    }
}
