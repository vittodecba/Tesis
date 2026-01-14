namespace AtonBeerTesis.Dtos
{
    // 1. Para mostrar en la lista (lo que ve el usuario)
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public string RolNombre { get; set; } = string.Empty; // Para mostrar "Supervisor" en vez de un número
    }

    // 2. Para CREAR uno nuevo
    public class UsuarioCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RolId { get; set; } // El ID del rol que elegimos en el desplegable
    }

    // 3. Para EDITAR uno existente
    public class UsuarioUpdateDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int RolId { get; set; }
        public bool Activo { get; set; }
    }
}