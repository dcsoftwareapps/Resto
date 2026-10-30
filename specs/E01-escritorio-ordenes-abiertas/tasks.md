# E01 · Escritorio → Órdenes abiertas · Tareas

Formato en [`docs/proceso.md`](../../docs/proceso.md). Requisitos: E02, E03 y E08 terminados.

- [ ] **E01-T001** Lectura de las mesas de E08 (`dining_tables`) con número, nombre, zona y estado para el mapa. Depende de E08-T001b.
- [ ] **E01-T002** [P] Reglas de Domain: estado de mesa, avance, orden de tarjetas y nivel de antigüedad.
  - Cumple: BR-ORD-02, 03, 05 y 07.
- [ ] **E01-T003** `GET /api/floor` con indicadores (incluye *Vendido en el turno* solo con `cash.view`). Depende de T001 y T002.
  - Cumple: BR-ORD-04, AC-ORD-01, 06 y 07.
- [ ] **E01-T004** `FloorPage`, `FloorKpis`, `TableMap` y `OrderCard`. Depende de T003.
  - Comparar contra `img/01`. Cumple: BR-ORD-01.
- [ ] **E01-T005** Navegación: mesa libre → *Nueva orden* (E02); mesa ocupada o *Abrir* → Punto de venta; *Cobrar* → E03. Depende de T004. (El botón *Entregar* de pedidos en línea lo agrega E18-T003b.)
  - Cumple: AC-ORD-03 y 04.
- [ ] **E01-T006** Tiempo real, temporizador de 30 s y `Resynced`. Depende de T004.
  - Cumple: BR-ORD-06, AC-ORD-05 y 08.
- [ ] **E01-T007** Recorrido manual de AC-ORD-01 a 08 y contestar P-01.
