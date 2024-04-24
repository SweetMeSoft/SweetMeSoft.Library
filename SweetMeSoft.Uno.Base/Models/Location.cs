namespace SweetMeSoft.Uno.Base.Models;

public class Location
{
    public Location(double Latitude, double Longitude)
    {
        this.Latitude = Latitude;
        this.Longitude = Longitude;
    }

    public double Latitude { get; set; }
    public double Longitude { get; set; }
    //public double Altitude { get; set; }
    //public double Accuracy { get; set; }
    //public double AltitudeAccuracy { get; set; }
    //public double Heading { get; set; }
    //public double Speed { get; set; }
    //public DateTime Timestamp { get; set; }
}
