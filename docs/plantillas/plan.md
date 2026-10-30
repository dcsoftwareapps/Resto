# Exx · Superficie → Módulo · Plan técnico

> Cómo se construye lo que pide `spec.md`. Si algo aquí contradice la spec, gana la spec. Ejemplo: [`specs/E04-escritorio-inventario/plan.md`](../../specs/E04-escritorio-inventario/plan.md).

## 1. Resumen
Un diagrama (mermaid) de qué proyectos y servicios participan.

## 2. Modelo de datos
Una tabla por entidad: propiedad, tipo C#, columna y notas. Recuerda `RestaurantId`, `decimal` y nada de borrado físico.

## 3. Reglas en Domain
| Archivo | Contenido | Reglas BR |
|---|---|---|

## 4. Persistencia y concurrencia
Configuraciones EF, migración y cómo se evitan los conflictos.

## 5. API
| Método y ruta | Permiso | Cuerpo / query | Respuesta |
|---|---|---|---|

## 6. Tiempo real
| Evento | Payload | Cuándo |
|---|---|---|

## 7. Pantallas
| Componente | Responsabilidad | Captura |
|---|---|---|

## 8. Contrato con otros epics
Las interfaces que otros llaman o que este epic llama. **No se cambian sin avisar.**

## 9. Pruebas
Qué cubre cada proyecto de pruebas y qué es manual.

## 10. Paquetes
Paquetes NuGet nuevos y por qué. Lo ideal es que sea "ninguno".
