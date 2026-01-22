using EcoleAndoharanofotsy.Utils;
using Npgsql;
using System.Linq;

namespace EcoleAndoharanofotsy.Model
{
    public class Ecole
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string TypeEtablissement { get; set; }
        public List<Categorie> CategorieEcole {  get; set; }
        public Coordonnees Emplacement { get; set; }

        public Ecole(int id, string nom, string typeEtablissement, Coordonnees emplacement, 
            List<Categorie> categorie)
        {
            Id = id;
            Nom = nom;
            TypeEtablissement = typeEtablissement;
            Emplacement = emplacement;
            CategorieEcole = categorie;
        }

        public Ecole(int id)
        {
            Id = id;
            Nom = String.Empty;
            TypeEtablissement = String.Empty;
            Emplacement = new(0, 0, 0);
            CategorieEcole = [];
        }

        public void Delete()
        {
            using var conn = DatabaseConnect.Connect();
            conn.Open();
            string sql = "delete from ecole where id_ecole=@id_ecole";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id_ecole", Id);
            cmd.ExecuteNonQuery();
        }

        public void Update(string? nom, string? type_etablissement, 
            List<Categorie>? categorie_ecole, Coordonnees? emplacement)
        {   
            if (nom == null && type_etablissement == null &&
                categorie_ecole == null && emplacement == null) return;
            using var conn = DatabaseConnect.Connect();
            conn.Open();
            using var transaction = conn.BeginTransaction();
            try
            {
                string sql = "update ecole set ";
                List<string> updates = [];

                if (nom != null)
                    updates.Add("nom_ecole=@name");
                if (type_etablissement != null)
                    updates.Add("type_etablissement=@type");
                if (emplacement != null)
                    updates.Add("emplacement = ST_SetSRID(ST_MakePoint(@longitude, @latitude), 4326)");

                sql += string.Join(",", updates);

                sql += " where id_ecole=@id_ecole";

                using var cmd = new NpgsqlCommand(sql, conn);
                if (nom != null) cmd.Parameters.AddWithValue("name", nom);
                if (type_etablissement != null) cmd.Parameters.AddWithValue("type", type_etablissement);
                if (emplacement != null)
                {
                    cmd.Parameters.AddWithValue("longitude", emplacement.Longitude);
                    cmd.Parameters.AddWithValue("latitude", emplacement.Latitude);
                }
                cmd.Parameters.AddWithValue("id_ecole", Id);
                cmd.ExecuteNonQuery();

                if (categorie_ecole != null)
                {
                    Categorie.DeleteCategory(Id, conn);
                    Categorie.InsertCategory(Id, categorie_ecole, conn);
                }

                transaction.Commit();
            } catch(Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public void Create()
        {
            using var conn = DatabaseConnect.Connect();
            conn.Open();
            using var transaction = conn.BeginTransaction();
            try
            {
                string sql = "insert into ecole(nom_ecole,type_etablissement,emplacement) values " +
                    "(@nom,@type,ST_SetSRID(ST_MakePoint(@longitude, @latitude), 4326)) returning id_ecole";

                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("nom", Nom);
                cmd.Parameters.AddWithValue("type", TypeEtablissement);
                cmd.Parameters.AddWithValue("longitude", Emplacement.Longitude);
                cmd.Parameters.AddWithValue("latitude", Emplacement.Latitude);

                int id_ecole = (int)cmd.ExecuteScalar();

                Categorie.InsertCategory(id_ecole, CategorieEcole, conn);

                transaction.Commit();
            } catch(Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public static List<Ecole> ReadAll()
        {
            List<Ecole> liste_ecole = [];
            using var conn = DatabaseConnect.Connect();
            conn.Open();
            string sql = "select id_ecole,nom_ecole,type_etablissement," +
                "ST_X(emplacement) AS longitude,ST_Y(emplacement) AS latitude" +
                " from v_detailEcole";
            using var cmd = new NpgsqlCommand(sql, conn);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int id_ecole = reader.GetInt32(0);
                string nom_ecole = reader.GetString(1);
                string type = reader.GetString(2);
                double longitutde = reader.GetDouble(3);
                double latitude = reader.GetDouble(4);
                List<Categorie> categories = Categorie.Get(id_ecole);
                liste_ecole.Add(new(id_ecole, nom_ecole, type, new(latitude, longitutde, 0), categories));
            }
            return liste_ecole;
        }

        public static int ReadLastId()
        {
            int res = 0;
            using var conn = DatabaseConnect.Connect();
            conn.Open();
            string sql = "select MAX(id_ecole) from ecole";
            using var cmd = new NpgsqlCommand(sql, conn);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                res = reader.GetInt32(0);
            }
            return res;
        }


        public static Ecole GetById(int id)
        {
            using var conn = DatabaseConnect.Connect();
            conn.Open();
            string sql = "select id_ecole,nom_ecole,type_etablissement," +
                "ST_X(emplacement) AS longitude,ST_Y(emplacement) AS latitude" +
                " from v_detailEcole where id_ecole=@id_ecole ";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id_ecole", id);

            Ecole result = new Ecole(id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                int id_ecole = reader.GetInt32(0);
                string nom_ecole = reader.GetString(1);
                string type = reader.GetString(2);
                double longitutde = reader.GetDouble(3);
                double latitude = reader.GetDouble(4);
                List<Categorie> categories = Categorie.Get(id_ecole);
                result = new(id_ecole, nom_ecole, type, new(latitude, longitutde, 0), categories);
            }
            return result;
        }

        public static List<Ecole> Read(string? nom, Coordonnees? rayon_recherche, 
            List<Categorie>? id_categories)
        {
            List<Ecole> liste_ecole = [];
            using var conn = DatabaseConnect.Connect();
            conn.Open();
            string sql = "select id_ecole,nom_ecole,type_etablissement," +
                "ST_X(emplacement) AS longitude,ST_Y(emplacement) AS latitude" +
                " from v_detailEcole where 1=1 ";
            if (nom != null)
            {
                sql += " and nom_ecole ilike @nom ";
            }
            if (rayon_recherche != null)
            {
                sql += " and ST_DWithin(emplacement::geography," +
                    "ST_SetSRID(ST_MakePoint(@latitude, @longitude), 4326)::geography, @range)";
            }
            if (id_categories != null && id_categories.Count > 0)
            {
                sql += " and id_categorie in ( ";
                for (int i = 0; i < id_categories.Count; i++)
                {

                    if (i == id_categories.Count - 1) sql += $"@categorie{i}";
                    else sql += $"@categorie{i},";
                }
                sql += $")" +
                    $" group by id_ecole,nom_ecole,type_etablissement,emplacement " +
                    $" having count(distinct id_categorie) = {id_categories.Count} ";
            }

            using var cmd = new NpgsqlCommand(sql, conn);
            if (nom != null)
            {
                cmd.Parameters.AddWithValue("nom", $"%{nom}%");
            }
            if (rayon_recherche != null)
            {
                cmd.Parameters.AddWithValue("latitude", rayon_recherche.Latitude);
                cmd.Parameters.AddWithValue("longitude", rayon_recherche.Longitude);
                cmd.Parameters.AddWithValue("range", rayon_recherche.Range);
            }
            if (id_categories != null)
            {
                for (int i = 0; i < id_categories.Count; i++)
                {
                    cmd.Parameters.AddWithValue($"categorie{i}", id_categories[i].Id);
                }
            }

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int id_ecole = reader.GetInt32(0);
                string nom_ecole = reader.GetString(1);
                string type = reader.GetString(2);
                double longitutde = reader.GetDouble(3);
                double latitude = reader.GetDouble(4);
                List<Categorie> categories = Categorie.Get(id_ecole);
                liste_ecole.Add(new(id_ecole, nom_ecole, type, new(latitude, longitutde, 0), categories));
            }
            return liste_ecole;
        }

        
    }
}
