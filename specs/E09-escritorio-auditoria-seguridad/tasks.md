# E09 · Escritorio → Auditoría y seguridad · Tareas

Formato en [`docs/proceso.md`](../../docs/proceso.md). Requisitos: E00-T006 (auditoría) y E07-T001 (catálogo de permisos).

- [ ] **E09-T001** Tablas `security_rules`, `security_alerts` y `override_tokens` + migración + reglas por defecto.
- [ ] **E09-T002** `IManagerOverride` (emitir y consumir) + `RequirePermissionOrOverride`. Depende de T001.
  - Cumple: BR-SEG-01 y 02, AC-SEG-03.
- [ ] **E09-T003** `ISecurityAlerts` + avisos + `SecurityAlertRaised`. Reemplazar las implementaciones provisionales de E00, E02 y E04. Depende de T001.
  - Cumple: BR-SEG-03 y 04.
- [ ] **E09-T004** [P] Disparadores de cancelaciones por turno y de diferencia de corte con `ISecurityRules`. Depende de T003.
  - Cumple: AC-SEG-05 y 06.
- [ ] **E09-T005** API de alertas y reglas. Depende de T003.
  - Cumple: BR-SEG-05, AC-SEG-07.
- [ ] **E09-T006** API de auditoría con búsqueda sin acentos, detalle y exportación. Depende de T001.
  - Cumple: BR-SEG-06 y 07, AC-SEG-08 a 10.
- [ ] **E09-T007** `ManagerPinDialog` compartido. Depende de T002.
  - Comparar contra `img/03`. Cumple: AC-SEG-01 y 02.
- [ ] **E09-T008** `SecurityPage`. Depende de T005.
  - Comparar contra `img/02`.
- [ ] **E09-T009** `AuditPage` con panel de detalle. Depende de T006.
  - Comparar contra `img/01` y `04`.
- [ ] **E09-T010** Recorrido manual de AC-SEG-01 a 10 y contestar P-01 y P-02.
