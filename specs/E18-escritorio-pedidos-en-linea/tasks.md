# E18 · Escritorio → Pedidos en línea · Tareas

Formato en [`docs/proceso.md`](../../docs/proceso.md). Requisitos: E02-T014, E05-T005 y E12-T008. **Decidir P-01 antes de T008.**

- [ ] **E18-T001** Tablas + migración.
- [ ] **E18-T002** `IIncomingOrders` + `IncomingOrderReceived` + sonido de pedidos pendientes. Depende de T001.
  - Cumple: BR-LIN-01, AC-LIN-01.
- [ ] **E18-T003** Aceptar (crea la orden y la envía), rechazar, entregar y atrasado. Depende de T002.
  - Cumple: BR-LIN-02, 04, 05 y 10, AC-LIN-02 a 05.
- [ ] **E18-T004** `IDeliveryChannelGateway` + falso + simulador. Depende de T002.
- [ ] **E18-T005** Pausar canales y precios por canal. Depende de T004.
  - Cumple: BR-LIN-06 y 07, AC-LIN-06.
- [ ] **E18-T006** `OnlineOrdersPage` y `RejectDialog`. Depende de T003.
  - Comparar contra `img/01` y `04`.
- [ ] **E18-T007** Página web de pedidos: carrito, *checkout*, zona, mínimo y estado del pedido. Depende de T002 y E17-T004 (componentes compartidos).
  - Comparar contra `img/03`. Cumple: BR-LIN-09, AC-LIN-07.
- [ ] **E18-T008** Adaptador real (integrador o apps) + *webhooks* + cancelación por la app. Depende de T004 y P-01.
  - Cumple: BR-LIN-03.
- [ ] **E18-T009** `MenuSyncService` (cambios y agotados). Depende de T008.
  - Cumple: BR-LIN-08, AC-LIN-08.
- [ ] **E18-T010** `OnlineSettingsTab`. Depende de T005.
  - Comparar contra `img/05`.
- [ ] **E18-T010b** `IPersonalDataEraser` para `incoming_orders`. Depende de E00-T024e.
  - Cumple: E00 BR-PLT-16, AC-PLT-19.
- [ ] **E18-T011** Recorrido manual y contestar P-01 a P-03.
