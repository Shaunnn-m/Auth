using Authentication.Application.Common;

namespace Authentication.Application.Errors
{
    public static class AuthenticationErrors
    {
        public static readonly Error InvalidCredentials =
            new(
                "Authentication.InvalidCredentials",
                "The email or password is incorrect.",
                401);

        public static readonly Error InvalidToken =
            new(
                "Authentication.InvalidToken",
                "The supplied token is invalid.",
                401);


        public static readonly Error TokenExpired =
            new(
                "Authentication.TokenExpired",
                "The supplied token has expired.",
                401);
        
        public static readonly Error TokenRevoked =
            new(
                "Authentication.TokenRevoked",
                "The supplied token has been revoked.",
                401);
        
        public static readonly Error InvalidRefreshToken =
            new(
                "Authentication.InvalidRefreshToken",
                "The supplied refresh token is invalid.",
                401);
        
        public static readonly Error RefreshTokenExpired =
            new(
                "Authentication.RefreshTokenExpired",
                "The supplied refresh token has expired.",
                401);
        
        public static readonly Error SessionNotFound =
            new(
                "Authentication.SessionNotFound",
                "The session was not found.",
                404);

        public static readonly Error SessionAlreadyRevoked =
            new(
                "Authentication.SessionAlreadyRevoked",
                "The requested session has already been revoked.",
                404);

        public static readonly Error SessionReuseDetected =
            new(
                "Authentication.SessionReuseDetected",
                "A reuse of a revoked refresh token has been detected.",
                401);

    }
}
