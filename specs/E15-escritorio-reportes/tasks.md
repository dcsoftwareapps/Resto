# E15 · Escritorio → Reportes · Tareas

Formato en [`docs/proceso.md`](../../docs/proceso.md). Requisitos: E03 completo y E05-T001.

- [ ] **E15-T001** Guardar `UnitCostSnapshot` al enviar a cocina (cambio en E02).
  - Cumple: BR-REP-03, AC-REP-03.
- [ ] **E15-T002** Tablas de resumen + `DailySummaryJob` + regenerar día. Depende de T001.
  - Cumple: BR-REP-01 y 06.
- [ ] **E15-T003** `IReport` + tablero (indicadores con variación, por día, por hora, por canal, por mesero y platillos). Depende de T002.
  - Cumple: BR-REP-02 y 04, AC-REP-01 y 02.
- [ ] **E15-T004** Exportación a Excel y PDF con auditoría. Depende de T003.
  - Cumple: BR-REP-07, AC-REP-05.
- [ ] **E15-T005** [P] Reportes de ventas: día, hora, canal, mesero, platillos y costo-utilidad. Depende de T003.
- [ ] **E15-T006** [P] Reportes de control: descuentos, promociones, cancelaciones, propinas y cortes. Depende de T003.
- [ ] **E15-T007** [P] Reportes de inventario: consumo teórico contra real, movimientos, mermas y compras por proveedor. Depende de T003.
  - Cumple: BR-REP-05, AC-REP-04.
- [ ] **E15-T008** [P] Reporte de facturas. Depende de T003 y E14.
- [ ] **E15-T009** `ReportsDashboardPage`, `PeriodPicker`, `KpiCard`, `BarChart` y `HBarChart`, responsivos. Depende de T003.
  - Comparar contra `img/01`. Cumple: AC-REP-08.
- [ ] **E15-T010** `ReportPage` genérico. Depende de T004.
  - Comparar contra `img/02`.
- [ ] **E15-T011** Prueba de rendimiento con 180 días (AC-REP-06), recorrido manual y P-01.
