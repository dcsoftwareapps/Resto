# E15 · Escritorio → Reportes · Plan técnico

## 1. Modelo de datos

| Tabla | Columnas | Notas |
|---|---|---|
| `daily_sales_summary` | `RestaurantId`, `BusinessDay`, `Channel`, `WaiterId?`, `Hour`, `Tickets`, `Sales`, `NetSales`, `Discounts`, `Tips`, `Cost` | Se genera al cerrar cada día de operación (BR-REP-06) |
| `daily_item_summary` | `RestaurantId`, `BusinessDay`, `MenuItemId`, `Units`, `Sales`, `NetSales`, `Cost` | |
| `order_items.UnitCostSnapshot` | (columna nueva en la tabla de E02) | Costo de receta al enviar a cocina (BR-REP-03) |

- Un `BackgroundService` (`DailySummaryJob`) corre cada 15 minutos y genera el resumen de los días de operación que ya terminaron y no tienen resumen.
- Se puede regenerar a mano un día con `POST /api/reports/rebuild?day=` (permiso `settings.manage`).

## 2. Consultas (`Resto.Infrastructure/Reports/`)

- Una clase por reporte (`SalesByDayQuery`, `TopItemsQuery`, `TheoreticalVsActualQuery`, …).
- Cada una combina el resumen diario para los días cerrados con la consulta en vivo para hoy.
- Se usa SQL con EF (`FromSql`) o LINQ. Lo que sea más legible, con `AsNoTracking`.
- Interfaz común: `IReport<TFilter, TRow>` con `Columns` (para la tabla y el Excel) y `ExecuteAsync`.

## 3. API (`/api/reports`)

| Método y ruta | Permiso |
|---|---|
| `GET /api/reports/dashboard?from=&to=&channel=&branchId=` | `reports.view` |
| `GET /api/reports/{key}?…filtros` (JSON) | `reports.view` |
| `GET /api/reports/{key}/export?format=xlsx\|pdf&…` | `reports.view` |

## 4. Pantallas (`Resto.UI/Features/Reports/`)

| Componente | Captura |
|---|---|
| `ReportsDashboardPage.razor` (`/reportes`) | 01 |
| `PeriodPicker.razor`, `KpiCard.razor` (con variación) | 01 |
| `BarChart.razor` (vertical, con *tooltip* y etiqueta en máximo y actual) y `HBarChart.razor` | 01 |
| `ReportPage.razor` (`/reportes/{key}`) genérico: filtros + `DataTable` + exportar | — |

Las gráficas son componentes propios con HTML y CSS, como en el demo, sin librería de gráficas: son pocas y simples.

## 5. Pruebas

| Proyecto | Qué cubre |
|---|---|
| Domain.Tests | Variación, diferencia teórico contra real (AC-REP-04) |
| Api.Tests | AC-REP-02, 03 y 07; resumen diario = consulta en vivo para el mismo día (prueba de equivalencia); AC-REP-06 con 180 días de datos generados |
| Manual | AC-REP-01, 05 y 08 |

## 6. Paquetes

Ninguno nuevo (`ClosedXML` y `QuestPDF` ya están).
