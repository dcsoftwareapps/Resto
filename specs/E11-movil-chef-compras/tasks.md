# E11 · Móvil → Chef y compras · Tareas

Formato en [`docs/proceso.md`](../../docs/proceso.md). Requisitos: E00 (PIN, *push* y cola) y E06-T004.

- [ ] **E11-T001** [P] `QuantityStepper` compartido con el paso por unidad.
  - Cumple: BR-CHF-03.
- [ ] **E11-T002** `KitchenBuyerShell` con la vista según permiso.
  - Cumple: BR-CHF-05, AC-CHF-09.
- [ ] **E11-T003** `MissingTab` + selección local + *Enviar a compras*. Depende de T001 y T002.
  - Comparar contra `img/01` y `02`. Cumple: BR-CHF-01, AC-CHF-01 a 03.
- [ ] **E11-T004** [P] `SearchTab` y `MyRequestsTab`. Depende de T002.
  - Comparar contra `img/03` y `04`.
- [ ] **E11-T005** `BuyerListPage` agrupada por proveedor con unidad de compra. Depende de T002.
  - Comparar contra `img/05`. Cumple: BR-CHF-07, AC-CHF-04.
- [ ] **E11-T006** `MarkPurchasedSheet` con comprado y no encontrado, más *Terminé las compras*. Depende de T005.
  - Comparar contra `img/06` y `07`. Cumple: BR-CHF-04, AC-CHF-05 a 07.
- [ ] **E11-T007** Marcar los endpoints de E06 como `[OfflineCapable]` + estado "pendiente de enviar". Depende de T006.
  - Cumple: BR-CHF-06, AC-CHF-08.
- [ ] **E11-T007b** `CountTab`: toma abierta, conteo rápido y entrada rápida, con cola sin conexión. Depende de E04-T019b.
  - Comparar contra `img/08`. Cumple: BR-CHF-08, AC-CHF-10 a 12.
- [ ] **E11-T008** Recorrido manual en celular real y contestar P-01.
