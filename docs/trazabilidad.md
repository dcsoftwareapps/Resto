# Trazabilidad: de lo que se pidió a dónde está

Cada cosa que se pidió para Resto, y el epic y la regla donde quedó escrita. Si alguien pregunta "¿dónde quedó X?", aquí está la respuesta. Si algo de esta lista no tiene epic, es un hueco y se arregla antes de construir.

## 1. Requisitos originales del producto

| # | Lo que se pidió | Dónde quedó |
|---|---|---|
| 1 | App de escritorio modular para restaurante | Todos los epics *Escritorio*. Módulos por plan en E21 |
| 2 | Inventario llenado directo en escritorio | E04 §3.1 y §3.2 (alta y ajustes) |
| 3 | Inventario llenado desde el celular, súper fácil, con botones + para el comprador | E11 §3 (Faltan, lista del comprador, *Contar*), BR-CHF-03, BR-CHF-08 |
| 4 | Merma configurable por porcentaje | E08 BR-CFG-04 (general y por insumo), E04 BR-INV-02, E05 BR-REC-01 |
| 5 | Chef pide pendientes o faltantes al comprador por celular o escritorio | E11 §3, E06 BR-COM-03 |
| 6 | Que no le dé más trabajo al chef ni al comprador | Constitución §1.1 (máximo 3 toques), E11 completo |
| 7 | Propiedades configurables con valores por defecto | E08 BR-CFG-05 (propiedades extra), E08 §3 |
| 8 | Roles configurables | E07 (matriz de permisos y catálogo completo) |
| 9 | Crear usuarios | E07 §3, PIN en E00 BR-PLT-01 |
| 10 | Punto de venta con órdenes de mesero auditadas (quién, cómo, cuándo creó o modificó) | E02 (historial de la orden), E00 BR-PLT-06 |
| 11 | Todo auditado: quién pidió, cuándo se compró, el comprador dice "ya se compró" | E00 BR-PLT-06, E06 BR-COM-10, E09 (consulta) |
| 12 | Ingredientes y cantidades por platillo para descontar inventario | E05 (recetas), E04 BR-INV-02 |
| 13 | Las órdenes del celular del mesero llegan solas al tablero de órdenes abiertas | E10 → E01 (tiempo real con SignalR, `docs/contratos.md`) |
| 14 | Cobrar, pagar y dar cambio | E03 BR-CAJ-04 |
| 15 | IVA o cualquier impuesto, configurable a toda la cuenta o a productos | E08 §3 Impuestos (`tax_rules`, `tax_rule_products`), E02 (totales) |
| 16 | Comanda impresa en terminales de cocina o en pantalla | E12 (pantalla y ESC/POS por estación) |
| 17 | Abrir comanda en cocina y botón "despachada" completa o por platillo | E12 BR-COC-04 y BR-COC-05 |
| 18 | Avisos entre comprador y quien pidió, con enlace que abre la app | E00 §3.3 (campana y *push*), E06 BR-COM-03, E11 §3.7 |
| 19 | Cortes de caja, cierres parciales | E03 (corte X, cierre Z por denominación, retiros) |
| 20 | Colores formales y profesionales | Constitución §1.6 y §8 (azul marino y cobre) |

## 2. Funciones del PDF de Soft Restaurant que se agregaron

| Función de Soft Restaurant | Dónde quedó |
|---|---|
| Facturación electrónica CFDI 4.0 y autofactura | E14 |
| Factura global | E14 |
| Menú digital con QR | E17 |
| Kiosko de autoservicio | E19 |
| Pedidos de Uber Eats, Rappi, DiDi y pedido web | E18 |
| Promociones 2x1, por porcentaje, día y horario | E13 |
| Reportes de ventas por día, hora, canal y mesero; utilidad por platillo | E15 |
| Terminal bancaria integrada | E16 |
| Seguridad antifraude: PIN de gerente, alertas por cancelaciones y reimpresiones | E09 |
| Multisucursal y almacenes con traspasos | E20 |
| Paquetes o combos | E05 |
| Cajón de dinero | E12 (abre al cobrar en efectivo) |
| Monitoreo remoto (ver la venta desde el celular) | E15 (reportes en web), E21 (consola) |
| Drive thru y para llevar | E02 |
| Envío del corte por correo o WhatsApp | E03 |
| Migración desde otro sistema por Excel | E04 (insumos) y E05 (menú y recetas, BR-REC-12) |

## 3. Pendientes del Excel `Resto_Pendientes.xlsx`

| # | Pendiente | Dónde quedó |
|---|---|---|
| 1 | Timbrado CFDI 4.0 con PAC y portal público | E14 |
| 2 | Login con PIN y bloqueo por inactividad | E00 BR-PLT-01 y BR-PLT-04 |
| 3 | Impresión real ESC/POS y cajón de dinero | E12 |
| 4 | *Push* reales y modo sin internet | E00 §7 (sin conexión) y T020 (*push*) |
| 5 | Unidad de compra contra unidad de receta | E04 BR-INV-15 |
| 6 | Terminal bancaria con conciliación | E16 |
| 7 | Reportes históricos, costo real contra teórico | E15, E04 (kardex y toma de inventario) |
| 8 | Integración real con Uber Eats, Rappi y DiDi | E18 |
| 9 | Varios almacenes con traspasos | E20 |
| 10 | Multisucursal | E20 |
| 11 | Kiosko físico | E19 |

## 4. Lo que agregó la revisión (no lo pidió nadie, pero un sistema real lo necesita)

| Tema | Dónde quedó |
|---|---|
| Cambiar de mesa y cancelar una orden completa | E02 §3, BR-POS-17 |
| Kardex por insumo y toma de inventario física | E04 §3.5 y §3.6, BR-INV-17 |
| Contar inventario desde el celular, *No lo encontré* | E11 §3.5 y §3.6, BR-CHF-08 |
| Alérgenos, calorías y traducción sugerida | E05 BR-REC-11 y BR-REC-13 |
| Respaldos probados, monitoreo y seguridad | E00 §3.6, BR-PLT-14 y BR-PLT-15 |
| Aviso de privacidad (LFPDPPP) y borrado de datos de clientes | E00 BR-PLT-16, constitución §1.8 |
| Accesibilidad | Constitución §1.7 |
| Ayuda y soporte por WhatsApp | E21 §3.5 |
| Suscripciones, prueba gratis y planes | E21 |

## 5. Lo que no entra

Ver [`epics.md` → Fuera del producto por ahora](epics.md#fuera-del-producto-por-ahora).
