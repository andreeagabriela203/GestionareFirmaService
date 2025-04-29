using FirmaService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace FirmaService.Controllers
{
    public class CautDispController : Controller
    {
        private readonly string connectionString = "Server=LAPTOP-JIGFAA7Q;Database=FirmaService;Integrated Security=True;Encrypt=False;";
        // GET: Index
        [HttpGet]
        public IActionResult Index()
        {
           
            ViewBag.IsSearchPerformed = false; // Nu s-a efectuat nicio căutare
            return View(new List<CautDisp>());
        }

        // POST: Index
        [HttpPost]
        public async Task<IActionResult> Index(string Nume, string Prenume)
        {
            ViewBag.IsSearchPerformed = true;
            // Verificam daca au fost introduse numele si prenumele
            if (string.IsNullOrEmpty(Nume) || string.IsNullOrEmpty(Prenume))
            {
                ModelState.AddModelError("", "Numele și prenumele sunt obligatorii.");
                return View(new List<CautDisp>()); // Returnam pagina cu eroare
            }

            // Lista unde vom stoca dispozitivele găsite
            List<CautDisp> dispozitive = new List<CautDisp>();

            string sql = @"
        SELECT d.TipDispozitiv, d.Model, d.Marca, d.Status
        FROM Dispozitive d
        INNER JOIN Clienti c ON d.ClientID = c.ClientiId
        WHERE c.Nume = @Nume AND c.Prenume = @Prenume";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    // Adăugăm parametrii
                    command.Parameters.AddWithValue("@Nume", Nume);
                    command.Parameters.AddWithValue("@Prenume", Prenume);

                    // Executăm query-ul
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            dispozitive.Add(new CautDisp
                            {
                                TipDispozitiv = reader["TipDispozitiv"].ToString(),
                                Model = reader["Model"].ToString(),
                                Marca = reader["Marca"].ToString(),
                                Status = reader["Status"].ToString()
                            });
                        }
                    }
                }
            }

            return View(dispozitive);
        }

    }
}
