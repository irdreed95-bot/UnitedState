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
