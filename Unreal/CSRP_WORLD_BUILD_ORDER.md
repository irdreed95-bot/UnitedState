# Corrupt State RP — World Build Order

This is the production order for the Unreal world. The uploaded PDF remains the gameplay/world-layout source of truth.

## Phase 1 — World foundation
1. Create `/Game/Maps/CSRP_Main` from the UE5 Open World/World Partition workflow.
2. Compile the `CorruptStateRP` C++ module.
3. Place exactly one `CSRPWorldBootstrap` actor in the map.
4. Keep the server authoritative: Citizen Program interactions spawn on the server only.

## Phase 2 — City structure
Build the districts from `CSRPCityLayout`:
- Downtown/commercial
- Government/civil services
- Banking
- Rich residential
- Poor residential
- Gang district
- Coast
- Port
- Forest
- Mountains
- Military base
- Desert prison

## Phase 3 — Realistic environment assets
Use modular, license-cleared assets. Prefer CC0/public-domain sources; record CC BY attribution when required.
Do not copy GTA/FiveM/Roblox assets or unknown-license packs.

Prioritize:
- roads/sidewalks
- government buildings
- bank
- police station
- hospital
- court
- university
- driving school
- dealership
- mechanic/garage
- residential buildings
- port/coastal structures
- military/prison structures
- Arabic signage

## Phase 4 — Performance
- Use World Partition for large-world streaming.
- Use HLOD for distant scenery.
- Generate collision and LODs.
- Reuse modular materials/meshes.
- Avoid importing a giant monolithic city mesh.
- Keep mobile performance as a first-class constraint.

## Phase 5 — Gameplay integration
Connect the physical locations to:
- Citizen Program
- jobs
- government factions
- police/EMS/army systems
- vehicles
- gang territories
- shops/services

Do not mark a phase complete until it has been actually built/tested in Unreal.
