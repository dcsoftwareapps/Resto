# E04 · Escritorio → Inventario · Tareas

**Cómo usar este archivo:**
- Toma una tarea cuyas dependencias estén marcadas.
- Crea la rama `E04-Txxx-nombre-corto`.
- Al terminar, marca la casilla con quién la hizo, la herramienta y la fecha. Por ejemplo: `[x] … (Nombre + Codex, 2026-11-04)`.

`[P]` = se puede hacer en paralelo con otras `[P]` del mismo bloque.

**Requisito previo de E00:** solución creada, `RestaurantId` con filtro global, `IAuditLog`, permisos y `RestaurantHub`.

## Bloque A · Dominio (sin base de datos)

- [ ] **E04-T001** [P] Enum `UnitOfMeasure` con `QuickStep` y `RoundUpForPurchase`.
  - Archivos: `src/Resto.Domain/Inventory/UnitOfMeasure.cs` y sus pruebas.
  - Cumple: BR-INV-07, BR-INV-11 (redondeo).
- [ ] **E04-T002** [P] `NameNormalizer`.
  - Cumple: BR-INV-08. Probar "Limón", " limon ", "LIMÓN" y "Limón amarillo".
- [ ] **E04-T003** [P] `WasteCalculator.ConsumptionFor`.
  - Cumple: BR-INV-02 y BR-INV-14. Prueba con los datos de AC-INV-08: 0.09 × 3 × 1.05 = **0.284**.
- [ ] **E04-T004** [P] `StockLevel` y `ExpiryRule` con días configurables.
  - Cumple: BR-INV-03, 04 y 05. Incluir los bordes: existencia = mínimo es "bajo", 0 días = solo hoy, y 5 días (AC-INV-17).
- [ ] **E04-T005** [P] `PurchaseUnit` y `ReorderSuggestion.Suggest`.
  - Cumple: BR-INV-11 y 15. Camarón 2.1 / mín. 4 → **6 kg**. Tostadas 45 / mín. 80 → **115 pz**, y con caja de 50 → **3 cajas**. Costo: $240 ÷ 150 pz → **$1.60** (AC-INV-18 y 19).
- [ ] **E04-T006** Entidad `Ingredient` con `ApplyMovement`, `InventoryMovement` y unidad de compra. Depende de T001 y T005.
  - Cumple: BR-INV-01 (piso en 0 y `Shortfall`), BR-INV-09 (antes y después) y BR-INV-12.
  - Prueba AC-INV-05: merma de 5 sobre 3.2 deja 0 con `Shortfall` de 1.8.

## Bloque B · Persistencia y API

- [ ] **E04-T007** Configuración EF de `Ingredient`, `InventoryMovement`, `IngredientCategory` e `InventoryImport` + migración `AddInventory` + datos semilla de categorías. Depende de T006.
  - Índice único por `NormalizedName`, check `stock >= 0` y `jsonb` para `CustomFields`.
- [ ] **E04-T008** `InventoryService.AdjustAsync` con bloqueo `FOR UPDATE`, auditoría, y evento `StockChanged` con `Shortfall` cuando se descuenta más de lo que hay. Depende de T007.
  - Cumple: AC-INV-05 (el aviso; la alerta de seguridad la crea E09 al escuchar el evento).
  - Prueba de concurrencia: AC-INV-14 (dos ajustes en paralelo).
  - Prueba de auditoría: AC-INV-16.
- [ ] **E04-T009** Endpoints GET de lista, resumen, detalle y movimientos con filtros. Depende de T007.
  - Cumple: BR-INV-06, AC-INV-01, 02, 03 y 12 (403 para Mesero).
- [ ] **E04-T010** Endpoints POST y PUT de alta, edición, desactivar, ajuste y ajuste rápido. Depende de T008.
  - Cumple: AC-INV-04, 06, 07 (409 por duplicado) y 13 (403 para Compras al ajustar).
- [ ] **E04-T011** `IInventoryConsumption`: consumir y devolver, con bloqueo ordenado por `Id`. Depende de T008.
  - Cumple: AC-INV-08 y 09, BR-INV-13. Esto es contrato con E02: **avisar al responsable de E02 al terminar**.
- [ ] **E04-T012** Aviso `IngredientBelowMinimum` solo al cruzar el mínimo. Depende de T008.
  - Cumple: AC-INV-10 (un solo aviso en dos ventas seguidas).
- [ ] **E04-T013** [P] `IInventoryReceiving` e `IReorderQuery` para E06 y E11. Depende de T008.
  - Al recibir una compra, convierte de unidad de compra a unidad base y actualiza `UnitCost` = pagado ÷ unidades base (AC-INV-19).
- [ ] **E04-T013b** [P] `IngredientImportRow` (validación de fila) en Domain. Depende de T002 y T005.
  - Cumple: BR-INV-16. Una prueba por cada tipo de error: unidad, número negativo, nombre vacío y nombre duplicado dentro del mismo archivo.
- [ ] **E04-T013c** Importación: plantilla, vista previa y confirmación en una sola transacción, con `ClosedXML`. Depende de T008 y T013b.
  - Cumple: AC-INV-20, 21 y 22. Prueba con un `.xlsx` de ejemplo en `tests/Resto.Api.Tests/Inventory/Fixtures/insumos-131.xlsx`.

## Bloque C · Pantallas

- [ ] **E04-T014** [P] Pasar los estilos del demo a `components.css`: tabla, indicadores, chips, barra de nivel y modal. Puede hacerse en cuanto exista E00.
  - Comparar contra `img/01`.
- [ ] **E04-T015** `InventoryPage`, `InventorySummaryCards`, `InventoryFilters` e `IngredientTable` con los estados cargando, vacío, sin resultados y error. Depende de T009 y T014.
  - Comparar contra `img/01`, `02` y `03`.
- [ ] **E04-T016** `AdjustStockDialog` y botones − y +. Depende de T010 y T015.
  - Comparar contra `img/04`. Aviso de `Shortfall` (AC-INV-05).
- [ ] **E04-T017** `IngredientFormDialog`, para alta y edición, con campos dinámicos de propiedades. Depende de T010 y T015.
  - Comparar contra `img/05`. Incluir los campos de unidad de compra. Validaciones en bUnit.
- [ ] **E04-T017b** `ImportIngredientsDialog` con plantilla, subir y vista previa. Depende de T013c y T015.
  - Comparar contra `img/10`.
  - Cumple: AC-INV-20 a 22 en la UI. Botón *Importar de Excel* en el encabezado y en el estado vacío.
- [ ] **E04-T019** `MovementHistoryPanel` (kardex paginado) al hacer clic en el nombre. Depende de T009 y T015.
  - Comparar contra `img/11`. Cumple: AC-INV-25.
- [ ] **E04-T019b** Toma de inventario: tablas, API y `StockCountPage`. Depende de T008 y T015.
  - Comparar contra `img/12`. Cumple: BR-INV-17, AC-INV-23 y 24.
- [ ] **E04-T020** Suscripción a `StockChanged`: la fila se actualiza sola. Depende de T015.
  - Cumple: AC-INV-15. Prueba manual con dos ventanas.
- [ ] **E04-T021** Permisos en la UI: esconder el menú y las acciones. Depende de T015.
  - Cumple: AC-INV-12 y 13. Comparar contra `img/09`.

## Bloque D · Cierre

- [ ] **E04-T022** Recorrido manual de AC-INV-01 a 25 (AC-INV-11 se prueba al terminar E06) en Windows (MAUI) y web, en modo claro y oscuro. Anotar aquí lo que falle.
- [x] **E04-T023** Contestar las preguntas abiertas P-01 a P-04 de la spec. (Benji, 2026-10-30: todas sí)
