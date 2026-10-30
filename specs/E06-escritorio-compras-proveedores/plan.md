# E06 · Escritorio → Compras y proveedores · Plan técnico

## 1. Modelo de datos

| Tabla | Columnas | Notas |
|---|---|---|
| `suppliers` | `Id`, `RestaurantId`, `Name`, `NormalizedName`, `Contact`, `Phone`, `Email`, `DeliveryDays`, `PaymentTerms` (`Cash` o `CreditDays` + `CreditDays`), `Rfc`, `Notes`, `IsActive` | Único `(RestaurantId, NormalizedName)` |
| `purchase_requests` | `Id`, `RestaurantId`, `IngredientId`, `RequestedQtyBase`, `RequestedQtyPurchase?`, `RequestedBy`, `RequestedAt`, `Status` (`Pending`, `Purchased`, `NotFound`, `Cancelled`), `PurchasedQtyBase`, `AmountPaid`, `NotFoundReason?`, `Note?`, `ResolvedBy`, `ResolvedAt`, `PurchaseOrderId?` | Índice único parcial `(IngredientId) WHERE Status='Pending'` (BR-COM-01) |
| `purchase_orders` | `Id`, `RestaurantId`, `Number`, `SupplierId?`, `Status` (`Draft`, `Sent`, `Partial`, `Received`, `Cancelled`), `EstimatedTotal`, `SentAt`, `SentBy`, `SentVia` | |

En esta migración se agrega la FK `ingredients.SupplierId → suppliers.Id` (pendiente desde E04).

## 2. Domain (`Resto.Domain/Purchasing/`)

| Archivo | Contenido | Reglas |
|---|---|---|
| `PurchaseRequest.cs` | `MarkPurchased(qty, paid)`, `MarkNotFound(reason, note)` y `Cancel()` | BR-COM-04 y 05 |
| `PurchaseOrderBuilder.cs` | Agrupa por proveedor y estima el costo | BR-COM-07 |
| `PurchaseOrder.cs` | Recepción parcial o total | BR-COM-08 |

## 3. Contrato que ofrece

```csharp
public interface IPurchaseRequests    // lo usan E04 y E11
{
    Task<PurchaseRequestDto> AddAsync(Guid ingredientId, decimal qtyBase, bool notify, CancellationToken ct);
    Task<int> AddManyAsync(IReadOnlyList<(Guid IngredientId, decimal QtyBase)> items, CancellationToken ct); // un solo aviso
    Task MarkPurchasedAsync(Guid requestId, decimal qtyBase, decimal? amountPaid, CancellationToken ct);
    Task MarkNotFoundAsync(Guid requestId, NotFoundReason reason, string? note, CancellationToken ct);
    Task FinishShoppingAsync(CancellationToken ct); // BR-COM-06
}
```

## 4. API (`/api/purchasing`)

| Método y ruta | Permiso |
|---|---|
| `GET /api/purchasing/suggestions` (de `IReorderQuery`) | `purchasing.request` |
| `GET /api/purchasing/requests?status=` | `purchasing.view` o `purchasing.request` (solo las propias) |
| `POST /api/purchasing/requests` `{ IngredientId, QtyBase }` y `POST …/requests/batch` | `purchasing.request` |
| `PATCH /api/purchasing/requests/{id}` (cantidad, solo pendientes) | `purchasing.request` |
| `POST /api/purchasing/requests/{id}/purchased` `{ QtyBase, AmountPaid? }` | `purchasing.buy` |
| `POST /api/purchasing/requests/{id}/not-found` · `…/cancel` | `purchasing.buy` · `purchasing.request` |
| `POST /api/purchasing/finish` | `purchasing.buy` |
| `POST /api/purchasing/notify-buyer` | `purchasing.request` |
| `GET /api/purchasing/orders/draft` · `POST /api/purchasing/orders` `{ SupplierId }` · `POST …/orders/{id}/send` · `POST …/orders/{id}/receive` `{ Lines[] }` | `purchasing.buy` |
| CRUD `/api/purchasing/suppliers` | `purchasing.suppliers` |

**Tiempo real:** `PurchaseRequestChanged` al grupo `role:compras` y al usuario que pidió.

## 5. Pantallas (`Resto.UI/Features/Purchasing/`)

| Componente | Captura |
|---|---|
| `PurchasingPage.razor` (`/compras`) con pestañas | 01 |
| `SuggestionsPanel.razor` y `PurchaseRequestTable.razor` | 01 |
| `PurchaseOrdersBoard.razor` y `PurchaseOrderCard.razor` | 02 |
| `SuppliersTable.razor` y `SupplierFormDialog.razor` | 03 |
| `MarkPurchasedDialog.razor` (comparte la lógica con E11) | — |
| `ReceivePurchaseOrderDialog.razor` | — |

## 6. Pruebas

| Proyecto | Qué cubre |
|---|---|
| Domain.Tests | BR-COM-01, 04, 05, 07 y 08; AC-COM-06 (estimado de $1,920.00) |
| Api.Tests | AC-COM-01 a 05 y 07 a 10, incluida la llamada a `IInventoryReceiving` y el número de avisos (AC-COM-03 = 1) |

## 7. Paquetes

Ninguno. El envío por WhatsApp y correo usa el mismo servicio que E03 (`IMessageDelivery`).
