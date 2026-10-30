# E07 · Escritorio → Usuarios y roles · Plan técnico

## 1. Modelo de datos

E00 crea la versión mínima de estas tablas. E07 las completa.

| Tabla | Columnas |
|---|---|
| `users` | `Id`, `OrganizationId`, `RestaurantId`, `FullName`, `Initials`, `RoleId`, `PinHash`, `Email?`, `PasswordHash?`, `InvitationToken?`, `InvitationExpiresAt?`, `IsActive`, `LastAccessAt`, `SessionVersion` |
| `roles` | `Id`, `RestaurantId`, `Name`, `IsSystem` (Administrador), `PermissionsVersion` |
| `role_permissions` | `RoleId`, `Permission` (llave del catálogo) |

- **PIN único:** se valida en el servicio comparando contra los hashes de los usuarios activos. No se puede indexar un hash con sal, pero son pocos usuarios por restaurante.
- `SessionVersion` se incrementa al desactivar a alguien o al cerrar sus sesiones. El token lleva la versión.

## 2. Domain (`Resto.Domain/Users/`)

| Archivo | Contenido | Reglas |
|---|---|---|
| `Permissions.cs` (en Contracts) | Constantes del catálogo + metadatos (módulo y texto en pantalla) | Catálogo de la spec |
| `PinPolicy.cs` | Formato y PINs prohibidos | BR-USR-02 |
| `DefaultRoles.cs` | Los 6 roles y sus permisos | BR-USR-09 |
| `UserRules.cs` | Último administrador, desactivarse a sí mismo y borrar rol con usuarios | BR-USR-04, 05 y 08 |

## 3. Validación de versiones (en el middleware de E00)

En cada petición se comparan `token.permVersion` con `roles.PermissionsVersion` y `token.sessionVersion` con `users.SessionVersion`. Las versiones se guardan en caché de memoria por 30 segundos. Si no coinciden, se responde 401 con `X-Token-Stale: 1` y el cliente se renueva (BR-USR-07).

## 4. API (`/api/users`, `/api/roles`)

| Método y ruta | Permiso |
|---|---|
| `GET /api/users` · `POST /api/users` · `PUT /api/users/{id}` | `users.manage` |
| `POST /api/users/{id}/pin` `{ Pin }` | `users.manage` |
| `POST /api/users/{id}/deactivate` · `…/activate` · `…/revoke-sessions` · `…/resend-invitation` | `users.manage` |
| `POST /api/auth/accept-invitation` `{ Token, Password }` | público |
| `GET /api/roles` · `POST /api/roles` `{ Name, CopyFromRoleId? }` · `PUT /api/roles/{id}` · `DELETE /api/roles/{id}` | `users.manage` |
| `PUT /api/roles/{id}/permissions/{key}` `{ Granted }` | `users.manage` |
| `GET /api/permissions` (catálogo con textos) | `users.manage` |

## 5. Pantallas (`Resto.UI/Features/Users/`)

| Componente | Captura |
|---|---|
| `UsersPage.razor` (`/usuarios`) con pestañas | 01, 02 |
| `UsersTable.razor` | 01 |
| `PermissionMatrix.razor` (agrupada por módulo) | 02 |
| `UserFormDialog.razor` (alta y edición, con "Va a poder…") | 03 |
| `RoleFormDialog.razor` | 04 |

## 6. Pruebas

| Proyecto | Qué cubre |
|---|---|
| Domain.Tests | `PinPolicy`, `UserRules` y `DefaultRoles` = tabla de la spec |
| Api.Tests | AC-USR-01 a 09; prueba que **todo endpoint** declara un permiso que existe en el catálogo (recorre los endpoints registrados) |
| UI.Tests | Matriz con Administrador bloqueado |

## 7. Paquetes

Ninguno.
