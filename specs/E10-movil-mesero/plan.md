# E10 · Móvil → Mesero · Plan técnico

## 1. Resumen

Son pantallas Blazor en `Resto.UI/Features/Waiter/` que se muestran en `Resto.App` cuando el dispositivo es de tipo *Celular de mesero* (E00). No tiene tablas propias en el servidor: usa los endpoints de E02 y el menú de E05.

## 2. Datos locales (SQLite, `LocalDbContext` de E00)

| Tabla local | Columnas | Para qué |
|---|---|---|
| `waiter_drafts` | `TableId`, `OrderId?`, `Guests`, `LinesJson`, `UpdatedAt`, `UserId` | Lo que está por enviar (BR-MES-02) |
| `cache` (E00) | `menu:{lang}`, `floor`, `myorders` | Menú, mesas y órdenes para trabajar sin conexión |

## 3. Endpoint nuevo en E02 (lo agrega este epic)

| Método y ruta | Permiso | Sin conexión |
|---|---|---|
| `POST /api/orders/submit` `{ TableId, Guests, OrderId?, Lines[] (MenuItemId, Qty, Note, ExtraIds[]) }` | `pos.take` | ✅ |

- **Si la orden es nueva:** la abre, agrega las líneas y las envía en una sola transacción (BR-MES-03).
- **Si la mesa ya está ocupada por otro:** 409 con `{ Code: "TableTaken", By: "Karla Méndez" }`.
- **Si algún platillo está agotado:** 409 con `{ Code: "SoldOut", Items: [...] }`.

Los demás (`request-bill` y aceptar pedidos del QR de E18) ya existen.

## 4. Pantallas

| Componente | Captura |
|---|---|
| `WaiterTablesPage.razor` (`/mesero`) + `TableButton.razor` + `QrPendingCard.razor` | 01, 07 |
| `GuestsSheet.razor` | 02 |
| `WaiterMenuPage.razor` + `CategoryChips.razor` + `MenuRow.razor` (stepper) | 03 |
| `WaiterOrderPage.razor` + `DraftLine.razor` (notas y extras) + `SentLine.razor` | 04, 05 |
| `InAppBanner.razor` (de E00) | 06 |

- **Estilo:** `waiter.css` con alturas mínimas de 44 px, botón principal fijo abajo con `env(safe-area-inset-bottom)` y tema claro forzado.
- **Tiempo real:** se suscribe a `OrderChanged` (mis mesas), `ItemsDispatched` (estado *Listo*) e `IncomingOrderReceived` (tarjeta del QR).

## 5. Pruebas

| Proyecto | Qué cubre |
|---|---|
| Domain.Tests | Agrupación de líneas del borrador (BR-MES-04) |
| Api.Tests | `submit` nuevo, agregar a una existente, mesa tomada, agotado e idempotencia (AC-MES-05, 06 y 10) |
| UI.Tests | `WaiterTablesPage` con mesas propias y ajenas (AC-MES-01), y precio con extras (AC-MES-03) |
| Manual | En un celular Android real: AC-MES-04, 06, 07 y 09; uso con una mano; *push* |

## 6. Paquetes

Ninguno además de E00.
