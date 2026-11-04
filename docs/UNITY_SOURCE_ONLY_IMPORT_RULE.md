# Unity Source-Only Import Rule

## IMPORTANT — DO NOT UPLOAD THE LARGE PROJECT ARCHIVE OR ASSET FILES

The supplied `CorruptStaterp.zip` is a reference source archive only. Do not commit or upload its large binary assets, city packs, textures, models, audio, videos, Unity Library/Temp/Obj folders, APK/AAB files, or other files that exceed GitHub's 25 MB browser upload limit.

## Allowed workflow

1. Inspect the archive and identify source code required by the current Unity project.
2. Copy only the necessary source-code text into the matching existing files under `Unity/Assets/Scripts/` (or create small, clearly named source files when needed).
3. Preserve the current project's structure and Unity 6 compatibility.
4. Do not replace working project files blindly. Compare before copying and retain existing functionality.
5. Do not add large assets to GitHub. If a source file references an asset that is not present in the repository, document its exact expected path and report that the asset itself is still required separately.
6. After source changes, run the existing GitHub Actions Android build and inspect logs. A successful build alone does not prove that missing 3D assets are present.

## Current known issue

The current `WorldBuilder.cs` loads prefabs from `Resources/Generated3D/`, but the visible repository tree currently shows only `Unity/Assets/Resources/Data/`. Do not claim the city asset prefabs are available until their actual files are verified.

## Required reporting

Before importing anything, list the exact source files selected from the archive and why each is needed. Report any required large assets separately; never upload them as part of this source-only import.
