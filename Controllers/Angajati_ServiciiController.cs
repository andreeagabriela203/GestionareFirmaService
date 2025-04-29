using Microsoft.AspNetCore.Mvc;
using FirmaService.Models;
using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace FirmaService.Controllers
{
    public class Angajati_ServiciiController : Controller
    {
        private readonly EFCoreDbContext _context;
        private string connectionString = "Server=LAPTOP-JIGFAA7Q;Database=FirmaService;Integrated Security=True;;Encrypt=False;";

        public Angajati_ServiciiController(EFCoreDbContext context)
        {
            _context = context;
        }

        // GET: Afisare Angajati_Servicii in pagina
        public async Task<IActionResult> IndexAsync()
        {
            List<Angajati_ServiciiViewModel> rezultat = new List<Angajati_ServiciiViewModel>();

            string sql = @"
    SELECT
        asv.Angajati_ServiciiId,
        s.Denumire AS Serviciu,
        d.TipDispozitiv,
        d.Marca,
        d.Model,
        p.TipPiesa,
        d.Status,   
        a.Nume AS AngajatNume,
        a.Prenume AS AngajatPrenume,
        asv.Cost,
        asv.Data_Incepere,
        asv.DataFinalizare
    FROM
        Angajati_Servicii asv
    JOIN
        Servicii s ON asv.ServiciiId = s.ServiciiId
    JOIN
        Dispozitive d ON asv.DispozitiveId = d.DispozitiveID
    LEFT JOIN
        Piese p ON asv.PieseID = p.PieseId
    JOIN
        Angajati a ON asv.AngajatiId = a.AngajatiId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var serviciu = new Angajati_ServiciiViewModel
                            {
                                Angajati_ServiciuId = reader.GetInt32(reader.GetOrdinal("Angajati_ServiciiId")),
                                Serviciu = reader["Serviciu"].ToString(),
                                TipDispozitiv = reader["TipDispozitiv"].ToString(),
                                Marca = reader["Marca"].ToString(),
                                Model = reader["Model"].ToString(),
                                TipPiesa = reader["TipPiesa"]?.ToString(),
                                AngajatNume = reader["AngajatNume"].ToString(),
                                AngajatPrenume = reader["AngajatPrenume"].ToString(),
                                Cost = reader.GetInt32(reader.GetOrdinal("Cost")),
                                Status = reader["Status"].ToString(),
                                DataIncepere = reader.IsDBNull(reader.GetOrdinal("Data_Incepere")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("Data_Incepere")),
                                DataFinalizare = reader.IsDBNull(reader.GetOrdinal("DataFinalizare")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("DataFinalizare"))
                            };

                            rezultat.Add(serviciu);
                        }
                    }
                }
            }

            return View(rezultat);
        }

        public Angajati_Servicii GetAngajatServiciuById(int id)
        {
            Angajati_Servicii angajatServiciu = null;
            string sql = "SELECT * FROM Angajati_Servicii WHERE Angajati_ServiciiId = @Angajati_ServiciiId";

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, sqlConnection))
                {
                    command.Parameters.AddWithValue("@Angajati_ServiciiId", id);
                    sqlConnection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            angajatServiciu = new Angajati_Servicii
                            {
                                Angajati_ServiciiId = reader.GetInt32(reader.GetOrdinal("Angajati_ServiciiId")),
                                ServiciiId = reader.GetInt32(reader.GetOrdinal("ServiciiId")),
                                AngajatiId = reader.GetInt32(reader.GetOrdinal("AngajatiId")),
                                DispozitiveId = reader.GetInt32(reader.GetOrdinal("DispozitiveId")),
                                PieseID = reader.IsDBNull(reader.GetOrdinal("PieseID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("PieseID")),
                                Cost = reader.IsDBNull(reader.GetOrdinal("Cost")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Cost")),
                                Data_Incepere = reader.IsDBNull(reader.GetOrdinal("Data_Incepere")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("Data_Incepere")),
                                DataFinalizare = reader.IsDBNull(reader.GetOrdinal("DataFinalizare")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("DataFinalizare"))

                            };
                        }
                    }
                }
            }
            sql = @"UPDATE a2
            SET a2.Cost = COALESCE(s.CostServiciu, 0) + COALESCE(p.Cost, 0)
            FROM Angajati_servicii a2 
            INNER JOIN Servicii s ON a2.ServiciiId = s.ServiciiId 
            INNER JOIN Piese p ON a2.PieseId = p.PieseId";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();
                command.ExecuteNonQuery();
            }


            return angajatServiciu;
        }




        // GET: Angajati_Servicii/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Angajati_Servicii/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Angajati_Servicii angajatServiciu)
        {
            if (ModelState.IsValid)
            {
                // Comanda SQL pentru inserarea unui serviciu asociat unui angajat
                string sql = @"
            INSERT INTO Angajati_Servicii (AngajatiId, ServiciiId, DispozitiveId, PieseID, Cost, Data_Incepere, DataFinalizare) 
            VALUES (@AngajatiId, @ServiciiId, @DispozitiveId, @PieseID, @Cost, @Data_Incepere, @DataFinalizare)";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Adăugăm parametrii pentru interogare
                        command.Parameters.AddWithValue("@AngajatiId", angajatServiciu.AngajatiId);
                        command.Parameters.AddWithValue("@ServiciiId", angajatServiciu.ServiciiId);
                        command.Parameters.AddWithValue("@DispozitiveId", angajatServiciu.DispozitiveId);
                        command.Parameters.AddWithValue("@PieseID", angajatServiciu.PieseID.HasValue ? (object)angajatServiciu.PieseID.Value : DBNull.Value);  // Dacă PieseID este null, adăugăm DBNull.Value
                        command.Parameters.AddWithValue("@Cost", angajatServiciu.Cost.HasValue ? (object)angajatServiciu.Cost.Value : DBNull.Value); // Dacă Cost este null, adăugăm DBNull.Value

                        // Verificăm dacă DataIncepere și DataFinalizare sunt valori valide și le adăugăm în comanda SQL
                        command.Parameters.AddWithValue("@Data_Incepere", angajatServiciu.Data_Incepere.HasValue ? (object)angajatServiciu.Data_Incepere.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@DataFinalizare", angajatServiciu.DataFinalizare.HasValue ? (object)angajatServiciu.DataFinalizare.Value : DBNull.Value);

                        // Deschidem conexiunea
                        connection.Open();

                        // Executăm comanda
                        command.ExecuteNonQuery();
                    }
                }

                // După ce inserăm datele, redirecționăm către o altă acțiune, de obicei "Index"
                return RedirectToAction("Index");
            }


            return View(angajatServiciu);
        }


        // GET: Angajati_Servicii/Edit
        public async Task<IActionResult> Edit(int id)
        {
            var angajati_servicii = GetAngajatServiciuById(id);
            if (angajati_servicii == null)
            {
                return NotFound();
            }
            return View(angajati_servicii);
        }
        // POST: Angajati_Servicii/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Angajati_Servicii angajatServiciu)
        {
            // Verificăm dacă id-ul din URL se potrivește cu cel al entității editate
            if (id != angajatServiciu.Angajati_ServiciiId)
            {
                return NotFound();  // Dacă ID-ul nu se potrivește, returnăm eroare 404
            }

            // Dacă modelul este valid, continuăm cu actualizarea
            if (ModelState.IsValid)
            {
                string sql = @"
            UPDATE Angajati_Servicii
            SET  
                Cost = @Cost, 
                Data_Incepere = @Data_Incepere, 
                DataFinalizare = @DataFinalizare
            WHERE Angajati_ServiciiId = @Angajati_ServiciiId";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Adăugăm parametrii pentru fiecare câmp
                        command.Parameters.AddWithValue("@Angajati_ServiciiId", angajatServiciu.Angajati_ServiciiId);

                        command.Parameters.AddWithValue("@Cost", (object)angajatServiciu.Cost ?? DBNull.Value); // Verificăm dacă Cost este null

                        command.Parameters.AddWithValue("@Data_Incepere", angajatServiciu.Data_Incepere.HasValue ? (object)angajatServiciu.Data_Incepere.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@DataFinalizare", angajatServiciu.DataFinalizare.HasValue ? (object)angajatServiciu.DataFinalizare.Value : DBNull.Value);

                        // Deschidem conexiunea și executăm comanda SQL
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
               

                // După actualizare, redirecționăm utilizatorul la pagina Index
                return RedirectToAction("Index");
          
        }
          

            // Dacă ModelState nu este valid, returnăm view-ul cu erorile
            return View(angajatServiciu);
}


    


        // GET: Angajati_Servicii/Delete/5
        public IActionResult Delete(int id)
        {
            // Creăm un obiect pentru a păstra datele angajatului
            Angajati_Servicii angajatServiciu = null;

            // Căutăm entitatea Angajati_Servicii pe baza ID-ului
            string sqlSelect = "SELECT * FROM Angajati_Servicii WHERE Angajati_ServiciiId = @Angajati_ServiciiId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                {
                    // Adăugăm parametrul ID
                    command.Parameters.AddWithValue("@Angajati_ServiciiId", id);

                    // Deschidem conexiunea
                    connection.Open();

                    // Executăm comanda și citim datele
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            angajatServiciu = new Angajati_Servicii
                            {
                                Angajati_ServiciiId = reader.GetInt32(reader.GetOrdinal("Angajati_ServiciiId")),
                                AngajatiId = reader.GetInt32(reader.GetOrdinal("AngajatiId")),
                                ServiciiId = reader.GetInt32(reader.GetOrdinal("ServiciiId")),
                                DispozitiveId = reader.GetInt32(reader.GetOrdinal("DispozitiveId")),
                                PieseID = reader.IsDBNull(reader.GetOrdinal("PieseID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("PieseID")),
                                Cost = reader.IsDBNull(reader.GetOrdinal("Cost")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Cost")),
                                Data_Incepere = reader.IsDBNull(reader.GetOrdinal("Data_Incepere")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("Data_Incepere")),
                                DataFinalizare = reader.IsDBNull(reader.GetOrdinal("DataFinalizare")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("DataFinalizare"))
                            };
                        }
                    }
                }
            }

            // Dacă nu am găsit entitatea, returnăm NotFound
            if (angajatServiciu == null)
            {
                return NotFound();
            }

            // Returnăm view-ul Delete confirmare
            return View(angajatServiciu);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            // Ștergem entitatea din tabelul Angajati_Servicii pe baza ID-ului
            string sqlDelete = "DELETE FROM Angajati_Servicii WHERE Angajati_ServiciiId = @Angajati_ServiciiId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sqlDelete, connection))
                {
                    // Adăugăm parametrul ID
                    command.Parameters.AddWithValue("@Angajati_ServiciiId", id);

                    // Deschidem conexiunea
                    connection.Open();

                    // Executăm comanda de ștergere
                    command.ExecuteNonQuery();
                }
            }

            // După ștergere, redirecționăm utilizatorul înapoi la index
            return RedirectToAction("Index");
        }


        private bool Angajati_ServiciiExists(int id)
        {
            return _context.Angajati_Servicii.Any(e => e.Angajati_ServiciiId == id);
        }
    }
}
