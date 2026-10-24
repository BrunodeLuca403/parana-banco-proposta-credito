using System;
using System.Collections.Generic;
using System.Text;

namespace PropostaCredito.Application.Common
{
    public record Error
    {
        public static Error None => new(string.Empty, string.Empty, ErrorTypeEnum.Failure);

        private Error(string code, string description, ErrorTypeEnum type, string? details = null)
        {
            Code = code;
            Description = description ?? string.Empty;
            Type = type;
            Details = details;
        }

        public string Code { get; init; }
        public string Description { get; init; }
        public ErrorTypeEnum Type { get; init; }
        public string? Details { get; init; }

        public static Error Validation(string code, string description, string? details = null) =>
            new(code, description, ErrorTypeEnum.Validation, details);

        public static Error NotFound(string code, string description, string? details = null) =>
            new(code, description, ErrorTypeEnum.NotFound, details);

        public static Error Conflict(string code, string description, string? details = null) =>
            new(code, description, ErrorTypeEnum.Conflict, details);

        public static Error Unauthorized(string code, string description, string? details = null) =>
            new(code, description, ErrorTypeEnum.Unauthorized, details);

        public static Error Failure(string code, string description, string? details = null) =>
            new(code, description, ErrorTypeEnum.Failure, details);
    }
}
