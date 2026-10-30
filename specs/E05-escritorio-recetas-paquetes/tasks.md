# E05 · Escritorio → Recetas y paquetes · Tareas

Formato en [`docs/proceso.md`](../../docs/proceso.md). Requisitos: E00 y E04-T007 (insumos en base de datos).

- [ ] **E05-T001** [P] `RecipeCosting` (costo y margen).
  - Cumple: BR-REC-01 y 02, AC-REC-01 y 02.
- [ ] **E05-T002** [P] `PortionCalculator`.
  - Cumple: BR-REC-03, AC-REC-03 y caso "sin receta".
- [ ] **E05-T003** `MenuItem` (paquete, agotado hoy y canales) + `RecipeResolver`. Depende de T001.
  - Cumple: BR-REC-04, 07, 08 y 10, AC-REC-04 a 07.
- [ ] **E05-T004** Tablas + migración `AddMenu` + seed con los 15 platillos, 2 paquetes y 3 extras del demo. Depende de T003.
- [ ] **E05-T005** `IMenuCatalog` con caché e invalidación por `StockChanged`. Depende de T004.
  - Cumple: BR-REC-09.
- [ ] **E05-T006** API de platillos, recetas, componentes, agotado hoy, categorías y extras, con auditoría de precio. Depende de T004.
  - Cumple: BR-REC-06, BR-REC-05, AC-REC-08 a 11.
- [ ] **E05-T007** Subida y recorte de foto. Depende de T006 y E00 P-01.
- [ ] **E05-T008** `RecipesPage`, `MenuItemList` y `RecipeEditor`. Depende de T006.
  - Comparar contra `img/01` y `02`.
- [ ] **E05-T009** `BundleEditor`. Depende de T008.
  - Comparar contra `img/03`.
- [ ] **E05-T010** `MenuItemFormDialog` (con alérgenos y calorías), `MenuCategoriesPage` y `ExtrasPage`. Depende de T006.
  - Comparar contra `img/04` y `05`. Cumple: BR-REC-13, AC-REC-13.
- [ ] **E05-T010b** Sugerir traducción con `ITranslationService`. Depende de T010 y P-03.
  - Cumple: BR-REC-11, AC-REC-14.
- [ ] **E05-T010c** Importar menú y recetas de Excel + `ImportMenuDialog`. Depende de T006 y E04-T013c (mismo patrón).
  - Comparar contra `img/06`. Cumple: BR-REC-12, AC-REC-12.
- [ ] **E05-T011** Recorrido manual de AC-REC-01 a 14 y contestar P-01 a P-03.
