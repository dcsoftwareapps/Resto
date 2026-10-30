# E09 · Escritorio → Auditoría y seguridad · Plan técnico

## 1. Modelo de datos

| Tabla | Columnas |
|---|---|
| `security_rules` | `RestaurantId` (PK), `FreeBillPrints`, `DiscountAlertPercent`, `CancellationsPerShiftAlert`, `AlertOnServedCancellation`, `CashDifferenceAlert` |
| `security_alerts` | `Id`, `RestaurantId`, `Severity` (`High`, `Medium`), `Kind`, `Title`, `Detail`, `UserId` (involucrado), `EntityType`, `EntityId`, `OccurredAt`, `ReviewedBy`, `ReviewedAt` |
| `override_tokens` | `Token` (hash), `RestaurantId`, `RequestedBy`, `AuthorizedBy`, `Permission`, `EntityType`, `EntityId`, `ExpiresAt`, `UsedAt` |

`audit_entries` es de E00. Este epic solo agrega índices de búsqueda: `pg_trgm` sobre `unaccent(lower(Action || ' ' || Detail))`.

## 2. Contratos que ofrece

```csharp
public interface IManagerOverride   // E02, E03 y E07
{
    // Lo llama el endpoint /api/security/override cuando alguien escribe el PIN.
    Task<OverrideResult> IssueAsync(string pin, string permission, string entityType, Guid entityId, CancellationToken ct);
    // Lo llama el endpoint protegido: true si el token es válido para esta acción, y lo marca usado.
    Task<Guid?> ConsumeAsync(string token, string permission, string entityType, Guid entityId, CancellationToken ct);
}

public interface ISecurityAlerts    // E02 y E03. Para E00 y E04, E09 escucha PinLockedOut y StockChanged con Shortfall
{
    Task RaiseAsync(SecurityAlert alert, CancellationToken ct);
}

public interface ISecurityRules     // E02 y E03
{
    SecurityRulesDto Current { get; }
}
```

**Patrón en los endpoints de otros epics:** `RequirePermissionOrOverride(Permissions.PosDiscount)`. Si el usuario tiene el permiso, pasa. Si no, busca `OverrideToken` en el cuerpo y lo consume con `IManagerOverride.ConsumeAsync`.

## 3. API

| Método y ruta | Permiso |
|---|---|
| `POST /api/security/override` `{ Pin, Permission, EntityType, EntityId }` → `{ Token, AuthorizedByName }` | sesión |
| `GET /api/security/override/candidates?permission=` (nombres para el modal) | sesión |
| `GET /api/security/alerts?reviewed=` · `POST …/alerts/{id}/review` · `POST …/alerts/review-all` | `security.manage` |
| `GET/PUT /api/security/rules` | `security.manage` |
| `GET /api/audit?q=&module=&userId=&from=&to=&page=` · `GET /api/audit/{id}` · `GET /api/audit/export?…` | `audit.view` |

**Tiempo real:** `SecurityAlertRaised` al grupo `role:` de quienes tienen `security.manage`. El contador del menú se actualiza solo.

## 4. Pantallas

| Componente | Captura |
|---|---|
| `ManagerPinDialog.razor` (en `Resto.UI/Shared`, lo usan E02 y E03) | 03 |
| `SecurityPage.razor` (`/seguridad`) + `AlertList.razor` + `SecurityRulesForm.razor` | 02 |
| `AuditPage.razor` (`/auditoria`) + `AuditFilters.razor` + `AuditTable.razor` + `AuditDetailPanel.razor` | 01 |

## 5. Pruebas

| Proyecto | Qué cubre |
|---|---|
| Domain.Tests | Disparadores de BR-SEG-03 con sus umbrales |
| Api.Tests | AC-SEG-01 a 06 y 08 a 10; un solo uso y vencimiento del token; búsqueda sin acentos ("camaron" encuentra "Camarón") |
| UI.Tests | Modal de PIN (Enter, errores) |

## 6. Paquetes

Ninguno. La exportación a Excel usa `ClosedXML` (ya incluido en E04).
