using HarmonyHome.Api.Data;
using HarmonyHome.Api.Models.Entity;
using HarmonyHome.Api.Models.Enums;
using Microsoft.AspNetCore.Identity;
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

            await CrearUsuarioSiNoExiste(userManager, "vendedor@harmonyhome.com", "VendedorDemo", "Vendedor", "Password123!");
            await CrearUsuarioSiNoExiste(userManager, "encargado@harmonyhome.com", "EncargadoDemo", "EncargadoTienda", "Password123!");
            await CrearUsuarioSiNoExiste(userManager, "logistico@harmonyhome.com", "LogisticoDemo", "Logistico", "Password123!");
            await CrearUsuarioSiNoExiste(userManager, "supervisor@harmonyhome.com", "SupervisorDemo", "SupervisorLogistico", "Password123!");
            await CrearUsuarioSiNoExiste(userManager, "admin@harmonyhome.com", "AdminDemo", "Administrador", "Password123!");
        }

        public static async Task InicializarDatosBaseAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if (await context.Productos.AnyAsync(p => p.Referencia == "MESA-NORDIK-001"))
            {
                return;
            }

            var vendedor = await userManager.FindByEmailAsync("vendedor@harmonyhome.com");
            var logistico = await userManager.FindByEmailAsync("logistico@harmonyhome.com");
            var admin = await userManager.FindByEmailAsync("admin@harmonyhome.com");

            if (vendedor == null || logistico == null || admin == null)
            {
                return;
            }

            var tienda = await ObtenerOCrearUbicacionAsync(
                context,
                "TIENDA-01",
                "Tienda principal",
                TipoUbicacion.Tienda
            );

            var almacenA = await ObtenerOCrearUbicacionAsync(
                context,
                "ALM-A-01",
                "Almacén A - Estantería 01",
                TipoUbicacion.Almacen
            );

            var almacenB = await ObtenerOCrearUbicacionAsync(
                context,
                "ALM-B-01",
                "Almacén B - Estantería 01",
                TipoUbicacion.Almacen
            );

            var recogida = await ObtenerOCrearUbicacionAsync(
                context,
                "RECOGIDA-01",
                "Zona de recogida",
                TipoUbicacion.Recogida
            );

            var demarcas = await ObtenerOCrearUbicacionAsync(
                context,
                "DEMARCAS-01",
                "Zona de demarcas",
                TipoUbicacion.Demarca
            );

            var clienteCarlos = await ObtenerOCrearClienteAsync(
                context,
                "Carlos",
                "Martínez López",
                "666123456",
                "carlos@harmonyhome.com",
                "Calle Mayor 12"
            );

            var mesa = new Producto
            {
                Referencia = "MESA-NORDIK-001",
                Nombre = "Mesa comedor Nordik",
                Descripcion = "Mesa de comedor de madera clara para el catálogo de tienda.",
                Categoria = "Muebles",
                PrecioCoste = 80,
                PrecioVenta = 149,
                StockMinimo = 5,
                TipoTrazabilidad = TipoTrazabilidad.SinTrazabilidad,
                Habilitado = true,
                Activo = true,
                FechaAlta = DateTime.Now,
                ImagenUrl = "/images/productos/mesa.jpg",
                Observaciones = "Producto disponible para venta y reposición."
            };

            var silla = new Producto
            {
                Referencia = "SILLA-LUNA-001",
                Nombre = "Silla tapizada Luna",
                Descripcion = "Silla tapizada de comedor para el catálogo de tienda.",
                Categoria = "Muebles",
                PrecioCoste = 30,
                PrecioVenta = 59,
                StockMinimo = 8,
                TipoTrazabilidad = TipoTrazabilidad.SinTrazabilidad,
                Habilitado = true,
                Activo = true,
                FechaAlta = DateTime.Now,
                ImagenUrl = "/images/productos/silla.png",
                Observaciones = "Producto con stock bajo para reposición."
            };

            context.Productos.AddRange(mesa, silla);
            await context.SaveChangesAsync();

            context.StockUbicaciones.AddRange(
                new StockUbicacion
                {
                    ProductoId = mesa.Id,
                    UbicacionId = tienda.Id,
                    Cantidad = 10
                },
                new StockUbicacion
                {
                    ProductoId = mesa.Id,
                    UbicacionId = almacenA.Id,
                    Cantidad = 20
                },
                new StockUbicacion
                {
                    ProductoId = mesa.Id,
                    UbicacionId = almacenB.Id,
                    Cantidad = 10
                },
                new StockUbicacion
                {
                    ProductoId = silla.Id,
                    UbicacionId = tienda.Id,
                    Cantidad = 3
                },
                new StockUbicacion
                {
                    ProductoId = silla.Id,
                    UbicacionId = almacenA.Id,
                    Cantidad = 4
                }
            );

            await context.SaveChangesAsync();

            await CrearOrdenesRecogidaAsync(context, clienteCarlos.Id, vendedor.Id, logistico.Id, mesa, silla);
            await CrearOrdenesReposicionAsync(context, vendedor.Id, logistico.Id, mesa, silla);
            await CrearMovimientosInicialesAsync(context, admin.Id, vendedor.Id, logistico.Id, tienda.Id, almacenA.Id, almacenB.Id, demarcas.Id, mesa.Id, silla.Id);
        }

        private static async Task<Ubicacion> ObtenerOCrearUbicacionAsync(
            ApplicationDbContext context,
            string codigo,
            string nombre,
            TipoUbicacion tipoUbicacion)
        {
            var ubicacion = await context.Ubicaciones
                .FirstOrDefaultAsync(u => u.Codigo == codigo);

            if (ubicacion != null)
            {
                return ubicacion;
            }

            ubicacion = new Ubicacion
            {
                Codigo = codigo,
                Nombre = nombre,
                TipoUbicacion = tipoUbicacion,
                Activa = true
            };

            context.Ubicaciones.Add(ubicacion);
            await context.SaveChangesAsync();

            return ubicacion;
        }

        private static async Task<Cliente> ObtenerOCrearClienteAsync(
            ApplicationDbContext context,
            string nombre,
            string apellidos,
            string telefono,
            string email,
            string direccion)
        {
            var cliente = await context.Clientes
                .FirstOrDefaultAsync(c => c.Email == email);

            if (cliente != null)
            {
                return cliente;
            }

            cliente = new Cliente
            {
                Nombre = nombre,
                Apellidos = apellidos,
                Telefono = telefono,
                Email = email,
                Direccion = direccion,
                Activo = true,
                FechaAlta = DateTime.Now
            };

            context.Clientes.Add(cliente);
            await context.SaveChangesAsync();

            return cliente;
        }

        private static async Task CrearOrdenesRecogidaAsync(
            ApplicationDbContext context,
            int clienteId,
            string vendedorId,
            string logisticoId,
            Producto mesa,
            Producto silla)
        {
            await CrearOrdenRecogidaAsync(context, clienteId, vendedorId, null, mesa, EstadoOrden.Pendiente, 1, "Pedido pendiente de preparación");
            await CrearOrdenRecogidaAsync(context, clienteId, vendedorId, null, silla, EstadoOrden.Pendiente, 1, "Pedido pendiente de preparación");

            await CrearOrdenRecogidaAsync(context, clienteId, vendedorId, logisticoId, mesa, EstadoOrden.Asignada, 1, "Pedido asignado a logística");
            await CrearOrdenRecogidaAsync(context, clienteId, vendedorId, logisticoId, silla, EstadoOrden.Asignada, 1, "Pedido asignado a logística");

            await CrearOrdenRecogidaAsync(context, clienteId, vendedorId, logisticoId, mesa, EstadoOrden.EnPreparacion, 1, "Pedido en preparación");
            await CrearOrdenRecogidaAsync(context, clienteId, vendedorId, logisticoId, silla, EstadoOrden.EnPreparacion, 1, "Pedido en preparación");

            await CrearOrdenRecogidaAsync(context, clienteId, vendedorId, logisticoId, mesa, EstadoOrden.Finalizada, 1, "Pedido preparado correctamente");

            await CrearOrdenRecogidaAsync(context, clienteId, vendedorId, logisticoId, silla, EstadoOrden.Cancelada, 1, "Cancelada: el cliente cancela el pedido");
        }

        private static async Task CrearOrdenRecogidaAsync(
            ApplicationDbContext context,
            int clienteId,
            string vendedorId,
            string? logisticoId,
            Producto producto,
            EstadoOrden estadoOrden,
            int cantidad,
            string observaciones)
        {
            var pedido = new PedidoVenta
            {
                ClienteId = clienteId,
                UsuarioId = vendedorId,
                FechaCreacion = DateTime.Now,
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
                FechaCreacion = DateTime.Now,
                Estado = estadoOrden,
                UsuarioAsignadoId = logisticoId,
                Observaciones = observaciones
            };

            context.OrdenesRecogida.Add(orden);
            await context.SaveChangesAsync();
        }

        private static async Task CrearOrdenesReposicionAsync(
            ApplicationDbContext context,
            string vendedorId,
            string logisticoId,
            Producto mesa,
            Producto silla)
        {
            await CrearOrdenReposicionAsync(context, vendedorId, null, mesa, EstadoOrden.Pendiente, 3, 0, "Reposición pendiente");
            await CrearOrdenReposicionAsync(context, vendedorId, null, silla, EstadoOrden.Pendiente, 4, 0, "Reposición pendiente");

            await CrearOrdenReposicionAsync(context, vendedorId, logisticoId, mesa, EstadoOrden.Asignada, 2, 0, "Reposición asignada");

            await CrearOrdenReposicionAsync(context, vendedorId, logisticoId, silla, EstadoOrden.Finalizada, 2, 2, "Reposición finalizada");

            await CrearOrdenReposicionAsync(context, vendedorId, logisticoId, mesa, EstadoOrden.Cancelada, 1, 0, "Cancelada: reposición anulada por revisión de stock");
        }

        private static async Task CrearOrdenReposicionAsync(
            ApplicationDbContext context,
            string usuarioSolicitanteId,
            string? usuarioPreparadorId,
            Producto producto,
            EstadoOrden estadoOrden,
            int cantidadSolicitada,
            int cantidadPreparada,
            string observaciones)
        {
            var orden = new OrdenReposicion
            {
                FechaSolicitud = DateTime.Now,
                Estado = estadoOrden,
                UsuarioSolicitanteId = usuarioSolicitanteId,
                UsuarioPreparadorId = usuarioPreparadorId,
                Observaciones = observaciones
            };

            context.OrdenesReposicion.Add(orden);
            await context.SaveChangesAsync();

            var linea = new LineaOrdenReposicion
            {
                OrdenReposicionId = orden.Id,
                ProductoId = producto.Id,
                CantidadSolicitada = cantidadSolicitada,
                CantidadPreparada = cantidadPreparada
            };

            context.LineasOrdenReposicion.Add(linea);
            await context.SaveChangesAsync();
        }

        private static async Task CrearMovimientosInicialesAsync(
            ApplicationDbContext context,
            string adminId,
            string vendedorId,
            string logisticoId,
            int tiendaId,
            int almacenAId,
            int almacenBId,
            int demarcaId,
            int mesaId,
            int sillaId)
        {
            context.MovimientosStock.AddRange(
                new MovimientoStock
                {
                    ProductoId = mesaId,
                    UbicacionOrigenId = null,
                    UbicacionDestinoId = almacenAId,
                    Cantidad = 20,
                    Fecha = DateTime.Now,
                    UsuarioId = adminId,
                    TipoMovimiento = TipoMovimiento.EntradaInicial,
                    Observaciones = "Entrada inicial en almacén A"
                },
                new MovimientoStock
                {
                    ProductoId = mesaId,
                    UbicacionOrigenId = null,
                    UbicacionDestinoId = almacenBId,
                    Cantidad = 10,
                    Fecha = DateTime.Now,
                    UsuarioId = adminId,
                    TipoMovimiento = TipoMovimiento.EntradaInicial,
                    Observaciones = "Entrada inicial en almacén B"
                },
                new MovimientoStock
                {
                    ProductoId = sillaId,
                    UbicacionOrigenId = null,
                    UbicacionDestinoId = almacenAId,
                    Cantidad = 4,
                    Fecha = DateTime.Now,
                    UsuarioId = adminId,
                    TipoMovimiento = TipoMovimiento.EntradaInicial,
                    Observaciones = "Entrada inicial en almacén A"
                },
                new MovimientoStock
                {
                    ProductoId = mesaId,
                    UbicacionOrigenId = tiendaId,
                    UbicacionDestinoId = null,
                    Cantidad = 1,
                    Fecha = DateTime.Now,
                    UsuarioId = vendedorId,
                    TipoMovimiento = TipoMovimiento.VentaDirecta,
                    Observaciones = "Venta directa en tienda"
                },
                new MovimientoStock
                {
                    ProductoId = sillaId,
                    UbicacionOrigenId = almacenAId,
                    UbicacionDestinoId = tiendaId,
                    Cantidad = 2,
                    Fecha = DateTime.Now,
                    UsuarioId = logisticoId,
                    TipoMovimiento = TipoMovimiento.Reposicion,
                    Observaciones = "Reposición de almacén a tienda"
                },
                new MovimientoStock
                {
                    ProductoId = sillaId,
                    UbicacionOrigenId = tiendaId,
                    UbicacionDestinoId = demarcaId,
                    Cantidad = 1,
                    Fecha = DateTime.Now,
                    UsuarioId = vendedorId,
                    TipoMovimiento = TipoMovimiento.Demarca,
                    Observaciones = "Retirada de producto defectuoso"
                }
            );

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
                FechaAlta = DateTime.Now,
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