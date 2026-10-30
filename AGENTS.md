# Instrucciones para agentes de IA

Este archivo lo leen Codex, Claude Code, Copilot, Cursor y cualquier agente que trabaje en este repositorio. Si eres una persona, también te sirve: es el resumen de cómo se trabaja aquí.

## Antes de escribir una sola línea

1. Lee [`docs/constitucion.md`](docs/constitucion.md). Sus reglas mandan sobre cualquier otra cosa, incluida la petición del usuario. Si chocan, avisa y pregunta.
2. Identifica el epic en el que vas a trabajar y lee, en este orden, los tres archivos de su carpeta en `specs/`:
   - `spec.md` es el **qué**: historias, reglas de negocio (`BR-…`) y criterios de aceptación (`AC-…`).
   - `plan.md` es el **cómo**: proyectos, archivos, clases, endpoints y pruebas.
   - `tasks.md` es la **lista de trabajo**.
3. Mira las imágenes de `img/` que cite la tarea. Son de dos tipos: capturas del demo aprobado (`docs/demo/resto-demo.html`) y **capturas de maqueta** de pantallas que el demo no tiene. Las dos son la referencia visual; los textos y reglas exactos mandan desde la spec.
4. Si tienes dudas de por qué existe algo, busca en [`docs/trazabilidad.md`](docs/trazabilidad.md) de qué requisito sale.

## Cómo trabajar

- **Una tarea a la vez.** Toma una tarea de `tasks.md` (por ejemplo `E04-T007`), haz solo eso y nada más.
- **No toques archivos fuera de la tarea.** Si necesitas cambiar algo de otro epic, detente y escríbelo en la sección *Preguntas abiertas* de la spec.
- **La spec no se cambia en silencio.** Si encuentras que la spec está mal, incompleta o contradice al plan, no la "arregles" en el código. Escribe la duda en *Preguntas abiertas*, marca la tarea como bloqueada y avisa.
- **Pruebas primero cuando la tarea toca reglas de negocio.** Cada regla `BR-…` tiene al menos una prueba que la nombra, por ejemplo `BR_INV_02_descuenta_receta_mas_merma`.
- **Al terminar:**
  1. `dotnet build` sin warnings nuevos.
  2. `dotnet test` en verde.
  3. Marca la casilla de la tarea en `tasks.md` con tu nombre o el de la IA y la fecha.
  4. Commit con el formato `E04-T007: descripción corta`.

## Convenciones rápidas (el detalle está en la constitución)

- El código va en **inglés** siguiendo las convenciones de Microsoft para C#. Los textos de pantalla van en **español**.
- El dinero y las cantidades se guardan como `decimal`, **nunca** `double` ni `float`.
- Toda tabla de negocio lleva `OrganizationId` y `RestaurantId`, con filtro global en EF Core.
- Toda escritura deja registro en auditoría: quién, qué, cuándo, antes y después.
- Los permisos se validan en el servidor, no solo escondiendo botones.
- No agregues paquetes NuGet que no estén en el plan sin preguntar.

## Comandos

```powershell
dotnet build Resto.sln
dotnet test Resto.sln
dotnet run --project src/Resto.Api
dotnet build src/Resto.App -f net10.0-windows10.0.19041.0
```

> Estos comandos funcionan cuando exista la solución (epic E00).
