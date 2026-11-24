# Database migration 002 — account and administration foundation

This migration is additive and depends on `001_initial.sql`.
It does not alter existing rows in `players`, `ledger`, `audit_log`, or `refresh_sessions`.

## Objects created
- `account_identities`: external provider identities linked to existing player UUIDs.
- `admin_accounts` and `admin_sessions`: separate owner/admin identities and sessions.
- `account_security_logs` and `account_recovery`: security events and hashed recovery tokens.
- `promo_codes` and `promo_redemptions`: promotional reward definitions and one redemption per player/code.

## Important
- No owner account is inserted by this migration. The first owner must be created through a one-time protected bootstrap flow after the admin API is implemented.
- Never put owner email/password or OAuth secrets in SQL, Git, or chat.
- This file has not been applied to the live database.
