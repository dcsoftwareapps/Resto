# Resto

Sistema modular para restaurantes: punto de venta, inventario con recetas, compras, cocina, caja y facturación. Tiene app de escritorio, apps móviles para meseros y para chef y compras, y una pantalla de cocina.

Este repositorio se construye con **spec driven development**: primero se escribe *qué* se va a construir (spec), luego *cómo* (plan) y después la lista de tareas. El código se escribe tarea por tarea, a mano o con cualquier IA (Claude, Codex, Copilot, Cursor…), siempre siguiendo la spec.

## Por dónde empezar

| Si quieres… | Abre |
|---|---|
| Entender las reglas del proyecto (stack, convenciones, qué nunca se hace) | [`docs/constitucion.md`](docs/constitucion.md) |
| Ver todos los epics y en qué orden van | [`docs/epics.md`](docs/epics.md) |
| Ver qué interfaces y eventos comparten los epics | [`docs/contratos.md`](docs/contratos.md) |
| Ver todas las tablas y quién es dueño de cada una | [`docs/modelo-datos.md`](docs/modelo-datos.md) |
| Saber dónde quedó cada cosa que se pidió (requisitos, Soft Restaurant, pendientes) | [`docs/trazabilidad.md`](docs/trazabilidad.md) |
| Saber cómo trabajamos con specs, ramas y PRs | [`docs/proceso.md`](docs/proceso.md) |
| Ver un epic completo (el más detallado es Inventario) | [`specs/E04-escritorio-inventario/`](specs/E04-escritorio-inventario/) |
| Crear la spec de un epic nuevo | [`docs/plantillas/`](docs/plantillas/) |
| Ver el diseño aprobado (demo clickeable) | [`docs/demo/resto-demo.html`](docs/demo/resto-demo.html), ábrelo en el navegador |
| Darle instrucciones a una IA | [`AGENTS.md`](AGENTS.md) (Codex y la mayoría de agentes lo leen solos) |

## Estructura

```
Resto/
├─ AGENTS.md              → instrucciones para cualquier IA que trabaje en el repo
├─ CLAUDE.md              → apunta a AGENTS.md (Claude Code lo lee solo)
├─ docs/
│  ├─ constitucion.md     → reglas que no se negocian
│  ├─ epics.md            → mapa de epics, dependencias y reparto
│  ├─ contratos.md        → interfaces y eventos entre epics
│  ├─ modelo-datos.md     → todas las tablas y su dueño
│  ├─ trazabilidad.md     → de cada requisito al epic donde quedó
│  ├─ proceso.md          → cómo trabajamos
│  ├─ plantillas/         → spec.md, plan.md y tasks.md vacíos
│  └─ demo/               → demo clickeable aprobado (referencia visual)
├─ specs/
│  ├─ E00-plataforma-fundaciones/
│  ├─ …                   → un folder por epic, E00 a E21
│  └─ E04-escritorio-inventario/
│     ├─ spec.md          → qué y por qué (historias, reglas, criterios de aceptación)
│     ├─ plan.md          → cómo (proyectos, clases, endpoints, pruebas)
│     ├─ tasks.md         → tareas numeradas, con dueño y casilla
│     └─ img/             → capturas de cada pantalla
└─ src/ y tests/          → se crean en el epic E00 (Fundaciones)
```

## Estado

- [x] Constitución, mapa de epics, contratos, modelo de datos y proceso
- [x] Specs, planes y tareas de los 22 epics (E00 a E21), con capturas del demo
- [ ] Reunión de reparto: escribir responsables en `docs/epics.md`
- [ ] Contestar las preguntas abiertas que bloquean la ola 1 (ver abajo)
- [ ] E00 Fundaciones: solución .NET, base de datos, login, auditoría

## Preguntas que hay que contestar pronto

Están en la sección 8 de cada spec. Estas son las que frenan el arranque:

| Dónde | Pregunta |
|---|---|
| E00 P-01 | ¿Dónde se hospeda (Azure u otro)? |
| E00 P-02 | ¿Firebase para notificaciones *push*? |
| E00 P-04 | ¿Proveedor de correo? |
| E00 P-05 | ¿Quién redacta el aviso de privacidad y los términos? |
| E21 P-05 | ¿Qué dominio usamos? (`resto.mx` es provisional) |
| E03 P-01 | ¿WhatsApp Business API para mandar el corte? |
| E02 P-01, E08 P-01, E14 P-04 | Temas fiscales: IEPS en paquetes, IVA de la franja fronteriza, claves del SAT y propina. **Con contador.** |
| E14 P-01, E16 P-01, E18 P-01, E21 P-01 | Proveedores: PAC, terminal, integrador de delivery y procesador de suscripciones |
