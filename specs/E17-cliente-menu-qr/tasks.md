# E17 · Cliente → Menú QR · Tareas

Formato en [`docs/proceso.md`](../../docs/proceso.md). Requisitos: E05-T005 (menú con traducciones), E13 y E18-T002 (`IIncomingOrders`).

- [ ] **E17-T001** `table_qr_tokens` + regenerar + PDF de QR por mesa (`QRCoder` + QuestPDF).
  - Comparar contra `img/06`. Cumple: BR-QR-01, AC-QR-07.
- [ ] **E17-T002** Página SSR `/m/{token}` con menú, idioma y promociones. Depende de T001.
  - Comparar contra `img/01` y `02`. Cumple: BR-QR-02, 03 y 08, AC-QR-01 a 03.
- [ ] **E17-T003** Recursos ES y EN de la interfaz del cliente. Depende de T002.
- [ ] **E17-T004** Componentes compartidos `CustomerMenu/*` (los usan E18 y E19). Depende de T002.
  - Incluye `AllergenBadges` con los alérgenos y calorías de E05 (BR-REC-13).
- [ ] **E17-T005** Carrito y envío a `IIncomingOrders` con límites. Depende de T004.
  - Comparar contra `img/03`. Cumple: BR-QR-04 y 05, AC-QR-04.
- [ ] **E17-T006** Página de estado + pedir la cuenta. Depende de T005.
  - Comparar contra `img/04`. Cumple: BR-QR-06 y 07, AC-QR-05 y 06.
- [ ] **E17-T007** Modo sin pedidos (canal en pausa o QR solo para ver). Depende de T002.
  - Comparar contra `img/05`. Cumple: AC-QR-08.
- [ ] **E17-T008** Prueba de rendimiento en móvil, recorrido con QR impreso y contestar P-01 a P-03.
