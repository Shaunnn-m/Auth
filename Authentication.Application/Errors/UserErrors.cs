using Authentication.Application.Common;

namespace Authentication.Application.Errors
{
    public static class UserErrors
    {
        public static readonly Error EmailAlreadyExists =
            new(
                "User.EmailAlreadyExists",
                "A user with this email already exists.",
                409);

        public static readonly Error UserNotFound =
            new(
                "User.NotFound",
                "The requested user could not be found.",
                404);

        public static readonly Error UserInactive =
            new(
                "User.Inactive",
                "The user account has not been activated.",
                403);

        public static readonly Error UserAlreadyActive =
            new(
                "User.AlreadyActive",
                "The user account is already active.",
                409);


        public static readonly Error UserNotAuthenticated =
            new(
                "User.NotAuthenticated",
                "The user is not authenticated.",
                401);

        public static readonly Error InvalidCurrentPassword =
           new(
               "User.IncorrectPassword",
               "The provided current password is incorrect",
                400);
    }
}
