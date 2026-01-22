using Npgsql;

namespace EcoleAndoharanofotsy.Utils
{
    public class DatabaseConnect
    {
        private static string connectionString = $"" +
            $"Host=localhost;" +
            $"Username=postgres;" +
            $"Password=anel;" +
            $"Database=ecole";

        public static NpgsqlConnection Connect()
        {
            return new(connectionString);
        }
    }
}
