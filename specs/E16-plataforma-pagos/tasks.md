# E16 · Plataforma → Pagos con terminal · Tareas

Formato en [`docs/proceso.md`](../../docs/proceso.md). Requisitos: E03-T007. **Elegir el proveedor (P-01) antes de T006.**

- [ ] **E16-T001** Tablas + migración.
- [ ] **E16-T002** `IPaymentTerminal` + `FakePaymentTerminal`. Depende de T001.
- [ ] **E16-T003** Endpoints de cobro, estado y cancelar, más *webhook* idempotente y `TerminalPaymentUpdated`. Al aprobar, registra el pago en E03. Depende de T002.
  - Cumple: BR-PAG-07, BR-PAG-01 a 03, AC-PAG-01 a 04.
- [ ] **E16-T004** Reembolso con *override*. Depende de T003.
  - Cumple: BR-PAG-04, AC-PAG-06.
- [ ] **E16-T005** `TerminalChargePanel` en `ChargeDialog`. Depende de T003.
  - Comparar contra `img/01`, `02` y `03`. Cumple: BR-PAG-06.
- [ ] **E16-T006** Adaptador del proveedor real + vinculación de terminales. Depende de T002 y P-01.
- [ ] **E16-T007** `TerminalsSettingsTab`. Depende de T006.
  - Comparar contra `img/04`.
- [ ] **E16-T008** Importar liquidaciones + conciliación + `ReconciliationPage`. Depende de T006.
  - Comparar contra `img/05`. Cumple: BR-PAG-05, AC-PAG-05.
- [ ] **E16-T009** Recorrido con terminal real y contestar P-01 a P-03.
