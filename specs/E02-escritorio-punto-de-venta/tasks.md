# E02 · Escritorio → Punto de venta · Tareas

Formato y reglas en [`docs/proceso.md`](../../docs/proceso.md). `[P]` = se puede hacer en paralelo.

**Requisitos previos:** E00, E04, E05, E08 y E09 terminados. Las promociones (E13) llegan después por `IPromotionEngine`, que E02 define con una implementación que no aplica promociones.

## Bloque A · Dominio

- [ ] **E02-T001** [P] `PriceBreakdown` y `OrderPricing.Calculate`.
  - Cumple: BR-POS-08. Pruebas exactas de AC-POS-06 ($647.76 / $103.64 / $18.60 / $770.00), AC-POS-07 ($612.67 / $98.03 / $9.30 / $720.00) y AC-POS-08 ($654.50).
- [ ] **E02-T002** [P] `SalesChannel`, `OrderStatus`, `OrderItemStatus` y `OrderEventKind`.
- [ ] **E02-T003** Entidad `Order` con `AddItem` (agrupa según BR-POS-03), `RemoveUnsent`, `SendToKitchen` y eventos de historial. Depende de T002.
  - Cumple: BR-POS-07.
- [ ] **E02-T004** `CancelItem` + `CancelPolicy`, `ApplyDiscount` / `RemoveDiscount`, `RegisterBillPrint`, `MoveToTable`, `TransferTo`, `RequestBill`, y cierre (BR-POS-16). Depende de T003.

## Bloque B · Persistencia y API

- [ ] **E02-T005** Configuración EF + migración `AddOrders` (incluye el índice único parcial por mesa abierta) + seed con las 4 órdenes abiertas del demo. Depende de T004.
- [ ] **E02-T006** Endpoints de abrir, agregar, quitar, editar y consultar, con `[OfflineCapable]` donde aplica. Depende de T005.
  - Cumple: BR-POS-15, AC-POS-01 a 04 y 17.
- [ ] **E02-T007** Enviar a cocina: `IRecipeResolver` → `IInventoryConsumption` → `OrderSentToKitchen` → `IPrintService`. Si inventario falla, la comanda sale igual. Depende de T006.
  - Cumple: BR-POS-04, AC-POS-05.
- [ ] **E02-T008** Disponibilidad desde `IMenuCatalog` en la consulta del menú. Depende de T006.
  - Cumple: BR-POS-05, AC-POS-13.
- [ ] **E02-T009** Cancelar, con *override*, devolución de inventario y alerta. Depende de T006 y de `IManagerOverride` (E09).
  - Cumple: AC-POS-10 y 11.
- [ ] **E02-T010** Descuento manual con *override* y alerta. Depende de T009.
  - Cumple: AC-POS-08 y 09.
- [ ] **E02-T011** Imprimir y reimprimir cuenta con límite configurable. Depende de T009.
  - Cumple: AC-POS-12.
- [ ] **E02-T012** [P] Cambiar de mesa y transferir mesero. Depende de T006.
  - Cumple: AC-POS-15.
- [ ] **E02-T012b** Cancelar orden completa con *override*, inventario por línea, alerta y aviso a cocina. Depende de T009.
  - Cumple: BR-POS-17, AC-POS-18.
- [ ] **E02-T012c** `Order.MergeFrom` y `Order.SplitTo` + endpoints. Depende de T006.
  - Cumple: BR-POS-18 y 19, AC-POS-19 y 20.
- [ ] **E02-T013** `OrderChanged` y prueba de concurrencia de AC-POS-16. Depende de T006.
  - Cumple: BR-POS-13.
- [ ] **E02-T014** Exponer `IOrderService` e `IOrderQuery` para E01, E03, E10, E12 y E18. Depende de T006.

## Bloque C · Pantallas

- [ ] **E02-T015** `PosPage`, `OpenOrdersList` y `MenuGrid` con chips, agotado y quedan N. Depende de T006 y T008.
  - Comparar contra `img/01`.
- [ ] **E02-T016** `TicketPanel`, `TicketLine` y `PriceBreakdownView`. Depende de T015.
  - Comparar contra `img/01` y `06`.
- [ ] **E02-T016b** Ejemplo en vivo de la cuenta en *Configuración → Impuestos* (la pestaña es de E08) calculado con `OrderPricing`. Depende de T001 y E08-T009.
  - Comparar contra `E08 img/01`. Cumple: BR-CFG-02.
- [ ] **E02-T017** `OrderHistory`. Depende de T016.
  - Comparar contra `img/02`.
- [ ] **E02-T018** `NewOrderDialog` con los 3 canales. Depende de T015.
  - Comparar contra las imágenes de E01. Cumple: AC-POS-03.
- [ ] **E02-T019** `CancelItemDialog` y `ManagerPinDialog` compartido. Depende de T009 y T016.
  - Comparar contra `img/03` y `05`.
- [ ] **E02-T020** `DiscountDialog` con vista previa. Depende de T010 y T016.
  - Comparar contra `img/04`.
- [ ] **E02-T021** `MoveTableDialog`, `TransferWaiterDialog`, `ItemNoteEditor`, `VoidOrderDialog`, `MergeOrdersDialog` y `SplitOrderDialog`. Depende de T012, T012b, T012c y T016.
  - Comparar contra `img/07` a `10`.
- [ ] **E02-T022** Suscripción a `OrderChanged` y `ItemsDispatched`: los estados se actualizan solos. Depende de T016.

## Bloque D · Cierre

- [ ] **E02-T023** Recorrido manual de AC-POS-01 a 20, incluido sin conexión (abrir, agregar y enviar).
- [ ] **E02-T024** Contestar P-01 a P-03. **P-01 es fiscal:** confirmarla con el contador antes de facturar (E14).
