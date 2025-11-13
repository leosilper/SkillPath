using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SkillShiftHub.Api.Swagger;

public class AuthorizeCheckOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var methodInfo = context.MethodInfo;
        var controllerType = methodInfo.DeclaringType;

        if (controllerType == null)
            return;

        // Verifica se o método ou a classe tem [Authorize] e não tem [AllowAnonymous]
        var hasAuthorizeOnMethod = methodInfo.GetCustomAttributes<AuthorizeAttribute>(true).Any();
        var hasAuthorizeOnController = controllerType.GetCustomAttributes<AuthorizeAttribute>(true).Any();
        var hasAllowAnonymous = methodInfo.GetCustomAttributes<AllowAnonymousAttribute>(true).Any();

        var requiresAuth = (hasAuthorizeOnMethod || hasAuthorizeOnController) && !hasAllowAnonymous;

        if (requiresAuth)
        {
            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        }
                    ] = Array.Empty<string>()
                }
            };
        }
    }
}

