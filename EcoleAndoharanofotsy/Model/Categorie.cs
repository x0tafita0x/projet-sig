using EcoleAndoharanofotsy.Utils;
using Npgsql;

namespace EcoleAndoharanofotsy.Model
{
    public class Categorie
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Categorie(int id, string name) { Id = id; Name = name; }

        public static void DeleteCategory(int id_ecole, NpgsqlConnection conn)
        {
            string sql = "delete from categorie_ecole where id_ecole=@id";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id", id_ecole);
            cmd.ExecuteNonQuery();
        }

        public static void InsertCategory(int id_ecole, List<Categorie> categories, NpgsqlConnection conn)
        {
            string sql = "insert into categorie_ecole(id_ecole,id_categorie) values ";
            for (int i = 0; i < categories.Count; i++)
            {
                if (i == categories.Count - 1) sql += $"(@id_ecole,@id_categorie{i})";
                else sql += $"(@id_ecole,@id_categorie{i}),";
            }
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id_ecole", id_ecole);
            for (int i = 0; i < categories.Count; i++)
            {
                cmd.Parameters.AddWithValue($"id_categorie{i}", categories[i].Id);
            }
            cmd.ExecuteNonQuery();
        }

        public static List<Categorie> GetWithConn(int? id_ecole, NpgsqlConnection conn)
        {
            List<Categorie> list = [];
            string sql = "select c.id_categorie,nom_categorie " +
                "from categorie c " +
                "join categorie_ecole ce on ce.id_categorie=c.id_categorie " +
                "where 1=1 ";
            if (id_ecole != null) sql += " and id_ecole = @id_ecole";
            using var cmd = new NpgsqlCommand(sql, conn);
            if (id_ecole != null) cmd.Parameters.AddWithValue("id_ecole", id_ecole);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new(reader.GetInt32(0), reader.GetString(1)));
            }
            return list;
        }
        public static List<Categorie> ReadAll(NpgsqlConnection conn)
        {
            List<Categorie> list = [];
            string sql = "select * from categorie";
            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new(reader.GetInt32(0), reader.GetString(1)));
            }
            return list;
        }

        public static List<Categorie> Get(int? id_ecole)
        {
            using var conn = DatabaseConnect.Connect();
            conn.Open();
            return GetWithConn(id_ecole, conn);
        }
         public static List<Categorie> GetAll()
        {
            using var conn = DatabaseConnect.Connect();
            conn.Open();
            return ReadAll(conn);
        }

        public bool ExistingCategory(int id_ecole, NpgsqlConnection conn)
        {
            List<Categorie> categories = GetWithConn(id_ecole, conn);
            return categories.Contains(this);
        }
    }
}
