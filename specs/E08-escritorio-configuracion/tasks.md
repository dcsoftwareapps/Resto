# E08 · Escritorio → Configuración · Tareas

Formato en [`docs/proceso.md`](../../docs/proceso.md). Requisitos: E00 terminado. Va antes que E04, porque E04 lee la merma y las propiedades de aquí.

- [ ] **E08-T001** Tablas + migración `AddSettings` + valores por defecto al crear un restaurante + seed del demo (IVA, IEPS cerveza, 3 estaciones y 3 impresoras).
- [ ] **E08-T001b** `DiningTable` (`dining_tables`) + configuración EF + migración + seed de 12 mesas (antes en E01). Depende de T001.
- [ ] **E08-T002** `IRestaurantSettings` e `IPricingSettings` con caché y `SettingsChanged`. Reemplazar las provisionales de E02 y E04. Depende de T001.
  - Cumple: BR-CFG-09.
- [ ] **E08-T003** [P] Validaciones de Domain.
  - Cumple: BR-CFG-01, 04 a 06 y 08.
- [ ] **E08-T004** API de impuestos. Depende de T002.
  - Cumple: BR-CFG-02, AC-CFG-01 a 03.
- [ ] **E08-T005** API de merma general y por insumo. Depende de T002.
  - Cumple: AC-CFG-04 y 05.
- [ ] **E08-T006** [P] API de propiedades extra. Depende de T001.
  - Cumple: AC-CFG-06.
- [ ] **E08-T007** API de estaciones de cocina. Depende de T001.
  - Cumple: AC-CFG-07 (estaciones). Las impresoras y su prueba las hace E12-T004b.
- [ ] **E08-T008** API general (mesas, fondo, tiempos, horarios y categorías de insumo). Depende de T001b.
  - Cumple: AC-CFG-10.
- [ ] **E08-T009** `SettingsPage` + `TaxesTab`. Depende de T004.
  - Comparar contra `img/01`.
- [ ] **E08-T010** `WasteTab` y `CustomFieldsTab`. Depende de T005 y T006.
  - Comparar contra `img/02` y `03`.
- [ ] **E08-T011** `KitchenPrintingTab` (estaciones) y `RestaurantTab`. Depende de T007 y T008.
  - Comparar contra `img/04` y `05`.
- [ ] **E08-T012** Recorrido manual de AC-CFG-01 a 10 y contestar P-01 (con el contador) y P-02.
