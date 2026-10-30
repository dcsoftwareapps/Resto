# E01 · Escritorio → Órdenes abiertas · Plan técnico

## 1. Resumen

Pantalla de **solo lectura** que arma su vista con `IOrderQuery` (E02), las mesas de E08 y el resumen de caja de E03. No tiene tablas propias salvo `dining_tables`, que crea este epic porque E02, E08 y E10 la usan.

## 2. Modelo de datos

### `DiningTable` (`dining_tables`)

| Propiedad | Tipo | Notas |
|---|---|---|
| `Id`, `OrganizationId`, `RestaurantId` | `Guid` | |
| `Number` | `int` | Único por restaurante |
| `Name` | `string?` | "Terraza 2" |
| `Zone` | `string?` | "Salón", "Terraza" |
| `Seats` | `int` | |
| `IsActive` | `bool` | |
| `QrToken` | `string?` | Lo llena E17 |

El número de mesas que se edita en E08 crea o desactiva filas aquí.

## 3. API

| Método y ruta | Permiso | Respuesta |
|---|---|---|
| `GET /api/floor` | `orders.view` | `FloorDto { Tables[] (Id, Number, Name, Status, OrderId?, WaiterInitials, MinutesOpen), Kpis, Orders[] (OrderCardDto) }` |

- `OrderCardDto` incluye el avance, el total (de `OrderPricing`), si está prepagada y el `AgeLevel` (Normal, Warn o Late, según BR-ORD-07).
- *Vendido en el turno* sale de `ICashSessionService` (E03). Solo se manda con `cash.view`.

## 4. Pantallas (`Resto.UI/Features/Floor/`)

| Componente | Captura |
|---|---|
| `FloorPage.razor` (`/`, la pantalla de inicio de la caja) | 01 |
| `FloorKpis.razor` | 01 |
| `TableMap.razor` + `TableTile.razor` | 01 |
| `OrderCard.razor` | 01 |

- Se suscribe a `OrderChanged`, `ItemsDispatched`, `BillRequested` y `OrderPaid`, y aplica cada cambio sobre el estado local.
- Un `PeriodicTimer` de 30 s recalcula los minutos.
- Al recibir `Resynced` (E00), vuelve a pedir `GET /api/floor`.

## 5. Pruebas

| Proyecto | Qué cubre |
|---|---|
| Domain.Tests | Estado de mesa (BR-ORD-02), avance (BR-ORD-03), orden de tarjetas (BR-ORD-05) y nivel de antigüedad (BR-ORD-07) |
| Api.Tests | AC-ORD-01, 06 y 07 |
| UI.Tests | AC-ORD-03 y 04 (navegación), AC-ORD-05 (evento que actualiza la barra) |

## 6. Paquetes

Ninguno.
