# E00 · Plataforma → Fundaciones · Tareas

Formato y reglas en [`docs/proceso.md`](../../docs/proceso.md). `[P]` = se puede hacer en paralelo.

> **E00 se termina completo antes de empezar otro epic** (orden en [`docs/epics.md`](../../docs/epics.md)). Los dos pueden trabajar en E00 a la vez: hay muchas tareas `[P]`. Antes de T020 y T024b hay que decidir E00 P-01 (hospedaje) y P-02 (*push*).

## Bloque A · Solución

- [ ] **E00-T001** Crear la solución con la plantilla *MAUI Blazor Hybrid and Web App* + Domain, Contracts, Infrastructure, Api, Printing y los 3 proyectos de pruebas. Agregar `Directory.Build.props` y `Directory.Packages.props`. Comprobar que `dotnet build` y `dotnet test` corren en verde.
- [ ] **E00-T002** [P] GitHub Actions: compilar y probar en cada PR.
- [ ] **E00-T003** [P] `tokens.css`, `base.css` y `components.css` con los tokens de la constitución §8 y las clases del demo (`.btn`, `.pill`, `.chip`, `.kpi`, `.tbl`, `.panel`, `.modal`, `.seg`, `.toggle`). Fuentes de Google incluidas localmente en `wwwroot/fonts`.
  - Comparar contra `img/01` y `03`.

## Bloque B · Datos y multi restaurante

- [ ] **E00-T004** `TenantEntity`, `AppDbContext` con filtro global por `RestaurantId`, interceptor de campos de creación y modificación, y tablas `organizations` y `restaurants`. Migración `Initial`. Depende de T001.
- [ ] **E00-T005** Prueba de aislamiento genérica (BR-PLT-05, AC-PLT-07) que se ejecuta para toda entidad `TenantEntity` futura. Depende de T004.
- [ ] **E00-T006** `IAuditLog` + tabla `audit_entries` + interceptor `[Audited]` con antes y después. Depende de T004.
  - Cumple: BR-PLT-06, AC-PLT-08.
- [ ] **E00-T007** [P] `IClock` con `BusinessDay`. Depende de T001.
  - Cumple: BR-PLT-11, AC-PLT-15.
- [ ] **E00-T008** `DevSeed` con los datos del demo: restaurante, 9 usuarios y 6 roles. Insumos y platillos los agregan E04 y E05 en su propio seed. Depende de T004.

## Bloque C · Acceso

- [ ] **E00-T009** Tablas mínimas `users`, `roles` y `role_permissions` + catálogo `Permissions` en Contracts + `ICurrentUser`. Depende de T004.
- [ ] **E00-T010** Login con correo y JWT + refresh. Depende de T009.
  - Cumple: BR-PLT-03.
- [ ] **E00-T011** Dispositivos: códigos de vinculación, registro, `X-Device-Token` y revocar. Depende de T009.
  - Cumple: BR-PLT-02, AC-PLT-01 a 03.
- [ ] **E00-T012** Login con PIN + bloqueo por intentos. Depende de T011.
  - Cumple: BR-PLT-01, AC-PLT-04 y 05. Al bloquear, publica el evento interno `PinLockedOut` y avisa al gerente con `INotificationService`. E09 lo convierte en alerta.
- [ ] **E00-T013** `.RequirePermission()` en endpoints + `NoAccessPage` + `<PermissionView>`. Depende de T009.
  - Cumple: AC-PLT-14.
- [ ] **E00-T013b** `Modules`, `IModuleGate`, `.RequireModule()` y `<ModuleView>` + candado en `NavRail`, leyendo `organization_modules` (por defecto todos activos; E21 los cambia según el plan). Antes en E21. Depende de T013.
  - Cumple: BR-SUS-05, AC-SUS-04.
- [ ] **E00-T013c** Prueba de que todos los endpoints declaran su permiso y su módulo (antes en E21). Depende de T013b.
  - Cumple: AC-SUS-02.
- [ ] **E00-T014** Pantallas `LoginPage`, `PairDevicePage`, `PinPadPage` y `LockScreen` + inactividad por tipo de dispositivo. Depende de T010 a T012.
  - Comparar contra `img/06`, `07` y `08`. Cumple: BR-PLT-04, AC-PLT-06.

## Bloque D · Estructura de pantallas

- [ ] **E00-T015** `MainLayout`, `NavRail` (filtrado por permiso, contadores y "Nuevo"), `TopBar` y `PageHeader`. Depende de T003 y T013.
  - Comparar contra `img/01` y `05`.
- [ ] **E00-T016** [P] `ToastHost`, `ModalHost` y `ThemeService`. Depende de T003.
  - Comparar contra `img/03` y `04`.
- [ ] **E00-T017** [P] `ErrorBoundaryView` con código de referencia (`TraceId`). Depende de T015.

## Bloque E · Tiempo real y avisos

- [ ] **E00-T018** `RestaurantHub` con grupos, `IRealtimePublisher` y `RealtimeConnection` con reconexión y `Resynced`. Depende de T010.
  - Cumple: BR-PLT-13, AC-PLT-16.
- [ ] **E00-T019** `INotificationService`, tabla `notifications`, `NotificationBell` y limpieza a 30 días. Depende de T018.
  - Cumple: BR-PLT-12. Comparar contra `img/02`.
- [ ] **E00-T019b** [P] `IMessageDelivery` con correo (SMTP o API) y plantillas HTML con el logo. WhatsApp queda como implementación vacía hasta E03-T012. Depende de T001.
- [ ] **E00-T020** *Push* con FCM: registrar el token del dispositivo, enviar solo si la app no está abierta, y banner dentro de la app. Depende de T019 y de P-02.
  - Cumple: AC-PLT-13.

## Bloque F · Sin conexión

- [ ] **E00-T021** Middleware de `Idempotency-Key` + tabla. Depende de T004.
  - Cumple: BR-PLT-07, AC-PLT-11.
- [ ] **E00-T022** `LocalDbContext` (SQLite), `ApiClient` con `outbox` y `[OfflineCapable]`, y `OutboxSyncService` con reintentos. Depende de T021.
  - Cumple: BR-PLT-08 y 09.
- [ ] **E00-T023** `ConnectivityBanner` y la leyenda "Requiere internet" para módulos que no funcionan sin conexión. Depende de T022.
  - Comparar contra `img/09`. Cumple: AC-PLT-12.
- [ ] **E00-T024** Folios por restaurante y folio provisional sin conexión. Depende de T022.
  - Cumple: BR-PLT-10, AC-PLT-09 y 10. Se termina de probar con E02.

## Bloque G · Operación y seguridad

- [ ] **E00-T024b** Ambientes `Staging` y `Production`, respaldos con recuperación a un minuto y job mensual `RestoreDrill`. Depende de T004 y P-01.
  - Cumple: BR-PLT-14, AC-PLT-17.
- [ ] **E00-T024c** [P] `/health`, OpenTelemetry y alertas al equipo. Depende de T002.
- [ ] **E00-T024d** [P] HTTPS, HSTS, CSP, `AddRateLimiter` y secretos fuera del código. Depende de T010.
  - Cumple: BR-PLT-15, AC-PLT-18.
- [ ] **E00-T024e** `LegalLinks` + páginas de aviso de privacidad y términos + `IPersonalDataEraser`. Depende de T014 y P-05.
  - Cumple: BR-PLT-16, AC-PLT-19.

## Bloque H · Cierre

- [ ] **E00-T025** Recorrido manual de AC-PLT-01 a 19 en Windows, Android y web.
- [ ] **E00-T026** Contestar P-01 a P-05.
