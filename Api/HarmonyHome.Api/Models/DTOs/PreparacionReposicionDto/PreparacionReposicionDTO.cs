namespace HarmonyHome.Api.Models.DTOs.PreparacionReposicionDto
{
    public class PreparacionReposicionDTO
    {
        public int OrdenReposicionId { get; set; }

        public string EstadoOrden { get; set; } = string.Empty;

        public List<LineaPreparacionReposicionDTO> Lineas { get; set; } = new();
    }

    public class LineaPreparacionReposicionDTO
    {
        public int ProductoId { get; set; }

        public string ProductoReferencia { get; set; } = string.Empty;

        public string ProductoNombre { get; set; } = string.Empty;

        public int CantidadSolicitada { get; set; }

        public List<UbicacionPreparacionReposicionDTO> Ubicaciones { get; set; } = new();
    }

    public class UbicacionPreparacionReposicionDTO
    {
        public int UbicacionId { get; set; }

        public string UbicacionCodigo { get; set; } = string.Empty;

        public string UbicacionNombre { get; set; } = string.Empty;

        public int CantidadDisponible { get; set; }

        public int CantidadARecoger { get; set; }
    }
}