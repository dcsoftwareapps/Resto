# E08 · Escritorio → Configuración · Tareas

Formato en [`docs/proceso.md`](../../docs/proceso.md). Requisitos: E00 terminado. Va antes que E04, porque E04 lee la merma y las propiedades de aquí.

- [ ] **E08-T001** Tablas propias (sin `tax_rule_products`, que agrega E05-T006b, ni referencias a impresoras, que agrega E12-T004b) + migración `AddSettings` + valores por defecto al crear un restaurante + seed del demo (definiciones de IVA e IEPS cerveza sin asociaciones a productos hasta E05-T006b, 3 estaciones; las 3 impresoras se crean en E12-T004b).
- [ ] **E08-T001b** `DiningTable` (`dining_tables`) + configuración EF + migración + seed de 12 mesas (antes en E01). Depende de T001.
- [ ] **E08-T002** `IRestaurantSettings` e `IPricingSettings` con caché y `SettingsChanged`. Implementaciones reales desde el inicio, sin proveedores provisionales. Depende de T001.
  - Cumple: BR-CFG-09.
- [ ] **E08-T003** [P] Validaciones de Domain.
  - Cumple: BR-CFG-01, 04 a 06 y 08.
- [ ] **E08-T004** API de impuestos. Depende de T002.
  - Verifica configuración y contrato: BR-CFG-01/02 y AC-CFG-03. Selección de productos: E05-T006b; ejemplo/recálculo AC-CFG-01/02: E02-T016b.
- [ ] **E08-T005** API de merma general. La API y tabla de merma por insumo son E04-T010b. Depende de T002.
  - Verifica persistencia/auditoría de AC-CFG-04; aplicación al insumo y AC-CFG-05: E04-T010b.
- [ ] **E08-T006** [P] API de definiciones de propiedades extra, visibilidad y confirmación de eliminación. No accede a valores de insumos: E04-T010b integra su eliminación real. Depende de T001.
  - Verifica definiciones; AC-CFG-06 completo queda en E04-T010b.
- [ ] **E08-T007** API de estaciones de cocina. Depende de T001.
  - Verifica al menos una estación activa; E05-T006b integra la salvaguarda por platillos (AC-CFG-07). Impresoras y prueba: E12-T004b.
- [ ] **E08-T008** API general (mesas, fondo, tiempos y horarios). Categorías: entidad/seed E04-T007 y API/editor E04-T007b. Depende de T001b.
  - Verifica configuración de mesas; salvaguarda con órdenes abiertas de AC-CFG-10: E02-T006b.
- [ ] **E08-T009** `SettingsPage` + `TaxesTab` (configuración propia; productos E05-T006b y ejemplo E02-T016b). Depende de T004.
  - Comparar contra `img/01`.
- [ ] **E08-T010** `WasteTab` (general) y `CustomFieldsTab` (definiciones). Integración por insumo: E04-T010b. Depende de T005 y T006.
  - Comparar contra `img/02` y `03`.
- [ ] **E08-T011** `KitchenPrintingTab` (estaciones) y `RestaurantTab`. Depende de T007 y T008.
  - Comparar contra `img/04` y `05`.
- [ ] **E08-T012** Recorrido de capacidades propias y contratos de AC-CFG-01 a 10 según spec §6.1; registrar escenarios de extremo a extremo pendientes con su dueño, sin acreditarlos. Contestar P-01 (con el contador) y P-02.
