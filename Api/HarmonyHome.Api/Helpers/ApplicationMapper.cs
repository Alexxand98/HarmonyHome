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
    }
}