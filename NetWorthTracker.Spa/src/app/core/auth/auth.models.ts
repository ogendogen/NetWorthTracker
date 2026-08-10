export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  expiresAt: string;
  userName: string;
}

export type AuthSession = LoginResponse
