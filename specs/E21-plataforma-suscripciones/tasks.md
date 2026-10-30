# E21 · Plataforma → Suscripciones y planes · Tareas

Formato en [`docs/proceso.md`](../../docs/proceso.md). Requisitos: E00, E07 y E08. La factura de la licencia necesita E14.. El candado por módulo (`IModuleGate`) ya existe desde E00; aquí solo se conecta con los planes.

- [ ] **E21-T001** Esquema `platform` + `plans` con los 3 planes de la spec.
  - Cumple: P-04 por confirmar.
- [ ] **E21-T002** [P] Precios y estados en Domain (sucursales, ciclo, prorrateo y transiciones).
  - Cumple: AC-SUS-03.
- [ ] **E21-T005** Filtro de solo lectura + `ReadOnlyBanner`. Depende de T001.
  - Cumple: AC-SUS-02.
- [ ] **E21-T006** Registro público + creación de organización, sucursal, administrador, roles y configuración. Depende de T001.
  - Comparar contra `img/01`. Cumple: AC-SUS-01 y 09.
- [ ] **E21-T007** `OnboardingWizard` con menús de ejemplo por giro. Depende de T006, E04 (importar) y E05.
- [ ] **E21-T008** `ISubscriptionProcessor` + falso + *webhooks* + jobs de recordatorio y estado. Depende de T002.
  - Cumple: BR-SUS-01 a 04 y 07, AC-SUS-05 y 06.
- [ ] **E21-T009** Adaptador real del procesador. Depende de T008 y P-01.
- [ ] **E21-T010** Factura de cada cobro con E14 (emisor Resto). Depende de T008 y E14-T006.
  - Cumple: BR-SUS-06.
- [ ] **E21-T011** `MyPlanTab`, `PlanComparison`, `ChargesHistory`, `LockedModulePage` y prueba de módulo por 14 días. Depende de T008.
  - Comparar contra `img/02` y `03`.
- [ ] **E21-T012** Consola interna `Resto.Admin` con modo soporte. Depende de T008.
  - Comparar contra `img/04`. Cumple: BR-SUS-08, AC-SUS-07.
- [ ] **E21-T012b** `HelpPanel` con centro de ayuda (artículos en Markdown dentro de `Resto.Web/help/`), enlace de WhatsApp y estado del servicio. Depende de E00-T015 y E00-T024c.
  - Comparar contra `img/05`. Cumple: AC-SUS-08.
- [ ] **E21-T013** Recorrido completo en el modo de pruebas del procesador y contestar P-01 a P-05.
