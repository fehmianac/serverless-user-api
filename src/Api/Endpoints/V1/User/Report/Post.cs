using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.User.Report;

public class Post : IEndpoint
{
    private static async Task<IResult> Handler([FromRoute] string id,
        [FromBody] UserReportRequest request,
        [FromServices] IApiContext apiContext,
        [FromServices] IUserRepository userRepository,
        [FromServices] IUserReportRepository userReportRepository,
        CancellationToken cancellationToken)
    {
        var reportedUser = await userRepository.GetAsync(id, cancellationToken);
        if (reportedUser == null)
            return Results.NotFound();

        await userReportRepository.SaveAsync(apiContext.CurrentUserId, reportedUser.Id, request.Reason, cancellationToken);
        return Results.Ok();
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("v1/user/{id}/report", Handler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("User");
    }

    public record UserReportRequest(string? Reason);
}