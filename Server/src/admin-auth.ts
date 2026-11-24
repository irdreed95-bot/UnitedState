import type { FastifyInstance } from "fastify";
import type { Pool, PoolClient } from "pg";
import crypto from "node:crypto";
import { promisify } from "node:util";

const scryptAsync = promisify(crypto.scrypt) as (
  password: string | Buffer,
  salt: string | Buffer,
  keylen: number
) => Promise<Buffer>;

const COOKIE_NAME = "__Host-cs_admin";
const SESSION_HOURS = 8;
const SESSION_MS = SESSION_HOURS * 60 * 60 * 1000;
const sha256 = (value: string) =>
  crypto.createHash("sha256").update(value).digest("hex");

function parseCookies(header = ""): Record<string, string> {
  const result: Record<string, string> = {};
  for (const part of header.split(";")) {
    const index = part.indexOf("=");
    if (index < 1) continue;
    const key = part.slice(0, index).trim();
    const value = part.slice(index + 1).trim();
    try {
      result[key] = decodeURIComponent(value);
    } catch {
      // Ignore malformed cookie values.
    }
  }
  return result;
}

function requireAllowedOrigin(req: any, reply: any): boolean {
  const configured = process.env.ADMIN_APP_ORIGIN;
  const origin = req.headers.origin;
  if (!configured) {
    reply.code(503).send({ error: "admin_origin_not_configured" });
    return false;
  }
  if (typeof origin !== "string" || origin !== configured) {
    reply.code(403).send({ error: "origin_not_allowed" });
    return false;
  }
  return true;
}

function setSessionCookie(reply: any, token: string) {
  reply.header(
    "Set-Cookie",
    `${COOKIE_NAME}=${encodeURIComponent(token)}; Path=/; HttpOnly; Secure; SameSite=Strict; Max-Age=${SESSION_HOURS * 60 * 60}`
  );
}

function clearSessionCookie(reply: any) {
  reply.header(
    "Set-Cookie",
    `${COOKIE_NAME}=; Path=/; HttpOnly; Secure; SameSite=Strict; Max-Age=0`
  );
}

async function createAdminSession(client: PoolClient, adminId: string) {
  const token = crypto.randomBytes(32).toString("base64url");
  await client.query(
    "INSERT INTO admin_sessions(token_hash, admin_id, expires_at) VALUES($1, $2, now() + interval '8 hours')",
    [sha256(token), adminId]
  );
  return token;
}

function passwordInput(value: unknown): value is string {
  return typeof value === "string" && value.length >= 12 && value.length <= 256;
}

async function hashPassword(password: string, salt: string) {
  return (await scryptAsync(password, salt, 64)).toString("hex");
}

async function verifyPassword(password: string, salt: string, expectedHex: string) {
  const expected = Buffer.from(expectedHex, "hex");
  const actual = Buffer.from(await hashPassword(password, salt), "hex");
  return expected.length === actual.length && crypto.timingSafeEqual(expected, actual);
}

const attempts = new Map<string, { count: number; resetAt: number }>();
function rateLimited(key: string): boolean {
  const now = Date.now();
  const current = attempts.get(key);
  if (!current || current.resetAt <= now) {
    attempts.set(key, { count: 1, resetAt: now + 15 * 60 * 1000 });
    return false;
  }
  current.count += 1;
  return current.count > 5;
}

async function findAdminFromCookie(pool: Pool, req: any) {
  const token = parseCookies(req.headers.cookie)[COOKIE_NAME];
  if (!token || token.length > 256) return null;
  const result = await pool.query(
    `SELECT a.id, a.email, a.role
       FROM admin_sessions s
       JOIN admin_accounts a ON a.id = s.admin_id
      WHERE s.token_hash = $1
        AND s.revoked_at IS NULL
        AND s.expires_at > now()
        AND a.is_active = true
      LIMIT 1`,
    [sha256(token)]
  );
  return result.rows[0] ?? null;
}

export async function registerAdminAuth(app: FastifyInstance, pool: Pool) {
  app.post("/admin/auth/bootstrap", async (req: any, reply: any) => {
    if (!requireAllowedOrigin(req, reply)) return;
    const ip = String(req.ip || "unknown");
    if (rateLimited(`bootstrap:${ip}`)) {
      return reply.code(429).send({ error: "too_many_attempts" });
    }

    const body = req.body as {
      email?: unknown;
      password?: unknown;
      bootstrapSecret?: unknown;
    };
    const configuredSecret = process.env.ADMIN_BOOTSTRAP_SECRET || "";
    if (configuredSecret.length < 32) {
      return reply.code(503).send({ error: "admin_bootstrap_not_configured" });
    }
    if (
      typeof body?.bootstrapSecret !== "string" ||
      body.bootstrapSecret.length !== configuredSecret.length ||
      !crypto.timingSafeEqual(
        Buffer.from(body.bootstrapSecret),
        Buffer.from(configuredSecret)
      )
    ) {
      return reply.code(401).send({ error: "invalid_bootstrap_credentials" });
    }

    const email =
      typeof body.email === "string" ? body.email.trim().toLowerCase() : "";
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email) || email.length > 254) {
      return reply.code(400).send({ error: "valid_email_required" });
    }
    if (!passwordInput(body.password)) {
      return reply.code(400).send({ error: "password_min_12_chars_required" });
    }

    const salt = crypto.randomBytes(16).toString("hex");
    const passwordHash = await hashPassword(body.password, salt);
    const client = await pool.connect();
    try {
      await client.query("BEGIN");
      await client.query("SELECT pg_advisory_xact_lock($1)", [8246112502]);
      const existing = await client.query(
        "SELECT id FROM admin_accounts WHERE role = 'owner' LIMIT 1"
      );
      if (existing.rowCount) {
        await client.query("ROLLBACK");
        return reply.code(409).send({ error: "owner_already_bootstrapped" });
      }
      const inserted = await client.query(
        `INSERT INTO admin_accounts
           (email, password_hash, password_salt, password_algorithm, role, email_verified_at)
         VALUES ($1, $2, $3, 'scrypt', 'owner', now())
         RETURNING id, email, role`,
        [email, passwordHash, salt]
      );
      const admin = inserted.rows[0];
      const token = await createAdminSession(client, admin.id);
      await client.query(
        "INSERT INTO account_security_logs(admin_id, action, details) VALUES($1, 'admin.owner_bootstrapped', $2::jsonb)",
        [admin.id, JSON.stringify({ email, ip })]
      );
      await client.query("COMMIT");
      setSessionCookie(reply, token);
      return reply.code(201).send({ admin });
    } catch (error: any) {
      await client.query("ROLLBACK").catch(() => undefined);
      if (error?.code === "23505") {
        return reply.code(409).send({ error: "admin_email_already_exists" });
      }
      throw error;
    } finally {
      client.release();
    }
  });

  app.post("/admin/auth/login", async (req: any, reply: any) => {
    if (!requireAllowedOrigin(req, reply)) return;
    const ip = String(req.ip || "unknown");
    if (rateLimited(`login:${ip}`)) {
      return reply.code(429).send({ error: "too_many_attempts" });
    }
    const body = req.body as { email?: unknown; password?: unknown };
    const email =
      typeof body?.email === "string" ? body.email.trim().toLowerCase() : "";
    if (!email || typeof body?.password !== "string" || body.password.length > 256) {
      return reply.code(400).send({ error: "credentials_required" });
    }

    const result = await pool.query(
      `SELECT id, email, password_hash, password_salt, password_algorithm, role
         FROM admin_accounts
        WHERE lower(email) = $1 AND is_active = true
        LIMIT 1`,
      [email]
    );
    if (
      !result.rowCount ||
      result.rows[0].password_algorithm !== "scrypt" ||
      !(await verifyPassword(
        body.password,
        result.rows[0].password_salt,
        result.rows[0].password_hash
      ))
    ) {
      return reply.code(401).send({ error: "invalid_credentials" });
    }

    const admin = result.rows[0];
    const client = await pool.connect();
    try {
      await client.query("BEGIN");
      const token = await createAdminSession(client, admin.id);
      await client.query(
        "UPDATE admin_accounts SET last_login_at = now(), updated_at = now() WHERE id = $1",
        [admin.id]
      );
      await client.query(
        "INSERT INTO account_security_logs(admin_id, action, details) VALUES($1, 'admin.login', $2::jsonb)",
        [admin.id, JSON.stringify({ ip })]
      );
      await client.query("COMMIT");
      setSessionCookie(reply, token);
      return { admin: { id: admin.id, email: admin.email, role: admin.role } };
    } catch (error) {
      await client.query("ROLLBACK").catch(() => undefined);
      throw error;
    } finally {
      client.release();
    }
  });

  app.get("/admin/auth/me", async (req: any, reply: any) => {
    const admin = await findAdminFromCookie(pool, req);
    if (!admin) return reply.code(401).send({ error: "unauthorized" });
    return { admin };
  });

  app.post("/admin/auth/logout", async (req: any, reply: any) => {
    if (!requireAllowedOrigin(req, reply)) return;
    const token = parseCookies(req.headers.cookie)[COOKIE_NAME];
    if (token) {
      await pool.query(
        "UPDATE admin_sessions SET revoked_at = now() WHERE token_hash = $1 AND revoked_at IS NULL",
        [sha256(token)]
      );
    }
    clearSessionCookie(reply);
    return { ok: true };
  });
}
