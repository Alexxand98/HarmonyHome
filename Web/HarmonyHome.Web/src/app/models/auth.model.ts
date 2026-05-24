export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthUser {
  id: string;
  userName: string;
  email: string;
  role: string;
}

export interface LoginResponse {
  user: AuthUser;
  token: string;
}