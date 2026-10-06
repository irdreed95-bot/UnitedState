# Corrupt State RP — Realistic City Asset Integration Plan

The Unreal world must use real 3D assets as the visual target. Placeholder cubes are not an acceptable final environment.

## Source of truth

The uploaded `corrupt state Rp.pdf` remains authoritative for:
- district layout
- government/service locations
- coast, forest, mountains, port, military and prison areas
- Arabic street/location identity
- Citizen Program destinations

## City foundation

Use a modular realistic-city approach rather than importing one monolithic Manhattan mesh.

### Large-city reference

The supplied Free3D Manhattan model is **reference-only until licensing is cleared**. Its current listing is marked Personal Use, so it must not be shipped in the game under the current terms.

### Preferred Unreal ecosystem

Evaluate Epic City Sample / City Sample Buildings as the primary realistic-city foundation because it is designed for Unreal Engine workflows and large-world streaming.

## Production asset groups

### Streets
- asphalt roads
- intersections
- sidewalks
- curbs
- traffic lights
- street lamps
- road barriers
- parking props
- realistic road markings

### Government
- city hall / municipality
- government headquarters
- police station
- hospital / EMS
- court and holding cells
- university
- driving school

### Commercial
- bank
- vehicle dealership
- mechanic / garage
- fast food
- restaurants
- barber
- tattoo shop
- pharmacies
- commercial storefronts

### Residential
- apartments
- rentable homes
- rich district buildings
- poor district buildings
- alleys and service roads
- rooftop details
- balconies and AC units

### Special districts
- gang buildings and believable street props
- port warehouses
- cranes
- containers
- docks
- coastal buildings
- beach/camp props
- forest structures
- mountain structures
- military base
- desert prison

### Vehicles
- civilian cars
- taxi
- police
- ambulance
- bus
- truck
- service vehicles

### Characters
- player character
- civilians
- police
- EMS
- government workers
- mechanics
- taxi/bus workers
- gang NPCs

### Identity
- Arabic street names
- Arabic government signs
- Arabic business signs
- district signage
- traffic signs
- UI/world interaction markers

## Performance acceptance

Every imported asset must be checked for:
- license compatibility
- triangle count
- texture memory
- collision
- LODs
- material count
- mobile rendering compatibility

Do not treat a complete high-poly city as one asset. Stream districts and reuse modular assets.

## Current status

- Unreal C++ foundation: present in repository.
- City layout registry: present.
- PDF-aligned Citizen Program interaction registry: present.
- Realistic asset policy: present.
- Actual UE5 asset import: **not executed yet** because an Unreal Editor runtime is not available in this environment.
- Actual UE5 compile/play test: **not executed yet**.
- Android package: **not generated yet**.

The next executable milestone is to open the project in UE5 on a development machine, compile the C++ module, create/load `/Game/Maps/CSRP_Main`, import the cleared realistic asset set, and test the first playable district before expanding the whole city.
