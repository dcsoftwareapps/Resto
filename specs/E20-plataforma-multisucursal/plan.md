# E20 · Plataforma → Multisucursal · Plan técnico

## 1. Modelo de datos

| Cambio | Detalle |
|---|---|
| `user_branch_roles` (nueva) | `UserId`, `RestaurantId`, `RoleId`, `PinHash`. Reemplaza `users.RoleId` y `users.PinHash` (migración que copia los valores actuales). |
| `menu_items.Scope` | `Organization` o `Branch`. Los de organización llevan `RestaurantId = null` y el filtro global se ajusta: `RestaurantId == actual OR (Scope == Organization AND OrganizationId == actual)`. |
| `menu_item_branch_overrides` (nueva) | `MenuItemId`, `RestaurantId`, `Price?`, `IsAvailable?`, `KitchenStationId?`, `OverridesRecipe` |
| `storage_locations` (nueva) | `Id`, `RestaurantId`, `Name`, `IsDefault` |
| `ingredient_stocks` (nueva) | `IngredientId`, `StorageLocationId`, `Stock` |
| `ingredients.Stock` | Pasa a ser la suma, mantenida en la misma transacción |
| `kitchen_stations.ConsumptionLocationId` | Almacén de consumo |
| `inventory_movements.StorageLocationId` | Y nuevos tipos `TransferOut`, `TransferIn` y `TransferLoss` |
| `stock_transfers` (nueva) | `Id`, `Number`, `FromRestaurantId`, `FromLocationId`, `ToRestaurantId`, `ToLocationId`, `Status` (`InTransit`, `Received`, `Cancelled`), `SentBy`, `SentAt`, `ReceivedBy`, `ReceivedAt` |
| `stock_transfer_lines` (nueva) | `TransferId`, `IngredientId`, `QtySent`, `QtyReceived`, `UnitCost` |

**Migración de almacenes:** el valor de la propiedad de texto "Almacén" de cada insumo (E04) crea las `storage_locations` y asigna ahí la existencia actual. La propiedad se desactiva.

## 2. Sesión

- El JWT lleva `rest` (la sucursal actual) y la lista de sucursales permitidas.
- `POST /api/auth/switch-branch { RestaurantId }` emite un token nuevo con los permisos del rol en esa sucursal.
- Los dispositivos registrados no pueden cambiar de sucursal (BR-PLT-02).

## 3. API

| Método y ruta | Permiso |
|---|---|
| `GET /api/org/dashboard?from=&to=` | `branches.manage` |
| `POST /api/org/branches` `{ Name, Address, TimeZone, CopyFromId? }` | `branches.manage` |
| `POST /api/auth/switch-branch` | sesión |
| `PUT /api/menu/items/{id}/branch-override` | `menu.edit` |
| CRUD `/api/inventory/locations` · `GET /api/inventory/ingredients/{id}/stocks` | `inventory.adjust` |
| `POST /api/inventory/transfers` · `POST …/transfers/{id}/receive` · `…/cancel` · `GET …/transfers` | `inventory.adjust` |
| Los reportes de E15 aceptan `branchId=all` | `reports.view` + `branches.manage` |

## 4. Pantallas

| Componente |
|---|
| `BranchSwitcher.razor` (en `TopBar` de E00) |
| `OrganizationPage.razor` (`/organizacion`) + `BranchCard.razor` + `NewBranchDialog.razor` |
| Selector de alcance en `RecipesPage` (E05) + `BranchOverridePanel.razor` |
| `StorageLocationsTab.razor`, `TransfersPage.razor` y `TransferFormDialog.razor` (en Inventario) |
| Columna de rol por sucursal en `UsersTable` (E07) |

## 5. Pruebas

| Proyecto | Qué cubre |
|---|---|
| Api.Tests | AC-SUC-02 a 07; **prueba de aislamiento de E00 extendida** para que un usuario con 2 sucursales no vea datos de la otra sin cambiar de sucursal; la migración de almacenes con datos del demo |
| Domain.Tests | Traspaso con merma (AC-SUC-04) y sobrescritura de precio |

## 6. Paquetes

Ninguno.
