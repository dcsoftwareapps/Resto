# E13 · Escritorio → Promociones · Tareas

Formato en [`docs/proceso.md`](../../docs/proceso.md). Requisitos: E02-T001 (`OrderPricing`) y E05-T004.

- [ ] **E13-T001** `Promotion.IsActiveAt` (días, horario, medianoche, vigencia y canales).
  - Cumple: BR-PRO-01 y 07, AC-PRO-07, 08 y 10.
- [ ] **E13-T002** `PromotionEngine` (NxM con la más barata, porcentaje, no acumulables, paquetes y hora en que se agregó). Depende de T001.
  - Cumple: BR-PRO-02 a 06, AC-PRO-01 a 06.
- [ ] **E13-T003** Conectar `IPromotionEngine` en `OrderPricing` (E02) + tabla `order_item_discounts`. Depende de T002.
  - Cumple: BR-PRO-08.
- [ ] **E13-T004** Tablas + migración + seed con las 3 promociones del demo. Depende de T001.
- [ ] **E13-T005** API con auditoría y `PromotionsChanged`. Depende de T004.
  - Cumple: BR-PRO-09, AC-PRO-09.
- [ ] **E13-T006** `PromotionsPage`, `PromotionCard` y `BundlesPanel`. Depende de T005.
  - Comparar contra `img/01`.
- [ ] **E13-T007** `PromotionFormDialog` con los 4 tipos, vigencia y canales. Depende de T005.
  - Comparar contra `img/02` y `04`.
- [ ] **E13-T008** Etiquetas "2×1 ahora" en caja, mesero, QR y kiosko, actualizadas cada minuto. Depende de T005.
  - Comparar contra `img/03`.
- [ ] **E13-T009** Recorrido manual y contestar P-01 a P-03.
