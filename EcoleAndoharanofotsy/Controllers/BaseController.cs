using EcoleAndoharanofotsy.Model;
using Microsoft.AspNetCore.Mvc;

namespace EcoleAndoharanofotsy.Controllers
{
    [ApiController]
    [Route("ecole_api")]
    public class BaseController : Controller
    {
        [HttpPost("create")]
        public IActionResult Create([FromBody] EcoleModel model)
        {
            string nom = model.Nom ?? null;
            string typeEtablissement = model.TypeEtablissement ?? null;
            double longitude = model.Longitude.Value;
            double latitude = model.Latitude.Value;
            List<Categorie> categories = model.Categorie ?? null;
            try
            {
                new Ecole(0, nom, typeEtablissement, new(longitude, latitude, 0), categories).Create();
                return Ok("Insert success");
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("lastId")]
        public IActionResult LastId()
        {
            try
            {
                int res = Ecole.ReadLastId();
                return new JsonResult(res);
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("read")]
        public IActionResult Read([FromBody] EcoleModel model)
        {
            string nom = model.Nom ?? null;
            string typeEtablissement = model.TypeEtablissement ?? null;
            List<Categorie> categories = model.Categorie ?? null;
            try
            {
                Coordonnees? rayon_recherche = null;
                if (model.Longitude != null && model.Latitude != null && model.Range != null)
                {
                    double range = model.Range.Value;
                    double longitude = model.Longitude.Value;
                    double latitude = model.Latitude.Value;
                    rayon_recherche = new((double)latitude, (double)longitude, (int)range);
                }
                List<Ecole> liste_ecole = Ecole.Read(nom, rayon_recherche, categories);
                return Ok(new JsonResult(liste_ecole));
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("read_by_id")]
        public IActionResult GetById(int id)
        {
            try
            {
                return Ok(Ecole.GetById(id));
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("update")]
        public IActionResult Update([FromBody] EcoleModel model)
        {
            int id = model.Id.Value;
            string nom = model.Nom ?? null;
            string typeEtablissement = model.TypeEtablissement ?? null;
            double longitude = model.Longitude.Value;
            double latitude = model.Latitude.Value;
            List<Categorie> categories = model.Categorie ?? null;
            try
            {
                Coordonnees? emplacement = null;
                if (latitude != null && longitude != null)
                {
                    emplacement = new((double)latitude, (double)longitude, 0);
                }
                new Ecole(id).Update(nom, typeEtablissement, categories, emplacement);
                return Ok("Update success");
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("delete")]
        public IActionResult Delete([FromBody] int id_ecole)
        {
            try
            {
                new Ecole(id_ecole).Delete();
                return Ok("Delete success:"+id_ecole);
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("readAllCat")]
        public IActionResult ReadAllCategorie()
        {
            try
            {
                List<Categorie> liste_categorie = Categorie.GetAll();
                return Ok(new JsonResult(liste_categorie));
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("readAllEco")]
        public IActionResult ReadAllEcole()
        {
            try
            {
                List<Ecole> liste_Ecole = Ecole.ReadAll();
                return Ok(new JsonResult(liste_Ecole));
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
    public class EcoleModel
    {
        public int? Id {get;set;}
        public string? Nom { get; set; }
        public string? TypeEtablissement { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public List<Categorie>? Categorie { get; set; }
        public double? Range {get; set;}
    }
    
}
