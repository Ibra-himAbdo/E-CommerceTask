namespace E_CommerceTask.Shared.DTOs;

public record AuthResponseDto(
    string Token,
    string UserId,
    string Email,
    string Role);