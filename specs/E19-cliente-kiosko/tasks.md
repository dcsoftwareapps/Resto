# E19 · Cliente → Kiosko · Tareas

Formato en [`docs/proceso.md`](../../docs/proceso.md). Requisitos: E16-T003, E17-T004 y E12.

- [ ] **E19-T001** `POST /api/kiosk/checkout` con validación del pago aprobado y del monto, y turnos K-N.
  - Cumple: BR-KIO-02 y 03, AC-KIO-01 y 02.
- [ ] **E19-T002** `KioskWelcomePage` y `KioskMenuPage` con los componentes de E17 en tamaño kiosko. Depende de E17-T004.
  - Comparar contra `img/01` y `04`.
- [ ] **E19-T003** `KioskCartPage` con cobro en terminal. Depende de T001 y E16-T005.
  - Comparar contra `img/02`. Cumple: BR-KIO-01, AC-KIO-04.
- [ ] **E19-T004** `KioskTicketPage` con impresión del turno y regreso automático. Depende de T003.
  - Comparar contra `img/03`. Cumple: BR-KIO-06, AC-KIO-05.
- [ ] **E19-T005** Inactividad y `IdlePrompt`. Depende de T002.
  - Comparar contra `img/05`. Cumple: BR-KIO-04, AC-KIO-03.
- [ ] **E19-T006** Modo kiosko en Android (bloqueo, pantalla encendida y salida con PIN) + guía para iOS. Depende de T002.
  - Cumple: BR-KIO-05, AC-KIO-06.
- [ ] **E19-T007** Pausa desde E18. Depende de E18-T005.
  - Cumple: BR-KIO-07.
- [ ] **E19-T008** Recorrido con hardware real y contestar P-01 y P-02.
