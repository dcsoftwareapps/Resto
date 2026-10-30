# E19 · Cliente → Kiosko · Plan técnico

## 1. Resumen

Son pantallas en `Resto.UI/Features/Kiosk/` que se muestran en `Resto.App` cuando el dispositivo es de tipo *Kiosko* (E00). Reutilizan los componentes de `Features/CustomerMenu/` (E17) con una clase CSS `kiosk` para agrandar. No tiene tablas propias.

## 2. Endpoint nuevo

| Método y ruta | Permiso | Notas |
|---|---|---|
| `POST /api/kiosk/checkout` `{ Lines[], CustomerName?, PaymentId }` | dispositivo *Kiosko* | Solo procede si `terminal_transactions[PaymentId]` está *Aprobada*. Crea la orden, registra el pago y envía a cocina en una transacción (BR-KIO-02). Idempotente por `PaymentId`. |

Flujo:
1. La app genera el `PaymentId`.
2. Llama a `POST /api/payments/terminal/charge` (E16) con la terminal asignada al kiosko.
3. Espera `TerminalPaymentUpdated`.
4. Si es *Aprobada*, llama a `checkout`.
5. El servidor valida que el monto aprobado = total calculado con `OrderPricing`.

El turno de caja que recibe los pagos del kiosko se configura en el dispositivo (`devices.CashSessionRegisterId`).

## 3. Pantallas

| Componente | Captura |
|---|---|
| `KioskWelcomePage.razor` | — |
| `KioskMenuPage.razor` (usa `CustomerMenu/*`) | 01 |
| `KioskCartPage.razor` + `TerminalChargePanel` (E16) | 02 |
| `KioskTicketPage.razor` (turno + estado, regresa a los 15 s) | 03 |
| `IdlePrompt.razor` ("¿Sigues ahí?") | — |

## 4. Modo kiosko (MAUI)

- **Android:** `StartLockTask()` (*screen pinning*), `KeepScreenOn` y ocultar las barras del sistema. Para salir, `StopLockTask()` después del PIN de gerente.
- **iOS:** se documenta cómo activar Acceso Guiado. La app no puede forzarlo.
- **Windows** (pantallas táctiles grandes): ventana a pantalla completa sin bordes.

## 5. Pruebas

| Proyecto | Qué cubre |
|---|---|
| Api.Tests | `checkout` con pago aprobado, rechazado o no existente, monto distinto e idempotencia (AC-KIO-01 y 02) |
| UI.Tests | Inactividad (AC-KIO-03) y terminal fuera de línea (AC-KIO-04) |
| Manual | Tablet Android en modo kiosko con terminal y impresora (AC-KIO-01, 05 y 06) |

## 6. Paquetes

Ninguno.
