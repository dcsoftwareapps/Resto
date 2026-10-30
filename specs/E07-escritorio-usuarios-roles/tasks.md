# E07 · Escritorio → Usuarios y roles · Tareas

Formato en [`docs/proceso.md`](../../docs/proceso.md). Requisitos: E00-T009 a T013.

- [ ] **E07-T001** Catálogo completo `Permissions` con metadatos, y `DefaultRoles` igual a la tabla de la spec.
  - Cumple: BR-USR-09.
- [ ] **E07-T002** [P] `PinPolicy` y `UserRules`.
  - Cumple: BR-USR-01, BR-USR-02, 04, 05 y 08.
- [ ] **E07-T003** Completar tablas (`Email`, invitación, `SessionVersion` y `PermissionsVersion`) + migración. Depende de T001.
- [ ] **E07-T004** Validación de versiones en el middleware. Depende de T003.
  - Cumple: BR-USR-07, AC-USR-03 y 05.
- [ ] **E07-T005** API de usuarios (alta, edición, PIN, activar, desactivar y cerrar sesiones). Depende de T003.
  - Cumple: BR-USR-06, AC-USR-01, 02, 05 y 06.
- [ ] **E07-T006** Invitación por correo y aceptarla. Depende de T005.
  - Cumple: BR-USR-03, AC-USR-08.
- [ ] **E07-T007** API de roles y permisos con auditoría. Depende de T003.
  - Cumple: BR-USR-10, AC-USR-04 y 07.
- [ ] **E07-T008** Prueba de que todos los endpoints usan permisos del catálogo. Depende de T001.
- [ ] **E07-T009** `UsersPage`, `UsersTable` y `UserFormDialog`. Depende de T005.
  - Comparar contra `img/01`, `03` y `06`.
- [ ] **E07-T010** `PermissionMatrix` y `RoleFormDialog`. Depende de T007.
  - Comparar contra `img/02` y `04`.
- [ ] **E07-T011** Recorrido manual de AC-USR-01 a 09 y contestar P-01 y P-02.
