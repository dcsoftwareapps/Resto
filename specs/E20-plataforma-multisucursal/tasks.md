# E20 · Plataforma → Multisucursal · Tareas

Formato en [`docs/proceso.md`](../../docs/proceso.md). Requisitos: toda la ola 1 y E15. **Si se contesta que sí a P-01, el Bloque B (almacenes) se adelanta a la ola 1.**

## Bloque A · Sucursales y usuarios

- [ ] **E20-T001** `user_branch_roles` + migración de datos + `switch-branch`.
  - Cumple: BR-SUC-03, AC-SUC-03.
- [ ] **E20-T002** Extender la prueba de aislamiento para usuarios con varias sucursales. Depende de T001.
  - Cumple: BR-SUC-01.
- [ ] **E20-T003** `BranchSwitcher` + rol por sucursal en Usuarios. Depende de T001.
  - Comparar contra `img/03`.
- [ ] **E20-T004** Alta de sucursal copiando otra. Depende de T001.
  - Cumple: BR-SUC-07, AC-SUC-06.

## Bloque B · Almacenes y traspasos

- [ ] **E20-T005** `storage_locations` + `ingredient_stocks` + migración desde la propiedad "Almacén" + almacén de consumo por estación.
  - Cumple: BR-SUC-04, AC-SUC-05.
- [ ] **E20-T006** Traspasos: enviar, recibir con diferencia y cancelar. Depende de T005.
  - Cumple: BR-SUC-05, AC-SUC-04.
- [ ] **E20-T007** `StorageLocationsTab`, `TransfersPage` y `TransferFormDialog`. Depende de T006.
  - Comparar contra `img/04`. Comparar el filtro contra `img/01`.

## Bloque C · Menú y reportes

- [ ] **E20-T008** Menú de organización + sobrescrituras por sucursal. Depende de T001.
  - Cumple: BR-SUC-02, AC-SUC-02.
- [ ] **E20-T009** Reportes con `branchId=all` + `OrganizationPage`. Depende de T001 y E15.
  - Comparar contra `img/02`. Cumple: BR-SUC-06, AC-SUC-01 y 07.
- [ ] **E20-T010** Recorrido con 2 sucursales de prueba y contestar P-01 y P-02.
