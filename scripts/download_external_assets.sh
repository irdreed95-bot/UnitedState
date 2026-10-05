#!/usr/bin/env bash
set -euo pipefail

BASE="https://raw.githubusercontent.com"
mkdir -p assets/external/city assets/external/vehicles assets/external/character assets/external/roads assets/external/nature assets/fonts

download() {
  local url="$1"
  local out="$2"
  echo "Downloading $(basename "$out")"
  if command -v curl >/dev/null 2>&1; then
    curl -fsSL --retry 3 --retry-delay 2 "$url" -o "$out"
  elif command -v wget >/dev/null 2>&1; then
    wget -q --tries=3 -O "$out" "$url"
  else
    echo "ERROR: neither curl nor wget is available in the Android build image." >&2
    exit 127
  fi
  test -s "$out"
}

# Kenney CC0 City Kit Commercial / Suburban.
for pair in "a commercial_a" "b commercial_b"; do
  set -- $pair
  download "$BASE/Hidencod/tge-assets/main/packs/city-kit-commercial/building-$1.glb" "assets/external/city/$2.glb"
done
for pair in "skyscraper-a skyscraper_a" "skyscraper-b skyscraper_b" "skyscraper-c skyscraper_c"; do
  set -- $pair
  download "$BASE/Hidencod/tge-assets/main/packs/city-kit-commercial/building-$1.glb" "assets/external/city/$2.glb"
done

for pair in "a suburban_a" "b suburban_b" "c suburban_c"; do
  set -- $pair
  download "$BASE/Hidencod/tge-assets/main/packs/city-kit-suburban/building-type-$1.glb" "assets/external/city/$2.glb"
done

# Kenney CC0 Car Kit - confirmed files.
download "$BASE/Hidencod/tge-assets/main/packs/car-kit/sedan.glb" assets/external/vehicles/sedan.glb
download "$BASE/Hidencod/tge-assets/main/packs/car-kit/suv.glb" assets/external/vehicles/suv.glb
download "$BASE/Hidencod/tge-assets/main/packs/car-kit/taxi.glb" assets/external/vehicles/taxi.glb
download "$BASE/Hidencod/tge-assets/main/packs/car-kit/police.glb" assets/external/vehicles/police.glb
download "$BASE/Hidencod/tge-assets/main/packs/city-kit-roads/road-straight.glb" assets/external/roads/road-straight.glb

# Quaternius Universal Base Characters, CC0.
download "$BASE/programasweights/avatar/main/public/assets/character.glb" assets/external/character/citizen.glb

# Noto Sans Arabic, SIL Open Font License.
download "https://raw.githubusercontent.com/notofonts/noto-fonts/main/unhinted/ttf/NotoSansArabic/NotoSansArabic-Regular.ttf" assets/fonts/NotoSansArabic-Regular.ttf

cat > assets/external/ASSET_SOURCES.txt <<'EOF'
Corrupt State RP external asset manifest

Kenney City Kit Commercial / Suburban / Car Kit
Original license: CC0 1.0
Official source: https://kenney.nl/assets/city-kit-commercial
Official source: https://kenney.nl/assets/city-kit-suburban
Official source: https://kenney.nl/assets/car-kit
Build-time mirror: https://github.com/Hidencod/tge-assets

Quaternius Universal Base Characters
Original license: CC0 1.0
Source: https://quaternius.com/packs/universalbasecharacters.html
Build-time mirror: https://github.com/programasweights/avatar

Noto Sans Arabic
License: SIL Open Font License
Source: https://github.com/notofonts/noto-fonts

All assets are incorporated into the compiled game; the project does not ship the source packs as standalone downloads.
EOF

echo "External asset bootstrap complete:"
find assets/external -type f -printf '%p %s bytes\n' | sort
