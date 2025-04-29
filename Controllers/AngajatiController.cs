using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FirmaService;
using FirmaService.Models;
using Microsoft.Data.SqlClient;

namespace FirmaService.Controllers
{
    public class AngajatiController : Controller
    {
      //  private readonly EFCoreDbContext _context;
        private string connectionString = "Server=LAPTOP-JIGFAA7Q;Database=FirmaService;Integrated Security=True;;Encrypt=False;";


        public Angajati GetAngajatById(int id)
        {
            Angajati angajat = null;
            string sql = "SELECT * FROM Angajati WHERE AngajatiId = @AngajatiId";

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                using( SqlCommand command = new SqlCommand(sql, sqlConnection))
                {
                    // Adăugăm parametrul
                    command.Parameters.AddWithValue("@AngajatiId", id);

                    // Deschidem conexiunea
                    sqlConnection.Open();

                    //Executamm comanda
                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            angajat = new Angajati
                            {
                                AngajatiId = reader.GetInt32(reader.GetOrdinal("AngajatiId")),
                                Nume = reader.GetString(reader.GetOrdinal("Nume")),
                                Prenume = reader.GetString(reader.GetOrdinal("Prenume")),
                                DataNastere = reader.GetDateTime(reader.GetOrdinal("DataNastere")),
                                DataAngajare = reader.GetDateTime(reader.GetOrdinal("DataAngajare")),
                                Salariu = reader.GetInt32(reader.GetOrdinal("Salariu")),
                                CNP = reader.IsDBNull(reader.GetOrdinal("CNP")) ? (long?)null : reader.GetInt64(reader.GetOrdinal("CNP")),
                                Adresa = reader.IsDBNull(reader.GetOrdinal("Adresa")) ? null : reader.GetString(reader.GetOrdinal("Adresa"))

                            };
                        }
                    }

                }
            }

            return angajat;
        }

        //GET: Afisare Angajati in pagina
        public async Task<IActionResult> Index()
        {
            List<Angajati> angajatiList = new List<Angajati>();

            string sql = "SELECT * FROM Angajati"; 

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            angajatiList.Add(new Angajati
                            {
                                AngajatiId = reader.GetInt32(reader.GetOrdinal("AngajatiId")),
                                Nume = reader.GetString(reader.GetOrdinal("Nume")),
                                Prenume = reader.GetString(reader.GetOrdinal("Prenume")),
                                DataNastere = reader.GetDateTime(reader.GetOrdinal("DataNastere")),
                                DataAngajare = reader.GetDateTime(reader.GetOrdinal("DataAngajare")),
                                Salariu = reader.GetInt32(reader.GetOrdinal("Salariu")),
                                CNP = reader.IsDBNull(reader.GetOrdinal("CNP")) ? (long?)null : reader.GetInt64(reader.GetOrdinal("CNP")),
                                Adresa = reader.IsDBNull(reader.GetOrdinal("Adresa")) ? null : reader.GetString(reader.GetOrdinal("Adresa"))
                            });
                        }
                    }
                }
            }

            return View(angajatiList);
        }   

        // GET: Angajati/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Adaugare nou angajat
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Angajati angajat)  
        {
            // Verificam daca angajatul are peste 18 ani
            var today = DateTime.Today;
            var age = today.Year - angajat.DataNastere.Year;

            if (angajat.DataNastere.Date > today.AddYears(-age))
            {
                age--;
            }

            if (age < 18)
            {
                ModelState.AddModelError("DataNastere", "Angajatul trebuie să aibă cel puțin 18 ani.");
                return View(angajat); // Returnam pagina de creare cu eroarea de validare
            }
            if (ModelState.IsValid)
            {
                string sql = @"INSERT INTO Angajati (Nume, Prenume, DataNastere, DataAngajare, Salariu, CNP, Adresa) 
                       VALUES (@Nume, @Prenume, @DataNastere, @DataAngajare, @Salariu, @CNP, @Adresa)";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Adaugam parametrii
                        command.Parameters.AddWithValue("@Nume", angajat.Nume);
                        command.Parameters.AddWithValue("@Prenume", angajat.Prenume);
                        command.Parameters.AddWithValue("@DataNastere", (object)angajat.DataNastere);
                        command.Parameters.AddWithValue("@DataAngajare", (object)angajat.DataAngajare);
                        command.Parameters.AddWithValue("@Salariu", (object)angajat.Salariu);
                        command.Parameters.AddWithValue("@CNP", (object)angajat.CNP ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Adresa", (object)angajat.Adresa ?? DBNull.Value);
                        // Deschidem conexiunea
                        connection.Open();

                        // Executam comanda
                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Index");

            }
            return View(angajat); 

        }

        // GET: Angajati/Edit
        public async Task<IActionResult> Edit(int id)
        {
            var angajati = GetAngajatById(id);
            if (angajati == null)
            {
                return NotFound();
            }
            return View(angajati);
        }

        // POST: Editare detalii angajat
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Angajati angajati)
        {
            if (id != angajati.AngajatiId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string sql = @"UPDATE Angajati
                       SET 
                           Nume = @Nume, Prenume = @Prenume, DataNastere = @DataNastere, 
                           DataAngajare = @DataAngajare, Salariu = @Salariu, CNP = @CNP, 
                           Adresa = @Adresa
                       WHERE AngajatiId = @AngajatiId";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@AngajatiId", angajati.AngajatiId);
                        command.Parameters.AddWithValue("@Nume", angajati.Nume);
                        command.Parameters.AddWithValue("@Prenume", angajati.Prenume);
                        command.Parameters.AddWithValue("@DataNastere", angajati.DataNastere);
                        command.Parameters.AddWithValue("@DataAngajare", angajati.DataAngajare);
                        command.Parameters.AddWithValue("@Salariu", angajati.Salariu);
                        command.Parameters.AddWithValue("@CNP", (object)angajati.CNP ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Adresa", (object)angajati.Adresa ?? DBNull.Value);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Index");
            }

            return View(angajati);
        }


        // GET: Angajati/Delete
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var angajati = GetAngajatById(id);           
            return View(angajati);   //Afiseaza pagina de confirmare pentru stergere
        }

        // POST: Stergere angajat
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            // SQL pentru ștergere
            string sql = "DELETE FROM Angajati WHERE AngajatiId = @AngajatiId";

            // Executăm comanda SQL cu parametrul
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@AngajatiId", id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }


      
    }
}
