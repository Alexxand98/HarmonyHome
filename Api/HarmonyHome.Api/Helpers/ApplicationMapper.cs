using HarmonyHome.Api.Models.DTO;
using HarmonyHome.Api.Models.DTOs;
using HarmonyHome.Api.Models.Entity;

namespace HarmonyHome.Api.Helpers
{
    public static class ApplicationMapper
    {
        public static ProductoDTO ToProductoDTO(Producto producto)
        {
            return new ProductoDTO
            {
                Id = producto.Id,
                Referencia = producto.Referencia,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Categoria = producto.Categoria,
                PrecioCoste = producto.PrecioCoste,
                PrecioVenta = producto.PrecioVenta,
                StockMinimo = producto.StockMinimo,
                TipoTrazabilidad = producto.TipoTrazabilidad,
                TipoTrazabilidadNombre = producto.TipoTrazabilidad.ToString(),
                Habilitado = producto.Habilitado,
                Activo = producto.Activo,
                FechaAlta = producto.FechaAlta,
                ImagenUrl = producto.ImagenUrl,
                Observaciones = producto.Observaciones
            };
        }

        public static Producto ToProducto(CreateProductoDTO dto)
        {
            return new Producto
            {
                Referencia = dto.Referencia,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Categoria = dto.Categoria,
                PrecioCoste = dto.PrecioCoste,
                PrecioVenta = dto.PrecioVenta,
                StockMinimo = dto.StockMinimo,
                TipoTrazabilidad = dto.TipoTrazabilidad,
                Habilitado = dto.Habilitado,
                Activo = true,
                FechaAlta = DateTime.UtcNow,
                ImagenUrl = dto.ImagenUrl,
                Observaciones = dto.Observaciones
            };
        }

        public static void UpdateProducto(Producto producto, UpdateProductoDTO dto)
        {
            producto.Referencia = dto.Referencia;
            producto.Nombre = dto.Nombre;
            producto.Descripcion = dto.Descripcion;
            producto.Categoria = dto.Categoria;
            producto.PrecioCoste = dto.PrecioCoste;
            producto.PrecioVenta = dto.PrecioVenta;
            producto.StockMinimo = dto.StockMinimo;
            producto.TipoTrazabilidad = dto.TipoTrazabilidad;
            producto.Habilitado = dto.Habilitado;
            producto.Activo = dto.Activo;
            producto.ImagenUrl = dto.ImagenUrl;
            producto.Observaciones = dto.Observaciones;
        }

        public static UbicacionDTO ToUbicacionDTO(Ubicacion ubicacion)
        {
            return new UbicacionDTO
            {
                Id = ubicacion.Id,
                Codigo = ubicacion.Codigo,
                Nombre = ubicacion.Nombre,
                TipoUbicacion = ubicacion.TipoUbicacion,
                TipoUbicacionNombre = ubicacion.TipoUbicacion.ToString(),
                Activa = ubicacion.Activa
            };
        }

        public static Ubicacion ToUbicacion(CreateUbicacionDTO dto)
        {
            return new Ubicacion
            {
                Codigo = dto.Codigo,
                Nombre = dto.Nombre,
                TipoUbicacion = dto.TipoUbicacion,
                Activa = true
            };
        }

        public static void UpdateUbicacion(Ubicacion ubicacion, UpdateUbicacionDTO dto)
        {
            ubicacion.Codigo = dto.Codigo;
            ubicacion.Nombre = dto.Nombre;
            ubicacion.TipoUbicacion = dto.TipoUbicacion;
            ubicacion.Activa = dto.Activa;
        }

        public static ClienteDTO ToClienteDTO(Cliente cliente)
        {
            return new ClienteDTO
            {
                Id = cliente.Id,
                Nombre = cliente.Nombre,
                Apellidos = cliente.Apellidos,
                Telefono = cliente.Telefono,
                Email = cliente.Email,
                Direccion = cliente.Direccion,
                Activo = cliente.Activo,
                FechaAlta = cliente.FechaAlta
            };
        }


        public static Cliente ToCliente(CreateClienteDTO dto)
        {
            return new Cliente
            {
                Nombre = dto.Nombre,
                Apellidos = dto.Apellidos,
                Telefono = dto.Telefono,
                Email = dto.Email,
                Direccion = dto.Direccion,
                Activo = true,
                FechaAlta = DateTime.UtcNow
            };
        }


        public static void UpdateCliente(Cliente cliente, UpdateClienteDTO dto)
        {
            cliente.Nombre = dto.Nombre;
            cliente.Apellidos = dto.Apellidos;
            cliente.Telefono = dto.Telefono;
            cliente.Email = dto.Email;
            cliente.Direccion = dto.Direccion;
            cliente.Activo = dto.Activo;
        }
       public static StockUbicacionDTO ToStockUbicacionDTO(StockUbicacion stock)
        {
            return new StockUbicacionDTO
            {
                Id = stock.Id,
                ProductoId = stock.ProductoId,
                ProductoReferencia = stock.Producto?.Referencia ?? string.Empty,
                ProductoNombre = stock.Producto?.Nombre ?? string.Empty,
                UbicacionId = stock.UbicacionId,
                UbicacionCodigo = stock.Ubicacion?.Codigo ?? string.Empty,
                UbicacionNombre = stock.Ubicacion?.Nombre ?? string.Empty,
                TipoUbicacion = stock.Ubicacion?.TipoUbicacion ?? 0,
                TipoUbicacionNombre = stock.Ubicacion?.TipoUbicacion.ToString() ?? string.Empty,
                Cantidad = stock.Cantidad
            };
        }


        public static LineaVentaDTO ToLineaVentaDTO(LineaPedidoVenta linea)
        {
            return new LineaVentaDTO
            {
                Id = linea.Id,
                ProductoId = linea.ProductoId,
                ProductoReferencia = linea.Producto?.Referencia ?? string.Empty,
                ProductoNombre = linea.Producto?.Nombre ?? string.Empty,
                Cantidad = linea.Cantidad,
                PrecioUnitario = linea.PrecioUnitario,
                Subtotal = linea.Subtotal
            };
        }
        public static VentaDTO ToVentaDTO(PedidoVenta pedido)
        {
            return new VentaDTO
            {
                Id = pedido.Id,
                ClienteId = pedido.ClienteId,
                ClienteNombreCompleto = pedido.Cliente == null ? string.Empty : $"{pedido.Cliente.Nombre} {pedido.Cliente.Apellidos}".Trim(),
                UsuarioId = pedido.UsuarioId,
                FechaCreacion = pedido.FechaCreacion,
                Estado = pedido.Estado,
                EstadoNombre = pedido.Estado.ToString(),
                TipoPedidoVenta = pedido.TipoPedidoVenta,
                TipoPedidoVentaNombre = pedido.TipoPedidoVenta.ToString(),
                Total = pedido.Total,
                Observaciones = pedido.Observaciones,
                Lineas = pedido.Lineas.Select(ToLineaVentaDTO).ToList()
            };
        }



        public static LineaPedidoClienteDTO ToLineaPedidoClienteDTO(LineaPedidoVenta linea)
        {
            return new LineaPedidoClienteDTO
            {
                Id = linea.Id,
                ProductoId = linea.ProductoId,
                ProductoReferencia = linea.Producto?.Referencia ?? string.Empty,
                ProductoNombre = linea.Producto?.Nombre ?? string.Empty,
                Cantidad = linea.Cantidad,
                PrecioUnitario = linea.PrecioUnitario,
                Subtotal = linea.Subtotal
            };
        }


        public static PedidoClienteDTO ToPedidoClienteDTO(PedidoVenta pedido, OrdenRecogida? ordenRecogida = null)
        {
            return new PedidoClienteDTO
            {
                Id = pedido.Id,
                ClienteId = pedido.ClienteId,
                ClienteNombreCompleto = pedido.Cliente == null ? string.Empty : $"{pedido.Cliente.Nombre} {pedido.Cliente.Apellidos}".Trim(),
                UsuarioId = pedido.UsuarioId,
                FechaCreacion = pedido.FechaCreacion,
                Estado = pedido.Estado,
                EstadoNombre = pedido.Estado.ToString(),
                TipoPedidoVenta = pedido.TipoPedidoVenta,
                TipoPedidoVentaNombre = pedido.TipoPedidoVenta.ToString(),
                Total = pedido.Total,
                Observaciones = pedido.Observaciones,
                OrdenRecogidaId = ordenRecogida?.Id,
                EstadoOrdenRecogidaNombre = ordenRecogida?.Estado.ToString(),
                Lineas = pedido.Lineas.Select(ToLineaPedidoClienteDTO).ToList()
            };
        }



        public static OrdenRecogidaDTO ToOrdenRecogidaDTO(OrdenRecogida orden)
        {

            var pedido = orden.PedidoVenta;

            return new OrdenRecogidaDTO
            {
               
                Id = orden.Id,
               
                PedidoVentaId = orden.PedidoVentaId,
                
                FechaCreacion = orden.FechaCreacion,
               
                Estado = orden.Estado,
               
                EstadoNombre = orden.Estado.ToString(),
                UsuarioAsignadoId = orden.UsuarioAsignadoId,
               
                Observaciones = orden.Observaciones,

                ClienteId = pedido?.ClienteId ?? 0,
                ClienteNombreCompleto = pedido?.Cliente == null ? string.Empty : $"{pedido.Cliente.Nombre} {pedido.Cliente.Apellidos}".Trim(),

                TotalPedido = pedido?.Total ?? 0,

                LineasPedido = pedido?.Lineas.Select(ToLineaPedidoClienteDTO).ToList() ?? new List<LineaPedidoClienteDTO>()
            };
        }



        public static LineaOrdenReposicionDTO ToLineaOrdenReposicionDTO(LineaOrdenReposicion linea)
        {
            return new LineaOrdenReposicionDTO
            {
                Id = linea.Id,
                ProductoId = linea.ProductoId,
                ProductoReferencia = linea.Producto?.Referencia ?? string.Empty,
                ProductoNombre = linea.Producto?.Nombre ?? string.Empty,
                CantidadSolicitada = linea.CantidadSolicitada,
                CantidadPreparada = linea.CantidadPreparada
            };
        }



        public static OrdenReposicionDTO ToOrdenReposicionDTO(OrdenReposicion orden)
        {
            return new OrdenReposicionDTO
            {
                Id = orden.Id,
                FechaSolicitud = orden.FechaSolicitud,
                Estado = orden.Estado,
                EstadoNombre = orden.Estado.ToString(),
                UsuarioSolicitanteId = orden.UsuarioSolicitanteId,
                UsuarioPreparadorId = orden.UsuarioPreparadorId,
                Observaciones = orden.Observaciones,
                Lineas = orden.Lineas.Select(ToLineaOrdenReposicionDTO).ToList()
            };
        }



        public static DemarcaDTO ToDemarcaDTO(MovimientoStock movimiento)
        {

            return new DemarcaDTO
            {
                MovimientoStockId = movimiento.Id,
                ProductoId = movimiento.ProductoId,
                ProductoReferencia = movimiento.Producto?.Referencia ?? string.Empty,
                ProductoNombre = movimiento.Producto?.Nombre ?? string.Empty,
                UbicacionId = movimiento.UbicacionOrigenId ?? 0,
                UbicacionCodigo = movimiento.UbicacionOrigen?.Codigo ?? string.Empty,
                UbicacionNombre = movimiento.UbicacionOrigen?.Nombre ?? string.Empty,
                Cantidad = movimiento.Cantidad,
                TipoMovimiento = movimiento.TipoMovimiento,
                TipoMovimientoNombre = movimiento.TipoMovimiento.ToString(),
                UsuarioId = movimiento.UsuarioId,
                Fecha = movimiento.Fecha,
                Motivo = movimiento.Observaciones
            };


        }

    }


}