using Support.Auth.Id.Models.DTOs;

namespace Support.Auth.Id.Services.Interfaces;

public interface ISecureTokenGenerator<T> where T : class
{
    T Generate();
}
