using System;

namespace QLine.Application.DTO
{
    public sealed class StaffDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
    }
}
