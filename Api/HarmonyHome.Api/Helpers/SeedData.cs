using HarmonyHome.Api.Models.Entity;
using Microsoft.AspNetCore.Identity;
using HarmonyHome.Api.Data;
using HarmonyHome.Api.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace HarmonyHome.Api.Helpers
{
    public static class SeedData
    {
        public static async Task InicializarRolesYUsuariosAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles =
            {
                "Vendedor",
                "EncargadoTienda",
                "Logistico",
                "SupervisorLogistico",
                "Administrador"
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            await CrearUsuarioSiNoExiste(
                userManager,
                "vendedor@harmonyhome.com",
                "VendedorDemo",
                "Vendedor",
                "Password123!"
            );

            await CrearUsuarioSiNoExiste(
                userManager,
                "encargado@harmonyhome.com",
                "EncargadoDemo",
                "EncargadoTienda",
                "Password123!"
            );

            await CrearUsuarioSiNoExiste(
                userManager,
                "logistico@harmonyhome.com",
                "LogisticoDemo",
                "Logistico",
                "Password123!"
            );

            await CrearUsuarioSiNoExiste(
                userManager,
                "supervisor@harmonyhome.com",
                "SupervisorDemo",
                "SupervisorLogistico",
                "Password123!"
            );

            await CrearUsuarioSiNoExiste(
                userManager,
                "admin@harmonyhome.com",
                "AdminDemo",
                "Administrador",
                "Password123!"
            );


        }

        public static async Task InicializarDatosPruebaAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if (await context.Productos.AnyAsync(p => p.Referencia == "SEED-REC-001"))
            {
                return;
            }

            var admin = await userManager.FindByEmailAsync("admin@harmonyhome.com");

            if (admin == null)
            {
                return;
            }

            var tienda = new Ubicacion
            {
                Codigo = "TIENDA-01",
                Nombre = "Tienda principal",
                TipoUbicacion = TipoUbicacion.Tienda,
                Activa = true
            };

            var almacenA = new Ubicacion
            {
                Codigo = "ALM-A-01",
                Nombre = "Almacén A - Estantería 01",
                TipoUbicacion = TipoUbicacion.Almacen,
                Activa = true
            };

            var almacenB = new Ubicacion
            {
                Codigo = "ALM-B-01",
                Nombre = "Almacén B - Estantería 01",
                TipoUbicacion = TipoUbicacion.Almacen,
                Activa = true
            };

            context.Ubicaciones.AddRange(tienda, almacenA, almacenB);
            await context.SaveChangesAsync();

            var cliente = new Cliente
            {
                Nombre = "Dennis",
                Apellidos = "Pasquel",
                Telefono = "600123456",
                Email = "dennis@test.com",
                Direccion = "Calle Prueba 1",
                Activo = true,
                FechaAlta = DateTime.UtcNow
            };

            context.Clientes.Add(cliente);
            await context.SaveChangesAsync();

            var productoRecogida = new Producto
            {
                Referencia = "SEED-REC-001",
                Nombre = "Producto para órdenes de recogida",
                Descripcion = "Producto de prueba para generar órdenes de recogida",
                Categoria = "Pruebas",
                PrecioCoste = 5,
                PrecioVenta = 12,
                StockMinimo = 5,
                TipoTrazabilidad = TipoTrazabilidad.SinTrazabilidad,
                Habilitado = true,
                Activo = true,
                FechaAlta = DateTime.UtcNow,
                ImagenUrl = "/images/productos/producto-prueba.jpg",
                Observaciones = "Producto seed para pruebas"
            };

            var productoBajoTienda = new Producto
            {
                Referencia = "SEED-BAJO-TIENDA",
                Nombre = "Producto bajo stock tienda",
                Descripcion = "Producto para probar bajo stock en tienda",
                Categoria = "Pruebas",
                PrecioCoste = 4,
                PrecioVenta = 9,
                StockMinimo = 5,
                TipoTrazabilidad = TipoTrazabilidad.SinTrazabilidad,
                Habilitado = true,
                Activo = true,
                FechaAlta = DateTime.UtcNow,
                ImagenUrl = "/images/productos/producto-prueba.jpg",
                Observaciones = "Debe aparecer en bajo stock tienda"
            };

            var productoOkTienda = new Producto
            {
                Referencia = "SEED-OK-TIENDA",
                Nombre = "Producto con stock suficiente tienda",
                Descripcion = "Producto que no debe aparecer en bajo stock tienda",
                Categoria = "Pruebas",
                PrecioCoste = 4,
                PrecioVenta = 9,
                StockMinimo = 5,
                TipoTrazabilidad = TipoTrazabilidad.SinTrazabilidad,
                Habilitado = true,
                Activo = true,
                FechaAlta = DateTime.UtcNow,
                ImagenUrl = "/images/productos/producto-prueba.jpg",
                Observaciones = "No debe aparecer en bajo stock tienda"
            };

            context.Productos.AddRange(productoRecogida, productoBajoTienda, productoOkTienda);
            await context.SaveChangesAsync();

            context.StockUbicaciones.AddRange(
                new StockUbicacion
                {
                    ProductoId = productoRecogida.Id,
                    UbicacionId = almacenA.Id,
                    Cantidad = 100
                },
                new StockUbicacion
                {
                    ProductoId = productoRecogida.Id,
                    UbicacionId = almacenB.Id,
                    Cantidad = 50
                },
                new StockUbicacion
                {
                    ProductoId = productoBajoTienda.Id,
                    UbicacionId = tienda.Id,
                    Cantidad = 3
                },
                new StockUbicacion
                {
                    ProductoId = productoBajoTienda.Id,
                    UbicacionId = almacenA.Id,
                    Cantidad = 20
                },
                new StockUbicacion
                {
                    ProductoId = productoOkTienda.Id,
                    UbicacionId = tienda.Id,
                    Cantidad = 10
                }
            );

            await context.SaveChangesAsync();

            await CrearOrdenesRecogidaSeed(context, cliente.Id, admin.Id, productoRecogida);
        }

        private static async Task CrearOrdenesRecogidaSeed(
    ApplicationDbContext context,
    int clienteId,
    string usuarioId,
    Producto producto)
        {
            await CrearOrdenRecogidaSeed(context, clienteId, usuarioId, producto, EstadoOrden.Pendiente, 1, "Seed pendiente 1");
            await CrearOrdenRecogidaSeed(context, clienteId, usuarioId, producto, EstadoOrden.Pendiente, 1, "Seed pendiente 2");

            await CrearOrdenRecogidaSeed(context, clienteId, usuarioId, producto, EstadoOrden.Asignada, 1, "Seed asignada 1");
            await CrearOrdenRecogidaSeed(context, clienteId, usuarioId, producto, EstadoOrden.Asignada, 1, "Seed asignada 2");

            await CrearOrdenRecogidaSeed(context, clienteId, usuarioId, producto, EstadoOrden.EnPreparacion, 1, "Seed en preparación 1");
            await CrearOrdenRecogidaSeed(context, clienteId, usuarioId, producto, EstadoOrden.EnPreparacion, 1, "Seed en preparación 2");

            await CrearOrdenRecogidaSeed(context, clienteId, usuarioId, producto, EstadoOrden.Finalizada, 1, "Seed finalizada 1");
            await CrearOrdenRecogidaSeed(context, clienteId, usuarioId, producto, EstadoOrden.Finalizada, 1, "Seed finalizada 2");

            await CrearOrdenRecogidaSeed(context, clienteId, usuarioId, producto, EstadoOrden.Cancelada, 1, "Seed cancelada 1");
            await CrearOrdenRecogidaSeed(context, clienteId, usuarioId, producto, EstadoOrden.Cancelada, 1, "Seed cancelada 2");
        }

        private static async Task CrearOrdenRecogidaSeed(
            ApplicationDbContext context,
            int clienteId,
            string usuarioId,
            Producto producto,
            EstadoOrden estadoOrden,
            int cantidad,
            string observaciones)
        {
            var pedido = new PedidoVenta
            {
                ClienteId = clienteId,
                UsuarioId = usuarioId,
                FechaCreacion = DateTime.UtcNow,
                Estado = estadoOrden == EstadoOrden.Finalizada
                    ? EstadoPedido.ListoRecogida
                    : estadoOrden == EstadoOrden.Cancelada
                        ? EstadoPedido.Cancelado
                        : EstadoPedido.Pendiente,
                TipoPedidoVenta = TipoPedidoVenta.PedidoCliente,
                Total = cantidad * producto.PrecioVenta,
                Observaciones = observaciones
            };

            pedido.Lineas.Add(new LineaPedidoVenta
            {
                ProductoId = producto.Id,
                Cantidad = cantidad,
                PrecioUnitario = producto.PrecioVenta,
                Subtotal = cantidad * producto.PrecioVenta
            });

            var orden = new OrdenRecogida
            {
                PedidoVenta = pedido,
                FechaCreacion = DateTime.UtcNow,
                Estado = estadoOrden,
                UsuarioAsignadoId = estadoOrden == EstadoOrden.Pendiente ? null : usuarioId,
                Observaciones = estadoOrden == EstadoOrden.Cancelada
                    ? $"Cancelada: {observaciones}"
                    : observaciones
            };

            context.OrdenesRecogida.Add(orden);
            await context.SaveChangesAsync();
        }


        private static async Task CrearUsuarioSiNoExiste(
            UserManager<ApplicationUser> userManager,
            string email,
            string userName,
            string role,
            string password)
        {
            var usuario = await userManager.FindByEmailAsync(email);

            if (usuario != null)
            {
                return;
            }

            var nuevoUsuario = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                NombreCompleto = userName,
                Activo = true,
                FechaAlta = DateTime.UtcNow,
                EmailConfirmed = true
            };

            var resultado = await userManager.CreateAsync(nuevoUsuario, password);

            if (resultado.Succeeded)
            {
                await userManager.AddToRoleAsync(nuevoUsuario, role);
            }
        }
    }
}