# Corrupt State RP — Asset Library Register

Status: initial research register; no external asset files have been downloaded or imported yet.
Target: Unity 6.0.63f1, Android, URP/mobile-first.
Quality target: realistic / PBR. Low-poly packs are excluded from final production assets.

## Approved constraints
- Free assets only.
- Commercial use inside the game must be explicitly permitted by the asset's license.
- Record the exact asset page, creator, license, source URL, file format, and download date for every selected asset.
- Do not use ripped GTA/FiveM/game assets, unclear licenses, or assets marked personal-use-only.
- Do not treat a marketplace's "free" filter as proof of commercial rights; verify each asset's own license.
- Do not commit large binary asset packs to Git unless intentionally approved; prefer a documented acquisition/import process and Git LFS only if configured.

## Initial source register

| Source | Intended use | License evidence | Current decision |
|---|---|---|---|
| Poly Haven — https://polyhaven.com/ | PBR models, environment props, rocks, vegetation, HDRIs and surface detail | All assets are CC0; commercial use and redistribution are permitted without attribution. https://polyhaven.com/license | Approved source; select individual assets by suitability and mobile cost. |
| ambientCG — https://ambientcg.com/ | PBR materials for asphalt, concrete, metal, walls, floors, terrain and interiors | CC0 1.0; commercial use and inclusion of raw files in games are permitted. https://docs.ambientcg.com/license/ | Approved source for materials; not a source for a complete city. |
| CGAxis Free — https://cgaxis.com/free/ | Selected PBR textures and realistic 3D models | Source states royalty-free commercial use, no attribution; free account required. Review the current license attached to each download. | Candidate source; per-item license verification required. |

## Quality / import gate (must pass before an asset is accepted)
1. License: commercial use allowed; source page and license evidence archived.
2. Visual: realistic PBR quality; no stylized/low-poly asset accepted as final.
3. Format: prefer FBX or GLB/ glTF; verify scale, orientation, materials and texture paths.
4. Unity: import into the project's actual Unity 6 version; resolve shader/material conversion for URP.
5. Android: inspect triangle count, material count, texture dimensions, compression, memory use and draw calls.
6. Runtime: add appropriate colliders/LODs where needed; test in an Android build on target hardware.
7. Provenance: record creator, original asset URL, license, attribution text if required, and any modifications.

## PDF-driven asset backlog

### World / city
- Downtown skyline and modular realistic commercial buildings
- Rich residential villas and apartments
- Poor residential blocks and gang neighborhood
- Arabic street signs and street-name boards: Baghdad, Riyadh, Abu Dhabi, Kuwait, Doha, Manama, Muscat, Sana'a, Damascus
- Roads, highways, coastal and mountain roads, sidewalks, curbs, road markings, streetlights, traffic signs and barriers
- Shops, offices, parking areas, street furniture and construction props

### Vehicles
- Civilian cars: economy, used-car market and luxury dealership
- Police cars, ambulance, fire trucks, taxi, buses, delivery vans and heavy trucks
- Boats, port vessels, airport ground vehicles and military vehicles
- Each vehicle requires a separate check for interior, wheels, rigging, LODs and mobile performance

### Characters / animation
- Realistic rigged male/female civilian characters and clothing variants
- Police, medical, fire/rescue, military, prison staff and job-specific outfits
- Mixamo-compatible skeleton/animation workflow only after reviewing the applicable Adobe terms
- Face/body assets must be optimized for Android; do not use a single high-poly character as a crowd solution

### Institutions / interiors
- Municipality, police station (two floors), fire/rescue station, hospital, bank, court and basement cells
- University, pharmacy, restaurants, barbershop, driving school, RP law school, tattoo shop and media buildings
- Airport terminal, port cranes/containers/ships, dealership showrooms and casino/theatre/stadium
- Prison: three floors, guard towers, cells, separate women's wing
- Military base: gates, hospital, armory, training yards and helicopter landing area

### Natural / remote areas
- Coast, beach, sea, campsite and coastal structures
- Forest, river, trees, flowers, birds and rural props
- Mountain, cliffs, rocky plateaus and military access roads
- Desert terrain, dry vegetation, rocks and prison surroundings

## Known exclusions / caution
- The 129-model Low Poly City Kit (https://github.com/kayquer/kit-cidade-lowpoly) is CC0 and mobile-friendly, but is explicitly low-poly and therefore NOT approved as final city art. It may only be used as a temporary blockout if separately approved.
- Any free Sketchfab / CGTrader / TurboSquid / Unity Asset Store item must be checked individually. A free download does not automatically mean commercial use is allowed.
- A GLB/FBX file extension does not guarantee Unity 6 or Android compatibility.

## Progress
- [x] Initial source license review: Poly Haven, ambientCG.
- [x] Initial candidate source review: CGAxis Free (per-download license check remains).
- [ ] Select and record exact realistic building assets.
- [ ] Select and record exact road / street-prop assets.
- [ ] Select and record exact civilian and emergency vehicle assets.
- [ ] Select and record character / clothing / animation assets.
- [ ] Select and record institution interiors and specialized props.
- [ ] Import and validate accepted assets in Unity 6 URP.
- [ ] Run Android performance and visual checks.


## Additional asset leads (screened, not yet approved for final use)

| Lead | What it offers | License / caveat | Decision |
|---|---|---|---|
| Ara 3D Gen — https://github.com/olatunde85/3d-gen | Open-source workflow for generating rigged, animated human characters and exporting GLB/FBX/OBJ; project describes CC0 base assets and commercial exports. | Review the current repository license, generated-content terms, clothing/texture provenance, and actual character quality before adopting. | Candidate NPC pipeline; not yet validated in Unity or on Android. |
| ModKit — https://github.com/JaronKBragg7337/asset-pack-ue-threejs-blender-unity | 38 modular building and prop pieces in FBX/GLB/Blend. | Source states CC0 and commercial use; modular kit geometry is a construction base, not a finished photorealistic city. | Optional architectural blockout / kitbash only; not approved as final city art. |
| Grab3D Police Station — https://grab3d.com/models/buildings/police-station/ | Single police-station exterior in GLB/FBX/BLEND/OBJ; page lists 17,736 triangles and 2K textures. | Page states CC0 commercial use. Visual style and completeness (especially interior) still require inspection. | Candidate for visual review; do not assume it fulfills the detailed two-floor police facility in the PDF. |

## Explicit gaps after this pass
- No verified free photorealistic full-city kit has been identified.
- No verified complete set of realistic civilian/emergency vehicles has been identified.
- No complete realistic interior packs have yet been verified for the hospital, court, bank, university, prison and military base.
- The character leads found so far are either a generator workflow or individual characters; neither is a validated crowd/NPC library for this project.
- The low-poly city packs and hand-painted low/mid-poly characters remain excluded from the final realism target.


## Second research pass — catalogued leads and quality decisions

| Asset / catalog | License evidence | Quality / mobile notes | Decision |
|---|---|---|---|
| Poly Haven models — https://polyhaven.com/models | Poly Haven states its library is CC0; commercial use is allowed without attribution. | Models are generally realistic / scanned / PBR, but many are high-poly and some are props rather than complete buildings. Select suitable texture sizes and inspect geometry for Android. | Primary realistic source. Select individual models only after triangle/material/texture inspection. |
| 3D Valley buildings catalog — https://3dvalley.com/3d-models/buildings | Catalog describes listed models as CC0 and links to source libraries. | Mixed styles and geometry budgets; some assets have tens of thousands of triangles or more. Confirm original creator/source license on every model page. | Discovery index only; not blanket approval. |
| 3DAssets.dev realistic buildings — https://3dassets.dev/assets?category=buildings&style=realistic | Site states listed GLB assets are free for commercial use; verify individual asset/source terms before use. | Search catalog reports self-contained GLB files under 25 MB. File size alone does not establish acceptable draw calls, textures or geometry for Android. | Candidate discovery index; individual asset review required. |
| Grab3D Police Station — https://grab3d.com/models/buildings/police-station/ | Page states CC0 1.0, commercial use allowed. | Listed as part of a “Lightweight Cozy Buildings Kit”; despite being a two-storey police station and 17,736 triangles, the style is stylized, not photorealistic. | Excluded from final realistic-art set. Temporary layout reference only if separately approved. |
| Grab3D Compact Car — https://grab3d.com/models/vehicles/compact-car/ | Page states CC0 1.0, commercial use allowed. | Part of a lightweight cozy cars kit; stylized, 17,338 triangles and 2K textures. | Excluded from final realistic vehicle set. |
| Grab3D 1990s Compact Car — https://grab3d.com/models/vehicles/1990s-compact-car/ | Page states CC0 1.0, commercial use allowed. | Explicitly part of a lightweight retro/cozy kit; stylized appearance. | Excluded from final realistic vehicle set. |
| FPS Buildings Environment Kit — https://github.com/petroulacl/fps-buildings-env-kit | Repository claims CC0 / public-domain sources. | Listed Kenney city/building pieces and Quaternius vegetation are stylized/low-poly; it also contains HDRIs and PBR ground materials. Audit original pack licenses and provenance before copying. | Not a final realistic city solution; possible temporary blockout/material reference only. |

### Second-pass selection rules
- A license pass never overrides the visual-quality pass. CC0 stylized models remain excluded from the final realistic art target.
- Aggregators are discovery tools, not license authorities. Store the original creator's license URL and asset page for every accepted download.
- For Android, prefer modular architecture with shared atlases/materials, LODs, occlusion/visibility management and compressed textures; high-poly scanned hero assets must be simplified or baked before broad use.
- No vehicle is accepted as drivable until wheels, pivots, steering, suspension, collision, interior visibility and controller integration are checked.
- No institution is accepted from an exterior-only model when the PDF requires detailed accessible interiors and multi-floor gameplay.

## Second-pass status
- [x] Record realistic-source catalogues for building discovery.
- [x] Classify Grab3D police station and cars as stylized and exclude them from final realism.
- [x] Record a CC0 environment-kit repository but exclude its low-poly building/vegetation packs from final art.
- [ ] Acquire exact photorealistic building assets with original license pages.
- [ ] Acquire realistic drivable civilian and emergency vehicles with rigged moving parts.
- [ ] Acquire complete realistic institution interiors.
- [ ] Import, optimize and validate in the actual Unity project and Android build.
