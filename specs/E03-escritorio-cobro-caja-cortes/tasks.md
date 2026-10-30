# E03 · Escritorio → Cobro, caja y cortes · Tareas

Formato en [`docs/proceso.md`](../../docs/proceso.md). Requisitos: E02 y E12 terminados. La terminal integrada llega en E16; mientras, el pago con tarjeta se captura a mano como dice la spec.

## Bloque A · Dominio

- [ ] **E03-T001** [P] `TipCalculator` y `SplitCalculator`.
  - Cumple: BR-CAJ-02 y 03, AC-CAJ-02 y 04.
- [ ] **E03-T002** [P] `PaymentValidator` para los 4 métodos.
  - Cumple: BR-CAJ-04, AC-CAJ-03 y 06.
- [ ] **E03-T003** [P] `Denominations` y `CashCutCalculator`.
  - Cumple: BR-CAJ-09 y 12, AC-CAJ-11.
- [ ] **E03-T004** `CashSession` con pagos, retiros, efectivo esperado y cierre. Depende de T001 a T003.
  - Cumple: BR-CAJ-01, 06, 07 y 09, AC-CAJ-09 y 10.

## Bloque B · Persistencia y API

- [ ] **E03-T005** Tablas + migración `AddCash`. Depende de T004.
- [ ] **E03-T006** Abrir turno y consultar turno. Depende de T005.
  - Cumple: AC-CAJ-01.
- [ ] **E03-T007** Cotización y registro de pagos por parte; cierre de la orden con `OrderPaid`. Efectivo `[OfflineCapable]`. Depende de T006.
  - Cumple: BR-CAJ-05 y 14, AC-CAJ-05, 07 y 14.
- [ ] **E03-T008** Retiros. Depende de T006.
  - Cumple: AC-CAJ-08.
- [ ] **E03-T009** Corte X y corte Z con alerta por diferencia y aviso de órdenes abiertas. Depende de T006.
  - Cumple: BR-CAJ-08 a 10.
- [ ] **E03-T010** Anular cobro con *override* y reapertura de la orden. Depende de T007.
  - Cumple: BR-CAJ-11, AC-CAJ-13.
- [ ] **E03-T010b** Cajón de dinero: pulso `ESC p` con `IPrintService` (E12), apertura al cobrar en efectivo o retirar, y `POST /api/cash/drawer/open` con motivo, permiso u *override* y auditoría. Depende de T007, T008 y E12-T004b.
  - Cumple: BR-COC-13, AC-COC-11 y 12.
- [ ] **E03-T011** PDF del corte (QuestPDF) + `CutDeliveryService` por correo con `IMessageDelivery` + configuración de envío. Depende de T009 y E00-T019b.
  - Cumple: BR-CAJ-13, AC-CAJ-12.
- [ ] **E03-T012** WhatsApp en `IMessageDelivery` (plantilla aprobada) y envío del corte por ahí. Depende de T011 y de P-01.
- [ ] **E03-T013** [P] `IPaidOrderQuery` e `ICashSessionService` para E01, E14, E15 y E16. Depende de T007.

## Bloque C · Pantallas

- [ ] **E03-T014** `ChargeDialog` con propina, dividir, los 4 métodos, botones rápidos y partes. Depende de T007.
  - Comparar contra `img/01` y `02`.
- [ ] **E03-T015** `PaidSummary` y `ReceiptPreview` con QR (provisional hasta E14). Depende de T014.
  - Comparar contra `img/03`.
- [ ] **E03-T016** `CashPage` con indicadores, vista previa X, configuración de envío e historial. Depende de T009 y T011.
  - Comparar contra `img/04`.
- [ ] **E03-T017** `WithdrawalDialog`, `CloseCashDialog`, `OpenSessionDialog` y `VoidPaymentDialog`. Depende de T016.
  - Comparar contra `img/05`, `06`, `07` y `08`.

## Bloque D · Cierre

- [ ] **E03-T018** Recorrido manual de AC-CAJ-01 a 14 con una impresora térmica real.
- [ ] **E03-T019** Contestar P-01 a P-03.
