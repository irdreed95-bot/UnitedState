#!/usr/bin/env bash
set -euo pipefail

BASE="https://raw.githubusercontent.com"
mkdir -p assets/external/city assets/external/vehicles assets/external/character

download() {
  local url="$1"
  local out="$2"
  echo "Downloading $(basename "$out")"
  curl -fsSL --retry 3 --retry-delay 2 "$url" -o "$out"
  test -s "$out"
}

# Kenney assets are CC0. The mirrored GLB files below come from a public asset
# library that identifies the original source as Kenney and preserves the CC0 license.
download "$BASE/Hidencod/tge-assets/main/packs/city-kit-commercial/building-a.glb" assets/external/city/commercial_a.glb
download "$BASE/Hidencod/tge-assets/main/packs/city-kit-commercial/building-b.glb" assets/external/city/commercial_b.glb
download "$BASE/Hidencod/tge-assets/main/packs/city-kit-commercial/building-skyscraper-a.glb" assets/external/city/skyscraper_a.glb
download "$BASE/Hidencod/tge-assets/main/packs/city-kit-commercial/building-skyscraper-b.glb" assets/external/city/skyscraper_b.glb
download "$BASE/Hidencod/tge-assets/main/packs/city-kit-commercial/building-skyscraper-c.glb" assets/external/city/skyscraper_c.glb
download "$BASE/Hidencod/tge-assets/main/packs/city-kit-suburban/building-type-a.glb" assets/external/city/suburban_a.glb
download "$BASE/Hidencod/tge-assets/main/packs/city-kit-suburban/building-type-b.glb" assets/external/city/suburban_b.glb
download "$BASE/Hidencod/tge-assets/main/packs/city-kit-suburban/building-type-c.glb" assets/external/city/suburban_c.glb
download "$BASE/Hidencod/tge-assets/main/packs/car-kit/sedan.glb" assets/external/vehicles/sedan.glb
download "$BASE/Hidencod/tge-assets/main/packs/car-kit/suv.glb" assets/external/vehicles/suv.glb
download "$BASE/Hidencod/tge-assets/main/packs/car-kit/taxi.glb" assets/external/vehicles/taxi.glb
download "$BASE/Hidencod/tge-assets/main/packs/car-kit/police.glb" assets/external/vehicles/police.glb
download "$BASE/programasweights/avatar/main/public/assets/character.glb" assets/external/character/citizen.glb

cat > assets/external/ASSET_SOURCES.txt <<'EOF'
Corrupt State RP external asset manifest

Kenney City Kit Commercial / Suburban / Car Kit
Original license: CC0 1.0
Source: https://kenney.nl/assets/city-kit-commercial
Source: https://kenney.nl/assets/city-kit-suburban
Source: https://kenney.nl/assets/car-kit

Quaternius Universal Base Characters
Original license: CC0 1.0
Source: https://quaternius.com/packs/universalbasecharacters.html

Build-time mirrors:
https://github.com/Hidencod/tge-assets
https://github.com/programasweights/avatar

The game incorporates these assets into the compiled game and does not expose the source packs as a standalone download.
EOF

echo "External asset bootstrap complete:"
find assets/external -type f -printf '%p %s bytes\n' | sort
