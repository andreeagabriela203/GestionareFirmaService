using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FirmaService.Models
{
    public class Angajati
    {

        public int AngajatiId { get; set; }
        public string Nume { get; set; }
        public string Prenume { get; set; }
        public DateTime DataNastere { get; set; }
        public DateTime DataAngajare { get; set; }
        public int Salariu { get; set; }
        public long? CNP { get; set; }
 
        public string Adresa { get; set; }


    }
  
}
