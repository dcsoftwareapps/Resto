# E12 · Cocina → Pantalla e impresión · Tareas

Formato en [`docs/proceso.md`](../../docs/proceso.md). Requisitos: E02 y E08 terminados.

## Bloque A · Impresión

- [ ] **E12-T001** [P] `EscPosBuilder` con CP850, QR y corte, más sus pruebas de referencia.
- [ ] **E12-T002** [P] Plantillas: comanda, cancelación, cuenta, ticket, corte y prueba. Depende de T001.
  - Comparar contra `img/04`.
- [ ] **E12-T003** [P] `NetworkPrinterTransport` y `WindowsUsbPrinterTransport`. Depende de T001.
- [ ] **E12-T004** `print_jobs` + `IPrintService` (servidor) + `PrintJobRequested`. Depende de T002.
- [ ] **E12-T004b** Impresoras: tabla `printers`, API, `PrinterFormDialog` con *Probar* y la sección de impresoras de *Configuración → Comandas e impresoras* (la pestaña es de E08). Depende de T003 y E08-T011.
  - Comparar contra `E08 img/04` y `E08 img/06`. Cumple: BR-CFG-07, AC-CFG-08 y 09.
- [ ] **E12-T005** `PrintQueueWorker` en la caja anfitriona con reintentos y aviso de falla. Depende de T003 y T004.
  - Cumple: BR-COC-10 y 11, AC-COC-07 y 08.
- [ ] **E12-T006** `LocalPrintService` sin conexión. Depende de T003 y E00-T022.
  - Cumple: AC-COC-09.

## Bloque B · Pantalla de cocina

- [ ] **E12-T007** [P] Reglas de Domain: orden, colores, filtro y promedio.
  - Cumple: BR-COC-01, BR-COC-02, 03, 05 y 09.
- [ ] **E12-T008** API de tarjetas, despachar, regresar y reimprimir + `ItemsDispatched`. Depende de T007.
  - Cumple: BR-COC-04 a 06, AC-COC-03, 04 y 06.
- [ ] **E12-T009** `KitchenDisplayPage` con filtro, indicadores, tarjetas y despachadas. Tema oscuro y *wake lock*. Depende de T008.
  - Comparar contra `img/01`, `02` y `03`. Cumple: BR-COC-12, AC-COC-01, 02 y 05.
- [ ] **E12-T010** Destello y sonido de comanda nueva + cancelaciones (quitar de la pantalla y comanda de cancelación). Depende de T009 y T004.
  - Cumple: BR-COC-07 y 08, AC-COC-10.
- [ ] **E12-T011** Recorrido manual con impresora y tablet reales, y contestar P-01 y P-02.
