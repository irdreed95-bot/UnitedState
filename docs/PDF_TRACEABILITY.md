# PDF traceability
Status: Implemented / Partial / Blocked / Deferred / Not tested.
| Area | Status | Location |
|---|---|---|
| Unity Android pipeline | Implemented/CI | .github/workflows/unity-android.yml |
| Legacy Godot/Unreal workflows | Removed in this batch | .github/workflows |
| License secret policy | Implemented | .gitignore + GitHub Secrets |
| Data-driven jobs/factions/streets foundation | Implemented | Unity/Assets/Data |
| PostgreSQL economy ledger | Implemented foundation | Server/sql |
| Server-authoritative economy transaction | Implemented foundation | Server/src/server.ts |
| Email/password auth | Partial | Server + Unity AuthClient |
| Facebook/phone providers | Blocked until provider credentials | docs/DEPLOY.md |
| Chat | Deferred | online phase |
| Proximity/radio voice | Blocked until licensed provider | docs/DEPLOY.md |
| Full PDF city | Partial | Unity/Assets/Scripts/WorldBuilder.cs |
| Weapons/vehicles/NPC | Partial | Unity/Assets/Scripts |
| Admin RBAC/audit | Partial | Server + Unity Admin UI |
| Performance real-device verification | Not tested | docs/PERF.md |
| Release APK/AAB/store compliance | Not tested | docs/PLAY_COMPLIANCE.md |
