export interface UsuarioRegistro {
  nombre: string;
  apellido: string;
  email: string;
  contrasena: string;
  rolId: number;
}

export interface UsuarioResponse {
  id: number;
  nombre: string;
  apellido: string;
  email: string;
  rolId: number;
  rolNombre?: string;
}

export interface UsuarioLogin {
  email: string;
  contrasena: string;
}

export interface LoginResponse {
  token: string;
  usuario: UsuarioResponse;
}