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
using System.Collections.Specialized;


namespace FirmaService.Controllers
{
    public class DispozitiveController : Controller
    {
        private readonly EFCoreDbContext _context;
        private string connectionString = "Server=LAPTOP-JIGFAA7Q;Database=FirmaService;Integrated Security=True;;Encrypt=False;";


        public DispozitiveController(EFCoreDbContext context)
        {
            _context = context;
        }

        // GET: Afisare Dispozitive in pagina
        public async Task<IActionResult> Index()
        {
            List<Dispozitive> dispozitiveList = new List<Dispozitive>();

            string sql = "SELECT * FROM Dispozitive"; // Interogarea SQL pentru tabela Dispozitive

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync(); // Deschidem conexiunea asincron

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync()) // Citim rezultatele asincron
                    {
                        while (await reader.ReadAsync()) // Citim fiecare rând
                        {
                            dispozitiveList.Add(new Dispozitive
                            {
                                DispozitiveID = reader.GetInt32(reader.GetOrdinal("DispozitiveID")),
                                ClientID = reader.GetInt32(reader.GetOrdinal("ClientID")),
                                TipDispozitiv = reader.GetString(reader.GetOrdinal("TipDispozitiv")),
                                Model = reader.IsDBNull(reader.GetOrdinal("Model")) ? null : reader.GetString(reader.GetOrdinal("Model")),
                                Marca = reader.IsDBNull(reader.GetOrdinal("Marca")) ? null : reader.GetString(reader.GetOrdinal("Marca")),
                                Problema = reader.GetString(reader.GetOrdinal("Problema")),
                                Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status"))
                            });
                        }
                    }
                }
            }

            return View(dispozitiveList); 
        }

        public Dispozitive GetDispozitivById(int id)
        {
            Dispozitive dispozitiv = null;
            string sql = "SELECT * FROM Dispozitive WHERE DispozitiveID = @DispozitiveID";

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, sqlConnection))
                {
                    command.Parameters.AddWithValue("@DispozitiveID", id);
                    sqlConnection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            dispozitiv = new Dispozitive
                            {
                                DispozitiveID = reader.GetInt32(reader.GetOrdinal("DispozitiveID")),
                                ClientID = reader.GetInt32(reader.GetOrdinal("ClientID")),
                                TipDispozitiv = reader.GetString(reader.GetOrdinal("TipDispozitiv")),
                                Model = reader.IsDBNull(reader.GetOrdinal("Model")) ? null : reader.GetString(reader.GetOrdinal("Model")),
                                Marca = reader.IsDBNull(reader.GetOrdinal("Marca")) ? null : reader.GetString(reader.GetOrdinal("Marca")),
                                Problema = reader.GetString(reader.GetOrdinal("Problema")),
                                Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status"))
                            };
                        }
                    }
                }
            }

            return dispozitiv;
        }

        // GET: Deatlii Dispozitive
        public async Task<IActionResult> Details(int id)
        {
			Console.WriteLine($"ID-ul primit este: {id}");


			string sql = @"SELECT c.Nume, c.Prenume, c.AdresaEmail FROM Clienti c INNER JOIN Dispozitive d
                          ON d.ClientId=c.ClientiId WHERE d.DispozitiveId = @id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            ViewBag.Nume = reader["Nume"].ToString();
                            ViewBag.Prenume = reader["Prenume"].ToString();
                            ViewBag.AdresaEmail = reader["AdresaEmail"].ToString();
                        }
                    }
                }
            }

            return View();
        }

        // GET: Creare Dispozitive
        public IActionResult Create()
        {
            return View();
        }

        // POST: Dispozitive/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string nume, string prenume, Dispozitive dispozitiv)
        {
            if (string.IsNullOrWhiteSpace(nume) || string.IsNullOrWhiteSpace(prenume))
            {
                ModelState.AddModelError(string.Empty, "Numele și prenumele clientului sunt obligatorii.");
                return View(dispozitiv);
            }
            if (dispozitiv.Status != "Finalizat" && dispozitiv.Status != "In lucru")
            {
                ModelState.AddModelError(string.Empty, "Statusul dispozitivului trebuie să fie 'Finalizat' sau 'In lucru'.");
                return View(dispozitiv);
            }
            // SQL pentru a verifica dacă există un client cu Nume și Prenume
            string checkClientSql = @"SELECT ClientiId FROM Clienti WHERE Nume = @Nume AND Prenume = @Prenume";

            int? clientId = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                // Verificăm dacă există clientul
                using (SqlCommand command = new SqlCommand(checkClientSql, connection))
                {
                    command.Parameters.AddWithValue("@Nume", nume);
                    command.Parameters.AddWithValue("@Prenume", prenume);
                    

                    var result = await command.ExecuteScalarAsync();
                    if (result != null)
                    {
                        clientId = Convert.ToInt32(result); // Clientul există
                    }
                }

                // Dacă clientul nu există, returnăm un mesaj de eroare
                if (clientId == null)
                {
                    ModelState.AddModelError(string.Empty, "Clientul cu numele și prenumele specificate nu există.");
                    return View(dispozitiv); // Rămânem pe aceeași pagină pentru a corecta datele
                }
                

                // Dacă clientul există, creăm dispozitivul
                string insertDeviceSql = @"INSERT INTO Dispozitive (ClientID, TipDispozitiv, Model, Marca, Problema, Status) 
                                   VALUES (@ClientID, @TipDispozitiv, @Model, @Marca, @Problema, @Status)";

                using (SqlCommand insertCommand = new SqlCommand(insertDeviceSql, connection))
                {
                    insertCommand.Parameters.AddWithValue("@ClientID", clientId);
                    insertCommand.Parameters.AddWithValue("@TipDispozitiv", dispozitiv.TipDispozitiv);
                    insertCommand.Parameters.AddWithValue("@Model", (object)dispozitiv.Model ?? DBNull.Value);
                    insertCommand.Parameters.AddWithValue("@Marca", (object)dispozitiv.Marca ?? DBNull.Value);
                    insertCommand.Parameters.AddWithValue("@Problema", dispozitiv.Problema);
                    insertCommand.Parameters.AddWithValue("@Status", (object)dispozitiv.Status ?? DBNull.Value);
                    
                    await insertCommand.ExecuteNonQueryAsync();
                }
            }

            return RedirectToAction("Index");
        }



        // GET: Dispozitive/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var a = GetDispozitivById(id);
            if (a == null)
            {
                return NotFound();
            }
            return View(a);
        }

        // POST: Dispozitive/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Dispozitive dispozitiv)
        {
            if (id != dispozitiv.DispozitiveID)
            {
                return NotFound();
            }
            ModelState.Remove("ClientNume");
            ModelState.Remove("ClientPrenume");
            if (ModelState.IsValid)
            {
                string sql = @"UPDATE Dispozitive
                       SET 
                            TipDispozitiv = @TipDispozitiv, Model = @Model, 
                           Marca = @Marca, Problema = @Problema, Status = @Status
                       WHERE DispozitiveID = @DispozitiveID";



                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@DispozitiveID", dispozitiv.DispozitiveID);
                        command.Parameters.AddWithValue("@TipDispozitiv", dispozitiv.TipDispozitiv);
                        command.Parameters.AddWithValue("@Model", (object)dispozitiv.Model ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Marca", (object)dispozitiv.Marca ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Problema", dispozitiv.Problema);
                        command.Parameters.AddWithValue("@Status", (object)dispozitiv.Status ?? DBNull.Value);
                        

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Index");
            }

            return View(dispozitiv);
        }


        // GET: Dispozitive/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dispozitive = await _context.Dispozitive
                .FirstOrDefaultAsync(m => m.DispozitiveID == id);
            if (dispozitive == null)
            {
                return NotFound();
            }

            return View(dispozitive);
        }

        // POST: Dispozitive/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            // SQL pentru ștergere
            string sql = "DELETE FROM Dispozitive WHERE DispozitiveID = @DispozitiveID";

            // Executăm comanda SQL cu parametrul
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@DispozitiveID", id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }


        private bool DispozitiveExists(int id)
        {
            return _context.Dispozitive.Any(e => e.DispozitiveID == id);
        }
    }
}
