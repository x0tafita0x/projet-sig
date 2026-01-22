namespace EcoleAndoharanofotsy
{
    public class Coordonnees
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Range { get; set; }

        public Coordonnees(double latitude, double longitude, int range)
        {
            Latitude = latitude;
            Longitude = longitude;
            Range = range;
        }
    }
}
