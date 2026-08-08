import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { map, Observable } from 'rxjs';
import { API_BASE_URL } from '../../app.config';
import { AuthSession, LoginRequest, LoginResponse } from './auth.models';

const SESSION_STORAGE_KEY = 'net-worth-tracker.session';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = inject(API_BASE_URL);
  private readonly sessionState = signal<AuthSession | null>(this.readSession());

  readonly session = this.sessionState.asReadonly();
  readonly isAuthenticated = computed(() => this.isSessionValid(this.sessionState()));
  readonly userName = computed(() =>
    this.isAuthenticated() ? (this.sessionState()?.userName ?? null) : null,
  );

  login(credentials: LoginRequest): Observable<AuthSession> {
    return this.http.post<LoginResponse>(`${this.apiBaseUrl}/login`, credentials).pipe(
      map((response) => {
        const session: AuthSession = { ...response };
        this.writeSession(session);
        return session;
      }),
    );
  }

  logout(): void {
    this.sessionState.set(null);
    sessionStorage.removeItem(SESSION_STORAGE_KEY);
  }

  getAccessToken(): string | null {
    const session = this.sessionState();
    return this.isSessionValid(session) ? session.accessToken : null;
  }

  private readSession(): AuthSession | null {
    const rawSession = sessionStorage.getItem(SESSION_STORAGE_KEY);

    if (!rawSession) {
      return null;
    }

    try {
      const session: unknown = JSON.parse(rawSession);

      if (this.isAuthSession(session) && this.isSessionValid(session)) {
        return session;
      }
    } catch {
      // An invalid persisted session is equivalent to no session.
    }

    sessionStorage.removeItem(SESSION_STORAGE_KEY);
    return null;
  }

  private writeSession(session: AuthSession): void {
    this.sessionState.set(session);
    sessionStorage.setItem(SESSION_STORAGE_KEY, JSON.stringify(session));
  }

  private isAuthSession(value: unknown): value is AuthSession {
    if (typeof value !== 'object' || value === null) {
      return false;
    }

    const candidate = value as Partial<AuthSession>;
    return (
      typeof candidate.accessToken === 'string' &&
      typeof candidate.expiresAt === 'string' &&
      typeof candidate.userName === 'string'
    );
  }

  private isSessionValid(session: AuthSession | null): session is AuthSession {
    if (!session || Number.isNaN(Date.parse(session.expiresAt))) {
      return false;
    }

    const tokenExpiry = this.readTokenExpiry(session.accessToken);
    return (
      tokenExpiry !== null &&
      new Date(session.expiresAt).getTime() > Date.now() &&
      tokenExpiry > Date.now()
    );
  }

  private readTokenExpiry(accessToken: string): number | null {
    const payloadSegment = accessToken.split('.')[1];

    if (!payloadSegment) {
      return null;
    }

    try {
      const payload = JSON.parse(atob(payloadSegment.replace(/-/g, '+').replace(/_/g, '/'))) as {
        exp?: unknown;
      };
      return typeof payload.exp === 'number' && Number.isFinite(payload.exp)
        ? payload.exp * 1_000
        : null;
    } catch {
      return null;
    }
  }
}
