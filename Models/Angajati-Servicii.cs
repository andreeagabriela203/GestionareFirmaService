namespace FirmaService.Models
{
    public class Angajati_Servicii
    {
        public int Angajati_ServiciiId {  get; set; }
        public int ServiciiId { get; set; }
        public int AngajatiId {  get; set; }
        public int DispozitiveId {  get; set; }
        public int? PieseID {  get; set; }
        public int? Cost { get; set; }
        public DateTime? Data_Incepere { get; set; }
        public DateTime? DataFinalizare { get; set; }
    }
    public class Angajati_ServiciiViewModel
	{
		public int Angajati_ServiciuId { get; set; }
		public string Serviciu { get; set; }
        public string TipDispozitiv { get; set; }
        public string Marca { get; set; }
        public string Model { get; set; }
        public string TipPiesa { get; set; }
        public string AngajatNume { get; set; }
        public string AngajatPrenume { get; set; }
        public int Cost { get; set; }
        public string Status { get; set; }
        public DateTime? DataIncepere { get; set; }
        public DateTime? DataFinalizare { get; set; }
    }
}
