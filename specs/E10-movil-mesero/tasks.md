# E10 · Móvil → Mesero · Tareas

Formato en [`docs/proceso.md`](../../docs/proceso.md). Requisitos: E00 (PIN, *push* y cola sin conexión), E02-T014 y E05-T005.

- [ ] **E10-T001** Endpoint `POST /api/orders/submit` en E02 con errores `TableTaken` y `SoldOut`.
  - Cumple: BR-MES-03, AC-MES-05 y 10.
- [ ] **E10-T002** [P] Borrador local `waiter_drafts` con agrupación de líneas.
  - Cumple: BR-MES-02 y 04, AC-MES-04.
- [ ] **E10-T003** `WaiterTablesPage` (propias, ajenas y libres) y `GuestsSheet`. Depende de E02.
  - Comparar contra `img/01` y `02`. Cumple: BR-MES-09, BR-MES-01, AC-MES-01.
- [ ] **E10-T004** `WaiterMenuPage` con categorías y stepper. Depende de T002 y T003.
  - Comparar contra `img/03`. Cumple: AC-MES-02.
- [ ] **E10-T005** `WaiterOrderPage` con notas rápidas, extras, enviar y agregar más. Depende de T001 y T004.
  - Comparar contra `img/04` y `05`. Cumple: BR-MES-05, AC-MES-03 y 05.
- [ ] **E10-T006** Envío sin conexión con la cola de E00. Depende de T005.
  - Cumple: AC-MES-06.
- [ ] **E10-T007** Pedir la cuenta. Depende de T005.
  - Cumple: BR-MES-06, AC-MES-08.
- [ ] **E10-T008** Avisos "ya salió" (banner y *push*) solo para el mesero de la orden. Depende de E12.
  - Comparar contra `img/06`. Cumple: BR-MES-07, AC-MES-07.
- [ ] **E10-T009** Tarjeta de pedidos del QR y *Confirmar*. Depende de E18.
  - Comparar contra `img/07`. Cumple: BR-MES-08, AC-MES-09.
- [ ] **E10-T010** Recorrido manual en un Android real (y en iOS si ya hay Mac) y contestar P-01 y P-02.
