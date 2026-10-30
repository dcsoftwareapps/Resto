# E17 · Cliente → Menú QR · Plan técnico

## 1. Resumen

Se vive en `Resto.Web` como páginas **públicas** con renderizado estático en servidor (SSR). Solo el carrito usa interactividad (*Interactive Server* o un poco de JS), para no cargar WebAssembly en el celular del comensal. Los componentes van en `Resto.UI/Features/CustomerMenu/` porque los reutilizan E18 (página web de pedidos) y E19 (kiosko).

## 2. Modelo de datos

| Tabla | Columnas |
|---|---|
| `table_qr_tokens` | `Token` (32 caracteres aleatorios, PK), `RestaurantId`, `DiningTableId`, `CreatedAt`, `RevokedAt` |

Las traducciones (`NameEn`, `DescriptionEn`) y las fotos son de E05. `CATEN` del demo pasa a `menu_categories.NameEn`.

## 3. API y rutas

| Ruta | Tipo | Notas |
|---|---|---|
| `GET /m/{token}` | Página SSR | Menú (BR-QR-02 y 03) |
| `GET /m/{token}/pedido` | Página interactiva | Carrito y enviar |
| `GET /m/{token}/estado/{incomingId}` | Página | Estado (BR-QR-07) |
| `POST /public/qr/{token}/orders` `{ Lines[] }` | API | → `IIncomingOrders.ReceiveAsync` (E18). *Rate limit* por token (BR-QR-05). |
| `POST /public/qr/{token}/request-bill` | API | BR-QR-06 |
| `POST /api/tables/{id}/qr/regenerate` · `GET /api/tables/qr-sheet.pdf` | Interna, `settings.manage` | Regenerar e imprimir |

## 4. Componentes (`Resto.UI/Features/CustomerMenu/`)

| Componente | Captura |
|---|---|
| `CustomerHeader.razor` (restaurante, mesa o modo, selector de idioma) | 01 |
| `CustomerCategoryChips.razor`, `CustomerMenuItemCard.razor` (foto o color de categoría, etiquetas) | 01, 02 |
| `CustomerCart.razor` | 03 |
| `OrderStatusSteps.razor` | 04 |
| `CustomerStrings.resx` (ES y EN) con los textos de la interfaz del cliente | 02 |

## 5. Pruebas

| Proyecto | Qué cubre |
|---|---|
| Api.Tests | AC-QR-04, 06 y 07; *rate limit*; pedido sin confirmar que se acumula (BR-QR-05) |
| UI.Tests | Idioma (AC-QR-02), etiquetas de promoción y agotado, canal en pausa (AC-QR-08) |
| Manual | AC-QR-01 con Lighthouse en móvil y 4G simulado (< 2 s); probar el QR impreso |

## 6. Paquetes

| Paquete | Para qué |
|---|---|
| `QRCoder` (MIT) | Generar los QR del PDF de mesas |
