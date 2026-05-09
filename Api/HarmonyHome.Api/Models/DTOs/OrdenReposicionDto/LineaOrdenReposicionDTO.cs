namespace HarmonyHome.Api.Models.DTOs
{
    public class LineaOrdenReposicionDTO
    {
        public int Id { get; set; }

        public int ProductoId { get; set; }

        public string ProductoReferencia { get; set; } = string.Empty;

        public string ProductoNombre { get; set; } = string.Empty;

        public int CantidadSolicitada { get; set; }

        public int CantidadPreparada { get; set; }
    }
}