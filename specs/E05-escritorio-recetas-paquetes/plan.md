# E05 · Escritorio → Recetas y paquetes · Plan técnico

## 1. Modelo de datos

| Tabla | Columnas | Notas |
|---|---|---|
| `menu_categories` | `Id`, `RestaurantId`, `Name`, `NameEn`, `SortOrder`, `IsActive` | |
| `menu_items` | `Id`, `RestaurantId`, `CategoryId`, `Kind` (`Dish`, `Bundle`), `Name`, `NameEn`, `Description`, `DescriptionEn`, `Price` (`numeric(12,2)`), `KitchenStationId`, `PhotoUrl`, `Allergens` (flags), `Calories?`, `Channels` (flags), `SoldOutBusinessDay` (`date?`), `IsActive` | `SoldOutBusinessDay = hoy` significa "Agotado hoy" (BR-REC-07) |
| `recipe_lines` | `Id`, `MenuItemId`, `IngredientId`, `Quantity` (`numeric(12,3)` > 0) | Único `(MenuItemId, IngredientId)` |
| `bundle_components` | `BundleId`, `ComponentId`, `Quantity` (`int` ≥ 1) | `ComponentId` debe ser `Dish` |
| `extras` | `Id`, `RestaurantId`, `Name`, `NameEn`, `Price`, `IngredientId`, `Quantity`, `IsActive` | |
| `extra_targets` | `ExtraId`, `CategoryId?`, `MenuItemId?` | Uno de los dos |
| `menu_imports` | `Id`, `RestaurantId`, `FileName`, `UploadedBy`, `UploadedAt`, `Status`, `UpdateExisting`, `CreatedCount`, `UpdatedCount`, `ErrorCount`, `PreviewJson` (`jsonb`) | Igual que `inventory_imports` de E04 (BR-REC-12) |

Las fotos se guardan en almacenamiento de archivos (Azure Blob o similar, según E00 P-01). En la tabla solo va la URL.

## 2. Domain (`Resto.Domain/Menu/`)

| Archivo | Contenido | Reglas |
|---|---|---|
| `MenuItem.cs` | Validaciones, receta, componentes y agotado hoy | BR-REC-04, 07 y 08 |
| `RecipeCosting.cs` | `Cost(item, ingredients, wasteOf)` y `Margin(price, cost, taxRates)` | BR-REC-01, 02 |
| `PortionCalculator.cs` | `Portions(item, stockOf, wasteOf) → int?` (`null` = sin receta) | BR-REC-03 |
| `RecipeResolver.cs` | Implementa `IRecipeResolver` | BR-REC-10 |

## 2.1 Contratos que ofrece

```csharp
public interface IRecipeResolver   // lo usa E02 al enviar a cocina
{
    IReadOnlyList<ConsumptionLine> Resolve(Guid menuItemId, int units, IReadOnlyList<Guid> extraIds);
}

public interface IMenuCatalog      // lo usan E02, E10, E13, E17 y E19
{
    Task<IReadOnlyList<MenuEntryDto>> GetMenuAsync(SalesChannel channel, string lang, CancellationToken ct);
    // MenuEntryDto: Id, Name, Description, Category, Price, Kind, Station, PhotoUrl,
    //               Portions (int?), IsSoldOut, Extras[], Components[], SeparatePrice
}
```

`GetMenuAsync` se cachea por restaurante y se invalida con `StockChanged` (E04) y con cualquier cambio en el menú. Las porciones se recalculan solo para los platillos que usan el insumo que cambió.

## 3. API (`/api/menu`)

| Método y ruta | Permiso |
|---|---|
| `GET /api/menu?channel=&lang=` | sesión (o público para QR y kiosko con token, lo resuelven E17 y E19) |
| `GET /api/menu/items/{id}` (con costo, margen y porciones) | `menu.view` |
| `POST /api/menu/items`, `PUT /api/menu/items/{id}`, `POST …/deactivate` | `menu.edit` |
| `PUT /api/menu/items/{id}/recipe` `{ Lines[] }` | `menu.edit` |
| `PUT /api/menu/items/{id}/components` `{ Components[] }` | `menu.edit` |
| `POST /api/menu/items/{id}/sold-out-today` `{ Value }` | `menu.edit` o `kitchen.manage` |
| `POST /api/menu/items/{id}/photo` (multipart) | `menu.edit` |
| `POST /api/menu/translate` `{ Texts[] }` → sugerencias EN (BR-REC-11) | `menu.edit` |
| `GET /api/menu/import/template` · `POST /api/menu/import/preview` · `POST /api/menu/import/{id}/commit` (BR-REC-12, mismo patrón y tabla `menu_imports` que E04) | `menu.edit` |
| CRUD `/api/menu/categories` y `/api/menu/extras` | `menu.edit` |

## 4. Pantallas (`Resto.UI/Features/Menu/`)

| Componente | Captura |
|---|---|
| `RecipesPage.razor` (`/recetas`) + `MenuItemList.razor` | 01 |
| `RecipeEditor.razor` (indicadores + tabla + extras) | 01, 02 |
| `BundleEditor.razor` | 03 |
| `MenuItemFormDialog.razor` (foto, alérgenos, calorías y *Sugerir traducción*) | 04 |
| `MenuCategoriesPage.razor` | — |
| `ExtrasPage.razor` | 05 |
| `ImportMenuDialog.razor` | 06 |
| `AllergenPicker.razor` (lo reutilizan E17 y E19 para mostrar) | 04 |

## 5. Pruebas

| Proyecto | Qué cubre |
|---|---|
| Domain.Tests | AC-REC-01 a 07 exactos y `IRecipeResolver` con paquetes y extras |
| Api.Tests | AC-REC-08 a 11; caché invalidada por `StockChanged` |
| UI.Tests | Avisos de margen y "sin ahorro" |

## 6. Paquetes

Ninguno nuevo: la importación usa `ClosedXML` (E04). La traducción usa el cliente del servicio elegido en P-03 detrás de una interfaz `ITranslationService` interna. El almacenamiento de fotos depende de E00 P-01.
