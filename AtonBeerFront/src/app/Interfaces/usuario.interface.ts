export interface Usuario {
  id: number;
  nombre: string;
  apellido: string;
  email: string;
  activo: boolean;
  rolNombre: string;
  rolId: number; 
}

export interface UsuarioCreate {
  nombre: string;
  apellido: string;
  email: string;
  password: string;
  confirmarPassword?: string;
  rolId: number;
}

export interface UsuarioUpdate {
  id: number;
  nombre: string;
  apellido: string;
  email: string;
  rolId: number;
  activo: boolean;
}