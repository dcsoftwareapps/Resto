# E14 · Escritorio → Facturación · Tareas

Formato en [`docs/proceso.md`](../../docs/proceso.md). Requisitos: E03-T013 (`IPaidOrderQuery`). **Antes de T009 hay que tener contestadas P-01 (PAC) y P-04 (contador).**

- [ ] **E14-T001** [P] `RfcValidator` y `PaymentFormResolver`.
  - Cumple: BR-FAC-02 y 04, AC-FAC-02.
- [ ] **E14-T002** `CfdiDocument` + `CfdiBuilder`, que cuadra al centavo con `OrderPricing`.
  - Cumple: BR-FAC-03, AC-FAC-10.
- [ ] **E14-T003** `GlobalInvoiceBuilder`. Depende de T002.
  - Cumple: BR-FAC-05.
- [ ] **E14-T004** Tablas + migración + cifrado de secretos.
  - Cumple: BR-FAC-09.
- [ ] **E14-T005** `ICfdiProvider` + `FakeCfdiProvider`. Depende de T002.
- [ ] **E14-T006** `InvoicingService`: individual, global, cancelar y clientes frecuentes. Depende de T003 a T005.
  - Cumple: BR-FAC-01, 06, 08 y 11, AC-FAC-01, 03 a 05 y 08.
- [ ] **E14-T007** API interna + PDF + envío por correo. Depende de T006.
  - Cumple: BR-FAC-10, AC-FAC-09.
- [ ] **E14-T008** Portal público con *rate limit*. Depende de T006.
  - Cumple: BR-FAC-07, AC-FAC-06 y 07.
- [ ] **E14-T009** Adaptador del PAC real y pruebas en su ambiente de pruebas. Depende de T005 y P-01.
- [ ] **E14-T010** `InvoicingPage`, `InvoiceFormDialog` y `GlobalInvoiceDialog`. Depende de T007.
  - Comparar contra `img/01`, `02`, `04` y `05`.
- [ ] **E14-T011** `PublicInvoicingPage` y `PortalPreview`. Depende de T008.
  - Comparar contra `img/03`.
- [ ] **E14-T012** `InvoiceSettingsTab` con carga de sellos. Depende de T004.
  - Comparar contra `img/06`.
- [ ] **E14-T012b** `IPersonalDataEraser` para `fiscal_customers`. Depende de E00-T024e.
  - Cumple: E00 BR-PLT-16, AC-PLT-19.
- [ ] **E14-T013** QR del ticket (E12) apuntando al portal. Depende de T008.
- [ ] **E14-T014** Revisión con el contador de todas las reglas 🧾 y de un XML de muestra de cada tipo. Contestar P-02 a P-04.
