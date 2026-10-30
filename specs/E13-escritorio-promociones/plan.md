# E13 · Escritorio → Promociones · Plan técnico

## 1. Modelo de datos

| Tabla | Columnas |
|---|---|
| `promotions` | `Id`, `RestaurantId`, `Name`, `Kind` (`PercentCategory`, `PercentProducts`, `BuyNPayM`), `Percent?`, `N?`, `M?`, `CategoryId?`, `DaysMask` (bits D…S), `StartTime`, `EndTime`, `ValidFrom?`, `ValidTo?`, `ChannelsMask?`, `IsActive` |
| `promotion_targets` | `PromotionId`, `MenuItemId` |
| `order_item_discounts` (nueva, la escribe E02) | `OrderItemId`, `PromotionId?` (`null` = manual), `Amount` |

## 2. Domain (`Resto.Domain/Promotions/`)

| Archivo | Contenido | Reglas |
|---|---|---|
| `Promotion.cs` | `IsActiveAt(DateTimeOffset local, SalesChannel)` | BR-PRO-01 y 07 |
| `PromotionEngine.cs` | Implementa `IPromotionEngine` | BR-PRO-02 a 06 |

```csharp
public interface IPromotionEngine   // la usa OrderPricing de E02
{
    // Cada línea trae su AddedAtLocal, MenuItemId, CategoryId, UnitPriceWithExtras, Qty y IsBundle.
    IReadOnlyList<LineDiscount> Apply(IReadOnlyList<PricingLine> lines, SalesChannel channel);
}
public sealed record LineDiscount(Guid LineId, Guid PromotionId, string Name, decimal Amount);
```

`OrderPricing` (E02) llama a `Apply` antes del descuento manual, y presenta los descuentos agrupados por nombre en el desglose.

## 3. API (`/api/promotions`)

| Método y ruta | Permiso |
|---|---|
| `GET /api/promotions` (con estado vigente ahora) | `promos.manage` |
| `POST`, `PUT /{id}`, `DELETE /{id}` (borrado lógico) | `promos.manage` |
| `POST /{id}/toggle` | `promos.manage` |
| `GET /api/promotions/active-now?channel=` (etiquetas para el menú) | sesión o público (QR y kiosko) |

**Tiempo real:** `PromotionsChanged`. Además, el menú recalcula las etiquetas cada minuto, porque una promo puede empezar sin que nadie toque nada.

## 4. Pantallas (`Resto.UI/Features/Promotions/`)

| Componente | Captura |
|---|---|
| `PromotionsPage.razor` (`/promociones`) + `PromotionCard.razor` + `BundlesPanel.razor` | 01 |
| `PromotionFormDialog.razor` | 02 |
| Etiqueta de promo en `MenuItemTile` (E02), `MenuRow` (E10) y menú del cliente (E17 y E19) | 03 |

## 5. Pruebas

| Proyecto | Qué cubre |
|---|---|
| Domain.Tests | AC-PRO-01 a 08 y 10 con reloj fijo (`IClock`); casos de medianoche y vigencia |
| Api.Tests | AC-PRO-09; `order_item_discounts` guardados al cobrar |

## 6. Paquetes

Ninguno.
