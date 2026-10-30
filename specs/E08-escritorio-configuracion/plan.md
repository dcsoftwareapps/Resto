# E08 · Escritorio → Configuración · Plan técnico

## 1. Modelo de datos

| Tabla | Columnas | Notas |
|---|---|---|
| `restaurant_settings` | `RestaurantId` (PK), `PricesIncludeTax`, `DefaultWastePercent`, `DefaultOpeningFloat`, `ExpiryWarningDays`, `MinMarginPercent`, `KitchenWarnMinutes`, `KitchenLateMinutes`, `RegisterIdleMinutes`, `PhoneIdleMinutes`, `PrintKitchenTickets`, `ShowKitchenDisplay`, `ReceiptPrinterId` | Una fila por restaurante, creada con valores por defecto |
| `tax_rules` | `Id`, `RestaurantId`, `Name`, `Rate` (`numeric(7,4)`), `Scope` (`Bill`, `Products`), `IsActive` | |
| `tax_rule_products` | `TaxRuleId`, `MenuItemId` | |
| `custom_field_definitions` | `Id`, `RestaurantId`, `Key` (slug inmutable), `Name`, `Type` (`Text`, `Number`, `Date`), `IsVisible`, `SortOrder` | |
| `kitchen_stations` | `Id`, `RestaurantId`, `Name`, `PrinterId?`, `IsActive`, `SortOrder` | |
| `printers` | `Id`, `RestaurantId`, `Name`, `Connection` (`Network`, `Usb`), `Host`, `Port`, `UsbName`, `UsbDeviceId` (dispositivo de E00), `PaperWidthMm`, `HasCashDrawer` | |
| `ingredient_categories` | (definida en E04, se edita aquí) | |

Zona horaria, día de operación y nombre viven en `restaurants` (E00) y se editan aquí.

## 2. Contratos que ofrece

```csharp
public interface IRestaurantSettings   // E01, E04, E05, E12
{
    decimal DefaultWastePercent { get; }
    int ExpiryWarningDays { get; }
    decimal MinMarginPercent { get; }
    int KitchenWarnMinutes { get; }
    int KitchenLateMinutes { get; }
    decimal DefaultOpeningFloat { get; }
    bool PrintKitchenTickets { get; }
    bool ShowKitchenDisplay { get; }
    IReadOnlyList<CustomFieldDefinition> InventoryCustomFields { get; }
    IReadOnlyList<KitchenStationInfo> Stations { get; }
}

public interface IPricingSettings      // E02, E03, E13, E14
{
    bool PricesIncludeTax { get; }
    IReadOnlyList<TaxRuleInfo> ActiveTaxRules { get; } // Name, Rate, Scope, ProductIds
}
```

- Las dos se implementan con una caché por restaurante que se invalida con `SettingsChanged`.
- **Reemplazan** las implementaciones provisionales `DefaultRestaurantSettings` y `DefaultPricingSettings` que usaron E02 y E04 mientras tanto.

## 3. API (`/api/settings`)

| Método y ruta | Permiso |
|---|---|
| `GET /api/settings` (todo junto para las pestañas) | `settings.manage` |
| `PUT /api/settings/general` | `settings.manage` |
| CRUD `/api/settings/taxes`, `PUT …/taxes/{id}/products` | `settings.manage` |
| `GET /api/settings/taxes/example` (BR-CFG-03) | `settings.manage` |
| `PUT /api/settings/waste` `{ DefaultPercent }` · `PUT /api/inventory/ingredients/{id}/waste` `{ Percent? }` | `settings.manage` |
| CRUD `/api/settings/custom-fields` | `settings.manage` |
| CRUD `/api/settings/stations`, CRUD `/api/settings/printers`, `POST …/printers/{id}/test` | `settings.manage` |
| CRUD `/api/settings/ingredient-categories` | `settings.manage` |

**Prueba de impresión:**
- *Red:* la hace el servidor si está en la misma red. Si no lo está (servidor en la nube), la hace el dispositivo: el endpoint emite `PrintTestRequested` al dispositivo de caja y este imprime con `Resto.Printing` (E12).
- *USB:* siempre la hace el dispositivo al que está conectada.

## 4. Pantallas (`Resto.UI/Features/Settings/`)

| Componente | Captura |
|---|---|
| `SettingsPage.razor` (`/configuracion`) con pestañas | — |
| `TaxesTab.razor` + `TaxRuleCard.razor` + `TaxExample.razor` | 01 |
| `WasteTab.razor` | 02 |
| `CustomFieldsTab.razor` (con confirmación en la misma fila) | 03 |
| `KitchenPrintingTab.razor` + `PrinterFormDialog.razor` | 04 |
| `RestaurantTab.razor` | 05 |

## 5. Pruebas

| Proyecto | Qué cubre |
|---|---|
| Domain.Tests | Validaciones BR-CFG-01, 04 a 06 y 08 |
| Api.Tests | AC-CFG-01 y 02 (con `OrderPricing` real), 03 a 07, 09 y 10; invalidación de caché con `SettingsChanged` |
| Manual | AC-CFG-08 con impresora real |

## 6. Paquetes

Ninguno.
