﻿namespace ListaComprasAPI.DTOs {
    public class UsuarioCreateDto {

        public string Nome { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Senha { get; set; } = null!; //Hash em UsuariosController.cs
    }
}
