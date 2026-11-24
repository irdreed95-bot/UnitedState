# CORRUPT STATE RP — Phased Implementation Roadmap

This roadmap is additive and designed to preserve the current live server and PostgreSQL data.

## Phase 0 — Baseline and safety (current)
- [x] Record the approved hosting, sign-in, and owner-permission decisions.
- [x] Create an isolated feature branch for architecture work.
- [x] Document system boundaries and migration safety rules.
- [ ] Capture a fresh schema-only backup and a data backup from the existing PostgreSQL instance.
- [ ] Record current Git commit, server health, and row counts for protected tables.
- Exit gate: backups exist, are stored outside the repo, and can be listed; no production changes have been made.

## Phase 1 — Cloudflare admin shell (read-only)
- [ ] Build Arabic RTL-first responsive admin UI using Cloudflare Workers + Static Assets.
- [ ] Add a login gate for the owner; initially use a protected preview/staging environment.
- [ ] Add an Admin API Worker with strict schemas, origin checks, rate limiting, and audit correlation IDs.
- [ ] Add read-only health/status integration through a protected origin route.
- [ ] Configure Cloudflare Tunnel to the existing Fastify API only after the origin access and authentication design is reviewed.
- Exit gate: owner can sign in to staging and view health without any database mutation or lifecycle operation.

## Phase 2 — Admin identity and permissions
- [ ] Define owner/admin/auditor permission matrix; owner receives full permissions.
- [ ] Securely bootstrap the owner; never auto-promote based on email.
- [ ] Add revocable admin sessions, step-up verification for sensitive operations, and immutable audit events.
- [ ] Test denied access for anonymous players and non-owner accounts.
- Exit gate: all admin endpoints enforce server-side permissions.

## Phase 3 — Player identity foundation
- [ ] Review the live schema and prepare an additive migration only.
- [ ] Add account identity/link/challenge/security-event tables.
- [ ] Backfill existing email/phone identities while preserving every player UUID.
- [ ] Keep current email/password login compatible.
- [ ] Add provider integration one at a time: Google, Discord, Facebook, Apple, then Google Play Games.
- [ ] Add account linking/unlinking with re-authentication, ownership proof, collision prevention, and recovery safeguards.
- Exit gate: existing login still works; every provider identity maps to exactly one player; linking cannot silently merge accounts.

## Phase 4 — Player and moderation management
- [ ] Search players by stable UUID, verified email, or provider identity with privacy controls.
- [ ] Add support actions and moderation endpoints with reason codes and audit logs.
- [ ] Implement account recovery and identity relinking as a verified support workflow.
- Exit gate: all player changes are server-authoritative and auditable.

## Phase 5 — Economy and promo codes
- [ ] Add a versioned reward catalogue and approved asset identifiers.
- [ ] Add promo-code creation, scheduling, expiry, limits, targeting, revocation, and redemption history.
- [ ] Redeem in a single PostgreSQL transaction with ledger/audit entries and idempotency.
- [ ] Add test cases for duplicate redemption, race conditions, expired codes, and invalid reward definitions.
- Exit gate: no duplicate or unauthorized reward can be granted.

## Phase 6 — Server operations
- [ ] Read-only status and bounded logs first.
- [ ] Add a local supervisor adapter with only named service actions.
- [ ] Add restart/stop/start confirmations and operation status polling.
- [ ] Test recovery after tunnel loss and host reboot.
- Exit gate: no arbitrary command execution; lifecycle actions report verified results.

## Phase 7 — Production readiness
- [ ] Move the game server to a persistent host if 24/7 availability is required; keep PostgreSQL data with a controlled backup/restore migration.
- [ ] Configure production domain, HTTPS, Cloudflare Access/WAF/rate limits as appropriate.
- [ ] Load-test API and WebSocket paths; do not claim a player capacity before measurement.
- [ ] Perform restore drill, account takeover tests, provider unlink/relink tests, and promo-code concurrency tests.
- Exit gate: documented recovery procedure, monitored production service, and successful security/load tests.

## Protected production invariants
- Existing database name, tables, player UUIDs, balances, ledger history, and current guest account are preserved.
- PostgreSQL stays private on 127.0.0.1:5432.
- Existing Fastify server on port 8081 is not stopped as part of documentation or UI scaffolding.
- No secret values are committed.
- No destructive migration, account merge, reward grant, or server lifecycle operation occurs without explicit review and verification.
