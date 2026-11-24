-- CORRUPT STATE RP: additive account/admin foundation.
-- Safe to apply after 001_initial.sql. Does not modify or delete existing players.
BEGIN;

CREATE TABLE IF NOT EXISTS account_identities (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  player_id uuid NOT NULL REFERENCES players(id) ON DELETE CASCADE,
  provider text NOT NULL CHECK (provider IN ('email','googleplay','google','discord','facebook','apple')),
  provider_subject text NOT NULL,
  email text,
  email_verified boolean NOT NULL DEFAULT false,
  created_at timestamptz NOT NULL DEFAULT now(),
  updated_at timestamptz NOT NULL DEFAULT now(),
  CONSTRAINT account_identities_provider_subject_key UNIQUE (provider, provider_subject),
  CONSTRAINT account_identities_player_provider_key UNIQUE (player_id, provider)
);
CREATE INDEX IF NOT EXISTS account_identities_player_idx ON account_identities(player_id);

-- Separate administrative identities: owner accounts are not game players.
CREATE TABLE IF NOT EXISTS admin_accounts (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  email text NOT NULL,
  password_hash text NOT NULL,
  password_salt text NOT NULL,
  password_algorithm text NOT NULL DEFAULT 'scrypt',
  role text NOT NULL DEFAULT 'admin' CHECK (role IN ('owner','admin','auditor')),
  is_active boolean NOT NULL DEFAULT true,
  email_verified_at timestamptz,
  last_login_at timestamptz,
  created_at timestamptz NOT NULL DEFAULT now(),
  updated_at timestamptz NOT NULL DEFAULT now()
);
CREATE UNIQUE INDEX IF NOT EXISTS admin_accounts_email_lower_key ON admin_accounts (lower(email));

CREATE TABLE IF NOT EXISTS admin_sessions (
  token_hash text PRIMARY KEY,
  admin_id uuid NOT NULL REFERENCES admin_accounts(id) ON DELETE CASCADE,
  expires_at timestamptz NOT NULL,
  revoked_at timestamptz,
  created_at timestamptz NOT NULL DEFAULT now()
);
CREATE INDEX IF NOT EXISTS admin_sessions_admin_expiry_idx ON admin_sessions(admin_id, expires_at DESC);

CREATE TABLE IF NOT EXISTS account_security_logs (
  id bigserial PRIMARY KEY,
  player_id uuid REFERENCES players(id) ON DELETE SET NULL,
  admin_id uuid REFERENCES admin_accounts(id) ON DELETE SET NULL,
  action text NOT NULL,
  provider text,
  details jsonb NOT NULL DEFAULT '{}'::jsonb,
  created_at timestamptz NOT NULL DEFAULT now()
);
CREATE INDEX IF NOT EXISTS account_security_logs_player_created_idx ON account_security_logs(player_id, created_at DESC);

CREATE TABLE IF NOT EXISTS account_recovery (
  token_hash text PRIMARY KEY,
  player_id uuid NOT NULL REFERENCES players(id) ON DELETE CASCADE,
  expires_at timestamptz NOT NULL,
  used_at timestamptz,
  created_at timestamptz NOT NULL DEFAULT now()
);
CREATE INDEX IF NOT EXISTS account_recovery_player_idx ON account_recovery(player_id, expires_at DESC);

CREATE TABLE IF NOT EXISTS promo_codes (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  code text NOT NULL UNIQUE,
  reward_type text NOT NULL CHECK (reward_type IN ('cash','bank','xp','vehicle','clothing','character','item','custom')),
  reward_payload jsonb NOT NULL DEFAULT '{}'::jsonb,
  starts_at timestamptz NOT NULL DEFAULT now(),
  expires_at timestamptz,
  max_redemptions integer CHECK (max_redemptions IS NULL OR max_redemptions > 0),
  per_player_limit integer NOT NULL DEFAULT 1 CHECK (per_player_limit > 0),
  is_active boolean NOT NULL DEFAULT true,
  created_by uuid REFERENCES admin_accounts(id) ON DELETE SET NULL,
  created_at timestamptz NOT NULL DEFAULT now()
);
CREATE INDEX IF NOT EXISTS promo_codes_active_window_idx ON promo_codes(is_active, starts_at, expires_at);

CREATE TABLE IF NOT EXISTS promo_redemptions (
  id bigserial PRIMARY KEY,
  promo_code_id uuid NOT NULL REFERENCES promo_codes(id) ON DELETE RESTRICT,
  player_id uuid NOT NULL REFERENCES players(id) ON DELETE RESTRICT,
  reward_snapshot jsonb NOT NULL,
  redeemed_at timestamptz NOT NULL DEFAULT now()
);
-- Multiple redemptions per player are supported up to promo_codes.per_player_limit.
-- The redemption API MUST lock the promo_codes row (SELECT ... FOR UPDATE),
-- count existing redemptions for this player, enforce both limits, and insert
-- the redemption + reward grant in the same transaction.
CREATE INDEX IF NOT EXISTS promo_redemptions_code_player_idx ON promo_redemptions(promo_code_id, player_id, redeemed_at DESC);
CREATE INDEX IF NOT EXISTS promo_redemptions_player_idx ON promo_redemptions(player_id, redeemed_at DESC);

COMMIT;
