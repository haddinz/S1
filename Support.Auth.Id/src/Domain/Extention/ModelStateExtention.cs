using Microsoft.AspNetCore.Mvc.ModelBinding;
using Support.Auth.Id.Models.DTOs.Response;

namespace Support.Auth.Id.Extention;

public class ModelStateExtention
{
    public static ApiResponse<Dictionary<string, string[]>> ModelStateResponse(ModelStateDictionary modelState)
    {
        Dictionary<string, string[]> cleanErros = modelState
            .Where(ms => ms.Value!.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
            );
        
        return ApiResponse.Failure(cleanErros);
    }
}