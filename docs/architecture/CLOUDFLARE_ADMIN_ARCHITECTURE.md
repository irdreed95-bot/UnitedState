# CORRUPT STATE RP — Cloudflare Administration Architecture

Status: approved architecture baseline  
Branch: feat/cloudflare-admin-architecture  
Scope: design and additive implementation plan only. This document does not change production code, database data, secrets, or the running server.

## Approved decisions

- Cloudflare is the hosting and edge platform for the administration website and its public-facing web/API layer.
- Player sign-in methods: email/password, Google Play Games, Google, Discord, Facebook, and Apple.
- The game owner has the full owner permission set.
- Existing PostgreSQL database, existing player IDs, existing credentials, current Node/Fastify server, and current game API remain the source of truth during migration.

## System boundaries

1. **Admin web application — Cloudflare Workers + Static Assets**
   - Responsive Arabic RTL-first owner dashboard, with English support later.
   - Served over HTTPS from a Cloudflare Worker.
   - Admin-only routes are protected by application authentication and authorization; Cloudflare Access may be added as an additional owner-only perimeter, not as a substitute for application RBAC.
2. **Admin API / control plane — Cloudflare Worker**
   - Validates owner sessions, CSRF/origin rules, permissions, request schemas, idempotency keys, and audit metadata.
   - Proxies only explicit, allowlisted operations to the game origin through a Cloudflare Tunnel.
   - Never accepts arbitrary shell commands or arbitrary SQL from the browser.
3. **Current game backend — existing Node.js + Fastify**
   - Remains the authoritative game API and owns all player/economy writes.
   - Initially remains on the current Termux/PRoot host at port 8081.
   - A remotely managed Cloudflare Tunnel may expose only the required HTTPS API hostname. PostgreSQL must not be published through the tunnel.
4. **Current database — existing PostgreSQL**
   - Remains the only production database; do not create a replacement database or migrate player data to D1.
   - PostgreSQL remains bound to 127.0.0.1:5432.
   - The existing players, ledger, audit_log, admin_roles, refresh_sessions, and mission_completions tables are retained.
5. **Operational agent**
   - A small local service, separate from the public web Worker, will later perform a strict allowlist of server lifecycle actions.
   - It will not expose a shell, generic process execution, or unrestricted database access.
   - Start/stop/restart is unavailable until a reliable supervisor and a persistent host are configured. A phone/Termux process is not a 24/7 production host.

## Request path

Browser (HTTPS) -> Cloudflare Worker + Static Assets -> Admin API Worker -> authenticated HTTPS origin through Cloudflare Tunnel -> existing Fastify service -> existing PostgreSQL.

The game client continues to call the game API. Admin traffic is isolated from player-facing endpoints. PostgreSQL is never directly reachable from the public Internet.

## Identity and account-linking model

A stable player UUID remains the primary account identity. External provider identities are linked to that UUID; provider IDs are not used as the player primary key.

Planned additive tables (names are provisional until migration review):
- account_identities: player_id, provider, provider_subject, verified_email, created_at, last_used_at; unique(provider, provider_subject).
- account_security_events: actor/player, event_type, safe metadata, timestamp; never store passwords or provider tokens.
- account_link_challenges: short-lived, single-use, hashed challenge/nonce, target provider, expiry, consumed_at.
- email_verification_tokens: hashed one-time tokens, expiry, consumed_at.
- account_recovery_requests: secure recovery state and audit trail.
- admin_sessions or equivalent revocable owner sessions, if not delegated to a managed identity provider.

Compatibility rules:
- Do not drop or rename players.email, players.phone, players.password_hash, players.password_salt, or refresh_sessions in the first migration.
- Backfill existing email/phone identities additively and preserve the current player's UUID.
- Keep existing email/password login working during rollout; dual-read/dual-write behavior must be tested before any later deprecation.
- Passwords remain salted, slow hashes; plaintext passwords are never stored.
- Provider access tokens and client secrets are never stored as plaintext in database rows or source control.
- Linking requires a live authenticated session, recent re-authentication, provider proof, unique provider subject, and a transaction.
- Unlinking is rejected if it would leave the player without a usable sign-in/recovery method.
- Never merge two player UUIDs automatically because their email addresses match.
- Google Play Games is a distinct game identity integration and must use the platform's server-side authentication/verification flow; it is not interchangeable with Google Sign-In.
- Apple sign-in is Sign in with Apple, not App Store login.

## Provider integration boundaries

- Email/password: retain the existing backend credential format first; add verified-email flow and throttling before expanding.
- Google: OpenID Connect authorization-code flow with state, nonce, PKCE where supported, and server-side token validation.
- Discord: OAuth2 authorization-code flow and server-side identity verification.
- Facebook: Facebook Login OAuth flow and server-side token/identity validation.
- Apple: Sign in with Apple, including nonce/state and signed identity-token verification.
- Google Play Games: Android Play Games Services SDK obtains the server authentication proof; backend verifies it with Google's supported server-side API before linking the stable player ID.
- Provider secrets belong in Cloudflare Worker secrets or a trusted origin secret store, never in Git.

## Owner permissions

The owner role receives the full permission set across:
- player lookup and account recovery/link management;
- moderation and player support;
- economy, ledger review, rewards, vehicles/items/cosmetics;
- promo-code creation, scheduling, limits, revocation, and redemption analytics;
- factions, jobs, ranks, and configuration;
- database health, controlled backup/restore workflows, and audited maintenance;
- server status and allowlisted lifecycle actions;
- staff roles, permission grants, and audit logs.

Full owner authority does not mean unsafe execution primitives. Every high-impact action must still require clear confirmation, show its target and consequences, use an idempotency key where appropriate, and write an immutable audit event. No arbitrary SQL or shell endpoint is exposed in the dashboard. Database maintenance uses named, reviewed operations and tested backup/restore procedures.

The initial owner must be provisioned explicitly through a secure one-time bootstrap procedure. Do not grant owner based on a client-supplied email, a public registration, or an unverified provider claim.

## Server lifecycle controls

- First deliver read-only health/status and recent logs.
- Then add a local supervisor adapter supporting only status/start/stop/restart for the named game service.
- Require an owner session, step-up authentication, reason, explicit confirmation, and audit event.
- Return operation IDs and poll status; never report success before the local supervisor confirms it.
- Do not implement arbitrary command, path, PID, or environment-variable controls.
- Termux/PRoot limitations and Android background restrictions must be tested. Production 24/7 availability requires a persistent host; Cloudflare Tunnel does not keep a sleeping phone alive.

## Promo-code model

Promo codes are backend-authoritative and redeemed transactionally. Planned fields include normalized code/hash, reward definition, start/end time, global/per-player limits, eligibility, active/revoked state, creator, and audit metadata. Redemption records enforce one-time and configured usage limits. Reward payloads use a strict allowlist (cash, bank, XP, approved vehicle/item/cosmetic/character identifiers); never accept arbitrary client-provided reward amounts or asset IDs. Every grant writes ledger and audit records.

## Security baseline

- HTTPS/WSS only at public boundaries.
- Cloudflare Tunnel for origin connectivity; do not expose PostgreSQL.
- Separate admin authentication from player authentication.
- Owner-only authorization checked server-side on every request.
- Short-lived access sessions, rotating/revocable refresh sessions, secure cookies where browser sessions are used, CSRF protections, strict CORS/origin policy, rate limits, and login abuse controls.
- Validate every request with schemas and enforce idempotency for money/reward/lifecycle mutations.
- Redact authorization headers, cookies, query tokens, provider codes, and secrets from logs.
- No secrets in GitHub, client builds, or browser JavaScript.
- Backup and restore are not considered complete until a restore test is performed.

## Explicit non-goals for the first deployment

- Do not move the existing PostgreSQL database to D1.
- Do not rewrite the game backend into a Worker.
- Do not expose raw database access to the dashboard.
- Do not stop or restart the current running server during architecture setup.
- Do not run schema migrations against the live database until a reviewed backup and migration plan are ready.
