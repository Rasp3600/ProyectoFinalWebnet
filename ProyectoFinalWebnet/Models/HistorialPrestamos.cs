namespace ProyectoFinalWebnet.Models
{
    public class HistorialPrestamos
    {
        public int ID { get; set; }
        public int LibroID { get; set; }
        public string UsuarioID { get; set; }
        public DateTime FechaPrestamos { get; set; }
        public DateTime DevolucionReal { get; set; }
   
        public string Estado { get; set; }

        public string UserName { get; set; }
    }
}
