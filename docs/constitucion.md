# Constitución de Resto

Las reglas que no se negocian. Si una spec, un plan o una petición contradice esto, gana este documento hasta que lo cambiemos los dos (Benji y Daniel) en un PR propio.

## 1. Principios de producto

1. **No darle más trabajo a nadie.** El chef, el comprador y el mesero ya están ocupados. Cada pantalla se diseña para resolverse en el menor número de toques posible. Si una acción común pide más de 3 toques, se justifica en la spec.
2. **Botones grandes y claros.** En celular, el área tocable mide al menos 44×44 px. Las acciones principales van abajo, al alcance del pulgar.
3. **Todo queda registrado.** Quién lo pidió, quién lo hizo y cuándo. No hay acciones anónimas.
4. **Nunca se deja de vender.** Si se cae el internet, el mesero sigue tomando órdenes y la caja sigue cobrando. Todo se sincroniza al volver la señal.
5. **Configurable por restaurante.** Impuestos, merma, roles, propiedades del inventario, estaciones e impresoras se configuran desde la app, no en el código.
6. **Se ve formal y profesional.** Paleta azul marino con acento cobre, igual que el demo aprobado.
7. **Accesible.** Contraste de texto AA (4.5:1), nada se comunica solo con color (siempre va texto o ícono), todo se puede usar con teclado en escritorio y los textos respetan el tamaño de letra del sistema en celular.
8. **Privacidad.** Solo se piden los datos personales que se usan. Antes de pedirlos se enlaza el aviso de privacidad (LFPDPPP) y se pueden borrar a petición del cliente (E00 BR-PLT-16).

## 2. Stack (decidido)

| Pieza | Tecnología |
|---|---|
| Lenguaje | C# 14 / .NET 10 LTS, 100% C# |
| Servidor | ASP.NET Core (Minimal APIs) + SignalR |
| Base de datos | PostgreSQL + EF Core |
| Pantallas | Blazor (Razor Class Library compartida) |
| App instalada | .NET MAUI Blazor Hybrid: Windows (caja), Android, iOS |
| Web | Blazor Web App: administración, pantalla de cocina, menú QR, pedidos web, portal de facturación y registro |
| Sin conexión | SQLite local en la app (EF Core) + cola de sincronización |
| Impresión | ESC/POS por red (puerto 9100) o USB en Windows |
| Pruebas | xUnit, bUnit (componentes), Testcontainers (PostgreSQL real en pruebas de API) |

No se agregan frameworks o paquetes fuera de esta lista sin anotarlo en el `plan.md` del epic y aprobarlo en el PR.

## 3. Estructura de la solución

```
src/
  Resto.Domain          entidades y reglas de negocio puras, sin EF ni HTTP
  Resto.Contracts       DTOs, comandos y eventos de SignalR compartidos
  Resto.Infrastructure  EF Core, PostgreSQL, PAC, pagos, correo
  Resto.Api             endpoints, hubs, autenticación, permisos
  Resto.UI              Razor Class Library: todas las pantallas, por Feature
  Resto.App             MAUI Blazor Hybrid
  Resto.Web             Blazor Web App
  Resto.Printing        tickets y comandas ESC/POS
  Resto.Admin           consola interna del equipo de Resto (E21)
tests/
  Resto.Domain.Tests
  Resto.Api.Tests
  Resto.UI.Tests
```

- **Domain no depende de nadie.** Las reglas (totales, impuestos, merma, sugeridos) viven ahí y las usan el servidor, la app y la web.
- **UI se organiza por feature:** `Resto.UI/Features/Inventory/…`, `Features/Purchasing/…`. Un epic toca, en lo posible, solo su carpeta.

## 4. Reglas técnicas

1. **Dinero y cantidades en `decimal`.** Dinero con 2 decimales y cantidades de inventario con 3. En PostgreSQL: `numeric(12,2)` y `numeric(12,3)`. Redondeo `MidpointRounding.AwayFromZero`.
2. **Multi restaurante.** Toda entidad de negocio tiene `OrganizationId` (el cliente que paga) y `RestaurantId` (la sucursal), y EF Core aplica un filtro global por el restaurante del usuario. Nunca se consulta sin ese filtro. Ver [`modelo-datos.md`](modelo-datos.md).
3. **Auditoría.** Toda escritura pasa por un servicio que registra `AuditEntry` con usuario, fecha UTC, módulo, acción, valores antes y después. La auditoría no se edita ni se borra.
4. **Permisos y plan en el servidor.** Cada endpoint declara su permiso (`.RequirePermission(Permissions.InventoryAdjust)`) y su módulo (`.RequireModule(Modules.Inventory)`, E00; E21 decide qué módulos tiene cada plan). La UI esconde lo que no puedes usar, pero eso no sustituye la validación.
5. **Fechas.** Se guardan en UTC. Se muestran en la zona del restaurante (por defecto `America/Tijuana`).
6. **Sin borrado físico** de datos de negocio. Se desactiva (`IsActive = false`) para no romper historial ni recetas.
7. **Concurrencia.** Las existencias se modifican con operaciones atómicas o `RowVersion`. Nunca "leo, sumo en memoria y guardo".
8. **Tiempo real.** Los cambios que otra pantalla necesita ver (comandas, existencias, avisos) se publican por SignalR con eventos definidos en `Resto.Contracts`. La lista completa está en [`contratos.md`](contratos.md).
9. **Contratos entre epics.** Un epic solo usa a otro a través de las interfaces de [`contratos.md`](contratos.md), nunca leyendo sus tablas directamente.
10. **Seguridad.** Solo HTTPS, secretos fuera del código y del repositorio, límite de intentos en endpoints públicos y respaldos probados (E00 BR-PLT-14 y 15).

## 5. Convenciones de código

- Convenciones de Microsoft para C# y .NET: `PascalCase` para tipos y miembros públicos, `_camelCase` para campos privados, `Async` al final de métodos asíncronos, *nullable* activado y *warnings as errors* en Domain.
- **Código en inglés y textos de pantalla en español.** Los textos de pantalla van en archivos de recursos para poder traducir después.
- Consultas EF simples y legibles. Usa `AsNoTracking()` en lecturas. Nada de cargar tablas completas para filtrar en memoria.
- Un componente Razor no habla con la base de datos. Llama a un servicio o cliente de API.

### Glosario (español → código)

| Pantalla | Código | | Pantalla | Código |
|---|---|---|---|---|
| Restaurante | `Restaurant` | | Platillo | `MenuItem` |
| Insumo | `Ingredient` | | Receta | `Recipe` / `RecipeLine` |
| Existencia | `Stock` | | Paquete | `Bundle` |
| Mínimo | `MinStock` | | Comanda / orden | `Order` / `OrderItem` |
| Merma | `Waste` (`WastePercent`) | | Estación de cocina | `KitchenStation` |
| Movimiento de inventario | `InventoryMovement` | | Mesero | `Waiter` |
| Unidad | `UnitOfMeasure` | | Corte de caja | `CashCut` (X / Z) |
| Almacén / ubicación | `StorageLocation` | | Lista de compras | `PurchaseRequest` |
| Proveedor | `Supplier` | | Auditoría | `AuditEntry` |
| Propiedad extra | `CustomField` | | Rol / permiso | `Role` / `Permission` |

## 6. Pruebas

- Cada regla de negocio `BR-…` tiene al menos una prueba con su ID en el nombre.
- Cada criterio de aceptación `AC-…` se cubre con una prueba de API o de componente, o se marca como *manual* en `tasks.md` con los pasos.
- Las pruebas de API usan PostgreSQL real (Testcontainers), no la base en memoria.

## 7. Definición de terminado

Una tarea está terminada cuando:

1. Compila sin warnings nuevos y pasan todas las pruebas.
2. Cumple los `AC-…` que la tarea cita.
3. Escribe auditoría si modifica datos.
4. Valida permisos en el servidor si expone un endpoint.
5. Se ve como el demo en escritorio (1366 px) y, si aplica, en celular (390 px).
6. Tiene su casilla marcada en `tasks.md` y su PR revisado por la otra persona.

## 8. Diseño

Los tokens salen del demo aprobado y se ponen en `Resto.UI/wwwroot/css/tokens.css`:

| Token | Claro | Oscuro |
|---|---|---|
| `--navy` | `#1B2B3F` | `#2C4766` |
| `--copper` | `#B8713A` | `#D08A52` |
| `--bg` | `#F6F4F0` | `#0F151D` |
| `--surface` | `#FFFFFF` | `#17202B` |
| `--ok` / `--warn` / `--bad` | `#2E7D5B` / `#9A6512` / `#C0392B` | `#55B387` / `#E3AA45` / `#E5705F` |

Tipografías: *Schibsted Grotesk* para títulos, *Instrument Sans* para texto e *IBM Plex Mono* para tickets.
