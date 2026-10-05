#!/usr/bin/env bash
set -euo pipefail

MODE="${1:---interactive}"

install_cli() {
  if command -v unity >/dev/null 2>&1; then
    echo "Unity CLI already installed:"
    unity --version || true
    return
  fi

  echo "Installing the official Unity CLI..."
  curl -fsSL https://unity.com/install.sh | bash
  export PATH="$HOME/.local/bin:$PATH"
  command -v unity >/dev/null 2>&1
  unity --version
}

find_license() {
  local found=""
  while IFS= read -r -d '' f; do
    found="$f"
    break
  done < <(find "$HOME" -type f \( -name '*.ulf' -o -name '*.xml' \) -print0 2>/dev/null)

  if [ -z "$found" ]; then
    return 1
  fi

  mkdir -p "$PWD/unity-license"
  cp "$found" "$PWD/unity-license/Unity_Personal_License$(basename "$found" | sed 's/.*\.//').ulf" 2>/dev/null || cp "$found" "$PWD/unity-license/Unity_Personal_License"
  echo "License file found: $found"
}

install_cli

if [ "$MODE" = "--install-only" ]; then
  echo "Codespace ready. Run this script again without --install-only to activate Unity Personal."
  exit 0
fi

echo
echo "Step 1/2: Sign in to Unity."
echo "If Unity opens a device/browser login flow, complete it in your browser."
unity auth login

echo
echo "Step 2/2: Activate Unity Personal."
unity license activate --personal --accept-eula

echo
echo "License status:"
unity license status --format json || true

echo
echo "Searching for the generated license file..."
if ! find_license; then
  echo
  echo "Activation succeeded, but no .ulf/.xml file was found under HOME."
  echo "Run: unity license status --format json"
  echo "Then inspect the Unity licensing files under HOME."
  exit 2
fi

echo
echo "Done. The license file is in ./unity-license/"
echo "Do NOT commit it to Git."
