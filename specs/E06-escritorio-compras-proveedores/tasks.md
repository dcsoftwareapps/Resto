# E06 · Escritorio → Compras y proveedores · Tareas

Formato en [`docs/proceso.md`](../../docs/proceso.md). Requisitos: E00 y E04-T013 (`IInventoryReceiving` e `IReorderQuery`).

- [ ] **E06-T001** [P] `PurchaseRequest`, `PurchaseOrderBuilder` y `PurchaseOrder` en Domain.
  - Cumple: BR-COM-04, 05, 07 y 08, AC-COM-06.
- [ ] **E06-T002** Tablas + migración `AddPurchasing` (incluye la FK `ingredients.SupplierId`) + seed con los 10 proveedores del demo. Depende de T001.
- [ ] **E06-T003** `IPurchaseRequests` con avisos (uno por lote) y `PurchaseRequestChanged`. Depende de T002.
  - Cumple: BR-COM-10, BR-COM-01 a 06, AC-COM-01 a 05.
- [ ] **E06-T004** Endpoints de sugeridos y solicitudes. Depende de T003.
  - Cumple: AC-COM-10.
- [ ] **E06-T005** Órdenes de compra: armar, enviar (`IMessageDelivery`) y recibir. Depende de T003.
  - Cumple: AC-COM-07 y 08.
- [ ] **E06-T006** [P] CRUD de proveedores. Depende de T002.
  - Cumple: BR-COM-09, AC-COM-09.
- [ ] **E06-T007** `PurchasingPage`, `SuggestionsPanel` y `PurchaseRequestTable`. Depende de T004.
  - Comparar contra `img/01`.
- [ ] **E06-T008** `PurchaseOrdersBoard` y `ReceivePurchaseOrderDialog`. Depende de T005.
  - Comparar contra `img/02` y `05`.
- [ ] **E06-T009** `SuppliersTable` y `SupplierFormDialog`. Depende de T006.
  - Comparar contra `img/03` y `04`.
- [ ] **E06-T010** Conectar el botón "A compras" de E04 (E04-T018) con `IPurchaseRequests`.
- [ ] **E06-T011** Recorrido manual de AC-COM-01 a 10 y contestar P-01 y P-02.
