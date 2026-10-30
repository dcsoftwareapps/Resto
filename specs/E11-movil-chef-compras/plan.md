# E11 · Móvil → Chef y compras · Plan técnico

## 1. Resumen

Son pantallas en `Resto.UI/Features/KitchenBuyer/` que se muestran en `Resto.App` cuando el dispositivo es *Celular de chef o compras*. No tienen tablas propias: usan los endpoints de E06 (`/api/purchasing/...`) y la consulta de faltantes de E04.

## 2. Datos locales

| Tabla local | Para qué |
|---|---|
| `chef_selection` (`IngredientId`, `QtyBase`) | Selección por enviar (BR-CHF-02) |
| `count_drafts` (`CountId?`, `IngredientId`, `Counted`, `CountedAt`) | Conteos capturados sin conexión (BR-CHF-08) |
| `cache`: `reorder`, `myrequests` y `buyerlist` | Trabajar sin conexión |

## 3. Endpoints usados (todos de E06)

| Uso | Endpoint | Sin conexión |
|---|---|---|
| Faltan | `GET /api/purchasing/suggestions` | caché |
| Buscar | `GET /api/inventory/ingredients?search=` (E04, con `inventory.view` o `purchasing.request`) | caché |
| Enviar a compras | `POST /api/purchasing/requests/batch` | ✅ |
| Mis pedidos / Lista del día | `GET /api/purchasing/requests?mine=true` / `?status=pending&includeToday=true` | caché |
| Comprado / No encontrado | `POST …/{id}/purchased` / `…/{id}/not-found` | ✅ |
| Terminé | `POST /api/purchasing/finish` | ✅ |
| Contar en una toma | `GET /api/inventory/counts?status=open` · `PUT /api/inventory/counts/{id}/lines/{ingredientId}` (E04) | ✅ |
| Conteo rápido / Entrada rápida | `POST /api/inventory/ingredients/{id}/adjustments` (E04, tipos `PhysicalCount` y `ManualEntry`) | ✅ |

Los tres endpoints de E06 que cambian datos se marcan `[OfflineCapable]` en este epic.

## 4. Pantallas

| Componente | Captura |
|---|---|
| `KitchenBuyerShell.razor` (selector según permiso + barra de pestañas) | 01 |
| `MissingTab.razor` + `NeedCard.razor` (agregar, listo y ajuste) | 01, 02 |
| `SearchTab.razor` | 03 |
| `MyRequestsTab.razor` | 04 |
| `BuyerListPage.razor` + `SupplierGroup.razor` | 05 |
| `MarkPurchasedSheet.razor` (comparte la lógica con `MarkPurchasedDialog` de E06) | 06 |
| `QuantityStepper.razor` en `Shared/` (lo usan E10 y E11) | 02, 06 |
| `CountTab.razor` (toma, conteo rápido y entrada rápida) | 08 |

**Tiempo real:** `PurchaseRequestChanged` e `IngredientBelowMinimum`.

## 5. Pruebas

| Proyecto | Qué cubre |
|---|---|
| Domain.Tests | Paso del ajuste por unidad (BR-CHF-03) |
| UI.Tests | AC-CHF-01, 02, 04 y 09 |
| Api.Tests | Ya cubiertos en E06; aquí se agrega idempotencia del lote desde la cola |
| Manual | AC-CHF-03, 05 a 08 en celular real |

## 6. Paquetes

Ninguno.
