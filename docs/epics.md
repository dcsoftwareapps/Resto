# Mapa de epics

Cada epic es una **superficie → módulo** que una persona puede llevar de principio a fin. Son 22 (E00 a E21). Cada uno tiene su carpeta en `specs/` con `spec.md`, `plan.md`, `tasks.md` e `img/` (capturas del demo, cuando la pantalla existe en el demo).

**Superficies:** Plataforma (lo que no se ve), Escritorio (caja y administración), Móvil (mesero, chef y compras), Cocina (pantalla e impresoras) y Cliente (QR, web y kiosko).

**Todo entra en el producto.** No hay piloto ni recortes.

**Regla de oro: un epic que se empieza se termina.** No se deja nada a medias ni con implementaciones provisionales. Por eso el orden sigue las dependencias reales entre tareas: cuando un epic arranca, todo lo que necesita ya está terminado. Las tareas que antes cruzaban epics se movieron al epic que llega después (por ejemplo, el botón "A compras" de Inventario ahora lo hace E06, y las impresoras pasaron de E08 a E12).

La columna **#** es el orden de construcción y **Fase** agrupa lo que se entrega junto. La hoja de ruta con el porqué está en el documento *Resto · Hoja de ruta*.

- **Fase 0 · Base:** E00 completo.
- **Fase 1 · Inventario:** configuración, inventario, recetas, compras y el celular del chef. En paralelo, usuarios y seguridad.
- **Fase 2 · Venta:** punto de venta, cocina, cobro, órdenes abiertas y celular del mesero.
- **Fase 3 · Negocio:** promociones, facturación, pagos, reportes y suscripciones.
- **Fase 4 · Canales:** pedidos en línea, menú QR y web, kiosko y multisucursal.

## Tabla de epics

| # | ID | Superficie → módulo | Qué entrega | Fase | Depende de | Responsable | Documentos |
|---|---|---|---|---|---|---|---|
| 1 | **E00** | Plataforma → Fundaciones | Solución .NET, base de datos, multi restaurante, login con usuario y PIN, auditoría, SignalR, sincronización sin conexión, tokens de diseño, respaldos, monitoreo, seguridad y aviso de privacidad | 0 | — | Por definir | [spec](../specs/E00-plataforma-fundaciones/spec.md) · [plan](../specs/E00-plataforma-fundaciones/plan.md) · [tareas](../specs/E00-plataforma-fundaciones/tasks.md) |
| 2 | **E08** | Escritorio → Configuración | Impuestos (cuenta o producto), merma general y por insumo, días de aviso de caducidad, propiedades del inventario, estaciones e impresoras, datos del restaurante | 1 | E00 | Por definir | [spec](../specs/E08-escritorio-configuracion/spec.md) · [plan](../specs/E08-escritorio-configuracion/plan.md) · [tareas](../specs/E08-escritorio-configuracion/tasks.md) |
| 3 | **E04** | Escritorio → Inventario | Lista con estado, filtros, ajustes, alta de insumos, movimientos, descuento automático por receta y merma, unidad de compra, importar de Excel, kardex y toma de inventario física | 1 | E08 | Por definir | [spec](../specs/E04-escritorio-inventario/spec.md) · [plan](../specs/E04-escritorio-inventario/plan.md) · [tareas](../specs/E04-escritorio-inventario/tasks.md) |
| 4 | **E05** | Escritorio → Recetas y paquetes | Receta por platillo, costo y margen, porciones disponibles, paquetes y extras, alérgenos, traducción sugerida e importar menú de Excel | 1 | E04 | Por definir | [spec](../specs/E05-escritorio-recetas-paquetes/spec.md) · [plan](../specs/E05-escritorio-recetas-paquetes/plan.md) · [tareas](../specs/E05-escritorio-recetas-paquetes/tasks.md) |
| 5 | **E06** | Escritorio → Compras y proveedores | Sugeridos, lista de compras, proveedores, órdenes de compra, entrada a inventario | 1 | E04 | Por definir | [spec](../specs/E06-escritorio-compras-proveedores/spec.md) · [plan](../specs/E06-escritorio-compras-proveedores/plan.md) · [tareas](../specs/E06-escritorio-compras-proveedores/tasks.md) |
| 6 | **E11** | Móvil → Chef y compras | Faltantes con un toque, lista del comprador por proveedor, marcar comprado, *No lo encontré*, contar inventario desde el celular, avisos entre ambos | 1 | E06 | Por definir | [spec](../specs/E11-movil-chef-compras/spec.md) · [plan](../specs/E11-movil-chef-compras/plan.md) · [tareas](../specs/E11-movil-chef-compras/tasks.md) |
| 7 | **E07** | Escritorio → Usuarios y roles | Alta de usuarios con PIN, roles configurables, matriz de permisos | 1 | E00 | Por definir | [spec](../specs/E07-escritorio-usuarios-roles/spec.md) · [plan](../specs/E07-escritorio-usuarios-roles/plan.md) · [tareas](../specs/E07-escritorio-usuarios-roles/tasks.md) |
| 8 | **E09** | Escritorio → Auditoría y seguridad | Consulta de auditoría, autorización con PIN de gerente, alertas antifraude y sus reglas (incluye las de inventario descontado de más que manda E04) | 1 | E07 | Por definir | [spec](../specs/E09-escritorio-auditoria-seguridad/spec.md) · [plan](../specs/E09-escritorio-auditoria-seguridad/plan.md) · [tareas](../specs/E09-escritorio-auditoria-seguridad/tasks.md) |
| 9 | **E02** | Escritorio → Punto de venta | Abrir orden (comedor, para llevar, drive thru), agregar platillos, enviar a cocina, cancelar con motivo, historial de la orden | 2 | E05, E08, E09 | Por definir | [spec](../specs/E02-escritorio-punto-de-venta/spec.md) · [plan](../specs/E02-escritorio-punto-de-venta/plan.md) · [tareas](../specs/E02-escritorio-punto-de-venta/tasks.md) |
| 10 | **E12** | Cocina → Pantalla e impresión | Comandas por estación, tiempos, despachar por platillo o completa, impresión ESC/POS por estación | 2 | E02 | Por definir | [spec](../specs/E12-cocina-pantalla-impresion/spec.md) · [plan](../specs/E12-cocina-pantalla-impresion/plan.md) · [tareas](../specs/E12-cocina-pantalla-impresion/tasks.md) |
| 11 | **E03** | Escritorio → Cobro, caja y cortes | Cobro en efectivo con cambio, tarjeta, mixto, propina, dividir cuenta, retiros, corte X, cierre Z por denominación | 2 | E12 | Por definir | [spec](../specs/E03-escritorio-cobro-caja-cortes/spec.md) · [plan](../specs/E03-escritorio-cobro-caja-cortes/plan.md) · [tareas](../specs/E03-escritorio-cobro-caja-cortes/tasks.md) |
| 12 | **E01** | Escritorio → Órdenes abiertas | Mapa de mesas, tarjetas de órdenes, estados y avance de cocina | 2 | E03 | Por definir | [spec](../specs/E01-escritorio-ordenes-abiertas/spec.md) · [plan](../specs/E01-escritorio-ordenes-abiertas/plan.md) · [tareas](../specs/E01-escritorio-ordenes-abiertas/tasks.md) |
| 13 | **E10** | Móvil → Mesero | Mesas, menú por categoría, notas y extras, enviar a cocina, pedir cuenta, avisos de "ya salió" | 2 | E12 | Por definir | [spec](../specs/E10-movil-mesero/spec.md) · [plan](../specs/E10-movil-mesero/plan.md) · [tareas](../specs/E10-movil-mesero/tasks.md) |
| 14 | **E13** | Escritorio → Promociones | 2x1 y porcentaje por categoría, por día y horario, aplicación automática | 3 | E02 | Por definir | [spec](../specs/E13-escritorio-promociones/spec.md) · [plan](../specs/E13-escritorio-promociones/plan.md) · [tareas](../specs/E13-escritorio-promociones/tasks.md) |
| 15 | **E14** | Escritorio → Facturación | CFDI 4.0 con PAC, validación de RFC, portal de autofacturación con QR, factura global | 3 | E03 | Por definir | [spec](../specs/E14-escritorio-facturacion/spec.md) · [plan](../specs/E14-escritorio-facturacion/plan.md) · [tareas](../specs/E14-escritorio-facturacion/tasks.md) |
| 16 | **E16** | Plataforma → Pagos | Terminal bancaria integrada y conciliación con el corte | 3 | E03 | Por definir | [spec](../specs/E16-plataforma-pagos/spec.md) · [plan](../specs/E16-plataforma-pagos/plan.md) · [tareas](../specs/E16-plataforma-pagos/tasks.md) |
| 17 | **E15** | Escritorio → Reportes | Ventas por día, hora, canal y mesero, utilidad por platillo, exportar a Excel | 3 | E14 | Por definir | [spec](../specs/E15-escritorio-reportes/spec.md) · [plan](../specs/E15-escritorio-reportes/plan.md) · [tareas](../specs/E15-escritorio-reportes/tasks.md) |
| 18 | **E21** | Plataforma → Suscripciones y planes | Registro en línea, prueba de 30 días, planes Esencial, Completo y Pro, cobro recurrente, módulos por plan, factura de la licencia, ayuda y soporte por WhatsApp, y consola interna | 3 | E14 | Por definir | [spec](../specs/E21-plataforma-suscripciones/spec.md) · [plan](../specs/E21-plataforma-suscripciones/plan.md) · [tareas](../specs/E21-plataforma-suscripciones/tasks.md) |
| 19 | **E18** | Escritorio → Pedidos en línea | Tablero de apps de delivery y pedido web, aceptar o rechazar, pausar canales | 4 | E01, E12 | Por definir | [spec](../specs/E18-escritorio-pedidos-en-linea/spec.md) · [plan](../specs/E18-escritorio-pedidos-en-linea/plan.md) · [tareas](../specs/E18-escritorio-pedidos-en-linea/tasks.md) |
| 20 | **E17** | Cliente → Menú QR | Menú con fotos en español e inglés, pedido desde la mesa, pedir la cuenta | 4 | E10, E13, E18 | Por definir | [spec](../specs/E17-cliente-menu-qr/spec.md) · [plan](../specs/E17-cliente-menu-qr/plan.md) · [tareas](../specs/E17-cliente-menu-qr/tasks.md) |
| 21 | **E19** | Cliente → Kiosko | Autoservicio con pago y turno | 4 | E16, E17 | Por definir | [spec](../specs/E19-cliente-kiosko/spec.md) · [plan](../specs/E19-cliente-kiosko/plan.md) · [tareas](../specs/E19-cliente-kiosko/tasks.md) |
| 22 | **E20** | Plataforma → Multisucursal | Varias sucursales, catálogos compartidos, almacenes con traspasos, reportes consolidados | 4 | E15 y toda la fase 2 | Por definir | [spec](../specs/E20-plataforma-multisucursal/spec.md) · [plan](../specs/E20-plataforma-multisucursal/plan.md) · [tareas](../specs/E20-plataforma-multisucursal/tasks.md) |

## Dependencias

Solo se dibuja la dependencia directa más cercana. Todo depende de E00.

```mermaid
flowchart LR
  E00[E00 Fundaciones] --> E08[E08 Configuración]
  E08 --> E04[E04 Inventario]
  E04 --> E05[E05 Recetas]
  E04 --> E06[E06 Compras]
  E06 --> E11[E11 Móvil chef]
  E00 --> E07[E07 Usuarios]
  E07 --> E09[E09 Seguridad]
  E05 --> E02[E02 Punto de venta]
  E08 --> E02
  E09 --> E02
  E02 --> E12[E12 Cocina]
  E12 --> E03[E03 Cobro]
  E03 --> E01[E01 Órdenes]
  E12 --> E10[E10 Móvil mesero]
  E02 --> E13[E13 Promociones]
  E03 --> E14[E14 Facturación]
  E03 --> E16[E16 Pagos]
  E14 --> E15[E15 Reportes]
  E14 --> E21[E21 Suscripciones]
  E01 --> E18[E18 Pedidos en línea]
  E12 --> E18
  E10 --> E17[E17 Menú QR y web]
  E13 --> E17
  E18 --> E17
  E16 --> E19[E19 Kiosko]
  E17 --> E19
  E15 --> E20[E20 Multisucursal]
```

## Fuera del producto por ahora

Para que nadie lo empiece por su cuenta. Si un cliente lo pide, se anota aquí y se decide si se vuelve un epic nuevo.

| Tema | Por qué no entra todavía |
|---|---|
| Reservaciones y lista de espera | No lo pidieron los usuarios ni está en el PDF de Soft Restaurant que comparamos |
| Nómina, asistencia y checador | Es otro producto; se resuelve con integraciones |
| Contabilidad (pólizas, bancos, cuentas por pagar) | Se exportan ventas, compras y facturas a Excel (E15) para el contador |
| Programa de lealtad y monedero | Se revisa cuando estén las ventas en línea (E18) |
| Tienda de apps o integraciones abiertas (API pública) | Primero se estabiliza la API interna |
| Pedido a domicilio con repartidores propios | Se cubre con las apps de delivery (E18) |

La relación entre lo que se pidió y cada epic está en [`trazabilidad.md`](trazabilidad.md).

## Para la reunión de reparto

El reparto queda por definir. Estos datos ayudan a decidirlo:

- **E00 lo hacen los dos.** Es el más grande y todo depende de él. Tiene muchas tareas `[P]` que se pueden repartir.
- **En la Fase 1 hay dos carriles que no se tocan:**
  - **Inventario:** E08 → E04 → E05 → E06 → E11
  - **Usuarios y seguridad:** E07 → E09
- Cuando terminan los dos carriles, arranca E02. Desde ahí, por ejemplo, uno sigue con E12 → E03 → E01 y el otro con E13 o E10 en cuanto se puedan.
- **Antes de la Fase 3 hay que decidir proveedores:** PAC (E14), terminal (E16), procesador de suscripciones (E21) e integrador de delivery (E18). Y hay que juntarse con un contador por las preguntas fiscales de E02, E08 y E14.

Cuando decidan, escriban el nombre en la columna *Responsable* de la tabla y en el encabezado de cada `spec.md`.

## Cómo crear la spec de un epic

1. Copia `docs/plantillas/` a `specs/E05-escritorio-recetas/`.
2. Abre el demo en la pantalla del módulo y toma capturas a `img/` (o pídeselo a la IA).
3. Llena `spec.md`. Puedes pedirle a la IA un borrador con este prompt:
   > Lee `docs/constitucion.md`, `docs/epics.md`, `specs/E04-escritorio-inventario/spec.md` como ejemplo, y el demo `docs/demo/resto-demo.html` en el módulo Recetas. Escribe `specs/E05-escritorio-recetas/spec.md` siguiendo la plantilla. No inventes reglas: lo que no esté claro va en Preguntas abiertas.
4. **La otra persona revisa la spec** antes de pasar al plan. Este paso es el más importante.
5. Luego se escriben el plan y las tareas, que también se pueden pedir a la IA con el mismo método.
